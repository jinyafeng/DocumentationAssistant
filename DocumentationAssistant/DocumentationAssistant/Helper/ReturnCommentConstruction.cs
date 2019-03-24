using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DocumentationAssistant.Helper
{
	/// <summary>
	/// The return comment construction.
	/// </summary>
	public class ReturnCommentConstruction
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ReturnCommentConstruction"/> class.
		/// </summary>
		/// <param name="returnType">The return type.</param>
		public ReturnCommentConstruction(TypeSyntax returnType)
		{
			if (returnType is PredefinedTypeSyntax)
			{
				Comment = GeneratePrefinedTypeComment(returnType as PredefinedTypeSyntax);
			}
			else if (returnType is IdentifierNameSyntax)
			{
				Comment = GenerateIdentifierNameTypeComment(returnType as IdentifierNameSyntax);
			}
			else if (returnType is QualifiedNameSyntax)
			{
				Comment = GenerateQualifiedNameTypeComment(returnType as QualifiedNameSyntax);
			}
			else if (returnType is GenericNameSyntax)
			{
				Comment = GenerateGenericTypeComment(returnType as GenericNameSyntax);
			}
			else if (returnType is ArrayTypeSyntax)
			{
				Comment = GenerateArrayTypeComment(returnType as ArrayTypeSyntax);
			}
			else
			{
				Comment = GenerateGeneralComment(returnType.ToFullString());
			}
		}

		/// <summary>
		/// Generates the comment.
		/// </summary>
		public string Comment { get; }

		/// <summary>
		/// Generates prefined type comment.
		/// </summary>
		/// <param name="returnType">The return type.</param>
		/// <returns>The comment.</returns>
		private static string GeneratePrefinedTypeComment(PredefinedTypeSyntax returnType)
		{
			// xxx will remind user to give it a specific name.
			return "The xxx.";
		}

		/// <summary>
		/// Generates identifier name type comment.
		/// </summary>
		/// <param name="returnType">The return type.</param>
		/// <returns>The comment.</returns>
		private static string GenerateIdentifierNameTypeComment(IdentifierNameSyntax returnType)
		{
			return GenerateGeneralComment(returnType.Identifier.ValueText);
		}

		/// <summary>
		/// Generates qualified name type comment.
		/// </summary>
		/// <param name="returnType">The return type.</param>
		/// <returns>The comment.</returns>
		private static string GenerateQualifiedNameTypeComment(QualifiedNameSyntax returnType)
		{
			return GenerateGeneralComment(returnType.ToString());
		}

		/// <summary>
		/// Generates array type comment.
		/// </summary>
		/// <param name="arrayTypeSyntax">The array type syntax.</param>
		/// <returns>The comment.</returns>
		private string GenerateArrayTypeComment(ArrayTypeSyntax arrayTypeSyntax)
		{
			return "An array of " + DetermineSpecificObjectName(arrayTypeSyntax.ElementType);
		}

		/// <summary>
		/// Generates generic type comment.
		/// </summary>
		/// <param name="returnType">The return type.</param>
		/// <returns>The comment.</returns>
		private static string GenerateGenericTypeComment(GenericNameSyntax returnType)
		{
			// ReadOnlyCollection IReadOnlyCollection 
			string genericTypeStr = returnType.Identifier.ValueText;
			if (genericTypeStr.Contains("ReadOnlyCollection"))
			{
				return "A read only collection of " + DetermineSpecificObjectName(returnType.TypeArgumentList.Arguments.First());
			}

			// IEnumerable IList List
			if (genericTypeStr == "IEnumerable" || genericTypeStr.Contains("List"))
			{
				return "A list of " + DetermineSpecificObjectName(returnType.TypeArgumentList.Arguments.First());
			}

			if (genericTypeStr.Contains("Dictionary"))
			{
				return GenerateGeneralComment(genericTypeStr);
			}

			return GenerateGeneralComment(genericTypeStr);
		}

		/// <summary>
		/// Generates general comment.
		/// </summary>
		/// <param name="returnType">The return type.</param>
		/// <returns>The comment.</returns>
		private static string GenerateGeneralComment(string returnType)
		{
			return DetermineStartedWord(returnType) + " " + returnType + ".";
		}

		/// <summary>
		/// Determines specific object name.
		/// </summary>
		/// <param name="specificType">The specific type.</param>
		/// <returns>The comment.</returns>
		private static string DetermineSpecificObjectName(TypeSyntax specificType)
		{
			if (specificType is IdentifierNameSyntax)
			{
				return Pluralizer.Pluralize(((IdentifierNameSyntax)specificType).Identifier.ValueText) + ".";
			}
			else
			{
				// xxx will remind user to give it a specific name.
				return "xxx";
			}
		}

		/// <summary>
		/// Determines started word.
		/// </summary>
		/// <param name="returnType">The return type.</param>
		/// <returns>The comment.</returns>
		private static string DetermineStartedWord(string returnType)
		{
			List<char> vowelChars = new List<char>() { 'a', 'e', 'i', 'o', 'u' };
			if (vowelChars.Contains(char.ToLower(returnType[0])))
			{
				return "An";
			}
			else
			{
				return "A";
			}
		}
	}
}
