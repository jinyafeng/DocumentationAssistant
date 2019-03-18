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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ConstructorCodeFixProvider)), Shared]
    public class ConstructorCodeFixProvider : CodeFixProvider
    {
        private const string title = "Add documentation header to this constructor.";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(ConstructorAnalyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ConstructorDeclarationSyntax>().First();

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: title,
                    createChangedDocument: c => AddDocumentationHeaderAsync(context.Document, root, declaration, c),
                    equivalenceKey: title),
                diagnostic);
        }

        private async Task<Document> AddDocumentationHeaderAsync(Document document, SyntaxNode root, ConstructorDeclarationSyntax declarationSyntax, CancellationToken cancellationToken)
        {
            var leadingTrivia = declarationSyntax.GetLeadingTrivia();
            var commentTrivia = await Task.Run(() => GetComments(declarationSyntax), cancellationToken);

            var newLeadingTrivia = leadingTrivia.Insert(leadingTrivia.Count - 1, SyntaxFactory.Trivia(commentTrivia));
            var newDeclaration = declarationSyntax.WithLeadingTrivia(newLeadingTrivia);

            var newRoot = root.ReplaceNode(declarationSyntax, newDeclaration);
            return document.WithSyntaxRoot(newRoot);
        }

        private static DocumentationCommentTriviaSyntax GetComments(ConstructorDeclarationSyntax declarationSyntax)
        {
            var list = SyntaxFactory.List<XmlNodeSyntax>();

            var comment = CommentHelper.GetConstructorComment(declarationSyntax.Identifier.ValueText);
            list = list.AddRange(DocumentationCommentHelper.GetSummaryPart(comment));
            if (declarationSyntax.ParameterList.Parameters.Any())
            {
                foreach (var parameter in declarationSyntax.ParameterList.Parameters)
                {
                    var parameterComment = CommentHelper.GetParameterComment(parameter.Identifier.ValueText);
                    list = list.AddRange(DocumentationCommentHelper.GetParameterPart(parameter.Identifier.ValueText, parameterComment));
                }
            }
            return SyntaxFactory.DocumentationCommentTrivia(SyntaxKind.SingleLineDocumentationCommentTrivia, list);
        }
    }
}
