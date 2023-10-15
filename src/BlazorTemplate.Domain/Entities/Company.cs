namespace BlazorTemplate.Domain.Entities
{
    public class Company : BaseEntity
    {
        private readonly List<Member> _members = new();

#pragma warning disable CS8618
        private Company() { }

        public Company(
            string companyName,
            Address address)
        {
            CompanyName = companyName;
            Address = address;
        }

        public string CompanyName { get; private set; }
        public Address Address { get; private set; }

        public IReadOnlyCollection<Member> Members => _members.AsReadOnly();
    }
}