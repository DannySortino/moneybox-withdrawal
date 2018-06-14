using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using System;

namespace Moneybox.App.Features
{
    public class WithdrawMoney
    {
        private IAccountRepository accountRepository;
        private INotificationService notificationService;

        public WithdrawMoney(IAccountRepository accountRepository, INotificationService notificationService)
        {
            this.accountRepository = accountRepository;
            this.notificationService = notificationService;
        }

        public void Execute(Guid fromAccountId, decimal amount)
        {
            if (amount < 0m)
            {
                throw new InvalidOperationException("Cannot withdraw a negative amount");
            }

            var account = this.accountRepository.GetAccountById(fromAccountId);

            var afterBalance = account.Balance - amount;

            if (afterBalance < 0m)
            {
                throw new InvalidOperationException("Insufficient funds to make a withdrawal");
            }

            account.Balance -= amount;
            account.Withdrawn -= amount;

            this.accountRepository.Update(account);

            if (afterBalance < Account.NearPayInLimit)
            {
                this.notificationService.NotifyFundsLow(account.User.Email);
            }
            
        }
    }
}
