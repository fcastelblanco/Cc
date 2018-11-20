namespace Cc.Upt.Domain.DataTransferObject
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
