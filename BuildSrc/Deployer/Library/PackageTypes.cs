
using System;

namespace Build.DotNetNuke.Deployer.Library
{
    public static class PackageTypes
    {
        public const string AuthSystem = "AuthSystem";
        public const string Language = "Language";
        public const string Module = "Module";
        public const string Skin = "Skin";
        public const string Container = "Container";

        public static bool IsInvalid(string packageType)
        {
            return !(
                    string.IsNullOrEmpty(packageType)
                    || packageType == Module
                    || packageType == Skin
                    || packageType == Container
                    || packageType == AuthSystem
                    || packageType == Language
                    );
        }
    }
}