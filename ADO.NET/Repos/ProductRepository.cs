using BusinessLayer.IRepository;
using BusinessLayer.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADO.NET.Repos
{
    public class ProductRepository : IProductRepository
    {
        public Product Add(Product a)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public void DeleteAll()
        {
            throw new NotImplementedException();
        }

        public bool Exist(Product a, bool ignoreId = false)
        {
            throw new NotImplementedException();
        }

        public List<Product> GetAll()
        {
            throw new NotImplementedException();
        }

        public Product GetByID(int id)
        {
            throw new NotImplementedException();
        }

        public bool HasStrips(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(Product a)
        {
            throw new NotImplementedException();
        }
    }
}
