using System;
using System.IO;
using System.Text;


namespace Pegatron.Unloader.MES.Connector.Helpers
{
    public static class LogHelper
    {
        private static readonly object _logLock = new object();
        private static readonly string LogFolder = Path.Combine(Path.GetTempPath(), "PegatronMESConnector");

        public static void WriteLog(string serviceName, string request, string response, long elapsedMs = 0)
        {
            try
            {
                lock (_logLock)
                {
                    if (!Directory.Exists(LogFolder)) Directory.CreateDirectory(LogFolder);

                    string filePath = Path.Combine(LogFolder, $"Log_{DateTime.Now:yyyyMMdd}.txt");
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Service: {serviceName} ({elapsedMs}ms)");
                    sb.AppendLine($"[REQ] {request}");
                    sb.AppendLine($"[RES] {response}");
                    sb.AppendLine(new string('=', 60));

                    File.AppendAllText(filePath, sb.ToString(), Encoding.UTF8);
                }
            }
            catch
            {
            }
        }

        public static void WriteWarning(string message)
        {
            try
            {
                if (!Directory.Exists(LogFolder)) Directory.CreateDirectory(LogFolder);
                string filePath = Path.Combine(LogFolder, $"Log_{DateTime.Now:yyyyMMdd}.txt");

                string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [WARNING] {message}{Environment.NewLine}{new string('=', 50)}{Environment.NewLine}";

                File.AppendAllText(filePath, logEntry, Encoding.UTF8);
            }
            catch
            {
            }
        }
    }
}
