using Azure.Messaging.ServiceBus;
using SBintermittenttrigger_repro48.Models.Constants;
using System;

namespace SBintermittenttrigger_repro48.Extensions
{
    public static class ServiceBusReceivedMessageExtensions
    {
        // This is a specific clone for the needs of the Azure Function Event Store Event Handler
        public static ServiceBusMessage Clone(this ServiceBusReceivedMessage message)
        {
            var retryCount = 0; // default retry to 0
            if (message.ApplicationProperties.ContainsKey(ApplicationValues.RetryCount))
                retryCount = (int)message.ApplicationProperties[ApplicationValues.RetryCount];

            var minutes = 1; // default minutes to 1
            if (retryCount > 1)
                minutes = (retryCount - 1) * 5;

            var newMsg = new ServiceBusMessage(message)
            {
                ScheduledEnqueueTime = DateTime.UtcNow.AddMinutes(minutes)
            };

            var sequenceNumber = message.SequenceNumber;
            if (message.ApplicationProperties.ContainsKey(ApplicationValues.OriginalSequenceNumber))
            {
                sequenceNumber = (long)message.ApplicationProperties[ApplicationValues.OriginalSequenceNumber];
            }

            if (newMsg.ApplicationProperties.ContainsKey(ApplicationValues.RetryCount))
                newMsg.ApplicationProperties[ApplicationValues.RetryCount] = ++retryCount;
            else
                newMsg.ApplicationProperties.Add(ApplicationValues.RetryCount, ++retryCount);

            if (newMsg.ApplicationProperties.ContainsKey(ApplicationValues.OriginalSequenceNumber))
                newMsg.ApplicationProperties[ApplicationValues.OriginalSequenceNumber] = sequenceNumber;
            else
                newMsg.ApplicationProperties.Add(ApplicationValues.OriginalSequenceNumber, sequenceNumber);

            if (newMsg.ApplicationProperties.ContainsKey(ApplicationValues.DelayMinutes))
                newMsg.ApplicationProperties[ApplicationValues.DelayMinutes] = minutes;
            else
                newMsg.ApplicationProperties.Add(ApplicationValues.DelayMinutes, minutes);

            return newMsg;
        }
    }
}
