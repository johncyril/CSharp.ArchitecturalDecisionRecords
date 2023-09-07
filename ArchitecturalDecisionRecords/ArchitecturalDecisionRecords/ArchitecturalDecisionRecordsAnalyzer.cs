using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ArchitecturalDecisionRecords
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ArchitecturalDecisionRecordsAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "ADR0001";
       
        private const string Category = "ArchiectralDecisions";

        public static readonly DiagnosticDescriptor ADR0001 = new DiagnosticDescriptor("ADR0001",
            "Architectural Decision Records should be formed correctly", "Ensure elements confirm to the ADR schema",
            Category, 
            DiagnosticSeverity.Warning, 
            isEnabledByDefault: true, 
            description: "Architectural Decision Records should only use fields as per the ADR schema.");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(ADR0001); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxTreeAction(AnalyzeSyntaxTree);
        }

        private static void AnalyzeSyntaxTree(SyntaxTreeAnalysisContext context)
        {
            var syntaxWalker = new AdrSyntaxWalker(context);
            syntaxWalker.Visit(context.Tree.GetRoot());              
        }     
    }
}
