using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;
using Isn.Common.Implementations;
using Isn.Common.LogHelper;
using Isn.Upt.Business.Definitions;
using Isn.Upt.Business.Implementations.Singleton;
using Isn.Upt.Domain;
using Isn.Upt.Domain.Enumerations;
using Isn.Upt.Presentation.AuthenticationWeb;
using Isn.Upt.Presentation.Controllers;
using Isn.Upt.Presentation.Models.Dto;

namespace Isn.Upt.Presentation.Areas.Basic.Controllers
{
    [Authorized]
    public class ReleaseController : BaseController
    {
        private readonly ICompanyService _companyService;
        private readonly ICompanyReleaseService _companyReleaseService;
        private readonly ICompanyUpdateService _companyUpdateService;


        private readonly string _directoryPath;
        private readonly IReleaseService _releaseService;
        private readonly IValidateService _validateService;

        public ReleaseController(IReleaseService releaseService, IValidateService validateService,
            ICompanyUpdateService companyUpdateService, ICompanyService company,
            ICompanyReleaseService companyReleaseService)
        {
            _releaseService = releaseService;
            _validateService = validateService;
            _companyUpdateService = companyUpdateService;
            _companyService = company;
            _companyReleaseService = companyReleaseService;
            _directoryPath = $"{HttpRuntime.AppDomainAppPath}Release";
            if (!Directory.Exists(_directoryPath))
                Directory.CreateDirectory(_directoryPath);
        }

        public ActionResult Index()
        {
            if (User.CompanyId == Guid.Empty)
                return View(_releaseService.GetList().OrderByDescending(x => x.Published));
            var lastUpdate = _companyUpdateService.GetLastUpdate(User.CompanyId);
            ViewBag.LastUpdate = lastUpdate != null ? lastUpdate.ReleaseId : Guid.Empty;

            return View(_releaseService.GetList().Where(x => x.IsSafe).Select(x => new Release
            {
                Id = x.Id,
                CreatedBy = x.CreatedBy,
                CreatedDate = x.CreatedDate,
                IsSafe = x.IsSafe,
                Description = x.Description,
                UpdatedBy = x.UpdatedBy,
                Notes = x.Notes,
                UserId = x.UserId,
                Published = x.Published,
                UpdatedDate = x.UpdatedDate,
                Version = x.Version

            }).OrderByDescending(x => x.Published));
        }

        public ActionResult Create()
        {
            ViewBag.Companys = _companyService.GetAllCompanys();
            return View();
        }

        [ValidateInput(false)]
        public ActionResult CreateRelease(string[] selectedCompanys)
        {
            var parameterInstance = ParameterSingleton.Instance;
            var nameVersion = $"{parameterInstance.ParameterList.FirstOrDefault(x => x.ParameterInternalIdentificator == ParameterInternalIdentificator.VersionValue).Value}.{DateTime.Now.Year.ToString().Substring(2)}.{DateTime.Now.Month}.{DateTime.Now.DayOfYear}.{DateTime.Now.Hour}";
            var path = $"{_directoryPath}/{nameVersion}";
            if (!Directory.Exists(path))
                return RedirectToAction("Index");

            var currentNotesData = Request.Form["Notes"];

            if (currentNotesData == null)
            {
                throw new Exception("Notes relase was not found");
            }

            if (string.IsNullOrEmpty(currentNotesData) || currentNotesData.Length < Convert.ToInt32(parameterInstance.ParameterList.FirstOrDefault(x => x.ParameterInternalIdentificator == ParameterInternalIdentificator.ValidDaysToken).Value))
            {
                ModelState.AddModelError(string.Empty, @"Ingrese las notas de la versión, al menos " + parameterInstance.ParameterList.FirstOrDefault(x => x.ParameterInternalIdentificator == ParameterInternalIdentificator.ValidDaysToken).Value + " caractéres");
                ViewBag.Companys = _companyService.GetAllCompanys();
                return View("Create");
            }

            if (!Directory.Exists($"{path}.zip"))
            {
                Compress.CompressFolder($"{path}.zip", path);
                byte[] releaseContent = System.IO.File.ReadAllBytes($"{path}.zip");
                var release = new Release
                {
                    UserId = User.Id,
                    IsSafe = true,
                    Published = DateTime.Now,
                    Version = nameVersion,
                    Notes = currentNotesData,
                    ReleaseContent = releaseContent
                };

                _releaseService.Create(release);
                Log.Instance.Info($"Usuario {User.Name} {User.LastName} creo el release {nameVersion}");

                if (selectedCompanys != null)
                    foreach (var company in selectedCompanys)
                        _companyReleaseService.Save(new CompanyRelease
                        {
                            CompanyId = new Guid(company),
                            ReleaseId = release.Id
                        });

                Directory.Delete(_directoryPath, true);
            }

            return RedirectToAction("Index");
        }

