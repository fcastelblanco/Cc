using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isn.Upt.Domain.Dto
{
    public class ValidateRuleDto
    {
        public string Input { get; set; }
        public string NameFile { get; set; }
        public string Extension { get; set; }
        public int LengthSplitUnderScore { get; set; }
        public bool BackUp { get; set; }
    }
}
