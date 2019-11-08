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
    public partial class RepairForm : Form
    {
        public string description;
        public string performer;
        public string data;

        public RepairForm()
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
                MessageBox.Show("Необходимо заполнить описание ремонтных работ устройства.", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            description = textBox1.Text.Replace(Convert.ToChar(13), ' ');
            performer = textBox2.Text;
            data = dateTimePicker1.Value.ToShortDateString();
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }
    }
}
