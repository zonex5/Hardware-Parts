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
    public partial class ReportSoftForm : Form
    {
        public ReportSoftForm()
        {
            InitializeComponent();
        }

        private void LoadSoft()
        {
            grid.Rows.Clear();
            string cmd1 = "SELECT [workplaces].[name] as wname, [soft].[name] as sname, [soft].[license], [soft].[key], [soft].[installdate] FROM [soft] INNER JOIN [workplaces] ON [soft].[workplace] = [workplaces].[id];";
            string cmd2 = "SELECT [workplaces].[name] as wname, [soft].[name] as sname, [soft].[license], [soft].[key], [soft].[installdate] FROM [soft] INNER JOIN [workplaces] ON [soft].[workplace] = [workplaces].[id] WHERE (wname LIKE '%" + filter.Text + "%') OR (sname LIKE '%" + filter.Text + "%') OR (installdate LIKE '%" + filter.Text + "%');";
            
            string cmd = filter.Text.Length > 0 ? cmd2 : cmd1;
            SQLiteCommand command = new SQLiteCommand(cmd, SQL.Connection);
            foreach (DbDataRecord record in command.ExecuteReader())
            {
                grid.Rows.Add();
                DataGridViewRow row = grid.Rows[grid.Rows.Count - 1];
                row.Cells[0].Value = grid.Rows.Count.ToString();
                row.Cells[1].Value = record["sname"].ToString();
                row.Cells[2].Value = record["wname"].ToString();
                row.Cells[3].Value = record["license"].ToString();
                row.Cells[4].Value = record["key"].ToString();
                row.Cells[5].Value = record["installdate"].ToString();
            }
            count.Text = grid.Rows.Count.ToString();
        }

        private void ReportSoftForm_Load(object sender, EventArgs e)
        {
            LoadSoft();
        }

        private void filter_TextChanged(object sender, EventArgs e)
        {
            LoadSoft();
        }

        DataGridViewPrinter MyDataGridViewPrinter;
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (printer.ShowDialog() != DialogResult.OK) return;

            PrintDocument MyPrintDocument = new PrintDocument();
            MyPrintDocument.DocumentName = "Програмное обеспечение";
            MyPrintDocument.PrinterSettings = printer.PrinterSettings;
            MyPrintDocument.DefaultPageSettings = printer.PrinterSettings.DefaultPageSettings;
            MyPrintDocument.DefaultPageSettings.Margins = new Margins(40, 40, 40, 40);
            MyPrintDocument.PrintPage += MyPrintDocument_PrintPage;

            MyDataGridViewPrinter = new DataGridViewPrinter(grid, MyPrintDocument, true, true, "Програмное обеспечение", new Font("Tahoma", 12, FontStyle.Bold, GraphicsUnit.Point), Color.Black, true);
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
