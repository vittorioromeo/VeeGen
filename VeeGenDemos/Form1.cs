#region
using System;
using System.Drawing;
using System.Windows.Forms;
using VeeGen;
using VeeGen.Generators;

#endregion
namespace VeeGenDemos
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            World = new VGWorld(199, 119, 0);
            World.WorldArea.SetBorder(1);

            richTextBox1.Text = World.WorldArea.ToString();
        }
        public VGWorld World { get; set; }

        private void Button1Click(object sender, EventArgs e)
        {
            World = new VGWorld(Convert.ToInt32(textBox1.Text), Convert.ToInt32(textBox2.Text), Convert.ToInt32(textBox3.Text));
            World.WorldArea.SetBorder(1);

            richTextBox1.Text = World.WorldArea.ToString();
        }

        private void Button2Click(object sender, EventArgs e)
        {
            VGArea currentArea = new VGArea(World,
                Convert.ToInt32(textBox21.Text), Convert.ToInt32(textBox22.Text),
                Convert.ToInt32(textBox23.Text), Convert.ToInt32(textBox24.Text));

            int coverage = Convert.ToInt32(textBox7.Text);
            int fc = Convert.ToInt32(textBox4.Text);
            int wc = Convert.ToInt32(textBox5.Text);
            int iterations = Convert.ToInt32(textBox6.Text);

            VGGCave cave = new VGGCave(0, 1, coverage, fc, wc, iterations);
            cave.Generate(currentArea);

            richTextBox1.Text = World.WorldArea.ToString();
        }

        private void Button3Click(object sender, EventArgs e)
        {
            VGArea currentArea = new VGArea(World,
                Convert.ToInt32(textBox21.Text), Convert.ToInt32(textBox22.Text),
                Convert.ToInt32(textBox23.Text), Convert.ToInt32(textBox24.Text));

            VGGBSPDungeon bsp = new VGGBSPDungeon(4, 3, 1, Convert.ToInt32(textBox8.Text), Convert.ToInt32(textBox11.Text), Convert.ToInt32(textBox9.Text),
                                                  checkBox1.Checked, checkBox4.Checked, Convert.ToInt32(textBox19.Text), Convert.ToInt32(textBox20.Text), checkBox5.Checked);
            bsp.Generate(currentArea);

            richTextBox1.Text = World.WorldArea.ToString();
        }

        private void NumericUpDown1ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown1.Value < 1) return;
            richTextBox1.Font = new Font("TerminalVector", (float) numericUpDown1.Value);
        }

        private void Button4Click(object sender, EventArgs e)
        {
            VGArea currentArea = new VGArea(World,
                Convert.ToInt32(textBox21.Text), Convert.ToInt32(textBox22.Text),
                Convert.ToInt32(textBox23.Text), Convert.ToInt32(textBox24.Text));

            VGGWalker walker = new VGGWalker(0, 1, Convert.ToInt32(textBox10.Text), Convert.ToInt32(textBox14.Text), Convert.ToInt32(textBox13.Text),
                                             Convert.ToInt32(textBox12.Text), Convert.ToInt32(textBox15.Text), checkBox2.Checked, checkBox3.Checked,
                                             Convert.ToInt32(textBox16.Text), Convert.ToInt32(textBox17.Text), Convert.ToInt32(textBox18.Text));
            walker.Generate(currentArea);

            richTextBox1.Text = World.WorldArea.ToString();
        }

        private void Button5Click(object sender, EventArgs e)
        {
            VGArea currentArea = new VGArea(World,
                Convert.ToInt32(textBox21.Text), Convert.ToInt32(textBox22.Text),
                Convert.ToInt32(textBox23.Text), Convert.ToInt32(textBox24.Text));

            VGGOutliner outliner = new VGGOutliner();
            outliner.Generate(currentArea);

            richTextBox1.Text = World.WorldArea.ToString();
        }

        private void Button6Click(object sender, EventArgs e)
        {
            VGArea currentArea = new VGArea(World,
                Convert.ToInt32(textBox21.Text), Convert.ToInt32(textBox22.Text),
                Convert.ToInt32(textBox23.Text), Convert.ToInt32(textBox24.Text));

            VGGCave cave = new VGGCave(mIterations: 3, mInitialSolidPercent: 75);
            VGGBSPDungeon bsp = new VGGBSPDungeon(mSplits: 9, mCarveOffset: 1);
            VGGOutliner outliner = new VGGOutliner();

            cave.Generate(currentArea);
            bsp.Generate(currentArea);
            outliner.Generate(currentArea);

            richTextBox1.Text = World.WorldArea.ToString();
        }

        private void textBox1_TextChanged(object sender, EventArgs e) { textBox23.Text = textBox1.Text; }

        private void textBox2_TextChanged(object sender, EventArgs e) { textBox24.Text = textBox2.Text; }

        private void textBox21_DoubleClick(object sender, EventArgs e)
        {            
            textBox21.Text = textBox22.Text = @"0"; textBox23.Text = textBox1.Text; textBox24.Text = textBox2.Text;
        }
    }
}