using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace DocumentationAssistant.Test
{
	/// <summary>
	/// The constructor unit test.
	/// </summary>
	[TestClass]
	public class ConstrcutorUnitTest : CodeFixVerifier
	{
		/// <summary>
		/// The public constructor test code.
		/// </summary>
		private const string PublicConstructorTestCode = @"
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp4
{
	class ConstructorTester
	{
		public ConstructorTester()
		{
		}
	}
}";

		/// <summary>
		/// The public contructor test fix code.
		/// </summary>
		private const string PublicContructorTestFixCode = @"
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp4
{
	class ConstructorTester
	{
        /// <summary>
        /// Initializes a new instance of the <see cref=""ConstructorTester""/> class.
        /// </summary>
        public ConstructorTester()
		{
		}
	}
}";

		/// <summary>
		/// The private constructor test code.
		/// </summary>
		private const string PrivateConstructorTestCode = @"
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp4
{
	class ConstructorTester
	{
		private ConstructorTester()
		{
		}
	}
}";

		/// <summary>
		/// The private contructor test fix code.
		/// </summary>
		private const string PrivateContructorTestFixCode = @"
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp4
{
	class ConstructorTester
	{
        /// <summary>
        /// Prevents a default instance of the <see cref=""ConstructorTester""/> class from being created.
        /// </summary>
        private ConstructorTester()
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
		[DataRow(PublicConstructorTestCode, PublicContructorTestFixCode, 10, 10)]
		[DataRow(PrivateConstructorTestCode, PrivateContructorTestFixCode, 10, 11)]
		public void ShowDiagnosticAndFix(string testCode, string fixCode, int line, int column)
		{
			DiagnosticResult expected = new DiagnosticResult
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

			VerifyCSharpFix(testCode, fixCode);
		}

		/// <summary>
		/// Gets c sharp code fix provider.
		/// </summary>
		/// <returns>A CodeFixProvider.</returns>
		protected override CodeFixProvider GetCSharpCodeFixProvider()
		{
			return new ConstructorCodeFixProvider();
		}

		/// <summary>
		/// Gets c sharp diagnostic analyzer.
		/// </summary>
		/// <returns>A DiagnosticAnalyzer.</returns>
		protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
		{
			return new ConstructorAnalyzer();
		}
	}
}
