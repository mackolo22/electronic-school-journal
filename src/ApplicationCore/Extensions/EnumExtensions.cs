using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace ApplicationCore.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            string displayName
                = enumValue.GetType()
                           .GetMember(enumValue.ToString())
                           .First()
                           .GetCustomAttribute<DisplayAttribute>()
                           .GetName();

            return displayName;
        }

        public static string GetDescription(this Enum enumValue)
        {
            string description
               = enumValue.GetType()
                          .GetMember(enumValue.ToString())
                          .First()
                          .GetCustomAttribute<DescriptionAttribute>()
                          .Description;

            return description;
        }
    }
}
