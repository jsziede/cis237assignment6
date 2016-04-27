using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using cis237Assignment6.Models;

namespace cis237Assignment6.Controllers
{
    [Authorize]
    public class BeveragesController : Controller
    {
        private BeverageJSziedeEntities1 db = new BeverageJSziedeEntities1();

        // GET: Beverages
        public ActionResult Index()
        {
            //Setup a variable to hold the beverages data
            DbSet<Beverage> BeveragesToFilter = db.Beverages;

            //Setup some strings to hold the data that might be in the session.
            //If there is nothing in the session we can still use these variables
            //as a default value.
            string filterBrand= "";
            string filterMin = "";
            string filterMax = "";
            string filterPack = "";

            //Define a min and a max for the cylinders
            decimal min = 0;
            decimal max = 100;

            //Check to see if there is a value in the session, and if there is, assign it
            //to the variable that we setup to hold the value.
            if (Session["brand"] != null && !String.IsNullOrWhiteSpace((string)Session["brand"]))
            {
                filterBrand = (string)Session["brand"];
            }

            if (Session["min"] != null && !String.IsNullOrWhiteSpace((string)Session["min"]))
            {
                filterMin = (string)Session["min"];
                min = Decimal.Parse(filterMin);
            }

            if (Session["max"] != null && !String.IsNullOrWhiteSpace((string)Session["max"]))
            {
                filterMax = (string)Session["max"];
                max = Decimal.Parse(filterMax);
            }

            if (Session["pack"] != null && !String.IsNullOrWhiteSpace((string)Session["pack"]))
            {
                filterPack = (string)Session["pack"];
            }

            //Do the filter on the BeveragesToFilter Dataset. Use the where that we used before
            //when doing EF work, only this time send in more lambda expressions to narrow it
            //down further. Since we setup default values for each of the filter parameters,
            //min, max, and filterMake, we can count on this always running with no errors.
            IEnumerable<Beverage> filtered = BeveragesToFilter.Where(beverage => beverage.price >= min &&
                                                                  beverage.price <= max &&
                                                                  beverage.name.Contains(filterBrand) &&
                                                                  beverage.pack.Contains(filterPack));

            //Convert the database set to a list now that the query work is done on it.
            //The view is expecting a List, so we convert the database set to a list.
            IEnumerable<Beverage> finalFiltered = filtered.ToList();

            //Place the string representation of the values that are in the session into
            //the viewbag so that they can be retrived and displayed on the view.
            ViewBag.filterMake = filterBrand;
            ViewBag.filterMin = filterMin;
            ViewBag.filterMax = filterMax;

            //Return the view with a filtered selection of the cars.
            return View(finalFiltered);

            //This is what used to be returned before a filter was setup.
            //return View(db.Cars.ToList());
        }

        // GET: Beverages/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Beverage beverage = db.Beverages.Find(id);
            if (beverage == null)
            {
                return HttpNotFound();
            }
            return View(beverage);
        }

        // GET: Beverages/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Beverages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name,pack,price,active")] Beverage beverage)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Beverages.Add(beverage);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch
                {
                    ViewBag.error = "Error: id already in use";
                    return RedirectToAction("Create");
                }
            }

            return View(beverage);
        }

        // GET: Beverages/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Beverage beverage = db.Beverages.Find(id);
            if (beverage == null)
            {
                return HttpNotFound();
            }
            return View(beverage);
        }

        // POST: Beverages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,pack,price,active")] Beverage beverage)
        {
            if (ModelState.IsValid)
            {
                db.Entry(beverage).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(beverage);
        }

        // GET: Beverages/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Beverage beverage = db.Beverages.Find(id);
            if (beverage == null)
            {
                return HttpNotFound();
            }
            return View(beverage);
        }

        // POST: Beverages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Beverage beverage = db.Beverages.Find(id);
            db.Beverages.Remove(beverage);
            db.SaveChanges();
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

        //We need to add the HttpPost so it limits the type of
        //requests it will handle to only POST. We can also specify
        //and Action Name if we don't want it to use the Method name
        //as the action name.
        [HttpPost, ActionName("Filter")]
        public ActionResult Filter()
        {
            //Get the form data that we sent out of the Request object.
            //The string that is used as a key to get the data matches the
            //name property of the form control
            string brand = Request.Form.Get("brand");
            string min = Request.Form.Get("min");
            string max = Request.Form.Get("max");
            string pack = Request.Form.Get("pack");

            //Store the form data into the session so that it can be retrived later
            //on to filter the data.
            Session["brand"] = brand;
            Session["min"] = min;
            Session["max"] = max;
            Session["pack"] = pack;

            //Redirect the user to the index page. We will do the work of actually
            //filtering the list in the index method.
            return RedirectToAction("Index");
        }
    }
}
