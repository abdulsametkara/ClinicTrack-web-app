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
        #region Tablolar
        public DbSet<Doktor> Doktorlar { get; set; }
        public DbSet<Hasta> Hastalar { get; set; }
        public DbSet<Kullanıcı> Kullanıcılar { get; set; }
        public DbSet<Randevu> Randevular { get; set; }
        public DbSet<Uzmanlık> Uzmanlıklar { get; set; }
        #endregion


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Clinick;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
        }
    }

}
