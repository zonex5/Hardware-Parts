using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HardwareParts
{
    public partial class ComingForm : Form
    {
        public ComingForm()
        {
            InitializeComponent();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            AddCaming form = new AddCaming();
            if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LoadData();
            }
        }

        void LoadData()
        {
            grid.Rows.Clear();
            // заполняем список с накладными
            SQLiteCommand command = new SQLiteCommand("SELECT coming.id as id, workers.name as name, coming.invoice as invoice, coming.date as dta FROM 'coming', 'workers' WHERE coming.worker=workers.id;", SQL.Connection);
            foreach (DbDataRecord record in command.ExecuteReader())
            {
                grid.Rows.Add();
                DataGridViewRow row = grid.Rows[grid.Rows.Count - 1];
                row.Cells[0].Value = grid.Rows.Count;
                row.Cells[1].Value = record["invoice"].ToString();
                row.Cells[2].Value = record["dta"].ToString();
                row.Cells[3].Value = record["name"].ToString();
                row.Cells[4].Value = record["id"].ToString();
            }
        }

        private void ComingForm_Load(object sender, EventArgs e)
        {
            LoadData();
        }
        
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (grid.SelectedRows.Count > 0)
            {
                SQL.ExecSQL("DELETE FROM 'coming' WHERE id='" + grid.SelectedRows[0].Cells[4].Value.ToString() + "';");
                LoadData();
            }
        }

        DataGridViewPrinter MyDataGridViewPrinter;
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (printer.ShowDialog() != DialogResult.OK) return;

            PrintDocument MyPrintDocument = new PrintDocument();
            MyPrintDocument.DocumentName = "Приходы техники";
            MyPrintDocument.PrinterSettings = printer.PrinterSettings;
            MyPrintDocument.DefaultPageSettings = printer.PrinterSettings.DefaultPageSettings;
            MyPrintDocument.DefaultPageSettings.Margins = new Margins(40, 40, 40, 40);
            MyPrintDocument.PrintPage += MyPrintDocument_PrintPage;

            MyDataGridViewPrinter = new DataGridViewPrinter(grid, MyPrintDocument, false, true, "Приходы техники", new Font("Tahoma", 12, FontStyle.Bold, GraphicsUnit.Point), Color.Black, true);
            MyPrintDocument.Print();
        }

        private void MyPrintDocument_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            bool more = MyDataGridViewPrinter.DrawDataGridView(e.Graphics);
            if (more == true)
                e.HasMorePages = true;
        }
    }
}
