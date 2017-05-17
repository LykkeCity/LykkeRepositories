using System.Collections.Generic;

namespace Lykke.Core.Settings
{
	public interface IBaseSettings
	{
		string DefaultRedirect { get; set; }
		Dictionary<string, string> Redirects { get; set; }
		DbSettings Db { get; set; }
	}

	public class BaseSettings : IBaseSettings
	{
		public string DefaultRedirect { get; set; }

		public Dictionary<string, string> Redirects { get; set; }

		public DbSettings Db { get; set; }
	}


	public class DbSettings
	{
		public string DataConnString { get; set; }
		public string LogsConnString { get; set; }
	}
}
