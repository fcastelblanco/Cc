using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using Cc.Common.ExtensionMethods;
using Cc.Common.Implementations.DataBaseHelper;
using Cc.Common.LogHelper;
using Cc.Upt.Business.Definitions;
using Cc.Upt.Business.Implementations.Singleton;
using Cc.Upt.Domain;
using Cc.Upt.Domain.Dto;
using Cc.Upt.Domain.Enumerations;
using Microsoft.Web.Administration;
using Microsoft.Win32;
using Path = Cc.Common.ExtensionMethods.Path;

namespace Cc.Upt.Business.Implementations
{
    public class UpdaterService : IUpdaterService
    {
        public const string UpdaterServiceName = "Isolucion servicio de autoactualización";
        private const string LastReleaseIsolucionParameter = "APP_isolucion_release";

        private const string WebFolderName = "Web";
        private const string WebFolderNameBu = "Web_Bu";

        private const string TemplatePathName = "Templates";
        private const string TemplatePathNameBu = "Templates_Bu";

        private const string LibreriaPathName = "Library";
        private const string LibreriaPathNameBu = "Library_Bu";

        private const string BancoConocimientoPathName = "BancoConocimiento";
        private const string BancoConocimientoPathNameBu = "BancoConocimiento_Bu";

        private const string BancoAnexoPathName = "BancoAnexo";
        private const string BancoAnexoPathNameBu = "BancoAnexo_Bu";

        private const string ServicesPathName = "Servicios";

        private const string SmartFlowPathName = "SmartFlow";
        private const string SmartFlowPathNameBu = "SmartFlow_Bu";

        private const string IsolucionServicioPathName = "IsolucionServicio";
        private const string IsolucionServicioPathNameBu = "IsolucionServicio_Bu";

        private const string GenericHandlerPathName = "GenericHandler";
        private const string GenericHandlerPathNameBu = "GenericHandler_Bu";

        private const string ScriptsCommitPathName = "ScriptsCommit";
        private const string ScriptsRollBackPathName = "ScriptsRollBack";
        private const string ScriptsPathName = "Scripts";

        private const int TriesForStopOrStartServices = 5;
        private readonly IGenericClientService _genericClientService;
        private readonly IParameterService _parameterService;

        public UpdaterService(IGenericClientService genericClientService, IParameterService parameterService)
        {
            _genericClientService = genericClientService;
            _parameterService = parameterService;
        }

        public void Execute()
        {
            var currentSiteList = IisExtension.GetSitesList();
            var servicesList = WindowsExtension.GetServices();
            var services = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services");

            if (services == null)
            {
                Log.Instance.Info(
                    "No fue posible obtener la información del registro de servicios del sistema operativo");
                return;
            }

            var ipmConfigurator =
                GetConfigurationFromXml<IpmConfiguratorDto>(AppDomain.CurrentDomain.BaseDirectory +
                                                            @"IpmConfigurator.xml");
            var companyList =
                GetConfigurationFromXml<ObservableCollection<CompanyMapperDto>>(
                    AppDomain.CurrentDomain.BaseDirectory + @"CompanyMapper.xml");

            ConfigurationUpdaterManager.Instance = new ConfigurationUpdaterManager
            {
                ApiUrl = ipmConfigurator.ApiUrl,
                UserName = ipmConfigurator.UserName,
                Password = ipmConfigurator.Password,
                CompanyId = ipmConfigurator.CompanyId
            };

            foreach (var companyMapperDto in companyList)
            {
                var updateResult = new Dictionary<UpdateVerification, List<Exception>>
                {
                    {UpdateVerification.DataBase, new List<Exception>()},
                    {UpdateVerification.Services, new List<Exception>()},
                    {UpdateVerification.Site, new List<Exception>()}
                };

                string thePathForUncompress;
                List<string> routesReplaced;

                if (ipmConfigurator.UpdateMode == UpdateMode.OnLine)
                {
                    Log.Instance.Info("Ejecutandose en modo online");

                    var dataRetrievedCompany = _genericClientService.Get<Company>(
                        "api/CompanyApi/GetCompanyByName/" + companyMapperDto.Name,
                        new Dictionary<string, string>
                        {
                            {"CompanyId", ConfigurationUpdaterManager.Instance.CompanyId.ToString().Base64Encode()}
                        });

                    if (dataRetrievedCompany == null)
                    {
                        Log.Instance.Info("No se encontró información de la compañia");
                        continue;
                    }

                    if (dataRetrievedCompany.Exception != null)
                    {
                        Log.Instance.Error(dataRetrievedCompany.Exception);
                        continue;
                    }

                    if (dataRetrievedCompany.Data == null)
                    {
                        Log.Instance.Info("No se encontró información de la compañia");
                        continue;
                    }

                    Log.Instance.Info("Empresa " + dataRetrievedCompany.Data.Name + " Id " +
                                      dataRetrievedCompany.Data.Id);

                    var companyUpdateList = _genericClientService.Get<IEnumerable<Release>>(
                        "api/CompanyApi/GetAvailableReleaseByCompanyId/" + dataRetrievedCompany.Data.Id,
                        new Dictionary<string, string>
                        {
                            {"CompanyId", ConfigurationUpdaterManager.Instance.CompanyId.ToString().Base64Encode()}
                        });

                    if (companyUpdateList == null)
                    {
                        Log.Instance.Info("No se encontró información de los release para la compañía " +
                                          dataRetrievedCompany.Data.Name);
                        continue;
                    }

                    if (companyUpdateList.Exception != null)
                    {
                        Log.Instance.Info("No se encontró información de los release para la compañía " +
                                          dataRetrievedCompany.Data.Name);

                        continue;
                    }

                    if (companyUpdateList.Data.Any())
                    {
                        var releaseList = (List<Release>)companyUpdateList.Data;

                        foreach (var release in releaseList)
                        {
                            routesReplaced = new List<string>();

                            if (!DowloadRelease(ipmConfigurator, release)) continue;

                            thePathForUncompress = Uncompress(ipmConfigurator, out _);
                            var directoriesOfDirectory = GetDirectoriesOfDirectory(thePathForUncompress);
                            Update(directoriesOfDirectory, routesReplaced, companyMapperDto, currentSiteList,
                                updateResult, servicesList, services, ipmConfigurator, release,
                                thePathForUncompress);

                            _genericClientService.Post<bool>(new CompanyUpdate
                            {
                                CompanyId = dataRetrievedCompany.Data.Id,
                                CreatedDate = DateTime.Now,
                                ReleaseId = release.Id,
                                Update = DateTime.Now
                            },
                                "api/CompanyApi/AddCompanyUpdate",
                                new Dictionary<string, string>
                                {
                                    {
                                        "CompanyId",
                                        ConfigurationUpdaterManager.Instance.CompanyId.ToString().Base64Encode()
                                    }
                                });
                        }
                    }
                    else
                    {
                        Log.Instance.Info("No se encontró información de los release para la compañía " +
                                          dataRetrievedCompany.Data.Name);
                    }
                }
                else
                {
                    routesReplaced = new List<string>();
                    thePathForUncompress = Uncompress(ipmConfigurator, out _);
                    var directoriesOfDirectory = GetDirectoriesOfDirectory(thePathForUncompress);
                    Update(directoriesOfDirectory, routesReplaced, companyMapperDto, currentSiteList, updateResult,
                        servicesList, services, ipmConfigurator, null, thePathForUncompress);
                }
            }
        }

