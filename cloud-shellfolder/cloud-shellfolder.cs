using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cloud_shellfolder
{
    public class shell_folder
    {
        private string guid { get; set; }
        private string ftitle { get; set; }
        private string fpath { get; set; }
        private string ipath { get; set; }

        public shell_folder(string g, string t, string p, string i)
        {
            // assign the parameters to variables
            this.guid = g;
            this.ftitle = t;
            this.fpath = p;
            this.ipath = i;
        }

        public void createFolder()
        {
            CreateShellFolder(guid, ftitle, fpath, ipath);
        }

        public void removeFolder()
        {
            RemoveShellFolder(guid);
        }

        // Based on https://stackoverflow.com/questions/23777688/pin-a-folder-to-navigation-pane-in-windows-explorer
        private static void CreateShellFolder(string strGUID, string strFolderTitle, string strTargetFolderPath, string strIconPath)
        {
            RegistryKey localKey, keyTemp, rootKey;
            if (Environment.Is64BitOperatingSystem)
                localKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64);
            else
                localKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32);

            rootKey = localKey.CreateSubKey(@"Software\Classes\CLSID\{" + strGUID + "}");
            rootKey.SetValue("", strFolderTitle, RegistryValueKind.String);
            rootKey.SetValue("System.IsPinnedToNameSpaceTree", unchecked((int)0x1), RegistryValueKind.DWord);
            rootKey.SetValue("SortOrderIndex", unchecked((int)0x42), RegistryValueKind.DWord);

            keyTemp = rootKey.CreateSubKey(@"DefaultIcon");
            keyTemp.SetValue("", strIconPath, RegistryValueKind.ExpandString);
            keyTemp.Close();

            keyTemp = rootKey.CreateSubKey(@"InProcServer32");
            keyTemp.SetValue("", @"%systemroot%\system32\shell32.dll", RegistryValueKind.ExpandString);
            keyTemp.Close();

            keyTemp = rootKey.CreateSubKey(@"Instance");
            keyTemp.SetValue("CLSID", "{0E5AAE11-A475-4c5b-AB00-C66DE400274E}", RegistryValueKind.String);
            keyTemp.Close();

            keyTemp = rootKey.CreateSubKey(@"Instance\InitPropertyBag");
            keyTemp.SetValue("Attributes", unchecked((int)0x11), RegistryValueKind.DWord);
            keyTemp.SetValue("TargetFolderPath", strTargetFolderPath, RegistryValueKind.ExpandString);
            keyTemp.Close();

            keyTemp = rootKey.CreateSubKey(@"ShellFolder");
            keyTemp.SetValue("FolderValueFlags", unchecked((int)0x28), RegistryValueKind.DWord);
            keyTemp.SetValue("Attributes", unchecked((int)0xF080004D), RegistryValueKind.DWord);
            keyTemp.Close();
            rootKey.Close();

            keyTemp = localKey.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Desktop\NameSpace\{" + strGUID + "}");
            keyTemp.SetValue("", strFolderTitle, RegistryValueKind.String);
            keyTemp.Close();

            keyTemp = localKey.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\HideDesktopIcons\NewStartPanel");
            keyTemp.SetValue("{" + strGUID + "}", unchecked((int)0x1), RegistryValueKind.DWord);
            keyTemp.Close();
        }

        private static void RemoveShellFolder(string strGUID)
        {
            RegistryKey localKey;
            if (Environment.Is64BitOperatingSystem)
                localKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64);
            else
                localKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32);

            localKey.DeleteSubKeyTree(@"Software\Classes\CLSID\{" + strGUID + "}", false);
            localKey.DeleteSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Desktop\NameSpace\{" + strGUID + "}", false);
            localKey.DeleteSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\HideDesktopIcons\NewStartPanel", false);
        }

        public void restartExplorer()
        {
            foreach (System.Diagnostics.Process exe in System.Diagnostics.Process.GetProcesses())
                if (exe.ProcessName == "explorer")
                    exe.Kill();
        }
    }
}
