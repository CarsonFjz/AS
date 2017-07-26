using System;
using System.Configuration;
using System.Runtime.CompilerServices;

namespace HR.Security.Core.Settings
{
    public static class AppSettingConfig
    {
        private static string AppSettingValue([CallerMemberName]string key = null)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public static string DefaultReturnUrl
        {
            get
            {
                return AppSettingValue();
            }
        }

        public static string CookieDomain
        {
            get
            {
                return AppSettingValue();
            }
        }

        public static bool SslRequirement
        {
            get
            {
                return Convert.ToBoolean(AppSettingValue());
            }
        }

        public static string ASConnectionString
        {
            get
            {
                return AppSettingValue();
            }
        }
    }
}
