using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.OData;

using ProductService.Models;
using System.Web.Http;
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Net;

namespace ProductService.Controllers
{
    public class ProductsController : ODataController
    {
        ProductsContext db = new ProductsContext();
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        [EnableQuery]
        public IQueryable<Product> Get()
        {
            return db.Products;
        }

        [EnableQuery]
        public SingleResult<Product> Get([FromODataUri] int key)
        {
            IQueryable<Product> result = db.Products.Where(p => p.Id == key);
            return SingleResult.Create(result);
        }

        public async Task<IHttpActionResult> Post(Product product)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            db.Products.Add(product);
            await db.SaveChangesAsync();
            return Created(product);
        }

        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<Product> product)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entity = await db.Products.FindAsync(key);
            if (entity == null) return NotFound();

            product.Patch(entity);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExist(key))
                    return NotFound();
                throw;
            }
            return Updated(entity);
        }

        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entity = await db.Products.FindAsync(key);
            if (entity == null) return NotFound();
            db.Products.Remove(entity);
            await db.SaveChangesAsync();

            return StatusCode(System.Net.HttpStatusCode.NoContent);
        }

        [EnableQuery]
        public SingleResult<Supplier> GetSupplier([FromODataUri] int key)
        {
            var supplier = db.Products.Where(p => p.Id == key).Select(p => p.Supplier);
            return SingleResult.Create(supplier);
        }

        [AcceptVerbs("POST", "PUT")]
        public async Task<IHttpActionResult> CreateRef([FromODataUri] int key, string navigationProperty, [FromBody] Uri link)
        {
            var product = await db.Products.SingleOrDefaultAsync(p => p.Id == key);
            if (product == null) return NotFound();

            switch (navigationProperty)
            {
                case "Supplier":
                    var relatedkey = Helpers.GetKeyFromUri<int>(Request, link);
                    var supplier = await db.Suppliers.SingleOrDefaultAsync(f => f.Id == relatedkey);
                    if (supplier == null)
                        return NotFound();

                    product.Supplier = supplier;
                    break;
                default:
                    return StatusCode(HttpStatusCode.NotImplemented);
            }

            await db.SaveChangesAsync();
            return StatusCode(HttpStatusCode.NoContent);
        }
        private bool ProductExist(int key)
        {
            return db.Products.Any(p => p.Id == key);
        }
    }
}