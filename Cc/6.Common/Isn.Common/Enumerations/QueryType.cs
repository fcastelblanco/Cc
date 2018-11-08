using System.ComponentModel;

namespace Isn.Common.Enumerations
{
    public enum QueryType
    {
        [Description("Insert")] Insert,
        [Description("Update")] Update,
        [Description("Delete")] Delete,
        [Description("Select")] Select
    }
}
