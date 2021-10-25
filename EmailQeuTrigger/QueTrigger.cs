using System.Threading.Tasks;
using BuyMyHouse.Services.Interfaces;
using Microsoft.Azure.Functions.Worker;

namespace EmailQeuTrigger
{
    public class QueTrigger
    {
        public IHouseService _houseService { get; set; }
        public IUserService _userService { get; set; }

        public QueTrigger(IUserService userService, IHouseService houseService)
        {
            this._userService = userService;
            this._houseService = houseService;
        }

        [Function("SendMail")]
        public async Task SendMail([QueueTrigger("email-que-item", Connection = "AzureWebJobsStorage")] string myQueueItem,  FunctionContext context)
        {
            var users = await _userService.GetUsers();
            foreach (var user in users)
            {
                await _houseService.SendMail(user);
            }
        }
    }
}
