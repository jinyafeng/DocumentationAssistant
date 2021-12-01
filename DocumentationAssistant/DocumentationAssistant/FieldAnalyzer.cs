using System.Collections.Immutable;
using System.Linq;
using DocumentationAssistant.Helper;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DocumentationAssistant
{
	/// <summary>
	/// The field analyzer.
	/// </summary>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class FieldAnalyzer : DiagnosticAnalyzer
	{
		/// <summary>
		/// The title.
		/// </summary>
		private const string Title = "The field must have a documentation header.";

		/// <summary>
		/// The category.
		/// </summary>
		private const string Category = DocumentationHeaderHelper.Category;

		/// <summary>
		/// The diagnostic id.
		/// </summary>
		public const string DiagnosticId = "ConstFieldDocumentationHeader";

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
			context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.FieldDeclaration);
		}

		/// <summary>
		/// Analyzes node.
		/// </summary>
		/// <param name="context">The context.</param>
		private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
		{
			FieldDeclarationSyntax node = context.Node as FieldDeclarationSyntax;

			// Only const field.
			if (!node.Modifiers.Any(SyntaxKind.ConstKeyword))
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

			VariableDeclaratorSyntax field = node.DescendantNodes().OfType<VariableDeclaratorSyntax>().First();
			context.ReportDiagnostic(Diagnostic.Create(Rule, field.GetLocation()));
		}
	}
}
