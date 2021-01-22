using BusinessLayer.IRepository;
using BusinessLayer.Model;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Text;
using System.Runtime.Serialization;
using System.Data;
using System.Linq;

namespace ADO.NET.Repos
{
    public class ProductRepository : IProductRepository
    {
        private SqlConnection context { get; set; }

        /// <summary> 
        /// Create Author Repository with database Connection 
        /// </summary>
        public ProductRepository(SqlConnection context)
        {
            this.context = context;
        }

        public Product Add(Product a)
        {
            int id = -1;
            String cmd = "INSERT INTO [dbo].[PRODUCT] (NAME,Price) VALUES (@NAME,@Price);SELECT CAST(scope_identity() AS int)";
            using (var insertCmd = new SqlCommand(cmd, this.context))
            {
                insertCmd.Parameters.AddWithValue("@NAME", a.Naam);
                insertCmd.Parameters.AddWithValue("@Price", a.Prijs);
                try
                {
                    context.Open();
                    id = (int)insertCmd.ExecuteScalar();
                    context.Close();
                }
                catch (Exception) { throw new InsertException(); }
            }
            if (id < 0) throw new AuthorAddException();
            return new Product(id, a.Naam, a.Prijs);
            throw new NotImplementedException();
        }

        public void Delete(long id)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("DELETE FROM [dbo].[PRODUCT] WHERE Id = @Id", this.context);
                cmd.Parameters.AddWithValue("@Id", id);
                context.Open();
                cmd.ExecuteNonQuery();
                context.Close();
            }
            catch (Exception) { throw new QueryException(); }
        }

        public void DeleteAll()
        {
            try
            {
                SqlCommand cmd = new SqlCommand("TRUNCATE TABLE [dbo].[PRODUCT]", this.context);
                context.Open();
                cmd.ExecuteNonQuery();
                context.Close();
            }
            catch (Exception) { throw new QueryException(); }
        }

        public bool Exist(Product a, bool ignoreId = false)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM [dbo].[PRODUCT] WHERE LOWER(NAME) = @NAME AND Price = @Price OR Id = @Id", this.context);
                cmd.Parameters.AddWithValue("@NAME", a.Naam.ToLower());
                cmd.Parameters.AddWithValue("@Price", a.Prijs);
                cmd.Parameters.AddWithValue("@Id", (!ignoreId) ? a.ProductId : -1);
                context.Open();
                int count = (int)cmd.ExecuteScalar();
                context.Close();
                return (count > 0);
            }
            catch (Exception) { throw new QueryException(); }
        }

        public List<Product> GetAll()
        {
            try
            {
                context.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM [dbo].[PRODUCT]", this.context);
                SqlDataAdapter reader = new SqlDataAdapter(cmd);
                DataTable table = new DataTable();
                reader.Fill(table);
                context.Close();
                if (table.Rows.Count > 0)
                    return table.AsEnumerable().Select(a => new Product(a.Field<long>("Id"), a.Field<string>("Name"), a.Field<double>("Prijs"))).ToList<Product>();
            }
            catch (Exception) { throw new QueryException(); }
            return new List<Product>();
        }

        public Product GetByID(long id)
        {
            try
            {
                context.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM [dbo].[PRODUCT] WHERE Id = @Id", this.context);
                cmd.Parameters.AddWithValue("@Id", id);
                SqlDataAdapter reader = new SqlDataAdapter(cmd);
                DataTable table = new DataTable();
                reader.Fill(table);
                context.Close();
                if (table.Rows.Count > 0)
                    return table.AsEnumerable().Select(a => new Product(a.Field<long>("Id"), a.Field<string>("Name"), a.Field<double>("Prijs"))).Single<Product>();
            }
            catch (Exception) { throw new QueryException(); }
            return null;
        }

        public void Update(Product a)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("UPDATE [dbo].[Authors] SET NAME = @NAME, PRIJS = @PRIJS WHERE Id = @Id", this.context);
                cmd.Parameters.AddWithValue("@NAME", a.Naam);
                cmd.Parameters.AddWithValue("@PRIJS", a.Prijs);
                cmd.Parameters.AddWithValue("@Id", a.ProductId);
                context.Open();
                cmd.ExecuteNonQuery();
                context.Close();
            }
            catch (Exception) { throw new QueryException(); }
        }
    }

    [Serializable]
    internal class QueryException : Exception
    {
        public QueryException()
        {
        }

        public QueryException(string message) : base(message)
        {
        }

        public QueryException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected QueryException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    [Serializable]
    internal class AuthorAddException : Exception
    {
        public AuthorAddException()
        {
        }

        public AuthorAddException(string message) : base(message)
        {
        }

        public AuthorAddException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AuthorAddException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    [Serializable]
    internal class InsertException : Exception
    {
        public InsertException()
        {
        }

        public InsertException(string message) : base(message)
        {
        }

        public InsertException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InsertException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
