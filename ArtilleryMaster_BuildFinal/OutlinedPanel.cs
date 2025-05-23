using System.Drawing;
using System.Windows.Forms;

namespace ArtilleryMaster_BuildFinal
{
    // Custom Panel 
    public class OutlinedPanel : Panel
    {
        public OutlinedPanel()
        {
            this.DoubleBuffered = true;
            this.BackColor = Color.Black;
            this.BorderStyle = BorderStyle.None; // Ensure no default border
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Fill background manually to avoid flicker and ensure outline is visible
            using (SolidBrush brush = new SolidBrush(this.BackColor))
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            // Draw a more pronounced outline (thicker and brighter)
            using (Pen pen = new Pen(Color.Cyan, 4)) // Thicker and more visible color
            {
                Rectangle rect = this.ClientRectangle;
                rect.Width -= 2;
                rect.Height -= 2;
                e.Graphics.DrawRectangle(pen, rect);
            }
        }
    }
}