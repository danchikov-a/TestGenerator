using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TestGeneratorLibrary.Models;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace TestGeneratorLibrary.Services
{
    public class NamespaceService
    {
        private readonly ClassService _classService;

        public NamespaceService(ClassService classService)
        {
            _classService = classService;
        }

        public ClassInfo GenerateTestClassWithNamespaces(ClassDeclarationSyntax classDeclaration)
        {
            SyntaxNode currentTreeNode = classDeclaration;
            var testClassDeclaration = _classService.GenerateTestsClass(classDeclaration);
            var testClassDeclarationBuilder = new StringBuilder(testClassDeclaration.Identifier.Text);

            var parent = (NamespaceDeclarationSyntax) currentTreeNode.Parent;
            var ns = NamespaceDeclaration(IdentifierName(parent.Name + ".Tests"))
                .WithMembers(new SyntaxList<MemberDeclarationSyntax>(testClassDeclaration));

            SyntaxNode currentNamespace = ns;
            testClassDeclarationBuilder.Insert(0, $"{ns.Name}.");
            currentTreeNode = currentTreeNode.Parent;

            while (currentTreeNode.Parent is NamespaceDeclarationSyntax)
            {
                parent = (NamespaceDeclarationSyntax) currentTreeNode.Parent;
                ns = NamespaceDeclaration(parent.Name)
                    .WithMembers(
                        new SyntaxList<MemberDeclarationSyntax>((NamespaceDeclarationSyntax) currentNamespace));
                currentNamespace = ns;
                testClassDeclarationBuilder.Insert(0, $"{ns.Name}.");
                currentTreeNode = currentTreeNode.Parent;
            }

            return new ClassInfo(testClassDeclarationBuilder.ToString(),
                (MemberDeclarationSyntax) currentNamespace);
        }
    }
}