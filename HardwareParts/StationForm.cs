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
    public partial class StationForm : Form
    {
        public string StationName { get; set; }
        public string DomainName { get; set; }
        public string IP { get; set; }

        public string ID { get; set; }

        public StationForm()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox2.Text.Length == 0)
                MessageBox.Show("Не задано доменное имя компьютера.", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            else
                try
                {
                    textBox3.Text = System.Net.Dns.GetHostEntry(textBox2.Text).AddressList[0].ToString();
                }
                catch
                {
                    MessageBox.Show("Не удаётся найти указанный узел.", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 0)
            {
                MessageBox.Show("Название рабочей станции не может быть пустым.", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                StationName = textBox1.Text;
                DomainName = textBox2.Text;
                IP = textBox3.Text;
                if (comboBox1.SelectedItem != null)
                    ID = (comboBox1.SelectedItem as Item).id;
                else ID = string.Empty;
                DialogResult = System.Windows.Forms.DialogResult.OK;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void StationForm_Load(object sender, EventArgs e)
        {
            // заполняем список с отделами
            SQLiteCommand command = new SQLiteCommand("SELECT * FROM 'workers';", SQL.Connection);
            foreach (DbDataRecord record in command.ExecuteReader())
            {
                string id = record["id"].ToString();
                string name = record["name"].ToString();
                Item item = new Item();
                item.id = id;
                item.name = name;
                comboBox1.Items.Add(item);
            }
        }
    }
}
