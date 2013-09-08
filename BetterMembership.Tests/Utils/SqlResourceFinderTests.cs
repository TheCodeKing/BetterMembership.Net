namespace BetterMembership.Tests.Utils
{
    using System.IO;
    using System.Reflection;

    using BetterMembership.Facades;
    using BetterMembership.Utils;

    using Moq;

    using NUnit.Framework;

    using StructureMap.AutoMocking;

    [TestFixture]
    public class SqlResourceFinderTests
    {
        private MoqAutoMocker<SqlResourceFinder> autoMocker;

        [SetUp]
        public void SetUp()
        {
            autoMocker = new MoqAutoMocker<SqlResourceFinder>();
        }

        [Test]
        public void GivenInvalidResourcePathWhenResourceRequestedThenFileNotFoundException()
        {
            // arrange
            var testClass = autoMocker.ClassUnderTest;
            var resourceManifestMock = Mock.Get(autoMocker.Get<IResourceManifestFacade>());
            resourceManifestMock.Setup(x => x.GetManifestResourceStream(It.IsAny<string>())).Returns<Stream>(null);

            // act
            Assert.Throws<FileNotFoundException>(() => testClass.LocateScript("test", "test"));

        }
    }
}