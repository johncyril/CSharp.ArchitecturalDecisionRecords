using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace ArchitecturalDecisionRecords
{
    public class AdrSyntaxRewriter : CSharpSyntaxRewriter
    {
        public AdrSyntaxRewriter() : base(true)
        {
        }

        public override SyntaxTrivia VisitTrivia(SyntaxTrivia trivia)
        {
            if (trivia.Kind() == SyntaxKind.SingleLineDocumentationCommentTrivia)
            {
                return AdrSyntaxExpression.Instance();
            }
            return base.VisitTrivia(trivia);
        }        
    }
}
