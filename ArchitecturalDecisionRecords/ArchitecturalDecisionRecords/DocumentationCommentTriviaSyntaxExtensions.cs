using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace ArchitecturalDecisionRecords
{
    public static class DocumentationCommentTriviaSyntaxExtensions
    {
        public static bool HasArchitecturalDecisionRecord(this DocumentationCommentTriviaSyntax documentationCommentTriviaSyntax)
        {
            return documentationCommentTriviaSyntax.ChildNodes()
                .OfType<XmlElementSyntax>()
                .Any(i => i.StartTag.Name.ToString().Equals(AdrSchema.AdrRootName));
        }
    }
}
