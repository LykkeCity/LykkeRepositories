using System;
using System.Threading.Tasks;
using Lykke.Core.Log;

namespace Lykke.AzureRepositories.Log
{
    public class CommonLogAdapter : ILog
    {
        private readonly global::Common.Log.ILog _commonLogImpl;

        public CommonLogAdapter(global::Common.Log.ILog commonLogImpl)
        {
            _commonLogImpl = commonLogImpl;
        }

        public Task WriteInfo(string component, string process, string context, string info, DateTime? dateTime = null)
        {
            return _commonLogImpl.WriteInfoAsync(component, process, context, info, dateTime);
        }

        public Task WriteWarning(string component, string process, string context, string info, DateTime? dateTime = null)
        {
            return _commonLogImpl.WriteWarningAsync(component, process, context, info, dateTime);
        }

        public Task WriteError(string component, string process, string context, Exception exeption, DateTime? dateTime = null)
        {
            return _commonLogImpl.WriteErrorAsync(component, process, context, exeption, dateTime);
        }

        public Task WriteFatalError(string component, string process, string context, Exception exeption, DateTime? dateTime = null)
        {
            return _commonLogImpl.WriteFatalErrorAsync(component, process, context, exeption, dateTime);
        }
    }
}