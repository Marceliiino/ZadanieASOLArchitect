using System;
using System.Data;
using System.Data.Entity;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Linq;
using Invoice.ApiCalls;
using Invoice.DB;

namespace Invoice
{
    public partial class ProductList : ResizeForm
    {
        invoice_dbEntities invoiceDbEntities = new invoice_dbEntities();
        private DbConnectorClass db;
        private SqlDataAdapter adapter;
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        public ProductList()
        {
            InitializeComponent();
            //ProductApi.GetProducts();
            db = new DbConnectorClass();
            SqlDataReader dbReader = db.RunQuery("select Catagory from dbo.Catagory group by Catagory order by CASE Catagory " +
                            "WHEN 'MEAT' THEN 1 WHEN 'FROZEN' THEN 2 WHEN 'PRODUCE' THEN 3 WHEN 'GROCERY' THEN 4  WHEN 'FRUIT' THEN 5 ELSE 6  END, Catagory");
            (this.ProductDataView.Columns[4] as DataGridViewComboBoxColumn).Items.Clear();
            while (dbReader.Read())
            {
                (this.ProductDataView.Columns[4] as DataGridViewComboBoxColumn).Items.Add(db.NullToNA(dbReader, "Catagory").Trim());
                this.CatagoryBox.Items.Add(db.NullToNA(dbReader, "Catagory").Trim());
            }
            dbReader.Close();
            GetProductList();
            AddBtns();
            this.MaximizedBounds = Screen.GetWorkingArea(this);
        }
        public void GetProductList() {
            this.ProductDataView.Rows.Clear();
            if (rbEF.Checked)
            {
                GetProductListEF();
            }
            else if (rbWebApi.Checked)
            {
                GetProductListWebApi();
            }
            else
            {
                GetProductListOriginal();
            }
        }

