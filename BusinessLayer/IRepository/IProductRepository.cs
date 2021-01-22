using BusinessLayer.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer.IRepository
{
    public interface IProductRepository
    {
        public Product Add(Product a);
        public Product GetByID(int id);
        public List<Product> GetAll();
        public void Delete(int id);
        public void DeleteAll();
        public void Update(Product a);
        public bool Exist(Product a, bool ignoreId = false);
        public bool HasStrips(int id);
    }
}
