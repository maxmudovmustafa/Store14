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

        String BaseWay, request;
        String parol = "warlock";
        String path = @"C:\Users\WarLock\Downloads/default-book.png";
        String sum = "";
        int id, index;

        public MainForm()
        {
            InitializeComponent();
            InitializeBackgroundWorker();
            BaseWay = @"server=127.0.0.1; port = " + 44999 + ";userid=root;password=" + parol + "; database=world;";
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

        private void bunifuButton1_Click(object sender, EventArgs e)
        {
            SelectAndSet();
            getCellsValues();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SelectAndSet();
            getCellsValues();
        }

        private void edit_Click(object sender, EventArgs e)
        {
            //**********************************************************************************************************************
           
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
            MessageBox.Show("Data updated!!");
            //SelectAndSet();
        }

        private void remove_Click(object sender, EventArgs e)
        {
              DialogResult dialogResult = MessageBox.Show("O`chirish?", "DIQQAT!!!", MessageBoxButtons.YesNo);
              if (dialogResult == DialogResult.Yes)
              {
                  Delete();
                  newFile();
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
                //image_book.Image = ResizeImage(Image.FromFile(op.FileName), 40,40);
            }
        }
 
        private void bunifuButton1_Click_1(object sender, EventArgs e)
        {
            System.Environment.Exit(1);
        }

              
        private void bunifuButton2_Click(object sender, EventArgs e)
        {
            if (bunifuButton2.Text.ToString().Equals("New"))
            {
                newFile();
                name_book.Focus();
            }
            else {
                if (name_book.Text == String.Empty)
                {
                    MessageBox.Show("Ismini toldiring");
                    return;
                }
                /*int com = Convert.ToInt32(toString(coming_amount.Text.ToString()));
                int ou = Convert.ToInt32(toString(out_going_amount.Text.ToString()));
                int pr = Convert.ToInt32(toString(coming_price.Text.ToString()));
                sum = convertToNumber(Convert.ToString((com - ou) * pr));*/

                Insert();
                SelectAndSet();
                string theDate = coming_date.Value.ToString("yyyy-MM-dd");
                newFile();
            
            }
        }

        private void execute_Click_1(object sender, EventArgs e)
        {
            
        }

        private void bunifuButton3_Click(object sender, EventArgs e)
        {
           int a = 0;
            foreach (DataGridViewRow r in books_datagrid.Rows)
            {
                    a += Convert.ToInt32(toString(r.Cells[9].Value.ToString()));
            }
                        
            MessageBox.Show(convertToNumber(a.ToString()));
            
        }

        private void bunifuButton4_Click(object sender, EventArgs e)
        {
            search_from_table();
        }

        private void bunifuButton4_KeyDown(object sender, KeyEventArgs e)
        {
            (books_datagrid.DataSource as DataTable).DefaultView.RowFilter = string.Format("nomi LIKE '{0}%' OR nomi LIKE '% {0}%'", tv_search.Text);
        }

        private void tv_search_KeyPress(object sender, KeyPressEventArgs e)
        {
         
           
        }

        private void bunifuButton6_Click(object sender, EventArgs e)
        {
            /*DialogResult dialogResult = MessageBox.Show("Rostan ham ma'lumotlarni tozalamoqchimisz?", "DIQQAT!!!", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {

                MySqlConnection conn;
                conn = new MySqlConnection();
                conn.ConnectionString = BaseWay;

                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "truncate books;";
                cmd.Prepare();
                cmd.ExecuteNonQuery();
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
            
            MySqlConnection conn;
            conn = new MySqlConnection();
            conn.ConnectionString = BaseWay;

            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            
            string sql = null;
            string data = null;
            
            Excel.Application xlApp ;
            Excel.Workbook xlWorkBook ;
            Excel.Worksheet xlWorkSheet ;
            object misValue = System.Reflection.Missing.Value;

            xlApp = new Excel.Application();
            xlWorkBook = xlApp.Workbooks.Add(misValue);
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

            sql = "SELECT Nomi, Kirim, Narxi, Kirim_sana, Chiqim, Chiqim_sana, Qogan, Summa FROM books";
            DataSet ds = new DataSet();
            MySqlDataAdapter adapter = new MySqlDataAdapter(sql, conn);
            adapter.Fill(ds);

            string[] names = { "Названия", "Приход", "Приход Цена", "Приход date", "Расход", "Расход date", "Ostatka", "Summa" };
            for (int w=0;w < names.Length;w++){
                xlWorkSheet.Cells[1, w+1] = names[w];
            }

            string today = DateTime.Now.ToString("MM/DD");
            Random rnd = new Random();
            int month = rnd.Next(1, 130);
            string name = today + month.ToString();
            int length = ds.Tables[0].Rows.Count;
            float sum = totalSum();

            for (int i = 0; i < length ; i++)
            {
                for (int j = 0; j <= ds.Tables[0].Columns.Count - 1; j++)
                {
                    data = ds.Tables[0].Rows[i].ItemArray[j].ToString();
                    xlWorkSheet.Cells[i + 2, j + 1] = data;
                    
                }
                backgroundWorker1.ReportProgress(i, i);
            }

            xlWorkSheet.Cells[length + 5, 1] = "Total Summa";
            xlWorkSheet.Cells[length + 5, 2] = Convert.ToString(sum);

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
            {
                MessageBox.Show(e.Error.Message);
            }
            else if (e.Cancelled)
            {
                MessageBox.Show("Canceled");
            }
            else
            {
                //MessageBox.Show("Excel File C:\\Documents/_xls");
                progress_bar.Visible = false;
            }
        }


//-----------------------------------                  LOGIC          ------------------------------------------//

        public void getCellsValues()
        {
            try
            {
                id = int.Parse(books_datagrid.CurrentRow.Cells[0].Value.ToString());

                if (books_datagrid.CurrentRow != null)
                {
                    int y = 0;
                    while (y <= 9)
                    {
                        books_datagrid.CurrentRow.Cells[y].Selected = true;
                        y++;
                    }

                    byte[] bytes = (byte[])books_datagrid.CurrentRow.Cells[1].Value;
                    MemoryStream ms = new MemoryStream(bytes);
                    //MessageBox.Show(books_datagrid.CurrentRow.Cells[3].Value.ToString());
                    //image_book.Image  = byteArrayToImage(c.CurrentRow.Cells[0].Value as byte);
                    image_book.Image = Image.FromStream(ms);
                    name_book.Text = books_datagrid.CurrentRow.Cells[2].Value.ToString();

                   /* coming_amount.Text = books_datagrid.CurrentRow.Cells[3].Value.ToString();
                    coming_price.Text = books_datagrid.CurrentRow.Cells[4].Value.ToString();
                    coming_date.Value = DateTime.Parse(books_datagrid.CurrentRow.Cells[5].Value.ToString());

                    out_going_amount.Text = books_datagrid.CurrentRow.Cells[6].Value.ToString();
                    out_going_date.Value = DateTime.Parse(books_datagrid.CurrentRow.Cells[7].Value.ToString());
                 */   
                    //MessageBox.Show(coming_amount.Text.ToString());
                    //MessageBox.Show(toString(coming_amount.Text.ToString()));
                }
            }
            catch (Exception ex)
            {
                id = -1;
            }
        }

        private float totalSum() {
            MySqlConnection conn;
            conn = new MySqlConnection();
            conn.ConnectionString = BaseWay;

            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

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

        private String toString(String s)
        {
            char[] delimiterChars = { ' ', ',', '.' };
            if (s == String.Empty) { s = "0"; }
            else {
                StringBuilder build = new StringBuilder();
                s = s.Trim();
                String[] a = s.Split(delimiterChars);
                foreach (var mod in a) {
                    build.Append(mod);
                }
                s = build.ToString();
            }
            return s;
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
                request = "Select id, image, Nomi, Kirim, Narxi, Kirim_sana, Chiqim, Chiqim_sana, Qogan, Summa from world.books order by Nomi";
                DataSet dataSet = new DataSet();

                MySqlDataAdapter adapter = new MySqlDataAdapter(request, conn);
                adapter.Fill(dataSet);

                books_datagrid.DataSource = dataSet.Tables[0].DefaultView;

                if (books_datagrid.RowCount > 2)
                {
                    books_datagrid.Rows[0].Visible = true;
                    books_datagrid.Columns["image"].Visible = false;
                }

                conn.Close();
            }), ui);
        }

        private void InsertKirim(MySqlConnection conn, long id_book) {
            
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "Insert into world.kirim (id_book, soni, date) Values(@id_book, @soni, @date)";
            cmd.Prepare();
            
            cmd.Parameters.AddWithValue("@id_book", id_book);
            cmd.Parameters.AddWithValue("@soni", coming_amount.Text.ToString());
            cmd.Parameters.AddWithValue("@date", coming_date.Value.ToShortDateString());

            cmd.ExecuteNonQuery();
        }

        private void InsertChiqim(MySqlConnection conn, long id_book) {
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "Insert into world.chiqim (id_book, soni, date) Values(@id_book, @soni, @date)";

            cmd.Prepare();

            cmd.Parameters.AddWithValue("@id_book", id_book);
            cmd.Parameters.AddWithValue("@soni", out_going_amount.Text.ToString());
            cmd.Parameters.AddWithValue("@date", out_going_date.Value.ToShortDateString());

            cmd.ExecuteNonQuery();
            
        }

        private void UpdateValues()
        {
            MySqlConnection conn;
            conn = new MySqlConnection();
            conn.ConnectionString = BaseWay;

            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;
            if (!coming_amount.Text.ToString().Equals("")) InsertChiqim(conn, id);
            else if (!out_going_amount.Text.ToString().Equals("")) InsertChiqim(conn, id);

            cmd.CommandText = "Update books set image=@image_book,"
            + " Nomi= @name_book, Kirim=@coming_amount, Narxi=@coming_price, Kirim_sana=@приход_date, "
            + "Chiqim= @out_going_amount, Chiqim_sana=@out_going_date, Qogan =@qoldi, Summa=@sum"
            + " where id=" + id + ";";

            cmd.Prepare();

            MemoryStream ms = new MemoryStream();
            image_book.Image.Save(ms, ImageFormat.Jpeg);
            byte[] img = ms.ToArray();
            cmd.Parameters.AddWithValue("@name_book", name_book.Text.ToString());
            cmd.Parameters.AddWithValue("@image_book", img);

            AddParametr(cmd);
            cmd.Parameters.AddWithValue("@coming_amount", coming_amount.Text.ToString());
            cmd.Parameters.AddWithValue("@coming_price", coming_price.Text.ToString());
            cmd.Parameters.AddWithValue("@приход_date", coming_date.Value.ToShortDateString().ToString());

            int qoldi = Convert.ToInt32(coming_amount.Text.ToString()) - Convert.ToInt32(out_going_amount.Text.ToString());
            cmd.Parameters.AddWithValue("@out_going_amount", out_going_amount.Text.ToString());
            cmd.Parameters.AddWithValue("@qoldi", qoldi);
            cmd.Parameters.AddWithValue("@out_going_date", out_going_date.Value.ToShortDateString().ToString());

            cmd.Parameters.AddWithValue("@Summa", sum);
            cmd.ExecuteNonQuery();
            cmd.ExecuteNonQuery();

            long id_book = cmd.LastInsertedId;

            InsertKirim(conn, id_book);
            InsertChiqim(conn, id_book);
            
            if (conn.State != ConnectionState.Closed)
            {
                conn.Close();
            }



        }

        private void Delete()
        {
            MySqlConnection conn;
            conn = new MySqlConnection();
            conn.ConnectionString = BaseWay;

            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "delete from books where id = " + id + ";";

            cmd.Prepare();

            cmd.ExecuteNonQuery();

            DeleteKirim(conn);
            DeleteChiqim(conn);
            if (conn.State != ConnectionState.Closed)
            {
                conn.Close();
            }

            books_datagrid.Rows.RemoveAt(books_datagrid.CurrentRow.Index);
        }

        private void DeleteKirim(MySqlConnection conn) {
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "delete from kirim where id_book = " + id + ";";

            cmd.Prepare();

            cmd.ExecuteNonQuery();

        }

        private void DeleteChiqim(MySqlConnection conn) {
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "delete from chiqim where id_book = " + id + ";";

            cmd.Prepare();

            cmd.ExecuteNonQuery();
        }
        private void Insert()
        {
            MySqlConnection conn;
            conn = new MySqlConnection();
            conn.ConnectionString = BaseWay;

            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "Insert into world.books (image, Nomi, Kirim, Narxi, Kirim_sana, Chiqim, Chiqim_sana, Qogan, Summa)" +
                "Values(@image_book, @name_book, @coming_amount, @coming_price, @приход_date, @out_going_amount, @qoldi, @out_going_date, @sum)";

            cmd.Prepare();
            MemoryStream ms = new MemoryStream();
            image_book.Image.Save(ms, ImageFormat.Jpeg);
            byte[] img = ms.ToArray();

            cmd.Parameters.AddWithValue("@image_book", img);
            cmd.Parameters.AddWithValue("@name_book", name_book.Text);

            cmd.Parameters.AddWithValue("@coming_amount", coming_amount.Text.ToString());
            cmd.Parameters.AddWithValue("@coming_price", coming_price.Text.ToString());
            cmd.Parameters.AddWithValue("@приход_date", coming_date.Value.ToShortDateString());

            int qoldi = Convert.ToInt32(coming_amount.Text.ToString()) - Convert.ToInt32(out_going_amount.Text.ToString());
            cmd.Parameters.AddWithValue("@out_going_amount", out_going_amount.Text.ToString());
            cmd.Parameters.AddWithValue("@qoldi", qoldi);
            cmd.Parameters.AddWithValue("@out_going_date", out_going_date.Value.ToShortDateString());
            cmd.Parameters.AddWithValue("@sum", sum);
            cmd.ExecuteNonQuery();

            long id_book = cmd.LastInsertedId;
            InsertKirim(conn, id_book);
            InsertChiqim(conn, id_book);

            if (conn.State != ConnectionState.Closed)
            {
                conn.Close();
            }

        }

