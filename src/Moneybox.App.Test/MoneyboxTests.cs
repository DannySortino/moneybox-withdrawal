using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moneybox.App.Features;
using Moneybox.App;
using Moneybox.App.DataAccess;
using System;
using Moq;
using Moneybox.App.Domain.Services;

namespace UnitTestProject1
{
    [TestClass]
    public class MoneyboxTests
    {
        Account testFromAccount, testToAccount;
        Mock<IAccountRepository> accountRepository;
        Mock<INotificationService> notificationService;


        public MoneyboxTests()
        {
            User testFromUser = new User()
            {
                Id = Guid.NewGuid(),
                Name = "testFrom",
                Email = "testFrom@test.com"
            };


            this.testFromAccount = new Account()
            {
                Id = Guid.NewGuid(),
                User = testFromUser,
                Balance = 100m,
                PaidIn = 200m,
                Withdrawn = 50m
            };

            User testToUser = new User()
            {
                Id = Guid.NewGuid(),
                Name = "testTo",
                Email = "testTo@test.com"
            };


            this.testToAccount = new Account()
            {
                Id = Guid.NewGuid(),
                User = testToUser,
                Balance = 100m,
                PaidIn = 200m,
                Withdrawn = 50m
            };

            this.accountRepository = new Mock<IAccountRepository>();
            this.notificationService = new Mock<INotificationService>();
            this.accountRepository.Setup(x => x.GetAccountById(this.testFromAccount.Id)).Returns(() => this.testFromAccount);
            this.accountRepository.Setup(x => x.GetAccountById(this.testToAccount.Id)).Returns(() => this.testToAccount);

        }

        [TestMethod]
        public void TestSufficentFromTransfer()
        {
            decimal startFromBalance = this.testFromAccount.Balance;

            TransferMoney transferMoneyFeature = new TransferMoney(accountRepository.Object, notificationService.Object);
            transferMoneyFeature.Execute(this.testFromAccount.Id, this.testToAccount.Id, 10.0m);

            Assert.AreEqual(this.testFromAccount.Balance, startFromBalance - 10.0m);
        }

        [TestMethod]
        public void TestSufficentToTransfer()
        {
            decimal startToBalance = this.testToAccount.Balance;

            TransferMoney transferMoneyFeature = new TransferMoney(accountRepository.Object, notificationService.Object);
            transferMoneyFeature.Execute(this.testFromAccount.Id, this.testToAccount.Id, 10.0m);

            Assert.AreEqual(this.testToAccount.Balance, startToBalance + 10.0m);
        }

        [TestMethod]
        public void TestIntsufficentFundsTransfer()
        {
            this.testFromAccount.Balance = 5.0m;
            decimal startFromBalance = this.testFromAccount.Balance;

            TransferMoney transferMoneyFeature = new TransferMoney(accountRepository.Object, notificationService.Object);
            try
            {
                transferMoneyFeature.Execute(this.testFromAccount.Id, this.testToAccount.Id, 10.0m);
            }
            catch (InvalidOperationException e)
            {
                StringAssert.Contains(e.Message, "Insufficient funds to make transfer");
                return;
            }
            Assert.Fail("No exception was thrown.");
        }

        [TestMethod]
        public void TestPayinLimitTransfer()
        {
            this.testToAccount.PaidIn = Account.PayInLimit;
            decimal startFromBalance = this.testFromAccount.Balance;

            TransferMoney transferMoneyFeature = new TransferMoney(accountRepository.Object, notificationService.Object);
            try
            {
                transferMoneyFeature.Execute(this.testFromAccount.Id, this.testToAccount.Id, 10.0m);
            }
            catch (InvalidOperationException e)
            {
                StringAssert.Contains(e.Message, "Account pay in limit reached");
                return;
            }
            Assert.Fail("No exception was thrown.");
        }

        [TestMethod]
        public void TestSufficentWithdraw()
        {
            decimal startBalance = this.testFromAccount.Balance;
            WithdrawMoney withdrawMoneyFeature = new WithdrawMoney(accountRepository.Object, notificationService.Object);
            withdrawMoneyFeature.Execute(testFromAccount.Id, 10.0m);

            Assert.AreEqual(this.testFromAccount.Balance, startBalance - 10.0m);
        }

        [TestMethod]
        public void TestInsufficentWithdraw()
        {
            this.testFromAccount.Balance = 1.0m;
            WithdrawMoney withdrawMoneyFeature = new WithdrawMoney(accountRepository.Object, notificationService.Object);

            try
            {
                withdrawMoneyFeature.Execute(testFromAccount.Id, 10.0m);
            }
            catch (InvalidOperationException e)
            {
                StringAssert.Contains(e.Message, "Insufficient funds to make a withdrawal");
                return;
            }
            Assert.Fail("No exception was thrown.");

        }

        [TestMethod]
        public void TestNegativeWithdraw()
        {
            WithdrawMoney withdrawMoneyFeature = new WithdrawMoney(accountRepository.Object, notificationService.Object);

            try
            {
                withdrawMoneyFeature.Execute(testFromAccount.Id, -10.0m);
            }
            catch (InvalidOperationException e)
            {
                StringAssert.Contains(e.Message, "Cannot withdraw a negative amount");
                return;
            }
            Assert.Fail("No exception was thrown.");

        }

        [TestMethod]
        public void TestNegativeTransfer()
        {
            TransferMoney transferMoneyFeature = new TransferMoney(accountRepository.Object, notificationService.Object);

            try
            {
                transferMoneyFeature.Execute(this.testFromAccount.Id, this.testToAccount.Id, -10.0m);
            }
            catch (InvalidOperationException e)
            {
                StringAssert.Contains(e.Message, "Cannot transfer a negative amount");
                return;
            }
            Assert.Fail("No exception was thrown.");

        }
    }
}
