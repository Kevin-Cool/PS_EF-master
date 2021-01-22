using BusinessLayer.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer.IRepository
{
    public interface IOrderRepository
    {
        public Bestelling Add(Bestelling a);
        public Bestelling GetByID(long id);
        public List<Bestelling> GetAll();
        public void Delete(long id);
        public void DeleteAll();
        public void Update(Bestelling a);
        public bool Exist(Bestelling a);
    }
}
