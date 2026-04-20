using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using FontAwesome.Sharp;

namespace CarRentalSystem.Forms
{
    public partial class CustomMessageBox : Form
    {
        // DLL Imports for Smooth Rounded Corners
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect, int nTopRect, int nRightRect, int nBottomRect,
            int nWidthEllipse, int nHeightEllipse);

        // DLL Imports for Custom Form Dragging
        [DllImport("user32.dll", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private extern static void SendMessage(IntPtr hwnd, int wmsg, int wparam, int lparam);

        // Private constructor so it can only be opened via the static Show() method
        private CustomMessageBox(string message, string title, IconChar icon, Color iconColor)
        {
            InitializeComponent();

            // Apply 15px rounded corners
            this.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 15, 15));

            lblTitle.Text = title;
            lblMessage.Text = message;

            // Set FontAwesome Icon and Color dynamically
            iconBox.IconChar = icon;
            iconBox.IconColor = iconColor;

            // DYNAMIC THEME: Match the top border line and OK button to the alert icon color!
            pnlTopBorder.BackColor = iconColor;
            btnOk.PrimaryColor = iconColor;
            btnOk.BorderColor = iconColor;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Allows the user to drag the borderless message box
        private void DragForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, 0x112, 0xf012, 0);
            }
        }

        // The static method used to summon the box from anywhere in the app
        public static void Show(string message, string title, IconChar icon, Color iconColor)
        {
            using (CustomMessageBox customMsg = new CustomMessageBox(message, title, icon, iconColor))
            {
                customMsg.ShowDialog();
            }
        }
    }
}