using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using System.Data;
using System.Drawing;


using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.IO;

namespace MySql
{
    class Class1
    {}
    /*
Bitmap def = null;
            BindingSource bSource = new BindingSource();
            bSource.DataSource = table;
            this.books_datagrid.DataSource = bSource;
            DataGridViewImageColumn imageColumn = new DataGridViewImageColumn();
            imageColumn.HeaderText = "Pic";
            books_datagrid.Columns.Insert(0, imageColumn);
            for (int i = 0; i < table.Rows.Count; i++)
            {
                try
                {
                    String pic = table.Rows[i]["Item_Pic"].ToString();
                    Byte[] bitmapData = Convert.FromBase64String(FixBase64ForImage(pic));
                    System.IO.MemoryStream streamBitmap = new System.IO.MemoryStream(bitmapData);
                    def = new Bitmap((Bitmap)Image.FromStream(streamBitmap));
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.StackTrace);
                }
                books_datagrid.Rows[i].Cells[0].Value = def;
            }
            //books_datagrid.Columns.Remove("Item_Pic");
            foreach (DataGridViewRow row in books_datagrid.Rows)
            {
                row.Height = 110;
            }
            foreach (DataGridViewColumn col in books_datagrid.Columns)
            {
                col.Width = 110;
            }

            for (int i = 0; i < books_datagrid.ColumnCount; i++)
            {
                books_datagrid.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                books_datagrid.AutoResizeColumns();
                books_datagrid.Columns[i].DefaultCellStyle.Font = new System.Drawing.Font("Verdana", 8F, FontStyle.Bold);
            }
    */
}