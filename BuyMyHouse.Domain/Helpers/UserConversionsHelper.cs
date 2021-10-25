using BuyMyHouse.Domain.DTO;

namespace BuyMyHouse.Domain.Helpers
{
    public static class UserConversionsHelper
    {
        public static UserDTO toDTO(UserDAL userDAL)
        {
            return new UserDTO()
            {
                UserId = userDAL.UserId,
                FirstName = userDAL.FirstName,
                LastName = userDAL.LastName,
                DateTime = userDAL.DateTime,
                Email = userDAL.Email,
                IncomeMonthly = userDAL.IncomeMonthly,
                IncomeYearly = userDAL.IncomeYearly,
                Age = userDAL.Age,
                Debts = userDAL.Debts,
                HasPermanentContract = userDAL.HasPermanentContract
            };
        }
        public static UserDAL toDAL(UserDTO userDTO)
        {
            return new UserDAL()
            {
                UserId = userDTO.UserId,
                FirstName = userDTO.FirstName,
                LastName = userDTO.LastName,
                DateTime = userDTO.DateTime,
                Email = userDTO.Email,
                IncomeMonthly = userDTO.IncomeMonthly,
                IncomeYearly = userDTO.IncomeYearly,
                Age = userDTO.Age,
                Debts = userDTO.Debts,
                HasPermanentContract = userDTO.HasPermanentContract

            };
        }
    }
}
