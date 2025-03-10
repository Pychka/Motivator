using SharedObjects.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Motivator.Entities;

namespace Motivator.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorizationController(CourseWorkKrasnopolskyContext context) : Controller
    {
        private readonly CourseWorkKrasnopolskyContext Context = context;
        [HttpPost]
        public async Task<IActionResult> GetToken(string login, string password)
        {
            var user = await Context.Users.FirstOrDefaultAsync(u => u.Login == login);
            if (user == null || user.Password != password)
                return BadRequest();
            string key = Guid.NewGuid().ToString();
            while (await Context.Tokens.AnyAsync(t => t.Key == key))
                key = Guid.NewGuid().ToString();
            var token = new Token() { Key = key, IsUsed = true, UserId = user.Id };
            await Context.Tokens.AddAsync(token);
            await Context.SaveChangesAsync();
            return Ok(new TokenDTO() { IsUsed = true, Key = key, UserId = user.Id, Id = token.Id });
        }
        [HttpGet]
        public async Task<IActionResult> Enter(string key)
        {
            var token = await Context.Tokens.FirstOrDefaultAsync(t => t.Key == key);
            if (token == null || !token.IsUsed)
                return BadRequest();
            return Ok(new UserDTO() { Id = token.User.Id, FirstName = token.User.FirstName, Login = token.User.Login, Name = token.User.Name, Surname = token.User.Surname });
        }
        [HttpPost("Registration")]
        public async Task<IActionResult> Registration(string name, string firstName, string surname, string login, string password)
        {
            if (await Context.Users.AnyAsync(u => u.Login == login))
                return BadRequest();
            await Context.Users.AddAsync(new() { Login = login, Name = name, FirstName = firstName, Surname = surname, Password = password });
            await Context.SaveChangesAsync();
            return Ok(new UserDTO() { Login = login, Name = name, FirstName = firstName, Surname = surname, Password = password });
        }
        [HttpPatch]
        public async Task<IActionResult> Exit(string key)
        {
            var token = await Context.Tokens.FirstOrDefaultAsync(t => t.Key == key);
            if (token == null || token.IsUsed == false)
                return BadRequest();
            token.IsUsed = false;
            await Context.SaveChangesAsync();
            return Ok();
        }
    }
}
