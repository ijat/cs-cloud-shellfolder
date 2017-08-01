using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cloud_shellfolder;

namespace cloud_shellfolder_example
{
    class Program
    {
        static void Main(string[] args)
        {
            string guid = "341ddd3b-b65b-490b-92fe-420a619c4a96"; // generate your own guid at https://www.guidgen.com/
            string title = "My Cloud App"; // anythin can
            string path = "E:\\asd"; // existed folder
            string icon_path = "E:\\shellIcon.ico"; //existed icon with ico format

            shell_folder myapp = new shell_folder(guid, title, path, icon_path);
            myapp.createFolder();       // create shell folder
            myapp.restartExplorer();    // restart needed to apply changes
            Console.ReadKey();          // wait for user to press anything in console
            myapp.removeFolder();       // remove back the shell folder
            myapp.restartExplorer();    // restart explorer to apply changes
        }
    }
}
