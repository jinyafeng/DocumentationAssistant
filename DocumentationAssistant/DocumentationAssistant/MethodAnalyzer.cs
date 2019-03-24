using System.Collections.Immutable;
using System.Linq;
using DocumentationAssistant.Helper;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DocumentationAssistant
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class MethodAnalyzer : DiagnosticAnalyzer
	{
		private const string Title = "The method must have a documentation header.";
		private const string Category = DocumentationHeaderHelper.Category;

		public const string DiagnosticId = "MethodDocumentationHeader";
		public const string MessageFormat = Title;

		private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, true);
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

		public override void Initialize(AnalysisContext context)
		{
			context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.MethodDeclaration);
		}

		private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
		{
			MethodDeclarationSyntax node = context.Node as MethodDeclarationSyntax;

			DocumentationCommentTriviaSyntax commentTriviaSyntax = node
				.GetLeadingTrivia()
				.Select(o => o.GetStructure())
				.OfType<DocumentationCommentTriviaSyntax>()
				.FirstOrDefault();

			if (commentTriviaSyntax != null)
			{
				bool hasSummary = commentTriviaSyntax
					.ChildNodes()
					.OfType<XmlElementSyntax>()
					.Any(o => o.StartTag.Name.ToString().Equals(DocumentationHeaderHelper.Summary));

				if (hasSummary)
				{
					return;
				}
			}

			context.ReportDiagnostic(Diagnostic.Create(Rule, node.Identifier.GetLocation()));
		}
	}
}
