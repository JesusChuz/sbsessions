using System;
using System.Text.Json.Serialization;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SBintermittenttrigger_repro48.Models;
using SBintermittenttrigger_repro48.Services;


namespace SBintermittenttrigger_repro48
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;

        public Function1(ILogger<Function1> logger)
        {
            _logger = logger;
        }

        [Function("SbQueueTrigger")]
        public async Task Run(
            [ServiceBusTrigger("%QueueName%", Connection =  "ServiceBusConnection", IsSessionsEnabled = true)]
            ServiceBusReceivedMessage message, ServiceBusMessageActions messageActions)
        {

            var messageBody = Encoding.UTF8.GetString(message.Body.ToArray());
            _logger.LogInformation($"EventHandler function triggered with message: {messageBody}");
            try
            {
                var @event = JsonConvert.DeserializeObject<EventQueueMessage>(messageBody);

                try
                {
                    if (@event != null)
                    {
                       // var service = new EventStoreEventProcessorService(_logger);
                       // await service.ProcessEventStoreEventQueueMessage(@event);
                        _logger.LogInformation("Message ID: {id}", message.MessageId);
                        _logger.LogInformation("Message Body: {body}", message.Body);
                        _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);
                        await messageActions.CompleteMessageAsync(message);
                    }
                    else
                    {
                        _logger.LogError($"Invalid message body: {messageBody}");
                    }
                }
                catch (Exception exc)
                {
                   await ServiceBusQueueService.RequeueMessageWithDelay(_logger, message, exc);
                }
            }
            catch (JsonReaderException)
            {
                _logger.LogError($"Invalid json message body: {messageBody}");
            }
            catch (Exception exc)
            {
                _logger.LogError($"Error with message body : {messageBody}\n{exc.ToString()}");
            }
        }
        // Complete the message
        // await messageActions.CompleteMessageAsync(message);
    }
}