        public T GetConfigurationFromXml<T>(string path)
        {
            return XmlExtension.GetDataFromXml<T>(path);
        }

        private void Update(IEnumerable<string> directoriesOfDirectory, ICollection<string> routesReplaced,
            CompanyMapperDto companyMapperDto,
            SiteCollection currentSiteList, IDictionary<UpdateVerification, List<Exception>> updateResult,
            List<ServiceController> servicesList, RegistryKey services,
            IpmConfiguratorDto ipmConfigurator, Release currentRelease, string thePathForUncompress)
        {
            Log.Instance.Info("Aplicando actualización a la empresa " + companyMapperDto.Name);

            var applicationPoolName = string.Empty;
            var listException = new List<Exception>();

            foreach (var directory in directoriesOfDirectory)
            {
                var currentDirectory = new DirectoryInfo(directory);
                Log.Instance.Info("Reemplazando la carpeta " + currentDirectory.Name);

                switch (currentDirectory.Name)
                {
                    case WebFolderName:
                    case WebFolderNameBu:

                        if (routesReplaced.Any(x => x == companyMapperDto.WebPath)) continue;
                        if (ManageApplicationPool(currentSiteList, companyMapperDto, ref applicationPoolName))
                        {
                            ManageFolder(currentDirectory, companyMapperDto.WebPath, companyMapperDto.Name,
                                routesReplaced,
                                listException);
                            updateResult[UpdateVerification.Site].AddRange(listException);

                            if (!string.IsNullOrEmpty(applicationPoolName))
                            {
                                Log.Instance.Info("Iniciando el pool de aplicación");

                                if (IisExtension.StartOrStopApplicationPool(true,
                                    applicationPoolName,
                                    TriesForStopOrStartServices))
                                    Log.Instance.Info("Pool " + applicationPoolName + " iniciado");
                            }
                        }
                        else
                        {
                            applicationPoolName = string.Empty;
                            listException.Add(new Exception(
                                "No se pudo detener el pool de la aplicación de la empresa " +
                                companyMapperDto.Name));
                        }

                        break;

                    case LibreriaPathName:
                    case LibreriaPathNameBu:

                        if (routesReplaced.Any(x => x == companyMapperDto.LibreriaPath)) continue;
                        ManageFolder(currentDirectory, companyMapperDto.LibreriaPath, companyMapperDto.Name,
                            routesReplaced,
                            listException);
                        updateResult[UpdateVerification.Site].AddRange(listException);

                        break;

                    case TemplatePathName:
                    case TemplatePathNameBu:

                        if (routesReplaced.Any(x => x == companyMapperDto.TemplatePath)) continue;
                        ManageFolder(currentDirectory, companyMapperDto.TemplatePath, companyMapperDto.Name,
                            routesReplaced,
                            listException);
                        updateResult[UpdateVerification.Site].AddRange(listException);

                        break;

                    case BancoConocimientoPathName:
                    case BancoConocimientoPathNameBu:

                        if (routesReplaced.Any(x => x == companyMapperDto.BancoConocimientoPath)) continue;
                        ManageFolder(currentDirectory, companyMapperDto.BancoConocimientoPath, companyMapperDto.Name,
                            routesReplaced, listException);
                        updateResult[UpdateVerification.Site].AddRange(listException);

                        break;

                    case BancoAnexoPathName:
                    case BancoAnexoPathNameBu:

                        if (routesReplaced.Any(x => x == companyMapperDto.BancoAnexoPath)) continue;
                        ManageFolder(currentDirectory, companyMapperDto.BancoAnexoPath, companyMapperDto.Name,
                            routesReplaced,
                            listException);
                        updateResult[UpdateVerification.Site].AddRange(listException);

                        break;

                    case GenericHandlerPathName:
                    case GenericHandlerPathNameBu:

                        if (routesReplaced.Any(x => x == companyMapperDto.GenericHandlerPath)) continue;

                        if (StopService(servicesList, services, companyMapperDto.SmartFlowPath,
                            out var exceptionSmartFlowStopping, out var serviceController))
                        {
                            ManageFolder(currentDirectory, companyMapperDto.GenericHandlerPath, companyMapperDto.Name,
                                routesReplaced,
                                listException);

                            while (true)
                                if (!WindowsExtension.StartService(serviceController.ServiceName))
                                {
                                    Log.Instance.Info("No se pudo iniciar el servicio " +
                                                      serviceController.ServiceName +
                                                      " se procederá a restaurar el backup de generic handler");
                                    RestoreBackUp(new DirectoryInfo(companyMapperDto.GenericHandlerPath));
                                    listException.Add(new Exception(
                                        "No se pudo iniciar el servicio " +
                                        serviceController.ServiceName));
                                }
                                else
                                {
                                    break;
                                }
                        }
                        else
                            listException.Add(new Exception(
                                "No se pudo detener el servicio smartflow de la empresa " + companyMapperDto.Name + " para poder reemplazar el Generic Handler",
                                exceptionSmartFlowStopping));

                        updateResult[UpdateVerification.Services].AddRange(listException);

                        break;

                    case ServicesPathName:
                        var servicesDirectory = Directory.GetDirectories(currentDirectory.FullName);

                        foreach (var serviceDirectory in servicesDirectory)
                        {
                            var currentServiceDirectory = new DirectoryInfo(serviceDirectory);

                            Log.Instance.Info("Reemplazando la carpeta " + currentServiceDirectory.Name);

                            switch (currentServiceDirectory.Name)
                            {
                                case SmartFlowPathName:
                                case SmartFlowPathNameBu:

                                    if (routesReplaced.Any(x => x == companyMapperDto.SmartFlowPath)) continue;

                                    ManageService(servicesList, services, companyMapperDto.SmartFlowPath,
                                        routesReplaced, currentServiceDirectory, listException,
                                        companyMapperDto.Name, out var serviceControllerSmartFlow);

                                    Log.Instance.Info("Iniciando servicio " + serviceControllerSmartFlow.ServiceName);

                                    while (true)
                                        if (!WindowsExtension.StartService(serviceControllerSmartFlow.ServiceName))
                                        {
                                            Log.Instance.Info("No se pudo iniciar el servicio " +
                                                              serviceControllerSmartFlow.ServiceName +
                                                              " se procederá a restaurar el backup");
                                            RestoreBackUp(new DirectoryInfo(companyMapperDto.SmartFlowPath));
                                            listException.Add(new Exception(
                                                "No se pudo iniciar el servicio " +
                                                serviceControllerSmartFlow.ServiceName));
                                        }
                                        else
                                        {
                                            break;
                                        }

                                    updateResult[UpdateVerification.Services].AddRange(listException);

                                    break;

                                case IsolucionServicioPathName:
                                case IsolucionServicioPathNameBu:

                                    if (routesReplaced.Any(x => x == companyMapperDto.IsolucionServicioPath))
                                        continue;

                                    ManageService(servicesList, services, companyMapperDto.IsolucionServicioPath,
                                        routesReplaced, currentServiceDirectory, listException,
                                        companyMapperDto.Name, out var serviceControllerIsolucionServicio);

                                    Log.Instance.Info("Iniciando servicio " +
                                                      serviceControllerIsolucionServicio.ServiceName);

                                    while (true)
                                        if (!WindowsExtension.StartService(serviceControllerIsolucionServicio
                                            .ServiceName))
                                        {
                                            Log.Instance.Info("No se pudo iniciar el servicio " +
                                                              serviceControllerIsolucionServicio.ServiceName +
                                                              " se procederá a restaurar el backup");
                                            RestoreBackUp(new DirectoryInfo(companyMapperDto.IsolucionServicioPath));
                                            listException.Add(new Exception(
                                                "No se pudo iniciar el servicio " +
                                                serviceControllerIsolucionServicio.ServiceName));
                                        }
                                        else
                                        {
                                            break;
                                        }

                                    updateResult[UpdateVerification.Services].AddRange(listException);

                                    break;
                            }
                        }

                        break;

                    case ScriptsCommitPathName:

                        Log.Instance.Info("Ejecutando scripts");

                        if (!ExecuteScriptsUpdate(currentDirectory, ipmConfigurator, currentRelease,
                            companyMapperDto, updateResult))
                            Log.Instance.Info(
                                "Ocurrieron problemas al intentar actualizar la base de datos del cliente " +
                                companyMapperDto.Name);

                        Log.Instance.Info("Scripts ejecutados");

                        break;
                }
            }

            Log.Instance.Info("Finalizando proceso");
            UpdateToLastVersion(currentRelease, companyMapperDto);
            FinishProcess(ipmConfigurator, thePathForUncompress);
        }

