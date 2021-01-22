using BusinessLayer;
using BusinessLayer.IRepository;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using ADO.NET.Repos;

namespace ADO.NET
{
    public class UnitOfWork : IUnitOfWork
    {
        private String connectionString;
        private SqlConnection connection;

        public UnitOfWork(String environment = "development")
        {
            //setConnectionString(environment);
            connection = new SqlConnection("Data Source=DESKTOP-7B94T84\\sqlexpress;Initial Catalog=Bestellingssysteem;Integrated Security=True");

            Customers = new CustomerRepository(connection);
            Orders = new OrderRepository(connection);
            Products = new ProductRepository(connection);
        }

        public void setConnectionString(String environment)
        {
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false);
            var configuration = builder.Build();
            if (environment.ToLower() == "development")
                connectionString = configuration.GetConnectionString("Development").ToString();
            else
                connectionString = configuration.GetConnectionString("Production").ToString();
        }

        public ICustomerRepository Customers { get; private set; }
        public IOrderRepository Orders { get; private set; }
        public IProductRepository Products { get; private set; }
    }
}
