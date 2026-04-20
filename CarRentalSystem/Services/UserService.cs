using System;
using BCrypt.Net;
using CarRentalSystem.Models;
using CarRentalSystem.Repositories;

namespace CarRentalSystem.Services
{
    public class UserService
    {
        private readonly UserRepository _userRepository;

        // This static property holds the currently logged-in user's profile for the whole app
        public static UserModel CurrentUser { get; private set; }

        public UserService()
        {
            _userRepository = new UserRepository();
        }

        public UserModel GetUserByUsername(string username)
        {
            return _userRepository.GetUserByUsername(username);
        }

        // Fulfills AUT001 & AUT005: Authenticates the user and sets the active session
        public bool AuthenticateUser(string username, string rawPassword)
        {
            var user = _userRepository.GetUserByUsername(username);

            // If the user doesn't exist, login fails
            if (user == null)
                return false;

            // BCrypt securely verifies the typed password against the hashed database password
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(rawPassword, user.PasswordHash);

            if (isPasswordValid)
            {
                CurrentUser = user; // Successfully logged in! Set the session.
                return true;
            }

            return false;
        }

        // Fulfills AUT007: Logs the user out by clearing the session
        public void LogoutUser()
        {
            CurrentUser = null;
        }

        // Fulfills AUT002, AUT003, AUT004: Hashes the password and saves the new user
        public void RegisterUser(string username, string rawPassword, string role, string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(rawPassword))
                throw new ArgumentException("Username and Password are required.");

            // NEVER save a plain text password. Hash it!
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(rawPassword);

            var newUser = new UserModel
            {
                Username = username,
                PasswordHash = hashedPassword,
                Role = role,
                FirstName = firstName,
                LastName = lastName,
                IsActive = true
            };

            _userRepository.CreateUser(newUser);
        }
    }
}