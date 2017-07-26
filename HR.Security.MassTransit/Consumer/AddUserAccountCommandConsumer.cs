using HR.Message.Contract.Command;
using HR.Message.Contract.Event;
using HR.Security.Core.Results;
using HR.Security.Core.Services.Users;
using MassTransit;
using System;
using System.Threading.Tasks;

namespace HR.Security.MassTransit.Consumer
{
    public class AddUserAccountCommandConsumer : IConsumer<AddUserAccountCommand>
    {
        private readonly IUserAccountService _userAccountService;

        public AddUserAccountCommandConsumer(IUserAccountService userAccountService)
        {
            this._userAccountService = userAccountService;
        }

        public async Task Consume(ConsumeContext<AddUserAccountCommand> context)
        {
            //1. 根据Command的UserName和Password新增用户。
            var resultAndUser = await _userAccountService.AddUserAsync(context.Message.UserName, context.Message.Password);

            var result = resultAndUser.Item1;
            var userAccount = resultAndUser.Item2;

            //2. 判断是否新增成功。
            switch(result)
            {
                //3. 如果添加用户成功则根据CountryCode发送UserAccountAddedEvent。
                case AddUserResult.Succeeded:

                    switch(context.Message.CountryCode)
                    {
                        case CountryCode.CN:

                            var userCNAddedEvent = new UserAccountCNAddedEvent()
                            {
                                StaffID = context.Message.StaffID,
                                UserID = userAccount.ID,
                                UserName = userAccount.UserName
                            };

                            await context.Publish(userCNAddedEvent);

                            break;

                        case CountryCode.US:

                            var userUSAddedEvent = new UserAccountUSAddedEvent()
                            {
                                StaffID = context.Message.StaffID,
                                UserID = userAccount.ID,
                                UserName = userAccount.UserName
                            };

                            await context.Publish(userUSAddedEvent);

                            break;
                    }

                    break;
            }

            //4. 生成Response应答。
            var response = new AddUserAccountResponse()
            {
                IsSucceeded = result == AddUserResult.Succeeded ? true : false,
                ErrorMsg = result == AddUserResult.Succeeded ? string.Empty : result.ToString()
            };

            await context.RespondAsync(response);
        }
    }
}
