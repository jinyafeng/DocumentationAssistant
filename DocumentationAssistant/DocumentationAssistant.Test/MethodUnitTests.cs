using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace DocumentationAssistant.Test
{
	/// <summary>
	/// The method unit test.
	/// </summary>
	[TestClass]
	public class MethodUnitTest : CodeFixVerifier
	{
		/// <summary>
		/// The basic test code.
		/// </summary>
		private const string BasicTestCode = @"
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp4
{
	public class MethodTester
	{
		public void ShowBasicMethodTester()
		{
		}
	}
}";

		/// <summary>
		/// The basic test fix code.
		/// </summary>
		private const string BasicTestFixCode = @"
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp4
{
	public class MethodTester
	{
        /// <summary>
        /// Shows basic method tester.
        /// </summary>
        public void ShowBasicMethodTester()
		{
		}
	}
}";

		/// <summary>
		/// The method with parameter test code.
		/// </summary>
		private const string MethodWithParameterTestCode = @"
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp4
{
	public class MethodTester
	{
		public void ShowMethodWithParameterTester(string param1, int param2, bool param3)
		{
		}
	}
}";
		/// <summary>
		/// The method with parameter test fix code.
		/// </summary>
		private const string MethodWithParameterTestFixCode = @"
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp4
{
	public class MethodTester
	{
        /// <summary>
        /// Shows method with parameter tester.
        /// </summary>
        /// <param name=""param1"">The param1.</param>
        /// <param name=""param2"">The param2.</param>
        /// <param name=""param3"">The param3.</param>
        public void ShowMethodWithParameterTester(string param1, int param2, bool param3)
		{
		}
	}
}";

		/// <summary>
		/// The method with return test code.
		/// </summary>
		private const string MethodWithReturnTestCode = @"
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp4
{
	public class MethodTester
	{
		public MethodTester ShowMethodWithReturnTester()
		{
			return null;
		}
	}
}";

		/// <summary>
		/// The method with return test fix code.
		/// </summary>
		private const string MethodWithReturnTestFixCode = @"
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp4
{
	public class MethodTester
	{
        /// <summary>
        /// Shows method with return tester.
        /// </summary>
        /// <returns>A MethodTester.</returns>
        public MethodTester ShowMethodWithReturnTester()
		{
			return null;
		}
	}
}";

		/// <summary>
		/// Nos diagnostics show.
		/// </summary>
		/// <param name="testCode">The test code.</param>
		[DataTestMethod]
		[DataRow("")]
		public void NoDiagnosticsShow(string testCode)
		{
			VerifyCSharpDiagnostic(testCode);
		}

		/// <summary>
		/// Shows diagnostic and fix.
		/// </summary>
		/// <param name="testCode">The test code.</param>
		/// <param name="fixCode">The fix code.</param>
		/// <param name="line">The line.</param>
		/// <param name="column">The column.</param>
		[DataTestMethod]
		[DataRow(BasicTestCode, BasicTestFixCode, 10, 15)]
		[DataRow(MethodWithParameterTestCode, MethodWithParameterTestFixCode, 10, 15)]
		[DataRow(MethodWithReturnTestCode, MethodWithReturnTestFixCode, 10, 23)]
		public void ShowDiagnosticAndFix(string testCode, string fixCode, int line, int column)
		{
			DiagnosticResult expected = new DiagnosticResult
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

			VerifyCSharpFix(testCode, fixCode);
		}

		/// <summary>
		/// Gets c sharp code fix provider.
		/// </summary>
		/// <returns>A CodeFixProvider.</returns>
		protected override CodeFixProvider GetCSharpCodeFixProvider()
		{
			return new MethodCodeFixProvider();
		}

		/// <summary>
		/// Gets c sharp diagnostic analyzer.
		/// </summary>
		/// <returns>A DiagnosticAnalyzer.</returns>
		protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
		{
			return new MethodAnalyzer();
		}
	}
}
