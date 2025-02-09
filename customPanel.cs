using System;
using System.Drawing;
using System.Windows.Forms;

namespace Profil
{
    public class CustomPanel : Panel
    {
        public Color BorderColor { get; set; } = Color.FromArgb(200, 200, 200); // Domyślny kolor obramowania

        public CustomPanel()
        {
            // Włącz podwójne buforowanie, aby uniknąć migotania
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true); // Wymuś odświeżenie podczas zmiany rozmiaru
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Wyczyszczenie tła, aby uniknąć powielania rysunków
            base.OnPaintBackground(e);
            e.Graphics.Clear(this.BackColor);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            using (Pen pen = new Pen(BorderColor, 1)) // Kolor i grubość obramowania
            {
                // Poprawka: Prostokąt musi być mniejszy o 1 piksel od wszystkich krawędzi, aby nie wychodził poza obszar
                Rectangle rect = new Rectangle(1, 1, this.ClientSize.Width - 2, this.ClientSize.Height - 2);
                e.Graphics.DrawRectangle(pen, rect);
            }
        }



    }
}
