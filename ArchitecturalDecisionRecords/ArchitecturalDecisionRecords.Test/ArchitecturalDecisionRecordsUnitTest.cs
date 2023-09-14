using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using VerifyCS = ArchitecturalDecisionRecords.Test.Verifiers.CSharpCodeFixVerifier<
    ArchitecturalDecisionRecords.ArchitecturalDecisionRecordsAnalyzer,
    ArchitecturalDecisionRecords.ArchitecturalDecisionRecordsFixer>;

namespace ArchitecturalDecisionRecords.Test
{
    [TestClass]
    public class ArchitecturalDecisionRecordsUnitTest
    {
        //No diagnostics expected to show up
        [TestMethod]
        public async Task TestMethod1()
        {
            var test = @"";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        //Diagnostic triggered and checked for
        [TestMethod]
        public async Task TestAdrPoorlyFormed()
        {
            var test = @"            
            namespace ConsoleApplication1
            {
                ///<adr>
                ///<some>
                ///</some
                ///</adr>
                class {|#0:TypeName|}
                {   
                }
            }";

            var expected = VerifyCS.Diagnostic(ArchitecturalDecisionRecordsAnalyzer.ADR0001.Id).WithLocation(4,20).WithArguments("TypeName", "'<' is an unexpected token. The expected token is '>'. Line 4, position 17.");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        //Diagnostic and CodeFix both triggered and checked for
        [TestMethod]
        public async Task TestAdrWrongElements()
        {
            var test = @"
            namespace ConsoleApplication1
            {
                ///<adr>
                ///<some>
                ///</some>
                ///</adr>
                class {|#0:TypeName|}
                {   
                }
            }";

            var expected = VerifyCS.Diagnostic(ArchitecturalDecisionRecordsAnalyzer.ADR0001.Id).WithLocation(4,20).WithArguments("TypeName", "The element 'adr' has invalid child element 'some'. List of possible elements expected: 'useCase'.");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);


        }

        //Diagnostic and CodeFix both triggered and checked for
        [TestMethod]
        public async Task TestAdrMissingElements()
        {
            var test = @"            
            namespace ConsoleApplication1
            {
                ///<adr>
                ///</adr>
                class {|#0:TypeName|}
                {   
                }
            }";

            var expected = VerifyCS.Diagnostic(ArchitecturalDecisionRecordsAnalyzer.ADR0001.Id).WithLocation(4,20).WithArguments("TypeName", "The element 'adr' has incomplete content. List of possible elements expected: 'useCase'.");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);


        }
        
        //Diagnostic and CodeFix both triggered and checked for
        [TestMethod]
        public async Task TestAdrMissingElements_FixedByFixer()
        {
            var expectedSource = @"            
            namespace ConsoleApplication1
            {
/// <adr>
/// <useCase></useCase>
/// <concernBeingAddressed></concernBeingAddressed>
/// <agreedApproach></agreedApproach>
/// <acceptedDownside></acceptedDownside>
/// <otherOptionConsidered></otherOptionConsidered>
/// <engineerSignOff></engineerSignOff>
/// </adr>
                class {|#0:TypeName|}
                {   
                }
            }";
            
            var test = @"            
            namespace ConsoleApplication1
            {
                ///<adr>
                ///</adr>
                class {|#0:TypeName|}
                {   
                }
            }";
            var expectedDiagnostic = VerifyCS.Diagnostic(ArchitecturalDecisionRecordsAnalyzer.ADR0001.Id).WithSpan(4, 20,5,26).WithArguments("TypeName", "The element 'adr' has incomplete content. List of possible elements expected: 'useCase'.");
            
            await VerifyCS.VerifyCodeFixAsync(test, expectedDiagnostic, expectedSource);


        }

    }
}
