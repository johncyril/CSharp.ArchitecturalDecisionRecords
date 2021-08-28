using ArchitetcturalDecisionRecords;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Schema;

namespace ArchitecturalDecisionRecords
{
    public sealed class AdrSchema : XmlSchemaSet
    {
        public const string AdrRootName = "adr";
        const string xsdName = "ArchitecturalDecisionRecordSchema.xsd";
        private static readonly XmlSchemaSet instance = new AdrSchema();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static AdrSchema()
        {
        }

        private AdrSchema()
        {
            var assembley = Assembly.GetAssembly(typeof(ArchitecturalDecisionRecordsAnalyzer));
            var xmlResource = assembley.GetManifestResourceNames().Where(x => x.Contains(xsdName));
            if (!xmlResource.Any())
            {
                throw new ApplicationException("Cannot find Xsd Schema for Architectural decision records");
            }

            using (Stream schemaStream = Assembly.GetAssembly(typeof(ArchitecturalDecisionRecordsAnalyzer)).GetManifestResourceStream(xmlResource.First()))
            {
                this.Add(XmlSchema.Read(schemaStream, null));
                this.Compile();
            }
        }

        public static XmlSchemaSet Instance
        {
            get
            {
                return instance;
            }
        }
    }
}
