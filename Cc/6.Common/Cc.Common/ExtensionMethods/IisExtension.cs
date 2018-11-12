using System;
using Cc.Common.LogHelper;
using Microsoft.Web.Administration;

namespace Cc.Common.ExtensionMethods
{
    public static class IisExtension
    {
        private static readonly ServerManager IisManager = new ServerManager();

        public static SiteCollection GetSitesList()
        {
            return IisManager.Sites;
        }

        public static Application GetApplicationFromSite(Site site, string virtualDirectoryName)
        {
            foreach (Application application in site.Applications)
            {
                if (application.Path.ToLower() == "/" + virtualDirectoryName.ToLower())
                {
                    return application;
                }
            }

            return null;
        }

        public static string GetVirtualDirectoryPath()
        {
            var virtualDirectoryPath = string.Empty;

            foreach (var site in IisManager.Sites)
            {
                foreach (var application in site.Applications)
                {
                    foreach (var virtualDirectory in application.VirtualDirectories)
                    {
                        virtualDirectoryPath = virtualDirectory.Path;
                    }
                }
            }

            return virtualDirectoryPath;
        }

        public static bool StartOrStopApplicationPool(bool isForStart, string applicationPool, int tries)
        {
            try
            {
                using (var manager = new ServerManager())
                {
                    var theApplicationPool = manager.ApplicationPools[applicationPool];

                    if (theApplicationPool == null) return false;

                    var isDone = false;

                    if (isForStart)
                    {
                        if (theApplicationPool.State == ObjectState.Stopped)
                        {
                            theApplicationPool.Start();
                        }
                    }
                    else
                    {
                        if (theApplicationPool.State == ObjectState.Started)
                        {
                            theApplicationPool.Stop();
                        }
                    }

                    var triesApplied = 0;

                    theApplicationPool = manager.ApplicationPools[applicationPool];

                    while (true)
                    {
                        if (isForStart)
                        {
                            switch (theApplicationPool.State)
                            {
                                case ObjectState.Starting:
                                    break;
                                case ObjectState.Started:
                                    isDone = true;
                                    break;
                                case ObjectState.Stopping:
                                    break;
                                case ObjectState.Stopped:
                                    break;
                                case ObjectState.Unknown:
                                    return false;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }
                        else
                        {
                            switch (theApplicationPool.State)
                            {
                                case ObjectState.Starting:
                                    break;
                                case ObjectState.Started:
                                    break;
                                case ObjectState.Stopping:
                                    break;
                                case ObjectState.Stopped:
                                    isDone = true;
                                    break;
                                case ObjectState.Unknown:
                                    return false;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }

                        triesApplied++;

                        if (isDone)
                            break;

                        if (triesApplied > tries)
                        {
                            Log.Instance.Info("Intentos para " + (isForStart ? "iniciar" : "detener") + " el pool de aplicación agotados");
                            return false;
                        }

                        theApplicationPool = manager.ApplicationPools[applicationPool];
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex);
                return false;
            }
        }
    }
}
