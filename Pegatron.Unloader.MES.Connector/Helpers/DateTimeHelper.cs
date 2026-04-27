using System;

namespace Pegatron.Unloader.MES.Connector.Helpers
{
    public static class DateTimeHelper
    {
        public static string ToAasFormat(DateTime dt) => dt.ToString("yyyy/MM/dd HH:mm:ss");
        public static string ToMvixFormat(DateTime dt) => dt.ToString("yyyyMMddHHmmss");
    }
}
