using eTickets.Data;
using eTickets.Data.Services;
using eTickets.Data.Static;
using eTickets.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace eTickets.Controllers
{
    [Authorize(Roles = UserRoles.Admin)]
    public class CinemasController : Controller
    {
        AppDbContext db;
        IWebHostEnvironment _hostingEnvironment;

        public CinemasController(AppDbContext db, IWebHostEnvironment _hostingEnvironment)
        {
            this.db = db;
            this._hostingEnvironment = _hostingEnvironment; 
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            List<Cinema> cinemas = db.Cinemas.ToList();
            return View(cinemas);
        }


        //Get: Cinemas/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Cinema cinema, IFormFile Logo)
        {
            if (!ModelState.IsValid) return View(cinema);



            if (Logo != null && Logo.Length > 0)
            {
                // Save the uploaded file to the server
                string fileName = Path.GetFileName(Logo.FileName);
                string filePath = Path.Combine(_hostingEnvironment.WebRootPath, "images", fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    Logo.CopyTo(fileStream);
                }

                // Set the ProfilePictureURL property of the Actor object
                cinema.Logo = "/images/" + fileName;
            }



            db.Cinemas.Add(cinema);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        //Get: Cinemas/Details/1
        [AllowAnonymous]
        public IActionResult Details(int id)
        {
            Cinema cinemaDetails = db.Cinemas.Find(id);
            if (cinemaDetails == null) return View("NotFound");
            return View(cinemaDetails);
        }

        //Get: Cinemas/Edit/1
        public IActionResult Edit(int id)
        {
            Cinema cinemaDetails = db.Cinemas.Find(id);
            if (cinemaDetails == null) return View("NotFound");
            return View(cinemaDetails);
        }

        [HttpPost]
        public IActionResult Edit(int id, Cinema cinema, IFormFile Logo)
        {
            if (id != cinema.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(cinema);
            }

            string oldFilePath = null;
            if (!string.IsNullOrEmpty(cinema.Logo))
            {
                oldFilePath = Path.Combine(_hostingEnvironment.WebRootPath, "images", cinema.Logo.TrimStart('/'));
            }

            if (Logo != null && Logo.Length > 0)
            {
                // Save the uploaded file to the server
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(Logo.FileName);
                string filePath = Path.Combine(_hostingEnvironment.WebRootPath, "images", fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    Logo.CopyTo(fileStream);
                }

                // Delete the old profile picture file from the server
                if (!string.IsNullOrEmpty(oldFilePath) && System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }

                // Set the ProfilePictureURL property of the Actor object
                cinema.Logo = "/images/" + fileName;
            }


            db.Cinemas.Update(cinema);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //Get: Cinemas/Delete/1
        public IActionResult Delete(int id)
        {
            Cinema cinemaDetails = db.Cinemas.Find(id);
            if (cinemaDetails == null) return View("NotFound");
            return View(cinemaDetails);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirm(int id)
        {
            Cinema cinemaDetails = db.Cinemas.Find(id);
            if (cinemaDetails == null) return View("NotFound");

            db.Cinemas.Remove(cinemaDetails);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
