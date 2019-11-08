using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HardwareParts
{
    public partial class AddCaming : Form
    {
        public AddCaming()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 0)
            {
                MessageBox.Show("Необходимо указать номер накладной.", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string cmd = string.Format("INSERT INTO 'coming' ('invoice','date','worker') VALUES ('{0}','{1}','{2}');", textBox1.Text, dateTimePicker1.Value.ToLongDateString(), (comboBox1.SelectedItem as Item).id);
            SQL.ExecSQL(cmd);
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void AddCaming_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();

            // заполняем список работников
            SQLiteCommand command = new SQLiteCommand("SELECT * FROM 'workers';", SQL.Connection);
            foreach (DbDataRecord record in command.ExecuteReader())
            {
                Item item = new Item();
                item.id = record["id"].ToString();
                item.name = record["name"].ToString();
                comboBox1.Items.Add(item);
            }
            comboBox1.SelectedIndex = 0;
        }
    }
}
