using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;

public class CustomersDbContext : DbContext
{
    public CustomersDbContext() : base("name=YourConnectionString")
    {
    }

    public DbSet<Customer> Customers { get; set; }

    public List<Customer> GetCustomers()
    {
        return Customers.ToList();
    }
    
    public Customer GetCustomer(int id)
    {
        return Customers.Find(id);
    }
    public Customer CreateCustomerById(Customer customer)
    {
        var newCustomer = Customers.Add(customer);
        SaveChanges();
        return newCustomer;
    }

    public Customer UpdateCustomer(Customer customer)
    {
        var existingCustomer = Customers.Find(customer.Id);

        if (existingCustomer != null)
        {
            existingCustomer.Name = customer.Name; 
            SaveChanges();
        }

        return existingCustomer;

    }

    public bool DeleteCustomer(int id)
    {
        var existingCustomer = Customers.Find(id);
        if (existingCustomer != null)
        {
            Customers.Remove(existingCustomer);
            SaveChanges();
            return true;
        }
        return false;
    }

}
