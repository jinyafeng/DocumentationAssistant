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
    public class PropertyUnitTest : CodeFixVerifier
    {
        private const string TestCode = @"
using System;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            int i = 0;
            Console.WriteLine(i);
        }

        private string test { get; set; }
    }
}";

        private const string TestFixCode = @"
using System;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            int i = 0;
            Console.WriteLine(i);
        }

        /// <summary>
        /// The property comment.
        /// </summary>
        private string test { get; set; }
    }
}";

        [DataTestMethod]
        [DataRow("")]
        public void NoDiagnosticsShow(string testCode)
        {
            VerifyCSharpDiagnostic(testCode);
        }

        [DataTestMethod]
        [DataRow(TestCode,TestFixCode,14,24)]
        public void ShowDiagnosticAndFix(string testCode,string fixCode,int line,int column)
        {
            var expected = new DiagnosticResult
            {
                Id = PropertyAnalyzer.DiagnosticId,
                Message = PropertyAnalyzer.MessageFormat,
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
            return new PropertyCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new PropertyAnalyzer();
        }
    }
}
