using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBintermittenttrigger_repro48.Models
{
    public class EventQueueMessage
    {
        public Guid EventStoreEventId { get; set; }
        public int EventType { get; set; }
        public int OperatorId { get; set; }
        public Guid? ReservationId { get; set; }
    }
}
