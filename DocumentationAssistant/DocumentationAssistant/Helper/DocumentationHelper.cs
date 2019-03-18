using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace DocumentationAssistant.Helper
{
    public static class DocumentationCommentHelper
    {
        public const string Category = "DocumentationHeader";

        public const string Summary = "summary";

        public static DocumentationCommentTriviaSyntax GetOnlySummaryCommentTrivia(string content)
        {
            SyntaxList<XmlNodeSyntax> list = SyntaxFactory.List( GetSummaryPart(content));
            return SyntaxFactory.DocumentationCommentTrivia(SyntaxKind.SingleLineDocumentationCommentTrivia, list);
        }

        public static XmlNodeSyntax[] GetSummaryPart(string content)
        {
            /*
                 ///[0] <summary>
                 /// The code fix provider.
                 /// </summary>[1] [2]
             */

            // [0] " " + leading comment exterior trivia
            var xmlText0 = GetLineStartText();

            // [1] Summary
            var xmlElement = GetSummaryElement(content);

            // [2] new line 
            var xmlText1 = GetLineEndText();

            return new XmlNodeSyntax[] { xmlText0, xmlElement, xmlText1 };
            
        }

        public static XmlNodeSyntax[] GetParameterPart(string parameterName, string parameterContent)
        {
            ///[0] <param name="parameterName"></param>[1][2]

            // [0] -- line start text 
            var lineStartText = GetLineStartText();

            // [1] -- parameter text
            var parameterText = GetParameterElement(parameterName, parameterContent);

            // [2] -- line end text 
            var lineEndText = GetLineEndText();

            return new XmlNodeSyntax[] { lineStartText, parameterText, lineEndText };
        }

        public static XmlNodeSyntax[] GetReturnPart(string content)
        {
            ///[0] <returns></returns>[1][2]

            var lineStartText = GetLineStartText();

            var returnElement = GetReturnElement(content);

            var lineEndText = GetLineEndText();

            return new XmlNodeSyntax[] { lineStartText,returnElement,lineEndText};
        }

        private static XmlElementSyntax GetSummaryElement(string content)
        {
            var xmlName = SyntaxFactory.XmlName(SyntaxFactory.Identifier(DocumentationCommentHelper.Summary));
            var summaryStartTag = SyntaxFactory.XmlElementStartTag(xmlName);
            var summaryEndTag = SyntaxFactory.XmlElementEndTag(xmlName);

            return SyntaxFactory.XmlElement(
                summaryStartTag,
                SyntaxFactory.SingletonList<XmlNodeSyntax>(GetSummaryText(content)),
                summaryEndTag);
        }

        private static XmlElementSyntax GetParameterElement(string parameterName, string parameterContent)
        {
            var paramName = SyntaxFactory.XmlName("param");

            /// <param name="parameterName">[0][1]</param>[2]

            // [0] -- param start tag with attribute
            var paramAttribute = SyntaxFactory.XmlNameAttribute(parameterName);
            var startTag = SyntaxFactory.XmlElementStartTag(paramName, SyntaxFactory.SingletonList<XmlAttributeSyntax>(paramAttribute));

            // [1] -- content
            var content = SyntaxFactory.XmlText(parameterContent);

            // [2] -- end tag
            var endTag = SyntaxFactory.XmlElementEndTag(paramName);
            return SyntaxFactory.XmlElement(startTag, SyntaxFactory.SingletonList<SyntaxNode>(content), endTag);
        }

        private static XmlElementSyntax GetReturnElement(string content)
        {
            var xmlName = SyntaxFactory.XmlName("returns");
            /// <returns>[0]xxx[1]</returns>[2]

            var startTag = SyntaxFactory.XmlElementStartTag(xmlName);

            var contentText = SyntaxFactory.XmlText(content);

            var endTag = SyntaxFactory.XmlElementEndTag(xmlName);
            return SyntaxFactory.XmlElement(startTag,SyntaxFactory.SingletonList<XmlNodeSyntax>(contentText),endTag);
        }

        private static XmlTextSyntax GetSummaryText(string content)
        {
            content = " " + content;
            /*
                /// <summary>[0]
                /// The code fix provider.[1] [2]
                ///[3] </summary>
             */

            // [0] -- NewLine token
            var newLine0Token = GetNewLineToken();

            // [1] -- Content + leading comment exterior trivia
            var leadingTrivia = GetCommentExterior();
            var text1Token = SyntaxFactory.XmlTextLiteral(leadingTrivia, content, content, SyntaxFactory.TriviaList());

            // [2] -- NewLine token
            var newLine2Token = GetNewLineToken();

            // [3] -- " " + leading comment exterior
            var leadingTrivia2 = GetCommentExterior();
            var text2Token = SyntaxFactory.XmlTextLiteral(leadingTrivia2, " ", " ", SyntaxFactory.TriviaList());

            return SyntaxFactory.XmlText(newLine0Token, text1Token, newLine2Token, text2Token);
        }
        
        private static XmlTextSyntax GetLineStartText()
        {
            /*
                ///[0] <summary>
                /// The code fix provider.
                /// </summary>
            */

            // [0] " " + leading comment exterior trivia
            var xmlText0Leading = GetCommentExterior();
            var xmlText0LiteralToken = SyntaxFactory.XmlTextLiteral(xmlText0Leading, " ", " ", SyntaxFactory.TriviaList());
            var xmlText0 = SyntaxFactory.XmlText(xmlText0LiteralToken);
            return xmlText0;
        }
        
        private static XmlTextSyntax GetLineEndText()
        {
            /*
                /// <summary>
                /// The code fix provider.
                /// </summary>[0]
            */

            // [0] end line token.
            var xmlTextNewLineToken = GetNewLineToken();
            var xmlText = SyntaxFactory.XmlText(xmlTextNewLineToken);
            return xmlText;
        }
        
        private static SyntaxToken GetNewLineToken()
        {
            return SyntaxFactory.XmlTextNewLine(Environment.NewLine, false);
        }
        
        private static SyntaxTriviaList GetCommentExterior()
        {
            return SyntaxFactory.TriviaList(SyntaxFactory.DocumentationCommentExterior("///"));
        }
    }
}
