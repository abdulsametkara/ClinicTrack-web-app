using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClinickCore.Entities;

namespace ClinickDataAccess
{
    public class DatabaseBaglanti : DbContext
    {
        public DatabaseBaglanti(DbContextOptions<DatabaseBaglanti> options) : base(options)
        {
        }

        #region Tablolar
        public DbSet<Doktor> Doktorlar { get; set; }
        public DbSet<Hasta> Hastalar { get; set; }
        public DbSet<Kullanıcı> Kullanıcılar { get; set; }
        public DbSet<Randevu> Randevular { get; set; }
        public DbSet<Uzmanlık> Uzmanlıklar { get; set; }
        #endregion


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Migration için fallback (Program.cs'den inject edilirse bu çalışmaz)
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Clinick;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // İlk Admin Kullanıcısı (Seed Data)
            // Giriş: admin@clinicktrack.com / admin123
            const string adminPasswordHash = "jZae727K08KaOmKSgOaGzww/XVqGr/PKEgIMkjrcbJI=";
            
            modelBuilder.Entity<Kullanıcı>().HasData(new Kullanıcı
            {
                Id = 1,
                İsim = "Admin",
                Soyisim = "User",
                TCNo = "12345678901",
                Email = "admin@clinicktrack.com",
                Parola = adminPasswordHash,
                Rol = "Admin",
                OluşturulmaTarihi = new DateTime(2024, 1, 1)
            });
        }
    }

}
