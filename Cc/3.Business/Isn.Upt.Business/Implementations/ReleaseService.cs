using System;
using System.Collections.Generic;
using System.Linq;
using Isn.Upt.Business.Definitions;
using Isn.Upt.Data.Definitions;
using Isn.Upt.Data.Implementations;
using Isn.Upt.Domain;

namespace Isn.Upt.Business.Implementations
{
    public class ReleaseService : EntityService<Release>, IReleaseService
    {
        public ReleaseService(IContext context) : base(context)
        {
            Dbset = context.Set<Release>();
        }

        public IEnumerable<Release> GetList()
        {
            return GetAll();
        }

        public bool SetReleaseAsSafe(bool isSafe, Guid releaseId)
        {
            var currentRelease = FindBy(x => x.Id == releaseId).FirstOrDefault();

            if (currentRelease == null)
                return false;

            try
            {
                currentRelease.IsSafe = isSafe;
                Update(currentRelease);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public IEnumerable<Release> GetLatestRelease(Guid releaseId)
        {
            var currentRelease = FindBy(x => x.Id == releaseId).FirstOrDefault();

            return currentRelease != null
                ? GetList().Where(x => x.Published > currentRelease.Published).OrderBy(x => x.Published).ToList()
                : GetList().OrderBy(x => x.Published).ToList();
        }

        public Release GetReleaseById(Guid releaseId)
        {
            return FindBy(x => x.Id == releaseId).FirstOrDefault();
        }
    }
}