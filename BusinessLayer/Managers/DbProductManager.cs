using BusinessLayer.Exceptions;
using BusinessLayer.Interfaces;
using BusinessLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLayer.Managers
{
    public class DbProductManager : IManager<Product>
    {
        #region Properties
        private readonly IUnitOfWork uow;
        #endregion

        #region Ctor
        public DbProductManager(IUnitOfWork uow)
        {
            this.uow = uow;
        }
        #endregion

        #region Methods
        public IReadOnlyList<Product> HaalOp()
        {
            return uow.Products.GetAll();
        }

        public IReadOnlyList<Product> HaalOp(Func<Product, bool> predicate)
        {
            var producten = new List<Product>();
            foreach (var item in uow.Products.GetAll())
            {
                producten.Add(item);
            }
            var selection = producten.Where<Product>(predicate).ToList();
            return (IReadOnlyList<Product>)selection;
        }

        public void VoegToe(Product product)
        {
            if (uow.Products.Exist(product)) throw new ProductException("Already exists");
            try
            {
                uow.Products.Add(product);
            }
            catch (Exception) { throw new ProductException("Errod during adding of product"); }
        }

        public void Verwijder(Product product)
        {
            uow.Products.Delete(product.ProductId);
        }

        public Product HaalOp(long productId)
        {
            return uow.Products.GetByID(productId);
        }
    }
    #endregion
}
