namespace BlazorTemplate.Domain.Models
{
    public class Member : BaseEntity
    {
        private readonly List<MemberActivityLog> _activityLogs = new();
        private readonly List<MemberIPConstraint> _ipConstraints = new();

#pragma warning disable CS8618
        private Member() { }

        public Member(string email)
        {
            Email = email;
        }

        public Member(
            string firstName,
            string lastName,
            string email,
            Address address)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Address = address;
        }

        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public Address Address { get; private set; }

        public int? CompanyId { get; set; }

        public IReadOnlyCollection<MemberActivityLog> ActivityLogs => _activityLogs.AsReadOnly();
        public IReadOnlyCollection<MemberIPConstraint> IPConstraints => _ipConstraints.AsReadOnly();
    }
}