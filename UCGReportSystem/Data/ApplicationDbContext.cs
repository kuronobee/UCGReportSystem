using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCGReportSystem.Models;

namespace UCGReportSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Patient> Patients { get; set; }
        public DbSet<EchoReport> EchoReports { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=echoReports.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 患者とエコーレポートの関係を定義
            modelBuilder.Entity<EchoReport>()
                .HasOne(e => e.Patient)
                .WithMany()
                .HasForeignKey(e => e.PatientId);
        }
    }
}
