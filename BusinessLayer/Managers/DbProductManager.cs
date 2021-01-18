using BusinessLayer.Interfaces;
using BusinessLayer.Model;
using EntityFrameworkRepository;
using EntityFrameworkRepository.Models;
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
    public class DbProductManager : IManager<Model.Product>
    {
        #region Properties
        private EntityFrameworkRepository<EntityFrameworkRepository.Models.Product> _repository = new EntityFrameworkRepository<EntityFrameworkRepository.Models.Product>();
        /// <summary>
        /// An application of type Tuple (2 elementen als een eenheid): "alternatief" voor een aparte class CustomerKlant
        /// </summary>
        private Dictionary<long, (EntityFrameworkRepository.Models.Product, Model.Product)> _mappedObjects = new Dictionary<long, (EntityFrameworkRepository.Models.Product, Model.Product)>(); // key: long Id
        // private Dictionary<long, CustomerKlant> _mappedObjects;
        #endregion

        #region Ctor
        #endregion

        #region Methods
        public IReadOnlyList<Model.Product> HaalOp()
        {
            var ProductenLijst = new List<Model.Product>();
            // An EF object is tracked ONCE!!
            _mappedObjects = _repository.List().ToDictionary( /*key: Customer*/ c => c.Id,
                /*value: Tuple van Customer object en Klant object*/ c => (c, new Model.Product(c.Id, c.Name, (double)c.Price)));
            foreach (var dbItem in _mappedObjects.Values)
            {
                ProductenLijst.Add(dbItem.Item2);
            }
            return ProductenLijst;
        }

        public IReadOnlyList<Model.Product> HaalOp(Func<Model.Product, bool> predicate)
        {
            var producten = new List<Model.Product>();
            foreach (var item in _mappedObjects.Values)
            {
                producten.Add(item.Item2);
            }
            var selection = producten.Where<Model.Product>(predicate).ToList();
            return (IReadOnlyList<Model.Product>)selection;
        }

        public void VoegToe(Model.Product product)
        {
            // We mogen geen Id opgeven want database kent deze toe:
            var EFProduct = new EntityFrameworkRepository.Models.Product { /*Id = klant.KlantId,*/ Name = product.Naam, Price = (decimal)product.Prijs };
            product.ProductId = EFProduct.Id = _repository.Insert(EFProduct); // Customer wordt g-insert-eerd in de database; wordt direct weggeschreven wegens SaveChanges()
            _mappedObjects[product.ProductId] = (EFProduct, product);
        }

        public void Verwijder(Model.Product product)
        {
            _repository.Delete(_mappedObjects[product.ProductId].Item1);
            _mappedObjects.Remove(product.ProductId);
        }

        public Model.Product HaalOp(long productId)
        {
            if (_mappedObjects.ContainsKey(productId))
            {
                return _mappedObjects[productId].Item2; // we geven Klant object terug dat al klaarstond, via Item2 van Tuple
            }
            var EFProduct = _repository.GetById(productId);
            _mappedObjects[productId] = (EFProduct, new Model.Product(EFProduct.Id, EFProduct.Name, (double)EFProduct.Price));
            return _mappedObjects[productId].Item2; // we geven Klant object terug via Item2 van Tuple
        }
    }
    #endregion
}
