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
        Bitmap cow;

        Point player1;
        Point player2;
        List<Bullet> bullets;
        List<Cow> cows;
        Random r;

        public Form1()
        {
            InitializeComponent();

            ship = new Bitmap(Application.StartupPath + "\\Ship.png");
            bullet = new Bitmap(Application.StartupPath + "\\Bullet.png");
            cow = new Bitmap(Application.StartupPath + "\\Cow.png");
            
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.DoubleBuffered = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            newgame();
        }

        public void newgame() {
            player1 = new Point(100, (this.ClientSize.Height - ship.Height) / 2);
            player2 = new Point(this.Width - ship.Width - 100, (this.ClientSize.Height - ship.Height) / 2);

            bullets = new List<Bullet>();
            cows = new List<Cow>();
            r = new Random();

            timer1.Enabled = true;
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

                foreach (Cow c in cows) {
                    Rectangle p = new Rectangle(c.p.X, c.p.Y, cow.Width, cow.Height);
                    if (p.Contains(b.p)) {
                        c.h--;
                        b.h--;
                    }
                }
            }

            bullets.RemoveAll(b => b.h == 0);
            cows.RemoveAll(c => c.h == 0);

            if (r.Next(1, 50) == 1) {
                Cow c = new Cow();
                c.p.X = r.Next(150, this.ClientSize.Width - 150);
                c.p.Y = r.Next(1, this.ClientSize.Height - cow.Height);
                c.h = 3;
                cows.Add(c);
            }

            this.Refresh();
        }

        private void win(string who)
        {
            timer1.Enabled = false;
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

            foreach (Cow c in cows) {
                g.DrawImage(cow, c.p);
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

                case Keys.Escape: Application.Exit(); break;
            }
        }

        private void fire(Point player, int velx)
        {
            Bullet b = new Bullet();
            b.p = player;
            b.p.X += ship.Width * Math.Sign(velx);
            b.p.Y += ship.Height / 2;
            b.v = new Point(velx, 0);
            b.h = 1;
            bullets.Add(b);
        }
    }

    public class Bullet
    {
        public Point p;
        public Point v;
        public int h;
    }

    public class Cow
    {
        public Point p;
        public int h;
    }
}
