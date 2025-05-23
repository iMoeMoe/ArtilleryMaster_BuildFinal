// File: ArtilleryMaster_BuildFinal/EditProjectilePanel.cs
// This is the EditProjectilePanel. It's like a control panel, but for projectiles. Pew pew.

using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace ArtilleryMaster_BuildFinal
{
    // HintTextBox: because sometimes you need a hint. Or two.
    public class HintTextBox : TextBox
    {
        private string _hint = "";
        [Category("Appearance")]
        public string Hint
        {
            get { return _hint; }
            set { _hint = value; Invalidate(); }
        }

        public HintTextBox()
        {
            SetStyle(ControlStyles.UserPaint, true);
            // Use a very dark gray to distinguish from black panel
            this.BackColor = Color.FromArgb(30, 30, 30);
            this.ForeColor = Color.White;
            this.BorderStyle = BorderStyle.FixedSingle;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Fill background to ensure it's not transparent
            using (SolidBrush bg = new SolidBrush(this.BackColor))
                e.Graphics.FillRectangle(bg, this.ClientRectangle);

            // Draw border for visibility
            using (Pen borderPen = new Pen(Color.White, 1))
                e.Graphics.DrawRectangle(borderPen, 0, 0, this.Width - 1, this.Height - 1);

            // Draw text or hint
            if (!string.IsNullOrEmpty(this.Text))
            {
                TextRenderer.DrawText(e.Graphics, this.Text, this.Font, this.ClientRectangle, this.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
            }
            else if (!this.Focused && !string.IsNullOrEmpty(Hint))
            {
                // Use a light color for hint for contrast
                using (var brush = new SolidBrush(Color.WhiteSmoke))
                {
                    var format = new StringFormat { LineAlignment = StringAlignment.Center };
                    Rectangle rect = this.ClientRectangle;
                    rect.Offset(2, 1);
                    e.Graphics.DrawString(Hint, this.Font, brush, rect, format);
                }
            }
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            Invalidate();
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            Invalidate();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            Invalidate();
        }
    }

    // EditProjectilePanel: where projectiles go to get a makeover.
    public class EditProjectilePanel : OutlinedPanel
    {
        public ComboBox ComboEditProjectile { get; private set; }
        public HintTextBox EditName { get; private set; }
        public HintTextBox EditVelocity { get; private set; }
        public ComboBox EditMphorkph { get; private set; }
        public HintTextBox EditImagePath { get; private set; }
        public Button BtnEditBrowse { get; private set; }
        public Button BtnEditSave { get; private set; }
        public Button BtnEditRemove { get; private set; }
        public Button BtnEditCancel { get; private set; }

        public event EventHandler SaveClicked;
        public event EventHandler RemoveClicked;
        public event EventHandler CancelClicked;
        public event EventHandler BrowseClicked;
        public event EventHandler SelectedProjectileChanged;

        public EditProjectilePanel()
        {
            this.BorderStyle = BorderStyle.None; // Ensure no default border
            this.BackColor = Color.Black;
            this.Size = new Size(400, 180);
            this.Visible = false;

            // ComboEditProjectile: pick your favorite (or least favorite) projectile.
            ComboEditProjectile = new ComboBox()
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(20, 15),
                Size = new Size(180, 25),
                BackColor = Color.Black,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            ComboEditProjectile.SelectedIndexChanged += (s, e) => SelectedProjectileChanged?.Invoke(this, e);

            EditName = new HintTextBox()
            {
                Location = new Point(20, 50),
                Size = new Size(160, 25),
                Hint = "Projectile Name"
            };

            EditVelocity = new HintTextBox()
            {
                Location = new Point(200, 50),
                Size = new Size(80, 25),
                Hint = "Velocity"
            };

            EditMphorkph = new ComboBox()
            {
                Location = new Point(290, 50),
                Size = new Size(50, 25),
                BackColor = Color.Black,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            EditMphorkph.Items.AddRange(new object[] { "m/h", "k/h", "m/s" });

            EditImagePath = new HintTextBox()
            {
                Location = new Point(20, 85),
                Size = new Size(200, 25),
                ReadOnly = true,
                Hint = "Image Path"
            };

            // BtnEditBrowse: because who doesn't love browsing for images?
            BtnEditBrowse = new Button()
            {
                Text = "Browse...",
                Location = new Point(230, 85),
                Size = new Size(75, 25),
                BackColor = Color.Black,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            BtnEditBrowse.Click += (s, e) => BrowseClicked?.Invoke(this, e);

            BtnEditSave = new Button()
            {
                Text = "Save",
                Location = new Point(20, 130),
                Size = new Size(75, 27),
                BackColor = Color.Black,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            BtnEditRemove = new Button()
            {
                Text = "Remove",
                Location = new Point(110, 130),
                Size = new Size(75, 27),
                BackColor = Color.Black,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            BtnEditCancel = new Button()
            {
                Text = "Cancel",
                Location = new Point(200, 130),
                Size = new Size(75, 27),
                BackColor = Color.Black,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            BtnEditSave.Click += (s, e) => SaveClicked?.Invoke(this, e);
            BtnEditRemove.Click += (s, e) => RemoveClicked?.Invoke(this, e);
            BtnEditCancel.Click += (s, e) => CancelClicked?.Invoke(this, e);

            this.Controls.Add(ComboEditProjectile);
            this.Controls.Add(EditName);
            this.Controls.Add(EditVelocity);
            this.Controls.Add(EditMphorkph);
            this.Controls.Add(EditImagePath);
            this.Controls.Add(BtnEditBrowse);
            this.Controls.Add(BtnEditSave);
            this.Controls.Add(BtnEditRemove);
            this.Controls.Add(BtnEditCancel);
        }
    }
}

// To-Do List (All Done!)
// - Added hints so you know what to type. No more guessing!
// - Made buttons look cool. UI is 90% of the grade, right?
// - Ensured projectiles can be edited, but not judged.