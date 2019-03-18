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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PropertyCodeFixProvider)), Shared]
    public class PropertyCodeFixProvider : CodeFixProvider
    {
        private const string title = "Add documentation header to this property.";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(PropertyAnalyzer.DiagnosticId); }
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

            var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<PropertyDeclarationSyntax>().First();

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: title,
                    createChangedDocument: c => AddDocumentationHeaderAsync(context.Document, root, declaration, c),
                    equivalenceKey: title),
                diagnostic);
        }

        private async Task<Document> AddDocumentationHeaderAsync(Document document, SyntaxNode root, PropertyDeclarationSyntax declarationSyntax, CancellationToken cancellationToken)
        {
            var leadingTrivia = declarationSyntax.GetLeadingTrivia();

            bool isBoolean = false;
            if (declarationSyntax.Type is PredefinedTypeSyntax)
            {
                isBoolean = ((PredefinedTypeSyntax)declarationSyntax.Type).Keyword.Kind() == SyntaxKind.BoolKeyword;
            }

            bool hasSetter = declarationSyntax.AccessorList.ChildNodes().Any(o => o.Kind() == SyntaxKind.SetAccessorDeclaration);

            var propertyComment = CommentHelper.GetPropertyComment(declarationSyntax.Identifier.ValueText,isBoolean,hasSetter);
            var commentTrivia = await Task.Run(() => DocumentationCommentHelper.GetOnlySummaryCommentTrivia(propertyComment), cancellationToken);

            var newLeadingTrivia = leadingTrivia.Insert(leadingTrivia.Count - 1, SyntaxFactory.Trivia(commentTrivia));
            var newDeclaration = declarationSyntax.WithLeadingTrivia(newLeadingTrivia);

            var newRoot = root.ReplaceNode(declarationSyntax, newDeclaration);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}
