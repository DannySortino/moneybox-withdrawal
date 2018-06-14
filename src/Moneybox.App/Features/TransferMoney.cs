using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using System;

namespace Moneybox.App.Features
{
    public class TransferMoney
    {
        private IAccountRepository accountRepository;
        private INotificationService notificationService;

        public TransferMoney(IAccountRepository accountRepository, INotificationService notificationService)
        {
            this.accountRepository = accountRepository;
            this.notificationService = notificationService;
        }

        public void Execute(Guid fromAccountId, Guid toAccountId, decimal amount)
        {
            var from = this.accountRepository.GetAccountById(fromAccountId);
            var to = this.accountRepository.GetAccountById(toAccountId);

            if (to.PaidIn + amount > Account.PayInLimit)
            {
                throw new InvalidOperationException("Account pay in limit reached");
            }

            from.Withdraw(amount);

            if (Account.PayInLimit - to.Payin(amount) < Account.NearPayInLimit)
            {
                this.notificationService.NotifyApproachingPayInLimit(to.User.Email);
            }

        }
    }
}
