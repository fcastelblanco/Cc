using Isn.Upt.Domain.Enumerations;
using System;

namespace Isn.Upt.Domain.Dto
{
    public class IpmConfiguratorDto
    {
        public UpdateMode UpdateMode { get; set; }
        public string ReleasePath { get; set; }
        public int IntervalExecution { get; set; } = 5;
        public string ApiUrl { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FtpUserName { get; set; }
        public string FtpPassword { get; set; }
        public string FtpPath { get; set; }
        public Guid CompanyId { get; set; }
        public int BufferSize { get; set; }
    }
}