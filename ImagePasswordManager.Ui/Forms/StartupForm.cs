using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using SimplePasswordManager.Models;
using SimplePasswordManager.States;

namespace SimplePasswordManager
{
    public partial class StartupForm : Form
    {
        public StartupForm()
        {
            InitializeComponent();

            _authenticationModels = new BindingList<AuthenticationModel> { new AuthenticationModel() };
            StatusLabel.Text = "Open an image file to begin.";

            InitializeGridView();
        }

        /// <summary>
        ///     Binding List
        /// </summary>
        private BindingList<AuthenticationModel> _authenticationModels { get; set; }

        /// <summary>
        ///     Initializes main grid
        /// </summary>
        private void InitializeGridView()
        {
            AuthenticationDataGridView.ColumnCount = 2;

            AuthenticationDataGridView.Columns[0].Name = "Name";
            AuthenticationDataGridView.Columns[0].DataPropertyName = "Name";
            AuthenticationDataGridView.Columns[0].Width = 350;

            AuthenticationDataGridView.Columns[1].Name = "Password";
            AuthenticationDataGridView.Columns[1].DataPropertyName = "Password";
            AuthenticationDataGridView.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            // AuthenticationDataGridView.AutoResizeColumns();
            // AuthenticationDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            AuthenticationDataGridView.AutoGenerateColumns = false;
            AuthenticationDataGridView.DataSource = _authenticationModels;
        }

        /// <summary>
        ///     De-selects first row/column once binding is complete
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AuthenticationDataGridView_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            AuthenticationDataGridView.ClearSelection();
        }

        /// <summary>
        ///     Opens a file/loads if has encryption/authentication
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog();

            openFileDialog.DefaultExt = ".jpg";
            openFileDialog.Filter = "Image types (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            var result = openFileDialog.ShowDialog();

            if (result != DialogResult.OK)
            {
                return;
            }

            AuthenticationState.ImageFilePath = openFileDialog.FileName;
            StatusLabel.Text = $"{AuthenticationState.ImageFilePath}";

            AuthenticationDataGridView.Enabled = true;
            AuthenticationDataGridView.DataSource = null;

            if (AuthenticationState.IsAuthenticatedFile())
            {
                _authenticationModels = AuthenticationState.Load();
                AuthenticationDataGridView.DataSource = _authenticationModels;
            }
            else
            {
                _authenticationModels = new BindingList<AuthenticationModel> { new AuthenticationModel() };
            }

            AuthenticationDataGridView.Invalidate();
        }

        /// <summary>
        ///     Saves a file with authentication
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists(AuthenticationState.ImageFilePath))
            {
                // Tabbing or de-selecting creates the new row in the grid yet creates an empty object to remove.
                var removeEmptyAuthenticationModels = _authenticationModels.Where(c => string.IsNullOrWhiteSpace(c.Name)).ToList();

                foreach (var model in removeEmptyAuthenticationModels)
                {
                    _authenticationModels.Remove(model);
                }

                AuthenticationState.Save(_authenticationModels);
                MessageBox.Show("Image saved.", "Image Authentication");
            }
            else
            {
                MessageBox.Show("Open an image file.", "Image Authentication");
            }
        }

        /// <summary>
        ///     Creates a new row/object
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (AuthenticationDataGridView.Focused && keyData == Keys.Tab &&
                AuthenticationDataGridView.CurrentCell.ColumnIndex == AuthenticationDataGridView.Columns.Count - 1 &&
                AuthenticationDataGridView.CurrentRow.Index == AuthenticationDataGridView.RowCount - 1)
            {
                AuthenticationDataGridView.BeginEdit(false);
                AuthenticationDataGridView.NotifyCurrentCellDirty(true);
                AuthenticationDataGridView.EndEdit();
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}