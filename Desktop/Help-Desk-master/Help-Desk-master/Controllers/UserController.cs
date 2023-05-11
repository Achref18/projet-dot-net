using HelpDesk.Data;
using HelpDesk.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Controllers;

[Authorize("RequireUser")]
public class UserController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly ApplicationDbContext _dbContext;

    public UserController(
        UserManager<User> userManager,
        ApplicationDbContext dbContext
    )
    {
        _userManager = userManager;
        _dbContext = dbContext;
    }



    public async Task<IActionResult> Index(string? title)
    {
            var mee = _dbContext.Complaints.Where(q => q.User.Id == "21e08c34-fb7c-46ba-b422-9be65bd8fa36");
        if (title == null )
        {
            return _dbContext.Complaints != null ?
                          View(await mee.ToListAsync()) :
                          Problem("Entity set '_dbContext.Complaints'  is null.");

        }

        var complaints = mee;

        if (!string.IsNullOrEmpty(title))
        {
            complaints = complaints.Where(d => d.Title.Contains(title));
        }


        var results = await complaints.ToListAsync();

        return View(results);
    }

    public async Task<IActionResult> Details(string id)
    {
        try
        {
            var complaint = await _dbContext.Complaints
                .Include(c => c.User)
                .Where(c => User.Identity != null && c.User != null && c.Id.ToString() == id &&
                            c.User.Email == User.Identity.Name)
                .SingleAsync();
            return View(complaint);
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
      
    }


    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Complaint complaint)
    {
        if (complaint.Title.Trim() == complaint.Description.Trim())
        {
            ModelState.AddModelError("name", "Name and Description must be different.");
        }

        if (!ModelState.IsValid)
        {
            return View(complaint);
        }

        var user = await _userManager.GetUserAsync(User);
        complaint.User = user;
        complaint.Action = null;
        await _dbContext.Complaints.AddAsync(complaint);
        await _dbContext.SaveChangesAsync();

        return RedirectToAction("Index");
    }
    
    public async Task<IActionResult> Update(string id)
    {
        
        try
        {
            var complaint = await _dbContext.Complaints
                .Include(c => c.User)
                .Where(c => User.Identity != null && c.User != null && c.Id.ToString() == id &&
                            c.User.Email == User.Identity.Name)
                .SingleAsync();
            if (complaint.IsClosed) return RedirectToAction("Index");
            return View(complaint);
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(Complaint complaint)
    {
        if (complaint.Title.Trim() == complaint.Description.Trim())
        {
            ModelState.AddModelError("name", "Name and Description must be different.");
        }

        if (!ModelState.IsValid)
        {
            return View(complaint);
        }
        if (complaint.IsClosed) return RedirectToAction("Index");
        complaint.Action = null;
        complaint.UpdatedAt =DateTime.Now;
        _dbContext.Complaints.Update(complaint);
        await _dbContext.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            var complaint = await _dbContext.Complaints
                .Include(c => c.User)
                .Where(c => User.Identity != null && c.User != null && c.Id.ToString() == id &&
                            c.User.Email == User.Identity.Name)
                .SingleAsync();
            if (complaint.IsClosed) return RedirectToAction("Index");
            return View(complaint);
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpPost,ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        try
        {
            var complaint = await _dbContext.Complaints
                .Where(c => User.Identity != null && c.User != null && c.Id.ToString() == id &&
                            c.User.Email == User.Identity.Name)
                .SingleAsync();
            if (complaint.IsClosed) return RedirectToAction("Index");
            _dbContext.Complaints.Remove(complaint);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }

    }
}