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
    public class DbBestellingManager : IManager<Bestelling>
    {
        #region Properties
        private EntityFrameworkRepository<Order> _repository = new EntityFrameworkRepository<Order>();
        private EntityFrameworkRepository<OrderProduct> _repositoryOrderProducts = new EntityFrameworkRepository<OrderProduct>();
        private EntityFrameworkRepository<Customer> _repositoryCustomer = new EntityFrameworkRepository<Customer>();
        private EntityFrameworkRepository<EntityFrameworkRepository.Models.Product> _repositoryProduct = new EntityFrameworkRepository<EntityFrameworkRepository.Models.Product>();
        /// <summary>
        /// An application of type Tuple (2 elementen als een eenheid): "alternatief" voor een aparte class CustomerKlant
        /// </summary>
        private Dictionary<long, (Order, Bestelling)> _mappedObjects = new Dictionary<long, (Order, Bestelling)>();
        private Dictionary<long, (OrderProduct, Model.Product)> _OrderProduct_mappedObjects = new Dictionary<long, (OrderProduct, Model.Product)>(); 
        private Dictionary<long, (Customer, Klant)> _Customer_Klant_mappedObjects = new Dictionary<long, (Customer, Klant)>();
        private Dictionary<long, (EntityFrameworkRepository.Models.Product, Model.Product)> _Product_mappedObjects = new Dictionary<long, (EntityFrameworkRepository.Models.Product, Model.Product)>(); // key: long Id
        #endregion

        #region Ctor
        #endregion

        #region Methods
        public IReadOnlyList<Bestelling> HaalOp()
        {
            var bestellingLijst = new List<Bestelling>();
            // An EF object is tracked ONCE!!
            /* _mappedObjects = _repository.List().ToDictionary(  c => c.Id,
                  c => (c, new Bestelling((int)c.Id, new Klant(c.Customer.Id, c.Customer.Name, c.Customer.Address), (DateTime)c.Time, c.OrderProducts.ToDictionary( o => new Model.Product(o.Product.Id, o.Product.Name, (double)o.Product.Price), o => o.Amount))));*/
            _mappedObjects = _repository.List().ToDictionary(
                    c => { return c.Id; }, 
                    c => { return (c,
                        new Bestelling((int)c.Id,
                            HaalKlantOp(c.CustomerId), 
                            (DateTime)c.Time,
                            HaalOrderProdctOpByOrderID(c.Id),
                            (c.Price is null? false : c.Paid == (int)c.Price),
                            c.Paid)
                        ); 
                    });
            //c.OrderProducts.ToDictionary(o => HaalProdctOp(o.ProductId), o => o.Amount)


            foreach (var dbItem in _mappedObjects.Values)//int bestellingId, Klant klant, DateTime tijdstip, Dictionary<Product, int> producten
            {
                bestellingLijst.Add(dbItem.Item2);
            }
            return bestellingLijst;
        }

        public IReadOnlyList<Bestelling> HaalOp(Func<Bestelling, bool> predicate)
        {
            var bestellingen = new List<Bestelling>();
            foreach (var item in _mappedObjects.Values)
            {
                bestellingen.Add(item.Item2);
            }
            var selection = bestellingen.Where<Bestelling>(predicate).ToList();
            return (IReadOnlyList<Bestelling>)selection;
        }

        public void VoegToe(Bestelling bestelling)
        {
            // We mogen geen Id opgeven want database kent deze toe:
            var order = new Order { CustomerId = bestelling.Klant.KlantId,
                                    Time = bestelling.Tijdstip,
                                    Paid = (bestelling.Betaald == true ?  (int)bestelling.PrijsBetaald : 0),
                                    Price = (decimal?)bestelling.Kostprijs()};

            bestelling.BestellingId = order.Id = _repository.Insert(order); // Customer wordt g-insert-eerd in de database; wordt direct weggeschreven wegens SaveChanges()
            order.OrderProducts = bestelling.GeefProducten().Select(p => VoegOrderProductToe(p.Key,p.Value,order.Id)).ToList();
            _mappedObjects[bestelling.BestellingId] = (order, bestelling);
        }
        public OrderProduct VoegOrderProductToe(Model.Product product,int amount, long orderID)
        {
            // We mogen geen Id opgeven want database kent deze toe:
            var orderProduct = new OrderProduct{ProductId = product.ProductId, Amount = amount , OrderId = orderID };

            orderProduct.Id = _repositoryOrderProducts.Insert(orderProduct);
            _OrderProduct_mappedObjects[orderProduct.Id] = (orderProduct, product);
            return orderProduct;
        }
        public void Verwijder(Bestelling bestelling)
        {
            _repository.Delete(_mappedObjects[bestelling.BestellingId].Item1);
            _mappedObjects.Remove(bestelling.BestellingId);
        }

        public Bestelling HaalOp(long bestellingid)
        {
            if (_mappedObjects.ContainsKey(bestellingid))
            {
                return _mappedObjects[bestellingid].Item2; 
            }
            var Order = _repository.GetById(bestellingid);
            _mappedObjects[bestellingid] = (Order,
                                            new Bestelling((int)Order.Id,
                                            HaalKlantOp(Order.CustomerId),
                                            (DateTime)Order.Time,
                                            Order.OrderProducts.ToDictionary(o => HaalProdctOp(o.ProductId), o => o.Amount),
                                            (Order.Paid == Order.Price),
                                            Order.Paid)
                                            );
            return _mappedObjects[bestellingid].Item2; 
        }
        public Klant HaalKlantOp(long klantId)
        {
            if (_Customer_Klant_mappedObjects.ContainsKey(klantId))
            {
                return _Customer_Klant_mappedObjects[klantId].Item2; 
            }
            var customer = _repositoryCustomer.GetById(klantId);
            _Customer_Klant_mappedObjects[klantId] = (customer, new Klant(customer.Id, customer.Name, customer.Address));
            return _Customer_Klant_mappedObjects[klantId].Item2; 
        }
        public Model.Product HaalProdctOp(long productId)
        {
            if (_Product_mappedObjects.ContainsKey(productId))
            {
                return _Product_mappedObjects[productId].Item2; 
            }
            var EFProduct = _repositoryProduct.GetById(productId);
            _Product_mappedObjects[productId] = (EFProduct, new Model.Product(EFProduct.Id, EFProduct.Name, (double)EFProduct.Price));
            return _Product_mappedObjects[productId].Item2; 
        }
        public Dictionary<Model.Product,int> HaalOrderProdctOpByOrderID(long orderID)
        {
            var ProductenLijst = new Dictionary<Model.Product, int>();
            var tempOrderProducts = _repositoryOrderProducts.List();
            foreach (var orderProduct in tempOrderProducts)
            {
                if(orderProduct.OrderId == orderID)
                    ProductenLijst[HaalProdctOp(orderProduct.ProductId)] = orderProduct.Amount;
            }
            return ProductenLijst; 

        }
    }
    #endregion
}
