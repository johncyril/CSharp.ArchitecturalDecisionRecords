using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ArchitecturalDecisionRecords
{
    public static class XmlElementSyntaxExtensions
    {
        public static string ToStringSummaryCommentsOmitted(this XmlElementSyntax syntax)
        {
           return syntax.ToString().Replace("///", "");
        }
    }
}
