using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;

namespace ArchitecturalDecisionRecords
{
    public static class AdrSyntaxFactory
    {
        /// <summary>
        /// Adds an Xml Documentation syntax node with a new line at the end
        /// </summary>
        /// <param name="syntaxFactory"></param>
        /// <returns></returns>
        public static SyntaxToken XmlDocumentationSyntaxNode(string elementName, bool startTagOnly = false, bool endTagOnly = false)
        {
            SyntaxToken elementSyntaxtoken;
            if (startTagOnly)
            {
                elementSyntaxtoken = SyntaxFactory.XmlTextNewLine(SyntaxFactory.XmlElementStartTag(SyntaxFactory.XmlName(elementName)).ToString(), false);
            }
            else if (endTagOnly)
            {
                elementSyntaxtoken = SyntaxFactory.XmlTextNewLine(SyntaxFactory.XmlElementEndTag(SyntaxFactory.XmlName(elementName)).ToString(), false);
            }
            else
            {
                var name = SyntaxFactory.XmlName(elementName);
                elementSyntaxtoken = SyntaxFactory.XmlTextNewLine(SyntaxFactory.XmlElement(SyntaxFactory.XmlElementStartTag(name), SyntaxFactory.XmlElementEndTag(name)).ToString(), false);
            }
            elementSyntaxtoken = elementSyntaxtoken.WithLeadingTrivia(elementSyntaxtoken.LeadingTrivia.Add(SyntaxFactory.DocumentationCommentExterior("/// ")));
            elementSyntaxtoken = elementSyntaxtoken.WithTrailingTrivia(elementSyntaxtoken.TrailingTrivia.Add(SyntaxFactory.DocumentationCommentExterior(Environment.NewLine)));
            return elementSyntaxtoken;
        }
    }
}
