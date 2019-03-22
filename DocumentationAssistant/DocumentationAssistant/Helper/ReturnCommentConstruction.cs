using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DocumentationAssistant.Helper
{
	public class ReturnCommentConstruction
	{
		public ReturnCommentConstruction(TypeSyntax returnType)
		{
			if (returnType is PredefinedTypeSyntax)
			{
				Comment = GetPrefinedTypeComment(returnType as PredefinedTypeSyntax);
			}
			else if (returnType is IdentifierNameSyntax)
			{
				Comment = GetIdentifierNameTypeComment(returnType as IdentifierNameSyntax);
			}
			else if (returnType is QualifiedNameSyntax)
			{
				Comment = GetQualifiedNameTypeComment(returnType as QualifiedNameSyntax);
			}
			else if (returnType is GenericNameSyntax)
			{
				Comment = GetGenericTypeComment(returnType as GenericNameSyntax);
			}
			else if (returnType is ArrayTypeSyntax)
			{
				Comment = GetArrayTypeComment(returnType as ArrayTypeSyntax);
			}
			else
			{
				Comment = GetGeneralComment(returnType.ToFullString());
			}
		}

		public string Comment { get; }

		private static string GetPrefinedTypeComment(PredefinedTypeSyntax returnType)
		{
			// xxx will remind user to give it a specific name.
			return "The xxx";
		}

		private static string GetIdentifierNameTypeComment(IdentifierNameSyntax returnType)
		{
			return GetGeneralComment(returnType.Identifier.ValueText);
		}

		private static string GetQualifiedNameTypeComment(QualifiedNameSyntax returnType)
		{
			return GetGeneralComment(returnType.ToFullString());
		}

		private string GetArrayTypeComment(ArrayTypeSyntax arrayTypeSyntax)
		{
			return "An array of " + DetermineSpecificObjectName(arrayTypeSyntax.ElementType);
		}

		private static string GetGenericTypeComment(GenericNameSyntax returnType)
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
				return GetGeneralComment(genericTypeStr);
			}

			return GetGeneralComment(genericTypeStr);
		}

		private static string GetGeneralComment(string returnType)
		{
			return GetStartedWord(returnType) + " " + returnType + ".";
		}

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

		private static string GetStartedWord(string returnType)
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
