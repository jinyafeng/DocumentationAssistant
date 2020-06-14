using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace DocumentationAssistant.Helper
{
	/// <summary>
	/// The comment helper.
	/// </summary>
	public static class CommentHelper
	{
		/// <summary>
		/// Creates class comment.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns>The class comment.</returns>
		public static string CreateClassComment(string name)
		{
			return CreateCommonComment(name);
		}

		/// <summary>
		/// Creates field comment.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns>The field comment.</returns>
		public static string CreateFieldComment(string name)
		{
			return CreateCommonComment(name);
		}

		/// <summary>
		/// Creates constructor comment.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="isPrivate">If true, the constructor accessibility is private.</param>
		/// <returns>The contructor comment.</returns>
		public static string CreateConstructorComment(string name, bool isPrivate)
		{
			if (isPrivate)
			{
				return $"Prevents a default instance of the <see cref=\"{name}\"/> class from being created.";
			}
			else
			{
				return $"Initializes a new instance of the <see cref=\"{name}\"/> class.";
			}
		}

		/// <summary>
		/// Creates property comment.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="isBoolean">If ture, the property type is boolean.</param>
		/// <param name="hasSetter">If ture, the property has setter.</param>
		/// <returns>The property comment.</returns>
		public static string CreatePropertyComment(string name, bool isBoolean, bool hasSetter)
		{
			string comment = "Gets";
			if (hasSetter)
			{
				comment += " or sets";
			}

			if (isBoolean)
			{
				comment += CreatePropertyBooleanPart(name);
			}
			else
			{
				comment += " the " + string.Join(" ", SpilitNameAndToLower(name, true));
			}
			return comment + ".";
		}

		/// <summary>
		/// Creates method comment.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns>The method comment.</returns>
		public static string CreateMethodComment(string name)
		{
			List<string> parts = SpilitNameAndToLower(name, false);
			parts[0] = Pluralizer.Pluralize(parts[0]);
			parts.Insert(1, "the");
			return string.Join(" ", parts) + ".";
		}

		/// <summary>
		/// Creates parameter comment.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns>The parameter comment.</returns>
		public static string CreateParameterComment(ParameterSyntax parameter)
		{
			bool isBoolean = false;
			if (parameter.Type.IsKind(SyntaxKind.PredefinedType))
			{
				isBoolean = (parameter.Type as PredefinedTypeSyntax).Keyword.IsKind(SyntaxKind.BoolKeyword);
			}
			else if (parameter.Type.IsKind(SyntaxKind.NullableType))
			{
				var type = (parameter.Type as NullableTypeSyntax).ElementType as PredefinedTypeSyntax;

				// If it is not predefined type syntax, it should be IdentifierNameSyntax.
				var x =parameter.Type as IdentifierNameSyntax;
				
				if (type != null)
				{
					isBoolean = type.Keyword.IsKind(SyntaxKind.BoolKeyword);
				}
			}

			if (isBoolean)
			{
				return "If true, " + string.Join(" ", SpilitNameAndToLower(parameter.Identifier.ValueText, true)) + ".";
			}
			else
			{
				return CreateCommonComment(parameter.Identifier.ValueText);
			}
		}

		/// <summary>
		/// Have the comment.
		/// </summary>
		/// <param name="commentTriviaSyntax">The comment trivia syntax.</param>
		/// <returns>A bool.</returns>
		public static bool HasComment(DocumentationCommentTriviaSyntax commentTriviaSyntax)
		{
			bool hasSummary = commentTriviaSyntax
				.ChildNodes()
				.OfType<XmlElementSyntax>()
				.Any(o => o.StartTag.Name.ToString().Equals(DocumentationHeaderHelper.Summary));

			bool hasInheritDoc = commentTriviaSyntax
				.ChildNodes()
				.OfType<XmlEmptyElementSyntax>()
				.Any(o => o.Name.ToString().Equals(DocumentationHeaderHelper.InheritDoc));

			return hasSummary || hasInheritDoc;
		}

		/// <summary>
		/// Creates property boolean part.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns>The property comment boolean part.</returns>
		private static string CreatePropertyBooleanPart(string name)
		{
			string booleanPart = " a value indicating whether ";

			var parts = SpilitNameAndToLower(name, true).ToList();

			string isWord = parts.FirstOrDefault(o => o == "is");
			if (isWord != null)
			{
				parts.Remove(isWord);
				parts.Insert(parts.Count - 1, isWord);
			}

			booleanPart += string.Join(" ", parts);
			return booleanPart;
		}

		/// <summary>
		/// Creates common comment.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns>The common comment.</returns>
		private static string CreateCommonComment(string name)
		{
			return $"The {string.Join(" ", SpilitNameAndToLower(name, true))}.";
		}

		/// <summary>
		/// Spilits name and make words lower.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="includeFirst">If true, the first character will be lower.</param>
		/// <returns>A list of words.</returns>
		private static List<string> SpilitNameAndToLower(string name, bool includeFirst)
		{
			List<string> parts = NameSpliter.Split(name);

			int i = includeFirst ? 0 : 1;
			for (; i < parts.Count; i++)
			{
				parts[i] = parts[i].ToLower();
			}
			return parts;
		}
	}
}
