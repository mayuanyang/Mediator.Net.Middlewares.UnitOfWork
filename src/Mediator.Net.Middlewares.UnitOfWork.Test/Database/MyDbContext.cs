﻿using System.Data.Entity;

namespace Mediator.Net.Middlewares.UnitOfWork.Test.Database
{
    public class MyDbContext : DbContext
    {
        public MyDbContext() : base("DefaultConnection") //: base($"Data Source=(localdb)\\v11.0; AttachDbFilename = {Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestUnitOfWork.mdf")}; Integrated Security = True; Connect Timeout = 30; User Instance = True")
        {
           
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Person>().ToTable("Person").HasKey(x => x.Id);
            modelBuilder.Entity<Car>().ToTable("Car").HasKey(x => x.Id);
        }

        public DbSet<Person> Persons { get; set; }
        public DbSet<Car> Cars { get; set; }
    }
}
