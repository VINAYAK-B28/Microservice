namespace AuthService.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; } // Used as Username for login
        public string Password { get; set; }
        public string? Pan { get; set; }
        public string? Aadhar { get; set; }
        public string Role { get; set; }
        public int RoleId { get; set; }
    }
    public class Buyer
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }

        public string Phone { get; set; } // Used as Username for login
        public string Password { get; set; }
        public string? Pan { get; set; }
        public string? Aadhar { get; set; }
    }

    public class Admin
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }

        public string Phone { get; set; } // Used as Username for login
        public string Password { get; set; }
        public string? Pan { get; set; }
        public string? Aadhar { get; set; }
    }

    public class Seller
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }

        public string Phone { get; set; } // Used as Username for login
        public string Password { get; set; }
        public string? Pan { get; set; }
        public string? Aadhar { get; set; }
    }

    public class UserRole
    {
        public int RoleId { get; set; } // User's unique identifier
        public string RoleName { get; set; } // Role associated with the user (e.g., Buyer, Seller, Admin)
    }



    public class UserModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }

        
    }

    public class UpdateProfileRequest
    {
        public IFormFile Pancard { get; set; }
        public IFormFile AadharCard { get; set; }
    }

    public class login
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
