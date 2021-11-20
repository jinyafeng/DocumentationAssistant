using DocumentationAssistant.Helper;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DocumentationAssistant
{
	/// <summary>
	/// The interface code fix provider.
	/// </summary>
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(InterfaceCodeFixProvider)), Shared]
	public class InterfaceCodeFixProvider : CodeFixProvider
	{
		/// <summary>
		/// The title.
		/// </summary>
		private const string title = "Add documentation header to this interface";

		/// <summary>
		/// Gets the fixable diagnostic ids.
		/// </summary>
		public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(InterfaceAnalyzer.DiagnosticId);

		/// <summary>
		/// Gets fix all provider.
		/// </summary>
		/// <returns>A FixAllProvider.</returns>
		public sealed override FixAllProvider GetFixAllProvider()
		{
			return WellKnownFixAllProviders.BatchFixer;
		}

		/// <summary>
		/// Registers code fixes async.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns>A Task.</returns>
		public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

			Diagnostic diagnostic = context.Diagnostics.First();
			Microsoft.CodeAnalysis.Text.TextSpan diagnosticSpan = diagnostic.Location.SourceSpan;

			InterfaceDeclarationSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<InterfaceDeclarationSyntax>().First();

			context.RegisterCodeFix(
				CodeAction.Create(
					title: title,
					createChangedDocument: c => this.AddDocumentationHeaderAsync(context.Document, root, declaration, c),
					equivalenceKey: title),
				diagnostic);
		}

		/// <summary>
		/// Adds documentation header async.
		/// </summary>
		/// <param name="document">The document.</param>
		/// <param name="root">The root.</param>
		/// <param name="declarationSyntax">The declaration syntax.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>A Document.</returns>
		private async Task<Document> AddDocumentationHeaderAsync(Document document, SyntaxNode root, InterfaceDeclarationSyntax declarationSyntax, CancellationToken cancellationToken)
		{
			SyntaxTriviaList leadingTrivia = declarationSyntax.GetLeadingTrivia();

			string comment = CommentHelper.CreateInterfaceComment(declarationSyntax.Identifier.ValueText);
			DocumentationCommentTriviaSyntax commentTrivia = await Task.Run(() => DocumentationHeaderHelper.CreateOnlySummaryDocumentationCommentTrivia(comment), cancellationToken);

			SyntaxTriviaList newLeadingTrivia = leadingTrivia.Insert(leadingTrivia.Count - 1, SyntaxFactory.Trivia(commentTrivia));
			InterfaceDeclarationSyntax newDeclaration = declarationSyntax.WithLeadingTrivia(newLeadingTrivia);

			SyntaxNode newRoot = root.ReplaceNode(declarationSyntax, newDeclaration);
			return document.WithSyntaxRoot(newRoot);
		}
	}
}