        private void UpdateToLastVersion(Release currentRelease, CompanyMapperDto companyMapperDto)
        {
            var licenceParameters = _parameterService.GetIsolucionParameterValues(
                new List<IsoluctionParameterDto>
                {
                    new IsoluctionParameterDto
                    {
                        ColumnOrField = "MOTOR_BASEDATOS",
                        LicenseParameter = LicenseParameter.DataBaseEngine
                    },

                    new IsoluctionParameterDto
                    {
                        ColumnOrField = "CADENA_CONEXION_SF",
                        LicenseParameter = LicenseParameter.ConnectionStringSf
                    },

                    new IsoluctionParameterDto
                    {
                        ColumnOrField = "SCHEMA_NAME",
                        LicenseParameter = LicenseParameter.SchemaName
                    }
                }, companyMapperDto.LicencePath);

            var dataBaseStrategy =
                licenceParameters.FirstOrDefault(x => x.Key == LicenseParameter.DataBaseEngine).Value
                    .ToUpper() == "ORACLE"
                    ? new StrategyToApply(new OracleManagement())
                    : new StrategyToApply(new SqlServerManagement());

            var connectionString = licenceParameters.FirstOrDefault(
                x => x.Key == LicenseParameter.ConnectionStringSf).Value;

            Log.Instance.Info("Buscando el parámetro: " + LastReleaseIsolucionParameter);
            const string theQuery = "select valor from parametro where nomparametro like'%" +
                                    LastReleaseIsolucionParameter + "%'";

            var dataRetrieved = dataBaseStrategy.ExcecuteSelect<object>(theQuery,
                new Dictionary<string, object>(),
                connectionString,
                out var ex);

            if (dataRetrieved == null)
            {
                Log.Instance.Info("No fue posible recopilar información de la base de datos");
                Log.Instance.Error(ex);
                return;
            }

            if (dataRetrieved.Any())
            {
                Log.Instance.Info("Acutalizando el parámetro: " + LastReleaseIsolucionParameter);

                if (dataBaseStrategy.ExcecuteQuery(
                    "update parametro set valor = '" + currentRelease.Id + "' where nomparametro like '%" +
                    LastReleaseIsolucionParameter + "%'",
                    new Dictionary<string, object>(),
                    connectionString,
                    out var exUpdate)) return;

                Log.Instance.Info("No fue posible actualizar el parámetro " + LastReleaseIsolucionParameter +
                                  " al valor de " + currentRelease.Id);
                Log.Instance.Error(exUpdate);
            }
            else
            {
                Log.Instance.Info("Insertando el parámetro: " + LastReleaseIsolucionParameter);

                if (!dataBaseStrategy.ExcecuteQuery(
                    @"insert into " + licenceParameters.FirstOrDefault(
                        x => x.Key == LicenseParameter.SchemaName).Value + @".parametro 
                    (
                        id_parametro,
                        nomparametro,
                        valor,
                        codwebsite,
                        codcategoriaparametro,
                        tipo,
                        valorpordefecto,
                        opciones,
                        systemprotected,
                        descripcioncorta,
                        descripcionlarga,
                        activo
                    )
                    values
                    (
                        (select max(Id_Parametro) + 1 from parametro),
                        'APP_isolucion_release',
                        '',
                        1,
                        1,
                        'Input',
                        '',
                        NULL,
                        1,
                        'Id del release actual de isolución',
                        'Id del release actual de isolución',
                        1
                    )",
                    new Dictionary<string, object>(),
                    connectionString,
                    out var exInsert))
                {
                    Log.Instance.Info("No fue posible crear el parámetro " + LastReleaseIsolucionParameter);
                    Log.Instance.Error(exInsert);
                    return;
                }

                Log.Instance.Info("Acutalizando el parámetro: " + LastReleaseIsolucionParameter);

                if (dataBaseStrategy.ExcecuteQuery(
                    "update parametro set valor = '" + currentRelease.Id + "' where nomparametro like '%" +
                    LastReleaseIsolucionParameter + "%'",
                    new Dictionary<string, object>(),
                    connectionString,
                    out var exUpdate)) return;

                Log.Instance.Info("No fue posible actualizar el parámetro " + LastReleaseIsolucionParameter +
                                  " al valor de " + currentRelease.Id);
                Log.Instance.Error(exUpdate);
            }
        }

