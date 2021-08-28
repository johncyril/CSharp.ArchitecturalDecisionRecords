using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

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
