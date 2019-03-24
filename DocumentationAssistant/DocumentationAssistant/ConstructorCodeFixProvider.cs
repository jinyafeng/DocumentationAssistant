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
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ConstructorCodeFixProvider)), Shared]
	public class ConstructorCodeFixProvider : CodeFixProvider
	{
		private const string title = "Add documentation header to this constructor.";

		public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(ConstructorAnalyzer.DiagnosticId);

		public sealed override FixAllProvider GetFixAllProvider()
		{
			return WellKnownFixAllProviders.BatchFixer;
		}

		public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

			Diagnostic diagnostic = context.Diagnostics.First();
			Microsoft.CodeAnalysis.Text.TextSpan diagnosticSpan = diagnostic.Location.SourceSpan;

			ConstructorDeclarationSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ConstructorDeclarationSyntax>().First();

			context.RegisterCodeFix(
				CodeAction.Create(
					title: title,
					createChangedDocument: c => AddDocumentationHeaderAsync(context.Document, root, declaration, c),
					equivalenceKey: title),
				diagnostic);
		}

		private async Task<Document> AddDocumentationHeaderAsync(Document document, SyntaxNode root, ConstructorDeclarationSyntax declarationSyntax, CancellationToken cancellationToken)
		{
			SyntaxTriviaList leadingTrivia = declarationSyntax.GetLeadingTrivia();
			DocumentationCommentTriviaSyntax commentTrivia = await Task.Run(() => CreateDocumentationCommentTriviaSyntax(declarationSyntax), cancellationToken);

			SyntaxTriviaList newLeadingTrivia = leadingTrivia.Insert(leadingTrivia.Count - 1, SyntaxFactory.Trivia(commentTrivia));
			ConstructorDeclarationSyntax newDeclaration = declarationSyntax.WithLeadingTrivia(newLeadingTrivia);

			SyntaxNode newRoot = root.ReplaceNode(declarationSyntax, newDeclaration);
			return document.WithSyntaxRoot(newRoot);
		}

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
					string parameterComment = CommentHelper.CreateParameterComment(parameter.Identifier.ValueText);
					list = list.AddRange(DocumentationHeaderHelper.CreateParameterPartNodes(parameter.Identifier.ValueText, parameterComment));
				}
			}
			return SyntaxFactory.DocumentationCommentTrivia(SyntaxKind.SingleLineDocumentationCommentTrivia, list);
		}
	}
}
