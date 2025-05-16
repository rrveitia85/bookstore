using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserCrudApi
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }

    public static class UserStore
    {
        public static List<User> Users = new List<User>
        {
            new User { Id = 1, Name = "Alice", Email = "alice@example.com" },
            new User { Id = 2, Name = "Bob", Email = "bob@example.com" }
        };
    }

    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok(UserStore.Users);

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var user = UserStore.Users.FirstOrDefault(u => u.Id == id);
            return user == null ? NotFound() : Ok(user);
        }

        [HttpPost]
        public IActionResult Post([FromBody] User user)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            user.Id = UserStore.Users.Max(u => u.Id) + 1;
            UserStore.Users.Add(user);
            return CreatedAtAction(nameof(Get), new { id = user.Id }, user);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] User updated)
        {
            var user = UserStore.Users.FirstOrDefault(u => u.Id == id);
            if (user == null) return NotFound();
            if (!ModelState.IsValid) return BadRequest(ModelState);

            user.Name = updated.Name;
            user.Email = updated.Email;
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var user = UserStore.Users.FirstOrDefault(u => u.Id == id);
            if (user == null) return NotFound();
            UserStore.Users.Remove(user);
            return NoContent();
        }
    }

    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        public LoggingMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            Console.WriteLine($"[{DateTime.Now}] Request: {context.Request.Method} {context.Request.Path}");
            await _next(context);
        }
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseMiddleware<LoggingMiddleware>();
            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
                .Build()
                .Run();
        }
    }
}
