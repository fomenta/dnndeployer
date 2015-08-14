using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Build.Templates.Helpers
{
    /// <summary>
    /// http://goo.gl/EkJSSm
    /// </summary>
    public class AssemblyReferences
    {
        public static void FindConflictingReferences(string compilationTargetFolder)
        {
            string mainAssemblyName = null;
            if (!IsDirectory(compilationTargetFolder))
            {
                mainAssemblyName = Path.GetFileName(compilationTargetFolder);
                compilationTargetFolder = Path.GetDirectoryName(compilationTargetFolder);
            }

            var assemblies = GetAssemblies(compilationTargetFolder, mainAssemblyName);
            var references = GetReferencesFromAllAssemblies(assemblies);

            var groupsOfConflicts = FindReferencesWithTheSameShortNameButDiffererntFullNames(references);

            foreach (var group in groupsOfConflicts)
            {
                Console.WriteLine("Possible conflicts for {0}:", group.Key);
                foreach (var reference in group)
                {
                    Console.WriteLine("{0} references {1}",
                                          reference.Assembly.Name.PadRight(25),
                                          reference.ReferencedAssembly.FullName);
                }
            }
        }

        private static bool IsDirectory(string path)
        {
            FileAttributes fa = File.GetAttributes(path);
            return (fa & FileAttributes.Directory) != 0;
        }

        private static IEnumerable<IGrouping<string, Reference>> FindReferencesWithTheSameShortNameButDiffererntFullNames(List<Reference> references)
        {
            return from reference in references
                   group reference by reference.ReferencedAssembly.Name
                       into referenceGroup
                       where referenceGroup.ToList().Select(reference => reference.ReferencedAssembly.FullName).Distinct().Count() > 1
                       select referenceGroup;
        }

        private static List<Reference> GetReferencesFromAllAssemblies(List<Assembly> assemblies)
        {
            var references = new List<Reference>();
            foreach (var assembly in assemblies)
            {
                foreach (var referencedAssembly in assembly.GetReferencedAssemblies())
                {
                    references.Add(new Reference
                    {
                        Assembly = assembly.GetName(),
                        ReferencedAssembly = referencedAssembly
                    });
                }
            }
            return references;
        }

        private static List<Assembly> GetAssemblies(string path, string mainAssemblyName = "")
        {
            var files = new List<FileInfo>();
            var directoryToSearch = new DirectoryInfo(path);

            if (!string.IsNullOrEmpty(mainAssemblyName))
            { files.AddRange(directoryToSearch.GetFiles(mainAssemblyName)); }
            else
            {
                files.AddRange(directoryToSearch.GetFiles("*.dll", SearchOption.AllDirectories));
                files.AddRange(directoryToSearch.GetFiles("*.exe", SearchOption.AllDirectories));
            }
            return files.ConvertAll(file => Assembly.LoadFile(file.FullName));
        }

        private class Reference
        {
            public AssemblyName Assembly { get; set; }
            public AssemblyName ReferencedAssembly { get; set; }
        }
    }
}
