using System;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ArchitecturalDecisionRecords
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
            if (xmlTrivia.HasArchitecturalDecisionRecord())
            {
                // check structure of Adr
                var adr = xmlTrivia.ChildNodes()
                .OfType<XmlElementSyntax>()
                .First(i => i.StartTag.Name.ToString().Equals(AdrSchema.AdrRootName));
                try
                {
                    var document = new XmlDocument();
                    document.Schemas.Add(_schema);                    
                    document.LoadXml(adr.ToStringSummaryCommentsOmitted());                    

                    bool errors = false;
                    Diagnostic diagnostic = null;
                    document.Validate((o, e) =>
                         {
                             diagnostic = Diagnostic.Create(ArchitecturalDecisionRecordsAnalyzer.ADR0001, adr.GetLocation(), className, e.Message, methodName);
                             errors = true;
                         });
                    if (errors)
                    {
                        _analysisContext.ReportDiagnostic(diagnostic);
                    }

                }
                catch (Exception e)
                {
                    var diagnostic = Diagnostic.Create(ArchitecturalDecisionRecordsAnalyzer.ADR0001, adr.GetLocation(), className, e.Message, methodName);
                    _analysisContext.ReportDiagnostic(diagnostic);
                }
            }
        }     
    }
}
