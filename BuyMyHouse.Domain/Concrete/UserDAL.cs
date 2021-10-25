using Microsoft.WindowsAzure.Storage.Table;

namespace BuyMyHouse.Domain
{
    public class UserDAL : TableEntity
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

        public UserDAL()
        {

        }

        public UserDAL(string userId, string firstName, string lastName, string email, string dateTime, double incomeMonthly, double incomeYearly, int age, double debts, bool hasPermanentContract)
        {
            UserId = userId;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            DateTime = dateTime;
            IncomeMonthly = incomeMonthly;
            IncomeYearly = incomeYearly;
            Age = age;
            Debts = debts;
            HasPermanentContract = hasPermanentContract;

            PartitionKey = userId;
            RowKey = firstName + lastName;
        }
    }
}
