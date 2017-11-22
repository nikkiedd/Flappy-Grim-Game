using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Media;
using Microsoft.VisualBasic;
namespace FlappyGrim
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        List<int> Pipe1 = new List<int>();
        List<int> Pipe2 = new List<int>();
        int PipeWidth = 55;
        int PipeDifferenceY = 140;   // the space available for Grim to move(up-down) between 2 pipes
        int PipeDifferenceX = 180;   // distance between consecutive pipes
        bool Start = true;
        bool flying;   // is true if Grim is flying
        int step = 5;   // Grim's speed
        int OriginalX, OriginalY;   // Grim's initial position
        bool ResetPipes = false;    // when restarting the game
        int points; 
        bool inPipe = false;    // if Grim's inside the space delimited by 2 pipes
        int score;  // the maximum score
        int ScoreDifference;

        private void Die()
        {
            flying = false; // Grim stops flying
            timer2.Enabled = false;
            timer3.Enabled = false;
            StartButton.Visible = true;
            StartButton.Enabled = true;
            ReadAndShowScore();
            points = 0;
            pictureBox1.Location = new Point(OriginalX, OriginalY);     // move Grim to initial pos
            ResetPipes = true;
            Pipe1.Clear();
            //Pipe2.Clear();
            pictureBox1.Image = Properties.Resources.randomgrim;
        }

        private void ReadAndShowScore()
        {
            using (StreamReader reader = new StreamReader("score.ini"))
            {
                int currentScore = int.Parse(ScoreLabel.Text);
                score = int.Parse(reader.ReadToEnd());  // maximum score
                reader.Close();
                if (currentScore >= 0)
                    ScoreDifference = score - int.Parse(ScoreLabel.Text) + 1;    // the number of points missing to surpass the maximum score( stored in the file)
                if (score < currentScore)
                {
                    MessageBox.Show(String.Format("Congrats, mortal! You beat the {0} points record. The new record is now {1}.", score, currentScore), "Flappy Grim", MessageBoxButtons.OK, MessageBoxIcon.None);
                    using (StreamWriter writer = new StreamWriter("score.ini"))    // write the new score to the file
                    {
                        writer.Write(ScoreLabel.Text);
                        writer.Close();
                    }
                }
                if (score > currentScore)
                    MessageBox.Show(String.Format("You died. You needed {0} more points to surpass the {1} points record.", ScoreDifference, score), "Flappy Grim", MessageBoxButtons.OK, MessageBoxIcon.None);
                if (score == currentScore)
                    MessageBox.Show(String.Format("You've reached the {0} points record. Try to beat it, human!", score), "Flappy Grim", MessageBoxButtons.OK, MessageBoxIcon.None);

            }
        }

        private void StartGame()
        {
            //pictureBox1.Image = Properties.Resources.randomgrim;
            ResetPipes = false;
            timer1.Enabled = true;
            timer2.Enabled = true;
            timer3.Enabled = true;
            Random random = new Random();
            int num = random.Next(40, this.Height - this.PipeDifferenceY);
            int num1 = num + this.PipeDifferenceY;
            Pipe1.Clear();
            Pipe1.Add(this.Width);
            Pipe1.Add(num);
            Pipe1.Add(this.Width);
            Pipe1.Add(num1);

            num = random.Next(40, (this.Height - this.PipeDifferenceY));
            num1 = num + this.PipeDifferenceY;
            Pipe2.Clear();
            Pipe2.Add(this.Width + PipeDifferenceX);
            Pipe2.Add(num);
            Pipe2.Add(this.Width + PipeDifferenceX);
            Pipe2.Add(num1);

            StartButton.Visible = false;
            StartButton.Enabled = false;
            flying = true;
            Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            StartGame();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            //  create pipes randomly
            if (Pipe1[0] + PipeWidth <= 0 | Start == true)
            {
                Random rnd = new Random();
                int px = this.Width;
                int py = rnd.Next(40, (this.Height - PipeDifferenceY));
                var p2x = px;
                var p2y = py + PipeDifferenceY;
                Pipe1.Clear();
                Pipe1.Add(px);
                Pipe1.Add(py);
                Pipe1.Add(p2x);
                Pipe1.Add(p2y);
            }
            else
            {
                Pipe1[0] = Pipe1[0] - 2;
                Pipe1[2] = Pipe1[2] - 2;
            }
            if (Pipe2[0] + PipeWidth <= 0)
            {
                Random rnd = new Random();
                int px = this.Width;
                int py = rnd.Next(40, (this.Height - PipeDifferenceY));
                var p2x = px;
                var p2y = py + PipeDifferenceY;
                int[] p1 = { px, py, p2x, p2y };
                Pipe2.Clear();
                Pipe2.Add(px);
                Pipe2.Add(py);
                Pipe2.Add(p2x);
                Pipe2.Add(p2y);
            }
            else
            {
                Pipe2[0] = Pipe2[0] - 2;
                Pipe2[2] = Pipe2[2] - 2;
            }
            if (Start == true)
                Start = false;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            // randomly drawing 4 pipes
            if (!ResetPipes && Pipe1.Any() && Pipe2.Any())
            {
                // first up
                e.Graphics.FillRectangle(Brushes.DarkSeaGreen, new Rectangle(Pipe1[0], 0, PipeWidth, Pipe1[1]));
                e.Graphics.DrawRectangle(new Pen(Brushes.Black, 2), new Rectangle(Pipe1[0], 0, PipeWidth, Pipe1[1]));
                e.Graphics.DrawRectangle(new Pen(Brushes.Black, 3), new Rectangle(Pipe1[0] - 10, Pipe1[3] - PipeDifferenceY, 75, 15));
                e.Graphics.FillRectangle(Brushes.ForestGreen, new Rectangle(Pipe1[0] - 10, Pipe1[3] - PipeDifferenceY, 75, 15)); // drawing the end of the pipe
                // first down
                e.Graphics.FillRectangle(Brushes.DarkSeaGreen, new Rectangle(Pipe1[2], Pipe1[3], PipeWidth, this.Height - Pipe1[3]));
                e.Graphics.DrawRectangle(new Pen(Brushes.Black, 2), new Rectangle(Pipe1[2], Pipe1[3], PipeWidth, this.Height - Pipe1[3]));
                e.Graphics.DrawRectangle(new Pen(Brushes.Black, 3), new Rectangle(Pipe1[2] - 10, Pipe1[3], 75, 15));
                e.Graphics.FillRectangle(Brushes.ForestGreen, new Rectangle(Pipe1[2] - 10, Pipe1[3], 75, 15)); // drawing the end of the pipe
                // second up
                e.Graphics.FillRectangle(Brushes.DarkSeaGreen, new Rectangle(Pipe2[0], 0, PipeWidth, Pipe2[1]));
                e.Graphics.DrawRectangle(new Pen(Brushes.Black, 2), new Rectangle(Pipe2[0], 0, PipeWidth, Pipe2[1]));
                e.Graphics.DrawRectangle(new Pen(Brushes.Black, 3), new Rectangle(Pipe2[0] - 10, Pipe2[3] - PipeDifferenceY, 75, 15));
                e.Graphics.FillRectangle(Brushes.ForestGreen, new Rectangle(Pipe2[0] - 10, Pipe2[3] - PipeDifferenceY, 75, 15)); // drawing the end of the pipe
                // second down
                e.Graphics.FillRectangle(Brushes.DarkSeaGreen, new Rectangle(Pipe2[2], Pipe2[3], PipeWidth, this.Height - Pipe2[3]));
                e.Graphics.DrawRectangle(new Pen(Brushes.Black, 2), new Rectangle(Pipe2[2], Pipe2[3], PipeWidth, this.Height - Pipe2[3]));
                e.Graphics.DrawRectangle(new Pen(Brushes.Black, 3), new Rectangle(Pipe2[2] - 10, Pipe2[3], 75, 15));
                e.Graphics.FillRectangle(Brushes.ForestGreen, new Rectangle(Pipe2[2] - 10, Pipe2[3], 75, 15)); // drawing the end of the pipe
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //var sound = new SoundPlayer(Properties.Resources.creepyMusic);
            //sound.Play(); - it makes the game crash :(
            //string soundfile = "Properties\\Resources\\creepymusic.wav";
            //byte[] bt = File.ReadAllBytes(soundfile);
            //var sound = new SoundPlayer(soundfile);
            //sound.Play();

            OriginalX = pictureBox1.Location.X;
            OriginalY = pictureBox1.Location.Y;
            if (!File.Exists("score.ini"))
                File.Create("score.ini").Dispose(); // create and close it

        }

        private void CheckForPoint()
        {
            Rectangle rec = pictureBox1.Bounds;
            Rectangle rec1 = new Rectangle(Pipe1[2] + 20, Pipe1[3] - PipeDifferenceY,15,PipeDifferenceY);   // first up
            Rectangle rec2 = new Rectangle(Pipe2[2] + 20, Pipe2[3] - PipeDifferenceY, 15, PipeDifferenceY); // first down
            Rectangle intersect1 = Rectangle.Intersect(rec, rec1);  // intersection between Grim and the first upward pipe
            Rectangle intersect2 = Rectangle.Intersect(rec, rec2);  // intersection between Grim and the first downward pipe

            if (!ResetPipes | Start)
            {
                if (intersect1!= Rectangle.Empty | intersect2 != Rectangle.Empty)   
                {
                    if (!inPipe)    
                    {
                        points++;
                        inPipe = true;
                    }
                }
                else
                {
                    inPipe = false;
                }
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)   // when pressing a key
        {
            switch(e.KeyCode)
            {
                case Keys.Space:
                    step = -5;
                    pictureBox1.Image = Properties.Resources.deviousgrim;
                    break;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e) // when releasing a key
        {
            switch (e.KeyCode)
            {
                case Keys.Space:
                    step = 5;
                    pictureBox1.Image = Properties.Resources.randomgrim;
                    break;
            }
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            pictureBox1.Location = new Point(pictureBox1.Location.X, pictureBox1.Location.Y+step);
            if (pictureBox1.Location.Y < 0) // in order not to let Grim fall out of the form
                pictureBox1.Location = new Point(pictureBox1.Location.X, 0);
            if (pictureBox1.Location.Y + pictureBox1.Height > this.ClientSize.Height)
                pictureBox1.Location = new Point(pictureBox1.Location.X, this.ClientSize.Height - pictureBox1.Height);
            CheckForCollision();
            if (flying)
                CheckForPoint();
            ScoreLabel.Text = Convert.ToString(points);
        }

        private void CheckForCollision()
        {
            Rectangle rec = pictureBox1.Bounds;
            Rectangle rec1 = new Rectangle(Pipe1[0], 0, PipeWidth, Pipe1[1]);   // first up
            Rectangle rec2 = new Rectangle(Pipe1[2], Pipe1[3], PipeWidth, this.Height - Pipe1[3]); // first down
            Rectangle rec3 = new Rectangle(Pipe2[0], 0, PipeWidth, Pipe2[1]);  // second up
            Rectangle rec4 = new Rectangle(Pipe2[2], Pipe2[3], PipeWidth, this.Height - Pipe2[3]);  // second down

            Rectangle intersect1 = Rectangle.Intersect(rec, rec1);
            Rectangle intersect2 = Rectangle.Intersect(rec, rec2);
            Rectangle intersect3 = Rectangle.Intersect(rec, rec3);
            Rectangle intersect4 = Rectangle.Intersect(rec, rec4);
            if (!ResetPipes | Start)
            {
                if (intersect1 != Rectangle.Empty | intersect2 != Rectangle.Empty | intersect3 != Rectangle.Empty | intersect4 != Rectangle.Empty)
                {
                    Die();
                }
            }

        }
    }
}
