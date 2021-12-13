﻿using System.Collections.Immutable;
using System.Linq;
using DocumentationAssistant.Helper;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DocumentationAssistant
{
	/// <summary>
	/// The property analyzer.
	/// </summary>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class PropertyAnalyzer : DiagnosticAnalyzer
	{
		/// <summary>
		/// The title.
		/// </summary>
		private const string Title = "The property must have a documentation header.";

		/// <summary>
		/// The category.
		/// </summary>
		private const string Category = DocumentationHeaderHelper.Category;

		/// <summary>
		/// The diagnostic id.
		/// </summary>
		public const string DiagnosticId = "PropertyDocumentationHeader";

		/// <summary>
		/// The message format.
		/// </summary>
		public const string MessageFormat = Title;

		/// <summary>
		/// The diagnostic descriptor rule.
		/// </summary>
		private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, true);

		/// <summary>
		/// Gets the supported diagnostics.
		/// </summary>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

		/// <summary>
		/// Initializes.
		/// </summary>
		/// <param name="context">The context.</param>
		public override void Initialize(AnalysisContext context)
		{
			context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.PropertyDeclaration);
		}

		/// <summary>
		/// Analyzes node.
		/// </summary>
		/// <param name="context">The context.</param>
		private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
		{
			PropertyDeclarationSyntax node = context.Node as PropertyDeclarationSyntax;

			if (Configuration.IsEnabledForPublicMembersOnly && PrivateMemberChecker.IsPrivateMember(node))
			{
				return;
			}

			DocumentationCommentTriviaSyntax commentTriviaSyntax = node
				.GetLeadingTrivia()
				.Select(o => o.GetStructure())
				.OfType<DocumentationCommentTriviaSyntax>()
				.FirstOrDefault();

			if (commentTriviaSyntax != null && CommentHelper.HasComment(commentTriviaSyntax))
			{
				return;
			}

			context.ReportDiagnostic(Diagnostic.Create(Rule, node.Identifier.GetLocation()));
		}
	}
}
