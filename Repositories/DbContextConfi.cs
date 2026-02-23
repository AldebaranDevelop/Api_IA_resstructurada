using Api_IA.Models.Items;
using ApiDrones.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace ApiDrones.Repositories
{
    public class DbContextConfi : DbContext
    {
        
        public DbSet<Executions> Executions { get; set; }
        public DbSet<Images> Images { get; set; }
        public DbSet<ProcessedImages2> ProcessedImages2 { get; set; }
        public DbSet<Predictions2> Predictions2 { get; set; }
        public DbSet<ResponseItems> ResponseItems { get; set; }


        public DbContextConfi(DbContextOptions<DbContextConfi> options)
            : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Executions>()
                .Property(e => e.Sql_created_time)
                .HasDefaultValueSql("GETDATE()") 
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Images>()
                .Property(e => e.Sql_created_time)
                .HasDefaultValueSql("GETDATE()")
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<ResponseItems>().HasNoKey();
        }
    }
}
