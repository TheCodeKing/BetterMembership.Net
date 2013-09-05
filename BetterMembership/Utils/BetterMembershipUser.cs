namespace BetterMembershipProvider.Utils
{
    using System;
    using System.Web.Security;

    using CuttingEdge.Conditions;

    internal class BetterMembershipUser : MembershipUser
    {
        private readonly Func<bool> isLockedOutDelegate;

        public BetterMembershipUser(string providerName, string name, object providerUserKey, string email, string passwordQuestion, string comment, bool isApproved, Func<bool> isLockedOutDelegate, DateTime creationDate, DateTime lastLoginDate, DateTime lastActivityDate, DateTime lastPasswordChangedDate, DateTime lastLockoutDate)
            : base(providerName, name, providerUserKey, email, passwordQuestion, comment, isApproved, false, creationDate, lastLoginDate, lastActivityDate, lastPasswordChangedDate, lastLockoutDate)

        {
            Condition.Requires(isLockedOutDelegate, "isLockedOutDelegate").IsNotNull();

            this.isLockedOutDelegate = isLockedOutDelegate;
        }

        public override bool IsLockedOut
        {
            get
            {
                return isLockedOutDelegate();
            }
        }
    }
}