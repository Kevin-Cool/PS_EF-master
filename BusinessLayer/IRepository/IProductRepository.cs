using BusinessLayer.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer.IRepository
{
    public interface IProductRepository
    {
        public Product Add(Product a);
        public Product GetByID(long id);
        public List<Product> GetAll();
        public void Delete(long id);
        public void DeleteAll();
        public void Update(Product a);
        public bool Exist(Product a, bool ignoreId = false);
    }
}
