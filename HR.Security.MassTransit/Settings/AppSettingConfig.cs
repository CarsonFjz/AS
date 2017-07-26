using System;
using System.Configuration;
using System.Runtime.CompilerServices;

namespace HR.Security.MassTransit.Settings
{
    public static class AppSettingConfig
    {
        private static string AppSettingValue([CallerMemberName]string key = null)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public static string TransportType
        {
            get
            {
                return AppSettingValue();
            }
        }

        public static string BaseQueueName
        {
            get
            {
                return AppSettingValue();
            }
        }

        public static string RabbitMqBaseUri
        {
            get
            {
                return AppSettingValue();
            }
        }

        public static string RabbitMqUserName
        {
            get
            {
                return AppSettingValue();
            }
        }

        public static string RabbitMqUserPassword
        {
            get
            {
                return AppSettingValue();
            }
        }
    }
}
