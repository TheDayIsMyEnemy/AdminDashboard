namespace BlazorTemplate.Domain.Entities
{
    public class MemberActivityLog : BaseEntity
    {
#pragma warning disable CS8618
        private MemberActivityLog() { }

        public MemberActivityLog(
            int memberId,
            ActivityType activityType,
            string ipAddress,
            string userAgent)
        {
            MemberId = memberId;
            ActivityType = activityType;
            Timestamp = DateTime.UtcNow;
            IPAddress = ipAddress;
            UserAgent = userAgent;
        }

        public ActivityType ActivityType { get; private set; }
        public DateTime Timestamp { get; private set; }
        public string IPAddress { get; private set; }
        public string UserAgent { get; private set; }

        public int MemberId { get; private set; }
    }
}
