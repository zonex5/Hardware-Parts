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
    public partial class StockForm : Form
    {
        public StockForm()
        {
            InitializeComponent();
        }

        private void LoadParts()
        {
            grid1.Rows.Clear();
            SQLiteCommand command = new SQLiteCommand("SELECT * FROM 'parts';", SQL.Connection);
            SQLiteDataReader reader = command.ExecuteReader();

            foreach (DbDataRecord record in reader)
            {
                string id = record["id"].ToString();
                string name = record["name"].ToString();

                grid1.Rows.Add(1);
                int index = grid1.Rows.Count - 1;
                grid1.Rows[index].Cells["n"].Value = (index + 1).ToString();
                grid1.Rows[index].Cells["name"].Value = name;
                grid1.Rows[index].Cells["id"].Value = id;
            }
        }

        private void LoadDevices()
        {
            grid2.Rows.Clear();
            SQLiteCommand command = new SQLiteCommand("SELECT id, name, number, installdate FROM 'placeparts' WHERE part= " + grid1.SelectedRows[0].Cells[2].Value.ToString() + ";", SQL.Connection);
            foreach (DbDataRecord record in command.ExecuteReader())
            {
                grid2.Rows.Add();
                DataGridViewRow row = grid2.Rows[grid2.Rows.Count - 1];
                row.Cells[0].Value = grid2.Rows.Count.ToString();
                row.Cells[1].Value = record["name"].ToString();
                row.Cells[2].Value = record["number"].ToString();
                row.Cells[3].Value = record["installdate"].ToString();
                row.Cells[4].Value = record["id"].ToString();
            }
            status1.Text = grid2.Rows.Count.ToString();
        }

        private void LoadRepairs()
        {
            grid3.Rows.Clear();
            SQLiteCommand command = new SQLiteCommand("SELECT * FROM 'repair' WHERE placepart=" + grid2.SelectedRows[0].Cells[4].Value.ToString() + ";", SQL.Connection);
            foreach (DbDataRecord record in command.ExecuteReader())
            {
                grid3.Rows.Add();
                DataGridViewRow row = grid3.Rows[grid3.Rows.Count - 1];
                row.Cells[0].Value = grid3.Rows.Count.ToString();
                row.Cells[1].Value = record["description"].ToString();
                row.Cells[2].Value = record["data"].ToString();
                row.Cells[3].Value = record["performer"].ToString();
            }
            status2.Text = grid3.Rows.Count.ToString();
        }

        private void StockForm_Load(object sender, EventArgs e)
        {
            LoadParts();

            if (grid1.SelectedRows.Count > 0)
            {
                LoadDevices();
                if (grid2.SelectedRows.Count > 0)
                    LoadRepairs();
            }
        }

        private void grid1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            grid2.Rows.Clear();
            grid3.Rows.Clear();
            LoadDevices();
            if (grid2.SelectedRows.Count > 0)
                LoadRepairs();
        }

        private void grid2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            grid3.Rows.Clear();
            LoadRepairs();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            Parts form = new Parts();
            form.ShowDialog();
        }
    }
}
