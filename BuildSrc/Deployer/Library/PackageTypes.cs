
using System;
using System.Collections.Generic;
using System.Linq;

namespace Build.DotNetNuke.Deployer.Library
{
    public static class PackageTypes
    {
        public const string AuthSystem = "AuthSystem";
        public const string Container = "Container";
        public const string JavaScriptLibrary = "JavaScriptLibrary";
        public const string Language = "Language";
        public const string Module = "Module";
        public const string Provider = "Provider";
        public const string Scripts = "Scripts";
        public const string Skin = "Skin";

        public static List<string> Subfolders
        {
            get { return new[] { AuthSystem, Container, JavaScriptLibrary, Language, Module, Provider, Scripts, Skin }.ToList();   }
        }

        public static bool IsInvalid(string packageType)
        {
            return !Subfolders.Contains(packageType);
        }
    }
}