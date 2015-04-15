using Build.Templates.Helpers;
using System.Activities;
using System.Activities.Statements;

namespace Build.Templates
{

    class Program
    {
        static void Main(string[] args)
        {
            //InvokeWorkflow();
            //FindConflictingReferencesInAssemblies();
            //CleanXAMLNamespaces();
        }

        private static void InvokeWorkflow()
        {
            //Activity workflow1 = new Workflow1();
            Activity workflow1 = new Sequence();
            WorkflowInvoker.Invoke(workflow1);
        }

        private static void FindConflictingReferencesInAssemblies()
        {
            const string compilationTargetFolder = @"C:\inetpub\dnn721_pbahia\bin\CreacionConductores.dll";
            AssemblyReferences.FindConflictingReferences(compilationTargetFolder);
        }

        private static void CleanXAMLNamespaces()
        {
            //args = new[] { "/report", @"C:\TFS\Facture\Chair\BRDLiteTemplate\BuildTemplates\BRDLite1.0.xaml" };
            //args = new[] { @"C:\TFS\Facture\Chair\BRDLiteTemplate\BuildTemplates\BRDLite1.0.xaml" };
            var args = new[] { @"C:\TFS\Facture\Chair\BRDLiteTemplate\BuildTemplates\BRDLite1.0-HOL-ExtendingTemplate.xaml" };
            XAMLCleaner.MainDoClean(args);
        }
    }
}
