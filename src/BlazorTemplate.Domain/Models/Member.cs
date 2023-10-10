namespace BlazorTemplate.Domain.Models
{
    public class Member : BaseEntity
    {
        private readonly List<MemberActivityLog> _activityLogs = new();
        private readonly List<MemberIPConstraint> _ipConstraints = new();

#pragma warning disable CS8618
        private Member() { }

        public Member(string accountId)
        {
            AccountId = accountId;
        }

        public Member(
            string firstName,
            string lastName,
            string accountId)
        {
            FirstName = firstName;
            LastName = lastName;
            AccountId = accountId;
        }

        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string AccountId { get; private set; }

        public int? CompanyId { get; set; }

        public IReadOnlyCollection<MemberActivityLog> ActivityLogs => _activityLogs.AsReadOnly();
        public IReadOnlyCollection<MemberIPConstraint> IPConstraints => _ipConstraints.AsReadOnly();
    }
}