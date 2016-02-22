using ProductService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.OData;

namespace ProductService.Controllers
{
    public class SuppliersController : ODataController
    {
        ProductsContext db = new ProductsContext();

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        [EnableQuery]
        public IQueryable<Supplier> Get()
        {
            return db.Suppliers;
        }

        public IQueryable<Product> GetProducts([FromODataUri] int key)
        {
            return db.Suppliers.Where(p => p.Id == key).SelectMany(p => p.Products);
        }
    }
}