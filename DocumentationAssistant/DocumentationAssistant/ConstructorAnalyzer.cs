using DocumentationAssistant.Helper;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace DocumentationAssistant
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ConstructorAnalyzer : DiagnosticAnalyzer
    {
        private const string Title = "Constructor should have documentation header.";
        private const string Category = DocumentationCommentHelper.Category;

        public const string DiagnosticId = "ConstructorDocumentationHeader";
        public const string MessageFormat = Title;

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, true);
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.ConstructorDeclaration);
        }

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node as ConstructorDeclarationSyntax;

            var commentTriviaSyntax = node
                .GetLeadingTrivia()
                .Select(o=>o.GetStructure())
                .OfType<DocumentationCommentTriviaSyntax>()
                .FirstOrDefault();

            if (commentTriviaSyntax!=null)
            {
                var hasSummary = commentTriviaSyntax
                    .ChildNodes()
                    .OfType<XmlElementSyntax>()
                    .Any(o=>o.StartTag.Name.ToString().Equals(DocumentationCommentHelper.Summary));

                if (hasSummary)
                {
                    return;
                }
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule,node.Identifier.GetLocation()));
        }
    }
}
