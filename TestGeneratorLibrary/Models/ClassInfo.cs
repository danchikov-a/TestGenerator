using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TestGeneratorLibrary.Models
{
    public class ClassInfo
    {
        public string ClassName
        {
            get;
        }

        public MemberDeclarationSyntax TestClassDeclarationSyntax
        {
            get;
        }

        public ClassInfo(string className, MemberDeclarationSyntax testClassDeclarationSyntax)
        {
            ClassName = className;
            TestClassDeclarationSyntax = testClassDeclarationSyntax;
        }
    }
}