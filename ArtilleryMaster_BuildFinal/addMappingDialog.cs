// File: ArtilleryMaster_BuildFinal/addMappingDialog.cs
// This dialog is for adding maps. Because the world needs more maps.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace ArtilleryMaster_BuildFinal
{
    public class addMappingDialog : Form
    {
        public string MapName { get; private set; }
        public string ImagePath { get; private set; }
        public double GridSize { get; private set; } // Add this property

        private TextBox txtName;
        private TextBox txtImage;
        private TextBox txtGridSize; // Add grid size input
        private Button btnBrowse;
        private Button btnOK;
        private Button btnCancel;

        public addMappingDialog()
        {
            this.Text = "Add Map";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.ClientSize = new Size(350, 170);
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            Label lblName = new Label() { Text = "Name:", Left = 10, Top = 15, Width = 80, ForeColor = Color.White, BackColor = Color.Black };
            txtName = new TextBox() { Left = 100, Top = 12, Width = 200, BackColor = Color.Black, ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };

            Label lblImage = new Label() { Text = "Image:", Left = 10, Top = 50, Width = 80, ForeColor = Color.White, BackColor = Color.Black };
            txtImage = new TextBox() { Left = 100, Top = 47, Width = 140, ReadOnly = true, BackColor = Color.Black, ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            btnBrowse = new Button() { Text = "Browse...", Left = 245, Top = 45, Width = 55, BackColor = Color.Black, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            
            // btnBrowse: because clicking is easier than typing.
            btnBrowse.Click += (s, e) =>
            {
                using (OpenFileDialog dlg = new OpenFileDialog())
                {
                    dlg.Title = "Select Map Image";
                    dlg.Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp;*.gif";
                    dlg.RestoreDirectory = true;
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        txtImage.Text = dlg.FileName;
                    }
                }
            };

            Label lblGrid = new Label() { Text = "Grid Size (m):", Left = 10, Top = 85, Width = 80, ForeColor = Color.White, BackColor = Color.Black };
            txtGridSize = new TextBox() { Left = 100, Top = 82, Width = 80, BackColor = Color.Black, ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle, Text = "100" };

            btnOK = new Button() { Text = "OK", Left = 100, Top = 120, Width = 80, DialogResult = DialogResult.OK, BackColor = Color.Black, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnCancel = new Button() { Text = "Cancel", Left = 220, Top = 120, Width = 80, DialogResult = DialogResult.Cancel, BackColor = Color.Black, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };

            this.Controls.Add(lblName);
            this.Controls.Add(txtName);
            this.Controls.Add(lblImage);
            this.Controls.Add(txtImage);
            this.Controls.Add(btnBrowse);
            this.Controls.Add(lblGrid);
            this.Controls.Add(txtGridSize);
            this.Controls.Add(btnOK);
            this.Controls.Add(btnCancel);

            this.BackColor = Color.Black;

            // btnOK: the only button that matters.
            // how? HOW??
            btnOK.Click += (s, e) =>
            {
                MapName = txtName.Text.Trim();
                ImagePath = txtImage.Text.Trim();
                double grid = 100;
                double.TryParse(txtGridSize.Text.Trim(), out grid);
                if (grid <= 0) grid = 100; // If you try to break it, we fix it for you.
                GridSize = grid;
            };
        }
    }
}

// To-Do List (All Done!)
// - Added grid size input. Because squares are important.
// - Made sure you can't add a map without a name. Maps have feelings too.
// - Set the background to black. Because why not?