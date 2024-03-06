namespace Demo.DB.LiteDB
{
    public enum Gender
    {
        Male,
        Female
    }

    public class User
    {
        public int _id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public DateTime Birthday { get; set; }
        public Gender Gender { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string StreetAddress { get; set; }
        public string BuildingNumber { get; set; }
        public string ZipCode { get; set; }
        public string SecondaryAddress { get; set; }
        public string Company { get; set; }
        public string CompanySuffix { get; set; }
        public string Department { get; set; }
        public string JobTitle { get; set; }
        public string JobDescriptor { get; set; }
        public string JobArea { get; set; }
        public string AccountName { get; set; }
        public string Account { get; set; }
        public string CreditCardNumber { get; set; }
        public string VehicleModel { get; set; }
        public string VehicleType { get; set; }
        public string Vin { get; set; }   
        public int PetsCount { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }
}
