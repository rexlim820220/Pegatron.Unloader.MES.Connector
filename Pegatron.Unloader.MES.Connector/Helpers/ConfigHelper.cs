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

            if (!File.Exists(ConfigPath))
            {
                throw new FileNotFoundException(
                    "【嚴重錯誤】找不到設定檔！\n" +
                    "請確認 AULinkConfig.json 存在於執行目錄中。\n" +
                    "目前掃描路徑為：" + ConfigPath + "\n" +
                    "提示：請檢查專案中 JSON 檔的『複製到輸出目錄』是否設為『一律複製』。"
                );
            }

            try
            {
                string json = File.ReadAllText(ConfigPath);
                _currentConfig = JsonConvert.DeserializeObject<ConfigModel>(json);

                if (_currentConfig == null)
                {
                    throw new Exception("JSON 解析結果為 null，請檢查格式是否符合 ConfigModel。");
                }

                if (string.IsNullOrEmpty(_currentConfig.BaseUrl))
                {
                    throw new Exception("設定檔內容錯誤：BaseUrl 不可為空。");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("讀取 AULinkConfig.json 時發生異常：" + ex.Message);
            }

            return _currentConfig;
        }
    }
}
