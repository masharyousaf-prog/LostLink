using Gotlost.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Gotlost.Controllers
{
    public class SignupController : Controller
    {
        private readonly ApplicationDbContext db;

        public SignupController(ApplicationDbContext _db)
        {
            db = _db;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Signup(
            string FullName,
            string Email,
            string PhoneNumber,
            string Password,
            IFormFile ProfilePicture)
        {
            string profilePicturePath = null;

            // Handle image upload
            if (ProfilePicture != null && ProfilePicture.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                Directory.CreateDirectory(uploadsFolder); // Ensure the folder exists

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(ProfilePicture.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ProfilePicture.CopyToAsync(stream);
                }

                profilePicturePath = "/uploads/" + uniqueFileName; // Store relative path in DB
            }

            // Create user
            var sign = new User()
            {
                FullName = FullName,
                Email = Email,
                PhoneNumber = PhoneNumber,
                Password = Password,
                ProfilePicturePath = profilePicturePath
            };

            db.Users.Add(sign);
            await db.SaveChangesAsync();

            return RedirectToAction("Index", "Home"); // Or any other page after successful signup
        }
    }
}
