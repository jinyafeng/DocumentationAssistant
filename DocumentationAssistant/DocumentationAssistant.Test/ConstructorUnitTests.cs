using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TestHelper;
using DocumentationAssistant;

namespace DocumentationAssistant.Test
{
    [TestClass]
    public class ConstrcutorUnitTest : CodeFixVerifier
    {
        private const string TestCode = @"
using System;

namespace Test
{
    class Program
    {
        public Program()
        {
        }

        static void Main(string[] args)
        {
            int i = 0;
            Console.WriteLine(i);
        }
    }
}";

        private const string TestFixCode = @"
using System;

namespace Test
{
    class Program
    {
        /// <summary>
        /// The constructor comment.
        /// </summary>
        public Program()
        {
        }

        static void Main(string[] args)
        {
            int i = 0;
            Console.WriteLine(i);
        }
    }
}";

        [DataTestMethod]
        [DataRow("")]
        public void NoDiagnosticsShow(string testCode)
        {
            VerifyCSharpDiagnostic(testCode);
        }

        [DataTestMethod]
        [DataRow(TestCode,TestFixCode,8,16)]
        public void ShowDiagnosticAndFix(string testCode,string fixCode,int line,int column)
        {
            var expected = new DiagnosticResult
            {
                Id = ConstructorAnalyzer.DiagnosticId,
                Message = ConstructorAnalyzer.MessageFormat,
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", line, column)
                        }
            };

            VerifyCSharpDiagnostic(testCode, expected);

            VerifyCSharpFix(testCode,fixCode);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new ConstructorCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new ConstructorAnalyzer();
        }
    }
}
