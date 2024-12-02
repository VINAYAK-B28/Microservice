namespace AuthService.Service
{
    public interface IUserService
    {
        Task<bool> UpdateUserProfileAsync(int userId, string pancardPath, string aadharCardPath, string role);
    }

}
