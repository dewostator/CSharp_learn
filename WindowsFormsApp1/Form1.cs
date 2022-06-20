using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //         button1.Click += button1_Click;
            openFileDialog1.Filter = "CSV files(*.csv)|*.csv|All files(*.*)|*.*";
            dataGridView1.ColumnCount = 7;



        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }
            // получаем выбранный файл
            string filename = openFileDialog1.FileName;
            // читаем файл в строку
            string fileText = System.IO.File.ReadAllText(filename);
            var streamReader = File.OpenText(filename);
            string[] lines = System.IO.File.ReadAllLines(filename);
            {
                string line = lines[0];
                string s = Regex.Replace(line, "\"", string.Empty);
                string[] columns = s.Split(';');

                for (int i = 0; i < dataGridView1.ColumnCount; i++)
                    dataGridView1.Columns[i].Name = columns[i];
            }

            for (int i = 1; i < lines.GetLength(0); i++)
            {
                string line = lines[i];
                string s = Regex.Replace(line, "\"", string.Empty);
                string[] columns = s.Split(';');
                dataGridView1.Rows.Add(columns);
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {

            System.Data.DataTable table = new DataTable("ParentTable");

            table.Columns.Add("indent");
            table.Columns.Add("ParentProcessID");
            table.Columns.Add("ParentProcessName");
            table.Columns.Add("ParentProcessPath");

            String currentProcessID = textBox1.Text.ToString();
            String selectedPID = currentProcessID;
            String currentProcessName = null;
            String currentProcessPath = null;
            int rowIndex = -1;

            //Поиск первого вхождения процесса

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["ProcessID"].Value != null) // Need to check for null if new row is exposed
                {
                    if (row.Cells["ProcessID"].Value.ToString().Equals(currentProcessID))
                    {
                        rowIndex = row.Index;
                        break;
                    }
                }
            }

            table.Rows.Add(
                "1",
                dataGridView1.Rows[rowIndex].Cells[1].Value,
                dataGridView1.Rows[rowIndex].Cells[2].Value,
                dataGridView1.Rows[rowIndex].Cells[3].Value);

            ShowTable(table);

            table.Rows.Add(
                "1",
                dataGridView1.Rows[rowIndex].Cells[4].Value,
                dataGridView1.Rows[rowIndex].Cells[5].Value,
                dataGridView1.Rows[rowIndex].Cells[6].Value);


            currentProcessID = dataGridView1.Rows[rowIndex].Cells[4].Value.ToString();
            currentProcessPath = dataGridView1.Rows[rowIndex].Cells[5].Value.ToString();
            currentProcessName = dataGridView1.Rows[rowIndex].Cells[6].Value.ToString();
            ShowTable(table);

            

            Boolean found = true;
            int looplimit = 10;
            while (found)
            {
                found = false;
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {

                    if (row.Cells["ProcessID"].Value != null) // Need to check for null if new row is exposed
                    {
                        if (
                            row.Cells["ProcessID"].Value.ToString().Equals(currentProcessID) &&
                            row.Cells["ProcessName"].Value.ToString().Equals(currentProcessName) &&
                            row.Cells["ProcessPath"].Value.ToString().Equals(currentProcessPath)
                            )
                        {
                            found = true;

                            table.Rows.Add(
                                dataGridView1.Rows[row.Index].Cells[4].Value,
                                dataGridView1.Rows[row.Index].Cells[5].Value,
                                dataGridView1.Rows[row.Index].Cells[6].Value);


                            currentProcessID = dataGridView1.Rows[row.Index].Cells[4].Value.ToString();
                            currentProcessPath = dataGridView1.Rows[row.Index].Cells[5].Value.ToString();
                            currentProcessName = dataGridView1.Rows[row.Index].Cells[6].Value.ToString();
                            break;
                        }

                    }

                }

            }

            DataRow rootRow = table.Rows[table.Rows.Count - 1];
            

            treeView1.Nodes.Clear();


            LoadRoot(rootRow);

            treeView1.ExpandAll();

            SearchRecursive(treeView1.Nodes, selectedPID);
        }

        public void LoadRoot(DataRow row)
        {
            var pid = row;
            TreeNode tds = treeView1.Nodes.Add(pid[0].ToString()+"   " + pid[1].ToString()+ pid[2].ToString());
            FindClilds(row, tds);
        }

        private void FindClilds(DataRow rootrow, TreeNode td)
        {
            var dataTable = new DataTable("ParentTable");
            dataTable.Columns.Add("PID");
            dataTable.Columns.Add("ProcessName");
            dataTable.Columns.Add("ProcessPath");

            String currentProcessID = rootrow[0].ToString();
            String currentProcessPath = rootrow[1].ToString();
            String currentProcessName = rootrow[2].ToString();


            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["ProcessID"].Value != null) // Need to check for null if new row is exposed
                {

                    if (
                        row.Cells["ParentProcessID"].Value.ToString().Equals(currentProcessID) &&
                        row.Cells["ParentProcessName"].Value.ToString().Equals(currentProcessName) &&
                        row.Cells["ParentProcessPath"].Value.ToString().Equals(currentProcessPath)
                        )
                    {

                        dataTable.Rows.Add(
                             dataGridView1.Rows[row.Index].Cells[1].Value.ToString(),
                            dataGridView1.Rows[row.Index].Cells[2].Value.ToString(),
                            dataGridView1.Rows[row.Index].Cells[3].Value.ToString());

                    }

                }
            }

            foreach (DataRow row in dataTable.Rows)
            {
                var pid = row;
                TreeNode tds = td.Nodes.Add(pid[0].ToString() + "   " + pid[1].ToString() + pid[2].ToString());
                FindClilds(row, tds);
            }
        }


        private bool SearchRecursive(IEnumerable nodes, string searchFor)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Text.ToUpper().Contains(searchFor))
                {
                    treeView1.SelectedNode = node;
                    node.BackColor = Color.Yellow;
                }
                if (SearchRecursive(node.Nodes, searchFor))
                    return true;
            }
            return false;
        }

            private static void ShowTable(DataTable table)
        {
            foreach (DataColumn col in table.Columns)
            {
                Console.Write("{0,-14}", col.ColumnName);
            }
            Console.WriteLine();

            foreach (DataRow row in table.Rows)
            {
                foreach (DataColumn col in table.Columns)
                {
                    if (col.DataType.Equals(typeof(DateTime)))
                        Console.Write("{0,-14:d}", row[col]);
                    else if (col.DataType.Equals(typeof(Decimal)))
                        Console.Write("{0,-14:C}", row[col]);
                    else
                        Console.Write("{0,-14}", row[col]);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }
}
