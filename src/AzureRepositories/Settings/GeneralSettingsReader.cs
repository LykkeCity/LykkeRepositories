using System.Text;
using Lykke.Core.Azure.Blob;

namespace Lykke.AzureRepositories.Settings
{
	public static class GeneralSettingsReader
	{
		public static T ReadGeneralSettings<T>(string connectionString, string blobUrl = "settings/generalsettings.json")
		{
			var settingsStorage = new AzureBlobStorage(connectionString);

			var split = blobUrl.Split('/');

			var settingsData = settingsStorage.GetAsync(split[0], split[1]).Result.AsBytes();
			var str = Encoding.UTF8.GetString(settingsData);

			return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(str);
		}

		public static T ReadSettingsFromData<T>(string jsonData)
		{
			return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jsonData);
		}
	}
}
