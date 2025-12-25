using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gotlost.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext db;
        public AdminController(ApplicationDbContext db)
        {
            this.db = db;
        }

        public IActionResult Index()
        {
            return View();
        }


        public IActionResult AdminProfile()
        {
            return View();
        }

        public IActionResult Login(string Password, string Email)
        {
            string adminpassword = "admin@123";
            string adminemail = "admin@gmail.com";
            if (Email == adminemail && Password == adminpassword)
            {
                return View("AdminProfile");
            }
            else
            {
                return View("Index");
            }
        }
        public IActionResult Logout()
        {
            return View("Index");
        }
        public async Task<IActionResult> AllFoundItems()
        {
            var items = await db.FoundItems.Include(f => f.User).ToListAsync();
            return View(items);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var item = await db.FoundItems
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.FoundItemId == id);

            if (item == null) return NotFound();

            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await db.FoundItems
                              .Include(f => f.Claims)
                              .FirstOrDefaultAsync(f => f.FoundItemId == id);

            if (item == null)
                return NotFound();

            if (item.Claims.Any())
            {
                TempData["Error"] = "Cannot delete. There are claims associated with this item.";
                return RedirectToAction("AllFoundItems");
            }

            db.FoundItems.Remove(item);
            await db.SaveChangesAsync();
            return RedirectToAction("AllFoundItems");
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var item = await db.FoundItems
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.FoundItemId == id);

            if (item == null) return NotFound();

            return View(item);
        }

        public async Task<IActionResult> AllClaims()
        {
            var claims = await db.Claims
                .Include(c => c.User)
                .Include(c => c.FoundItem)
                .ToListAsync();

            return View(claims);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var claim = await db.Claims.FindAsync(id);
            if (claim == null) return NotFound();

            claim.Status = status;
            await db.SaveChangesAsync();

            return RedirectToAction("AllClaims");
        }
        public async Task<IActionResult> AllUsers()
        {
            var users = await db.Users.ToListAsync();
            return View(users);
        }


        // GET: /Users/DeleteUser/5
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await db.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            return View(user);
        }

        // POST: /Users/DeleteUser/5
        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUserConfirmed(int id)
        {
            var user = await db.Users
                                .Include(u => u.Claims)  // Include related claims if necessary
                                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null)
                return NotFound();

            // Remove associated claims (if any) before deleting the user
            if (user.Claims != null && user.Claims.Any())
            {
                db.Claims.RemoveRange(user.Claims);  // Remove claims related to the user
            }

            db.Users.Remove(user);  // Now remove the user

            await db.SaveChangesAsync();

            TempData["Success"] = "User and associated claims deleted successfully.";
            return RedirectToAction("AllUsers");  // Redirect to a page showing all users
        }
        public async Task<IActionResult> Messages()
        {
            var messages = await db.ContactUsMessages.ToListAsync();
            return View(messages);
        }
        public IActionResult Back()
        {
            return View("AdminProfile");
        }

    }
}
