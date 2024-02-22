namespace BlazorTemplate.Domain.Entities
{
    public class Address
    {
#pragma warning disable CS8618
        private Address() { }

        public Address(
            string street,
            string city,
            string state,
            string country,
            string postCode)
        {
            Street = street;
            City = city;
            State = state;
            Country = country;
            PostCode = postCode;
        }

        public string Street { get; private set; }
        public string City { get; private set; }
        public string State { get; private set; }
        public string PostCode { get; private set; }
        public string Country { get; private set; }
    }
}