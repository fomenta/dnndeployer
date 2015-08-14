using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Build.CodeIntegrityTests
{
    [TestClass]
    public class TODO_Tests
    {

        /*
         * Comentarios Checkin:
            Updated references
            Merged from trunk

         * Estas subcarpetas no se necesitan sino en FCBUILD y maquina DEV, solo para compilar:
            "Lib\BuildScript" copiada a "bin\BuildScript"
            "Lib\Silverlight" copiada a "bin\Silverlight"
        
         * 
         * --------------------------------------------------------------------
         * 
         * Archivos innecesarios referenciados en .dnn: Deberia ser asi:
            <license/>
            <releaseNotes/>

         * Que la referencia se asi para el que compila los paquetes de DNN
            <Import Project="..\..\bin\BuildScripts\ModulePackage.Targets" />

         * Que no se referencia al folder de instalacion de dotnetnuke absoluto o relativamente
            \dotnetnuke\

         * Que no se referencia al folder TFS absoluto o relativamente
            \TFS\

         * Que no se referencia al folder "Lib" directamente sino al "bin" DNN relativamente
            \Lib\

         * Que no se referencia al folder de instalacion de Devart
            \dotConnect\

         * Referencias GUID de un proyecto globalmente (en todas las otras soluciones y proyectos sea el mismo)
            <ProjectGuid>{5B5474E2-78EC-40D7-98A7-E2C16422089E}</ProjectGuid>
         */
        [TestMethod]
        public void ToDoMethod()
        {
        }
    }
}
