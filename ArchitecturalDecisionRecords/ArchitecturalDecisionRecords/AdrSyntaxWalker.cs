using ArchitecturalDecisionRecords;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace ArchitetcturalDecisionRecords
{
    public class AdrSyntaxWalker : CSharpSyntaxWalker
    { 
        private string className;
        private readonly SyntaxTreeAnalysisContext _analysisContext;
        private readonly XmlSchemaSet _schema;

        public AdrSyntaxWalker(SyntaxTreeAnalysisContext analysisContext)
        {
            _analysisContext = analysisContext;
            _schema = AdrSchema.Instance;
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            if (node.HasStructuredTrivia)
            {
                className = node.Identifier.ToString();

                var xmlTrivia = node.GetLeadingTrivia()
               .Select(i => i.GetStructure())
               .OfType<DocumentationCommentTriviaSyntax>()
                .FirstOrDefault();

                CheckForAndValidateAdrTrivia(xmlTrivia, className);
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

                CheckForAndValidateAdrTrivia(xmlTrivia, className, methodName);
            }

            base.VisitMethodDeclaration(node);
        }

        private void CheckForAndValidateAdrTrivia(DocumentationCommentTriviaSyntax xmlTrivia, string className, string methodName = null)
        {
            var hasAdr = xmlTrivia.ChildNodes()
                   .OfType<XmlElementSyntax>()
                   .Any(i => i.StartTag.Name.ToString().Equals(AdrSchema.AdrRootName));

            if (hasAdr)
            {
                // check structure of Adr
                var adr = xmlTrivia.ChildNodes()
                .OfType<XmlElementSyntax>()
                .First(i => i.StartTag.Name.ToString().Equals(AdrSchema.AdrRootName));
                try
                {
                    var document = new XmlDocument();
                    document.Schemas = _schema;                    
                    document.LoadXml(adr.ToStringSummaryCommentsOmitted());                    

                    bool errors = false;
                    Diagnostic diagnostic = null;
                    document.Validate((o, e) =>
                         {
                             diagnostic = Diagnostic.Create(ArchitecturalDecisionRecordsAnalyzer.Rule, adr.GetLocation(), className, e.Message, methodName);
                             errors = true;
                         });
                    if (errors)
                    {
                        _analysisContext.ReportDiagnostic(diagnostic);
                    }

                }
                catch (Exception e)
                {
                    var diagnostic = Diagnostic.Create(ArchitecturalDecisionRecordsAnalyzer.Rule, adr.GetLocation(), className, e.Message, methodName);
                    _analysisContext.ReportDiagnostic(diagnostic);
                }
            }
        }     
    }
}
