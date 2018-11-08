using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Isn.Upt.Business.Definitions;
using Isn.Upt.Data.Definitions;
using Isn.Upt.Data.Implementations;
using Isn.Upt.Domain;

namespace Isn.Upt.Business.Implementations
{
    public class CompanyUpdateService : EntityService<CompanyUpdate>, ICompanyUpdateService
    {
        private readonly IReleaseService _releaseService;
        private readonly ICompanyReleaseService _companyReleaseService;

        public CompanyUpdateService(IContext context, IReleaseService releaseService, ICompanyReleaseService companyReleaseService) : base(context)
        {
            _releaseService = releaseService;
            _companyReleaseService = companyReleaseService;
            Dbset = context.Set<CompanyUpdate>();
        }

        public List<CompanyUpdate> GetCompanyUpdateList(Guid companyId)
        {
            return FindBy(x => x.CompanyId == companyId).ToList();
        }

        public bool Save(CompanyUpdate companyUpdate)
        {
            try
            {
                Create(companyUpdate);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public CompanyUpdate ValidateXmlFile(string path, string userName)
        {
            using (var streamReader = new StreamReader(path))
            {
                var theXmlSerializer = new XmlSerializer(typeof(CompanyUpdate));
                return (CompanyUpdate) theXmlSerializer.Deserialize(streamReader);
            }
        }

        public CompanyUpdate GetLastUpdate(Guid companyId)
        {
            return FindBy(x => x.CompanyId == companyId).OrderBy(x => x.Update).FirstOrDefault();
        }

        public bool CreateXml(CompanyUpdate companyUpdate, string path)
        {
            using (var writer = XmlWriter.Create(path))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("CompanyUpdate");
                writer.WriteElementString("ID", companyUpdate.Id.ToString());
                writer.WriteElementString("ReleaseId", companyUpdate.ReleaseId.ToString());
                writer.WriteElementString("Update", companyUpdate.Update.ToString(CultureInfo.InvariantCulture));
                writer.WriteElementString("CompanyId", companyUpdate.CompanyId.ToString());
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            return true;
        }

        public IEnumerable<Release> GetAvailableReleaseByCompanyId(Guid companyId)
        {
            var currentCompanyUpdate = Dbset.Where(x => x.CompanyId == companyId).OrderByDescending(x => x.Update)
                .FirstOrDefault();

            List<Release> releaseList;

            if (currentCompanyUpdate == null)
            {
                releaseList = _releaseService.GetList().OrderBy(x => x.Published).ToList();

                return releaseList.Select(x => new Release
                {
                    Id = x.Id,
                    Published = x.Published,
                    CreatedBy = x.CreatedBy,
                    CreatedDate = x.CreatedDate,
                    Description = x.Description,
                    IsSafe = x.IsSafe,
                    Notes = x.Notes,
                    UserId = x.UserId,
                    Version = x.Version

                }).ToList();
            }

            var currentRelease = _releaseService.GetReleaseById(currentCompanyUpdate.ReleaseId);

            if (currentRelease == null)
            {
                return null;
            }

            var companyRelease = _companyReleaseService.GetCompanyReleaseList(companyId);
            releaseList = _releaseService.GetList().Where(x => x.Published > currentRelease.Published).ToList();
            var temporalReleaseList = new List<Release>();

            foreach (var release in releaseList)
            {
                if (companyRelease.Any(x => x.ReleaseId == release.Id))
                {
                    temporalReleaseList.Add(release);
                }
            }

            if (temporalReleaseList.Any())
            {
                return temporalReleaseList.Select(x => new Release
                {
                    Id=x.Id,
                    Published = x.Published,
                    CreatedBy = x.CreatedBy,
                    CreatedDate = x.CreatedDate,
                    Description = x.Description,
                    IsSafe = x.IsSafe,
                    Notes = x.Notes,
                    UserId = x.UserId,
                    Version = x.Version

                }).ToList();
            }

            return releaseList.Select(x => new Release
            {
                Id = x.Id,
                Published = x.Published,
                CreatedBy = x.CreatedBy,
                CreatedDate = x.CreatedDate,
                Description = x.Description,
                IsSafe = x.IsSafe,
                Notes = x.Notes,
                UserId = x.UserId,
                Version = x.Version

            }).ToList();
        }
    }
}