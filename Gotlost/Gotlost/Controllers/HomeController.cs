using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Gotlost.Models;
using Gotlost.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Gotlost.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;
    public HomeController(ApplicationDbContext _db)
    {
        _context = _db;
    }
    public IActionResult Index()
    {
        var recentFoundItems = _context.FoundItems
        .Include(f => f.User) // Ensures User is loaded
        .OrderByDescending(f => f.FoundDate)
        .Take(6)
        .ToList();

        return View(recentFoundItems);
    }

    public IActionResult Aboutus()
    {
        return View();
    }
    
  
    public IActionResult Contactus()
    {
        return View();
    }
    [HttpPost] 
    public IActionResult msg(string Name, string Email, string Message)
    {
        if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Message))
        {
           
            return View("Contactus");
        }

        var c = new ContactUs()
        {
            Name = Name,
            Email = Email,
            Message = Message,
            SentAt = DateTime.Now 
        };

        _context.ContactUsMessages.Add(c);
        _context.SaveChanges();

        
        return RedirectToAction("Contactus", new { success = true });
    }
   

}
