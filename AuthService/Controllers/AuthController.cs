using AuthService.DB;
using AuthService.Models;
using AuthService.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthDbContext _dbContext;
        private readonly JWTSettings _jwtSettings;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public AuthController(AuthDbContext dbContext, IConfiguration configuration, HttpClient httpClient)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _jwtSettings = configuration.GetSection("JWTSettings").Get<JWTSettings>();
            _httpClient = httpClient;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(User user)
        {
            // Hash the password
            string hashedPassword = HashPassword(user.Password);

            // Common SQL parameters for all users
            var parameters = new
            {
                Username = user.UserName,
                Email = user.Email,
                Phone = user.Phone,
                Password = hashedPassword,
                RoleName = user.Role
            };

            // Insert the user into the correct table based on the role
            switch (user.Role.ToLower())
            {
                case "buyer":
                    // Insert data directly into the buyer table
                    await _dbContext.LoadStoredProc("RegisterUser")
                        .WithSqlParam("Username", user.UserName)
                        .WithSqlParam("Email", user.Email)
                        .WithSqlParam("Phone", user.Phone)
                        .WithSqlParam("Password", hashedPassword)
                        .WithSqlParam("RoleName",user.Role)
                        .ExecuteStoredProc<dynamic>();
                    break;

                case "admin":
                    // Insert data directly into the admin table
                    await _dbContext.LoadStoredProc("RegisterUser")
                        .WithSqlParam("Username", user.UserName)
                        .WithSqlParam("Email", user.Email)
                        .WithSqlParam("Phone", user.Phone)
                        .WithSqlParam("Password", hashedPassword)
                        .WithSqlParam("RoleName", user.Role)
                        .ExecuteStoredProc<dynamic>();
                    break;

                case "seller":
                    // Insert data directly into the seller table
                    await _dbContext.LoadStoredProc("RegisterUser")
                        .WithSqlParam("Username", user.UserName)
                        .WithSqlParam("Email", user.Email)
                        .WithSqlParam("Phone", user.Phone)
                        .WithSqlParam("Password", hashedPassword)
                        .WithSqlParam("RoleName", user.Role)
                        .ExecuteStoredProc<dynamic>();
                    break;

                default:
                    return BadRequest("Invalid role.");
            }

            return Ok("User registered successfully.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            try
            {
                dynamic dbUser = null;
                string roleName = null;

                // Check each table for the user
                dbUser = await _dbContext.Buyer.SingleOrDefaultAsync(b => b.UserName == loginRequest.UserName);
                if (dbUser != null)
                {
                    roleName = "Buyer";
                }

                if (dbUser == null)
                {
                    dbUser = await _dbContext.Seller.SingleOrDefaultAsync(s => s.UserName == loginRequest.UserName);
                    if (dbUser != null)
                    {
                        roleName = "Seller";
                    }
                }

                if (dbUser == null)
                {
                    dbUser = await _dbContext.Admin.SingleOrDefaultAsync(a => a.UserName == loginRequest.UserName);
                    if (dbUser != null)
                    {
                        roleName = "Admin";
                    }
                }

                if (dbUser == null)
                    return Unauthorized("Invalid username or credentials.");

                if (!VerifyPassword(loginRequest.Password, dbUser.Password))
                    return Unauthorized("Invalid credentials.");

                // Map RoleName to RoleId
                var roleId = await _dbContext.UserRoles
                    .Where(r => r.RoleName == roleName)
                    .Select(r => r.RoleId)
                    .FirstOrDefaultAsync();

                // Generate a JWT token with role-based claims
                var user = new User
                {
                    Id = dbUser.Id,
                    UserName = dbUser.UserName,
                    Email = dbUser.Email, // Assuming Email is present; handle nulls if not
                    Phone = dbUser.Phone,
                    Role = roleName,
                    RoleId = roleId
                };

                var token = GenerateJWT1(user);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during login: {ex.Message}");
                return StatusCode(500, "An error occurred while processing the login.");
            }
        }

        private string GenerateJWT1(dynamic user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.Key);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.MobilePhone,user.Phone),
             new Claim("UserId", user.Id.ToString()),
             new Claim("RoleId", user.RoleId.ToString()),
                }),

                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }



        //[HttpPost("login")]
        //public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        //{
        //    var dbUser = await _dbContext.Users.SingleOrDefaultAsync(u => u.Username == loginRequest.Username);
        //    if (dbUser == null || !VerifyPassword(loginRequest.Password, dbUser.Password))
        //        return Unauthorized("Invalid credentials.");

        //    var token = GenerateJWT(dbUser);
        //    return Ok(new { Token = token });
        //}



        private string GenerateJWT(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.Key);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.MobilePhone,user.Phone),
             new Claim("UserId", user.Id.ToString()),
             new Claim("RoleId", user.RoleId.ToString()),
            //new Claim(ClaimTypes., user.Id)
                }),

                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var jwtToken = tokenHandler.WriteToken(token);

            Console.WriteLine($"Generated JWT Token: {jwtToken}");

            return jwtToken;
        }


        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        private bool VerifyPassword(string enteredPassword, string storedHashedPassword)
        {
            var hashedEnteredPassword = HashPassword(enteredPassword);
            return hashedEnteredPassword == storedHashedPassword;
        }
    }

    public class LoginRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
