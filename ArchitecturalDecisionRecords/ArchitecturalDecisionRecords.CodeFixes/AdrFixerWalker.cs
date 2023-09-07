using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ArchitecturalDecisionRecords
{
    internal class AdrFixerWalker : CSharpSyntaxWalker
    {

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            if (node.HasStructuredTrivia)
            {
                var xmlTrivia = node.GetLeadingTrivia()
                    .Select(i => i.GetStructure())
                    .OfType<DocumentationCommentTriviaSyntax>()
                    .FirstOrDefault();

                //FixAdrTrivia(xmlTrivia);
            }
            base.VisitClassDeclaration(node);
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            if (node.HasStructuredTrivia)
            {
                var methodName = node.Identifier.ToString();

                var xmlTrivia = node.GetLeadingTrivia()
                    .Select(i => i.GetStructure())
                    .OfType<DocumentationCommentTriviaSyntax>()
                    .FirstOrDefault();

                //CheckForAndValidateAdrTrivia(xmlTrivia, className, methodName);
            }

            base.VisitMethodDeclaration(node);
        }
    }
}
