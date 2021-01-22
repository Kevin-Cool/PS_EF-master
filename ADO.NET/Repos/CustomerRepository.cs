using BusinessLayer.IRepository;
using BusinessLayer.Model;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

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
                catch (Exception e) { throw e; }
            }
            if (id < 0) throw new NullReferenceException();
            return new Klant(id, a.Naam, a.Adres);
        }

        public void Delete(long id)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("DELETE FROM [dbo].[CUSTOMER] WHERE CUSTOMER_ID = @Id;", this.context);
                cmd.Parameters.AddWithValue("@Id", id);
                context.Open();
                cmd.ExecuteNonQuery();
                context.Close();
            }
            catch (Exception e) { throw e; }
        }

        public void DeleteAll()
        {
            try
            {
                SqlCommand cmd = new SqlCommand("TRUNCATE TABLE [dbo].[CUSTOMER];", this.context);
                context.Open();
                cmd.ExecuteNonQuery();
                context.Close();
            }
            catch (Exception e) { throw e; }
        }

        public bool Exist(Klant a, bool ignoreId = false)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM [dbo].[CUSTOMER] WHERE LOWER(NAME) = @Name AND LOWER(ADDRESS) = @Address OR CUSTOMER_ID = @Id", this.context);
                cmd.Parameters.AddWithValue("@Name", a.Naam.ToLower());
                cmd.Parameters.AddWithValue("@Address", a.Adres.ToLower());
                cmd.Parameters.AddWithValue("@Id", (!ignoreId) ? a.KlantId : -1);
                context.Open();
                int count = (int)cmd.ExecuteScalar();
                context.Close();
                return (count > 0);
            }
            catch (Exception e) { throw e; }
        }

        public List<Klant> GetAll()
        {
            try
            {
                context.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM [dbo].[CUSTOMER]", this.context);
                SqlDataAdapter reader = new SqlDataAdapter(cmd);
                DataTable table = new DataTable();
                reader.Fill(table);
                context.Close();
                if (table.Rows.Count > 0)
                    return table.AsEnumerable().Select(a => new Klant(a.Field<long>("CUSTOMER_ID"), a.Field<string>("NAME"), a.Field<string>("ADDRESS"))).ToList<Klant>();
            }
            catch (Exception e) { throw e; }
            return new List<Klant>();
        }

        public Klant GetByID(long id)
        {
            try
            {
                context.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM [dbo].[CUSTOMER] WHERE CUSTOMER_ID = @Id", this.context);
                cmd.Parameters.AddWithValue("@Id", id);
                SqlDataAdapter reader = new SqlDataAdapter(cmd);
                DataTable table = new DataTable();
                reader.Fill(table);
                context.Close();
                if (table.Rows.Count > 0)
                    return table.AsEnumerable().Select(a => new Klant(a.Field<long>("CUSTOMER_ID"), a.Field<string>("NAME"), a.Field<string>("ADDRESS"))).Single<Klant>();
            }
            catch (Exception e) { throw e; }
            return null;
        }

        public void Update(Klant a)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("UPDATE [dbo].[CUSTOMER] SET NAME = @Name, ADDRESS = @Address WHERE CUSTOMER_ID = @Id", this.context);
                cmd.Parameters.AddWithValue("@Name", a.Naam);
                cmd.Parameters.AddWithValue("@Address", a.Adres);
                cmd.Parameters.AddWithValue("@Id", a.KlantId);
                context.Open();
                cmd.ExecuteNonQuery();
                context.Close();
            }
            catch (Exception e) { throw e; }
        }
    }
}
