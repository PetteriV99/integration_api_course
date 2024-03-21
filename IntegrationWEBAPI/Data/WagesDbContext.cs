using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;

public class WagesDbContext : DbContext
{
    public WagesDbContext() : base("name=YourConnectionString")
    {
    }

    public DbSet<Wage> Wages { get; set; }

    public Wage CreateWage(Wage wage)
    {
        var newWage = Wages.Add(wage);
        SaveChanges();
        return newWage;
    }
}
