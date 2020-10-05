using System;
using System.Linq;
using Invoice.ApiCalls;
using Invoice.DB;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InvoiceTests
{
    [TestClass]
    public class UnitTest1
    {
        invoice_dbEntities invoiceDbEntities = new invoice_dbEntities();
  
        [TestMethod]
        public void CRUDProductTest()
        {
            int pid = 9876;
            var product = new product()
            {
                product_id = pid,
                product1 = "TestProduct",
                price = 123,
                quantity = 321,
                catagory = "Test Catagory",
                subcatagory = "Test SubCatagory",
                note = "Note"
            };

            var addedProduct = this.invoiceDbEntities.products.Add(product);
            pid = addedProduct.product_id;
            this.invoiceDbEntities.SaveChanges();
            
            var products = ProductApi.GetProductList(null);
            var testProduct = products.FirstOrDefault(x => x.product_id == addedProduct.product_id);
            Assert.IsNotNull(testProduct,"Test product not found.");
            Assert.AreEqual(testProduct.price, product.price, "Products are different.");

            testProduct = this.invoiceDbEntities.products.Remove(product);
            this.invoiceDbEntities.SaveChanges();

            products = ProductApi.GetProductList(null);
            Assert.IsNull(products.FirstOrDefault(x => x.product_id == addedProduct.product_id), "Test product is not removed.");

        }
    }
}
