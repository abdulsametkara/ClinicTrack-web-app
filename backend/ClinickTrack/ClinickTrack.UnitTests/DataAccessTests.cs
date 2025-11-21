using Xunit;
using ClinickDataAccess;
using Microsoft.EntityFrameworkCore;
using ClinickCore.Entities;
using ClinickDataAccess.Repository;
using System.Collections.Generic;
using System.Linq;

public class DataAccessTests
{
    [Fact]
    public void DatabaseBaglanti_Property_Accessors_Work()
    {
        // InMemory db ile context newle
        var opts = new DbContextOptionsBuilder<DatabaseBaglanti>()
            .UseInMemoryDatabase("unit_test_db1").Options;
        var db = new DatabaseBaglanti(opts);
        Assert.NotNull(db);
        Assert.NotNull(db.Doktorlar);
        Assert.NotNull(db.Hastalar);
        Assert.NotNull(db.Kullanıcılar);
        Assert.NotNull(db.Randevular);
        Assert.NotNull(db.Uzmanlıklar);
    }

    [Fact]
    public void GenericRepository_CRUD_Works_With_InMemory()
    {
        // Arrange
        var opts = new DbContextOptionsBuilder<DatabaseBaglanti>()
            .UseInMemoryDatabase("unit_test_db2").Options;
        var db = new DatabaseBaglanti(opts);
        var repo = new GenericRepository<Kullanıcı>(db);
        var entity = new Kullanıcı
        {
            İsim = "A",
            Soyisim = "B",
            TCNo = "12345678901",
            Email = "test@email.com",
            Parola = "pass1",
            Rol = "Hasta"
        };
        // CREATE
        repo.Create(entity);
        Assert.Equal(1, db.Kullanıcılar.Count());
        // GETALL
        var all = repo.GetAll().ToList();
        Assert.Single(all);
        // GETBYID
        var id = all[0].Id;
        var e2 = repo.GetById(id);
        Assert.NotNull(e2);
        // UPDATE
        e2.Soyisim = "C"; repo.Update(e2);
        Assert.Equal("C", db.Kullanıcılar.First().Soyisim);
        // DELETE
        repo.Delete(e2);
        Assert.Empty(db.Kullanıcılar);
    }

    [Fact]
    public void GenericRepository_DeleteByRange_Works()
    {
        var opts = new DbContextOptionsBuilder<DatabaseBaglanti>()
            .UseInMemoryDatabase("unit_test_db3").Options;
        var db = new DatabaseBaglanti(opts);
        var repo = new GenericRepository<Kullanıcı>(db);
        var u1 = new Kullanıcı {
            İsim = "u1",
            Soyisim = "S1",
            TCNo = "10000000001",
            Email = "u1@e.com",
            Parola = "p1",
            Rol = "R1"
        };
        var u2 = new Kullanıcı {
            İsim = "u2",
            Soyisim = "S2",
            TCNo = "10000000002",
            Email = "u2@e.com",
            Parola = "p2",
            Rol = "R2"
        };
        repo.Create(u1); repo.Create(u2);
        Assert.Equal(2, db.Kullanıcılar.Count());
        repo.DeleteByRange(db.Kullanıcılar.ToList());
        Assert.Empty(repo.GetAll());
    }

    [Fact]
    public void CanInstantiate_KullaniciTablosuGuncelleme()
    {
        var migration = new ClinickDataAccess.Migrations.KullaniciTablosuGuncelleme();
        Assert.NotNull(migration);
    }

    [Fact]
    public void CanInstantiate_SeedDataMigration()
    {
        var migration = new ClinickDataAccess.Migrations.SeedDataMigration();
        Assert.NotNull(migration);
    }
}
