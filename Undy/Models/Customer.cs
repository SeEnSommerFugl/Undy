namespace Undy.Models
{
    public class Customer
    {
        public Guid CustomerID { get; set; }
        public int CustomerNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int PhoneNumber { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public int PostalCode { get; set; }

    }
}
