using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBintermittenttrigger_repro48.Models.Constants
{
    public static class ApplicationValues
    {
        public const string RetryCount = "3";
        public const string OriginalSequenceNumber = "001";
        public const string DelayMinutes = "1";

        public const string ServiceBusConnectionSettingName = "Endpoint=sb://jesumesbadria.servicebus.windows.net/;SharedAccessKeyName=queueukey;SharedAccessKey=ISQrkvXFjiGNUZHyPwq9x0Y2xdcBBtCBZ+ASbI26x7c=";
        public const string ServiceBusQueueSettingName = "myqueue";
//        public const string CoreSqlConnectionStringName = "FSP";
    }
}
