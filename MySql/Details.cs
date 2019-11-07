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

namespace MySql
{
    public partial class Details : Form
    {
        String BaseWay, request;
        String parol = "warlock";
        String path = @"C:\Users\WarLock\Downloads/default-book.png";
        String sum = "";
        int id, index;

        public Details()
        {
            InitializeComponent();
            BaseWay = @"server=127.0.0.1; port = " + 44999 + ";userid=root;password=" + parol + "; database=world;";
            
        }

        private void bunifuButton6_Click(object sender, EventArgs e)
        {

        }

        private void edit_Click(object sender, EventArgs e)
        {

        }

        private void bunifuButton5_Click(object sender, EventArgs e)
        {

        }

        private void bunifuButton1_Click(object sender, EventArgs e)
        {

        }


        private void SelectAndSet()
        {
            TaskScheduler ui = TaskScheduler.FromCurrentSynchronizationContext();
            Task.Factory.StartNew(() =>
            {

                MySqlConnection conn;
                conn = new MySqlConnection();
                conn.ConnectionString = BaseWay;

                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                return conn;
            }).ContinueWith((task =>
            {
                MySqlConnection conn = task.Result;
                request = "select s.id, s.image, s.nomi , b.id, b.soni, b.sana, c.id, c.soni, c.sana from world.books s, world.kirim b, world.chiqim c where b.id_book = s.id and s.id = c.id_book;";
                DataSet dataSet = new DataSet();

                MySqlDataAdapter adapter = new MySqlDataAdapter(request, conn);
                adapter.Fill(dataSet);

                books_datagrid.DataSource = dataSet.Tables[0].DefaultView;

                if (books_datagrid.RowCount > 2)
                {
                    books_datagrid.Rows[0].Visible = true;
                    //books_datagrid.Columns["id_book"].Visible = false;
                }

                conn.Close();
            }), ui);
        }

        private void Details_Load(object sender, EventArgs e)
        {
            SelectAndSet();
        }
    }
}
