
using LiteDB;

namespace MyLoadTest
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
        public string Email { get; set; }
        public int Age { get; set; }
        public DateTime Birthday { get; set; }
        public Gender Gender { get; set; }
        public string City { get; set; }
        public string Company { get; set; }
        public string Department { get; set; }
        public string Account { get; set; }
        public string Vehicle { get; set; }
        public string Vin { get; set; }
        public string PhoneNumber { get; set; }
        public string JobTitle { get; set; }
        public string CreditCardNumber { get; set; }
        public string State { get; set; }
        public string StreetAddress { get; set; }
        public string ZipCode { get; set; }
        public int PetsNumber { get; set; }
       
    }
}
