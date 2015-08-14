using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Build.Templates.Helpers
{
    /// <summary>
    /// http://goo.gl/Rt9Ma
    /// </summary>
    public class XAMLCleaner
    {
        public static void MainDoClean(string[] args)
        {
            //args = new[] { "/report", @"C:\TFS\Facture\Chair\BRDLiteTemplate\BuildTemplates\BRDLite1.0.xaml" };
            //args = new[] { @"C:\TFS\Facture\Chair\BRDLiteTemplate\BuildTemplates\BRDLite1.0.xaml" };
            args = new[] { @"C:\TFS\Facture\Chair\BRDLiteTemplate\BuildTemplates\BRDLite1.0-HOL-ExtendingTemplate.xaml" };

            if (args.Length == 2)
            {
                if (args[0].StartsWith("/report", StringComparison.OrdinalIgnoreCase))
                {
                    XamlCleaner.RemoveUnusedNamespaces(args[1], args[1], true);
                }
                else
                {
                    XamlCleaner.RemoveUnusedNamespaces(args[0], args[1], false);
                }
            }
            else if (args.Length == 1)
            {
                XamlCleaner.RemoveUnusedNamespaces(args[0], args[0], false);
            }
            else
            {
                PrintUsage();
            }
        }

        static void PrintUsage()
        {
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine(" XAMLCleaner.exe <xaml_file> [<new_xaml_file>]");
            Console.WriteLine(" - removes unused namespaces from xaml_file and");
            Console.WriteLine(" optionally puts the changes in new_xaml_file.");
            Console.WriteLine(" XAMLCleaner.exe /report <xaml_file>");
            Console.WriteLine(" - prints the unused namespaces found in xaml_file.");
            Console.WriteLine();
        }
    }

    class XmlNamespace
    {
        public String Prefix { get; set; }
        public String Namespace { get; set; }
        public String Declaration { get; set; }
    }

    static class XamlCleaner
    {
        public static void RemoveUnusedNamespaces(String inputFile, String outputFile, bool reportOnly)
        {
            List<XmlNamespace> namespaces = new List<XmlNamespace>();
            List<String> ignoredNamespaces = new List<String>();
            String fileContents = File.ReadAllText(inputFile, Encoding.UTF8);
            String newFileContents = fileContents;
            Regex ignorableRegex = new Regex("(:Ignorable=\")(.*?)\"", RegexOptions.Singleline);
            Regex regex = new Regex("(xmlns\\:)(.*?)=\"(.*?)\"", RegexOptions.Singleline);

            foreach (Match m in ignorableRegex.Matches(fileContents))
            {
                ignoredNamespaces.AddRange(m.Groups[2].Value.Split(' '));
            }

            foreach (Match m in regex.Matches(fileContents))
            {
                namespaces.Add(new XmlNamespace() { Prefix = m.Groups[2].Value, Namespace = m.Groups[3].Value, Declaration = m.Groups[0].Value });
            }

            foreach (XmlNamespace ns in namespaces)
            {
                // Only remove namespaces that are not in the Ignore section
                // and contain version=10
                // and are not used in the file
                if (!ignoredNamespaces.Contains(ns.Prefix) &&
                    ns.Namespace.IndexOf("version=10", StringComparison.OrdinalIgnoreCase) >= 0 &&
                    !fileContents.Contains(ns.Prefix + ":"))
                {
                    Console.WriteLine("Removing unused namespace: {0}", ns.Declaration);
                    newFileContents = newFileContents.Replace(ns.Declaration, String.Empty);
                }
            }

            if (!reportOnly)
            {
                File.WriteAllText(outputFile, newFileContents, Encoding.UTF8);
            }

        }
    }
}
