using System;
using System.Threading.Tasks;
using BuyMyHouse.Services.Interfaces;
using Microsoft.Azure.Functions.Worker;

namespace AzureFuncTimerTrigger
{
    public class TimeTrigger
    {
        public IHouseService _houseService { get; set; }
        public IUserService _userService { get; set; }

        public TimeTrigger(IUserService userService, IHouseService houseService)
        {
            this._userService = userService;
            this._houseService = houseService;
        }

        [Function("CalculateMortgage")]
        public async Task CalculateMortgage([TimerTrigger("* * * * *"/*"59 23 * * *"*/)] MyInfo myTimer, FunctionContext context)
        {
            var users = await _userService.GetUsers();
            foreach (var user in users)
            {
                await _houseService.CalculateMortgage(user);
            }
        }

        [Function("SendMailMessageOnQueue")]
        public async Task SendMailMessageOnQueue([TimerTrigger("* * * * *"/*"0 9 * * *"*/)] MyInfo myTimer, FunctionContext context)
        {
            await _userService.SendToQue();
        }

    }

    public class MyInfo
    {
        public MyScheduleStatus ScheduleStatus { get; set; }

        public bool IsPastDue { get; set; }
    }

    public class MyScheduleStatus
    {
        public DateTime Last { get; set; }

        public DateTime Next { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
