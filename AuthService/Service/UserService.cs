using AuthService.DB;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Service
{
    public class UserService : IUserService
    {
        private readonly AuthDbContext _context;

        public UserService(AuthDbContext context)
        {
            _context = context;
        }


        public async Task<bool> UpdateUserProfileAsync(int userId, string pancardPath, string aadharCardPath, string role)
        {
            var command = _context.LoadStoredProc("UpdateUserProfile")
                .WithSqlParam("UserId", userId)
                .WithSqlParam("Role", role)
                .WithSqlParam("PancardPath", pancardPath)
                .WithSqlParam("AadharCardPath", aadharCardPath);

            var result = await command.ExecuteStoredProc<int>();
            return result.Any();  // Returns true if the update was successful
        }
    }

}
