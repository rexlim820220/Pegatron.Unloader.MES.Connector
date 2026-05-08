using System;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Pegatron.Unloader.MES.Connector.Helpers
{
    public class ConfigModel
    {
        public string BaseUrl { get; set; }
        public string EqId { get; set; }
        public string ApiKey { get; set; }
        public int TimeoutSeconds { get; set; }
        public bool IsMockMode { get; set; }
        public ServiceSwitches ServiceSwitches { get; set; }
        public Dictionary<string, string> MappingIDs { get; set; }
    }

    public class ServiceSwitches
    {
        public bool EnableEqStatus { get; set; }
        public bool EnableRecipe { get; set; }
        public bool EnableLotCheck { get; set; }
    }

    public class ConfigHelper
    {
        private static ConfigModel _currentConfig;
        private static readonly string ConfigPath =
            Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "AULinkConfig.json"
            );
        public static ConfigModel GetConfig()
        {
            if (_currentConfig != null) return _currentConfig;

            try
            {
                if (File.Exists(ConfigPath))
                {
                    string json = File.ReadAllText(ConfigPath);
                    _currentConfig = JsonConvert.DeserializeObject<ConfigModel>(json);
                }
                else
                {
                    _currentConfig = new ConfigModel
                    {
                        BaseUrl = "http://localhost",
                        TimeoutSeconds = 10
                    };
                }
            }
            catch
            {
                _currentConfig = new ConfigModel { BaseUrl = "http://localhost", TimeoutSeconds = 10 };
            }
            return _currentConfig;
        }
    }
}
