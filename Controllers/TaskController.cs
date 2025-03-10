using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Motivator.Entities;
using SharedObjects.Entities;

namespace Motivator.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController(CourseWorkKrasnopolskyContext context) : Controller
    {
        private readonly CourseWorkKrasnopolskyContext Context = context;

        [HttpPost]
        public async Task<IActionResult> Post(string key, int userId, int goalId, string title, string? description)
        {
            var token = await Context.Tokens.FirstOrDefaultAsync(t => t.Key == key);
            var goal = await Context.Goals.FirstOrDefaultAsync(g => g.Id == goalId);
            if (token == null || goal == null || goal.UserId != userId || token.IsUsed == false)
                return BadRequest();

            Step step = new() { Description = description, GoalId = goal.Id, Title = title, IsCompleted = false };
            await Context.Steps.AddAsync(step);
            await Context.SaveChangesAsync();
            return Ok();
        }
        [HttpGet("StepsById")]
        public async Task<IActionResult> GetListById(string key, int stepId)
        {
            var token = await Context.Tokens.FirstOrDefaultAsync(t => t.Key == key);
            if (token == null || !token.IsUsed) return BadRequest();

            var steps = Context.Steps.Where(s => s.Id > stepId && s.Goal.UserId == token.UserId);
            if (steps == null) return NotFound();

            List<StepDTO> list = await steps.Select(step => new StepDTO()
            {
                Description = step.Description,
                Id = step.Id,
                IsCompleted = step.IsCompleted,
                Title = step.Title,
                GoalId = step.GoalId
            }).ToListAsync();
            return Ok(list);
        }
        [HttpGet("Steps")]
        public async Task<IActionResult> GetList(string key, int goalId)
        {
            var token = await Context.Tokens.FirstOrDefaultAsync(t => t.Key == key);
            if (token == null || !token.IsUsed) return BadRequest();

            var goal = token.User.Goals.FirstOrDefault(g => g.Id == goalId);
            if (goal == null) return BadRequest();

            List<StepDTO> list = goal.Steps.Select(step => new StepDTO()
            {
                Description = step.Description,
                Id = step.Id,
                IsCompleted = step.IsCompleted,
                Title = step.Title,
                GoalId = step.GoalId
            }).ToList();
            return Ok(list);
        }
        [HttpGet("Step")]
        public async Task<IActionResult> Get(string key, int stepId)
        {
            var token = await Context.Tokens.FirstOrDefaultAsync(t => t.Key == key);
            if (token == null || !token.IsUsed)
                return BadRequest();
            var step = await Context.Steps.FirstOrDefaultAsync(s => s.Id == stepId && s.Goal.UserId == token.UserId);
            if (step == null)
                return BadRequest();
            return Ok(new StepDTO() { Id = step.Id, Description = step.Description, IsCompleted = step.IsCompleted, Title = step.Title });
        }
        [HttpPatch("Complete")]
        public async Task<IActionResult> Complete(string key, int stepId, bool complete)
        {
            var token = await Context.Tokens.FirstOrDefaultAsync(t => t.Key == key);
            if (token == null || !token.IsUsed)
                return BadRequest();
            var step = Context.Steps.FirstOrDefault(g => g.Id == stepId && g.Goal.UserId == token.UserId);
            if (step == null)
                return BadRequest();
            step.IsCompleted = complete;
            await Context.SaveChangesAsync();
            return Ok();
        }
        [HttpPatch]
        public async Task<IActionResult> Patch(string key, int stepId, string description, string title)
        {
            var token = await Context.Tokens.FirstOrDefaultAsync(t => t.Key == key);
            if (token == null || !token.IsUsed)
                return BadRequest();
            var step = Context.Steps.FirstOrDefault(g => g.Id == stepId && g.Goal.UserId == token.UserId);
            if (step == null)
                return BadRequest();
            step.Description = description;
            step.Title = title;
            await Context.SaveChangesAsync();
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(string key, int stepId)
        {
            var token = await Context.Tokens.FirstOrDefaultAsync(t => t.Key == key);
            if (token == null || !token.IsUsed)
                return BadRequest();
            var step = Context.Steps.FirstOrDefault(g => g.Id == stepId && g.Goal.UserId == token.UserId);
            if (step == null)
                return BadRequest();
            Context.Steps.Remove(step);
            await Context.SaveChangesAsync();
            return Ok();
        }
    }
}
