using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Schema;
namespace ArchitecturalDecisionRecords
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ArchitecturalDecisionRecordsFixer)), Shared]
    public class ArchitecturalDecisionRecordsFixer : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(ArchitecturalDecisionRecordsAnalyzer.ADR0001.Id);

        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            // TODO: Replace the following code with your own analysis, generating a CodeAction for each fix to suggest

            foreach (var diagnostic in context.Diagnostics)
            {
                var diagnosticSpan = diagnostic.Location.SourceSpan;

                // Find the trivia representing the ADR
                var nodeContainingAdr = root.FindNode(diagnosticSpan);

                // Register a code action that will invoke the fix.
                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: "Correctly format ADR",
                        createChangedDocument: c => CorrectlyFormatAdr(context.Document, nodeContainingAdr, c),
                        equivalenceKey: nameof(ArchitecturalDecisionRecordsFixer)
                        ),
                    diagnostic);
            }         
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
            var syntaxTokenList = new SyntaxTokenList();
            foreach (XmlSchemaElement element in schema.Schemas().Cast<XmlSchema>().First().Elements.Values)
            {
                if (element.ElementSchemaType is XmlSchemaComplexType complexType)
                {
                    // Start Tag for complex elements
                    syntaxTokenList = syntaxTokenList.Add(AdrSyntaxFactory.XmlDocumentationSyntaxNode(element.Name, startTagOnly:true));

                    // Get the sequence particle of the complex type.
                    XmlSchemaSequence sequence = complexType.ContentTypeParticle as XmlSchemaSequence;

                    // Iterate over each XmlSchemaElement in the Items collection.
                    foreach (XmlSchemaElement childElement in sequence.Items)
                    {
                        //syntaxTokenWay
                        syntaxTokenList = syntaxTokenList.Add(AdrSyntaxFactory.XmlDocumentationSyntaxNode(childElement.Name));
                    }                  

                    // End Tag for complex elements
                    syntaxTokenList = syntaxTokenList.Add(AdrSyntaxFactory.XmlDocumentationSyntaxNode(element.Name, endTagOnly: true));
                }
                else
                {
                   elementNodes = elementNodes.Add(SyntaxFactory.XmlEmptyElement(element.Name));
                   syntaxTokenList = syntaxTokenList.Add(AdrSyntaxFactory.XmlDocumentationSyntaxNode(element.Name));
                }
            }

            // Log.Info(elementNodes.ToFullString());
            // https://github.com/dotnet/roslyn/issues/5350#issuecomment-160764691
            var adrTrivia = SyntaxFactory.SyntaxTrivia(SyntaxKind.DocumentationCommentExteriorTrivia, syntaxTokenList.ToFullString());
            
            var triviaToReplace = nodeContainingAdrToFix.GetLeadingTrivia().First(x => x.Kind() == SyntaxKind.SingleLineDocumentationCommentTrivia);

            var root = await document.GetSyntaxRootAsync(cancellationToken);
            root = root.ReplaceTrivia(triviaToReplace, adrTrivia);
            root = root.NormalizeWhitespace();

            return document.WithSyntaxRoot(root);

        }
    }
}
