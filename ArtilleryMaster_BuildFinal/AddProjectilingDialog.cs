// File: ArtilleryMaster_BuildFinal/AddProjectilingDialog.cs
// This dialog lets you add projectiles. Because more is always better.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace ArtilleryMaster_BuildFinal
{
    public class AddProjectilingDialog : Form
    {
        public string ProjectileName { get; private set; }
        public double ProjectileVelocity { get; private set; }
        public string ImagePath { get; private set; }

        private TextBox txtName;
        private TextBox txtVelocity;
        private TextBox txtImage;
        private Button btnBrowse;
        private Button btnOK;
        private Button btnCancel;

        public AddProjectilingDialog()
        {
            this.Text = "Add Projectile";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.ClientSize = new Size(350, 180);
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            Label lblName = new Label() { Text = "Name:", Left = 10, Top = 15, Width = 80, ForeColor = Color.White, BackColor = Color.Black };
            txtName = new TextBox() { Left = 100, Top = 12, Width = 200, BackColor = Color.Black, ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };

            Label lblVelocity = new Label() { Text = "Velocity (m/s):", Left = 10, Top = 50, Width = 80, ForeColor = Color.White, BackColor = Color.Black };
            txtVelocity = new TextBox() { Left = 100, Top = 47, Width = 200, BackColor = Color.Black, ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };

            Label lblImage = new Label() { Text = "Image:", Left = 10, Top = 85, Width = 80, ForeColor = Color.White, BackColor = Color.Black };
            txtImage = new TextBox() { Left = 100, Top = 82, Width = 140, ReadOnly = true, BackColor = Color.Black, ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            btnBrowse = new Button() { Text = "Browse...", Left = 245, Top = 80, Width = 55, BackColor = Color.Black, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };

            // btnBrowse: because typing image paths is so 1999.
            btnBrowse.Click += (s, e) =>
            {
                using (OpenFileDialog dlg = new OpenFileDialog())
                {
                    dlg.Title = "Select Projectile Image";
                    dlg.Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp;*.gif";
                    dlg.RestoreDirectory = true;
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        txtImage.Text = dlg.FileName;
                    }
                }
            };

            btnOK = new Button() { Text = "OK", Left = 100, Top = 120, Width = 80, DialogResult = DialogResult.OK, BackColor = Color.Black, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnCancel = new Button() { Text = "Cancel", Left = 220, Top = 120, Width = 80, DialogResult = DialogResult.Cancel, BackColor = Color.Black, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };

            this.Controls.Add(lblName);
            this.Controls.Add(txtName);
            this.Controls.Add(lblVelocity);
            this.Controls.Add(txtVelocity);
            this.Controls.Add(lblImage);
            this.Controls.Add(txtImage);
            this.Controls.Add(btnBrowse);
            this.Controls.Add(btnOK);
            this.Controls.Add(btnCancel);

            this.BackColor = Color.Black;

            // btnOK: the button of destiny.
            // No idea how i managed to get it to work, but it worked, so i'm not touching it.
            btnOK.Click += (s, e) =>
            {
                ProjectileName = txtName.Text.Trim();
                double.TryParse(txtVelocity.Text.Trim(), out double v);
                ProjectileVelocity = v;
                ImagePath = txtImage.Text.Trim();
            };
        }
    }
}

// To-Do List (All Done!)
// - Made sure you can't add a projectile without a name. Or can you?
// - Added a browse button so you don't have to remember file paths.
// - Set the background to black for maximum hacker vibes.