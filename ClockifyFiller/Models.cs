using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClockifyFiller
{
    public class Estimate
    {
        public string estimate { get; set; }
        public string type { get; set; }
    }

    public class Membership
    {
        public string userId { get; set; }
        public object hourlyRate { get; set; }
        public object costRate { get; set; }
        public string targetId { get; set; }
        public string membershipType { get; set; }
        public string membershipStatus { get; set; }
    }

    public class Projects
    {
        public string id { get; set; }
        public string name { get; set; }
        public object hourlyRate { get; set; }
        public string clientId { get; set; }
        public string workspaceId { get; set; }
        public bool billable { get; set; }
        public List<Membership> memberships { get; set; }
        public string color { get; set; }
        public Estimate estimate { get; set; }
        public bool archived { get; set; }
        public string duration { get; set; }
        public string clientName { get; set; }
        public string note { get; set; }
        public object costRate { get; set; }
        public TimeEstimate timeEstimate { get; set; }
        public object budgetEstimate { get; set; }
        public bool template { get; set; }
        public bool @public { get; set; }
    }

    public class TimeEstimate
    {
        public string estimate { get; set; }
        public string type { get; set; }
        public object resetOption { get; set; }
        public bool active { get; set; }
        public bool includeNonBillable { get; set; }
    }

    public class TimeEntryToPost
    {
        public string start { get; set; }
        public string end { get; set; }
        public string projectId { get; set; }
        public bool billable { get; set; } = false;
        public string description { get; set; } = String.Empty;
        public IEnumerable<object> customFields { get; set; } = Enumerable.Empty<object>();
        public object? tagIds { get; set; } = null;
        public object? taskId { get; set; } = null;
    }
}
