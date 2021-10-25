using BuyMyHouse.Domain.DTO;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BuyMyHouse.Services.Interfaces
{
    public interface IHouseService
    {
        Task<List<HouseDTO>> GetHouses(double min, double max);
        Task<HouseDTO> CreateHouse(HouseDTO houseDTO);
        Task<HouseDTO> GetEntity(string partioningKey, string rowKey);
        Task<HouseDTO> UpdateHouse(HouseDTO houseDTO);
        Task<bool> UpdateHousePicture(string userId, string firstname, string lastname, Stream stream);
        Task CalculateMortgage(UserDTO userDTO);
        Task SendMail(UserDTO userDTO);
        Task<bool> DeleteHouse(string partioningKey, string rowKey);
    }
}
