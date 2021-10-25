using BuyMyHouse.Domain.DTO;
using BuyMyHouse.Domain.Helpers;
using BuyMyHouse.Infrastructure;
using BuyMyHouse.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuyMyHouse.Services
{
    public class UserService : IUserService
    {
        private  TableStorageUser db_User { get; set; }
        private  QueStorage _queStorage { get; set; }
        public UserService()
        {  }

        public UserService(TableStorageUser tableStorageUser, QueStorage storage)
        {
            this.db_User = tableStorageUser;
            this._queStorage = storage;
        }

        public async Task<UserDTO> CreateUser(UserDTO request)
        {
            var userDAL = UserConversionsHelper.toDAL(request);
            var result = await db_User.CreateEntity(userDAL);
            var userDTO = UserConversionsHelper.toDTO(result);
            return userDTO;
        }

        public async Task<UserDTO> GetEntity(string partioningKey, string rowKey)
        {
            var result = await db_User.GetEntityByPartitionKeyAndRowKey(partioningKey, rowKey);
            if (result is null) throw new NullReferenceException("No user found.");
            var userDTO = UserConversionsHelper.toDTO(result);
            return userDTO;
        }

        public async Task<UserDTO> UpdateUser(UserDTO userDto)
        {
            var userDAL = await db_User.GetEntityByPartitionKeyAndRowKey(userDto.UserId, userDto.FirstName + userDto.LastName);
            if (userDAL is null) throw new NullReferenceException($"No user found on with {userDto.UserId}.");
            var result = await db_User.UpdateEntity(userDAL);
            var userDTO = UserConversionsHelper.toDTO(result);
            return userDTO;
        }

        public async Task<bool> DeleteUser(string partioningKey, string rowKey)
        {
            var result = await db_User.DeleteEntity(partioningKey, rowKey);
            return result;
        }

        public async Task<List<UserDTO>> GetUsers()
        {
            var result = await db_User.GetAllUsers();
            var list = result.Select(x => UserConversionsHelper.toDTO(x)).ToList();
            return list;
        }

        public async Task SendToQue()
        {
            await  _queStorage.CreateQueueMessage("email-que-item", "Calculations are done. Sending email in background...");
        }


    }
}
