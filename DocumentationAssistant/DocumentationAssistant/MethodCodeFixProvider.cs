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
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MethodCodeFixProvider)), Shared]
	public class MethodCodeFixProvider : CodeFixProvider
	{
		private const string title = "Add documentation header to this method.";

		public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(MethodAnalyzer.DiagnosticId);

		public sealed override FixAllProvider GetFixAllProvider()
		{
			return WellKnownFixAllProviders.BatchFixer;
		}

		public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

			Diagnostic diagnostic = context.Diagnostics.First();
			Microsoft.CodeAnalysis.Text.TextSpan diagnosticSpan = diagnostic.Location.SourceSpan;

			MethodDeclarationSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();

			context.RegisterCodeFix(
				CodeAction.Create(
					title: title,
					createChangedDocument: c => AddDocumentationHeaderAsync(context.Document, root, declaration, c),
					equivalenceKey: title),
				diagnostic);
		}

		private async Task<Document> AddDocumentationHeaderAsync(Document document, SyntaxNode root, MethodDeclarationSyntax declarationSyntax, CancellationToken cancellationToken)
		{
			SyntaxTriviaList leadingTrivia = declarationSyntax.GetLeadingTrivia();
			DocumentationCommentTriviaSyntax commentTrivia = await Task.Run(() => CreateDocumentationCommentTriviaSyntax(declarationSyntax), cancellationToken);

			SyntaxTriviaList newLeadingTrivia = leadingTrivia.Insert(leadingTrivia.Count - 1, SyntaxFactory.Trivia(commentTrivia));
			MethodDeclarationSyntax newDeclaration = declarationSyntax.WithLeadingTrivia(newLeadingTrivia);

			SyntaxNode newRoot = root.ReplaceNode(declarationSyntax, newDeclaration);
			return document.WithSyntaxRoot(newRoot);
		}

		private static DocumentationCommentTriviaSyntax CreateDocumentationCommentTriviaSyntax(MethodDeclarationSyntax declarationSyntax)
		{
			SyntaxList<SyntaxNode> list = SyntaxFactory.List<SyntaxNode>();

			string methodComment = CommentHelper.CreateMethodComment(declarationSyntax.Identifier.ValueText);
			list = list.AddRange(DocumentationHeaderHelper.CreateSummaryPartNodes(methodComment));

			if (declarationSyntax.ParameterList.Parameters.Any())
			{
				foreach (ParameterSyntax parameter in declarationSyntax.ParameterList.Parameters)
				{
					string parameterComment = CommentHelper.CreateParameterComment(parameter.Identifier.ValueText);
					list = list.AddRange(DocumentationHeaderHelper.CreateParameterPartNodes(parameter.Identifier.ValueText, parameterComment));
				}
			}

			string returnType = declarationSyntax.ReturnType.ToString();
			if (returnType != "void")
			{
				string returnComment = new ReturnCommentConstruction(declarationSyntax.ReturnType).Comment;
				list = list.AddRange(DocumentationHeaderHelper.CreateReturnPartNodes(returnComment));
			}

			return SyntaxFactory.DocumentationCommentTrivia(SyntaxKind.SingleLineDocumentationCommentTrivia, list);
		}
	}
}
