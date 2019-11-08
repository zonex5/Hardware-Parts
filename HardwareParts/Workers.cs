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
    public partial class Workers : Form
    {
        public Workers()
        {
            InitializeComponent();
        }

        private void Workers_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            grid.Rows.Clear();
            SQLiteCommand command = new SQLiteCommand("SELECT * FROM 'workers';", SQL.Connection);
            SQLiteDataReader reader = command.ExecuteReader();

            foreach (DbDataRecord record in reader)
            {
                string id = record["id"].ToString();
                string name = record["name"].ToString();

                grid.Rows.Add(1);
                int index = grid.Rows.Count - 1;
                grid.Rows[index].Cells["n"].Value = (index + 1).ToString();
                grid.Rows[index].Cells["name"].Value = name;
                grid.Rows[index].Cells["id"].Value = id;
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Удалить сотрудника из списка?", "Внимание!", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                if (grid.SelectedRows.Count >= 0)
                {
                    SQL.ExecSQL("DELETE FROM 'workers' WHERE id=" + grid.SelectedRows[0].Cells["id"].Value.ToString());
                    LoadData();
                }
            }
        }       

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            EditorForm form = new EditorForm("Новый сотрудник");
            if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // добавляем запись
                SQL.ExecSQL("INSERT INTO 'workers' ('name') VALUES ('" + form.Value + "');");
                LoadData();
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (grid.SelectedRows.Count >= 0)
            {
                EditorForm form = new EditorForm("Редактировать данные сотрудника", grid.SelectedRows[0].Cells["name"].Value.ToString());
                if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    // изменить запись
                    SQL.ExecSQL("DELETE FROM 'workers' WHERE id=" + grid.SelectedRows[0].Cells["id"].Value.ToString() + "; INSERT INTO 'workers' ('id', 'name') VALUES ('" + grid.SelectedRows[0].Cells["id"].Value.ToString() + "','" + form.Value + "');");
                    LoadData();
                }
            }
        }
    }
}
