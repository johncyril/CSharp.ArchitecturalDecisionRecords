using System;
using System.IO;

namespace ArchitecturalDecisionRecords.Debug
{
    public static class Log
    {
        /// <summary>
        /// Insert the file path to where you would like log files saved.
        /// </summary>
        private const string Path = @"C:\dev\temporary\generator.log";

        public static void Info(string text) =>
#pragma warning disable RS1035
            File.AppendAllText(Path, $"[{DateTime.Now}] {text}" + Environment.NewLine);
#pragma warning restore RS1035
    }
}