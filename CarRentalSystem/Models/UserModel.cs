using System;

namespace CarRentalSystem.Models
{
    public class UserModel
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        // Helper property to easily display the full name in the UI
        public string FullName => $"{FirstName} {LastName}";
    }
}