using BusinessLayer.Exceptions;
using BusinessLayer.Interfaces;
using BusinessLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLayer.Managers
{
    public class DbBestellingManager : IManager<Bestelling>
    {
        #region Properties
        private readonly IUnitOfWork uow;
        #endregion

        #region Ctor
        public DbBestellingManager(IUnitOfWork uow)
        {
            this.uow = uow;
        }
        #endregion

        #region Methods
        public IReadOnlyList<Bestelling> HaalOp()
        {
            return uow.Orders.GetAll();
        }

        public IReadOnlyList<Bestelling> HaalOp(Func<Bestelling, bool> predicate)
        {
            var producten = new List<Bestelling>();
            foreach (var item in uow.Orders.GetAll())
            {
                producten.Add(item);
            }
            var selection = producten.Where<Bestelling>(predicate).ToList();
            return (IReadOnlyList<Bestelling>)selection;
        }

        public void VoegToe(Bestelling bestelling)
        {
            if (uow.Orders.Exist(bestelling)) throw new ProductException("Already exists");
            try
            {
                uow.Orders.Add(bestelling);
            }
            catch (Exception) { throw new ProductException("Errod during adding of product"); }
        }

        public void Verwijder(Bestelling bestelling)
        {
            uow.Orders.Delete(bestelling.BestellingId);
        }

        public Bestelling HaalOp(long BestellingId)
        {
            return uow.Orders.GetByID(BestellingId);
        }
    }
    #endregion
}
