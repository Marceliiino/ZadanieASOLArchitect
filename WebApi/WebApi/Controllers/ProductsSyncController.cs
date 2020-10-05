using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Invoice.DB;

namespace WebApi.Controllers
{
    public class ProductsSyncController : ApiController
    {
        private invoice_dbEntities invoiceDbEntities = new invoice_dbEntities();

        // GET: api/ProductsSync
        public IQueryable<product> Getproducts(string category = null)
        {
            var products = String.IsNullOrWhiteSpace(category)
           ? invoiceDbEntities.products.AsQueryable()
           : invoiceDbEntities.products.Where(x => x.catagory == category);
            return products;
        }

        // GET: api/ProductsSync/5
        [ResponseType(typeof(product))]
        public IHttpActionResult Getproduct(int id)
        {
            product product = invoiceDbEntities.products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        // PUT: api/ProductsSync/5
        [ResponseType(typeof(void))]
        public IHttpActionResult Putproduct(int id, product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != product.product_id)
            {
                return BadRequest();
            }

            invoiceDbEntities.Entry(product).State = EntityState.Modified;

            try
            {
                invoiceDbEntities.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!productExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/ProductsSync
        [ResponseType(typeof(product))]
        public IHttpActionResult Postproduct(product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            invoiceDbEntities.products.Add(product);
            invoiceDbEntities.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = product.product_id }, product);
        }

        // DELETE: api/ProductsSync/5
        [ResponseType(typeof(product))]
        public IHttpActionResult Deleteproduct(int id)
        {
            product product = invoiceDbEntities.products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            invoiceDbEntities.products.Remove(product);
            invoiceDbEntities.SaveChanges();

            return Ok(product);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                invoiceDbEntities.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool productExists(int id)
        {
            return invoiceDbEntities.products.Count(e => e.product_id == id) > 0;
        }
    }
}