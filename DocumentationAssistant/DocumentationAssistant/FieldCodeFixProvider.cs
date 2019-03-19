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
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(FieldCodeFixProvider)), Shared]
	public class FieldCodeFixProvider : CodeFixProvider
	{
		private const string title = "Add documentation header to this field.";

		public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(FieldAnalyzer.DiagnosticId);

		public sealed override FixAllProvider GetFixAllProvider()
		{
			return WellKnownFixAllProviders.BatchFixer;
		}

		public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

			Diagnostic diagnostic = context.Diagnostics.First();
			Microsoft.CodeAnalysis.Text.TextSpan diagnosticSpan = diagnostic.Location.SourceSpan;

			FieldDeclarationSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<FieldDeclarationSyntax>().First();

			context.RegisterCodeFix(
				CodeAction.Create(
					title: title,
					createChangedDocument: c => AddDocumentationHeaderAsync(context.Document, root, declaration, c),
					equivalenceKey: title),
				diagnostic);
		}

		private async Task<Document> AddDocumentationHeaderAsync(Document document, SyntaxNode root, FieldDeclarationSyntax declarationSyntax, CancellationToken cancellationToken)
		{
			SyntaxTriviaList leadingTrivia = declarationSyntax.GetLeadingTrivia();

			VariableDeclaratorSyntax field = declarationSyntax.DescendantNodes().OfType<VariableDeclaratorSyntax>().First();
			string comment = CommentHelper.GetFieldComment(field.Identifier.ValueText);
			DocumentationCommentTriviaSyntax commentTrivia = await Task.Run(() => DocumentationCommentHelper.GetOnlySummaryCommentTrivia(comment), cancellationToken);

			SyntaxTriviaList newLeadingTrivia = leadingTrivia.Insert(leadingTrivia.Count - 1, SyntaxFactory.Trivia(commentTrivia));
			FieldDeclarationSyntax newDeclaration = declarationSyntax.WithLeadingTrivia(newLeadingTrivia);

			SyntaxNode newRoot = root.ReplaceNode(declarationSyntax, newDeclaration);
			return document.WithSyntaxRoot(newRoot);
		}
	}
}
