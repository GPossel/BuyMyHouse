namespace BuyMyHouse.Domain.DTO
{
    public class UserDTO
    {
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string DateTime { get; set; }
        public double IncomeMonthly { get; set; }
        public double IncomeYearly { get; set; }
        public int Age { get; set; }
        public double Debts { get; set; }
        public bool HasPermanentContract { get; set; }

    }
}
