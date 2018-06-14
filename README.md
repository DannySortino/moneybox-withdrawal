# Moneybox Money Withdrawal

The solution contains a .NET core library (Moneybox.App) which is structured into the following 3 folders:

* Domain - this contains the domain models for a user and an account, and a notification service.
* Features - this contains two operations, one which is implemented (transfer money) and another which isn't (withdraw money)
* DataAccess - this contains a repository for retrieving and saving an account (and the nested user it belongs to)

## The task

The task is to implement a money withdrawal in the WithdrawMoney.Execute(...) method in the features folder. For consistency, the logic should be the same as the TransferMoney.Execute(...) method i.e. notifications for low funds and exceptions where the operation is not possible. 

As part of this process however, you should look to refactor some of the code in the TransferMoney.Execute(...) method into the domain models, and make these models less susceptible to misuse. We're looking to make our domain models rich in behaviour and much more than just plain old objects, however we don't want any data persistance operations (i.e. data access repositories) to bleed into our domain. This should simplify the task of implementing WidthdrawMoney.Execute(...).

## Guidlines

* You should spend no more than 1 hour on this task, although there is no time limit
* You should fork or copy this repository into your own public repository (Gihub, BitBucket etc.) before you do your work
* Your solution must compile and run first time
* You should not alter the notification service or the the account repository interfaces
* You may add unit/integration tests using a test framework (and/or mocking framework) of your choice
* You may edit this README.md if you want to give more details around your work (e.g. why you have done something a particular way, or anything else you would look to do but didn't have time)

Once you have completed your work, send us a link to your public repository.

Good luck!

## Reasons for changes made
* Decided to privatise the setting of the parameters for the account, as these should not be publicly accessible. There may need to be a method of manually setting these if they would have to be manually changed. However, that would presumably be an edge case, and therefore by importing access to the Account class, it should not then instantly provide access to setting balance amount etc, as this causes the fragility in the code and makes it easy for errors in changing balances elsewhere. This was therefore also done for the User domain.

* As the setting of the User and Account parameters were privatised, this meant a constructer was needed to provide default values for these parameters. 

* By moving code of modifying the balance from the Features into the Account service, it made it possible to privatise the parameters as described before. Also, this meant that the same code could be used for withdrawals and transfers. As transfers effectively can be considered a withdrawal of funds from one account, then a pay in for another account. 

* The tests provide a mechanism for checking the errors that are being checked for in the code cause the exceptions correctly, whilst also checking that the correct actions work correctly.

## What I would like to change if given more time
* Would be good to create an interface that does the checks, so this can be removed from the Domain.
* Would also be good to extract the notification checks to remove them from the validation checks in the Features.
