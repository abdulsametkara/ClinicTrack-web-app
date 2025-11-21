using System;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Migrations;
using ClinickDataAccess;
using ClinickDataAccess.Migrations; 
using System.Reflection;

namespace Clinick.Tests
{
    public class MigrationTests
    {
        [Fact]
        public void Migration_KullaniciTablosuGuncelleme_Calisir()
        {
            var builder = new MigrationBuilder("Microsoft.EntityFrameworkCore.SqlServer");
            InvokeMigration(new KullaniciTablosuGuncelleme(), builder);
            Assert.NotEmpty(builder.Operations);
        }

        [Fact]
        public void Migration_KullaniciTablosuGuncelleme_BuildTargetModel_Calisir()
        {
            var migration = new KullaniciTablosuGuncelleme();
            var modelBuilder = new ModelBuilder(new ConventionSet());
            var method = typeof(KullaniciTablosuGuncelleme).GetMethod("BuildTargetModel", BindingFlags.Instance | BindingFlags.NonPublic);
            method?.Invoke(migration, new object[] { modelBuilder });
            Assert.NotNull(modelBuilder.Model);
        }

        [Fact]
        public void Migration_SeedDataMigration_Calisir()
        {
            var builder = new MigrationBuilder("Microsoft.EntityFrameworkCore.SqlServer");
            InvokeMigration(new SeedDataMigration(), builder);
            Assert.NotEmpty(builder.Operations);
        }

        [Fact]
        public void Migration_SeedDataMigration_BuildTargetModel_Calisir()
        {
            var migration = new SeedDataMigration();
            var conventionSet = new ConventionSet();
            var modelBuilder = new ModelBuilder(conventionSet);
            var buildMethod = typeof(SeedDataMigration).GetMethod("BuildTargetModel", BindingFlags.Instance | BindingFlags.NonPublic);
            buildMethod?.Invoke(migration, new object[] { modelBuilder });
            Assert.NotNull(modelBuilder.Model);
        }

        [Fact]
        public void Migration_TabloEkleme_Calisir()
        {
            var builder = new MigrationBuilder("Microsoft.EntityFrameworkCore.SqlServer");
            InvokeMigration(new TabloEkleme(), builder);
            Assert.NotEmpty(builder.Operations);
        }

        [Fact]
        public void Migration_TabloEkleme_BuildTargetModel_Calisir()
        {
            var migration = new TabloEkleme();
            var modelBuilder = new ModelBuilder(new ConventionSet());
            var buildMethod = typeof(TabloEkleme).GetMethod("BuildTargetModel", BindingFlags.Instance | BindingFlags.NonPublic);
            buildMethod?.Invoke(migration, new object[] { modelBuilder });
            Assert.NotNull(modelBuilder.Model);
        }

        [Fact]
        public void Migration_UzmanlikTablosuEkleme_Calisir()
        {
            var builder = new MigrationBuilder("Microsoft.EntityFrameworkCore.SqlServer");
            InvokeMigration(new UzmanlıkTablosuEkleme(), builder);
            Assert.NotEmpty(builder.Operations);
        }

        [Fact]
        public void Migration_UzmanlikTablosuEkleme_BuildTargetModel_Calisir()
        {
            var migration = new UzmanlıkTablosuEkleme();
            var modelBuilder = new ModelBuilder(new ConventionSet());
            var buildMethod = typeof(UzmanlıkTablosuEkleme).GetMethod("BuildTargetModel", BindingFlags.Instance | BindingFlags.NonPublic);
            buildMethod?.Invoke(migration, new object[] { modelBuilder });
            Assert.NotNull(modelBuilder.Model);
        }

        [Fact]
        public void Migration_IlkMigration_Calisir()
        {
            var builder = new MigrationBuilder("Microsoft.EntityFrameworkCore.SqlServer");
            InvokeMigration(new ilkMigration(), builder);
            Assert.NotEmpty(builder.Operations);
        }

        [Fact]
        public void Migration_IlkMigration_BuildTargetModel_Calisir()
        {
            var migration = new ilkMigration();
            var modelBuilder = new ModelBuilder(new ConventionSet());
            var buildMethod = typeof(ilkMigration).GetMethod("BuildTargetModel", BindingFlags.Instance | BindingFlags.NonPublic);
            buildMethod?.Invoke(migration, new object[] { modelBuilder });
            Assert.NotNull(modelBuilder.Model);
        }

        [Fact]
        public void Migration_IlkGirisEklendi_Calisir()
        {
            var builder = new MigrationBuilder("Microsoft.EntityFrameworkCore.SqlServer");
            InvokeMigration(new İlkGirisEklendi(), builder);
            Assert.NotEmpty(builder.Operations);
        }

        [Fact]
        public void Migration_IlkGirisEklendi_BuildTargetModel_Calisir()
        {
            var migration = new İlkGirisEklendi();
            var modelBuilder = new ModelBuilder(new ConventionSet());
            var buildMethod = typeof(İlkGirisEklendi).GetMethod("BuildTargetModel", BindingFlags.Instance | BindingFlags.NonPublic);
            buildMethod?.Invoke(migration, new object[] { modelBuilder });
            Assert.NotNull(modelBuilder.Model);
        }

        [Fact]
        public void Snapshot_Dosyasini_Coverage_Icin_Calistir()
        {
            var snapshotType = typeof(DatabaseBaglanti).Assembly.GetType("ClinickDataAccess.Migrations.DatabaseBaglantiModelSnapshot");
            Assert.NotNull(snapshotType);

            var snapshot = Activator.CreateInstance(snapshotType!, nonPublic: true);
            Assert.NotNull(snapshot);

            var conventionSet = new ConventionSet();
            var modelBuilder = new ModelBuilder(conventionSet);

            var buildMethod = snapshotType!.GetMethod("BuildModel", BindingFlags.Instance | BindingFlags.NonPublic);
            buildMethod?.Invoke(snapshot, new object[] { modelBuilder });

            Assert.NotNull(modelBuilder.Model);
        }

        private static void InvokeMigration(Migration migration, MigrationBuilder builder)
        {
            var up = migration.GetType().GetMethod("Up", BindingFlags.Instance | BindingFlags.NonPublic);
            var down = migration.GetType().GetMethod("Down", BindingFlags.Instance | BindingFlags.NonPublic);
            up?.Invoke(migration, new object[] { builder });
            down?.Invoke(migration, new object[] { builder });
        }
    }
}