using BusinessLayer.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer.IRepository
{
    public interface ICustomerRepository
    {
        public Klant Add(Klant klant);
        public Klant GetByID(long id);
        public List<Klant> GetAll();
        public void Delete(long id);
        public void DeleteAll();
        public void Update(Klant klant);
        public bool Exist(Klant klant, bool ignoreId = false);
    }
}
