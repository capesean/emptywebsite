using System;

namespace WEB.Models
{
    public static partial class ExtensionMethods
    {
        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static string GetRoleName(this Roles role)
        {
            if (role == Roles.Administrator) return "System Administrator";
            // this will be overwritten by codegenerator
            throw new ArgumentException("Invalid role in EnumExtensions.RoleName: " + role.ToString());
        }

    }
}