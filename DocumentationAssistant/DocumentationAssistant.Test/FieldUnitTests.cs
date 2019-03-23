using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace DocumentationAssistant.Test
{
	/// <summary>
	/// The field unit test.
	/// </summary>
	[TestClass]
	public class FieldUnitTest : CodeFixVerifier
	{
		/// <summary>
		/// The const field test code.
		/// </summary>
		private const string ConstFieldTestCode = @"
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp4
{
	class FieldTester
	{
		const int ConstFieldTester = 666;

		public FieldTester()
		{
		}
	}
}";

		/// <summary>
		/// The const field test fix code.
		/// </summary>
		private const string ConstFieldTestFixCode = @"
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp4
{
	class FieldTester
	{
        /// <summary>
        /// The const field tester.
        /// </summary>
        const int ConstFieldTester = 666;

		public FieldTester()
		{
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
		[DataRow(ConstFieldTestCode, ConstFieldTestFixCode, 10, 13)]
		public void ShowDiagnosticAndFix(string testCode, string fixCode, int line, int column)
		{
			DiagnosticResult expected = new DiagnosticResult
			{
				Id = FieldAnalyzer.DiagnosticId,
				Message = FieldAnalyzer.MessageFormat,
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
			return new FieldCodeFixProvider();
		}

		/// <summary>
		/// Gets c sharp diagnostic analyzer.
		/// </summary>
		/// <returns>A DiagnosticAnalyzer.</returns>
		protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
		{
			return new FieldAnalyzer();
		}
	}
}
