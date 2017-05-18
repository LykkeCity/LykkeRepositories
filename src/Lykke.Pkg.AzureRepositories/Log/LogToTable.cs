using System;
using System.Threading.Tasks;
using Lykke.AzureRepositories.Azure;
using Lykke.Core.Azure;
using Lykke.Core.Log;

namespace Lykke.AzureRepositories.Log
{


	public class LogToTable : ILog
	{
		private readonly INoSQLTableStorage<LogEntity> _tableStorage;

		public LogToTable(INoSQLTableStorage<LogEntity> tableStorage)
		{
			_tableStorage = tableStorage;
		}


		private async Task Insert(string level, string component, string process, string context, string type, string stack,
			string msg, DateTime? dateTime)
		{
			var dt = dateTime ?? DateTime.UtcNow;
			var newEntity = LogEntity.Create(level, component, process, context, type, stack, msg, dt);
			await _tableStorage.InsertAndGenerateRowKeyAsTimeAsync(newEntity, dt);
		}

		public Task WriteInfo(string component, string process, string context, string info, DateTime? dateTime = null)
		{
			return Insert("info", component, process, context, null, null, info, dateTime);
		}

		public Task WriteWarning(string component, string process, string context, string info, DateTime? dateTime = null)
		{
			return Insert("warning", component, process, context, null, null, info, dateTime);
		}

		public Task WriteError(string component, string process, string context, Exception type, DateTime? dateTime = null)
		{
			return Insert("error", component, process, context, type.GetType().ToString(), type.StackTrace, type.Message, dateTime);
		}

		public Task WriteFatalError(string component, string process, string context, Exception type, DateTime? dateTime = null)
		{
			return Insert("fatalerror", component, process, context, type.GetType().ToString(), type.StackTrace, type.Message, dateTime);
		}
	}
}
