using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace BT.Tools
{
    public static class ProjectSetup
    {
        [MenuItem("Tools/Setup/Generate Project Structure")]
        public static void GenerateProjectStructure()
        {
            string rootDirectory = "App";
            
            string[] directories =
            {
                "Art",
                "Scenes",
                "Prefabs",
                "Scripts",
                "SO",
            };
    
            string[] subDirectories =
            {
                "Art/Sprite",
                "Art/Texture",
                "Art/Shader",
                "Art/Material",
                "SO/RSE",
                "SO/RSO",
                "SO/SSO",
            };
            
            Folders.CreateFolders(rootDirectory,directories);
            Folders.CreateFolders(rootDirectory,subDirectories);
            
        }
        
        [MenuItem("Tools/Setup/Install Essential Packages")]
        public static void InstallPackages()
        {
            string[] packages =
            {
                "com.unity.textmeshpro"
            };
            
            Package.InstallPackages(packages);
        }
    
        static class Package
        {
            private static AddRequest _request;
            private static Queue<string> _packagesToInstall = new ();

            public static void InstallPackages(string[] packages)
            {
                foreach (var package in packages)
                {
                    _packagesToInstall.Enqueue(package);
                }
                
                if (_packagesToInstall.Count <= 0) return;
                StartNextPackageInstall();
            }
            
            private static async void StartNextPackageInstall()
            {
                _request = Client.Add(_packagesToInstall.Dequeue());
                
                while (!_request.IsCompleted) await Task.Delay(10);
                
                switch (_request.Status)
                {
                    case StatusCode.Success:
                        Debug.Log("Package installed" + _request.Result.packageId );
                        break;
                    case StatusCode.Failure:
                        Debug.LogError("Package install failed" + _request.Error.message);
                        break;
                }

                if (_packagesToInstall.Count <= 0) return;
                await Task.Delay(1000);
                StartNextPackageInstall();
            }
            
        }
    
        static class Folders
        {
            public static void CreateFolders(string rootPath, params string[] folders)
            {
                var path = Application.dataPath;
                var fullPath = Path.Combine(path, rootPath);
                
                if (!Directory.Exists(fullPath)) Directory.CreateDirectory(fullPath);
                
                foreach (var folder in folders)
                {
                    if (Directory.Exists(Path.Combine(path, folder)))
		            {

                        Directory.Move(Path.Combine(path,folder),Path.Combine(fullPath,folder));
                        Directory.Delete(Path.Combine(path,folder));
                    }
		            else Directory.CreateDirectory(Path.Combine(fullPath,folder));
                }
                
                AssetDatabase.Refresh();
            }
        }
        
    }
}
