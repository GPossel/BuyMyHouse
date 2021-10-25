using BuyMyHouse.Domain.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BuyMyHouse.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserDTO> CreateUser(UserDTO userDTO);
        Task<UserDTO> GetEntity(string partioningKey, string rowKey);
        Task<List<UserDTO>> GetUsers();
        Task<UserDTO> UpdateUser(UserDTO userDto);
        Task<bool> DeleteUser(string partioningKey, string rowKey);
        Task SendToQue();

        //Task<List<UserDTO>> GetUsersByRowKey(string rowKey);
        //Task<List<UserDTO>> GetUsersByPartitionKey(string partitionKey);
    }
}
