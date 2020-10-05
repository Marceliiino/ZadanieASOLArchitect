using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZadanieWebApplication.Models;

namespace ZadanieWebApplication.Controllers
{
    public class DefaultController : Controller
    {
        // GET: Default
        public ActionResult Index()
        {
            var entities = new InvoiceDbEntities();
            var aa = entities.products;
            return View(aa);
        }

        public ActionResult CreateproductView()
        {
            var aa = View();
            return aa;
        }

        public ActionResult Edit(int id)
        {
            throw new NotImplementedException();
        }

        public ActionResult Details(int id)
        {
            throw new NotImplementedException();
        }

        public ActionResult Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}