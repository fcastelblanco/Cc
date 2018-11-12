using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using Cc.Common.LogHelper;

namespace Cc.Common.ExtensionMethods
{
    public static class WindowsExtension
    {
        public static List<ServiceController> GetServices()
        {
            return ServiceController.GetServices().ToList();
        }

        public static void CopyFolder(DirectoryInfo sourcePath, DirectoryInfo destinationPath)
        {
            try
            {
                foreach (var folder in sourcePath.GetDirectories())
                    CopyFolder(folder, destinationPath.CreateSubdirectory(folder.Name));
                foreach (var file in sourcePath.GetFiles())
                    file.CopyTo(System.IO.Path.Combine(destinationPath.FullName, file.Name));
            }
            catch (IOException ex)
            {
                Log.Instance.Error(ex);
            }
        }

        public static void RenameFolder(DirectoryInfo sourcePath, DirectoryInfo destinationPath, string oldFolderName,
            string newFolderName)
        {
            foreach (var folder in sourcePath.GetDirectories())
                if (folder.Name.Equals(oldFolderName))
                    folder.MoveTo(System.IO.Path.Combine(destinationPath.FullName, newFolderName));
        }

        public static void RenameFile(DirectoryInfo sourcePath, DirectoryInfo destinationPath, string oldFileName,
            string newFileName)
        {
            foreach (var file in sourcePath.GetFiles())
                if (file.Exists)
                    if (file.Name.Equals(oldFileName))
                    {
                        var extention = file.Name.Split('.')[1];
                        file.MoveTo(System.IO.Path.Combine(sourcePath.FullName, newFileName + "." + extention));
                    }
        }

        public static void InstallService(string servicePath)
        {
            try
            {
                var installer = new AssemblyInstaller
                {
                    UseNewContext = true,
                    Path = servicePath
                };
                installer.Install(null);
                installer.Commit(null);
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex);
            }
        }

        public static bool IsServiceInstalled(string serviceName)
        {
            var serviceController = ServiceController.GetServices()
    .FirstOrDefault(s => s.ServiceName == serviceName);

            return serviceController != null;
        }

        public static bool StartService(string serviceName)
        {
            var serviceController = new ServiceController { ServiceName = serviceName };
            Log.Instance.Info("The {0} service status is currently set to {1}", serviceName,
                serviceController.Status.ToString());

            if (serviceController.Status == ServiceControllerStatus.Stopped)
            {
                Log.Instance.Info("Starting the {0} service ...", serviceName);

                try
                {
                    serviceController.Start();
                    serviceController.WaitForStatus(ServiceControllerStatus.Running);
                    Log.Instance.Info("The {0} service status is now set to {1}.", serviceName,
                        serviceController.Status.ToString());
                    return true;
                }
                catch (InvalidOperationException e)
                {
                    Log.Instance.Info("Could not start the {0} service.", serviceName);
                    Log.Instance.Error(e);
                    return false;
                }
            }

            Log.Instance.Info("Service {0} already running.", serviceName);
            return true;
        }

        public static bool StopService(string serviceName, out Exception exception)
        {
            exception = null;
            var serviceController = new ServiceController { ServiceName = serviceName };
            Log.Instance.Info("The {0} service status is currently set to {1}", serviceName,
                serviceController.Status.ToString());

            if (serviceController.Status == ServiceControllerStatus.Running)
            {
                Log.Instance.Info("Stopping the {0} service ...", serviceName);

                try
                {
                    serviceController.Stop();
                    serviceController.WaitForStatus(ServiceControllerStatus.Stopped);
                    Log.Instance.Info("The {0} service status is now set to {1}.", serviceName,
                        serviceController.Status.ToString());
                    return true;
                }
                catch (InvalidOperationException e)
                {
                    Log.Instance.Info("Could not stop the {0} service.", serviceName);
                    Log.Instance.Error(e);
                    exception = e;
                    return false;
                }
            }

            Log.Instance.Info("Cannot stop service {0} because it's already inactive.", serviceName);
            return true;
        }

        public static void DeleteFolderRecursive(DirectoryInfo directoryInfo)
        {
            directoryInfo.Attributes = FileAttributes.Normal;

            foreach (var childDir in directoryInfo.GetDirectories())
                DeleteFolderRecursive(childDir);

            foreach (var file in directoryInfo.GetFiles())
                file.IsReadOnly = false;

            if (Directory.Exists(directoryInfo.FullName))
            {
                directoryInfo.Delete(true);
            }
        }

        public static void DirectoryCopy(
            string sourceDirectoryName, string destinationDirectoryName, bool copySubDirectories, bool overWriteData = false)
        {
            var directoryInfo = new DirectoryInfo(sourceDirectoryName);
            var directories = directoryInfo.GetDirectories();

            if (!directoryInfo.Exists)
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirectoryName);

            if (!Directory.Exists(destinationDirectoryName))
            {
                Path.CreateDirectoryRecursively(destinationDirectoryName);
            }

            var files = directoryInfo.GetFiles();

            foreach (var file in files)
            {
                var temporalPath = System.IO.Path.Combine(destinationDirectoryName, file.Name);
                file.CopyTo(temporalPath, overWriteData);
            }

            if (!copySubDirectories) return;

            foreach (var subDirectory in directories)
            {
                var temporalPath = System.IO.Path.Combine(destinationDirectoryName, subDirectory.Name);
                DirectoryCopy(subDirectory.FullName, temporalPath, true, overWriteData);
            }
        }
    }
}