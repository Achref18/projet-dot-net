using HelpDesk.Data;
using HelpDesk.Enums;
using HelpDesk.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Controllers;
[Authorize("RequireTech")]
public class TechController : Controller
{
    private readonly ApplicationDbContext _dbContext;

    public TechController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async  Task<IActionResult> Index()
    {
        var complaints = await _dbContext.Complaints
            .Include(c => c.User).ToListAsync();
        return View(complaints);
    }

    public async Task<IActionResult> Details(string id)
    {
        try
        {
            var complaint = await _dbContext.Complaints
                .Include(c=>c.User)
                .SingleAsync(c => c.Id.ToString() == id);
            return View(complaint);
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
    }

    public async Task<IActionResult> Update(string id){
        try
        {
            var complaint = await _dbContext.Complaints
                .SingleAsync(c => c.Id.ToString() == id);
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
        try
        {
            var dbComplaint = await _dbContext.Complaints.SingleAsync(c=>c.Id.ToString() == complaint.Id.ToString());
            if (complaint.Action == null)
            {
                ModelState.AddModelError("action", "Action is required.");   
            }

            if (complaint.Action?.Trim() is { Length: <= 10 })
            {
                ModelState.AddModelError("action", "Action cannot be less than 10 characters.");  
            }
            if (complaint is { Status: Status.Open, IsClosed: true })
            {
                ModelState.AddModelError("status", "Please change Status when closing the complaint."); 
            }
            if (!ModelState.IsValid)
            {
                return View(complaint);
            }

            if(complaint.IsClosed)
            {
                dbComplaint.ClosedAt = DateTime.Now;
            }
            else
            {
                dbComplaint.ClosedAt = null;
            }
            
           
            dbComplaint.Action = complaint.Action;
            dbComplaint.IsClosed = complaint.IsClosed;
            dbComplaint.Status = complaint.Status;
            dbComplaint.UpdatedAt = DateTime.Now;
           


            _dbContext.Update(dbComplaint);
            await _dbContext.SaveChangesAsync();
            return  RedirectToAction("Index");
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
    }
}