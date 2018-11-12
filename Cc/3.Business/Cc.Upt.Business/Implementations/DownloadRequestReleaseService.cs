using System;
using System.IO;
using System.Linq;
using Cc.Common.Enumerations;
using Cc.Common.LogHelper;
using Cc.Upt.Business.Definitions;
using Cc.Upt.Data.Definitions;
using Cc.Upt.Data.Implementations;
using Cc.Upt.Domain;
using Cc.Upt.Domain.Enumerations;
using Path = Cc.Common.ExtensionMethods.Path;

namespace Cc.Upt.Business.Implementations
{
    public class DownloadRequestReleaseService : EntityService<DownloadRequestRelease>, IDownloadRequestReleaseService
    {
        private readonly IParameterService _parameterService;
        private readonly IReleaseService _releaseService;

        public DownloadRequestReleaseService(IContext context, IParameterService parameterService,
            IReleaseService releaseService) : base(context)
        {
            Dbset = context.Set<DownloadRequestRelease>();
            _parameterService = parameterService;
            _releaseService = releaseService;
        }

        public bool CreateDownloadRequestRelease(DownloadRequestRelease downloadRequestRelease)
        {
            try
            {
                downloadRequestRelease.Id = Guid.NewGuid();
                Create(downloadRequestRelease);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ExecuteRequestReleaseCreator()
        {
            Log.Instance.Info("Proceso de creación de releases para descargas iniciado");
            Log.Instance.Info("Revisando releases solicitados para descarga");

            var dataRetrievedRequestedForCreateList = FindBy(x =>
                x.DownloadRequestReleaseStatusType == DownloadRequestReleaseStatusType.RequestedForCreate).ToList();
            var parameterLocalPathForDownloadFiles =
                _parameterService.GetParameterValueByInternalIdentificator<string>(ParameterInternalIdentificator
                    .LocalPathForDownloadFiles);

            if (string.IsNullOrEmpty(parameterLocalPathForDownloadFiles))
            {
                Log.Instance.Info("El parámetro " +
                                  ParameterInternalIdentificator.LocalPathForDownloadFiles.GetDescription() +
                                  " no se encuentra configurado");
                return;
            }

            Path.CreateDirectoryRecursively(parameterLocalPathForDownloadFiles);

            Log.Instance.Info("Creando release solicitados para descarga");

            foreach (var item in dataRetrievedRequestedForCreateList)
            {
                item.DownloadRequestReleaseStatusType = DownloadRequestReleaseStatusType.Creating;
                Update(item);                

                var currentReleaseData = _releaseService.GetReleaseById(item.ReleaseId);

                if (currentReleaseData == null)
                {
                    Log.Instance.Info("No se encontró el release con Id: " + item.ReleaseId);
                    return;
                }

                if (currentReleaseData.ReleaseContent.Length == 0)
                {
                    Log.Instance.Info("El release con Id: " + item.ReleaseId + " no tiene contenido descargable");
                    return;
                }

                try
                {                    
                    File.WriteAllBytes(parameterLocalPathForDownloadFiles + @"\" + currentReleaseData.Version + ".zip",
                        currentReleaseData.ReleaseContent);
                }
                catch (Exception e)
                {
                    Log.Instance.Error(e);
                    return;
                }

                item.DownloadRequestReleaseStatusType = DownloadRequestReleaseStatusType.Ready;
                Update(item);
            }

            Log.Instance.Info("Revisando existencia de releases que deberían estar disponibles");

            var dataRetrievedRequestedForVailableList = FindBy(x =>
                x.DownloadRequestReleaseStatusType == DownloadRequestReleaseStatusType.Available).ToList();

            foreach (var item in dataRetrievedRequestedForVailableList)
            {                
                var currentReleaseData = _releaseService.GetReleaseById(item.ReleaseId);

                if (currentReleaseData == null)
                {
                    Log.Instance.Info("No se encontró el release con Id: " + item.ReleaseId);
                    return;
                }

                if (currentReleaseData.ReleaseContent.Length == 0)
                {
                    Log.Instance.Info("El release con Id: " + item.ReleaseId + " no tiene contenido descargable");
                    return;
                }

                try
                {
                    if (File.Exists(parameterLocalPathForDownloadFiles + @"\" + currentReleaseData.Version + ".zip"))
                    {
                        continue;
                    }

                    Path.CreateDirectoryRecursively(parameterLocalPathForDownloadFiles);
                    File.WriteAllBytes(parameterLocalPathForDownloadFiles + @"\" + currentReleaseData.Version + ".zip",
                        currentReleaseData.ReleaseContent);
                }
                catch (Exception e)
                {
                    Log.Instance.Error(e);
                    return;
                }               
            }

            Log.Instance.Info("Revisando releases disponibles expirados");

            var dataRetrievedAvailableExpired =
                FindBy(x => x.DownloadRequestReleaseStatusType == DownloadRequestReleaseStatusType.Available).ToList();
            var parameterValue =
                _parameterService.GetParameterValueByInternalIdentificator<int>(ParameterInternalIdentificator
                    .ReleaseDaysExpiration);

            Log.Instance.Info("Eliminando releases expirados");

            foreach (var item in dataRetrievedAvailableExpired)
            {
                var currentDifference = (DateTime.Now - item.CreatedDate).Days;

                if (currentDifference <= parameterValue) continue;

                var currentReleaseData = _releaseService.GetReleaseById(item.ReleaseId);

                if (currentReleaseData == null)
                {
                    Log.Instance.Info("No se encontró el release con Id: " + item.ReleaseId);
                    return;
                }

                Delete(item);

                if (!File.Exists(parameterLocalPathForDownloadFiles + @"\" + currentReleaseData.Version + ".zip"))
                    continue;

                try
                {
                    File.Delete(parameterLocalPathForDownloadFiles + @"\" + currentReleaseData.Version + ".zip");
                }
                catch (Exception e)
                {
                    Log.Instance.Error(e);
                    return;
                }
            }

            Log.Instance.Info("Proceso de creación de releases para descargas finalizado");
        }

        public DownloadRequestRelease GetDownloadRequestReleaseByReleaseId(Guid releaseId)
        {
            return FindBy(x => x.ReleaseId == releaseId).FirstOrDefault();
        }

        public bool IncreaseDownloadRequestReleaseDate(DownloadRequestRelease downloadRequestRelease)
        {
            try
            {
                var currentData = FindBy(x => x.Id == downloadRequestRelease.Id).FirstOrDefault();

                if (currentData == null)
                    return false;

                currentData.CreatedDate = downloadRequestRelease.CreatedDate;
                Update(currentData);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool UpdateDownloadRequestRelease(DownloadRequestRelease downloadRequestRelease)
        {
            try
            {
                var currentData = FindBy(x => x.Id == downloadRequestRelease.Id).FirstOrDefault();

                if (currentData == null)
                    return false;

                currentData.DownloadRequestReleaseStatusType = downloadRequestRelease.DownloadRequestReleaseStatusType;
                Update(currentData);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}