        private static IEnumerable<string> GetDirectoriesOfDirectory(string thePathForUncompress)
        {
            var dataFiles = Directory.GetFiles(thePathForUncompress, "*.zip");
            var directoriesOfDirectory = Directory.GetDirectories(thePathForUncompress);

            foreach (var directory in directoriesOfDirectory)
                WindowsExtension.DeleteFolderRecursive(new DirectoryInfo(directory));

            foreach (var dataFile in dataFiles)
            {
                var theFileInfoDataFile = new FileInfo(dataFile);
                ZipFile.ExtractToDirectory(theFileInfoDataFile.FullName, theFileInfoDataFile.DirectoryName);
            }

            directoriesOfDirectory = Directory.GetDirectories(thePathForUncompress);
            return directoriesOfDirectory;
        }

        private static void ManageFolder(FileSystemInfo currentDirectory, string path, string companyName,
            ICollection<string> routesReplaced,
            ICollection<Exception> listException)
        {
            if (currentDirectory.Name.EndsWith("Bu"))
            {
                Log.Instance.Info("Creando back up de la carpeta " + currentDirectory.FullName);
                CreateDirectoryBackUp(path);
            }

            Log.Instance.Info("Reemplazando contenido de la carpeta " + path);
            if (CopyContent(path, routesReplaced, currentDirectory,
                out var exception)) return;

            listException.Add(new Exception(
                "No se pudo reemplazar el contenido de la carpeta " +
                currentDirectory.FullName + " de la empresa " + companyName,
                exception));

            if (!currentDirectory.Name.EndsWith("Bu")) return;

            Log.Instance.Info("Restaurando la carpeta " + path);
            RestoreBackUp(new DirectoryInfo(path));
        }

        private static void RestoreBackUp(DirectoryInfo currentDirectory)
        {
            if (currentDirectory == null)
            {
                Log.Instance.Info("El directorio a restaurar no puede ser nulo");
                return;
            }

            if (currentDirectory.Parent == null)
            {
                Log.Instance.Info("No se encuentra el directorio padre del directorio " + currentDirectory.FullName);
                return;
            }

            var directoryExpected = Directory
                .GetDirectories(currentDirectory.Parent.FullName)
                .Select(x => new DirectoryInfo(x))
                .Where(x => x.Name.ToLower().Contains(currentDirectory.Name.ToLower()))
                .OrderByDescending(x => x.CreationTime).FirstOrDefault();

            if (directoryExpected == null)
            {
                Log.Instance.Info("No se encontró último backup");
                return;
            }

            WindowsExtension.DirectoryCopy(directoryExpected.FullName, currentDirectory.FullName, true, true);
        }

        private static void CreateDirectoryBackUp(string directoryToBackUp)
        {
            WindowsExtension.DirectoryCopy(directoryToBackUp,
                directoryToBackUp + "_" + DateTime.Now.ToString("ddMMyyyy"), true, true);
        }

        private static void ManageService(IEnumerable<ServiceController> servicesList, RegistryKey services,
            string path,
            ICollection<string> routesReplaced, FileSystemInfo currentServiceDirectory,
            ICollection<Exception> listException, string companyName, out ServiceController serviceController)
        {
            serviceController = null;

            if (StopService(servicesList, services, path,
                out var exceptionSmartFlowStopping, out serviceController))
                ManageFolder(currentServiceDirectory, path, companyName, routesReplaced, listException);
            else
                listException.Add(new Exception(
                    "No se pudo detener el servicio smartflow de la empresa " + companyName,
                    exceptionSmartFlowStopping));
        }

