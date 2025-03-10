using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Motivator.Entities;
using SharedObjects.Entities;

namespace Motivator.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GoalController(CourseWorkKrasnopolskyContext context) : Controller
    {
        private readonly CourseWorkKrasnopolskyContext Context = context;

        [HttpPost]
        public async Task<IActionResult> Post(string key, int userId, string title, string? description)
        {
            var token = await Context.Tokens.FirstOrDefaultAsync(t => t.Key == key);
            if (token == null || token.IsUsed == false)
                return BadRequest();
            Goal goal = new() { Description = description, UserId = userId, Title = title, IsCompleted = false, Steps = [] };
            await Context.Goals.AddAsync(goal);
            await Context.SaveChangesAsync();
            return Ok();
        }
        
        [HttpGet("Goals")]
        public async Task<IActionResult> GetList(string key, int goalId)
        {
            var token = await Context.Tokens.FirstOrDefaultAsync(t => t.Key == key);
            if (token == null || token.IsUsed == false)
                return BadRequest();
            var list = token.User.Goals.Where(g => g.Id > goalId);
            List<GoalDTO> result = list.Select(goal => new GoalDTO()
            {
                Description = goal.Description,
                Id = goal.Id,
                IsCompleted = goal.IsCompleted,
                Title = goal.Title,
                UserId = goal.UserId
            }).ToList();
            return Ok(result);
        }
        [HttpGet("Goal")]
        public async Task<IActionResult> Get(string key, int goalId)
        {
            var token = await Context.Tokens.FirstOrDefaultAsync(t => t.Key == key);
            if (token == null || !token.IsUsed)
                return BadRequest();
            var goal = token.User.Goals.FirstOrDefault(g => g.Id == goalId && g.UserId == token.UserId);
            if (goal == null)
                return BadRequest();
            return Ok(new GoalDTO() { Id = goal.Id, Description = goal.Description, IsCompleted = goal.IsCompleted, Title = goal.Title, UserId = goal.UserId });
        }
        [HttpPatch("Complete")]
        public async Task<IActionResult> Complete(string key, int goalId, bool complete)
        {
            var token = await Context.Tokens.FirstOrDefaultAsync(t => t.Key == key);
            if (token == null || !token.IsUsed)
                return BadRequest();
            var goal = token.User.Goals.FirstOrDefault(g => g.Id == goalId && g.UserId == token.UserId);
            if (goal == null)
                return BadRequest();
            goal.IsCompleted = complete;
            await Context.SaveChangesAsync();
            return Ok();
        }
        [HttpPatch]
        public async Task<IActionResult> Patch(string key, int goalId, string description, string title)
        {
            var token = await Context.Tokens.FirstOrDefaultAsync(t => t.Key == key);
            if (token == null || !token.IsUsed)
                return BadRequest();
            var goal = token.User.Goals.FirstOrDefault(g => g.Id == goalId && g.UserId == token.UserId);
            if (goal == null)
                return BadRequest();
            goal.Description = description;
            goal.Title = title;
            await Context.SaveChangesAsync();
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(string key, int goalId)
        {
            var token = await Context.Tokens.FirstOrDefaultAsync(t => t.Key == key);
            if (token == null || !token.IsUsed)
                return BadRequest();
            var goal = token.User.Goals.FirstOrDefault(g => g.Id == goalId && g.UserId == token.UserId);
            if (goal == null)
                return BadRequest();
            Context.Steps.RemoveRange(goal.Steps);
            Context.Goals.Remove(goal);
            await Context.SaveChangesAsync();
            return Ok();
        }
    }
}
