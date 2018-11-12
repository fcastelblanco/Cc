using System;
using System.ComponentModel;

namespace Cc.Common.Enumerations
{
    public static class EnumExtension
    {
        public static string GetDescription(this Enum value)
        {
            var type = value.GetType();
            if (!type.IsEnum) throw new ArgumentException(string.Format("Type '{0}' is not Enum", type));

            var members = type.GetMember(value.ToString());
            if (members.Length == 0)
                throw new ArgumentException(string.Format("Member '{0}' not found in type '{1}'", value, type.Name));

            var member = members[0];
            var attributes = member.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes.Length == 0)
                throw new ArgumentException(string.Format("'{0}.{1}' doesn't have DescriptionAttribute", type.Name,
                    value));

            var attribute = (DescriptionAttribute) attributes[0];
            return attribute.Description;
        }
    }
}