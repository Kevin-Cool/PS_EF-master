using BusinessLayer.IRepository;
using BusinessLayer.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ADO.NET.Repos
{
    public class OrderRepository : IOrderRepository
    {
        public Bestelling Add(Bestelling a)
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

        public bool Exist(Bestelling a, bool ignoreId = false)
        {
            throw new NotImplementedException();
        }

        public List<Bestelling> GetAll()
        {
            throw new NotImplementedException();
        }

        public Bestelling GetByID(int id)
        {
            throw new NotImplementedException();
        }

        public bool HasStrips(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(Bestelling a)
        {
            throw new NotImplementedException();
        }
    }
}