        public ActionResult Validate(ValidateFiles formData)
        {
            try
            {
                var parameterInstance = ParameterSingleton.Instance;

                var dataFiles = Request.Files;
                var nameVersion =
                    $"{parameterInstance.ParameterList.FirstOrDefault(x => x.ParameterInternalIdentificator == ParameterInternalIdentificator.VersionValue).Value}.{DateTime.Now.Year.ToString().Substring(2)}.{DateTime.Now.Month}.{DateTime.Now.DayOfYear}.{DateTime.Now.Hour}";

                var path = $"{_directoryPath}/{nameVersion}";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                for (int i = 0; i <= dataFiles.Count - 1; i++)
                {
                    var myfile = dataFiles[i];
                    if (myfile != null)
                    {
                        if (!_validateService.Validate(formData.FileTypeValidate, myfile.FileName, formData.RequiredBackup))
                            return Json(new { Success = false, Message = $"El archivo {myfile.FileName} no cumple con las especificaciones de nombre o extensión" }, JsonRequestBehavior.DenyGet);

                        myfile.SaveAs($"{path}/{myfile.FileName}");
                    }
                }
                return Json(new { Success = true, Message = "Archivo subido exitosamente" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex);
                var result = new { Success = false, ex.Message };
                return Json(result, JsonRequestBehavior.DenyGet);
            }
        }

        public ActionResult Download()
        {
            return View();
        }

        public ActionResult DownloadRelease(HttpPostedFileBase fileXml)
        {
            try
            {
                var fileName = Path.GetFileName(fileXml.FileName);
                var pathXml = Path.Combine(_directoryPath + $@"\{fileName}");
                fileXml.SaveAs(pathXml);
                var lastRelease = _companyUpdateService.ValidateXmlFile(pathXml, $"{User.Name} {User.LastName}");
                var recenteRelease = _releaseService.GetLatestRelease(lastRelease.ReleaseId).ToList();
                var releaseDistinctCompany = _companyReleaseService.GetReleaseListDownload(recenteRelease.Select(x => x.Id), lastRelease.CompanyId);
                recenteRelease = recenteRelease.Where(x => !releaseDistinctCompany.Contains(x.Id)).ToList();
                if (recenteRelease.Any())
                {
                    var companyUpdate = new CompanyUpdate
                    {
                        CompanyId = lastRelease.CompanyId,
                        Update = lastRelease.Update
                    };
                    byte[] data = CreateRelease(companyUpdate, recenteRelease);
                    Log.Instance.Info($"Iniciando descarga Isolucion_{recenteRelease.Last().Version}.zip");
                    return File(data, MediaTypeNames.Application.Octet, $@"Isolucion_{recenteRelease.Last().Version}.zip");
                }
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex);
            }

            return RedirectToAction("Index");
        }

