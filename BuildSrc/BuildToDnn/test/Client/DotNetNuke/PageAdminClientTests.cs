using Build.DotNetNuke.Deployer.Library;
using Build.DotNetNuke.Deployer.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;

namespace Build.Extensions.Tests.DotNetNuke
{
    /// <summary>
    /// Parameter name: portalAlias
    /// </summary>
    [TestClass]
    public class PageAdminClientTests : BaseUnitTest<PageAdminClient>
    {
        [TestMethod]
        public void Page_1_1_IsDeployerInstalled()
        {
            var success = client.IsDeployerInstalled();

            // display some data
            CheckAndDisplayResponse(client);
            Assert.IsTrue(success, "Success");
        }

        [TestMethod]
        public void Page_1_2_GetVersion()
        {
            var version = client.GetVersion();
            TestContext.WriteLine("Version: {0}", version);

            // display some data
            CheckAndDisplayResponse(client);
        }

        [TestMethod]
        public void Page_2_1_Portals()
        {
            var portalsJsonString = client.Portals();
            TestContext.WriteLine("Portals.Length: {0}", portalsJsonString.Length);

            CheckAndDisplayResponse(client);
        }

        [TestMethod]
        public void Page_2_2_Portal()
        {
            TestContext.WriteLine("******* {0} *******", "Get Portal By ID");
            var portalJsonString = client.Portal(id: 2);
            CheckAndDisplayResponse(client);

            TestContext.WriteLine("\r\n******* {0} *******", "Get Portal By Alias");
            portalJsonString = client.Portal(alias: "dnndev.me/alberta");
            CheckAndDisplayResponse(client);
        }

        [TestMethod]
        public void Page_2_3_PortalId_Success()
        {
            var portalAliases = new[] { 
                "dnndev.me", 
                "dnndev.me/alberta", 
                "dnndev.me/anatolikarpov", 
                "dnndev.me/avancesas", 
                "dnndev.me/emiliobossio",
                };

            foreach (var portalAlias in portalAliases)
            {
                var portalId = client.PortalId(portalAlias);
                TestContext.WriteLine("[{0}] {1}", portalId, portalAlias);
                CheckAndDisplayResponse(client);
            }
        }

        [TestMethod]
        public void Page_2_3_PortalId_Fail()
        {
            var portalAliases = new[] { 
                "dnndev.me/wasñkfasdf",
                };

            foreach (var portalAlias in portalAliases)
            {
                try
                {
                    var portalId = client.PortalId(portalAlias);
                    TestContext.WriteLine("[{0}] {1}", portalId, portalAlias);

                    CheckAndDisplayResponse(client);
                    Assert.AreEqual(HttpStatusCode.InternalServerError, client.LastResponse.StatusCode,
                        string.Format("Expected an exception for alias '{0}'.", portalAlias));
                }
                catch (Exception) { }
            }

        }

        [TestMethod]
        public void Page_2_4_Delete()
        { Assert.IsTrue(PageDelete(new PageGetRequest(id: 999))); }


        [TestMethod]
        public void Page_3_1_1_Add()
        {
            // example
            var page = new PageInfoDto
            {
                Name = "Testing UserControls",
                //Title = "",
                //Description = "",
                //Url = "",
                //IconFile = "",
                //IconFileLarge = "",
                //AfterPageFullName = "Inicio",
                //ParentPagePath = "",
                //BeforePagePath = "",
                AfterPagePath = "//Home",
                //PortalAlias = "",
                //PortalID = (int?)null,
                //Modules = PageModuleDto.ToList("forDNN.UsersExportImport"),
                Permissions = PagePermissionDto.ToList("Registered Users,VIEW"),
            };

            var tabID = client.PageAdd(page);

            CheckAndDisplayResponse(client);
        }

        [TestMethod]
        public void Page_3_1_1_Delete()
        { Assert.IsTrue(PageDelete(new PageGetRequest(tabFullName: "Added Page"))); }

