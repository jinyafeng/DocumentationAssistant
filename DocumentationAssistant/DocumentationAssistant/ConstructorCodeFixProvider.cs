using DocumentationAssistant.Helper;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DocumentationAssistant
{
	/// <summary>
	/// The constructor code fix provider.
	/// </summary>
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ConstructorCodeFixProvider)), Shared]
	public class ConstructorCodeFixProvider : CodeFixProvider
	{
		/// <summary>
		/// The title.
		/// </summary>
		private const string title = "Add documentation header to this constructor";

		/// <summary>
		/// Gets the fixable diagnostic ids.
		/// </summary>
		public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(ConstructorAnalyzer.DiagnosticId);

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

			ConstructorDeclarationSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ConstructorDeclarationSyntax>().First();

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
		private async Task<Document> AddDocumentationHeaderAsync(Document document, SyntaxNode root, ConstructorDeclarationSyntax declarationSyntax, CancellationToken cancellationToken)
		{
			SyntaxTriviaList leadingTrivia = declarationSyntax.GetLeadingTrivia();
			DocumentationCommentTriviaSyntax commentTrivia = await Task.Run(() => CreateDocumentationCommentTriviaSyntax(declarationSyntax), cancellationToken);

			SyntaxTriviaList newLeadingTrivia = leadingTrivia.Insert(leadingTrivia.Count - 1, SyntaxFactory.Trivia(commentTrivia));
			ConstructorDeclarationSyntax newDeclaration = declarationSyntax.WithLeadingTrivia(newLeadingTrivia);

			SyntaxNode newRoot = root.ReplaceNode(declarationSyntax, newDeclaration);
			return document.WithSyntaxRoot(newRoot);
		}

		/// <summary>
		/// Creates documentation comment trivia syntax.
		/// </summary>
		/// <param name="declarationSyntax">The declaration syntax.</param>
		/// <returns>A DocumentationCommentTriviaSyntax.</returns>
		private static DocumentationCommentTriviaSyntax CreateDocumentationCommentTriviaSyntax(ConstructorDeclarationSyntax declarationSyntax)
		{
			SyntaxList<XmlNodeSyntax> list = SyntaxFactory.List<XmlNodeSyntax>();

			bool isPrivate = false;
			if (declarationSyntax.Modifiers.Any(SyntaxKind.PrivateKeyword))
			{
				isPrivate = true;
			}

			string comment = CommentHelper.CreateConstructorComment(declarationSyntax.Identifier.ValueText, isPrivate);
			list = list.AddRange(DocumentationHeaderHelper.CreateSummaryPartNodes(comment));
			if (declarationSyntax.ParameterList.Parameters.Any())
			{
				foreach (ParameterSyntax parameter in declarationSyntax.ParameterList.Parameters)
				{
					string parameterComment = CommentHelper.CreateParameterComment(parameter);
					list = list.AddRange(DocumentationHeaderHelper.CreateParameterPartNodes(parameter.Identifier.ValueText, parameterComment));
				}
			}
			return SyntaxFactory.DocumentationCommentTrivia(SyntaxKind.SingleLineDocumentationCommentTrivia, list);
		}
	}
}
