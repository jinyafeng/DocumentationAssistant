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
	public class ClassAnalyzer : DiagnosticAnalyzer
	{
		private const string Title = "The class must have a documentation header.";
		private const string Category = DocumentationCommentHelper.Category;

		public const string DiagnosticId = "ClassDocumentationHeader";
		public const string MessageFormat = Title;

		private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, true);
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

		public override void Initialize(AnalysisContext context)
		{
			context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.ClassDeclaration);
		}

		private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
		{
			ClassDeclarationSyntax node = context.Node as ClassDeclarationSyntax;

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
					.Any(o => o.StartTag.Name.ToString().Equals(DocumentationCommentHelper.Summary));

				if (hasSummary)
				{
					return;
				}
			}

			context.ReportDiagnostic(Diagnostic.Create(Rule, node.Identifier.GetLocation()));
		}
	}
}
