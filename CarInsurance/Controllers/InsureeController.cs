using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CarInsurance.Models;

namespace CarInsurance.Controllers
{
    public class InsureeController : Controller
    {
        private InsuranceEntities db = new InsuranceEntities();

        // GET: Insuree
        public ActionResult Index()
        {
            return View(db.Insurees.ToList());
        }

        // GET: Insuree/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insuree insuree = db.Insurees.Find(id);
            if (insuree == null)
            {
                return HttpNotFound();
            }
            return View(insuree);
        }

        // GET: Insuree/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Insuree/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,EmailAddress,DateOfBirth,CarYear,CarMake,CarModel,DUI,SpeedingTickets,CoverageType,Quote")] Insuree insuree)
        {
            if (ModelState.IsValid)
            {
                CalculateQuote(insuree);
                db.Insurees.Add(insuree);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(insuree);
        }

        // GET: Insuree/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insuree insuree = db.Insurees.Find(id);
            if (insuree == null)
            {
                return HttpNotFound();
            }
            return View(insuree);
        }

        // POST: Insuree/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,EmailAddress,DateOfBirth,CarYear,CarMake,CarModel,DUI,SpeedingTickets,CoverageType,Quote")] Insuree insuree)
        {
            if (ModelState.IsValid)
            {
                CalculateQuote(insuree);
                db.Entry(insuree).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(insuree);
        }

        // GET: Insuree/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insuree insuree = db.Insurees.Find(id);
            if (insuree == null)
            {
                return HttpNotFound();
            }
            return View(insuree);
        }

        // POST: Insuree/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Insuree insuree = db.Insurees.Find(id);
            db.Insurees.Remove(insuree);
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


       // ASSIGNMENT //


        private void CalculateQuote(Insuree insuree)
        {
            int baseRate = 50;

            // Calculating age
            var age = DateTime.Today.Year - insuree.DateOfBirth.Year;
            if (insuree.DateOfBirth > DateTime.Today.AddYears(-age)) age--;
            if (age <= 18) baseRate += 100;
            else if (age > 18 && age <= 25) baseRate += 50;
            else baseRate += 25;


            //Adjusting the car year
            if (insuree.CarYear < 2000 || insuree.CarYear > 2015) baseRate += 25;

            // Adjusting the car make and model
            if (insuree.CarMake.ToLower() == "porsche")
            {
                baseRate += 25;
                if (insuree.CarModel.ToLower() == "911 carrera") baseRate += 25; // Porsche AND 911 Carrera
            }


            //Adjusting the speeding ticket
            baseRate += insuree.SpeedingTickets *10; // Extra $10 for every speeding ticket theuser has


            // Adjusting the DUI
            if (insuree.DUI) baseRate = (int)(baseRate * 1.25); // If the user has ever had a DUI

            //Adjusting coverage tpe
            if (insuree.CoverageType) baseRate = (int)(baseRate * 1.5); // Assuming CoverageType is a boolean where true indicates full coverage

            insuree.Quote = baseRate;
        }


        public ActionResult Admin()
        {
            var quotes = db.Insurees.Select(insuree => new
            {
                insuree.FirstName,
                insuree.LastName,
                insuree.EmailAddress,
                insuree.Quote
            }).ToList();

            return View(quotes);
        }




    }
}
