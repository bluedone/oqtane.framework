using System;
using System.Collections.Generic;

namespace Oqtane.Models
{
    public class Sync
    {
        public DateTime SyncDate { get; set; }
        public List<SyncEvent> SyncEvents { get; set; }
    }

    public class SyncEvent
    {
        public int TenantId { get; set; }
        public string EntityName { get; set; }
        public int EntityId { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}
