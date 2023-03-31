using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParksLookupApi.Models;

namespace ParksLookupApi.Controllers
{
  // [Authorize]
  [Route("api/[controller]")]
  [ApiController]
  public class ReviewsController : ControllerBase
  {
    private readonly ParksLookupApiContext _db;

    public ReviewsController(ParksLookupApiContext db)
    {
      _db = db;
    }

    // GET api/reviews
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Review>>> Get(string title)
    {
      IQueryable<Review> query = _db.Reviews.AsQueryable();

      if (title != null)
      {
        query = query.Where(entry => entry.Title == title);
      }

      return await query.ToListAsync();
    }

    // GET: api/Reviews/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Review>> GetReview(int id)
    {
      Review review = await _db.Reviews.FindAsync(id);

      if (review == null)
      {
        return NotFound();
      }

      return review;
    }

    // POST api/reviews
    [HttpPost]
    public async Task<ActionResult<Review>> Post(Review review)
    {
      _db.Reviews.Add(review);
      await _db.SaveChangesAsync();
      return CreatedAtAction(nameof(GetReview), new { id = review.ReviewId }, review);
    }


    // PUT: api/Reviews/5
    [HttpPut("{id}")]
public async Task<IActionResult> Put(int id, Review review)
{
    if (id != review.ReviewId)
    {
        return BadRequest();
    }

    var existingReview = await _db.Reviews.FindAsync(id);

    if (existingReview == null)
    {
        return NotFound();
    }

    if (existingReview.user_name != review.user_name)
    {
        return BadRequest("User name does not match.");
    }

    // Update only the title and description, not the user_name
    existingReview.Title = review.Title;
    existingReview.Description = review.Description;

    try
    {
        await _db.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException)
    {
        if (!ReviewExists(id))
        {
            return NotFound();
        }
        else
        {
            throw;
        }
    }

    return NoContent();
}

private bool ReviewExists(int id)
{
    return _db.Reviews.Any(e => e.ReviewId == id);
}

    // DELETE: api/Reviews/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReview(int id)
    {
      Review review = await _db.Reviews.FindAsync(id);
      if (review == null)
      {
        return NotFound();
      }

      _db.Reviews.Remove(review);
      await _db.SaveChangesAsync();

      return NoContent();
    }
  }
}