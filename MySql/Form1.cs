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
using System.Globalization;

namespace MySql
{

    public partial class MainForm : Form
    {

        public static int id = -1;
        public static String oldName = "";
        private String BaseWay, request, path = @"C:\Users\WarLock\Downloads/default-book.png",
        selectAndSet = "select *, k.Narxi*k.qoldiq as summa from (" +
            "SELECT b.id, b.image, b.Nomi, (SELECT SUM(k.soni) FROM kirim k WHERE k.id_book = b.id) AS Kirim,b.Kirim as Hozir_Kirim, b.Narxi,b.Kirim_sana, (SELECT SUM(c.soni) FROM "
                + " chiqim c WHERE b.id = c.id_book) AS Chiqim, b.chiqim as HozirChiqim ,b.Chiqim_sana, (SELECT SUM(k.soni) FROM kirim k WHERE k.id_book = b.id) -(SELECT "
                + " SUM(c.soni) FROM chiqim c WHERE b.id = c.id_book) qoldiq FROM books b) as k;",
        sum = "";
        byte[] oldImage = null;
        private int index;
        private Boolean clicked = false;

        public MainForm()
        {
            InitializeComponent();
            InitializeBackgroundWorker();
            String parol = "warlock";
            BaseWay = @"server=127.0.0.1; port = " + 44999 + ";userid=root;password=" + parol + "; database=world;";
        }
        static public void UpdateAllMain(){

        }
        private void InitializeBackgroundWorker()
        {
            backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);
            backgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker1_RunWorkerCompleted);
            backgroundWorker1.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker1_ProgressChanged);
        }  

        private void bunifuCustomDataGrid1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            index = e.RowIndex;
            getCellsValues();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            NewFile();
            SelectAndSet();
            getCellsValues();
        }

        private void remove_Click(object sender, EventArgs e)
        {
              DialogResult dialogResult = MessageBox.Show("O`chirish?", "DIQQAT!!!", MessageBoxButtons.YesNo);
              if (dialogResult == DialogResult.Yes)
              {
                if (id == -1)
                    MessageBox.Show("#ID NULL");
                else
                {
                    Delete();
                    NewFile();
                }
              }
        }

        private void image_book_Click(object sender, EventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Filter = "Choose Image(*.jpg; *.png) | *.jpg;*.png";
            if (op.ShowDialog() == DialogResult.OK)
            {
                Image image = ResizeImage(Image.FromFile(op.FileName), 150, 150);
                image_book.Image = image;
                save(image);
                clicked = true;
            }
        }
 
        private void bunifuButton1_Click_1(object sender, EventArgs e)
        {
            System.Environment.Exit(1);
        }
              
        private void bunifuButton2_Click(object sender, EventArgs e)
        {
                NewFile();
                name_book.Focus();
        }

        private void bunifuButton3_Click(object sender, EventArgs e)
        {
            String query = "SELECT b.id, b.image, b.Nomi, (SELECT SUM(k.soni) FROM kirim k WHERE k.id_book = b.id) AS Kirim, b.Narxi,b.Kirim_sana, (SELECT SUM(c.soni) FROM "
                + " chiqim c WHERE b.id = c.id_book) AS Chiqim,b.Chiqim_sana, (SELECT SUM(k.soni) FROM kirim k WHERE k.id_book = b.id) -(SELECT "
                + " SUM(c.soni) FROM chiqim c WHERE b.id = c.id_book) qoldiq FROM books b; ";

            TaskScheduler ui = TaskScheduler.FromCurrentSynchronizationContext();
            Task.Factory.StartNew(() =>
            {
                return OpenConnection();
            }).ContinueWith((task =>
            {
                MySqlConnection conn = task.Result;
                DataSet dataSet = new DataSet();

                MySqlDataAdapter adapter = new MySqlDataAdapter(selectAndSet, conn);
                adapter.Fill(dataSet);

                books_datagrid.DataSource = dataSet.Tables[0].DefaultView;

                if (books_datagrid.RowCount >= 1)
                {
                    books_datagrid.Rows[0].Visible = true;
                    books_datagrid.Columns["image"].Visible = false;
                }

                conn.Close();
            }), ui);
            MessageBox.Show("Refreshed");
        }

        private void bunifuButton4_Click(object sender, EventArgs e)
        {
            search_from_table();
        }

        private void bunifuButton6_Click(object sender, EventArgs e)
        {
            /*DialogResult dialogResult = MessageBox.Show("Rostan ham ma'lumotlarni tozalamoqchimisz?", "DIQQAT!!!", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
             MySqlConnection conn = OpenConnection();
             String query = "truncate books;";
             ExecuteConnection(conn, query).ExecuteNonQuery();
             if (conn.State != ConnectionState.Closed)
               {
                conn.Close();
               }
                SelectAndSet();
            }*/
        }

        private void bunifuButton5_Click(object sender, EventArgs e)
        {
            progress_bar.Visible = true;
            progress_bar.Value = 0;
            
            MySqlConnection conn = OpenConnection();
            string data = null;
            
            Excel.Application xlApp ;
            Excel.Workbook xlWorkBook ;
            Excel.Worksheet xlWorkSheet ;
            object misValue = System.Reflection.Missing.Value;

            xlApp = new Excel.Application();
            xlWorkBook = xlApp.Workbooks.Add(misValue);
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

            string query = "SELECT b.Nomi, (SELECT SUM(k.soni) FROM kirim k WHERE k.id_book = b.id) AS Kirim, b.Narxi, b.Kirim_sana, (SELECT SUM(c.soni) FROM "
                + " chiqim c WHERE b.id = c.id_book) AS Chiqim,b.chiqim,b.Chiqim_sana, (SELECT SUM(k.soni) FROM kirim k WHERE k.id_book = b.id) -(SELECT "
                + " SUM(c.soni) FROM chiqim c WHERE b.id = c.id_book) qoldiq FROM books b; ";

            DataSet ds = new DataSet();
            MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
            adapter.Fill(ds);

            string[] names = { "Названия", "Приход", "Приход Цена", "Приход date", "Расход", "Расход date", "Ostatka", "Qogan" ,"Summa"};
            for (int w=0;w < names.Length;w++){
                xlWorkSheet.Cells[1, w+1] = names[w];
            }

            string today = DateTime.Now.ToString("MM/DD");
            Random rnd = new Random();
            int month = rnd.Next(1, 130);
            string name = today + month.ToString();
            int length = ds.Tables[0].Rows.Count;
            //float sum = totalSum();

            for (int i = 0; i < length ; i++)
            {
                for (int j = 0; j <= ds.Tables[0].Columns.Count - 1; j++)
                {
                    data = ds.Tables[0].Rows[i].ItemArray[j].ToString();
                    xlWorkSheet.Cells[i + 2, j + 1] = data;
                    
                }
                backgroundWorker1.ReportProgress(i, i);
            }

            /*xlWorkSheet.Cells[length + 5, 1] = "Total Summa";
            xlWorkSheet.Cells[length + 5, 2] = Convert.ToString(sum);*/

            backgroundWorker1.RunWorkerAsync(100);
            xlWorkBook.SaveAs(name+".xls", Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
            xlWorkBook.Close(true, misValue, misValue);
            xlApp.Quit();
            MessageBox.Show(name);
            releaseObject(xlWorkSheet);
            releaseObject(xlWorkBook);
            releaseObject(xlApp);
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

//-----------------------------------                  LOGIC          ------------------------------------------//

        public void getCellsValues()
        {
            try
            {
                id = int.Parse(books_datagrid.CurrentRow.Cells[0].Value.ToString());
                if (books_datagrid.CurrentRow != null)
                {
                    int until = books_datagrid.Rows.Count;
                    for( int y=0; y < 12; y++ ) books_datagrid.CurrentRow.Cells[y].Selected = true;

                    name_book.Text = books_datagrid.CurrentRow.Cells[2].Value.ToString();
                    oldName = name_book.Text.ToString();
                    byte[] bytes = (byte[])books_datagrid.CurrentRow.Cells[1].Value;
                    if (bytes != null)
                    {
                        MemoryStream ms = new MemoryStream(bytes);
                        image_book.Image  = byteArrayToImage(bytes);
                        oldImage = bytes;
                        //image_book.Image = Image.FromStream(ms);
                    }
                }
            }
            catch (Exception ex)
            {
                id = -1;
            }
        }

        private float totalSum() {
            MySqlConnection conn = OpenConnection();

            string sql = "Select Summa from books;";
            DataSet ds = new DataSet();

            MySqlDataAdapter adapter = new MySqlDataAdapter(sql, conn);
            adapter.Fill(ds);
            int n = ds.Tables[0].Rows.Count;
            float data = 0;
            for (int i = 0; i < n-1; i++)
            {
                data += Convert.ToInt64(toString(ds.Tables[0].Rows[i].ItemArray[0].ToString()));
            }
            if (conn.State != ConnectionState.Closed)
            {
                conn.Close();
            }
            return data;
        }


        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }
            return destImage;
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

        private void search_from_table()
        {
            //(books_datagrid.DataSource as DataTable).DefaultView.RowFilter = string.Format("Nomi LIKE '{0}%' OR Nomi LIKE '% {0}%'", tv_search.Text);
            BindingSource bs = new BindingSource();
            bs.DataSource = books_datagrid.DataSource;
            bs.Filter = "Nomi like '%" + tv_search.Text + "%'";
            books_datagrid.DataSource = bs;
        }

        public Image byteArrayToImage(byte[] bytesArr)
        {
            using (MemoryStream memstr = new MemoryStream(bytesArr))
            {
                Image img = Image.FromStream(memstr);
                return img;
            }
        }

        private void save(Image image)
        {
            string subPath = "C:\\Store_files";
            bool exists = System.IO.Directory.Exists(subPath);

            if (!exists)
                System.IO.Directory.CreateDirectory(subPath);
            image.Save(subPath + "/" + image.ToString() + ".bmp");
            //bmp1.Save("c:\\button.gif", System.Drawing.Imaging.ImageFormat.Png);
        }

//------------------------------------                  SQL           -----------------------------------------//
       
        public void SelectAndSet()
        {
            MySqlConnection conn = OpenConnection();
            DataSet dataSet = new DataSet();

            MySqlDataAdapter adapter = new MySqlDataAdapter(selectAndSet, conn);
            adapter.Fill(dataSet);

            books_datagrid.DataSource = dataSet.Tables[0].DefaultView;

            if (books_datagrid.RowCount >= 1)
            {
                books_datagrid.Rows[0].Visible = true;
                books_datagrid.Columns["image"].Visible = false;
            }
 
            if (conn.State != ConnectionState.Closed)
            {
                conn.Close();
            }
        }

        private void InsertKirim(long id_book) {
            if (coming_amount.Text.ToString() =="") return;
            MySqlConnection conn = OpenConnection();
            String query = "Insert into world.kirim (id_book, soni, sana) Values(@id_book, @soni, @date)";
            MySqlCommand cmd = ExecuteConnection(conn, query);
        
            cmd.Parameters.AddWithValue("@id_book", id_book);
            cmd.Parameters.AddWithValue("@soni", coming_amount.Text.ToString());
            cmd.Parameters.AddWithValue("@date", coming_date.Value.ToShortDateString());

            cmd.ExecuteNonQuery();

            query = "Update books set Kirim=@soni, Kirim_sana=@sana where id=" + id + ";";
            cmd = ExecuteConnection(conn, query);
            cmd.Parameters.AddWithValue("@soni", coming_amount.Text.ToString());
            cmd.Parameters.AddWithValue("@sana", coming_date.Value.ToShortDateString());
            cmd.ExecuteNonQuery();
        }

        private void InsertChiqim(long id_book) {
            if (out_going_amount.Text.ToString().Equals("")) return;
            MySqlConnection conn = OpenConnection();
            String query = "Insert into world.chiqim (id_book, soni, sana) Values(@id_book, @soni, @date)";
            
            MySqlCommand cmd = ExecuteConnection(conn, query);

            cmd.Parameters.AddWithValue("@id_book", id_book);
            cmd.Parameters.AddWithValue("@soni", out_going_amount.Text.ToString());
            cmd.Parameters.AddWithValue("@date", out_going_date.Value.ToShortDateString());
            
            cmd.ExecuteNonQuery();

            query = "Update books set Chiqim=@soni, Chiqim_sana=@sana where id=" + id + ";";
            cmd = ExecuteConnection(conn, query);
            cmd.Parameters.AddWithValue("@soni", out_going_amount.Text.ToString());
            cmd.Parameters.AddWithValue("@sana", out_going_date.Value.ToShortDateString());
            cmd.ExecuteNonQuery();
        }

        private void UpdateName()
        { 
            MySqlConnection conn = OpenConnection();
            byte[] img = null;
            String query;
            if (clicked) {
                using (var ms = new MemoryStream()) {
                    image_book.Image.Save(ms, ImageFormat.Jpeg);
                    img = ms.ToArray();
                }
                
                    query = "Update books set image=@image_book where id=" + id + ";";
                    MySqlCommand cmd = ExecuteConnection(conn, query);

                    //cmd.Parameters.AddWithValue("@name_book", name_book.Text.ToString());
                    cmd.Parameters.AddWithValue("@image_book", img);
                    cmd.ExecuteNonQuery();
                
            }
            if (coming_price.Text.ToString() != "") {
                query = "Update books set Narxi= @price where id=" + id + ";";
                MySqlCommand cmd = ExecuteConnection(conn, query);
                cmd.Parameters.AddWithValue("@price", coming_price.Text.ToString());
                cmd.ExecuteNonQuery();
            }
            else {
                /*query = "Update books set Nomi= @name_book where id=" + id + ";";
                MySqlCommand cmd = ExecuteConnection(conn, query);
                cmd.Parameters.AddWithValue("@name_book", name_book.Text.ToString());
                cmd.ExecuteNonQuery();
                MessageBox.Show("Update Name");*/
            }
            oldName = name_book.Text.ToString();
            if (conn.State != ConnectionState.Closed)
            {
                conn.Close();
            }

        }

        private void Delete()
        {
            MySqlConnection conn = OpenConnection();
            String query = "delete from kirim where id_book = " + id + ";";
            String queryKirim = "delete from kirim where id_book = " + id + ";";
            String queryChiqim = "delete from chiqim where id_book = " + id + ";";

            ExecuteConnection(conn, query).ExecuteNonQuery();
            ExecuteConnection(conn, queryKirim).ExecuteNonQuery();
            ExecuteConnection(conn, queryChiqim).ExecuteNonQuery();

            if (conn.State != ConnectionState.Closed)
            {
                conn.Close();
            }

            books_datagrid.Rows.RemoveAt(books_datagrid.CurrentRow.Index);
            MessageBox.Show("O'chirildi");
        }

        private MySqlConnection OpenConnection() {
            MySqlConnection conn;
            conn = new MySqlConnection
            {
                ConnectionString = BaseWay
            };

            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

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

        private void Insert()
        {
            MySqlConnection conn = OpenConnection();

            String query = "Insert into world.books (image, Nomi, Kirim, Narxi, Kirim_sana, Chiqim, Chiqim_sana, Summa)" +
                "Values(@image_book, @name_book, @coming_amount, @coming_price, @приход_date, @out_going_amount, @out_going_date, @sum)";
            MySqlCommand cmd = ExecuteConnection(conn, query);
            
            MemoryStream ms = new MemoryStream();
            image_book.Image.Save(ms, ImageFormat.Jpeg);
            byte[] img = ms.ToArray();

            cmd.Parameters.AddWithValue("@image_book", img);
            cmd.Parameters.AddWithValue("@name_book", name_book.Text);

            cmd.Parameters.AddWithValue("@coming_amount", coming_amount.Text.ToString());
            cmd.Parameters.AddWithValue("@coming_price", coming_price.Text.ToString());
            cmd.Parameters.AddWithValue("@приход_date", coming_date.Value.ToShortDateString());

            cmd.Parameters.AddWithValue("@out_going_amount", out_going_amount.Text.ToString());
            cmd.Parameters.AddWithValue("@out_going_date", out_going_date.Value.ToShortDateString());
            cmd.Parameters.AddWithValue("@sum", sum);
            cmd.ExecuteNonQuery();

            long id_book = cmd.LastInsertedId;
            InsertKirim(id_book);
            InsertChiqim(id_book);

            if (conn.State != ConnectionState.Closed)
            {
                conn.Close();
            }

        }

        //--------------------------------------************************----------------------------------------------//
        private string FixBase64ForImage(string pic)
        {
            throw new NotImplementedException();
        }

        private void tv_search_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                search_from_table();
            }
        }

        private String convertToNumber(String value) {
            value = toString(value);
            if (value.Length <2) return value;
            int d = Convert.ToInt32(value);
            StringBuilder build = new StringBuilder();
            String[] a = new String[12];
            int i = 0;
            while( d > 1000) {
                int ss = d % 1000;
                if (ss <9 ) { a[i] = ".00"+ss; }
                else if (ss < 99 && ss > 9) { a[i] = ".0" + ss;}
                else { a[i] = ("."+Convert.ToString(ss)); }
                
                d = d / 1000;
                i++;
            }
            a[i] = Convert.ToString(d);

            while (i >= 0) {
                build.Append(a[i]);
                i--;
            }
            return build.ToString();
        }

        private void coming_price_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) {
                coming_price.Text = convertToNumber(coming_price.Text.ToString());
                //out_going_amount.Focus();
            }
        }

        private void out_going_amount_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //out_going_amount.Text = convertToNumber(out_going_amount.Text.ToString());
            }
        }

        private void coming_amount_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //coming_amount.Text = convertToNumber(coming_amount.Text.ToString());
                //string.Format("{0:#,###}", Convert.ToInt32(toString(coming_amount.Text.ToString())));
                coming_price.Focus();
            }
        }

        private void coming_price_KeyUp(object sender, KeyEventArgs e)
        {
           // if (coming_price.Text.ToString().Length > 2) coming_amount.Text = convertToNumber(coming_amount.Text.ToString());

        }

        private void NewFile() {
            name_book.Text = "";
            coming_amount.Text = "";
            coming_price.Text = "";
            out_going_amount.Text = "";
            image_book.Image  = ResizeImage(Image.FromFile(path), 150, 150);
            oldImage = null;
        }

        private void books_datagrid_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            id = int.Parse(books_datagrid.CurrentRow.Cells[0].Value.ToString());
            Details d = new Details();
            d.Show();
            this.Hide();
        }

        private void name_book_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                coming_amount.Focus();
            }
        }

        private void bunifuButton4_Click_1(object sender, EventArgs e)
        {
            if (name_book.Text == String.Empty)
            {
                MessageBox.Show("Ismini toldiring");
                return;
            }

            if (oldName==name_book.Text.ToString())
            {
                    UpdateName();
                    if (!coming_amount.Text.ToString().Equals("")) InsertKirim(id);
                    if (!out_going_amount.Text.ToString().Equals("")) InsertChiqim(id);   

                    //Ozgartirir kere!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            }
            else
            {
                Insert();
            }
            SelectAndSet();
            string theDate = coming_date.Value.ToString("yyyy-MM-dd");
            NewFile();
            clicked = false;
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            NewFile();
            SelectAndSet();
        }

        private void MainForm_Activated(object sender, EventArgs e)
        {
            
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private String toString(String s)
        {
            char[] delimiterChars = { ' ', ',', '.' };
            if (s == String.Empty) { s = "0"; }
            else
            {
                StringBuilder build = new StringBuilder();
                s = s.Trim();
                String[] a = s.Split(delimiterChars);
                foreach (var mod in a)
                {
                    build.Append(mod);
                }
                s = build.ToString();
            }
            return s;
        }

        private String IsNull(String value)
        {
            if (value == "") return "0";
            else return value;
        }

        private Boolean IsEmpty(String value)
        {
            if (value == "") return false;
            else return true;
        }

        private void AddParametr(MySqlCommand cmd)
        {
            coming_amount.Text = Convert.ToString(Convert.ToInt32(books_datagrid.CurrentRow.Cells[3].Value.ToString()) + Convert.ToInt32(coming_amount.Text.ToString()));
            coming_price.Text = books_datagrid.CurrentRow.Cells[4].Value.ToString();
            coming_date.Value = DateTime.Parse(books_datagrid.CurrentRow.Cells[5].Value.ToString());

            out_going_amount.Text = books_datagrid.CurrentRow.Cells[6].Value.ToString();
            out_going_date.Value = DateTime.Parse(books_datagrid.CurrentRow.Cells[7].Value.ToString());

        }


    }
}

/*
     //***********************************************                  EDIT            ************************************************************
           
            DataGridViewRow newDataRow = books_datagrid.Rows[index];
            MemoryStream ms = new MemoryStream();
            image_book.Image.Save(ms, ImageFormat.Jpeg);
            byte[] img = ms.ToArray();
            newDataRow.Cells[1].Value = img;
            newDataRow.Cells[2].Value = name_book.Text.ToString();
            
            newDataRow.Cells[3].Value = convertToNumber(coming_amount.Text.ToString());
            newDataRow.Cells[4].Value = convertToNumber(coming_price.Text.ToString());            
            newDataRow.Cells[5].Value = coming_date.Value;

            newDataRow.Cells[6].Value = convertToNumber(out_going_amount.Text.ToString());
            newDataRow.Cells[7].Value = out_going_date.Value;

            int com = Convert.ToInt32(toString(coming_amount.Text.ToString()));
            int ou = Convert.ToInt32(toString(out_going_amount.Text.ToString()));
            int pr = Convert.ToInt32(toString(coming_price.Text.ToString()));
            sum = convertToNumber(Convert.ToString((com - ou) * pr));
            newDataRow.Cells[8].Value = sum;

            UpdateValues();
     */