using System;

namespace Lykke.AzureRepositories.Extentions
{
    static class DateTimeExt
    {
        public static string StorageString(this DateTime datetime)
        {
            return datetime.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }
    }
}
