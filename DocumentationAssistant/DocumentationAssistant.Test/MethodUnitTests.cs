using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;
using System.Linq;

namespace DocumentationAssistant.Test
{
	/// <summary>
	/// The method unit test.
	/// </summary>
	[TestClass]
	public class MethodUnitTest : CodeFixVerifier
	{
		/// <summary>
		/// The inherit doc test code.
		/// </summary>
		private const string InheritDocTestCode = @"
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp4
{
	public class MethodTester
	{
		/// <inheritdoc/>
		public void ShowBasicMethodTester()
		{
		}
	}
}";

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
        /// Shows the basic method tester.
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
        /// Shows the method with parameter tester.
        /// </summary>
        /// <param name=""param1"">The param1.</param>
        /// <param name=""param2"">The param2.</param>
        /// <param name=""param3"">If true, param3.</param>
        public void ShowMethodWithParameterTester(string param1, int param2, bool param3)
		{
		}
	}
}";

		/// <summary>
		/// The method with parameter test code.
		/// </summary>
		private const string MethodWithBooleanParameterTestCode = @"
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp4
{
	public class MethodTester
	{
		public void ShowMethodWithBooleanParameterTester(bool isRed, bool? isAssociatedWithAllProduct)
		{
		}
	}
}";
		/// <summary>
		/// The method with parameter test fix code.
		/// </summary>
		private const string MethodWithBooleanParameterTestFixCode = @"
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp4
{
	public class MethodTester
	{
        /// <summary>
        /// Shows the method with boolean parameter tester.
        /// </summary>
        /// <param name=""isRed"">If true, is red.</param>
        /// <param name=""isAssociatedWithAllProduct"">If true, is associated with all product.</param>
        public void ShowMethodWithBooleanParameterTester(bool isRed, bool? isAssociatedWithAllProduct)
		{
		}
	}
}";

		/// <summary>
		/// The method with parameter test code.
		/// </summary>
		private const string MethodWithNullableStructParameterTestCode = @"
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp4
{
	public class MethodTester
	{
		public void Show(DiagnosticResult? param1, int param2, bool param3)
		{
		}
	}
}";

		/// <summary>
		/// The method with parameter test fix code.
		/// </summary>
		private const string MethodWithNullableStructParameterTestFixCode = @"
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp4
{
	public class MethodTester
	{
        /// <summary>
        /// Shows the.
        /// </summary>
        /// <param name=""param1"">The param1.</param>
        /// <param name=""param2"">The param2.</param>
        /// <param name=""param3"">If true, param3.</param>
        public void Show(DiagnosticResult? param1, int param2, bool param3)
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
        /// Shows the method with return tester.
        /// </summary>
        /// <returns>A MethodTester.</returns>
        public MethodTester ShowMethodWithReturnTester()
		{
			return null;
		}
	}
}";

		/// <summary>
		/// The method with string return test code.
		/// </summary>
		private const string MethodWithStringReturnTestCode = @"
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp4
{
	public class MethodTester
	{
		public string ShowMethodWithStringReturnTester()
		{
			return null;
		}
	}
}";

		/// <summary>
		/// The method with string return test fix code.
		/// </summary>
		private const string MethodWithStringReturnTestFixCode = @"
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp4
{
	public class MethodTester
	{
        /// <summary>
        /// Shows the method with string return tester.
        /// </summary>
        /// <returns>A string.</returns>
        public string ShowMethodWithStringReturnTester()
		{
			return null;
		}
	}
}";

		/// <summary>
		/// The method with object return test code.
		/// </summary>
		private const string MethodWithObjectReturnTestCode = @"
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp4
{
	public class MethodTester
	{
		public object ShowMethodWithObjectReturnTester()
		{
			return null;
		}
	}
}";

		/// <summary>
		/// The method with object return test fix code.
		/// </summary>
		private const string MethodWithObjectReturnTestFixCode = @"
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp4
{
	public class MethodTester
	{
        /// <summary>
        /// Shows the method with object return tester.
        /// </summary>
        /// <returns>An object.</returns>
        public object ShowMethodWithObjectReturnTester()
		{
			return null;
		}
	}
}";

		/// <summary>
		/// The method with int return test code.
		/// </summary>
		private const string MethodWithIntReturnTestCode = @"
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp4
{
	public class MethodTester
	{
		public int ShowMethodWithIntReturnTester()
		{
			return null;
		}
	}
}";

		/// <summary>
		/// The method with int return test fix code.
		/// </summary>
		private const string MethodWithIntReturnTestFixCode = @"
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp4
{
	public class MethodTester
	{
        /// <summary>
        /// Shows the method with int return tester.
        /// </summary>
        /// <returns>An int.</returns>
        public int ShowMethodWithIntReturnTester()
		{
			return null;
		}
	}
}";

		/// <summary>
		/// The method with list int return test code.
		/// </summary>
		private const string MethodWithListIntReturnTestCode = @"
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp4
{
	public class MethodTester
	{
		public List<int> ShowMethodWithListIntReturnTester()
		{
			return null;
		}
	}
}";

		/// <summary>
		/// The method with list int return test fix code.
		/// </summary>
		private const string MethodWithListIntReturnTestFixCode = @"
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp4
{
	public class MethodTester
	{
        /// <summary>
        /// Shows the method with list int return tester.
        /// </summary>
        /// <returns><![CDATA[List<int>]]></returns>
        public List<int> ShowMethodWithListIntReturnTester()
		{
			return null;
		}
	}
}";

		/// <summary>
		/// The method with list list int return test code.
		/// </summary>
		private const string MethodWithListListIntReturnTestCode = @"
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp4
{
	public class MethodTester
	{
		public List<List<int>> ShowMethodWithListListIntReturnTester()
		{
			return null;
		}
	}
}";

		/// <summary>
		/// The method with list list int return test fix code.
		/// </summary>
		private const string MethodWithListListIntReturnTestFixCode = @"
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp4
{
	public class MethodTester
	{
        /// <summary>
        /// Shows the method with list list int return tester.
        /// </summary>
        /// <returns><![CDATA[List<List<int>>]]></returns>
        public List<List<int>> ShowMethodWithListListIntReturnTester()
		{
			return null;
		}
	}
}";

		/// <summary>
		/// The method with list qualified name return test code.
		/// </summary>
		private const string MethodWithListQualifiedNameReturnTestCode = @"
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp4
{
	public class MethodTester
	{
		public List<A.B> ShowMethodWithListQualifiedNameReturnTester()
		{
			return null;
		}
	}
}";

		/// <summary>
		/// The method with list qualified name return test fix code.
		/// </summary>
		private const string MethodWithListQualifiedNameReturnTestFixCode = @"
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp4
{
	public class MethodTester
	{
        /// <summary>
        /// Shows the method with list qualified name return tester.
        /// </summary>
        /// <returns><![CDATA[List<A.B>]]></returns>
        public List<A.B> ShowMethodWithListQualifiedNameReturnTester()
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
		[DataRow(InheritDocTestCode)]
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
		[DataRow(BasicTestCode, BasicTestFixCode, 10, 15)]
		[DataRow(MethodWithParameterTestCode, MethodWithParameterTestFixCode, 10, 15)]
		[DataRow(MethodWithBooleanParameterTestCode, MethodWithBooleanParameterTestFixCode, 10, 15)]
		[DataRow(MethodWithNullableStructParameterTestCode, MethodWithNullableStructParameterTestFixCode, 10, 15)]
		[DataRow(MethodWithReturnTestCode, MethodWithReturnTestFixCode, 10, 23)]
		[DataRow(MethodWithStringReturnTestCode, MethodWithStringReturnTestFixCode, 10, 17)]
		[DataRow(MethodWithObjectReturnTestCode, MethodWithObjectReturnTestFixCode, 10, 17)]
		[DataRow(MethodWithIntReturnTestCode, MethodWithIntReturnTestFixCode, 10, 14)]
		[DataRow(MethodWithListIntReturnTestCode, MethodWithListIntReturnTestFixCode, 10, 20)]
		[DataRow(MethodWithListListIntReturnTestCode, MethodWithListListIntReturnTestFixCode, 10, 26)]
		[DataRow(MethodWithListQualifiedNameReturnTestCode, MethodWithListQualifiedNameReturnTestFixCode, 10, 20)]
		public void ShowDiagnosticAndFix(string testCode, string fixCode, int line, int column)
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

			this.VerifyCSharpDiagnostic(testCode, expected);

			this.VerifyCSharpFix(testCode, fixCode);
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



        #region GetExceptions
        private const string MethodWithException = @"
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp4
{
	public class MethodTester
	{
        /// <summary>
        /// Shows the method with list list int return tester.
        /// </summary>
        /// <returns><![CDATA[List<List<int>>]]></returns>
        public List<List<int>> ShowMethodWithListListIntReturnTester()
		{
			throw new Exception(""test"");
		}
	}
}";

        [TestMethod]
        public async Task GetExceptions_ReturnsMatches()
        {
            var exceptions = MethodCodeFixProvider.GetExceptions(MethodWithException);
            Assert.AreEqual(1, exceptions.ToList().Count);
        }


        private const string MethodWithNoException = @"
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp4
{
	public class MethodTester
	{
        /// <summary>
        /// Shows the method with list list int return tester.
        /// </summary>
        /// <returns><![CDATA[List<List<int>>]]></returns>
        public List<List<int>> ShowMethodWithListListIntReturnTester()
		{
			return null;
		}
	}
}";

        [TestMethod]
        public async Task GetExceptions_ReturnsNoMatches_WhenNoExceptions()
        {
            var exceptions = MethodCodeFixProvider.GetExceptions(MethodWithNoException);
            Assert.AreEqual(0, exceptions.ToList().Count);
        }

        private const string MethodWithDuplicateException = @"
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp4
{
	public class MethodTester
	{
        /// <summary>
        /// Shows the method with list list int return tester.
        /// </summary>
        /// <returns><![CDATA[List<List<int>>]]></returns>
        public List<List<int>> ShowMethodWithListListIntReturnTester()
		{
			throw new Exception(""test"");
            throw new Exception(""test"");
		}
	}
}";

        [TestMethod]
        public async Task GetExceptions_ReturnsDistinctMatches_WhenDuplicateExceptions()
        {
            var exceptions = MethodCodeFixProvider.GetExceptions(MethodWithDuplicateException);
            Assert.AreEqual(1, exceptions.ToList().Count);
        }
        #endregion
    }
}
