using BusinessLayer.IRepository;
using BusinessLayer.Model;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADO.NET.Repos
{
    public class CustomerRepository : ICustomerRepository
    {
        private SqlConnection context { get; set; }

        public CustomerRepository(SqlConnection context)
        {
            this.context = context;
        }

        public Klant Add(Klant a)
        {
            int id = -1;
            String cmd = "INSERT INTO [dbo].[CUSTOMER] (NAME, ADDRESS) VALUES (@Name, @Address); SELECT CAST(scope_identity() AS int)";
            using (var insertCmd = new SqlCommand(cmd, this.context))
            {
                insertCmd.Parameters.AddWithValue("@Name", a.Naam);
                insertCmd.Parameters.AddWithValue("@Address", a.Adres);
                try
                {
                    context.Open();
                    id = (int)insertCmd.ExecuteScalar();
                    context.Close();
                }
                catch (Exception) { throw new InsertException(); }
            }
            if (id < 0) throw new SqlException();
            return new Author(id, a.Firstname, a.Surname);
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public void DeleteAll()
        {
            throw new NotImplementedException();
        }

        public bool Exist(Klant a, bool ignoreId = false)
        {
            throw new NotImplementedException();
        }

        public List<Klant> GetAll()
        {
            throw new NotImplementedException();
        }

        public Klant GetByID(int id)
        {
            throw new NotImplementedException();
        }

        public bool HasStrips(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(Klant a)
        {
            throw new NotImplementedException();
        }
    }
}