        private static bool StopService(IEnumerable<ServiceController> servicesList, RegistryKey services, string path,
            out Exception exception, out ServiceController serviceController)
        {
            try
            {
                Log.Instance.Info("Ruta a encontrar: " + path);

                exception = null;
                serviceController = null;

                foreach (var serviceControllerItem in servicesList)
                {
                    var serviceRegistryDefinition = services.OpenSubKey(serviceControllerItem.ServiceName);
                    if (serviceRegistryDefinition == null) continue;
                    var servicePath = (string)serviceRegistryDefinition.GetValue("ImagePath");
                    try
                    {
                        Log.Instance.Info("Ruta actual: " + servicePath);

                        if (servicePath.StartsWith(((char)34).ToString()) &&
                            servicePath.EndsWith(((char)34).ToString()))
                            servicePath = servicePath.Substring(1, servicePath.Length - 2);

                        var serviceRealPath = new FileInfo(servicePath);

                        if (serviceRealPath.Directory == null)
                            continue;

                        if (string.IsNullOrEmpty(serviceRealPath.Directory.FullName)) continue;

                        if (path != serviceRealPath.Directory.FullName) continue;

                        Log.Instance.Info("Ruta encontrada: " + serviceRealPath.Directory.FullName);

                        if (WindowsExtension.StopService(serviceControllerItem.ServiceName, out var exceptionService))
                        {
                            serviceController = serviceControllerItem;
                            Log.Instance.Info(serviceControllerItem.ServiceName + " detenido");
                            return true;
                        }

                        exception = exceptionService;
                    }
                    catch (Exception e)
                    {
                        exception = e;
                        Log.Instance.Error(e);
                        serviceController = null;
                    }
                }

                return false;
            }
            catch (Exception e)
            {
                exception = e;
                Log.Instance.Error(e);
                serviceController = null;
                return false;
            }
        }

        private static bool ManageApplicationPool(SiteCollection currentSiteList, CompanyMapperDto companyMapperDto,
            ref string applicationPoolName)
        {
            var currentSite =
                currentSiteList.FirstOrDefault(x =>
                    string.Equals(x.Name, companyMapperDto.Name, StringComparison.CurrentCultureIgnoreCase));

            Log.Instance.Info("Buscando sitio con el nombre " +
                              companyMapperDto.Name);

            if (currentSite == null)
                Log.Instance.Info("No se encontró sitio con el siguiente nombre " +
                                  companyMapperDto.Name);

            if (currentSite != null)
            {
                Log.Instance.Info("Deteniendo el pool de aplicación");
                applicationPoolName = currentSite.ApplicationDefaults.ApplicationPoolName;
                if (!IisExtension.StartOrStopApplicationPool(false, applicationPoolName,
                    TriesForStopOrStartServices)) return false;

                Log.Instance.Info("Pool " + currentSite.ApplicationDefaults.ApplicationPoolName + " detenido");
                return true;
            }

            Log.Instance.Info("Buscando directorio virtual con el nombre " +
                              companyMapperDto.Name);

            foreach (var dataCurrentSite in currentSiteList)
            {
                var dataApplicatonRetrieved =
                    IisExtension.GetApplicationFromSite(dataCurrentSite, companyMapperDto.Name);

                if (dataApplicatonRetrieved == null) continue;

                applicationPoolName = dataApplicatonRetrieved.ApplicationPoolName;
                Log.Instance.Info("Deteniendo el pool de aplicación");

                if (!IisExtension.StartOrStopApplicationPool(false, applicationPoolName,
                    TriesForStopOrStartServices)) return false;

                Log.Instance.Info("Pool " + dataApplicatonRetrieved.ApplicationPoolName + " detenido");
                return true;
            }

            Log.Instance.Info("No se encontró directorio virtual con el nombre " +
                              companyMapperDto.Name);
            return false;
        }

