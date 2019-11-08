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
    public partial class DeviceForm : Form
    {
        string place;
        public DeviceForm(string place)
        {
            this.place = place;
            InitializeComponent();
        }

        private void DeviceForm_Load(object sender, EventArgs e)
        {
            SQLiteCommand command = new SQLiteCommand("SELECT * FROM 'parts';", SQL.Connection);
            foreach (DbDataRecord record in command.ExecuteReader())
            {
                Item item = new Item();
                item.id = record["id"].ToString();
                item.name = record["name"].ToString();
                comboBox1.Items.Add(item);
                if (comboBox1.Items.Count > 0)
                    comboBox1.SelectedIndex = 0;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex < 0 || textBox1.Text.Length == 0 || textBox2.Text.Length == 0)
            {
                MessageBox.Show("Необходимо заполнить все поля.", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            string cmd = string.Format("INSERT INTO 'placeparts' ('workplace','part','name','number','installdate') VALUES ('{0}','{1}','{2}','{3}','{4}');", place, (comboBox1.SelectedItem as Item).id, textBox1.Text, textBox2.Text, dateTimePicker1.Value.ToShortDateString());
            SQLiteCommand command = new SQLiteCommand(cmd, SQL.Connection);
            command.ExecuteNonQuery();
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
    }
}
