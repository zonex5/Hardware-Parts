using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HardwareParts
{
    public partial class EditorForm : Form
    {
        public string Value { get; set; }

        public EditorForm(string caption, string value = "")
        {
            InitializeComponent();
            Text = caption;
            textBox1.Text = value;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 0)
                MessageBox.Show("Поле ввода не может быть пустым.", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            else
            {
                Value = textBox1.Text;
                DialogResult = System.Windows.Forms.DialogResult.OK;
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                button1_Click(null, null);
        }
    }
}
