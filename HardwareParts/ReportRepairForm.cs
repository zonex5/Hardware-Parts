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
    public partial class ReportRepairForm : Form
    {
        public ReportRepairForm()
        {
            InitializeComponent();
        }

        private void LoadParts()
        {
            combo1.Items.Clear();
            combo1.Items.Add(new Item() { id = "", name = "-- Все --" });

            SQLiteCommand command = new SQLiteCommand("SELECT * FROM 'parts';", SQL.Connection);
            SQLiteDataReader reader = command.ExecuteReader();

            foreach (DbDataRecord record in reader)
            {
                Item item = new Item();
                item.id = record["id"].ToString();
                item.name = record["name"].ToString();
                combo1.Items.Add(item);
            }
            combo1.SelectedIndex = 0;
        }

        private void ReportRepairForm_Load(object sender, EventArgs e)
        {
            LoadParts();
            LoadRepairs();
        }

        private void LoadRepairs()
        {
            grid.Rows.Clear();
            string cmd1 = "SELECT [placeparts].[name], [repair].[description], [repair].[data], [repair].[performer] FROM [repair] INNER JOIN [placeparts] ON [repair].[placepart] = [placeparts].[id];";
            string cmd2 = "SELECT [placeparts].[name], [repair].[description], [repair].[data], [repair].[performer] FROM [repair] INNER JOIN [placeparts] ON [repair].[placepart] = [placeparts].[id] WHERE placeparts.part=" + (combo1.SelectedItem as Item).id + ";";
            string cmd3 = "SELECT [placeparts].[name], [repair].[description], [repair].[data], [repair].[performer] FROM [repair] INNER JOIN [placeparts] ON [repair].[placepart] = [placeparts].[id] WHERE ([placeparts].[name] LIKE '%" + filter.Text + "%') OR ([repair].[description] LIKE '%" + filter.Text + "%') OR ([repair].[performer] LIKE '%" + filter.Text + "%');";

            string cmd = combo1.SelectedIndex > 0 ? cmd2 : filter.Text.Length > 0 ? cmd3 : cmd1;
            SQLiteCommand command = new SQLiteCommand(cmd, SQL.Connection);
            foreach (DbDataRecord record in command.ExecuteReader())
            {
                grid.Rows.Add();
                DataGridViewRow row = grid.Rows[grid.Rows.Count - 1];
                row.Cells[0].Value = grid.Rows.Count.ToString();
                row.Cells[1].Value = record["name"].ToString();
                row.Cells[2].Value = record["description"].ToString();
                row.Cells[3].Value = record["data"].ToString();
                row.Cells[4].Value = record["performer"].ToString();
            }
            count.Text = grid.Rows.Count.ToString();
        }

        private void сomboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadRepairs();
        }

        private void filter_TextChanged(object sender, EventArgs e)
        {
            if (filter.Text.Length > 0)
            {
                combo1.SelectedIndex = 0;
            }
            LoadRepairs();
        }

        DataGridViewPrinter MyDataGridViewPrinter;
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (printer.ShowDialog() != DialogResult.OK) return;

            PrintDocument MyPrintDocument = new PrintDocument();
            MyPrintDocument.DocumentName = "Проделанные ремонтные работы";
            MyPrintDocument.PrinterSettings = printer.PrinterSettings;
            MyPrintDocument.DefaultPageSettings = printer.PrinterSettings.DefaultPageSettings;
            MyPrintDocument.DefaultPageSettings.Margins = new Margins(40, 40, 40, 40);
            MyPrintDocument.PrintPage += MyPrintDocument_PrintPage;

            MyDataGridViewPrinter = new DataGridViewPrinter(grid, MyPrintDocument, true, true, "Проделанные ремонтные работы", new Font("Tahoma", 12, FontStyle.Bold, GraphicsUnit.Point), Color.Black, true);
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
