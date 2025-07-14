using BudgetManager.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

public class BudgetManagerDbContext : DbContext
{
    public DbSet<Category> Categories { set; get; }
    public DbSet<Expense> Expenses { set; get; }
    public DbSet<User> Users { get; set; }


    public BudgetManagerDbContext(DbContextOptions<BudgetManagerDbContext> options)
         : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(c => c.CategoryID);
            entity.Property(c => c.Name).IsRequired();
        });

        modelBuilder.Entity<Expense>(entity =>
        {
            entity.HasKey(e => e.ExpenseID);
            entity.Property(e => e.Date).IsRequired();
            entity.Property(e => e.Price).IsRequired().HasColumnType("decimal(18, 2)");
            entity.HasOne(e => e.Category)
            .WithMany(c => c.Expenses)
            .HasForeignKey(e => e.CategoryID);
        });

        modelBuilder.Entity<Category>().HasData(
           new Category { CategoryID = 1, Name = "Groceries" },
           new Category { CategoryID = 2, Name = "Transport" },
           new Category { CategoryID = 3, Name = "Mobile Communication" },
           new Category { CategoryID = 4, Name = "Internet" },
           new Category { CategoryID = 5, Name = "Entertainment" });

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();


    }
}