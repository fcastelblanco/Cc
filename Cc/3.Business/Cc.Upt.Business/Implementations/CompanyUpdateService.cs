using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Cc.Upt.Business.Definitions;
using Cc.Upt.Data.Definitions;
using Cc.Upt.Data.Implementations;
using Cc.Upt.Domain;

namespace Cc.Upt.Business.Implementations
{
    public class CompanyUpdateService : Repository<ServerUpdate>, ICompanyUpdateService
    {
        private readonly IReleaseService _releaseService;
        private readonly ICompanyReleaseService _companyReleaseService;

        public CompanyUpdateService(IContext context, IReleaseService releaseService, ICompanyReleaseService companyReleaseService) : base(context)
        {
            _releaseService = releaseService;
            _companyReleaseService = companyReleaseService;
            Dbset = context.Set<ServerUpdate>();
        }

        public List<ServerUpdate> GetServerUpdateList(Guid serverId)
        {
            return FindBy(x => x.ServerId == serverId).ToList();
        }

        public bool Save(ServerUpdate companyUpdate)
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

        public ServerUpdate ValidateXmlFile(string path, string userName)
        {
            using (var streamReader = new StreamReader(path))
            {
                var theXmlSerializer = new XmlSerializer(typeof(ServerUpdate));
                return (ServerUpdate) theXmlSerializer.Deserialize(streamReader);
            }
        }

        public ServerUpdate GetLastUpdate(Guid serverId)
        {
            return FindBy(x => x.ServerId == serverId).OrderBy(x => x.Update).FirstOrDefault();
        }

        public bool CreateXml(ServerUpdate serverUpdate, string path)
        {
            using (var writer = XmlWriter.Create(path))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("CompanyUpdate");
                writer.WriteElementString("Id", serverUpdate.Id.ToString());
                writer.WriteElementString("ReleaseId", serverUpdate.ReleaseId.ToString());
                writer.WriteElementString("Update", serverUpdate.Update.ToString(CultureInfo.InvariantCulture));
                writer.WriteElementString("ServerId", serverUpdate.ServerId.ToString());
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            return true;
        }

        public IEnumerable<Release> GetAvailableReleaseByServerId(Guid serverId)
        {
            var currentCompanyUpdate = Dbset.Where(x => x.ServerId == serverId).OrderByDescending(x => x.Update)
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
                    CreatedOn = x.CreatedOn,
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

            var companyRelease = _companyReleaseService.GetCompanyReleaseList(serverId);
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
                    CreatedOn = x.CreatedOn,
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
                CreatedOn = x.CreatedOn,
                Description = x.Description,
                IsSafe = x.IsSafe,
                Notes = x.Notes,
                UserId = x.UserId,
                Version = x.Version

            }).ToList();
        }
    }
}