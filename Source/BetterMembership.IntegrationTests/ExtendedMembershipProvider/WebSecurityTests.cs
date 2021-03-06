﻿namespace BetterMembership.IntegrationTests.ExtendedMembershipProvider
{
    using System.Collections.Generic;

    using BetterMembership.IntegrationTests.Helpers;
    using BetterMembership.Web;

    using NUnit.Framework;

    using WebMatrix.WebData;

    [TestFixture]
    public class WebSecurityTests : BaseMembershipTests
    {
        [TestCase(SqlClientProviderWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientProviderWithoutEmail)]
        [TestCase(SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithoutEmail)]
        public void GivenMultipleProvidersWhenProviderUsedForCreateUserAndAccountThenUserAccountIsCreated(
            string providerName)
        {
            // arrange
            var testClass = this.WithExtendedProvider(providerName);
            var testUser = testClass.WithUnregisteredUser().Value;
            var profile = new Dictionary<string, object>();
            if (testClass.HasEmailColumnDefined())
            {
                profile.Add(testClass.AsBetter().UserEmailColumn, testUser.Email);
            }

            // act
            var result = testClass.CreateUserAndAccount(testUser.UserName, testUser.Password, true, profile);

            // assert
            Assert.That(result, Is.Not.Null);
        }

        [TestCase(SqlClientProviderWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientProviderWithoutEmail)]
        [TestCase(SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithoutEmail)]
        public void GivenUnConfirmedUserWhenLoginThenUserCannotAuthenticate(string providerName)
        {
            // arrange
            var testClass = this.WithExtendedProvider(providerName);
            var testUser = testClass.WithUnconfirmedUser().Value;

            // act
            var webSecurityResult = testClass.ValidateUser(testUser.UserName, testUser.Password);

            // assert
            Assert.That(webSecurityResult, Is.False);
        }

        [TestCase(SqlClientProviderWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientProviderWithoutEmail)]
        [TestCase(SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithoutEmail)]
        public void GivenUnConfirmedUserWhenMoreThanMaxNumberOfPasswordAttemptsThenAccountIsNotLockedOut(
            string providerName)
        {
            // arrange
            var testClass = this.WithExtendedProvider(providerName);
            var testUser =
                testClass.WithUnconfirmedUser()
                         .WithInvalidPasswordAttempts(testClass.MaxInvalidPasswordAttempts + 1)
                         .Value;

            // act
            var webSecurityIsLockedOut = testClass.IsAccountLockedOut(
                testUser.UserName, testClass.MaxInvalidPasswordAttempts, testClass.PasswordAttemptWindowInSeconds());

            // assert
            Assert.That(webSecurityIsLockedOut, Is.False);
        }

        [TestCase(SqlClientProviderWithEmail)]
        [TestCase(SqlClientProviderWithUniqueEmail)]
        [TestCase(SqlClientProviderWithoutEmail)]
        [TestCase(SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientCeProviderWithoutEmail)]
        public void GivenUnregisteredUserWhenCreateUserAndAccountThenUserAccountIsCreated(string providerName)
        {
            // arrange
            var testClass = this.WithExtendedProvider(providerName);
            var testUser = testClass.WithUnregisteredUser().Value;
            var profile = new Dictionary<string, object>();
            if (testClass.HasEmailColumnDefined())
            {
                profile.Add(testClass.AsBetter().UserEmailColumn, testUser.Email);
            }

            // act
            var id = testClass.CreateUserAndAccount(testUser.UserName, testUser.Password, true, profile);

            // assert
            Assert.That(id, Is.Not.Null);
        }
    }
}