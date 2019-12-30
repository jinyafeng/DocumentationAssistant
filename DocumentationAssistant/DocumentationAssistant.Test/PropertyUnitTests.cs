using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace DocumentationAssistant.Test
{
	/// <summary>
	/// The property unit test.
	/// </summary>
	[TestClass]
	public class PropertyUnitTest : CodeFixVerifier
	{
		/// <summary>
		/// The property with getter setter test code.
		/// </summary>
		private const string PropertyWithGetterSetterTestCode = @"
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp4
{
	public class PropertyTester
	{
		public string PersonName { get; set; }
	}
}";

		/// <summary>
		/// The property with getter setter test fix code.
		/// </summary>
		private const string PropertyWithGetterSetterTestFixCode = @"
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp4
{
	public class PropertyTester
	{
        /// <summary>
        /// Gets or sets the person name.
        /// </summary>
        public string PersonName { get; set; }
	}
}";

		/// <summary>
		/// The property only getter test code.
		/// </summary>
		private const string PropertyOnlyGetterTestCode = @"
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp4
{
	public class PropertyTester
	{
		public string PersonName { get; }
	}
}";

		/// <summary>
		/// The property only getter test fix code.
		/// </summary>
		private const string PropertyOnlyGetterTestFixCode = @"
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp4
{
	public class PropertyTester
	{
        /// <summary>
        /// Gets the person name.
        /// </summary>
        public string PersonName { get; }
	}
}";

		/// <summary>
		/// The property only getter test fix code.
		/// </summary>
		private const string PropertyPrivateGetterTestCode = @"
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp4
{
	public class PropertyTester
	{
        public string PersonName { get; private set; }
	}
}";

		/// <summary>
		/// The property only getter test fix code.
		/// </summary>
		private const string PropertyPrivateGetterTestFixCode = @"
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp4
{
	public class PropertyTester
	{
        /// <summary>
        /// Gets the person name.
        /// </summary>
        public string PersonName { get; private set; }
	}
}";

		/// <summary>
		/// The property only getter test fix code.
		/// </summary>
		private const string PropertyInternalGetterTestCode = @"
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp4
{
	public class PropertyTester
	{
        public string PersonName { get; internal set; }
	}
}";

		/// <summary>
		/// The property only getter test fix code.
		/// </summary>
		private const string PropertyInternalGetterTestFixCode = @"
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp4
{
	public class PropertyTester
	{
        /// <summary>
        /// Gets the person name.
        /// </summary>
        public string PersonName { get; internal set; }
	}
}";

		/// <summary>
		/// The boolean property test code.
		/// </summary>
		private const string BooleanPropertyTestCode = @"
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp4
{
	public class PropertyTester
	{
		public bool IsTesterStarted { get; set; }
	}
}";

		/// <summary>
		/// The boolean property test fix code.
		/// </summary>
		private const string BooleanPropertyTestFixCode = @"
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp4
{
	public class PropertyTester
	{
        /// <summary>
        /// Gets or sets a value indicating whether tester is started.
        /// </summary>
        public bool IsTesterStarted { get; set; }
	}
}";

		/// <summary>
		/// The expression body property test code.
		/// </summary>
		private const string ExpressionBodyPropertyTestCode = @"
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp4
{
	public class PropertyTester
	{
		public string PersonName => ""Person Name"";
	}
}";

		/// <summary>
		/// The expression body property test fix code.
		/// </summary>
		private const string ExpressionBodyPropertyTestFixCode = @"
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp4
{
	public class PropertyTester
	{
        /// <summary>
        /// Gets the person name.
        /// </summary>
        public string PersonName => ""Person Name"";
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
		[DataRow(PropertyWithGetterSetterTestCode, PropertyWithGetterSetterTestFixCode, 10, 17)]
		[DataRow(PropertyOnlyGetterTestCode, PropertyOnlyGetterTestFixCode, 10, 17)]
		[DataRow(PropertyPrivateGetterTestCode, PropertyPrivateGetterTestFixCode, 10, 23)]
		[DataRow(PropertyInternalGetterTestCode, PropertyInternalGetterTestFixCode, 10, 23)]
		[DataRow(BooleanPropertyTestCode, BooleanPropertyTestFixCode, 10, 15)]
		[DataRow(ExpressionBodyPropertyTestCode, ExpressionBodyPropertyTestFixCode, 10, 17)]
		public void ShowDiagnosticAndFix(string testCode, string fixCode, int line, int column)
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

			this.VerifyCSharpDiagnostic(testCode, expected);

			this.VerifyCSharpFix(testCode, fixCode);
		}

		/// <summary>
		/// Gets c sharp code fix provider.
		/// </summary>
		/// <returns>A CodeFixProvider.</returns>
		protected override CodeFixProvider GetCSharpCodeFixProvider()
		{
			return new PropertyCodeFixProvider();
		}

		/// <summary>
		/// Gets c sharp diagnostic analyzer.
		/// </summary>
		/// <returns>A DiagnosticAnalyzer.</returns>
		protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
		{
			return new PropertyAnalyzer();
		}
	}
}
