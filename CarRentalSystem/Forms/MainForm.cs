using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using CarRentalSystem.Services;

namespace CarRentalSystem.Forms
{
    public partial class MainForm : Form
    {
        private readonly UserService _userService;

        // DLL Imports for Smooth Rounded Corners
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse
        );

        // DLL Imports for Custom Form Dragging
        [DllImport("user32.dll", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private extern static void SendMessage(IntPtr hwnd, int wmsg, int wparam, int lparam);

        public MainForm()
        {
            InitializeComponent();
            _userService = new UserService();

            // Apply a sleek 20px border radius to the borderless form
            this.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
        }

        // Handles the 'X' button click to close the app
        private void lblClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // Allows the user to drag the borderless card by clicking anywhere on the background
        private void DragForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, 0x112, 0xf012, 0);
            }
        }

        // Executes the Login sequence
        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter both a username and password.", "Required Fields", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Calls the secure BCrypt authentication logic
            bool isSuccess = _userService.AuthenticateUser(username, password);

            if (isSuccess)
            {
                // AUT005 Validated: Alert the user of success and their role
                MessageBox.Show($"Welcome back, {UserService.CurrentUser.FullName}!\nRole: {UserService.CurrentUser.Role}",
                                "Login Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // TODO: Open the 1280x720 Overview Dashboard here
            }
            else
            {
                MessageBox.Show("Invalid username or password. Please try again.", "Authentication Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPassword.Text = string.Empty; // Clear password on failure
                txtPassword.Focus();
            }
        }
    }
}