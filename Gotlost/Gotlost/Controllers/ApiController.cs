using Gotlost.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gotlost.Controllers
{
    [Route("api/{controller}")]
    [ApiController]
    public class ApiController : Controller
    {
        private readonly ApplicationDbContext db;
        public ApiController(ApplicationDbContext _db)
        {
            db = _db;
        }

        [HttpGet]

        public IActionResult GetAll()
        {
            var users = db.Users
                .Include(u => u.FoundItems)
                .Include(u => u.Claims)
                .ToList();

            return Ok(users);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int Id)
        {
            var user = db.Users
       .Include(u => u.FoundItems)
       .Include(u => u.Claims)
       .FirstOrDefault(u => u.UserId == Id);

            return user == null ? NotFound() : Ok(user);
        }

        [HttpPost]
        public IActionResult Post([FromBody] User u)
        {
            db.Users.Add(u);
            db.SaveChanges();
            return CreatedAtAction(nameof(Get), new { Id = u.UserId }, u);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] User update)
        {
            var u = db.Users.Find(id);
            if (u == null) return NotFound();

            u.FullName = update.FullName;
            u.Email = update.Email;
            u.PhoneNumber = update.PhoneNumber;
            u.ProfilePicturePath = update.ProfilePicturePath;
            u.Password = update.Password;

            db.SaveChanges();
            return Ok(u);


        }

        [HttpDelete("{id}")]

        public IActionResult Delete(int id)
        {
            var prod = db.Users.Find(id);
            if (prod == null) return NotFound();

            db.Users.Remove(prod);
            db.SaveChanges();
            return NoContent();
        }


    }
}
