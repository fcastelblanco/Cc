using System;
using Cc.Common.Implementations;
using Cc.Upt.Domain.Enumerations;

namespace Cc.Upt.Business.Implementations.Singleton
{
    public class ConfigurationUpdaterManager : HelperSingleton<ConfigurationUpdaterManager>
    {
        public UpdateMode UpdateMode { get; set; }
        public string FtpPathCompresedFiles { get; set; }

        public string ApiUrl { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public Guid CompanyId { get; set; }
        public string FtpUserName { get; set; }
        public string FtpUserPassword { get; set; }
        public string FtpPath { get; set; }
        public int BufferSize { get; set; } = 2048;
    }
}