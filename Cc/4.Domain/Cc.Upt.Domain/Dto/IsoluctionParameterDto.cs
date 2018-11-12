using Cc.Upt.Domain.Enumerations;

namespace Cc.Upt.Domain.Dto
{
    public class IsoluctionParameterDto
    {
        public string Table { get; set; }
        public string ColumnOrField { get; set; }
        public string Where { get; set; }
        public ParameterIsolucion ParameterIsolucion { get; set; }
        public LicenseParameter LicenseParameter { get; set; }
        public bool IsForCreateParameterAndFillIt { get; set; } = false;
        public bool IsForFillParameter { get; set; } = false;
        public string ScriptInsertOracle { get; set; }
        public string ScriptInsertSql { get; set; }
        public string ScriptCreateColumnOracle { get; set; }
        public string ScriptCreateColumnSql { get; set; }
    }
}