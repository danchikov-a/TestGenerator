using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TestGeneratorLibrary.Services;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using FileInfo = TestGeneratorLibrary.Models.FileInfo;

namespace TestGeneratorLibrary
{
    public class TestGenerator
    {
        private readonly NamespaceService _namespaceService;

        public TestGenerator(NamespaceService namespaceService)
        {
            _namespaceService = namespaceService;
        }

        public IEnumerable<FileInfo> Generate(string source)
        {
            return CSharpSyntaxTree
                .ParseText(source)
                .GetCompilationUnitRoot()
                .DescendantNodes().OfType<ClassDeclarationSyntax>()
                .Where(node => node.Modifiers.Any(n => n.Kind() == SyntaxKind.PublicKeyword))
                .ToList()
                .Select(_namespaceService.GenerateTestClassWithNamespaces).ToList()
                .Select(classData => new FileInfo(classData.ClassName, CompilationUnit()
                    .WithUsings(new SyntaxList<UsingDirectiveSyntax>().Add(
                        UsingDirective(QualifiedName(IdentifierName("NUnit"), IdentifierName("Framework")))))
                    .AddMembers(classData.TestClassDeclarationSyntax)
                    .NormalizeWhitespace().ToFullString())).ToList();
        }
    }
}