namespace BlazorTemplate.Domain.Entities
{
    public class Member : BaseEntity
    {
        private readonly List<MemberActivityLog> _activityLogs = new();
        private readonly List<MemberIPConstraint> _ipConstraints = new();

#pragma warning disable CS8618
        private Member() { }

        public Member(string identityGuid)
        {
            IdentityGuid = identityGuid;
        }

        public Member(
            string firstName,
            string lastName,
            string identityGuid)
        {
            FirstName = firstName;
            LastName = lastName;
            IdentityGuid = identityGuid;
        }

        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string? IdentityGuid { get; private set; }
        public Address? Address { get; private set; }

        public int? CompanyId { get; set; }

        public IReadOnlyCollection<MemberActivityLog> ActivityLogs => _activityLogs.AsReadOnly();
        public IReadOnlyCollection<MemberIPConstraint> IPConstraints => _ipConstraints.AsReadOnly();
    }
}