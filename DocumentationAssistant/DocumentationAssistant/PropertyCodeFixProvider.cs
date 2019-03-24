using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DocumentationAssistant.Helper;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DocumentationAssistant
{
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PropertyCodeFixProvider)), Shared]
	public class PropertyCodeFixProvider : CodeFixProvider
	{
		private const string title = "Add documentation header to this property.";

		public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(PropertyAnalyzer.DiagnosticId);

		public sealed override FixAllProvider GetFixAllProvider()
		{
			return WellKnownFixAllProviders.BatchFixer;
		}

		public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

			Diagnostic diagnostic = context.Diagnostics.First();
			Microsoft.CodeAnalysis.Text.TextSpan diagnosticSpan = diagnostic.Location.SourceSpan;

			PropertyDeclarationSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<PropertyDeclarationSyntax>().First();

			context.RegisterCodeFix(
				CodeAction.Create(
					title: title,
					createChangedDocument: c => AddDocumentationHeaderAsync(context.Document, root, declaration, c),
					equivalenceKey: title),
				diagnostic);
		}

		private async Task<Document> AddDocumentationHeaderAsync(Document document, SyntaxNode root, PropertyDeclarationSyntax declarationSyntax, CancellationToken cancellationToken)
		{
			SyntaxTriviaList leadingTrivia = declarationSyntax.GetLeadingTrivia();

			bool isBoolean = false;
			if (declarationSyntax.Type is PredefinedTypeSyntax)
			{
				isBoolean = ((PredefinedTypeSyntax)declarationSyntax.Type).Keyword.Kind() == SyntaxKind.BoolKeyword;
			}

			bool hasSetter = false;
			if (declarationSyntax.AccessorList!=null && declarationSyntax.AccessorList.Accessors.Any(o => o.Kind() == SyntaxKind.SetAccessorDeclaration))
			{
				hasSetter = true;
			}

			string propertyComment = CommentHelper.CreatePropertyComment(declarationSyntax.Identifier.ValueText, isBoolean, hasSetter);
			DocumentationCommentTriviaSyntax commentTrivia = await Task.Run(() => DocumentationHeaderHelper.CreateOnlySummaryDocumentationCommentTrivia(propertyComment), cancellationToken);

			SyntaxTriviaList newLeadingTrivia = leadingTrivia.Insert(leadingTrivia.Count - 1, SyntaxFactory.Trivia(commentTrivia));
			PropertyDeclarationSyntax newDeclaration = declarationSyntax.WithLeadingTrivia(newLeadingTrivia);

			SyntaxNode newRoot = root.ReplaceNode(declarationSyntax, newDeclaration);
			return document.WithSyntaxRoot(newRoot);
		}
	}
}
