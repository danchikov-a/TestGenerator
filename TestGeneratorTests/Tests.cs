using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using TestGeneratorLibrary;
using TestGeneratorLibrary.Services;

namespace Tests
{
    [TestFixture]
    public class Tests
    {
        private const int AMOUNT_OF_TESTS = 1;
        private const int AMOUNT_OF_METHODS = 3;
        private const int FIRST_TEST = 1;
        private const string INPUT = @"
            namespace TestGeneratorTests.TestFiles
            {
                public class BooTestClass
                {
                    public void firstMethod()
                    {
                        
                    }

                    public int secondMethod()
                    {
                        return 5;
                    }

                    public double thirdMethod()
                    {
                        return 2.2;
                    }

                    private void privateMethod()
                    {
                        
                    }
                }
            }";

        private readonly TestGenerator _testGenerator = new(new NamespaceService(new ClassService()));
        private List<TestGeneratorLibrary.Models.FileInfo> _tests;
        
        [SetUp]
        public void setUp()
        {
            _tests = _testGenerator.Generate(INPUT);
        }
        
        [Test]
        public void TestAmountTest()
        {
            Assert.That(_tests, Has.Count.EqualTo(AMOUNT_OF_TESTS));
        }
        
        [Test]
        public void MethodAmountTest()
        {
            var methods = CSharpSyntaxTree
                .ParseText(_tests[FIRST_TEST].FileContent)
                .GetCompilationUnitRoot()
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .ToList();
            
            Assert.That(methods, Has.Count.EqualTo(AMOUNT_OF_METHODS));
        }
    }
}