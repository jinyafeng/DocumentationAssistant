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
    public class MethodUnitTest : CodeFixVerifier
    {
        private const string TestCode = @"
using System;

namespace Test
{
    class Program
    {
        static void Main()
        {
            int i = 0;
            Console.WriteLine(i);
        }
    }
}";

        private const  string x = "\"";
        private const string TestFixCode = @"
using System;

namespace Test
{
    class Program
    {
        /// <summary>
        /// The method comment.
        /// </summary>
        static void Main()
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
        [DataRow(TestCode,TestFixCode,8,21)]
        public void ShowDiagnosticAndFix(string testCode,string fixCode,int line,int column)
        {
            var expected = new DiagnosticResult
            {
                Id = MethodAnalyzer.DiagnosticId,
                Message = MethodAnalyzer.MessageFormat,
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
            return new MethodCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new MethodAnalyzer();
        }
    }
}
