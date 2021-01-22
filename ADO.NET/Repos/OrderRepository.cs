using BusinessLayer.IRepository;
using BusinessLayer.Model;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ADO.NET.Repos
{
    public class OrderRepository : IOrderRepository
    {
        private SqlConnection context { get; set; }

        public OrderRepository(SqlConnection context)
        {
            this.context = context;
        }

        public Bestelling Add(Bestelling a)
        {
            int id = -1;
            String cmd = "INSERT INTO [dbo].[ORDER] (PAID, PRICE, CUSTOMER_ID, TIME) VALUES (@Paid, @Price, @CustomerId, @Time); SELECT CAST(scope_identity() AS int)";
            using (var insertCmd = new SqlCommand(cmd, this.context))
            {
                insertCmd.Parameters.AddWithValue("@Paid", a.Betaald);
                insertCmd.Parameters.AddWithValue("@Price", a.Kostprijs());
                insertCmd.Parameters.AddWithValue("@CustomerId", a.Klant.KlantId);
                insertCmd.Parameters.AddWithValue("@Time", a.Tijdstip);
                try
                {
                    context.Open();
                    id = (int)insertCmd.ExecuteScalar();
                    context.Close();
                }
                catch (Exception e) { throw e; }
            }
            if (id < 0) throw new NullReferenceException();
            return new Bestelling(id, a.Klant, a.Tijdstip);
        }

        public void Delete(long id)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("DELETE FROM [dbo].[ORDER] WHERE ORDER_ID = @Id;", this.context);
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
                SqlCommand cmd = new SqlCommand("TRUNCATE TABLE [dbo].[ORDER];", this.context);
                context.Open();
                cmd.ExecuteNonQuery();
                context.Close();
            }
            catch (Exception e) { throw e; }
        }

        public bool Exist(Bestelling a)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM [dbo].[ORDER] WHERE ID = @Id", this.context);
                cmd.Parameters.AddWithValue("@Id", a.BestellingId);
                context.Open();
                int count = (int)cmd.ExecuteScalar();
                context.Close();
                return (count > 0);
            }
            catch (Exception e) { throw e; }
        }

        public List<Bestelling> GetAll()
        {
            try
            {
                context.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM [dbo].[ORDER]", this.context);
                SqlDataAdapter reader = new SqlDataAdapter(cmd);
                DataTable table = new DataTable();
                reader.Fill(table);
                context.Close();
                if (table.Rows.Count > 0)
                {
                    List<Klant> klanten = new CustomerRepository(this.context).GetAll();
                    List<Product> product = new ProductRepository(this.context).GetAll();

                    List<Bestelling> bestellingen = table.AsEnumerable().Select(a => new Bestelling(a.Field<int>("ID"), klanten.FirstOrDefault(k => k.KlantId.Equals(a.Field<int>("CUSTOMER_ID"))), a.Field<DateTime>("TIME"))).ToList();

                    foreach (Bestelling b in bestellingen)
                    {
                        context.Open();
                        SqlCommand cmd2 = new SqlCommand("SELECT * FROM [dbo].[ORDER_PRODUCT] WHERE ORDER_ID = @OrderId", this.context);
                        cmd2.Parameters.AddWithValue("@OrderId", b.BestellingId);
                        SqlDataAdapter reader2 = new SqlDataAdapter(cmd);
                        DataTable table2 = new DataTable();
                        reader2.Fill(table2);
                        context.Close();

                        if (table2.Rows.Count > 0)
                        {
                            foreach (DataRow dr in table2.AsEnumerable())
                            {
                                b.VoegProductToe(product.First(p => p.ProductId.Equals(dr.Field<long>("PRODUCT_ID"))), dr.Field<int>("AMOUNT"));
                            }
                        }
                    }
                    return bestellingen;
                }
            }
            catch (Exception e) { throw e; }
            return new List<Bestelling>();
        }

        public Bestelling GetByID(long id)
        {
            try
            {
                context.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM [dbo].[ORDER] WHERE ORDER_ID = @OrderId", this.context);
                cmd.Parameters.AddWithValue("@OrderId", id);
                SqlDataAdapter reader = new SqlDataAdapter(cmd);
                DataTable table = new DataTable();
                reader.Fill(table);
                context.Close();
                if (table.Rows.Count > 0)
                {
                    List<Product> product = new ProductRepository(this.context).GetAll();
                    Bestelling bestelling = table.AsEnumerable().Select(a => new Bestelling(a.Field<int>("ID"), new CustomerRepository(this.context).GetByID(a.Field<int>("CUSTOMER_ID")), a.Field<DateTime>("TIME"))).Single();

                    context.Open();
                    SqlCommand cmd2 = new SqlCommand("SELECT * FROM [dbo].[ORDER_PRODUCT] WHERE ORDER_ID = @OrderId", this.context);
                    cmd2.Parameters.AddWithValue("@OrderId", id);
                    SqlDataAdapter reader2 = new SqlDataAdapter(cmd2);
                    DataTable table2 = new DataTable();
                    reader2.Fill(table);
                    context.Close();

                    if (table2.Rows.Count > 0)
                    {
                        foreach (DataRow dr in table.AsEnumerable())
                        {
                            bestelling.VoegProductToe(product.First(p => p.ProductId.Equals(dr.Field<int>("PRODUCT_ID"))), dr.Field<int>("AMOUNT"));
                        }
                    }
                    return bestelling;
                }
            }
            catch (Exception e) { throw e; }
            return null;
        }

        public void Update(Bestelling a)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("UPDATE [dbo].[ORDER] SET PAID = @Paid WHERE ORDER_ID = @OrderId; TRUNCATE TABLE [dbo].[ORDER_PRODUCT];", this.context);
                cmd.Parameters.AddWithValue("@OrderId", a.BestellingId);
                cmd.Parameters.AddWithValue("@Paid", a.Betaald);
                context.Open();
                cmd.ExecuteNonQuery();
                context.Close();

                Dictionary<Product, int> dic = (Dictionary<Product, int>)a.GeefProducten();

                foreach (Product p in dic.Keys)
                {
                    int id = -1;
                    String cmd2 = "INSERT INTO [dbo].[ORDER_PRODUCT] (ORDER_ID, PRODUCT_ID, AMOUNT) VALUES (@Paid, @Price, @CustomerId); SELECT CAST(scope_identity() AS int)";
                    using (var insertCmd = new SqlCommand(cmd2, this.context))
                    {
                        insertCmd.Parameters.AddWithValue("@OrderId", a.BestellingId);
                        insertCmd.Parameters.AddWithValue("@ProductId", p.ProductId);
                        insertCmd.Parameters.AddWithValue("@Amount", dic.Keys.First(q => q.ProductId.Equals(p.ProductId)));
                        try
                        {
                            context.Open();
                            id = (int)insertCmd.ExecuteScalar();
                            context.Close();
                        }
                        catch (Exception e) { throw e; }
                    }
                }
            }
            catch (Exception e) { throw e; }
        }
    }
}
