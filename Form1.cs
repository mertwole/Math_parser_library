using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Math_parser_library
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            Parser_lib.Parameters.AddVariable("a", 10);
            
            Parser_lib.Parameters.AddArray("b", new float[] { 10, 0, 9 });

            Parser_lib.Parameters.AddFunction("Get10", new Parser_lib.Parameters.FunctionCode((x) => 10));
            Parser_lib.Parameters.AddFunction("TripleSumm", new Parser_lib.Parameters.FunctionCode((x) => x[0] + x[1] + x[2]));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                label1.Text = Parser_lib.Parser.Parse(textBox1.Text).ToString();
            }
            catch
            {
                label1.Text = "failed";
            }
        }
    }
}
