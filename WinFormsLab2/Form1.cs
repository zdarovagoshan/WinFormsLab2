using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace WinFormsLab2
{
    public partial class Form1 : Form
    {
        int x, y;
        bool isRed = true;
        public BindingSource data {
            get;
            set;
        }
        public Color color
        {
            get; set;
        }
        private SeriesChartType type = SeriesChartType.Line;
        private Dictionary<int, BindingSource> dataBindings;
        public Form1()
        {
            data = new BindingSource();
            dataBindings = new Dictionary<int, BindingSource>();
            data.Add(new Data() { X = 0, Y = 0 });
            data.Add(new Data() { X = 1, Y = 1 });
            data.Add(new Data() { X = 2, Y = 4 });
            data.ListChanged += new ListChangedEventHandler(DataValueChanged);
            dataBindings.Add(1, data);
            InitializeComponent();
            label1.Text = "Table 1";
            comboBox1.Items.Add(new ChartType() { index = 3, description = "Draw as lines" });
            comboBox1.Items.Add(new ChartType() { index = 4, description = "Draw as spline" });
            Random rnd = new Random();
            color = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
            comboBox2.Items.Add(new ChartColor() { index = 1, color = color });
            comboBox2.SelectedIndex = 0;
            dataGridView1.DataSource = data;
            ChangeChartData();
        }
        public void ChangeChartData()
        {
            chart1.DataSource = null;
            chart1.Series[0].ChartType = type;
            chart1.DataSource = data;
            chart1.Series[0].Color = color;
        }
        private void DrawAsLines_Click(object sender, EventArgs e)
        {
            type = SeriesChartType.Line;
            ChangeChartData();
        }
        private void DrawAsSpline_Click(object sender, EventArgs e)
        {
            type = SeriesChartType.Spline;
            ChangeChartData();
        }
        private void AddButton_Click(object sender, EventArgs e)
        {
            data.Add(new Data() { X = 4, Y = 16 });
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(Color.White);
            var pen = new Pen(color);
            int l = ClientRectangle.Left;
            int t = ClientRectangle.Top + menuStrip1.Height;
            int r = ClientRectangle.Right;
            int b = ClientRectangle.Bottom;
            e.Graphics.DrawLine(pen, l, t, r, b);
            e.Graphics.DrawLine(pen, r, t, l, b);
            e.Graphics.DrawString($"({x},{y})", DefaultFont,
            new SolidBrush(Color.Black), x, y);
        }
        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            Invalidate();
        }
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            x = e.X;
            y = e.Y;
            Invalidate();
        }
        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button.HasFlag(MouseButtons.Right))
            {
                isRed = !isRed;
                Invalidate();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            type = (SeriesChartType)(((ChartType)comboBox1.SelectedItem).index);
            ChangeChartData();
        }

        private void DataValueChanged(object sender, EventArgs e)
        {
            ChangeChartData();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            // получаем выбранный файл
            string filename = saveFileDialog1.FileName;
            // сохраняем текст в файл
            string Text = "";
            foreach (var a in data.List)
            {
                Text += a.ToString() + '\n';
            }
            File.WriteAllText(filename, Text);
            MessageBox.Show("Файл сохранен");
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            // получаем выбранный файл
            string filename = openFileDialog1.FileName;
            // читаем файл в строку
            string fileText = File.ReadAllText(filename);
            List<string> tmp = fileText.Split('\n').ToList(); tmp.RemoveAt(tmp.Count - 1);
            data = new BindingSource();
            tmp.ForEach(x => data.Add(new Data()
            {
                X = Convert.ToDouble(x.Split(' ')[0]),
                Y = Convert.ToDouble(x.Split(' ')[1])
            }));
            data.ListChanged += new ListChangedEventHandler(DataValueChanged);
            Random rnd = new Random();
            int lastKey = dataBindings.Keys.Last() + 1;
            color = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
            comboBox2.Items.Add(new ChartColor() { index = lastKey, color = color });
            dataBindings.Add(lastKey, data);
            dataGridView1.DataSource = data;
            comboBox2.SelectedIndex = dataBindings.Last().Key - 1;
            label1.Text = "Table " + comboBox2.SelectedItem.ToString();
            ChangeChartData();
            MessageBox.Show("Файл загружен");
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChartColor tmp = (ChartColor)comboBox2.SelectedItem;
            data = dataBindings[tmp.index];
            color = tmp.color;
            label1.Text = "Table " + comboBox2.SelectedItem.ToString();
            dataGridView1.DataSource = data;
            ChangeChartData();
        }

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var about = new About();
            about.ShowDialog();
        }
    }
    public class Data
    {
        public double X { get; set; }
        public double Y { get; set; }
        public override string ToString() { return $"{X} {Y}"; }
    }
    public class ChartType
    {
        public int index { get; set; }
        public string description { get; set; }
        public override string ToString()
        {
            return description;
        }
    }
    public class ChartColor
    {
        public int index { get; set; }
        public Color color { get; set; }
        public override string ToString()
        {
            return index.ToString();
        }
    }
}
