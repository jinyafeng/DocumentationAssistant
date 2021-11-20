using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace DocumentationAssistant.Test
{
	/// <summary>
	/// The enum unit test.
	/// </summary>
	[TestClass]
	public class EnumUnitTest : CodeFixVerifier
	{
		/// <summary>
		/// The test code.
		/// </summary>
		private const string TestCode = @"
using System;
using System.Collections.Generic;
using System.Text;

namespace Test
{
	enum EnumTester
	{
	}
}";

		/// <summary>
		/// The test fix code.
		/// </summary>
		private const string TestFixCode = @"
using System;
using System.Collections.Generic;
using System.Text;

namespace Test
{
    /// <summary>
    /// The enum tester.
    /// </summary>
    enum EnumTester
	{
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
			this.VerifyCSharpDiagnostic(testCode);
		}

		/// <summary>
		/// Shows diagnostic and fix.
		/// </summary>
		/// <param name="testCode">The test code.</param>
		/// <param name="fixCode">The fix code.</param>
		/// <param name="line">The line.</param>
		/// <param name="column">The column.</param>
		[DataTestMethod]
		[DataRow(TestCode, TestFixCode, 8, 7)]
		public void ShowDiagnosticAndFix(string testCode, string fixCode, int line, int column)
		{
			DiagnosticResult expected = new DiagnosticResult
			{
				Id = EnumAnalyzer.DiagnosticId,
				Message = EnumAnalyzer.MessageFormat,
				Severity = DiagnosticSeverity.Warning,
				Locations =
					new[] {
							new DiagnosticResultLocation("Test0.cs", line, column)
						}
			};

			this.VerifyCSharpDiagnostic(testCode, expected);

			this.VerifyCSharpFix(testCode, fixCode);
		}

		/// <summary>
		/// Gets c sharp code fix provider.
		/// </summary>
		/// <returns>A CodeFixProvider.</returns>
		protected override CodeFixProvider GetCSharpCodeFixProvider()
		{
			return new EnumCodeFixProvider();
		}

		/// <summary>
		/// Gets c sharp diagnostic analyzer.
		/// </summary>
		/// <returns>A DiagnosticAnalyzer.</returns>
		protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
		{
			return new EnumAnalyzer();
		}
	}
}
