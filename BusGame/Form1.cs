using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BusGame
{
    public partial class Form1 : Form
    {
        Bitmap ship;
        Bitmap bullet;

        Point player1;
        Point player2;
        List<Bullet> bullets;

        public Form1()
        {
            InitializeComponent();
            newgame();
        }

        public void newgame() {
            ship = new Bitmap("C:\\Users\\Josh\\Desktop\\Ship.png");
            bullet = new Bitmap("C:\\Users\\Josh\\Desktop\\Bullet.png");

            player1 = new Point(100, this.Height / 2);
            player2 = new Point(this.Width - 100, this.Height / 2);
            bullets = new List<Bullet>();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Rectangle p1 = new Rectangle(player1.X, player1.Y, ship.Width, ship.Height);
            Rectangle p2 = new Rectangle(player2.X, player2.Y, ship.Width, ship.Height);

            foreach (Bullet b in bullets) {
                b.p.X += b.v.X;

                if (p1.Contains(b.p)) {
                    win("Player 2");
                    return;
                } else if (p2.Contains(b.p)) {
                    win("Player 1");
                    return;
                }
            }

            this.Refresh();
        }

        private void win(string who)
        {
            MessageBox.Show(who + " wins!");
            newgame();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            g.FillRectangle(Brushes.White, new Rectangle(0, 0, this.Size.Width, this.Size.Height));

            g.DrawImage(ship, player1);
            ship.RotateFlip(RotateFlipType.RotateNoneFlipX);
            g.DrawImage(ship, player2);
            ship.RotateFlip(RotateFlipType.RotateNoneFlipX);

            foreach (Bullet b in bullets) {
                g.DrawImage(bullet, b.p);
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode) {
                case Keys.Q: player1.Y -= 10; break;
                case Keys.A: player1.Y += 10; break;
                case Keys.E: fire(player1, 30); break;

                case Keys.O: player2.Y -= 10; break;
                case Keys.L: player2.Y += 10; break;
                case Keys.U: fire(player2, -30); break;
            }
        }

        private void fire(Point player, int velx)
        {
            Bullet b = new Bullet();
            b.p = player;
            b.p.X += ship.Width * Math.Sign(velx);
            b.p.Y += ship.Height / 2;
            b.v = new Point(velx, 0);
            bullets.Add(b);
        }
    }

    public class Bullet
    {
        public Point p;
        public Point v;
    }
}
