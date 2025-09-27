using System;
using System.Windows.Forms;

namespace SqlServerTool
{
    public partial class InputBoxForm : Form
    {
        private InputBoxForm(string prompt, string title, string defaultValue)
        {
            InitializeComponent();
            this.Text = title;
            lblPrompt.Text = prompt;
            txtInput.Text = defaultValue;
        }

        public string UserInput => txtInput.Text;

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        /// <summary>
        /// Displays a prompt in a dialog box, waits for the user to input text or click a button, and then returns a string containing the contents of the text box.
        /// </summary>
        /// <param name="prompt">The message to display in the dialog box.</param>
        /// <param name="title">The title for the dialog box.</param>
        /// <param name="defaultValue">The default text to display in the input box.</param>
        /// <returns>The user's input, or an empty string if the user clicked Cancel.</returns>
        public static (DialogResult Result, string Input) Show(string prompt, string title, string defaultValue = "")
        {
            using (var form = new InputBoxForm(prompt, title, defaultValue))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    return (result, form.UserInput);
                }
                return (result, string.Empty);
            }
        }
    }
}
