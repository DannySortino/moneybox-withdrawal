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
        User testFromUser, testToUser;
        Mock<IAccountRepository> accountRepository;
        Mock<INotificationService> notificationService;


        public MoneyboxTests()
        {
            this.testFromUser = new User(
                id : Guid.NewGuid(),
                name : "testFrom",
                email : "testFrom@test.com");


            this.testFromAccount = new Account(
                id : Guid.NewGuid(),
                user : testFromUser,
                balance : 100m,
                paidIn : 200m,
                withdrawn : 50m);

            this.testToUser = new User(
                id: Guid.NewGuid(),
                name: "testTo",
                email: "testTo@test.com");


            this.testToAccount = new Account(
                id: Guid.NewGuid(),
                user: testToUser,
                balance: 100m,
                paidIn: 200m,
                withdrawn: 50m);

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
            this.testFromAccount = new Account(
                id: Guid.NewGuid(),
                user: testFromUser,
                balance: 5.0m,
                paidIn: 200m,
                withdrawn: 50m);
            this.accountRepository.Setup(x => x.GetAccountById(this.testFromAccount.Id)).Returns(() => this.testFromAccount);

            decimal startFromBalance = this.testFromAccount.Balance;

            TransferMoney transferMoneyFeature = new TransferMoney(accountRepository.Object, notificationService.Object);
            try
            {
                transferMoneyFeature.Execute(this.testFromAccount.Id, this.testToAccount.Id, 10.0m);
            }
            catch (InvalidOperationException e)
            {
                StringAssert.Contains(e.Message, "Insufficient funds to complete action");
                return;
            }
            Assert.Fail("No exception was thrown.");
        }

        [TestMethod]
        public void TestPayinLimitTransfer()
        {
            this.testToAccount = new Account(
               id: Guid.NewGuid(),
               user: testToUser,
               balance: 100.0m,
               paidIn: Account.PayInLimit,
               withdrawn: 50m);
            this.accountRepository.Setup(x => x.GetAccountById(this.testToAccount.Id)).Returns(() => this.testToAccount);

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
            this.testFromAccount = new Account(
                id: Guid.NewGuid(),
                user: testFromUser,
                balance: 1.0m,
                paidIn: 200m,
                withdrawn: 50m);
            this.accountRepository.Setup(x => x.GetAccountById(this.testFromAccount.Id)).Returns(() => this.testFromAccount);


            WithdrawMoney withdrawMoneyFeature = new WithdrawMoney(accountRepository.Object, notificationService.Object);

            try
            {
                withdrawMoneyFeature.Execute(testFromAccount.Id, 10.0m);
            }
            catch (InvalidOperationException e)
            {
                StringAssert.Contains(e.Message, "Insufficient funds to complete action");
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
                StringAssert.Contains(e.Message, "Cannot complete action with a negative amount");
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
                StringAssert.Contains(e.Message, "Cannot complete action with a negative amount");
                return;
            }
            Assert.Fail("No exception was thrown.");

        }
    }
}