        public ActionResult DownloadFirstRelease()
        {
            try
            {
                var recenteRelease = _releaseService.GetLatestRelease(Guid.Empty).ToList();
                var releaseDistinctCompany = _companyReleaseService.GetReleaseListDownload(recenteRelease.Select(x => x.Id), User.CompanyId);
                recenteRelease = recenteRelease.Where(x => !releaseDistinctCompany.Contains(x.Id)).ToList();
                if (recenteRelease.Any())
                {
                    var companyUpdate = new CompanyUpdate
                    {
                        CompanyId = User.CompanyId,
                        Update = DateTime.Now
                    };
                    byte[] data = CreateRelease(companyUpdate, recenteRelease);
                    Log.Instance.Info($"Iniciando descarga Isolucion_{recenteRelease.Last().Version}.zip");
                    return File(data, MediaTypeNames.Application.Octet, $@"Isolucion_{recenteRelease.Last().Version}.zip");
                }
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex);
            }
            return RedirectToAction("Index");
        }

        private byte[] CreateRelease(CompanyUpdate companyUpdate, List<Release> recenteRelease)
        {
            var pathDownload = Path.Combine(_directoryPath + $@"\Isolucion_{User.Name}_{recenteRelease.Last().Version}");
            Directory.CreateDirectory(pathDownload);
            foreach (var release in recenteRelease)
            {
                Log.Instance.Info($"Copiando release {release.Version} para descargar");
                System.IO.File.WriteAllBytes($@"{_directoryPath}\{release.Version}.zip", release.ReleaseContent);
                var path = Path.Combine(_directoryPath + $@"\{release.Version}.zip");
                if (!Directory.Exists(path))
                    System.IO.File.Copy(path, $@"{pathDownload}\{release.Version}.zip", true);

                _companyUpdateService.Save(new CompanyUpdate
                {
                    ReleaseId = release.Id,
                    CompanyId = companyUpdate.CompanyId,
                    Update = companyUpdate.Update
                });
            }

            _companyUpdateService.CreateXml(new CompanyUpdate
            {
                ReleaseId = recenteRelease.LastOrDefault().Id,
                CompanyId = companyUpdate.CompanyId,
                Update = companyUpdate.Update
            }, $@"{pathDownload}\CompanyUpdate.xml");

            if (!Directory.Exists($"{pathDownload}.zip"))
            {
                Compress.CompressFolder($"{pathDownload}.zip", pathDownload);
                Log.Instance.Info($"Comprimiendo archivo {pathDownload}.zip para descargar");
            }

            var fs = System.IO.File.OpenRead($"{pathDownload}.zip");
            var data = new byte[fs.Length];
            var br = fs.Read(data, 0, data.Length);
            if (br != fs.Length)
                throw new IOException($"{pathDownload}.zip");
            return data;
        }

        public ActionResult Edit(Guid id)
        {
            ViewBag.Companys = _companyService.GetAllCompanys();
            return View(_releaseService.FindBy(x => x.Id == id).FirstOrDefault());
        }

        [ValidateInput(false)]
        public ActionResult EditRelase(Release release)
        {
            var currentNotesData = Request.Form["Notes"];
            var parameterInstance = ParameterSingleton.Instance;
            if (currentNotesData == null)
            {
                throw new Exception("Release notes was not found");
            }

            if (string.IsNullOrEmpty(currentNotesData) || currentNotesData.Length < Convert.ToInt32(parameterInstance.ParameterList.FirstOrDefault(x => x.ParameterInternalIdentificator == ParameterInternalIdentificator.ValidDaysToken).Value))
            {
                ModelState.AddModelError(string.Empty, @"Ingrese las notas de la versión, al menos " + parameterInstance.ParameterList.FirstOrDefault(x => x.ParameterInternalIdentificator == ParameterInternalIdentificator.ValidDaysToken).Value + " caractéres");
                ViewBag.Companys = _companyService.GetAllCompanys();
                return View("Create");
            }
            var releaseCopy = _releaseService.FindBy(x => x.Id == release.Id).FirstOrDefault();
            releaseCopy.IsSafe = release.IsSafe;
            releaseCopy.Notes = currentNotesData;
            _releaseService.Update(releaseCopy);
            return RedirectToAction("Index");
        }

    }
}