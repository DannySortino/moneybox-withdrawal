﻿using Moneybox.App.DataAccess;
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
            var account = this.accountRepository.GetAccountById(fromAccountId);

            if (account.Withdraw(amount) < Account.NearPayInLimit)
            {
                this.notificationService.NotifyFundsLow(account.User.Email);
            }
            
        }
    }
}
