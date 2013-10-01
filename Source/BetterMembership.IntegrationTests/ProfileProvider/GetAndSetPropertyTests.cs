namespace BetterMembership.IntegrationTests.ProfileProvider
{
    using System;
    using System.Configuration;

    using BetterMembership.IntegrationTests.Helpers;

    using NUnit.Framework;

    [TestFixture]
    public class GetAndSetPropertyTests : BaseProfileProviderTests
    {
        [TestCase(SqlClientCeProfileProviderWithEmail, BaseMembershipTests.SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeProfileProviderWithoutEmail, BaseMembershipTests.SqlClientCeProviderWithoutEmail)]
        [TestCase(SqlClientCeProfileProviderWithUniqueEmail, BaseMembershipTests.SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientProfileProviderWithEmail, BaseMembershipTests.SqlClientProviderWithEmail)]
        [TestCase(SqlClientProfileProviderWithoutEmail, BaseMembershipTests.SqlClientProviderWithoutEmail)]
        [TestCase(SqlClientProfileProviderWithUniqueEmail, BaseMembershipTests.SqlClientProviderWithUniqueEmail)]
        public void GivenConfirmedUsersWhenSetPropertyValuesWithValidColumnsThenSuccess(
            string providerName, string membershipProviderName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var memProvider = this.WithMembershipProvider(membershipProviderName);
            var user = memProvider.WithConfirmedUser().Value;
            var context = new SettingsContext();
            context["UserName"] = user.UserName;
            var properties = new SettingsPropertyValueCollection();
            if (memProvider.AsBetter().HasEmailColumnDefined)
            {
                var emailProperty = new SettingsProperty(memProvider.AsBetter().UserEmailColumn)
                                        {
                                            PropertyType =
                                                typeof(string)
                                        };
                properties.Add(
                    new SettingsPropertyValue(emailProperty) { PropertyValue = user.Email, Deserialized = true });
            }

            // act // assert
            Assert.DoesNotThrow(() => testClass.SetPropertyValues(context, properties));
        }

        [TestCase(SqlClientCeProfileProviderWithEmail, BaseMembershipTests.SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeProfileProviderWithoutEmail, BaseMembershipTests.SqlClientCeProviderWithoutEmail)]
        [TestCase(SqlClientCeProfileProviderWithUniqueEmail, BaseMembershipTests.SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientProfileProviderWithEmail, BaseMembershipTests.SqlClientProviderWithEmail)]
        [TestCase(SqlClientProfileProviderWithoutEmail, BaseMembershipTests.SqlClientProviderWithoutEmail)]
        [TestCase(SqlClientProfileProviderWithUniqueEmail, BaseMembershipTests.SqlClientProviderWithUniqueEmail)]
        public void GivenConfirmedUsersWhenSetPropertyValuesWithInvalidColumnsThenThrowNotSupportedException(
            string providerName, string membershipProviderName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var memProvider = this.WithMembershipProvider(membershipProviderName);
            var user = memProvider.WithConfirmedUser().Value;
            var context = new SettingsContext();
            context["UserName"] = user.UserName;
            var properties = new SettingsPropertyValueCollection
                                 {
                                     new SettingsPropertyValue(
                                         new SettingsProperty("invalidColumn")
                                             {
                                                 PropertyType = typeof(string)
                                             })
                                         {
                                             SerializedValue
                                                 =
                                                 "Value"
                                         }
                                 };
            if (memProvider.AsBetter().HasEmailColumnDefined)
            {
                var emailProperty = new SettingsProperty(memProvider.AsBetter().UserEmailColumn)
                                        {
                                            PropertyType =
                                                typeof(string)
                                        };
                properties.Add(
                    new SettingsPropertyValue(emailProperty) { PropertyValue = user.Email, Deserialized = true });
            }

            // act // assert
            Assert.Throws<NotSupportedException>(() => testClass.SetPropertyValues(context, properties));
        }

        [TestCase(SqlClientCeProfileProviderWithEmail, BaseMembershipTests.SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeProfileProviderWithoutEmail, BaseMembershipTests.SqlClientCeProviderWithoutEmail)]
        [TestCase(SqlClientCeProfileProviderWithUniqueEmail, BaseMembershipTests.SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientProfileProviderWithEmail, BaseMembershipTests.SqlClientProviderWithEmail)]
        [TestCase(SqlClientProfileProviderWithoutEmail, BaseMembershipTests.SqlClientProviderWithoutEmail)]
        [TestCase(SqlClientProfileProviderWithUniqueEmail, BaseMembershipTests.SqlClientProviderWithUniqueEmail)]
        public void GivenConfirmedUsersWhenGetPropertyValuesWithInvalidColumnsThenNoException(
            string providerName, string membershipProviderName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var memProvider = this.WithMembershipProvider(membershipProviderName);
            var user = memProvider.WithConfirmedUser().Value;
            var context = new SettingsContext();
            context["UserName"] = user.UserName;
            var properties = new SettingsPropertyCollection
                                 {
                                     new SettingsProperty("invalidColumn")
                                         {
                                             PropertyType = typeof(string)
                                         }
                                 };
            if (memProvider.AsBetter().HasEmailColumnDefined)
            {
                properties.Add(
                    new SettingsProperty(memProvider.AsBetter().UserEmailColumn) { PropertyType = typeof(string) });
            }

            // act // assert
            Assert.DoesNotThrow(() => testClass.GetPropertyValues(context, properties));
        }

        [TestCase(SqlClientCeProfileProviderWithEmail, BaseMembershipTests.SqlClientCeProviderWithEmail)]
        [TestCase(SqlClientCeProfileProviderWithoutEmail, BaseMembershipTests.SqlClientCeProviderWithoutEmail)]
        [TestCase(SqlClientCeProfileProviderWithUniqueEmail, BaseMembershipTests.SqlClientCeProviderWithUniqueEmail)]
        [TestCase(SqlClientProfileProviderWithEmail, BaseMembershipTests.SqlClientProviderWithEmail)]
        [TestCase(SqlClientProfileProviderWithoutEmail, BaseMembershipTests.SqlClientProviderWithoutEmail)]
        [TestCase(SqlClientProfileProviderWithUniqueEmail, BaseMembershipTests.SqlClientProviderWithUniqueEmail)]
        public void GivenConfirmedUsersWhenGetPropertyValuesWithValidColumnsThenPropertyReturnedSuccessfully(
            string providerName, string membershipProviderName)
        {
            // arrange
            var testClass = this.WithProvider(providerName);
            var memProvider = this.WithMembershipProvider(membershipProviderName);
            var user = memProvider.WithConfirmedUser().Value;
            var context = new SettingsContext();
            context["UserName"] = user.UserName;
            var properties = new SettingsPropertyCollection();
            if (memProvider.AsBetter().HasEmailColumnDefined)
            {
                properties.Add(
                    new SettingsProperty(memProvider.AsBetter().UserEmailColumn) { PropertyType = typeof(string) });
            }

            properties.Add(new SettingsProperty(memProvider.AsBetter().UserIdColumn) { PropertyType = typeof(int) });

            // act
            var profile = testClass.GetPropertyValues(context, properties);

            // assert
            Assert.That(profile, Is.Not.Null);
            Assert.That(profile[memProvider.AsBetter().UserIdColumn], Is.Not.Null);
            Assert.That(profile[memProvider.AsBetter().UserIdColumn].PropertyValue, Is.GreaterThan(0));
            Assert.That(profile[memProvider.AsBetter().UserIdColumn].IsDirty, Is.False);
            if (memProvider.AsBetter().HasEmailColumnDefined)
            {
                Assert.That(profile[memProvider.AsBetter().UserEmailColumn].PropertyValue, Is.EqualTo(user.Email));
                Assert.That(profile[memProvider.AsBetter().UserEmailColumn].IsDirty, Is.False);
            }
        }
    }
}