namespace BetterMembership.Web
{
    using System;
    using System.Web.Security;

    using CuttingEdge.Conditions;

    [Serializable]
    internal class BetterMembershipUser : MembershipUser
    {
        private readonly bool isEmailSupported;

        private readonly Func<bool> isLockedOutDelegate;

        public BetterMembershipUser(
            string providerName, 
            string name, 
            object providerUserKey, 
            string email, 
            string passwordQuestion, 
            string comment, 
            bool isApproved, 
            Func<bool> isLockedOutDelegate, 
            DateTime creationDate, 
            DateTime lastLoginDate, 
            DateTime lastActivityDate, 
            DateTime lastPasswordChangedDate, 
            DateTime lastLockoutDate, 
            bool isEmailSupported)
            : base(
                providerName, 
                name, 
                providerUserKey, 
                email, 
                passwordQuestion, 
                comment, 
                isApproved, 
                false, 
                creationDate, 
                lastLoginDate, 
                lastActivityDate, 
                lastPasswordChangedDate, 
                lastLockoutDate)
        {
            Condition.Requires(isLockedOutDelegate, "isLockedOutDelegate").IsNotNull();

            this.isLockedOutDelegate = isLockedOutDelegate;
            this.isEmailSupported = isEmailSupported;
        }

        public override string Comment
        {
            get
            {
                return string.Empty;
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        public override string Email
        {
            get
            {
                return base.Email;
            }

            set
            {
                if (!this.isEmailSupported)
                {
                    throw new NotSupportedException();
                }

                base.Email = value;
            }
        }

        public override bool IsLockedOut
        {
            get
            {
                return this.isLockedOutDelegate();
            }
        }

        public override bool IsOnline
        {
            get
            {
                throw new NotSupportedException();
            }
        }
    }
}