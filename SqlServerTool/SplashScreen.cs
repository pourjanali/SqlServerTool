using System;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices; // Required for DllImport

namespace SqlServerTool
{
    /// <summary>
    /// The splash screen form. It displays an image and closes after a set time,
    /// launching the main application form.
    /// </summary>
    public partial class SplashScreen : Form
    {
        private System.Windows.Forms.Timer timer;

        // Import the GDI32 function to create rounded corners on the form
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,     // x-coordinate of upper-left corner
            int nTopRect,      // y-coordinate of upper-left corner
            int nRightRect,    // x-coordinate of lower-right corner
            int nBottomRect,   // y-coordinate of lower-right corner
            int nWidthEllipse, // width of ellipse
            int nHeightEllipse // height of ellipse
        );

        public SplashScreen()
        {
            InitializeComponent();

            // --- Code for Rounded Corners ---
            // Set the form's region to a rounded rectangle.
            // This clips the form's corners, making them transparent.
            // The last two parameters define the radius of the corner curves.
            // I have increased them from 30 to 60 for a more rounded effect.
            this.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, this.Width, this.Height, 150, 150));
            // --- End of Code for Rounded Corners ---


            // Set up a timer to close the splash screen
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000; // 1 seconds
            timer.Tick += Timer_Tick;
            this.Load += (s, e) => timer.Start();
        }

        /// <summary>
        /// Handles the timer tick event.
        /// </summary>
        private void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();

            // Start the main form on a new thread to ensure the application
            // continues running after the splash screen closes.
            Thread thread = new Thread(() =>
            {
                Application.Run(new Form1());
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            // Close this splash screen form
            this.Close();
        }
    }
}

