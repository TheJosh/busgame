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

        int t;

        Ship player1;
        Ship player2;
        List<Bullet> bullets;
        List<Cow> cows;
        List<Msg> msgs;
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
            player1.lastshot = 0;
            player1.ammo = 12;

            player2 = new Ship();
            player2.p = new Point(this.Width - ship.Width - 100, (this.ClientSize.Height - ship.Height) / 2);
            player2.h = 5;
            player2.lastshot = 0;
            player2.ammo = 12;

            bullets = new List<Bullet>();
            cows = new List<Cow>();
            msgs = new List<Msg>();
            r = new Random();

            t = 0;
            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            t++;

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
                        if (c.h == 0) {
                            b.s.h++;
                            msg("blugh", b.p.X + 20, b.p.Y - 40, Brushes.Red);
                            msg("+1", b.s.p.X + 10, b.s.p.Y - 20, Brushes.Green);

                        } else {
                            msg("moo", b.p.X + 23, b.p.Y - 40, Brushes.Red);
                        }
                    }
                }
            }

            bullets.RemoveAll(b => b.h <= 0);
            cows.RemoveAll(c => c.h <= 0);

            foreach (Bullet b in bullets) {
                if (p1.Contains(b.p)) {
                    player1.h--;
                    b.h--;
                    msg("-1", player1.p.X + 10, player1.p.Y - 20, Brushes.Red);
                    return;
                } else if (p2.Contains(b.p)) {
                    player2.h--;
                    b.h--;
                    msg("-1", player2.p.X + 10, player2.p.Y - 20, Brushes.Red);
                    return;
                }
            }

            if (player1.h == 0 && player2.h == 0) win("No one");
            if (player1.h == 0) win("Player 2");
            if (player2.h == 0) win("Player 1");

            bullets.RemoveAll(b => b.h <= 0);

            if (cows.Count < 3) {
                if (r.Next(1, 50) == 1) {
                    Cow c = new Cow();
                    c.p.X = r.Next(150, this.ClientSize.Width - 150);
                    c.p.Y = r.Next(1, this.ClientSize.Height - cow.Height);
                    c.h = 3;
                    cows.Add(c);
                }
            }

            if (t % 50 == 0) {
                player1.ammo = Math.Min(player1.ammo + 5, 15);
                player2.ammo = Math.Min(player2.ammo + 5, 15);
            }

            foreach (Msg m in msgs) {
                m.p.Y -= 4;
                m.a++;
            }

            msgs.RemoveAll(m => m.a == 10);

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
            Font f;

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

            f = new System.Drawing.Font("Arial", 12);
            foreach (Msg m in msgs) {
                g.DrawString(m.s, f, m.b, m.p);
            }

            f = new System.Drawing.Font("Arial", 26, FontStyle.Bold);
            g.DrawString(player1.h.ToString(), f, Brushes.White, new PointF(10, 10));
            g.DrawString(player2.h.ToString(), f, Brushes.White, new PointF(this.Size.Width - 40, 10));

            f = new System.Drawing.Font("Arial", 16);
            g.DrawString(player1.ammo.ToString(), f, Brushes.White, new PointF(33, 29));
            g.DrawString(player2.ammo.ToString(), f, Brushes.White, new PointF(this.Size.Width - 65, 29));
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode) {
                case Keys.Q: player1.p.Y -= 10; break;
                case Keys.A: player1.p.Y += 10; break;
                case Keys.E: fire(player1, 65); break;

                case Keys.O: player2.p.Y -= 10; break;
                case Keys.L: player2.p.Y += 10; break;
                case Keys.U: fire(player2, -65); break;

                case Keys.Escape: Application.Exit(); break;
            }
        }

        private void fire(Ship player, int velx)
        {
            if (t - player.lastshot < 4) return;
            if (player.ammo == 0) return;

            Bullet b = new Bullet();
            b.p = player.p;
            b.p.X += ship.Width * Math.Sign(velx);
            b.p.Y += ship.Height / 2;
            b.v = new Point(velx, 0);
            b.h = 1;
            b.s = player;
            bullets.Add(b);

            player.lastshot = t;
            player.ammo--;
        }

        private void msg(string s, int x, int y, Brush b)
        {
            Msg m = new Msg();
            m.p = new Point(x,y);
            m.a = 0;
            m.s = s;
            m.b = b;
            msgs.Add(m);
        }
    }

    public class Ship {
        public Point p;
        public int h;
        public int lastshot;
        public int ammo;
    }

    public class Bullet
    {
        public Point p;
        public Point v;
        public int h;
        public Ship s;
    }

    public class Cow
    {
        public Point p;
        public int h;
    }

    public class Msg
    {
        public Point p;
        public string s;
        public int a;
        public Brush b;
    }
}
