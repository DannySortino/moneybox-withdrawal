using System;

namespace Moneybox.App
{
    public class Account
    {
        public Account(User user, Guid? id = null, decimal balance = 0.0m, decimal withdrawn = 0.0m, decimal paidIn = 0.0m)
        {
            this.Id = id ?? Guid.NewGuid();
            this.User = user;
            this.Balance = balance;
            this.Withdrawn = withdrawn;
            this.PaidIn = paidIn;
        }

        public const decimal PayInLimit = 4000m;

        public const decimal LowFundsValue = 500m;

        public const decimal NearPayInLimit = 500m;

        public Guid Id { get; private set; }

        public User User { get; private set; }

        public decimal Balance { get; private set; }

        public decimal Withdrawn { get; private set; }

        public decimal PaidIn { get; private set; }

        public decimal Withdraw(decimal amount)
        {
            if (amount < 0m)
            {
                throw new InvalidOperationException("Cannot complete action with a negative amount");
            }

            var afterBalance = this.Balance - amount;

            if (afterBalance < 0m)
            {
                throw new InvalidOperationException("Insufficient funds to complete action");
            }

            this.Balance -= amount;
            this.Withdrawn -= amount;

            return this.Balance;
        }

        public decimal Payin(decimal amount)
        {

            this.Balance += amount;
            this.PaidIn += amount;

            return this.Balance;
        }
    }
}
