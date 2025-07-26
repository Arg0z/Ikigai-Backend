using Ikigai_Backend.Constants;
using Ikigai_Backend.Database;
using Ikigai_Backend.DbModels;
using Ikigai_Backend.DTOs.UserDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ikigai_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IkigaiDbContext _context;
        private readonly JwtTokenService _jwtService;

        public UsersController(IkigaiDbContext context, JwtTokenService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        // GET: api/Users
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<GetUserDTO>>> GetUsers()
        {
            var users = await _context.Users
                .Include(u => u.UserRoles)
                .Select(u => new GetUserDTO
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    Created_at = u.Created_at
                })
                .ToListAsync();

            return users;
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GetUserDTO>> GetUser(int id)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .Where(u => u.Id == id)
                .Select(u => new GetUserDTO
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    Created_at = u.Created_at
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // POST: api/Users
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<GetUserDTO>> PostUser([FromBody] SignUpRequestAdminDTO request)
        {
            var roles = request.Roles?.Select(r => Enum.Parse<Roles>(r)).ToList()
                ?? new List<Roles> { Roles.User };

            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                UserRoles = roles.Select(r => new UserRole { RoleName = r }).ToList(),
                Created_at = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var userDto = new GetUserDTO
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Created_at = user.Created_at
            };

            return CreatedAtAction("GetUser", new { id = user.Id }, userDto);
        }

        // POST: api/Users/signup
        [HttpPost("signup")]
        [AllowAnonymous]
        public async Task<ActionResult<GetUserDTO>> Signup([FromBody] SignupRequestUserDTO request)
        {
            var roles = new List<Roles> { Roles.User }; // Always assign User role
            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                UserRoles = roles.Select(r => new UserRole { RoleName = r }).ToList(),
                Created_at = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var userDto = new GetUserDTO
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Created_at = user.Created_at
            };

            return CreatedAtAction("GetUser", new { id = user.Id }, userDto);
        }

        // POST: api/Users/signup/admin
        [HttpPost("signup/admin")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<GetUserDTO>> AdminSignup([FromBody] SignupRequestUserDTO request, [FromBody] List<string> roles)
        {
            var parsedRoles = roles?.Select(r => Enum.Parse<Roles>(r)).ToList() ?? new List<Roles> { Roles.User };
            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                UserRoles = parsedRoles.Select(r => new UserRole { RoleName = r }).ToList(),
                Created_at = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var userDto = new GetUserDTO
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Created_at = user.Created_at
            };

            return CreatedAtAction("GetUser", new { id = user.Id }, userDto);
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutUser(int id, [FromBody] UpdateUserAdminDTO userDTO)
        {
            if (id != userDTO.Id)
                return BadRequest("User ID mismatch");


            var user = await _context.Users.Include(u => u.UserRoles).FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
                return NotFound();

            user.Name = userDTO.Name;
            user.Email = userDTO.Email;

            // Update roles if provided
            if (userDTO.Roles != null)
            {
                // Remove existing roles
                user.UserRoles.Clear();
                // Add new roles
                foreach (var roleStr in userDTO.Roles)
                {
                    if (Enum.TryParse<Roles>(roleStr, out var role))
                    {
                        user.UserRoles.Add(new UserRole { UserId = user.Id, RoleName = role });
                    }
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestUserDTO login)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Email == login.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(login.Password, user.PasswordHash))
            {
                return Unauthorized("Invalid credentials");
            }

            var token = _jwtService.GenerateToken(
                user.Email,
                user.UserRoles.Select(ur => ur.RoleName.ToString()).ToList()
            );

            return Ok(new { token });
        }

        // PUT: api/Users/5/self
        [HttpPut("{id}/updateUserByUser")]
        [Authorize]
        public async Task<IActionResult> UpdateUserByUser(int id, [FromBody] UpdateUserDTO userDTO)
        {
            if (id != userDTO.Id)
                return BadRequest("User ID mismatch");

            // Get user from DB
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            // Ensure the authenticated user is updating their own record
            var userEmail = User.Identity?.Name;
            if (userEmail == null || !string.Equals(user.Email, userEmail, StringComparison.OrdinalIgnoreCase))
                return Forbid();

            user.Name = userDTO.Name;
            user.Email = userDTO.Email;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }
    }
}
