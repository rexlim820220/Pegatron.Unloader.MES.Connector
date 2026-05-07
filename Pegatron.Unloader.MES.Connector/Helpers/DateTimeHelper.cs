using System;
using System.Runtime.InteropServices;

namespace Pegatron.Unloader.MES.Connector.Helpers
{
    public static class DateTimeHelper
    {
        // 引用 Win32 API
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetLocalTime(ref SystemTime lpSystemTime);

        [StructLayout(LayoutKind.Sequential)]
        private struct SystemTime
        {
            public ushort wYear;
            public ushort wMonth;
            public ushort wDayOfWeek;
            public ushort wDay;
            public ushort wHour;
            public ushort wMinute;
            public ushort wSecond;
            public ushort wMilliseconds;
        }
        public static string ToAasFormat(DateTime dt) => dt.ToString("yyyy/MM/dd HH:mm:ss");
        public static string ToMvixFormat(DateTime dt) => dt.ToString("yyyyMMddHHmmss");

        public static bool SyncSystemTime(DateTime dt)
        {
            SystemTime st = new SystemTime
            {
                wYear = (ushort)dt.Year,
                wMonth = (ushort)dt.Month,
                wDay = (ushort)dt.Day,
                wHour = (ushort)dt.Hour,
                wMinute = (ushort)dt.Minute,
                wSecond = (ushort)dt.Second,
                wMilliseconds = (ushort)dt.Millisecond
            };
            return SetLocalTime(ref st);
        }
    }
}
