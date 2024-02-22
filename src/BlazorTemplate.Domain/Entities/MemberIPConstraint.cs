namespace BlazorTemplate.Domain.Entities
{
    public class MemberIPConstraint : BaseEntity
    {
#pragma warning disable CS8618
        private MemberIPConstraint() { }

        public MemberIPConstraint(
            int memberId,
            string ipAddress,
            ConstraintType type)
        {
            MemberId = memberId;
            IPAddress = ipAddress;
            Type = type;
            CreatedAt = DateTime.UtcNow;
        }

        public MemberIPConstraint(
            int memberId,
            string ipAddress,
            ConstraintType type,
            DateTime expiresAt)
                : this(memberId, ipAddress, type)
        {
            if (CreatedAt >= expiresAt)
                throw new ArgumentException("Invalid expiration date");

            ExpiresAt = expiresAt;
        }

        public string IPAddress { get; private set; }
        public ConstraintType Type { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? ExpiresAt { get; private set; }

        public int MemberId { get; private set; }
    }
}