using BusinessLayer.Exceptions;
using BusinessLayer.Interfaces;
using BusinessLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLayer.Managers
{
    /*
    public class CustomerKlant
    {
        public Customer Item1 {get;set;}
        public Klant Item2 {get; set;}
    }
     */
    public class DbKlantManager: IManager<Klant>
    {
        #region Properties
        private readonly IUnitOfWork uow;
        #endregion

        #region Ctor
        public DbKlantManager(IUnitOfWork uow)
        {
            this.uow = uow;
        }
        #endregion

        #region Methods
        public IReadOnlyList<Klant> HaalOp()
        {
            return uow.Customers.GetAll();
        }

        public IReadOnlyList<Klant> HaalOp(Func<Klant, bool> predicate)
        {
            var klanten = new List<Klant>();
            foreach (var item in uow.Customers.GetAll())
            {
                klanten.Add(item);
            }
            var selection = klanten.Where<Klant>(predicate).ToList();
            return (IReadOnlyList<Klant>)selection;
        }

        public void VoegToe(Klant klant)
        {
            if (uow.Customers.Exist(klant)) throw new ProductException("Already exists");
            try
            {
                uow.Customers.Add(klant);
            }
            catch (Exception) { throw new ProductException("Errod during adding of product"); }
        }

        public void Verwijder(Klant klant)
        {
            uow.Customers.Delete(klant.KlantId);
        }

        public Klant HaalOp(long klantId)
        {
            return uow.Customers.GetByID(klantId);
        }
    }
    #endregion
}
