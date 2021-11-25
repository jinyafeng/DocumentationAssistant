using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using DocumentationAssistant.Helper;
using DocumentationAssistant.Settings;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DocumentationAssistant
{
    /// <summary>
    ///   The method code fix provider.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MethodCodeFixProvider)), Shared]
    public class MethodCodeFixProvider : CodeFixProvider
    {
        private static Regex regEx = new Regex(@"throw\s+new\s+\w+", RegexOptions.IgnoreCase | RegexOptions.Multiline);

        /// <summary>
        ///   The title.
        /// </summary>
        private const string title = "Add documentation header to this method";

        /// <summary>
        ///   Gets the fixable diagnostic ids.
        /// </summary>
        public override sealed ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(MethodAnalyzer.DiagnosticId);

        /// <summary>
        ///   Gets fix all provider.
        /// </summary>
        /// <returns> A FixAllProvider. </returns>
        public override sealed FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        /// <summary>
        ///   Registers code fixes async.
        /// </summary>
        /// <param name="context"> The context. </param>
        /// <returns> A Task. </returns>
        public override sealed async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            Diagnostic diagnostic = context.Diagnostics.First();
            Microsoft.CodeAnalysis.Text.TextSpan diagnosticSpan = diagnostic.Location.SourceSpan;

            MethodDeclarationSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();

            if (BridgedOptions.Options?.IsEnabledForPublishMembersOnly == true && PrivateMemberVerifier.IsPrivateMember(declaration))
            {
                return;
            }

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: title,
                    createChangedDocument: c => this.AddDocumentationHeaderAsync(context.Document, root, declaration, c),
                    equivalenceKey: title),
                diagnostic);
        }

        /// <summary>
        ///   Adds documentation header async.
        /// </summary>
        /// <param name="document"> The document. </param>
        /// <param name="root"> The root. </param>
        /// <param name="declarationSyntax"> The declaration syntax. </param>
        /// <param name="cancellationToken"> The cancellation token. </param>
        /// <returns> A Task. </returns>
        private async Task<Document> AddDocumentationHeaderAsync(Document document, SyntaxNode root, MethodDeclarationSyntax declarationSyntax, CancellationToken cancellationToken)
        {
            SyntaxTriviaList leadingTrivia = declarationSyntax.GetLeadingTrivia();
            DocumentationCommentTriviaSyntax commentTrivia = await Task.Run(() => CreateDocumentationCommentTriviaSyntax(declarationSyntax), cancellationToken);

            SyntaxTriviaList newLeadingTrivia = leadingTrivia.Insert(leadingTrivia.Count - 1, SyntaxFactory.Trivia(commentTrivia));
            MethodDeclarationSyntax newDeclaration = declarationSyntax.WithLeadingTrivia(newLeadingTrivia);

            SyntaxNode newRoot = root.ReplaceNode(declarationSyntax, newDeclaration);
            return document.WithSyntaxRoot(newRoot);
        }

        /// <summary>
        ///   Creates documentation comment trivia syntax.
        /// </summary>
        /// <param name="declarationSyntax"> The declaration syntax. </param>
        /// <returns> A DocumentationCommentTriviaSyntax. </returns>
        private static DocumentationCommentTriviaSyntax CreateDocumentationCommentTriviaSyntax(MethodDeclarationSyntax declarationSyntax)
        {
            SyntaxList<SyntaxNode> list = SyntaxFactory.List<SyntaxNode>();

            string methodComment = CommentHelper.CreateMethodComment(declarationSyntax.Identifier.ValueText);
            list = list.AddRange(DocumentationHeaderHelper.CreateSummaryPartNodes(methodComment));

            if (declarationSyntax.ParameterList.Parameters.Any())
            {
                foreach (ParameterSyntax parameter in declarationSyntax.ParameterList.Parameters)
                {
                    string parameterComment = CommentHelper.CreateParameterComment(parameter);
                    list = list.AddRange(DocumentationHeaderHelper.CreateParameterPartNodes(parameter.Identifier.ValueText, parameterComment));
                }
            }

            var exceptions = regEx.Matches(declarationSyntax.Body.ToFullString()).OfType<Match>()
                                                                                 .Select(m => m.Groups[0].Value)
                                                                                 .Distinct();

            if (exceptions.Any())
            {
                foreach (var exception in exceptions)
                {
                    list = list.AddRange(DocumentationHeaderHelper.CreateExceptionNodes(exception));
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
