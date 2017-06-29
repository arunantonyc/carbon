using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using carbon.Areas.Interest.Models;
using carbon.Data;
using carbon.Logic;

namespace carbon.Areas.Interest.Controllers
{
    public class CompaniesController : Controller
    {
        private companyContext db = new companyContext();

        // GET: Interest/Companies
        public ActionResult Index()
        {
            return View(db.Get(null));
        }

        // GET: Interest/Companies/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var compStores = db.GetStores(id);            
            if (compStores == null)
            {
                return HttpNotFound();
            }
            return View(compStores);
        }

        // GET: Interest/Companies/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Interest/Companies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CompanyId,Name,Title,Status")] Company company)
        {
            if (ModelState.IsValid)
            {
                //db.Companies.Add(company);
                db.Add(company);
                return RedirectToAction("Index");
            }

            return View(company);
        }

        // GET: Interest/Companies/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var companies = db.Get(id);
            if (companies.Count < 1)
                return HttpNotFound();
            Company company = companies[0];
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }

        // POST: Interest/Companies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CompanyId,Name,Title,Status")] Company company)
        {
            if (ModelState.IsValid)
            {
                //db.Entry(company).State = EntityState.Modified;
                db.Edit(company);
                return RedirectToAction("Index");
            }
            return View(company);
        }

        // GET: Interest/Companies/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var companies = db.Get(id);
            if (companies.Count < 1)
                return HttpNotFound();
            Company company = companies[0];
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }

        // POST: Interest/Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            db.Delete(id);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }



        // GET: Interest/Companies/Store/Details/5
        public ActionResult StoreDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var stores = db.GetStore(id);
            if (stores == null)
            {
                return HttpNotFound();
            }
            return View(stores[0]);
        }

        // GET: Interest/Companies/Create
        public ActionResult StoreCreate(int? id)
        {
            if (id == null)
                return View();
            var companies = db.Get(id);
            if ((companies == null) && (companies.Count < 1))
            {
                return HttpNotFound();
            }
            DateTime dtNow = DateTime.Now;
            Store store = new Store()
            {
                CompanyId = (int)id,
                StoreName = companies[0].Name,
                OpenTime = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day, 10, 0, 0),
                CloseTime = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day, 21, 0, 0)
            };
            return View(store);
        }

        // POST: Interest/Companies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StoreCreate([Bind(Include = "CompanyId, StoreName, Landmark, Address, OpenTime, CloseTime, Phone, Location")] Store store)
        {
            if (ModelState.IsValid)
            {                
                // calculate grid info
                if (!string.IsNullOrEmpty(store.Location))
                {
                    Grid grd = new Grid();
                    grd.GetGrid(store.Location);
                    store.Latitude = grd.Location.Lat;
                    store.Longitude = grd.Location.Lng;
                    store.Grid = double.Parse(grd.Id);
                }
                
                // store in DB
                db.AddStore(store);
                return RedirectToAction("Details", new { id = store.CompanyId });
            }

            return View(store);
        }

        // GET: Interest/Companies/Edit/5
        public ActionResult StoreEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var stores = db.GetStore(id);
            if (stores.Count < 1)
                return HttpNotFound();
            Store store = stores[0];
            if (store == null)
            {
                return HttpNotFound();
            }
            return View(store);
        }

        // POST: Interest/Companies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StoreEdit([Bind(Include = "StoreId, CompanyId, StoreName, Landmark, Address, OpenTime, CloseTime, Phone, Location")] Store store)
        {
            if (ModelState.IsValid)
            {
                // calculate grid info
                if (!string.IsNullOrEmpty(store.Location))
                {
                    Grid grd = new Grid();
                    grd.GetGrid(store.Location);
                    store.Latitude = grd.Location.Lat;
                    store.Longitude = grd.Location.Lng;
                    store.Grid = double.Parse(grd.Id);
                }
                // store in DB
                db.EditStore(store);
                return RedirectToAction("Details", new { id = store.CompanyId });
            }
            return View(store);
        }

        // GET: Interest/Companies/StoreDelete/5
        public ActionResult StoreDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var stores = db.GetStore(id);
            if (stores.Count < 1)
                return HttpNotFound();
            Store store = stores[0];
            if (store == null)
            {
                return HttpNotFound();
            }
            return View(store);
        }

        // POST: Interest/Companies/Delete/5
        [HttpPost, ActionName("StoreDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult StoreDeleteConfirmed(int id)
        {
            db.DeleteStore(id);
            return RedirectToAction("Index");
        }
    }
}
