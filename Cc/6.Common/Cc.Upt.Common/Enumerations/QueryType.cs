using System.ComponentModel;

namespace Cc.Upt.Common.Enumerations
{
    public enum QueryType
    {
        [Description("Insert")] Insert,
        [Description("Update")] Update,
        [Description("Delete")] Delete,
        [Description("Select")] Select
    }
}