//--------------------------------------************************----------------------------------------------//
        private void AddParametr(MySqlCommand cmd)
        {
            coming_amount.Text = Convert.ToString(Convert.ToInt32(books_datagrid.CurrentRow.Cells[3].Value.ToString()) + Convert.ToInt32(coming_amount.Text.ToString()));
            coming_price.Text = books_datagrid.CurrentRow.Cells[4].Value.ToString();
            coming_date.Value = DateTime.Parse(books_datagrid.CurrentRow.Cells[5].Value.ToString());

            out_going_amount.Text = books_datagrid.CurrentRow.Cells[6].Value.ToString();
            out_going_date.Value = DateTime.Parse(books_datagrid.CurrentRow.Cells[7].Value.ToString());
                 
        }

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

        private void execute_Click(object sender, EventArgs e)
        {
            if (name_book.Text == String.Empty)
            {
                MessageBox.Show("Ismini toldiring");
                return;
            }
            /*int com = Convert.ToInt32(toString(coming_amount.Text.ToString()));
            int ou = Convert.ToInt32(toString(out_going_amount.Text.ToString()));
            int pr = Convert.ToInt32(toString(coming_price.Text.ToString()));
            sum = convertToNumber(Convert.ToString((com - ou) * pr));*/
            
            Insert();
            SelectAndSet();
            string theDate = coming_date.Value.ToString("yyyy-MM-dd");
            newFile();
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
                out_going_amount.Focus();
            }
        }

        private void out_going_amount_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                out_going_amount.Text = convertToNumber(out_going_amount.Text.ToString());
            }
        }

        private void coming_amount_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                coming_amount.Text = convertToNumber(coming_amount.Text.ToString());
                //coming_amount.Text = string.Format("{0:#,###}", Convert.ToInt32(toString(coming_amount.Text.ToString())));
                coming_price.Focus();
            }
        }

        private void coming_price_KeyUp(object sender, KeyEventArgs e)
        {
            if (coming_price.Text.ToString().Length > 2) coming_amount.Text = convertToNumber(coming_amount.Text.ToString());

        }

        private void name_book_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                coming_amount.Focus();
                bunifuButton2.Text = "SAVE";
            }
        }

        private void newFile() {
            name_book.Text = "";
            coming_amount.Text = "";
            coming_price.Text = "";
            out_going_amount.Text = "";
            image_book.Image = Image.FromFile(path);
        }

        private void calculateSum() { 
        
        }

        private void books_datagrid_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            Details d = new Details();
            d.Show();
        }
    }
}