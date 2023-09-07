using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Schema;
using ArchitecturalDecisionRecords.Debug;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Options;

namespace ArchitecturalDecisionRecords
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ArchitecturalDecisionRecordsFixer)), Shared]
    public class ArchitecturalDecisionRecordsFixer : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(ArchitecturalDecisionRecordsAnalyzer.ADR0001.Id); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            // TODO: Replace the following code with your own analysis, generating a CodeAction for each fix to suggest
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the trivia representing the ADR
            var nodeContainingAdr = root.FindNode(diagnosticSpan);
                //.GetStructure().ChildNodes()
                //.OfType<XmlElementSyntax>()
                //.Where(i => i.StartTag.Name.ToString().Equals(AdrSchema.AdrRootName));

            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "Correctly format ADR",
                    createChangedDocument: c => CorrectlyFormatAdr(context.Document, nodeContainingAdr, c),
                    equivalenceKey: nameof(ArchitecturalDecisionRecordsFixer)),
                diagnostic);
        }

        /// <summary>
        /// Correctly formats an ADR based on the ADR schema
        /// See https://learn.microsoft.com/en-us/dotnet/standard/data/xml/traversing-xml-schemas
        /// </summary>
        /// <param name="document"></param>
        /// <param name="syntaxTrivia"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<Document> CorrectlyFormatAdr(Document document, SyntaxNode nodeContainingAdrToFix, CancellationToken cancellationToken)
        {
           
            var schema = AdrSchema.Instance;
            
            var elementNodes = SyntaxFactory.List<XmlNodeSyntax>();
            foreach (XmlSchemaElement element in schema.Schemas().Cast<XmlSchema>().First().Elements.Values)
            {
                if (element.ElementSchemaType is XmlSchemaComplexType complexType)
                {
                    // Get the sequence particle of the complex type.
                    XmlSchemaSequence sequence = complexType.ContentTypeParticle as XmlSchemaSequence;

                    var childNodes = SyntaxFactory.List<XmlNodeSyntax>();
                    // Iterate over each XmlSchemaElement in the Items collection.
                    foreach (XmlSchemaElement childElement in sequence.Items)
                    {
                        var xmlName = SyntaxFactory.XmlName(childElement.Name);
                        //childNodes = childNodes.Add(SyntaxFactory.XmlElement(SyntaxFactory.XmlElementStartTag(xmlName), SyntaxFactory.XmlElementEndTag(xmlName)));
                        childNodes = childNodes.Add(SyntaxFactory.XmlElement(SyntaxFactory.XmlElementStartTag(xmlName), SyntaxFactory.XmlElementEndTag(xmlName)));
                        
                    }

                    elementNodes = elementNodes.Add(SyntaxFactory.XmlMultiLineElement(element.Name, childNodes));
                    
                }
                else
                {
                   elementNodes = elementNodes.Add(SyntaxFactory.XmlEmptyElement(element.Name));
                }
            }

           // Log.Info(elementNodes.ToFullString());
            // https://github.com/dotnet/roslyn/issues/5350#issuecomment-160764691
            //var adrTrivia = SyntaxFactory.DocumentationComment(elementNodes.ToArray());
            var adrTrivia = SyntaxFactory.Trivia(SyntaxFactory.DocumentationComment(elementNodes.ToArray()));
            var triviaToReplace = nodeContainingAdrToFix.ParentTrivia;

            var nodeToReplace = nodeContainingAdrToFix.Parent.ReplaceTrivia(triviaToReplace, adrTrivia);

            var root = await document.GetSyntaxRootAsync(cancellationToken);
            root = root.ReplaceNode(nodeContainingAdrToFix, nodeToReplace);

            return document.WithSyntaxRoot(root);
        }
    }
}