        [TestMethod]
        public void Page_3_1_2_Get()
        {
            var pages = client.PageGet(new PageGetRequest("//TestingUserControls"));

            CheckAndDisplayResponse(client);
        }

        [TestMethod]
        public void Page_3_2_List_Portal_0()
        {
            var pages = client.PageList(portalID: 0);
            CheckAndDisplayResponse(client);
        }

        [TestMethod]
        public void Page_3_2_List_Portal_1()
        {
            var pages = client.PageList(portalID: 1);
            CheckAndDisplayResponse(client);
        }

        [TestMethod]
        public void Page_3_2_ListToDictionary()
        {
            var dictPages = client.PageListToDictionary(portalID: 0);
            var tabs = new Hashtable(dictPages);

            TestContext.WriteLine("Hashtable.Count: {0}", tabs.Count);

            CheckAndDisplayResponse(client);
        }

        [TestMethod]
        public void Page_4_1_0_Delete_Partial_ZonaFranca7Dev()
        {
            const string START_PAGE_PATH = "Servicios";
            //const string START_PAGE_PATH = "Operaciones";

            var dictPages = client.PageListToDictionary(portalID: 0);
            var pageIDs = new List<int>();
            var pagesById = new Dictionary<int, string>();

            bool startingPointSurpassed = false;
            foreach (var item in dictPages)
            {
                if (!startingPointSurpassed && item.Key == START_PAGE_PATH)
                { startingPointSurpassed = true; }

                if (startingPointSurpassed)
                {
                    pageIDs.Add(item.Value);
                    pagesById.Add(item.Value, item.Key);
                }
            }

            const int MAX_RETRIES = 10;
            int retries = 0, deletedCount = 0;
            while (pageIDs.Count > 0)
            {
                if (++retries > MAX_RETRIES) { break; }

                for (int i = 0; i < pageIDs.Count; i++)
                {
                    var pageID = pageIDs[i];

                    try
                    {
                        var deleted = client.PageDelete(new PageGetRequest(id: pageID), deleteDescendants: true);
                        if (deleted)
                        {
                            pageIDs.Remove(pageID);
                            deletedCount++;
                        }
                        else { TestContext.WriteLine("Page not deleted: [{0}] {1}", pageID, pagesById[pageID]); }
                    }
                    catch (Exception ex)
                    {
                        TestContext.WriteLine("ERROR on [{0}] {1}: {2}", pageID, pagesById[pageID], ex.Message);
                    }
                }
            }

            TestContext.WriteLine("Pages deleted: {0}", pagesById.Count);
            CheckAndDisplayResponse(client);
            Assert.AreEqual(pagesById.Count, deletedCount, "A few pages were not deleted");
        }

        [TestMethod]
        public void Page_4_1_0_Import_Partial_ZonaFranca7Dev()
        { PageImport(SampleData.DnnTemplatesPage_0_Partial_ZonaFranca7Dev); }

        [TestMethod]
        public void Page_4_1_1_Delete_1_Servicios()
        { Assert.IsTrue(PageDelete(new PageGetRequest(tabFullName: "Servicios"), deleteDescendants: true)); }

        [TestMethod]
        public void Page_4_1_1_Import_1_TopPage_Servicios()
        { PageImport(SampleData.DnnTemplatesPage_1_TopPage_Servicios, afterPagePath: "//Home"); }

        [TestMethod]
        public void Page_4_1_2_Delete_2_Operaciones()
        { Assert.IsTrue(PageDelete(new PageGetRequest(tabFullName: "Operaciones"), deleteDescendants: true)); }

        [TestMethod]
        public void Page_4_1_2_Import_2_TopPage_Operaciones()
        { PageImport(SampleData.DnnTemplatesPage_2_TopPage_Operaciones, beforePagePath: "//ConsultasYReportes"); }

        [TestMethod]
        public void Page_4_1_2_Delete_3_Listado_de_Tarifas()
        { Assert.IsTrue(PageDelete(new PageGetRequest(tabFullName: "Consultas y Reportes/Maestros/Listado de Tarifas"))); }

