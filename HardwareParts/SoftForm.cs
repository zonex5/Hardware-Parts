using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HardwareParts
{
    public partial class SoftForm : Form
    {
        public string nameSoft;
        public string key;
        public string license;
        public string date = DateTime.Now.ToShortDateString();

        public SoftForm()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 0)
            {
                MessageBox.Show("Необходимо заполнить название программного обеспечения.", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            nameSoft = textBox1.Text;
            key = textBox2.Text;
            license = textBox3.Text;
            date = dateTimePicker1.Value.ToShortDateString();
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void SoftForm_Load(object sender, EventArgs e)
        {
            textBox1.Text = nameSoft;
            textBox2.Text = key;
            textBox3.Text = license;
            dateTimePicker1.Value = Convert.ToDateTime(date);
        }
    }
}
