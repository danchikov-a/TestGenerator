﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace TestGeneratorLibrary.Services
{
    public class ClassService
    {
        public ClassDeclarationSyntax GenerateTestsClass(ClassDeclarationSyntax classDeclaration)
        {
            return ClassDeclaration(classDeclaration.Identifier.Text + "Tests")
                .AddModifiers(Token(SyntaxKind.PublicKeyword))
                .WithMembers(
                    new SyntaxList<MemberDeclarationSyntax>(
                        GenerateTestMethods(GenerateMethodsCounts(classDeclaration))))
                .WithAttributeLists(
                    SingletonList(AttributeList(SingletonSeparatedList(Attribute(IdentifierName("TestFixture"))))));
        }

        private Dictionary<string, int> GenerateMethodsCounts(ClassDeclarationSyntax classDeclaration)
        {
            var methods =
                classDeclaration.DescendantNodes().OfType<MethodDeclarationSyntax>()
                    .Where(method => method.Modifiers.Any(token => token.Kind() == SyntaxKind.PublicKeyword))
                    .ToList();

            var methodsCounts = new Dictionary<string, int>();

            foreach (var method in methods)
            {
                if (methodsCounts.TryGetValue(method.Identifier.Text, out var methodCount))
                {
                    methodsCounts[method.Identifier.Text] = ++methodCount;
                }
                else
                {
                    methodsCounts.Add(method.Identifier.Text, 1);
                }
            }

            return methodsCounts;
        }

        private List<MemberDeclarationSyntax> GenerateTestMethods(Dictionary<string, int> methods)
        {
            var testMethods = new List<MemberDeclarationSyntax>();
            var methodBody = 
                ExpressionStatement(InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                        IdentifierName("Assert"), IdentifierName("Fail")))
                    .WithArgumentList(ArgumentList(SingletonSeparatedList(
                        Argument(LiteralExpression(SyntaxKind.StringLiteralExpression,
                            Literal("autogenerated")))))));

            foreach (var (key, value) in methods)
            {
                if (value == 1)
                {
                    var methodName = key + "Test";
                    testMethods.Add(GenerateTestMethod(methodName, methodBody));
                }
                else
                {
                    for (var overloadedMethodNumber = 1; overloadedMethodNumber <= value; overloadedMethodNumber++)
                    {
                        var methodName = key + overloadedMethodNumber + "Test";

                        testMethods.Add(GenerateTestMethod(methodName, methodBody));
                    }
                }
            }

            return testMethods;
        }
        private static MethodDeclarationSyntax GenerateTestMethod(string methodName, StatementSyntax methodBody)
        {
            return MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)), Identifier(methodName))
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                .WithAttributeLists(
                    SingletonList(AttributeList(SingletonSeparatedList(Attribute(IdentifierName("Test"))))))
                .WithBody(Block(methodBody));
        }
    }
}