        public void GetProductListOriginal()
        {
            try
            {
                String whereStr = "";
                if (this.CatagoryBox.SelectedItem != null && !this.CatagoryBox.SelectedItem.Equals(""))
                {
                    whereStr = "where Catagory = '" + this.CatagoryBox.SelectedItem + "'";
                }
                db = new DbConnectorClass();
                adapter = new SqlDataAdapter("select product_id as No, Product, Price, Quantity, Catagory, SubCatagory, Note from dbo.product " + whereStr + " order by CASE Catagory " +
                            "WHEN 'MEAT' THEN 1 WHEN 'FROZEN' THEN 2 WHEN 'PRODUCE' THEN 3 WHEN 'GROCERY' THEN 4  WHEN 'FRUIT' THEN 5 ELSE 6  END, Catagory, SubCatagory", db.GetConnection());
                // Create one DataTable with one column.
                DataSet DS = new DataSet();
                adapter.Fill(DS);
                for (int i = 0; i < DS.Tables[0].Rows.Count; i++)
                {
                    DataRow myRow = DS.Tables[0].Rows[i];
                    DataGridViewRow row = (DataGridViewRow)ProductDataView.Rows[0].Clone();
                    row.Cells[0].Value = myRow[0].ToString();
                    row.Cells[1].Value = myRow[1].ToString();
                    row.Cells[2].Value = myRow[2].ToString();
                    row.Cells[3].Value = myRow[3].ToString();
                    row.Cells[4].Value = myRow[4].ToString().Trim();
                    SelectedCatagory(row, myRow[4].ToString().Trim());
                    row.Cells[5].Value = myRow[5].ToString().Trim();
                    row.Cells[6].Value = myRow[6].ToString();
                    this.ProductDataView.Rows.Add(row);
                }
                //    this.ProductDataView.DataSource = DS.Tables[0];
                //    this.ProductDataView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void GetProductListEF()
        {
            try
            {
                var category = (string) this.CatagoryBox.SelectedItem;
                var products = String.IsNullOrWhiteSpace(category)
                    ? invoiceDbEntities.products.AsQueryable()
                    : invoiceDbEntities.products.Where(x => x.catagory == category);

                foreach (var product in products.OrderBy(x => x.product_id))
                {
                    // product_id as No, Product, Price, Quantity, Catagory, SubCatagory, Not
                    DataGridViewRow row = (DataGridViewRow) ProductDataView.Rows[0].Clone();
                    row.Cells[0].Value = product.product_id.ToString();
                    row.Cells[1].Value = product.product1.ToString();
                    row.Cells[2].Value = product.price.ToString();
                    row.Cells[3].Value = product.quantity.ToString();
                    row.Cells[4].Value = product.catagory.ToString().Trim();
                    SelectedCatagory(row, product.catagory.ToString().Trim());
                    row.Cells[5].Value = (string)product.subcatagory;
                    row.Cells[6].Value = (string)product.note;
                    this.ProductDataView.Rows.Add(row);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        public void GetProductListWebApi()
        {
            var category = (string)this.CatagoryBox.SelectedItem;
            var products = ProductApi.GetProductList(category);

            foreach (var product in products.OrderBy(x => x.product_id))
            {
                // product_id as No, Product, Price, Quantity, Catagory, SubCatagory, Not
                DataGridViewRow row = (DataGridViewRow)ProductDataView.Rows[0].Clone();
                row.Cells[0].Value = product.product_id.ToString();
                row.Cells[1].Value = product.product1.ToString();
                row.Cells[2].Value = product.price.ToString();
                row.Cells[3].Value = product.quantity.ToString();
                row.Cells[4].Value = product.catagory.ToString().Trim();
                SelectedCatagory(row, product.catagory.ToString().Trim());
                row.Cells[5].Value = (string)product.subcatagory;
                row.Cells[6].Value = (string)product.note;
                this.ProductDataView.Rows.Add(row);
            }
        }

        private void AddBtns()
        {
            DataGridViewButtonColumn SaveBtnColumn = new DataGridViewButtonColumn
            {
                HeaderText = "",
                Name = "Save",
                Text = "Save",
                UseColumnTextForButtonValue = true
            };
            this.ProductDataView.Columns.Add(SaveBtnColumn);
            DataGridViewButtonColumn DeleteBtnColumn = new DataGridViewButtonColumn
            {
                HeaderText = "",
                Name = "Delete",
                Text = "Delete",
                UseColumnTextForButtonValue = true
            };
            this.ProductDataView.Columns.Add(DeleteBtnColumn);
            this.ProductDataView.EditingControlShowing +=
                    new DataGridViewEditingControlShowingEventHandler(DataGridView_EditingControlShowing);

        }
        // Calls the Employee.RequestStatus method.
        private void DataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Ignore clicks that are not on button cells. 
            int rowNum = e.RowIndex;
            if (e.RowIndex >= 0) {
                DataGridViewRow row = this.ProductDataView.Rows[rowNum];
                if (e.ColumnIndex == this.ProductDataView.Columns["Save"].Index)
                    SaveProduce(row, false);
                if (e.ColumnIndex == this.ProductDataView.Columns["Delete"].Index)
                    DeleteProduce(row);
            }
        }
        String oldValue = "";
        public void DataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            
            if (e.Control is ComboBox combo)
            {
                oldValue = (String)combo.SelectedItem;
                // Remove an existing event-handler, if present, to avoid 
                // adding multiple handlers when the editing control is reused.
                combo.SelectedIndexChanged -= new EventHandler(DataGridView1_CellValueChanged);
                // Add the event handler. 
                combo.SelectedIndexChanged += new EventHandler(DataGridView1_CellValueChanged);
            }
        }
        private void DataGridView1_DataError(object sender, EventArgs e)
        {
            //do nothing when data error occur
        }
        private void DataGridView1_CellValueChanged(object sender, EventArgs e)
        {
            String catagoryName = (String)((ComboBox)sender).SelectedItem;
            int rowNum = (int)((DataGridViewComboBoxEditingControl)sender).EditingControlRowIndex;

            if (rowNum > 0)
            {
                DataGridViewRow row = this.ProductDataView.Rows[rowNum];
                if (oldValue != catagoryName)
                {
                    SelectedCatagory(row, catagoryName);
                    ((DataGridViewComboBoxEditingControl)sender).BackColor = Color.White;

                    DataGridViewComboBoxEditingControl combo = (DataGridViewComboBoxEditingControl)sender;
                    combo.SelectedIndexChanged -= new EventHandler(DataGridView1_CellValueChanged);
                    combo.DropDown += new EventHandler(combo_DropDown);
                    combo.GotFocus += new EventHandler(combo_DropDown);
                }
            }
        }
        void combo_DropDown(object sender, EventArgs e)
        {
            ((DataGridViewComboBoxEditingControl)sender).BackColor = Color.White;
        }


        private void SelectedCatagory(DataGridViewRow row, string catagoryName)
        {
            row.Cells[5].Value = 0;
            (row.Cells[5] as DataGridViewComboBoxCell).Items.Clear();
            if (!row.Cells[4].Value.Equals("")) {
                SqlDataReader dbReader = db.RunQuery("select distinct SubCatagory from dbo.Catagory where Catagory = '" + catagoryName.Trim() + "'");
                while (dbReader.Read())
                {
                    (row.Cells[5] as DataGridViewComboBoxCell).Items.Add(db.NullToNA(dbReader, "SubCatagory").Trim());
                }
                dbReader.Close();
            }
        }
        private void DeleteProduce(DataGridViewRow row)
        {
            // Retrieve the task ID.
            String productId = row.Cells[0].Value.ToString();
            try
            {
                var x = MessageBox.Show("Are you sure you want to delete? ", "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (DialogResult.Yes == x)
                {
                    if (rbEF.Checked)
                    {
                        DeleteProduceEF(productId);
                    }
                    else if (rbWebApi.Checked)
                    {
                        //DeleteProduceWebApi(productId);
                    }
                    else
                    {
                        DeleteProduceOriginal(productId);
                    }

                    // need to close this form after click 'OK' button]
                    this.ProductDataView.Rows.Remove(row);
                    GetProductList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DeleteProduceOriginal(String productId)
        {
            String sqlQuery = "";
            sqlQuery = "Delete FROM dbo.product WHERE product_id = " + productId;
            db.RunQuery(sqlQuery).Close();
        }

        private product GetProduct(String productId)
        {
            var pid = Convert.ToInt32(productId);
            if (String.IsNullOrWhiteSpace(productId))
            {
                return null;
            }

            return this.invoiceDbEntities.products.FirstOrDefault(x => x.product_id == pid);
        }

        private void DeleteProduceEF(String productId)
        {
            var product = this.GetProduct(productId);
            if (product == null)
            {
                return;
            }

            this.invoiceDbEntities.products.Remove(product);
            this.invoiceDbEntities.SaveChanges();
        }

        private void SaveProduce(DataGridViewRow row, bool saveAll)
        {
            // Retrieve the task ID.
            String productId = "";
            String Product = "";
            String Price = "";
            String Quantity = "";
            String Catagory = "";
            String SubCatagory = "";
            String Note = "";
            if (row.Cells[0].Value != null)
                productId    = row.Cells[0].Value.ToString();
            if (row.Cells[1].Value != null)
                Product      = row.Cells[1].Value.ToString();
            if (row.Cells[2].Value != null)
                Price        = row.Cells[2].Value.ToString();
            if (row.Cells[3].Value != null)
                Quantity     = row.Cells[3].Value.ToString();
            if (row.Cells[4].Value != null)
                Catagory     = row.Cells[4].Value.ToString().ToUpper();
            if (row.Cells[5].Value != null)
                SubCatagory  = row.Cells[5].Value.ToString().ToUpper();
            if (row.Cells[6].Value != null)
                Note         = row.Cells[6].Value.ToString();

            if (Price.Equals(""))
            {
                Price = "0.00";
            }
            if (Quantity.Equals(""))
            {
                Quantity = "0";
            }
            try
            {
                var x = DialogResult.No;
                if (!saveAll)
                    x = MessageBox.Show("Are you sure you want to save? ", "Save", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (DialogResult.Yes == x || saveAll)
                {
                    if (rbEF.Checked)
                    {
                        SaveProduceEF(productId, Product, Price, Quantity, Catagory, SubCatagory, Note);
                    }
                    else if (rbWebApi.Checked)
                    {
                        //SaveProduceWebApi(productId, Product, Price, Quantity, Catagory, SubCatagory, Note);
                    }
                    else
                    {
                        SaveProduceOriginal(productId, Product, Price, Quantity, Catagory, SubCatagory, Note);
                    }

                    // need to close this form after click 'OK' button
                    GetProductList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SaveProduceOriginal(String productId, String Product, String Price, String Quantity, String Catagory, String SubCatagory, String Note)
        {
            String sqlQuery = "";
            if (productId.Equals(""))
            {
                sqlQuery = "INSERT INTO dbo.product " +
                           "(product, price, quantity, note, catagory, subcatagory) VALUES " +
                           "(N'" + Product + "', " +
                           " '" + Price + "', " +
                           " '" + Quantity + "', " +
                           " N'" + Note + "', " +
                           " N'" + Catagory + "', " +
                           " N'" + SubCatagory + "') ";
            }
            else
            {
                sqlQuery = "UPDATE dbo.product set " +
                           "product = N'" + Product + "', " +
                           "price = '" + Price + "', " +
                           "quantity = '" + Quantity + "', " +
                           "Note = N'" + Note + "', " +
                           "Catagory = N'" + Catagory + "', " +
                           "SubCatagory = N'" + SubCatagory + "' WHERE product_id = " + productId;
            }
            db.RunQuery(sqlQuery).Close();

        }

        private void SaveProduceEF(String productId, String Product, String Price, String Quantity, String Catagory, String SubCatagory, String Note)
        {
            var pid = Convert.ToInt32(productId);
            if (String.IsNullOrWhiteSpace(productId))
            {
                var product = new product()
                {
                    product_id = pid, product1 = Product, price = Convert.ToDecimal(Price),
                    quantity = Convert.ToInt32(Quantity), catagory = Catagory, subcatagory = SubCatagory, note = Note
                };
                this.invoiceDbEntities.products.Add(product);
            }
            else
            {
                var product = this.GetProduct(productId);
                if (product != null)
                {
                    product.product1 = Product;
                }
            }

            this.invoiceDbEntities.SaveChanges();
        }

        private void CloseBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void DragTitlePanel(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            for(int i = 0; i < this.ProductDataView.RowCount-1; i++)
            {
                DataGridViewRow row = this.ProductDataView.Rows[i];
                SaveProduce(row, true);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetProductList();
        }

        private void RbCheckedChanged(object sender, EventArgs e)
        {
            if (sender is RadioButton rb && rb.Checked)
            {
                this.GetProductList();
            }
        }
    }
}
