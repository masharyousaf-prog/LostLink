using Gotlost.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Claim = Gotlost.Models.Entities.Claim;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Gotlost.Models;

namespace Gotlost.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext db;

        // Static list tracks sessions in RAM. 
        // Stopping the server clears this list, invalidating all old cookies.
        private static HashSet<string> ActiveServerSessions = new HashSet<string>();

        public LoginController(ApplicationDbContext db)
        {
            this.db = db;
        }

        // ==========================================
        // FIX: FORCE PAGE RELOAD ON LOGOUT
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                await HttpContext.SignOutAsync("MyCookieAuth");
                // CRITICAL FIX: Redirect to self to clear the "Navbar" state
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string Email, string Password)
        {
            var user = db.Users.FirstOrDefault(i => i.Email == Email && i.Password == Password);
            if (user == null)
            {
                ViewBag.ErrorMessage = "Invalid login attempt.";
                return View("Index");
            }

            var userClaims = db.Claims.Where(c => c.UserId == user.UserId).ToList();

            var claimList = new List<System.Security.Claims.Claim>
            {
                new System.Security.Claims.Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new System.Security.Claims.Claim(ClaimTypes.Email, user.Email),
                new System.Security.Claims.Claim(ClaimTypes.Name, user.FullName ?? user.Email),
                // Store Profile Picture so Navbar doesn't show broken icon
                new System.Security.Claims.Claim("ProfilePicture", user.ProfilePicturePath ?? "")
            };

            foreach (var claim in userClaims)
            {
                claimList.Add(new System.Security.Claims.Claim("ClaimStatus", claim.Status));
                claimList.Add(new System.Security.Claims.Claim("FoundItemId", claim.FoundItemId.ToString()));
            }

            // Generate Session Token
            var sessionToken = Guid.NewGuid().ToString();
            ActiveServerSessions.Add(sessionToken); // Add to Server RAM
            claimList.Add(new System.Security.Claims.Claim("SessionToken", sessionToken)); // Add to User Cookie

            var identity = new ClaimsIdentity(claimList, "MyCookieAuth");
            var principal = new ClaimsPrincipal(identity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = false,
                AllowRefresh = true
            };

            await HttpContext.SignInAsync("MyCookieAuth", principal, authProperties);

            return RedirectToAction("Profile");
        }

        [Authorize]
        public async Task<IActionResult> Profile()
        {
            // Security Check: If server restarted, session is invalid
            var tokenClaim = User.FindFirst("SessionToken");
            if (tokenClaim == null || !ActiveServerSessions.Contains(tokenClaim.Value))
            {
                await HttpContext.SignOutAsync("MyCookieAuth");
                return RedirectToAction("Index");
            }

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

            int userId = int.Parse(userIdStr);

            var user = await db.Users
                .Include(u => u.FoundItems)
                .Include(u => u.Claims).ThenInclude(c => c.FoundItem)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null) return NotFound();

            return View(user);
        }

        // ==========================================
        // OTHER ACTIONS (UNCHANGED)
        // ==========================================

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ReportFound(string ItemName, string Description, DateTime FoundDate, string Location, IFormFile ImageFile)
        {
            // Session Check
            var tokenClaim = User.FindFirst("SessionToken");
            if (tokenClaim == null || !ActiveServerSessions.Contains(tokenClaim.Value))
            {
                await HttpContext.SignOutAsync("MyCookieAuth");
                return RedirectToAction("Index");
            }

            string imagePath = null;
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                Directory.CreateDirectory(uploadsFolder);
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }
                imagePath = "/uploads/" + uniqueFileName;
            }

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();
            int userId = int.Parse(userIdStr);

            var foundItem = new FoundItem
            {
                ItemName = ItemName,
                Description = Description,
                FoundDate = FoundDate,
                Location = Location,
                ImagePath = imagePath,
                UserId = userId
            };

            db.FoundItems.Add(foundItem);
            await db.SaveChangesAsync();

            return RedirectToAction("Profile");
        }

        public IActionResult Found() { return View(); }

        public IActionResult Gallery()
        {
            var items = db.FoundItems.ToList();
            return View(items);
        }

        [HttpGet]
        public IActionResult Claim(int foundItemId)
        {
            var foundItem = db.FoundItems.FirstOrDefault(f => f.FoundItemId == foundItemId);
            if (foundItem == null) return NotFound();
            ViewBag.FoundItem = foundItem;
            return View(new AddClaim { FoundItemId = foundItemId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Claim(AddClaim claim)
        {
            if (!ModelState.IsValid)
            {
                var foundItem = db.FoundItems.FirstOrDefault(f => f.FoundItemId == claim.FoundItemId);
                ViewBag.FoundItem = foundItem;
                return View(claim);
            }
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var newClaim = new Claim
                {
                    FoundItemId = claim.FoundItemId,
                    Description = claim.Description,
                    ClaimDate = DateTime.Now,
                    Status = "Pending",
                    UserId = userId
                };
                db.Claims.Add(newClaim);
                db.SaveChanges();
                return RedirectToAction("Profile");
            }
            catch (Exception)
            {
                ViewData["Error"] = "Error submitting claim.";
                var foundItem = db.FoundItems.FirstOrDefault(f => f.FoundItemId == claim.FoundItemId);
                ViewBag.FoundItem = foundItem;
                return View(claim);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteClaim(int id)
        {
            var claim = await db.Claims.FindAsync(id);
            if (claim != null)
            {
                db.Claims.Remove(claim);
                await db.SaveChangesAsync();
            }
            TempData["Message"] = "Claim deleted successfully.";
            return RedirectToAction("Profile");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MyCookieAuth");
            return RedirectToAction("Index");
        }

        public IActionResult Lost() { return View(); }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ReportLost(string ItemName, string Description, DateTime LostDate, string Location, IFormFile ImageFile)
        {
            return RedirectToAction("Profile");
        }
    }
}