using datingapp.Models; 
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace datingapp.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private static List<User> Users = new List<User>
        {
            new User { Id = 1, Name = "John Doe", Email = "john@example.com", PasswordHash = HashPassword("password123") },
            new User { Id = 2, Name = "Jane Smith", Email = "jane@example.com", PasswordHash = HashPassword("securepassword") }
        };

        [HttpGet]
        public IActionResult GetUsers()
        {
            var usersWithoutPasswords = Users.Select(u => new 
            {
                u.Id,
                u.Name,
                u.Email
            }).ToList(); 

            return Ok(usersWithoutPasswords);
        }

        [HttpGet("{id}")]
        public IActionResult GetUserById(int id)
        {
            var user = Users.Find(u => u.Id == id);
            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }

            var userWithoutPassword = new 
            {
                user.Id,
                user.Name,
                user.Email
            };

            return Ok(userWithoutPassword);
        }

        [HttpPost]
        public IActionResult CreateUser([FromBody] UserRequest request)
        {
            var newUser = new User
            {
                Id = Users.Count + 1,
                Name = request.Name,
                Email = request.Email,
                PasswordHash = HashPassword(request.Password)
            };

            Users.Add(newUser);
            return CreatedAtAction(nameof(GetUserById), new { id = newUser.Id }, newUser);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, [FromBody] UserRequest request)
        {
            var user = Users.Find(u => u.Id == id);
            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }

            user.Name = request.Name;
            user.Email = request.Email;

            if (!string.IsNullOrEmpty(request.Password))
            {
                user.PasswordHash = HashPassword(request.Password);
            }

            return Ok(user);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            var user = Users.Find(u => u.Id == id);
            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }

            Users.Remove(user);
            return NoContent(); 
        }

        private static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}
