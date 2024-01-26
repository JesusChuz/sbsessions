using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using SBintermittenttrigger_repro48.Extensions;
using SBintermittenttrigger_repro48.Models.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBintermittenttrigger_repro48.Services
{
    public class ServiceBusQueueService
    {
        public static async Task RequeueMessageWithDelay(ILogger<Function1> logger,
            ServiceBusReceivedMessage message, Exception exc)
        {
            var exceptionMessages = new StringBuilder();
           /* if (exc is ServiceException serviceException)
            {
                if (serviceException.RequestErrors.Any())
                {
                    exceptionMessages.AppendLine("Request Errors:");
                }
                foreach (var error in serviceException.RequestErrors)
                {
                    exceptionMessages.AppendLine(error.Message);
                }
                exceptionMessages.AppendLine(exc.ToString());
            }
            else
            {
                exceptionMessages.AppendLine(exc.ToString());
                var innerException = exc;
                while (innerException is AggregateException exception && exception.InnerException != null)
                {
                    innerException = exception.InnerException;
                    exceptionMessages.AppendLine(innerException.ToString());
                }
            }*/


            logger.LogError(exc, exceptionMessages.ToString());

            var msgClone = message.Clone();
           //var retryCount = 3;
            var retryCount = (int)msgClone.ApplicationProperties[ApplicationValues.RetryCount];
           //var minutes = 1;
           var minutes = (int)msgClone.ApplicationProperties[ApplicationValues.DelayMinutes];

            if (retryCount <= 5)
            {
                logger.LogWarning($"Setting message to be retried in {minutes} minute(s)");

                var sbQueueName = ApplicationValues.ServiceBusQueueSettingName;
                //var sbConnString = "myqueue";
               //var sbConnString = "Endpoint=sb://jesumesbadria.servicebus.windows.net/;SharedAccessKeyName=queueukey;SharedAccessKey=ISQrkvXFjiGNUZHyPwq9x0Y2xdcBBtCBZ+ASbI26x7c=";
                var sbConnString = ApplicationValues.ServiceBusConnectionSettingName;

                // We are a .net 4.8 isolated v4 azure function so the language version is stuck at 7.3 and we would need to be version 8.0+ to use an await using() statement so I implemented the 
                // the await using w/ a try finally

                ServiceBusClient sbClient = null;
                try
                {
                    sbClient = new ServiceBusClient(sbConnString);

                    ServiceBusSender sbSender = null;
                    try
                    {
                        sbSender = sbClient.CreateSender(sbQueueName);
                        await sbSender.SendMessageAsync(msgClone);
                    }
                    finally
                    {
                        if (sbSender != null)
                            await sbSender.DisposeAsync();
                    }
                }
                finally
                {
                    if (sbClient != null)
                        await sbClient.DisposeAsync();
                }
            }
            else
            {
                var messageBody = Encoding.UTF8.GetString(message.Body.ToArray());
                logger.LogError(exc,
                    $"Error processing message: {messageBody}, message has been tried {retryCount} times in EventHandler function: {exc}");
                if (message.ApplicationProperties.ContainsKey("original-SequenceNumber"))
                    logger.LogCritical(
                        $"Exhausted all retries for message sequence # {message.ApplicationProperties["original-SequenceNumber"]}");
            }
        }
    }
}
