using System;
using System.Data;
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
            
            for (int i=0; i < dataGridView1.ColumnCount; i++ )
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

            String searchValue = "11012";
            int rowIndex = -1;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["ProcessID"].Value != null) // Need to check for null if new row is exposed
                {
                    if (row.Cells["ProcessID"].Value.ToString().Equals(searchValue))
                    {
                        rowIndex = row.Index;
                        break;
                    }
                }
            }
             table.Rows.Add("1",dataGridView1.Rows[rowIndex].Cells[4].Value, dataGridView1.Rows[rowIndex].Cells[5].Value, dataGridView1.Rows[rowIndex].Cells[5].Value);
            ShowTable(table);
           /*
            label1.Text = rowIndex.ToString();

            dataGridView1.FirstDisplayedScrollingRowIndex = rowIndex;
            dataGridView1.CurrentCell = dataGridView1[0, rowIndex];
            dataGridView1.Focus();
            */


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