        private bool ExecuteScriptsUpdate(FileSystemInfo currentDirectory, IpmConfiguratorDto ipmConfigurator,
            Release currentRelease, CompanyMapperDto companyMapperDto,
            IDictionary<UpdateVerification, List<Exception>> listException)
        {
            var parentDirectory = new DirectoryInfo(currentDirectory.FullName).Parent;

            if (parentDirectory == null)
                return false;

            var scriptsDirectories = Directory.GetDirectories(parentDirectory.FullName);
            var scriptsCommitInPath = new List<FileInfo>();
            var scriptsRollbackInPath = new List<FileInfo>();

            if (scriptsDirectories.Any(x => string.Equals(new DirectoryInfo(x).Name, ScriptsCommitPathName,
                    StringComparison.CurrentCultureIgnoreCase))
                &&
                scriptsDirectories.Any(x => string.Equals(new DirectoryInfo(x).Name, ScriptsRollBackPathName,
                    StringComparison.CurrentCultureIgnoreCase)))
            { 

                if (Directory.Exists(parentDirectory.FullName + @"\" + ScriptsCommitPathName))
                {
                    scriptsCommitInPath = Directory
                        .GetFiles(parentDirectory.FullName + @"\" + ScriptsCommitPathName, "*.sql")
                        .Select(x => new FileInfo(x)).OrderBy(x => Convert.ToInt32(x.Name)).ToList();
                }

                if (Directory.Exists(parentDirectory.FullName + @"\" + ScriptsRollBackPathName))
                {
                    scriptsRollbackInPath = Directory
                        .GetFiles(parentDirectory.FullName + @"\" + ScriptsRollBackPathName, "*.sql")
                        .Select(x => new FileInfo(x)).OrderBy(x => Convert.ToInt32(x.Name)).ToList();
                }

                if (scriptsCommitInPath.Count == 0 &&
                    scriptsRollbackInPath.Count == 0)
                    return true;

                if (scriptsCommitInPath.Count != scriptsRollbackInPath.Count)
                {
                    Log.Instance.Info(
                        "La cantidad de archivos de la carpeta scripts commit no es igual en cantidad que en la carpeta de scripts rollback. El release fue marcado como no seguro");

                    listException[UpdateVerification.DataBase].Add(new Exception(
                        "La cantidad de archivos de la carpeta scripts commit no es igual en cantidad que en la carpeta de scripts rollback. El release fue marcado como no seguro"));

                    if (ipmConfigurator.UpdateMode != UpdateMode.OnLine) return false;

                    SetReleaseSafe(new Release
                    {
                        IsSafe = false,
                        Id = currentRelease.Id
                    });
                    return false;
                }
            }

            var licenceParameters = _parameterService.GetIsolucionParameterValues(
                new List<IsoluctionParameterDto>
                {
                    new IsoluctionParameterDto
                    {
                        ColumnOrField = "MOTOR_BASEDATOS",
                        LicenseParameter = LicenseParameter.DataBaseEngine
                    },

                    new IsoluctionParameterDto
                    {
                        ColumnOrField = "CADENA_CONEXION_SF",
                        LicenseParameter = LicenseParameter.ConnectionStringSf
                    }
                }, companyMapperDto.LicencePath);

            var dataBaseStrategy =
                licenceParameters.FirstOrDefault(x => x.Key == LicenseParameter.DataBaseEngine).Value
                    .ToUpper() == "ORACLE"
                    ? new StrategyToApply(new OracleManagement())
                    : new StrategyToApply(new SqlServerManagement());

            var connectionString = licenceParameters.FirstOrDefault(
                x => x.Key == LicenseParameter.ConnectionStringSf).Value;

            var scriptsExecuted = new List<string>();

            foreach (var script in scriptsCommitInPath)
            {
                var realScriptData = File.ReadAllText(script.FullName);
                var theExceptionList = new List<Exception>();

                if (!dataBaseStrategy.ExcecuteQuery(realScriptData, null, connectionString, out var exception))
                {
                    Log.Instance.Info(
                        "El script " + script.Name + " no se ejecutó correctamente. Se inicia rollback de scripts");
                    theExceptionList.Add(new Exception("El script " + script.Name + " no se ejecutó correctamente",
                        exception));

                    foreach (var item in scriptsExecuted)
                    {
                        if (string.IsNullOrEmpty(item)) continue;

                        var currentFile = new FileInfo(item);

                        var scriptForRollBack =
                            scriptsRollbackInPath.FirstOrDefault(x => x.Name == currentFile.Name);

                        if (scriptForRollBack == null)
                        {
                            Log.Instance.Info("El script " + currentFile.Name +
                                              " no se encuentra dentro de la carpeta scripts rollback");
                            theExceptionList.Add(new Exception("El script " + script.Name + " no existe"));
                            continue;
                        }

                        realScriptData = File.ReadAllText(scriptForRollBack.FullName);

                        if (dataBaseStrategy.ExcecuteQuery(realScriptData, null, connectionString,
                            out exception)) continue;

                        Log.Instance.Info("El script " + scriptForRollBack.Name +
                                          " no se ejecutó correctamente");
                        theExceptionList.Add(new Exception("El script " + script.Name + " no se ejecutó correctamente",
                            exception));
                    }

                    SetReleaseSafe(new Release
                    {
                        IsSafe = false,
                        Id = currentRelease.Id
                    });

                    listException[UpdateVerification.DataBase].AddRange(theExceptionList);
                    return false;
                }

                scriptsExecuted.Add(script.FullName);
            }

            return true;
        }

        private static bool CopyContent(string pathExpected, ICollection<string> routesReplaced,
            FileSystemInfo currentDirectory, out Exception exception)
        {
            const int numberOfRetries = 3;
            var retries = 0;
            const int delayOnRetry = 3000;
            exception = null;

            while (retries < numberOfRetries)
                try
                {
                    if (string.IsNullOrEmpty(pathExpected)) return false;

                    routesReplaced.Add(pathExpected);
                    WindowsExtension.DirectoryCopy(currentDirectory.FullName, pathExpected, true, true);
                    return true;
                }
                catch (IOException e)
                {
                    exception = e;
                    Log.Instance.Error(e);
                    retries++;
                    Thread.Sleep(delayOnRetry);
                }

            return false;
        }

        private void SetReleaseSafe(Release currentRelease)
        {
            var releaseAsNotSafe = _genericClientService.Post<bool>(currentRelease, "api/ReleaseApi/SetReleaseAsSafe/",
                new Dictionary<string, string>
                {
                    {"CompanyId", ConfigurationUpdaterManager.Instance.CompanyId.ToString().Base64Encode()}
                });

            if (releaseAsNotSafe == null)
            {
                Log.Instance.Info("El release con versión " + currentRelease.Version +
                                  " no pudo ser marcado cómo no seguro");
            }
            else
            {
                if (releaseAsNotSafe.Exception != null)
                {
                    Log.Instance.Error(releaseAsNotSafe.Exception);
                }
                else
                {
                    if (!releaseAsNotSafe.Data)
                        Log.Instance.Info("El release con versión " + currentRelease.Version +
                                          " no pudo ser marcado cómo no seguro");
                    else
                        Log.Instance.Info("El release con versión " + currentRelease.Version +
                                          " fue marcado cómo no seguro");
                }
            }
        }

        private static void FinishProcess(IpmConfiguratorDto ipmConfigurator, string thePathForUncompress)
        {
            try
            {
                var dataZilFileList = Directory.GetFiles(ipmConfigurator.ReleasePath, "*.zip")
                    .Select(x => new FileInfo(x)).ToList();

                var theZipFile = dataZilFileList.OrderByDescending(x => x.CreationTime).FirstOrDefault();

                if (theZipFile != null)
                {
                    var thePathForMoveFiles = AppDomain.CurrentDomain.BaseDirectory + @"ZipFilesAnalized\";

                    Path.CreateDirectoryRecursively(thePathForMoveFiles);

                    var theContentReleaseToMove = AppDomain.CurrentDomain.BaseDirectory + @"ReleasesAnalized\" +
                                                  System.IO.Path.GetFileNameWithoutExtension(theZipFile.FullName) +
                                                  "_" +
                                                  DateTime.Now.ToString("ddMMyyyy");

                    Path.CreateDirectoryRecursively(theContentReleaseToMove);

                    var filesPathForMove = Directory.GetFiles(thePathForMoveFiles);

                    foreach (var file in filesPathForMove) File.Delete(file);

                    Directory.Delete(theContentReleaseToMove, true);

                    Log.Instance.Info("Moviendo archivos comprimidos");

                    File.Move(theZipFile.FullName,
                        thePathForMoveFiles +
                        System.IO.Path.GetFileNameWithoutExtension(theZipFile.FullName) + "_" +
                        DateTime.Now.ToString("ddMMyyyy") + ".zip");

                    Log.Instance.Info("Moviendo carpeta del release");

                    WindowsExtension.DirectoryCopy(thePathForUncompress, theContentReleaseToMove, true);
                }
                else
                {
                    Log.Instance.Info("No se pudo mover los archivos analizados");
                }
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex);
            }
        }

        private static string Uncompress(IpmConfiguratorDto ipmConfiguratorDto, out FileInfo fileInfo)
        {
            Log.Instance.Info("Obteniendo comprimidos disponibles en la carpeta " + ipmConfiguratorDto.ReleasePath);

            fileInfo = null;
            var fileListCompressed = Directory.GetFiles(ipmConfiguratorDto.ReleasePath, "*.zip")
                .Select(x => new FileInfo(x)).ToList();

            Log.Instance.Info("Obteniendo el último archivo");

            var lastFile = fileListCompressed.OrderByDescending(x => x.CreationTime).FirstOrDefault();

            if (lastFile == null)
            {
                Log.Instance.Info("No fue posible obtener información del los release disponibles");
                return string.Empty;
            }

            var uncompressedFilePath = AppDomain.CurrentDomain.BaseDirectory + @"UncompressedFiles\" +
                                       System.IO.Path.GetFileNameWithoutExtension(lastFile.Name);

            try
            {
                Log.Instance.Info("Preparando ruta de descompresión");

                if (!Directory.Exists(uncompressedFilePath)) Directory.CreateDirectory(uncompressedFilePath);

                var currentFilesDirectory = Directory.GetFiles(uncompressedFilePath);

                foreach (var file in currentFilesDirectory) File.Delete(file);

                Log.Instance.Info("Descomprimiendo archivo");
                ZipFile.ExtractToDirectory(lastFile.FullName, uncompressedFilePath);

                fileInfo = lastFile;
                Log.Instance.Info("Archivo descromprimido");

                return uncompressedFilePath;
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex);
                return string.Empty;
            }
        }

        private bool DowloadRelease(IpmConfiguratorDto ipmConfiguratorDto, Release release)
        {
            if (!release.IsSafe)
            {
                Log.Instance.Error(new Exception("El release actual no es seguro"));
                return false;
            }

            while (true)
            {
                Log.Instance.Info("Obteniendo información de release para descarga");

                var currentRequestRelease = _genericClientService.Get<DownloadRequestRelease>(
                    "api/ReleaseApi/GetDownloadRequestReleaseByReleaseId/" + release.Id,
                    new Dictionary<string, string>
                    {
                        {"CompanyId", ConfigurationUpdaterManager.Instance.CompanyId.ToString().Base64Encode()}
                    });

                if (currentRequestRelease == null)
                {
                    Log.Instance.Error(new Exception("No fue posible obtener la información del release"));
                    return false;
                }

                if (currentRequestRelease.Data == null)
                {
                    if (currentRequestRelease.Exception != null)
                        Log.Instance.Error(currentRequestRelease.Exception);
                    else
                        Log.Instance.Info("No se encontraró información del release para descargar");

                    Log.Instance.Info("Solicitando creación de release para descarga");

                    var dataDownloadRequestRelease = _genericClientService.Post<bool>(new DownloadRequestRelease
                    {
                        DownloadRequestReleaseStatusType = DownloadRequestReleaseStatusType.RequestedForCreate,
                        ReleaseId = release.Id
                    }, "api/ReleaseApi/CreateDownloadRequestRelease/",
                        new Dictionary<string, string>
                        {
                            {"CompanyId", ConfigurationUpdaterManager.Instance.CompanyId.ToString().Base64Encode()}
                        });

                    if (dataDownloadRequestRelease.Data)
                    {
                        Log.Instance.Info("Obteniendo información de release para descarga");

                        currentRequestRelease = _genericClientService.Get<DownloadRequestRelease>(
                            "api/ReleaseApi/GetDownloadRequestReleaseByReleaseId/" + release.Id,
                            new Dictionary<string, string>
                            {
                                {"CompanyId", ConfigurationUpdaterManager.Instance.CompanyId.ToString().Base64Encode()}
                            });
                    }
                    else
                    {
                        if (dataDownloadRequestRelease.Exception == null)
                            Log.Instance.Info(
                                "No fue posible crear una solicitud de creación de release para descargar");
                        else
                            Log.Instance.Error(dataDownloadRequestRelease.Exception);
                    }
                }

                var finisLoop = false;

                if (currentRequestRelease == null)
                {
                    Log.Instance.Info("No se encontró información del release para descargar");
                    return false;
                }

                if (currentRequestRelease.Data == null)
                {
                    Log.Instance.Info("No se encontró información del release para descargar");
                    return false;
                }

                Log.Instance.Info("Verificando estado de solicitud de descarga de archivos");

                switch (currentRequestRelease.Data.DownloadRequestReleaseStatusType)
                {
                    case DownloadRequestReleaseStatusType.Creating:
                        Log.Instance.Info("El release con Id " + release.Id + " y versión " +
                                          release.Version + " se encuentra en proceso de creación del paquete");
                        Thread.Sleep(TimeSpan.FromMinutes(ipmConfiguratorDto.IntervalExecution));
                        break;
                    case DownloadRequestReleaseStatusType.Ready:
                        Log.Instance.Info("El release con Id " + release.Id + " y versión " +
                                          release.Version + " se encuentra en listo para descarga");

                        Log.Instance.Info("Actualizando información del release a disponible");
                        var updatedDataRetrievedRequestRelease = _genericClientService.Post<bool>(
                            new DownloadRequestRelease
                            {
                                Id = currentRequestRelease.Data.Id,
                                DownloadRequestReleaseStatusType = DownloadRequestReleaseStatusType.Available
                            }, "api/ReleaseApi/UpdateDownloadRequestRelease/",
                            new Dictionary<string, string>
                            {
                                {"CompanyId", ConfigurationUpdaterManager.Instance.CompanyId.ToString().Base64Encode()}
                            });

                        if (!updatedDataRetrievedRequestRelease.Data)
                            Log.Instance.Info("El request release con Id " + currentRequestRelease.Data.Id +
                                              " no pudo ser actualizado");

                        finisLoop = true;
                        break;
                    case DownloadRequestReleaseStatusType.Using:
                        break;
                    case DownloadRequestReleaseStatusType.Available:
                        Log.Instance.Info("El release con Id " + release.Id + " y versión " +
                                          release.Version + " se encuentra en disponible para descarga");

                        var increaseDownloadRequestReleaseDate = _genericClientService.Post<bool>(
                            new DownloadRequestRelease
                            {
                                Id = currentRequestRelease.Data.Id,
                                CreatedDate = DateTime.Now
                            }, "api/ReleaseApi/IncreaseDownloadRequestReleaseDate/",
                            new Dictionary<string, string>
                            {
                                {"CompanyId", ConfigurationUpdaterManager.Instance.CompanyId.ToString().Base64Encode()}
                            });

                        if (increaseDownloadRequestReleaseDate == null)
                        {
                            Log.Instance.Info("No fue posible incrementar la disponibilidad del release " +
                                              release.Version);
                        }
                        else
                        {
                            if (increaseDownloadRequestReleaseDate.Exception != null)
                            {
                                Log.Instance.Error(increaseDownloadRequestReleaseDate.Exception);
                                Log.Instance.Info("No fue posible incrementar la disponibilidad del release " +
                                                  release.Version);
                            }
                            else
                            {
                                if (increaseDownloadRequestReleaseDate.Data)
                                {
                                    finisLoop = increaseDownloadRequestReleaseDate.Data;
                                    Log.Instance.Info("Disponibilidad del release " + release.Version +
                                                      " incrementado");
                                }
                                else
                                {
                                    Log.Instance.Info("No fue posible incrementar la disponibilidad del release " +
                                                      release.Version);
                                }
                            }
                        }

                        break;
                    case DownloadRequestReleaseStatusType.RequestedForCreate:
                        Log.Instance.Info("El release con Id " + release.Id + " y versión " +
                                          release.Version +
                                          " se encuentra en cola para crear y realizar su posterior descarga");
                        Thread.Sleep(TimeSpan.FromMinutes(ipmConfiguratorDto.IntervalExecution));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (finisLoop)
                    break;
            }

            Log.Instance.Info("Preparando directorio de ubicación de descargas");
            Path.CreateDirectoryRecursively(ipmConfiguratorDto.ReleasePath);

            var ftpServer = ipmConfiguratorDto.FtpPath;
            var ftpUserName = ipmConfiguratorDto.FtpUserName;
            var ftpUserPassword = ipmConfiguratorDto.FtpPassword;

            Log.Instance.Info("Verificando parámetros de conexión de ftp");

            if (string.IsNullOrEmpty(ftpServer) || string.IsNullOrEmpty(ftpUserName) ||
                string.IsNullOrEmpty(ftpUserPassword))
            {
                var dataParameters =
                    _genericClientService.Get<List<Parameter>>("api/ParameterApi/GetAllParameterList",
                        new Dictionary<string, string>
                        {
                            {"CompanyId", ConfigurationUpdaterManager.Instance.CompanyId.ToString().Base64Encode()}
                        });

                if (dataParameters == null)
                {
                    Log.Instance.Info("No fue posible obtener información de los parámetros");
                    return false;
                }

                if (dataParameters.Exception != null)
                {
                    Log.Instance.Error(dataParameters.Exception);
                    return false;
                }

                if (dataParameters.Data == null)
                {
                    Log.Instance.Info("No fue posible obtener información de los parámetros");
                    return false;
                }

                var ftpServerParameter = dataParameters.Data.FirstOrDefault(x =>
                    x.ParameterInternalIdentificator == ParameterInternalIdentificator.FtpPath);
                var ftpUserNameParameter = dataParameters.Data.FirstOrDefault(x =>
                    x.ParameterInternalIdentificator == ParameterInternalIdentificator.FtpUser);
                var ftpUserPasswordParameter = dataParameters.Data.FirstOrDefault(x =>
                    x.ParameterInternalIdentificator == ParameterInternalIdentificator.FtpPassword);

                if (ftpServerParameter == null
                    ||
                    ftpUserNameParameter == null
                    ||
                    ftpUserPasswordParameter == null)
                {
                    Log.Instance.Info("No se encuentran configurados los parámetros de ftp");
                    return false;
                }

                ftpServer = ftpServerParameter.Value;
                ftpUserName = ftpUserNameParameter.Value;
                ftpUserPassword = ftpUserPasswordParameter.Value;

                if (string.IsNullOrEmpty(ftpServer) || string.IsNullOrEmpty(ftpUserName) ||
                    string.IsNullOrEmpty(ftpUserPassword))
                {
                    Log.Instance.Info("No se encuentran configurados los parámetros de ftp o está sin valor asignado");
                    return false;
                }
            }

            Log.Instance.Info("Descargando el release " + release.Version);

            if (FtpExtension.Download(release.Version + ".zip", ftpServer, ftpUserName, ftpUserPassword,
                ipmConfiguratorDto.BufferSize, ipmConfiguratorDto.ReleasePath))
            {
                Log.Instance.Info("Release " + release.Version + " descargado");
                return true;
            }

            Log.Instance.Info("El release " + release.Version + " no pudo ser descargado");
            return false;
        }
    }
}