        //[TestMethod]
        //public void REST_4_1_2_Import_3_Listado_de_Tarifas()
        //{
        //    PageImport(SampleData.DnnTemplatesPage_3_NestedPage_ListadoDeTarifas,
        //      afterPagePath: "//ConsultasyReportes//Maestros//ArinesPendientesporReversar");
        //}

        [TestMethod]
        public void Page_4_1_2_Import_3_Listado_de_Tarifas_With_Module()
        {
            PageImport(SampleData.DnnTemplatesPage_3_NestedPage_ListadoDeTarifasWithModule,
                //afterPagePath: "//ConsultasyReportes//Maestros//ArinesPendientesporReversar");
              afterPageFullName: "Consultas y Reportes/Maestros/Arines Pendientes por Reversar");
        }

        [TestMethod]
        public void Page_4_1_2_Import_4_AdministrarUsuarios_WithModule()
        {
            PageImport(SampleData.DnnTemplatesPage_4_Operaciones_AdministrarUsuarios_WithModule, parentPagePath: "//Operaciones");
            //PageImport(SampleData.DnnTemplatesPage_4_Operaciones_AdministrarUsuarios_WithModule, beforePagePath: "//Operaciones//UsuariosZF");
            //PageImport(SampleData.DnnTemplatesPage_4_Operaciones_AdministrarUsuarios_WithModule, afterPagePath: "//Operaciones//UsuariosZF");
        }

        [TestMethod]
        public void Page_4_1_2_Delete_4_AdministrarUsuarios_WithModule()
        { Assert.IsTrue(PageDelete(new PageGetRequest(tabPath: "//Operaciones//AdministrarUsuarios"))); }


        [TestMethod]
        public void Page_4_1_3_Delete_Any()
        {
            const string DNN_URL = "http://dnndev.me";
            //var deleted = client.PageDelete(tabFullName: "Clubes", deleteDescendants: true);
            var deleted = client.PageDelete(new PageGetRequest(tabPath: "//Clubes"), deleteDescendants: true);
            CheckAndDisplayResponse(client);

            Assert.IsTrue(deleted);
        }

        [TestMethod]
        public void Page_4_2_0_Delete_Reportes()
        {
            const string DNN_URL = "http://dnndev.me/reporteador";
            const string START_PAGE_PATH = "Clubes";

            var dictPages = client.PageListToDictionary("dnndev.me/reporteador");
            var pageIDs = new List<int>();
            var pagesById = new Dictionary<int, string>();

            bool startingPointSurpassed = false;
            foreach (var item in dictPages)
            {
                if (!startingPointSurpassed && item.Key == START_PAGE_PATH)
                { startingPointSurpassed = true; }

                if (startingPointSurpassed)
                {
                    pageIDs.Add(item.Value);
                    pagesById.Add(item.Value, item.Key);
                }
            }

            const int MAX_RETRIES = 10;
            int retries = 0, deletedCount = 0;
            while (pageIDs.Count > 0)
            {
                if (++retries > MAX_RETRIES) { break; }

                for (int i = 0; i < pageIDs.Count; i++)
                {
                    var pageID = pageIDs[i];

                    try
                    {
                        var deleted = client.PageDelete(new PageGetRequest(id: pageID), deleteDescendants: true);
                        if (deleted)
                        {
                            pageIDs.Remove(pageID);
                            deletedCount++;
                        }
                        else { TestContext.WriteLine("Page not deleted: [{0}] {1}", pageID, pagesById[pageID]); }
                    }
                    catch (Exception ex)
                    {
                        TestContext.WriteLine("ERROR on [{0}] {1}: {2}", pageID, pagesById[pageID], ex.Message);
                    }
                }
            }

            TestContext.WriteLine("Pages deleted: {0}", pagesById.Count);
            CheckAndDisplayResponse(client);
            Assert.AreEqual(pagesById.Count, deletedCount, "A few pages were not deleted");
        }
    }
}
