using BuyMyHouse.Domain.Concrete;
using BuyMyHouse.Domain.DTO;

namespace BuyMyHouse.Domain.Helpers
{
    public static class HouseConversionHelper
    {
        public static HouseDTO toDTO(HouseDAL houseDAL)
        {
            return new HouseDTO()
            {
                HouseId = houseDAL.HouseId,
                Title = houseDAL.Title,
                Decsription = houseDAL.Decsription,
                Mortgage = houseDAL.Mortgage,
                ImageURL = houseDAL.ImageURL,
                Street = houseDAL.Street,
                HouseNumber = houseDAL.HouseNumber,
                Addition = houseDAL.Addition,
                ZipCode = houseDAL.ZipCode,
                Country = houseDAL.Country
            };
        }

        public static HouseDAL toDAL(HouseDTO houseDTO)
        {
            return new HouseDAL()
            {
                HouseId = houseDTO.HouseId,
                Title = houseDTO.Title,
                Decsription = houseDTO.Decsription,
                Mortgage = houseDTO.Mortgage,
                ImageURL = houseDTO.ImageURL,
                Street = houseDTO.Street,
                HouseNumber = houseDTO.HouseNumber,
                Addition = houseDTO.Addition,
                ZipCode = houseDTO.ZipCode,
                Country = houseDTO.Country
            };
        }


    }
}
