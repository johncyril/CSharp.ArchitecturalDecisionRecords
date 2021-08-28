using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = ArchitetcturalDecisionRecords.Test.CSharpCodeFixVerifier<
    ArchitetcturalDecisionRecords.ArchitecturalDecisionRecordsAnalyzer,
    ArchitetcturalDecisionRecords.ArchitecturalDecisionRecordsCodeFixProvider>;

namespace ArchitetcturalDecisionRecords.Test
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

        //Diagnostic and CodeFix both triggered and checked for
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

            var expected = VerifyCS.Diagnostic("ArchiecturalDecisionRecords").WithLocation(4,20).WithArguments("TypeName", "'<' is an unexpected token. The expected token is '>'. Line 4, position 17.");
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

            var expected = VerifyCS.Diagnostic("ArchiecturalDecisionRecords").WithLocation(4,20).WithArguments("TypeName", "The element 'adr' has invalid child element 'some'. List of possible elements expected: 'useCase'.");
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

            var expected = VerifyCS.Diagnostic("ArchiecturalDecisionRecords").WithLocation(4,20).WithArguments("TypeName", "The element 'adr' has incomplete content. List of possible elements expected: 'useCase'.");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);


        }

    }
}
