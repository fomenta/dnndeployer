using Build.DotNetNuke.Deployer.Library;
using Build.Extensions.DotNetNuke;
using dnncmd.Arguments;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace dnncmd
{
    internal class Program
    {
        private enum ExitCode : int
        {
            Success = 0,
            InvalidArguments,
            RuntimeError,
            ErrorInstallingModule
        };

        public static bool Verbose { get; set; }

        static int Main(string[] args)
        {
#if DEBUG
            TestConsoleApp(ref args);
#endif
            var consoleColor = Console.ForegroundColor;
            try
            {
                return ExecuteOptions(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR: {0}", ex.Message);
                return (int)ExitCode.RuntimeError;
            }
            finally { Console.ForegroundColor = consoleColor; }
        }

        #region Testing
        private static void TestConsoleApp(ref string[] args)
        {
            if (args != null && args.Length > 0) { return; }

            // version
            //args = AddAuthenticationArgs("module -v");
            // verify
            //args = AddAuthenticationArgs("module -y");
            // install
            //args = AddAuthenticationArgs(@"module -i -m C:\TFS\Facture\Modulos\Build\Main\test\Extensions.Tests\App_Data\SampleModules\Blog_06.00.05_Install.zip");
            // upgrade
            //args = AddAuthenticationArgs(@"module -i -m C:\TFS\Facture\Modulos\Build\Main\test\Extensions.Tests\App_Data\SampleModules\Blog_06.00.06_Install.zip");
            // downgrade (force)
            //args = AddAuthenticationArgs(@"module -f -i -m C:\TFS\Facture\Modulos\Build\Main\test\Extensions.Tests\App_Data\SampleModules\Blog_06.00.04_Install.zip");
            // downgrade (error)
            //args = AddAuthenticationArgs(@"module -i -m C:\TFS\Facture\Modulos\Build\Main\test\Extensions.Tests\App_Data\SampleModules\Blog_06.00.04_Install.zip");
            // uninstall
            //args = AddAuthenticationArgs(@"module -u -m Blog");
            //args = AddAuthenticationArgs(@"module -u -m DotNetNuke.Blog");
            // get
            //args = AddAuthenticationArgs("module --list");
            //args = AddAuthenticationArgs("module --list --pattern con --builtin");
            //args = AddAuthenticationArgs("module --list --pattern Palermo.Modules.Bascula");
            args = AddAuthenticationArgs(new[] { "module", "--list", "--pattern", "Palermo.Modules.Bascula" });
            //args = AddAuthenticationArgs("module --list --patron Palermo.Modules.Bascula");
            //args = AddAuthenticationArgs("module --list -p Palermo.Modules.Bascula");
            // %DNNCMD% module %DNN_AUTH% --list --pattern "Palermo.Modules.Bascula"

            // page portal list
            //args = AddAuthenticationArgs("page --portals");
            // page portal
            //args = AddAuthenticationArgs("page --portal --portalid 1");
            //args = AddAuthenticationArgs("page --portal --portalalias alberta");
            // page add
            //args = AddAuthenticationArgs(new[] { "page", "--add", "--name", "Added Page", "--after", "Home" });
            // page delete
            //args = AddAuthenticationArgs("page --delete --path //AddedPage");
            //args = AddAuthenticationArgs(new[] { "page", "--delete", "--fullname", "Added Page" });
            // add html module
            //args = AddAuthenticationArgs(new[] { "page", "--add", "--name", "Added Page", "--after", "Home", "--permissions", "Registered Users", "--modules", "DNN_HTML" });
            // page add
            //args = AddAuthenticationArgs(new[] { "page", "--add", "--name", "Added Page", "--after", "Home", "--permissions", "Registered Users,VIEW|EDIT", "--modules", "forDNN.UsersExportImport,ContentPane" });
            //args = AddAuthenticationArgs(new[] { "page", "--add", "--name", "Added Page", "--after", "Home", "--permissions", "Registered Users", "--modules", "forDNN.UsersExportImport" });
            // page add w/ module title
            //args = AddAuthenticationArgs(new[] { "page", "--add", "--name", "Added Page", "--after", "Home", "--permissions", "Registered Users,VIEW|EDIT", "--modules", "forDNN.UsersExportImport,ContentPane,Export Users, Import Users" });
            // page get
            //args = AddAuthenticationArgs("page -g --portalid 1 --path //Home");

            // add page on another alias
            //args = AddAuthenticationArgs("page --portalalias alberta --add --name Page1 --after Inicio");
            //args = AddAuthenticationArgs("page --portalalias alberta --get --fullname Page1");
            //args = AddAuthenticationArgs("page --portalalias alberta --delete --fullname Page1");
            //args = AddAuthenticationArgs("page --portalalias alberta --delete --path //Page1");

            // version
            //args = AddAuthenticationArgs("pagemodule --version");
            // page modules get
            //args = AddAuthenticationArgs("pagemodule --get --path //TestingUserControls");
            // page modules clear
            //args = AddAuthenticationArgs("pagemodule --clear --path //TestingUserControls");
            // page modules add
            //args = AddAuthenticationArgs("pagemodule --path //TestingUserControls --add --module forDNN.UsersExportImport --title \"User Export/Import\"");
            // page modules add with settings
            //args = AddAuthenticationArgs("pagemodule --path //TestingUserControls --add --module forDNN.UsersExportImport --title \"User Export/Import\" --settingname DefaultUserControl --settingvalue ControlKeyA ");
            // page modules add while keeping existing modules on page
            //args = AddAuthenticationArgs("pagemodule --path //TestingUserControls --add --module forDNN.UsersExportImport --keepexisting");
            // page modules add on specific pane
            //args = AddAuthenticationArgs("pagemodule --path //TestingUserControls --add --module forDNN.UsersExportImport --pane leftPanel");
            // page module re-add and change title
            //args = AddAuthenticationArgs("pagemodule --path //AddedPage --add --module forDNN.UsersExportImport --pane leftPanel");


            // add double quotes to parameters with blanks
            DisplayCommandLine(args);
        }

        private static void DisplayCommandLine(string[] args)
        {
            var argsList = new List<string>();
            foreach (var item in args)
            {
                if (item != null && item.Contains(" ")) { argsList.Add(string.Format("\"{0}\"", item)); }
                else { argsList.Add(item); }
            }

            var cmdLine = string.Join(" ", argsList.ToArray());
            Console.WriteLine("{0}", cmdLine);
        }

        private static string[] AddAuthenticationArgs(string argString)
        {
            const string QUOTE = "\"";
            var args = argString.Split(' ');
            // if found qouted text
            if (!argString.Contains(QUOTE))
            { return AddAuthenticationArgs(args); }
            else
            {
                var argList = new List<string>();
                var quoteOpen = false;
                var quotedParam = string.Empty;
                foreach (var item in args)
                {
                    if (!quoteOpen && item.StartsWith(QUOTE))
                    {
                        if (!item.EndsWith(QUOTE))
                        {
                            quotedParam = item.Substring(1);
                            quoteOpen = true;
                        }
                        // si no hay espacios, si no que la comilla de cerrado está al final del mismo split item
                        else
                        {
                            quotedParam += item.Substring(1, item.Length - 2);
                            argList.Add(item);
                            quoteOpen = false;
                        }
                    }
                    else if (quoteOpen)
                    {
                        if (item.EndsWith(QUOTE))
                        {
                            quoteOpen = false;
                            quotedParam += " " + item.Substring(0, item.Length - 1);
                            argList.Add(quotedParam);
                        }
                        else
                        {
                            quotedParam += item;
                        }
                    }
                    else
                    { argList.Add(item); }
                }

                return AddAuthenticationArgs(argList);
            }
        }

        private static string[] AddAuthenticationArgs(string[] args)
        {
            List<string> newArgs = new List<string>(args);
            return AddAuthenticationArgs(newArgs);
        }

        private static string[] AddAuthenticationArgs(List<string> newArgs)
        {
            newArgs.InsertRange(1, new[] { "-r", "http://dnn721", "--user", "host", "-p", "abc123$" });
            return newArgs.ToArray();
        }
        #endregion


        #region Execute *** Options
        private static int ExecuteOptions(string[] args)
        {
            string invokedVerb = null;
            object invokedVerbInstance = null;

            var options = new Options();
            if (!CommandLine.Parser.Default.ParseArguments(args, options,
                onVerbCommand:
                    (verb, subOptions) => { invokedVerb = verb; invokedVerbInstance = subOptions; })
                )
            { return (int)ExitCode.InvalidArguments; }

            switch (invokedVerb)
            {
                case "module":
                    return ExecuteModuleOptions((ModuleOptions)invokedVerbInstance);
                case "page":
                    return ExecutePageOptions((PageOptions)invokedVerbInstance);
                case "pagemodule":
                    return ExecutePageModuleOptions((PageModuleOptions)invokedVerbInstance);
                default:
                    throw new NotImplementedException();
            }
        }

        private static bool ExecuteCommonOptions(CommonOptions options)
        {
            if (Program.Verbose) { Console.WriteLine("Url: {0}", options.DotNetNukeRootUrl); }

            if (options.Version)
            {
                var client = new DeployerUnsecuredClient(options.DotNetNukeRootUrl);
                var version = client.GetVersion();
                Console.WriteLine("{0}", version);
            }
            else if (options.Verify)
            {
                var client = new DeployerUnsecuredClient(options.DotNetNukeRootUrl);
                var success = client.IsDeployerInstalled();
                Console.WriteLine(success);
            }
            else if (options.Verbose)
            {
                // save verbose state and keep processing
                Program.Verbose = true;
                return false;
            }
            else { return false; }

            return true;
        }

        private static int ExecuteModuleOptions(ModuleOptions options)
        {
            if (ExecuteCommonOptions(options)) { return (int)ExitCode.Success; }

            if (options.Install)
            {
                if (Program.Verbose) { Console.WriteLine("Installing {0}...", Path.GetFileName(options.Module)); }
                if (!Module.Install(options)) { return (int)ExitCode.ErrorInstallingModule; }
            }
            else if (options.Uninstall)
            {
                if (Program.Verbose) { Console.WriteLine("Uninstalling {0}...", Path.GetFileName(options.Module)); }
                if (!Module.Uninstall(options)) { return (int)ExitCode.ErrorInstallingModule; }
            }
            else if (options.List) { Module.List(options); }
            else if (options.ListDesktop) { Module.ListDesktop(options); }
            else { throw new NotImplementedException(); }

            return (int)ExitCode.Success;
        }

        private static int ExecutePageOptions(PageOptions options)
        {
            if (ExecuteCommonOptions(options)) { return (int)ExitCode.Success; }

            if (options.ListPortals) { Page.Portals(options); }
            else if (options.GetPortal) { Page.Portal(options); }
            else if (options.FindOutPortalID) { Page.PortalId(options); }
            else if (options.Add) { Page.Add(options); }
            else if (options.Get) { Page.Get(options); }
            else if (options.Delete) { Page.Delete(options); }
            else if (options.List) { Page.List(options); }
            else if (options.Import) { Page.Import(options); }
            else { throw new NotImplementedException(); }

            return (int)ExitCode.Success;
        }


        private static int ExecutePageModuleOptions(PageModuleOptions options)
        {
            if (ExecuteCommonOptions(options)) { return (int)ExitCode.Success; }

            if (options.Add) { PageModule.Add(options); }
            else if (options.Get) { PageModule.Get(options); }
            else if (options.Clear) { PageModule.Clear(options); }
            else { throw new NotImplementedException(); }

            return (int)ExitCode.Success;
        }
        #endregion

        #region Dnn Run***Action
        public static TResponse RunModuleAction<TResponse>(ModuleOptions options, Func<ModuleAdminClient, ModuleOptions, TResponse> func)
        {
            return RunAction<TResponse, ModuleAdminClient, ModuleOptions>(options, func);
        }

        public static TResponse RunPageAction<TResponse>(PageOptions options, Func<PageAdminClient, PageOptions, TResponse> func)
        {
            return RunAction<TResponse, PageAdminClient, PageOptions>(options, func);
        }

        public static TResponse RunPageModuleAction<TResponse>(PageModuleOptions options, Func<PageModuleAdminClient, PageModuleOptions, TResponse> func)
        {
            return RunAction<TResponse, PageModuleAdminClient, PageModuleOptions>(options, func);
        }

        public static TResponse RunAction<TResponse, TClass, TOptions>(TOptions options, Func<TClass, TOptions, TResponse> func)
            where TClass : AdminBaseClient, new()
            where TOptions : CommonOptions
        {
            var client = new TClass { TargetDotNetNukeRootUrl = options.DotNetNukeRootUrl, UserName = options.UserName, Password = options.Password };

            TResponse data = func(client, options);
            var response = client.LastResponse;

            if (data is bool && !Convert.ToBoolean(data)) { Console.WriteLine("ERROR: {0}", response.Content); }
            Console.WriteLine(FormatJSON(Convert.ToString(data)));

            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.NoContent)
            {
                if (response.ErrorException != null)
                { throw response.ErrorException; }
                else if (!string.IsNullOrWhiteSpace(response.ErrorMessage))
                { throw new ArgumentException(response.ErrorMessage); }
            }


            return data;
        }
        #endregion

        #region Dnn Module Actions
        private static class Module
        {
            public static bool Install(ModuleOptions options)
            { return RunModuleAction<bool>(options, (client, o) => client.ModuleInstall(o.Force, o.Module)); }

            public static bool Uninstall(ModuleOptions options)
            { return RunModuleAction<bool>(options, (client, o) => client.ModuleUninstall(o.Module)); }

            public static string List(ModuleOptions options)
            { return RunModuleAction<string>(options, (client, o) => client.ModuleGet(o.Pattern, o.BuiltIn)); }

            public static string ListDesktop(ModuleOptions options)
            { return RunModuleAction<string>(options, (client, o) => client.ModuleGetDesktop(o.Pattern)); }
        }
        #endregion

        #region Dnn Page Actions
        private static class Page
        {
            public static string Portals(PageOptions options)
            { return RunPageAction<string>(options, (client, o) => client.Portals()); }

            public static string Portal(PageOptions options)
            { return RunPageAction<string>(options, (client, o) => client.Portal(o.PortalAlias, o.PortalID)); }

            public static int PortalId(PageOptions options)
            { return RunPageAction<int>(options, (client, o) => client.PortalId(o.PortalAlias)); }

            public static int Add(PageOptions options)
            {
                var page = new PageInfoDto
                {
                    Name = options.Name,
                    Title = options.Title,
                    Description = options.Description,
                    Url = options.Url,
                    IconFile = options.IconFile,
                    IconFileLarge = options.IconFileLarge,
                    ParentPagePath = options.ParentPagePath,
                    BeforePagePath = options.BeforePagePath,
                    AfterPagePath = options.AfterPagePath,
                    ParentPageFullName = options.ParentPageFullName,
                    BeforePageFullName = options.BeforePageFullName,
                    AfterPageFullName = options.AfterPageFullName,
                    PortalAlias = options.PortalAlias,
                    PortalID = options.PortalID,
                    Modules = PageModuleDto.ToList(options.Modules),
                    Permissions = PagePermissionDto.ToList(options.Permissions),
                };

                return RunPageAction<int>(options, (client, o) => client.PageAdd(page));
            }

            public static string Get(PageOptions options)
            { return RunPageAction<string>(options, (client, o) => client.PageGet(new PageGetRequest(o.TabPath, o.TabFullName, o.TabID, o.PortalAlias, o.PortalID))); }

            public static bool Delete(PageOptions options)
            { return RunPageAction<bool>(options, (client, o) => client.PageDelete(new PageGetRequest(o.TabPath, o.TabFullName, o.TabID, o.PortalAlias, o.PortalID), o.deleteDescendants)); }

            public static string List(PageOptions options)
            { return RunPageAction<string>(options, (client, o) => client.PageList(o.PortalAlias, o.PortalID)); }

            public static int Import(PageOptions options)
            { return RunPageAction<int>(options, (client, o) => client.PageImport(o.PageTemplateFilePath, o.PortalAlias, o.PortalID, o.ParentPagePath, o.BeforePagePath, o.AfterPagePath)); }
        }
        #endregion

        #region Dnn Page Module Actions
        private static class PageModule
        {
            public static int Add(PageModuleOptions options)
            { return RunPageModuleAction<int>(options, (client, o) => client.Add(new PageModuleDto(o.ModuleName, o.ModuleTitle, o.PaneName, o.SettingName, o.SettingValue), new PageGetRequest(o.TabPath, o.TabFullName, o.TabID, o.PortalAlias, o.PortalID), clearExisting: !o.KeepExisting)); }

            public static bool Clear(PageModuleOptions options)
            { return RunPageModuleAction<bool>(options, (client, o) => client.Clear(new PageGetRequest(o.TabPath, o.TabFullName, o.TabID, o.PortalAlias, o.PortalID))); }

            public static string Get(PageModuleOptions options)
            { return RunPageModuleAction<string>(options, (client, o) => client.Get(new PageGetRequest(o.TabPath, o.TabFullName, o.TabID, o.PortalAlias, o.PortalID))); }
        }
        #endregion

        #region Common
        public static string FormatJSON(string json)
        {
            try
            { return JToken.Parse(json).ToString(); }
            catch (JsonReaderException)
            { return json; }
        }
        #endregion
    }
}
