using AuthService.DB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class ProfileController : ControllerBase
    {
        private readonly AuthDbContext _dbContext;

        public ProfileController(AuthDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost("profile")]
        public async Task<IActionResult> UpdateProfile(IFormFile panCard, IFormFile aadharCard)
        {
            try
            {
                // Extract user claims
                var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
                var roleId = User.Claims.FirstOrDefault(c => c.Type == "RoleId")?.Value;

                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(roleId))
                    return Unauthorized("User claims missing or invalid.");

                // Save files to local storage
                string panCardPath = SaveFileToLocalStorage(panCard, "Pancards");
                string aadharCardPath = SaveFileToLocalStorage(aadharCard, "AadharCards");

                // Call the stored procedure to update the profile
                using var command = _dbContext.LoadStoredProc("UpdateUserProfile")
                    .WithSqlParam("UserId", int.Parse(userId))
                    .WithSqlParam("RoleId", int.Parse(roleId))
                    .WithSqlParam("PanCardPath", panCardPath ?? string.Empty)
                    .WithSqlParam("AadharCardPath", aadharCardPath ?? string.Empty);

                await command.ExecuteNonQueryAsync();

                return Ok("Profile updated successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating profile: {ex.Message}");
                return StatusCode(500, "An error occurred while updating the profile.");
            }
        }

        private string SaveFileToLocalStorage(IFormFile file, string subFolder)
        {
            if (file == null) return null;

            var directoryPath = Path.Combine("LocalStorage", subFolder);
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            var filePath = Path.Combine(directoryPath, Guid.NewGuid() + Path.GetExtension(file.FileName));
            using var stream = new FileStream(filePath, FileMode.Create);
            file.CopyTo(stream);

            return filePath;
        }
    }
}

