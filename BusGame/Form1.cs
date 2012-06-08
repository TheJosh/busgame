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
        Bitmap bg;

        Ship player1;
        Ship player2;
        List<Bullet> bullets;
        List<Cow> cows;
        Random r;

        public Form1()
        {
            InitializeComponent();

            ship = new Bitmap(Application.StartupPath + "\\Ship.png");
            bullet = new Bitmap(Application.StartupPath + "\\Bullet.png");
            cow = new Bitmap(Application.StartupPath + "\\Cow.png");
            bg = new Bitmap(Application.StartupPath + "\\Bg.png");

            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.DoubleBuffered = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            newgame();
        }

        public void newgame() {
            player1 = new Ship();
            player1.p = new Point(100, (this.ClientSize.Height - ship.Height) / 2);
            player1.h = 5;

            player2 = new Ship();
            player2.p = new Point(this.Width - ship.Width - 100, (this.ClientSize.Height - ship.Height) / 2);
            player2.h = 5;

            bullets = new List<Bullet>();
            cows = new List<Cow>();
            r = new Random();

            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Rectangle p1 = new Rectangle(player1.p.X, player1.p.Y, ship.Width, ship.Height);
            Rectangle p2 = new Rectangle(player2.p.X, player2.p.Y, ship.Width, ship.Height);

            foreach (Bullet b in bullets) {
                b.p.X += b.v.X;
            }

            foreach (Bullet b in bullets) {
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

            foreach (Bullet b in bullets) {
                if (p1.Contains(b.p)) {
                    player1.h--;
                    b.h--;
                    return;
                } else if (p2.Contains(b.p)) {
                    player2.h--;
                    b.h--;
                    return;
                }
            }

            if (player1.h == 0 && player2.h == 0) win("No one");
            if (player1.h == 0) win("Player 2");
            if (player2.h == 0) win("Player 1");

            bullets.RemoveAll(b => b.h == 0);

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

            using (TextureBrush b = new TextureBrush(bg)) {
                g.FillRectangle(b, new Rectangle(0, 0, this.Size.Width, this.Size.Height));
            }

            g.DrawImage(ship, player1.p);
            ship.RotateFlip(RotateFlipType.RotateNoneFlipX);
            g.DrawImage(ship, player2.p);
            ship.RotateFlip(RotateFlipType.RotateNoneFlipX);

            foreach (Cow c in cows) {
                g.DrawImage(cow, c.p);
         
            }

            foreach (Bullet b in bullets) {
                g.DrawImage(bullet, b.p);
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode) {
                case Keys.Q: player1.p.Y -= 10; break;
                case Keys.A: player1.p.Y += 10; break;
                case Keys.E: fire(player1, 30); break;

                case Keys.O: player2.p.Y -= 10; break;
                case Keys.L: player2.p.Y += 10; break;
                case Keys.U: fire(player2, -30); break;

                case Keys.Escape: Application.Exit(); break;
            }
        }

        private void fire(Ship player, int velx)
        {
            Bullet b = new Bullet();
            b.p = player.p;
            b.p.X += ship.Width * Math.Sign(velx);
            b.p.Y += ship.Height / 2;
            b.v = new Point(velx, 0);
            b.h = 1;
            bullets.Add(b);
        }
    }

    public class Ship {
        public Point p;
        public int h;
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
