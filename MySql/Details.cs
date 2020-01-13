using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;
using System.Threading;
using Bunifu.Framework.UI;

namespace MySql
{
    public partial class Details : Form
    {
        String BaseWay, request;
        String path = @"C:\Users\WarLock\Downloads/default-book.png";
        String sum = "", old_amount="", old_date="";
        private int id, index, id_book, TableId = -1;

        public Details()
        {
            String parol = "warlock";
            InitializeComponent();
            BaseWay = @"server=127.0.0.1; port = " + 44999 + ";userid=root;password=" + parol + "; database=world;";        
        }

        private MySqlConnection OpenConnection() {
            MySqlConnection conn;
            conn = new MySqlConnection
            {
                ConnectionString = BaseWay
            };
            if (conn.State != ConnectionState.Open)
                conn.Open();

            return conn;
        }
        private MySqlCommand ExecuteConnection(MySqlConnection conn, String query)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand
                {
                    Connection = conn,
                    CommandText = query
                };

                cmd.Prepare();
                return cmd;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return new MySqlCommand();
            }
        }
        private DataView LoadDataSet(MySqlConnection conn, String query) {
            DataSet dataSet = new DataSet();

            MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
            adapter.Fill(dataSet);

            return dataSet.Tables[0].DefaultView;
        }

        private void getValues(int WhichTable) {
            switch (WhichTable) {
                case 1: {
                        getValuesTable(kirim_grid, 1);
                        break;
                    }
                case 2: {
                        getValuesTable(chiqim_grid, 2);
                        break;
                    }
            }
        }        
        private void getValuesTable(BunifuCustomDataGrid table, int value)
        {
            id = int.Parse(table.CurrentRow.Cells[0].Value.ToString());
            if (table.CurrentRow != null)
            {
                int until = table.Rows.Count;
                if (value == 1)
                {
                    for (int y = 0; y < until; y++)
                    {
                        kirim_grid.CurrentRow.Cells[y].Selected = true;
                        chiqim_grid.CurrentRow.Cells[y].Selected = false;

                    }
                }
                else
                {
                    for (int y = 0; y < 4; y++)
                    {
                        chiqim_grid.CurrentRow.Cells[y].Selected = true;
                        kirim_grid.CurrentRow.Cells[y].Selected = false;
                    }
                }
            }
            
                TableId = value;
                coming_amount.Text = table.CurrentRow.Cells[2].Value.ToString();
                приход_date.Value = DateTime.Parse(table.CurrentRow.Cells[3].Value.ToString());
            }
        
        private String getTableName()
        {
            String TableName;
            MessageBox.Show("ID?: Tabele" + TableId);
            switch (TableId)
            {
                case 1: { TableName = "kirim"; break; }
                case 2: { TableName = "chiqim"; break; }
                default: { TableName = null; break; }
            }
            return TableName;
        }
        private void searchTable(BunifuCustomDataGrid table)
        {
            BindingSource bs = new BindingSource();
            bs.DataSource = table.DataSource;
            //bs.Filter = "sana like '%" + tv_search.Text + "%'";
            table.DataSource = bs;
        }
        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                MessageBox.Show("Exception Occured while releasing object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }
        private void InitializeBackgroundWorker()
        {
            backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);
            backgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker1_RunWorkerCompleted);
            backgroundWorker1.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker1_ProgressChanged);
        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progress_bar.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
                MessageBox.Show(e.Error.Message);
            else if (e.Cancelled)
                MessageBox.Show("Canceled");
            else
                progress_bar.Visible = false;
        }

        private void bunifuButton6_Click(object sender, EventArgs e)
        {

        }

        private void edit_Click(object sender, EventArgs e)
        {
            MySqlConnection conn = OpenConnection();
            if (TableId != -1)
            {
                String TableName = getTableName();
                if (TableName == null) return;
                String query = "Update " + TableName + " set soni=@soni, sana=@sana where id = " + id;
                MySqlCommand cmd = ExecuteConnection(conn, query);

                cmd.Parameters.AddWithValue("@soni", coming_amount.Text.ToString());
                cmd.Parameters.AddWithValue("@sana", приход_date.Value.ToShortDateString());

                cmd.ExecuteNonQuery();
                conn.Close();
            }
            MessageBox.Show("O'zgaritirldi!");
            SelectAndSet1();
            SelectAndSet2();
        }

        private void bunifuButton5_Click(object sender, EventArgs e)
        {
                progress_bar.Visible = true;
                progress_bar.Value = 0;

                MySqlConnection conn = OpenConnection();
                string data = null;

                Excel.Application xlApp;
                Excel.Workbook xlWorkBook;
                Excel.Worksheet xlWorkSheet;
                object misValue = System.Reflection.Missing.Value;

                xlApp = new Excel.Application();
                xlWorkBook = xlApp.Workbooks.Add(misValue);
                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

            String query = "select k.soni ,k.sana from kirim k where k.id_book= " + id_book;
            String query2 = "select k.soni ,k.sana from chiqim k where k.id_book= " + id_book;
            DataSet ds = new DataSet();

            MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
            adapter.Fill(ds);

            DataSet ds2 = new DataSet();
            MySqlDataAdapter adapter2 = new MySqlDataAdapter(query2, conn);
            adapter2.Fill(ds2);

            string[] names = { "Названия", "Приход", "Приход date", "Расход", "Расход date", "Ostatka", "Summa"};
                for (int w = 0; w < names.Length; w++)
                {
                    xlWorkSheet.Cells[1, w + 1] = names[w];
                }

                string today = DateTime.Now.ToString("MM/DD");
                Random rnd = new Random();
                int month = rnd.Next(1, 130);
                string name = today + month.ToString();
                int length = ds.Tables[0].Rows.Count;

            progress_bar.Value = 50;
            for (int i = 0; i < length; i++)
                {
                    for (int j = 0; j <= ds.Tables[0].Columns.Count - 1; j++)
                    {
                        data = ds.Tables[0].Rows[i].ItemArray[j].ToString();
                        xlWorkSheet.Cells[i + 2, j + 1] = data;

                    }
                    backgroundWorker1.ReportProgress(i, i);
                }
            progress_bar.Value = 70;

            backgroundWorker1.RunWorkerAsync(100);
                xlWorkBook.SaveAs(name + ".xls", Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                xlWorkBook.Close(true, misValue, misValue);
                xlApp.Quit();
                MessageBox.Show(name);
                releaseObject(xlWorkSheet);
                releaseObject(xlWorkBook);
                releaseObject(xlApp);
            progress_bar.Visible = false;
        }

        private void bunifuButton1_Click(object sender, EventArgs e)
        {

        }

        private void kirim_grid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            getValues(1);
        }

        private void chiqim_grid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            getValues(2);
        }

        private void bunifuCustomLabel1_Click(object sender, EventArgs e)
        {

        }

        private void Details_FormClosing(object sender, FormClosingEventArgs e)
        {
            MainForm m =new MainForm();
            m.Show();
        }

        private void btn_delete_Click(object sender, EventArgs e)
        {
            if (TableId != -1)
            {
                MySqlConnection conn = OpenConnection();

                String TableName = getTableName();
                if (TableName == null) return;

                String query = "Delete from " + TableName + " where id = " + id;
                ExecuteConnection(conn, query).ExecuteNonQuery();
                conn.Close();


                SelectAndSet1();
                SelectAndSet2();
            }
        }

        private void bunifuButton2_Click(object sender, EventArgs e)
        {
            MySqlConnection conn = OpenConnection();
            String query = "Update books set Nomi=@nomi where id = " + id_book;
            MySqlCommand cmd = ExecuteConnection(conn, query);

            cmd.Parameters.AddWithValue("@nomi", bunifuTextBox1.Text.ToString());

            cmd.ExecuteNonQuery();
            MessageBox.Show("O'zgaritirldi!");
            conn.Close();

        }

        private void tv_search_KeyDown(object sender, KeyEventArgs e)
        {
            searchTable(kirim_grid);
            searchTable(chiqim_grid);
        }


        private void Details_Load(object sender, EventArgs e)
        {
            bunifuTextBox1.Text = MainForm.oldName;
            id_book = MainForm.id;
            SelectAndSet1();
            SelectAndSet2();
            int until = kirim_grid.Rows.Count;
            int until2 = chiqim_grid.Rows.Count;

            for (int y = 0; y < until; y++)
            {
                kirim_grid.CurrentRow.Cells[y].Selected = false;
            }
            for (int y = 0; y < until2; y++)
            {
                chiqim_grid.CurrentRow.Cells[y].Selected = false;
            }
        }
        
        //------------------------------------

        private void SelectAndSet1()
        {
                MySqlConnection conn = OpenConnection();
                String query = "select k.id, k.id_book, k.soni ,k.sana from kirim k where k.id_book= "+ id_book;
            
                kirim_grid.DataSource = LoadDataSet(conn, query);

               // kirim_grid.Rows[0].Visible = true;
                kirim_grid.Columns["id_book"].Visible = false;
                
                conn.Close();
        }

        private void SelectAndSet2()
        {
                MySqlConnection conn = OpenConnection();
                String query= "select k.id, k.id_book, k.soni ,k.sana from chiqim k where k.id_book= "+ id_book;

                chiqim_grid.DataSource = LoadDataSet(conn, query);

                //chiqim_grid.Rows[0].Visible = true;
                chiqim_grid.Columns["id_book"].Visible = false;
                
                conn.Close();
        }
        
    }
}
