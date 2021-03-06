﻿
namespace Sharezbold.FileMigration.Unittests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.SharePoint.Client;

    [TestClass]
    public class FileMigrationBuilderTest
    {
        [TestMethod]
        public void TestBuildfileMigratorWithoutSourceClientContext()
        {
            ClientContext clientContext = new ClientContext("http://blub");

            FileMigrationBuilder builder = FileMigrationBuilder.GetNewFileMigrationBuilder();
            builder.WithTargetClientContext(clientContext);
            builder.WithBandwith(10);
            builder.WithServiceAddress(new Uri("http://Address"));

            try
            {
                SharePoint2010And2013Migrator migrator = builder.CreateMigrator();
                Assert.Fail("Expected FileMigrationException");
            }
            catch (ValidationException)
            {
                // ok
            }
        }

        [TestMethod]
        public void TestBuildfileMigratorWithoutTargetClientContext()
        {
            ClientContext clientContext = new ClientContext("http://blub");

            FileMigrationBuilder builder = FileMigrationBuilder.GetNewFileMigrationBuilder();
            builder.WithSourceClientContext(clientContext);
            builder.WithBandwith(10);
            builder.WithServiceAddress(new Uri("http://Address"));

            try
            {
                SharePoint2010And2013Migrator migrator = builder.CreateMigrator();
                Assert.Fail("Expected FileMigrationException");
            }
            catch (ValidationException)
            {
                // ok
            }
        }

        [TestMethod]
        public void TestBuildfileMigratorWithoutServiceAddress()
        {
            ClientContext clientContext = new ClientContext("http://blub");

            FileMigrationBuilder builder = FileMigrationBuilder.GetNewFileMigrationBuilder();
            builder.WithTargetClientContext(clientContext);
            builder.WithSourceClientContext(clientContext);
            builder.WithBandwith(10);

            try
            {
                SharePoint2010And2013Migrator migrator = builder.CreateMigrator();
                Assert.Fail("Expected FileMigrationException");
            }
            catch (ValidationException)
            {
                // ok
            }
        }

        [TestMethod]
        public void TestBuildfileMigratorWithoutBandwith()
        {
            ClientContext clientContext = new ClientContext("http://blub");

            FileMigrationBuilder builder = FileMigrationBuilder.GetNewFileMigrationBuilder();
            builder.WithTargetClientContext(clientContext);
            builder.WithSourceClientContext(clientContext);
            builder.WithServiceAddress(new Uri("http://bulub"));

            SharePoint2010And2013Migrator migrator = builder.CreateMigrator();

            Assert.IsNotNull(migrator);
            Assert.IsTrue(typeof(SharePoint2010And2013Migrator) == migrator.GetType());
        }

        [TestMethod]
        public void TestBuildFileMigratorSuccessful()
        {
            ClientContext clientContext = new ClientContext("http://blub");

            FileMigrationBuilder builder = FileMigrationBuilder.GetNewFileMigrationBuilder();
            builder.WithTargetClientContext(clientContext);
            builder.WithSourceClientContext(clientContext);
            builder.WithBandwith(10);
            builder.WithServiceAddress(new Uri("http://address"));

            SharePoint2010And2013Migrator migrator = builder.CreateMigrator();
            
            Assert.IsNotNull(migrator);
            Assert.IsTrue(typeof(SharePoint2010And2013Migrator) == migrator.GetType());
        }
    }
}
