using Facade;
using FactoryAccount;
using IAccountInterface;
using QuanLyQuanCafe.DAO;
using QuanLyQuanCafe.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyQuanCafe
{
    public partial class fAdmin : Form
    {
        public List<Table> tableList = null;
        public IAccount loginAccount;
        public List<IAccount> acclist = null;
        AccountUiFacade Fac = new AccountUiFacade("AdoAccDAO");
        IAccount icust = null;

        BindingSource foodList = new BindingSource();
        //BindingSource accountList = new BindingSource();
        BindingSource categoryList = new BindingSource();
        //BindingSource tableList = new BindingSource();

        private bool isFlagFood = false;
        private bool isFlagCategory = false;
        private bool isFlagTable = false;
        private bool isFlagAccount = false;

        
        public fAdmin()
        {
            InitializeComponent();
            Load();
        }

        #region Load Binding Edit Hidden

        void Load()
        {
            LoadDateTimePickerBill();
            LoadListBillByDate(dtpkFromDate.Value, dtpkToDate.Value);
            LoadListFood();
            LoadListAccount();
            LoadCategoryIntoCombobox(cbFoodCategory);
            LoadListCategory();
            LoadListTable();

            dtgvFood.DataSource = foodList;
            dtgvAccount.DataSource = acclist.Where(x=>x.IsUsed == true).ToList();
            dtgvCategory.DataSource = categoryList;
            dtgvTable.DataSource = tableList;

            AddFoodBinding();
            AddAccountBinding();
            AddCategoryBinding();
            AddTableBinding();

            EditNameColumnsFood();
            EditNameColumnsCategory();
            EditNameColumnsTable();

            HiddenColumnsFood();
            HiddenColumnsCategory();
            HiddenColumnsTable();
            HiddenColumnsAccount();

            WidthColumnsTable();
            WidthColumnsCategory();
            WidthColumnsFood();

            FormatDatagridview();
            
        }

        void FormatDatagridview()
        {
            dtgvFood.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dtgvFood.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dtgvFood.EnableHeadersVisualStyles = false;
            dtgvFood.ColumnHeadersDefaultCellStyle.BackColor = Color.Blue;

            dtgvCategory.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dtgvCategory.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dtgvCategory.EnableHeadersVisualStyles = false;
            dtgvCategory.ColumnHeadersDefaultCellStyle.BackColor = Color.Blue;

            dtgvTable.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dtgvTable.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dtgvTable.EnableHeadersVisualStyles = false;
            dtgvTable.ColumnHeadersDefaultCellStyle.BackColor = Color.Blue;

            dtgvAccount.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dtgvAccount.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dtgvAccount.EnableHeadersVisualStyles = false;
            dtgvAccount.ColumnHeadersDefaultCellStyle.BackColor = Color.Blue;

            dtgvBill.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dtgvBill.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dtgvBill.EnableHeadersVisualStyles = false;
            dtgvBill.ColumnHeadersDefaultCellStyle.BackColor = Color.Blue;
        }

        #region Binding
        void AddAccountBinding()
        {
            txbAccountID.DataBindings.Add(new Binding("Text", dtgvAccount.DataSource, "Id", true, DataSourceUpdateMode.Never));
            txbAccountUsername.DataBindings.Add(new Binding("Text", dtgvAccount.DataSource, "UserName", true, DataSourceUpdateMode.Never));
            txbDisplayName.DataBindings.Add(new Binding("Text", dtgvAccount.DataSource, "DisplayName", true, DataSourceUpdateMode.Never));
            txbPhone.DataBindings.Add(new Binding("Text", dtgvAccount.DataSource, "Phone", true, DataSourceUpdateMode.Never));
            txbAddress.DataBindings.Add(new Binding("Text", dtgvAccount.DataSource, "Address", true, DataSourceUpdateMode.Never));
            txbCMND.DataBindings.Add(new Binding("Text", dtgvAccount.DataSource, "CMND", true, DataSourceUpdateMode.Never));
            txbEmail.DataBindings.Add(new Binding("Text", dtgvAccount.DataSource, "Email", true, DataSourceUpdateMode.Never));         
        }

        // Biding GioiTinh, binding Type, binding GioiTinh
        private void txbAccountUsername_TextChanged(object sender, EventArgs e)
        {
            if(isFlagAccount == false)
            {
                IAccount itemp = acclist.Where(x => x.Id == Convert.ToInt32(txbAccountID.Text)).SingleOrDefault();

                if (itemp.GioiTinh == true)
                    cbGioiTinh.SelectedIndex = 0;
                else
                    cbGioiTinh.SelectedIndex = 1;

                if (itemp.Type.Equals("Admin"))
                    cbAccountType.SelectedIndex = 0;
                else if (itemp.Type.Equals("Cashier"))
                    cbAccountType.SelectedIndex = 1;
                else
                    cbAccountType.SelectedIndex = 2;
            }
            
        }

        void AddFoodBinding()
        {
            txbFoodName.DataBindings.Add(new Binding("Text", dtgvFood.DataSource, "Name", true, DataSourceUpdateMode.Never));
            txbFoodID.DataBindings.Add(new Binding("Text", dtgvFood.DataSource, "ID", true, DataSourceUpdateMode.Never));
            nmFoodPrice.DataBindings.Add(new Binding("Value", dtgvFood.DataSource, "Price", true, DataSourceUpdateMode.Never));

        }
        void AddCategoryBinding()
        {
            txbCategoryName.DataBindings.Add(new Binding("Text", dtgvCategory.DataSource, "Name", true, DataSourceUpdateMode.Never));
            txbCategoryID.DataBindings.Add(new Binding("Text", dtgvCategory.DataSource, "ID", true, DataSourceUpdateMode.Never));
        }

        void AddTableBinding()
        {
            txbTableID.DataBindings.Add(new Binding("Text", dtgvTable.DataSource, "ID", true, DataSourceUpdateMode.Never));
            txbTableName.DataBindings.Add(new Binding("Text", dtgvTable.DataSource, "Name", true, DataSourceUpdateMode.Never));
            cbTableStatus.DataBindings.Add(new Binding("Text", dtgvTable.DataSource, "Status", true, DataSourceUpdateMode.Never));
        }

        //Binding Category for cb
        private void txbFoodID_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (dtgvFood.SelectedCells.Count > 0)
                {
                    int id = (int)dtgvFood.SelectedCells[0].OwningRow.Cells["CategoryID"].Value;

                    Category category = CategoryDAO.Instance.GetCategoryByID(id);

                    int index = -1;
                    int i = 0;
                    foreach (Category item in cbFoodCategory.Items)
                    {
                        if (item.ID == category.ID)
                        {
                            index = i;
                            break;
                        }
                        i++;
                    }
                    cbFoodCategory.SelectedIndex = index;

                }
            }
            catch (Exception)
            {
                return;
            }
        }

        #endregion

        #region Edit name columns
        void EditNameColumnsFood()
        {
            dtgvFood.Columns["Name"].HeaderText = "Tên món";
            dtgvFood.Columns["ID"].HeaderText = "Mã";
            dtgvFood.Columns["Price"].HeaderText = "Giá";
        }

        void EditNameColumnsCategory()
        {
            dtgvCategory.Columns["Name"].HeaderText = "Tên doanh mục";
            dtgvCategory.Columns["ID"].HeaderText = "Mã";
        }

        void EditNameColumnsTable()
        {
            dtgvTable.Columns["Name"].HeaderText = "Tên bàn";
            dtgvTable.Columns["ID"].HeaderText = "Mã";
            dtgvTable.Columns["Status"].HeaderText = "Trạng thái";
        }

        #endregion

        #region Load data

        void LoadDateTimePickerBill()
        {
            DateTime today = DateTime.Now;
            dtpkFromDate.Value = new DateTime(today.Year, today.Month, 1);
            dtpkToDate.Value = dtpkFromDate.Value.AddMonths(1).AddDays(-1);
        }

        void LoadListBillByDate(DateTime checkIn, DateTime checkOut)
        {
            dtgvBill.DataSource = BillDAO.Instance.GetBillListByDate(checkIn, checkOut);
        }

        void LoadListAccount()
        {
            acclist = Fac.GetAccounts("select * from Account");
        }

        void LoadListFood()
        {
            foodList.DataSource = FoodDAO.Instance.GetListFood();
        }

        void LoadListCategory()
        {
            categoryList.DataSource = CategoryDAO.Instance.GetListCategory();
        }

        void LoadListTable()
        {
            if(tableList == null)
                tableList = TableDAO.Instance.LoadTableList();
        }

        void LoadCategoryIntoCombobox(ComboBox cb)
        {
            cb.DataSource = CategoryDAO.Instance.GetListCategory();
            cb.DisplayMember = "Name";
        }
        
        #endregion

        #region Hidden Columns

        void HiddenColumnsAccount()
        {
            dtgvAccount.Columns["Id"].Visible = false;
            dtgvAccount.Columns["Password"].Visible = false;
            dtgvAccount.Columns["GioiTinh"].Visible = false;
            dtgvAccount.Columns["Birthday"].Visible = false;
            dtgvAccount.Columns["ImageId"].Visible = false;
            dtgvAccount.Columns["IsUsed"].Visible = false;
            dtgvAccount.Columns["ValidationType"].Visible = false;
            dtgvAccount.Columns["GioiTinh"].Visible = false;
        }

        void HiddenColumnsFood()
        {
            dtgvFood.Columns["CategoryID"].Visible = false;
            dtgvFood.Columns["IsUsed"].Visible = false;
        }

        void HiddenColumnsCategory()
        {
            dtgvCategory.Columns["IsUsed"].Visible = false;
        }

        void HiddenColumnsTable()
        {
            dtgvTable.Columns["IsUsed"].Visible = false;
        }

        #endregion

        #region Set width for columns
        void WidthColumnsTable()
        {
            dtgvTable.Columns["Id"].Width = 200;
            dtgvTable.Columns["Name"].Width = 300;
            dtgvTable.Columns["Status"].Width = 205;
        }

        void WidthColumnsFood()
        {
            dtgvFood.Columns["Id"].Width = 200;
            dtgvFood.Columns["Name"].Width = 300;
            dtgvFood.Columns["Price"].Width = 205;
        }

        void WidthColumnsCategory()
        {
            dtgvCategory.Columns["Id"].Width = 350;
            dtgvCategory.Columns["Name"].Width = 355;
        }
        #endregion

        #endregion

        #region Event


        #region Event Food
        private void btnShowFood_Click(object sender, EventArgs e)
        {
            if (isFlagFood == false)
                LoadListFood();
            else if (btnAddFood.Text.Equals("Lưu"))     // Xử lý cho trường hợp thêm
            {
                ControlItemFood(isFlagFood);
                btnEditFood.Enabled = true;
                btnAddFood.Text = "Thêm";
                isFlagFood = false;
                FoodDAO.Instance.DeleteFoodEmpty(Convert.ToInt32(txbFoodID.Text));       // xóa món ăn khỏi cơ sở dữ liệu
                LoadListFood();

                dtgvFood.Enabled = true;
            }
            else if(btnEditFood.Text.Equals("Lưu"))     // xử lý cho trường hợp sửa
            {
                LoadListFood();
                ControlItemFood(isFlagFood);
                btnAddFood.Enabled = true;
                btnEditFood.Text = "Sửa";
                isFlagFood = false;
            }

        }

        

        bool CheckEmpty()
        {
            if (txbFoodName.Text == "")
            {
                MessageBox.Show("Không được để trống tên món ăn.");
                txbFoodName.Focus();
                return true;
            }
            return false;
        }

        void ControlItemFood(bool flagFood)
        {
            if (flagFood == false)
            {
                btnShowFood.Text = "Hủy";
                txbFoodName.ReadOnly = false;
                nmFoodPrice.ReadOnly = false;
                pndtgvFood.Enabled = false;

                //btnEditFood.Enabled = false;
                btnDeleteFood.Enabled = false;
                //btnShowFood.Enabled = false;

                pnSearchFood.Enabled = false;
            }
            else
            {
                btnShowFood.Text = "Xem";
                txbFoodName.ReadOnly = true;
                nmFoodPrice.ReadOnly = true;

                pndtgvFood.Enabled = true;

                //btnEditFood.Enabled = true;
                btnDeleteFood.Enabled = true;
               // btnShowFood.Enabled = true;

                pnSearchFood.Enabled = true;
            }
        }
        private void btnAddFood_Click(object sender, EventArgs e)
        {
            int idFoodNew = 0;
            if (isFlagFood == false)
            {
                FoodDAO.Instance.InsertFood("", 1, 0);          // Tạo dữ liệu mặc định
                idFoodNew = FoodDAO.Instance.GetMaxIdFood();
                txbFoodID.Text = idFoodNew.ToString();
                txbFoodName.Text = "";
                nmFoodPrice.Value = 0;

                ControlItemFood(isFlagFood);
                btnEditFood.Enabled = false;
                btnAddFood.Text = "Lưu";

                isFlagFood = true;

            }
            else
            {
                int idFood = Convert.ToInt32(txbFoodID.Text);
                string name = txbFoodName.Text;
                int categoryID = (cbFoodCategory.SelectedItem as Category).ID;
                float price = (float)nmFoodPrice.Value;
                if (!CheckEmpty())
                {
                    var rs = MessageBox.Show("Bạn muốn thêm món ăn?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (rs == DialogResult.Yes)
                    {
                        if (FoodDAO.Instance.UpdateFood(idFood, name, categoryID, price))
                        {
                            MessageBox.Show("Thêm món thành công");
                            LoadListFood();
                            if (insertFood != null)
                                insertFood(this, new EventArgs());
                            ControlItemFood(isFlagFood);
                            btnEditFood.Enabled = true;
                            btnAddFood.Text = "Thêm";
                            isFlagFood = false;

                            LoadListFood();
                        }
                        else
                        {
                            MessageBox.Show("Có lỗi khi thêm");
                        }
                    }
                    else
                    {
                        ControlItemFood(isFlagFood);
                        btnEditFood.Enabled = true;
                        btnAddFood.Text = "Thêm";
                        isFlagFood = false;
                        FoodDAO.Instance.DeleteFoodEmpty(idFood);       // xóa món ăn khỏi cơ sở dữ liệu
                        LoadListFood();
                    }
                    dtgvFood.Enabled = true;
                }
            }
        }

        private void btnEditFood_Click(object sender, EventArgs e)
        {
            if (isFlagFood == false)
            {
                ControlItemFood(isFlagFood);
                btnAddFood.Enabled = false;
                btnEditFood.Text = "Lưu";

                isFlagFood = true;
            }
            else
            {
                if (!CheckEmpty())
                {
                    var rs = MessageBox.Show("Bạn muốn thêm món ăn?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (rs == DialogResult.Yes)
                    {
                        string name = txbFoodName.Text;
                        int categoryID = (cbFoodCategory.SelectedItem as Category).ID;
                        float price = (float)nmFoodPrice.Value;
                        int idFood = Convert.ToInt32(txbFoodID.Text);

                        if (FoodDAO.Instance.UpdateFood(idFood, name, categoryID, price))
                        {
                            MessageBox.Show("Sửa món thành công");
                            LoadListFood();
                            if (updateFood != null)
                                updateFood(this, new EventArgs());
                            ControlItemFood(isFlagFood);
                            btnAddFood.Enabled = true;
                            btnEditFood.Text = "Sửa";
                            isFlagFood = false;
                        }
                        else
                        {
                            MessageBox.Show("Có lỗi khi sửa");
                        }
                    }
                    else
                    {

                        LoadListFood();
                        ControlItemFood(isFlagFood);
                        btnAddFood.Enabled = true;
                        btnEditFood.Text = "Sửa";
                        isFlagFood = false;
                    }
                }
            }

        }

        private void btnDeleteFood_Click(object sender, EventArgs e)
        {
            int idFood = Convert.ToInt32(txbFoodID.Text);
            if (FoodDAO.Instance.IsFoodExistInTable(idFood))
            {
                MessageBox.Show("Món ăn đang bán cho khách không đucợ phép xóa");
                return;
            }

            if (FoodDAO.Instance.DeleteFood(idFood))
            {
                MessageBox.Show("Xóa món thành công");
                LoadListFood();
                if (deleteFood != null)
                    deleteFood(this, new EventArgs());
            }
            else
            {
                MessageBox.Show("Có lỗi khi xóa");
            }
        }


        List<Food> SearchFoodByName(string name)
        {
            List<Food> listFood = FoodDAO.Instance.SearchFoodByName(name);
            return listFood;
        }
        private void btnSearchFood_Click(object sender, EventArgs e)
        {
            foodList.DataSource = SearchFoodByName(txbSearchFoodName.Text);
        }

        private event EventHandler insertFood;
        public event EventHandler InsertFood
        {
            add { insertFood += value; }
            remove { insertFood -= value; }
        }

        private event EventHandler deleteFood;
        public event EventHandler DeleteFood
        {
            add { deleteFood += value; }
            remove { deleteFood -= value; }
        }

        private event EventHandler updateFood;
        public event EventHandler UpdateFood
        {
            add { updateFood += value; }
            remove { updateFood -= value; }
        }

        #endregion

        #region Event Category

        void ControlItemCategory(bool flagCategory)
        {
            if (flagCategory == false)
            {
                btnShowCategory.Text = "Hủy";
                txbCategoryName.ReadOnly = false;
                pndtgvCategory.Enabled = false;

                //btnEditCategory.Enabled = false;
                btnDeleteCategory.Enabled = false;
                //btnShowFood.Enabled = false;

                //pnSearchCategory.Enabled = false;
            }
            else
            {
                btnShowCategory.Text = "Xem";
                txbCategoryName.ReadOnly = true;
                pndtgvCategory.Enabled = true;

                //btnEditCategory.Enabled = true;
                btnDeleteCategory.Enabled = true;
                //btnShowCategory.Enabled = true;

                //pnSearchCategory.Enabled = true;
            }
        }

        private void btnShowCategory_Click(object sender, EventArgs e)
        {
            if (isFlagCategory == false)
                LoadListCategory();
            else if (btnAddCategory.Text.Equals("Lưu"))     // Xử lý cho trường hợp thêm
            {
                ControlItemCategory(isFlagCategory);
                btnEditCategory.Enabled = true;
                btnAddCategory.Text = "Thêm";
                isFlagCategory = false;
                CategoryDAO.Instance.DeleteCategoryEmpty(Convert.ToInt32(txbCategoryID.Text));       // xóa món ăn khỏi cơ sở dữ liệu
                LoadListCategory();

                dtgvCategory.Enabled = true;
            }
            else if (btnEditCategory.Text.Equals("Lưu"))     // xử lý cho trường hợp sửa
            {
                LoadListCategory();
                ControlItemCategory(isFlagCategory);
                btnAddCategory.Enabled = true;
                btnEditCategory.Text = "Sửa";
                isFlagCategory = false;
            }
        }

        private void btnAddCategory_Click(object sender, EventArgs e)
        {
            int idCategoryNew = 0;
            if (isFlagCategory == false)
            {
                CategoryDAO.Instance.InsertCategory("");          // Tạo dữ liệu mặc định
                idCategoryNew = CategoryDAO.Instance.GetMaxIdCategory();
                txbCategoryID.Text = idCategoryNew.ToString();
                txbCategoryName.Text = "";


                ControlItemCategory(isFlagCategory);
                btnEditCategory.Enabled = false;
                btnAddCategory.Text = "Lưu";
                isFlagCategory = true;

            }
            else
            {
                if (!CheckDataEmptyCategory())
                {
                    int idCategory = Convert.ToInt32(txbCategoryID.Text);
                    string name = txbCategoryName.Text;

                    var rs = MessageBox.Show("Bạn muốn thêm món ăn?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (rs == DialogResult.Yes)
                    {
                        if (CategoryDAO.Instance.UpdateCategory(idCategory, name))
                        {
                            MessageBox.Show("Thêm doanh mục thành công");
                            if (insertCategory != null)
                                insertCategory(this, new EventArgs());
                            ControlItemCategory(isFlagCategory);
                            btnEditCategory.Enabled = true;
                            btnAddCategory.Text = "Thêm";
                            isFlagCategory = false;

                            LoadListCategory();
                        }
                        else
                        {
                            MessageBox.Show("Có lỗi khi thêm");
                        }
                    }
                    else
                    {
                        ControlItemCategory(isFlagCategory);
                        btnEditCategory.Enabled = true;
                        btnAddCategory.Text = "Thêm";
                        isFlagCategory = false;
                        CategoryDAO.Instance.DeleteCategoryEmpty(idCategory);       // xóa doanh mục khỏi cơ sở dữ liệu
                        LoadListCategory();
                    }
                    dtgvCategory.Enabled = true;
                }
            }
        }

        private bool CheckDataEmptyCategory()
        {
            if (string.IsNullOrEmpty(txbCategoryName.Text))
            {
                MessageBox.Show("Không được để trống tên doanh mục.");
                txbCategoryName.Focus();
                return true;
            }
            return false;
        }

        bool IsCategoryHaveFood(int idCategory)
        {
            int count = FoodDAO.Instance.GetCountFoodByCategoryID(idCategory);
            return count > 0;
        }

        private void btnDeleteCategory_Click(object sender, EventArgs e)
        {
            int idCategory = Convert.ToInt32(txbCategoryID.Text);
            if (IsCategoryHaveFood(idCategory))
            {
                MessageBox.Show("Doach mục đang chứa món ăn nên không được xóa");
                return;
            }
            else
            {
                if (CategoryDAO.Instance.DeleteCategory(idCategory))
                {
                    MessageBox.Show("Xóa doanh mục thành công");
                    LoadListCategory();
                    if (deleteCategory != null)
                        deleteCategory(this, new EventArgs());
                }
                else
                {
                    MessageBox.Show("Có lỗi khi xóa");
                }
            }
        }

        private void btnEditCategory_Click(object sender, EventArgs e)
        {
            if (isFlagCategory == false)
            {
                ControlItemCategory(isFlagCategory);
                btnAddCategory.Enabled = false;
                btnEditCategory.Text = "Lưu";

                isFlagCategory = true;
            }
            else
            {
                if (!CheckDataEmptyCategory())
                {
                    var rs = MessageBox.Show("Bạn muốn chỉnh sửa doanh mục?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (rs == DialogResult.Yes)
                    {
                        string name = txbCategoryName.Text;

                        int idCategory = Convert.ToInt32(txbCategoryID.Text);

                        if (CategoryDAO.Instance.UpdateCategory(idCategory, name))
                        {
                            MessageBox.Show("Sửa doanh mục thành công");
                            LoadListCategory();
                            if (updateCategory != null)
                                updateCategory(this, new EventArgs());
                            ControlItemCategory(isFlagCategory);
                            btnAddCategory.Enabled = true;
                            btnEditCategory.Text = "Sửa";
                            isFlagCategory = false;
                        }
                        else
                        {
                            MessageBox.Show("Có lỗi khi sửa");
                        }
                    }
                    else
                    {
                        LoadListCategory();
                        ControlItemCategory(isFlagCategory);
                        btnAddCategory.Enabled = true;
                        btnEditCategory.Text = "Sửa";
                        isFlagCategory = false;
                    }
                }
            }
        }

        private event EventHandler insertCategory;
        public event EventHandler InsertCategory
        {
            add { insertCategory += value; }
            remove { insertCategory -= value; }
        }

        private event EventHandler deleteCategory;
        public event EventHandler DeleteCategory
        {
            add { deleteCategory += value; }
            remove { deleteCategory -= value; }
        }

        private event EventHandler updateCategory;
        public event EventHandler UpdateCategory
        {
            add { updateCategory += value; }
            remove { updateCategory -= value; }
        }
        #endregion

        #region Event Table


        private void ControlItemTable(bool flagTable)
        {
            if (flagTable == false)
            {
                btnShowTable.Text = "Hủy";
                txbTableName.ReadOnly = false;
                pndtgvTable.Enabled = false;
                //btnEditTable.Enabled = false;
                btnDeleteTable.Enabled = false;
                //btnShowTable.Enabled = false;

                //pnSearchCategory.Enabled = false;
            }
            else
            {
                btnShowTable.Text = "Xem";
                txbTableName.ReadOnly = true;
                pndtgvTable.Enabled = true;

                //btnEditCategory.Enabled = true;
                btnDeleteTable.Enabled = true;
                btnShowTable.Enabled = true;

                //pnSearchCategory.Enabled = true;
            }
        }

        private void btnAddTable_Click(object sender, EventArgs e)
        {

            int idTableNew = 0;
            if (isFlagTable == false)
            {
                TableDAO.Instance.InsertTable("", "Trống", true);          // Tạo dữ liệu mặc định
                idTableNew = TableDAO.Instance.GetMaxIdTable();
                txbTableID.Text = idTableNew.ToString();
                txbTableName.Text = "";
                cbTableStatus.SelectedIndex = 0;

                ControlItemTable(isFlagTable);
                btnEditTable.Enabled = false;
                btnAddTable.Text = "Lưu";
                isFlagTable = true;

            }
            else
            {
                if (!CheckDataEmptyTable())
                {
                    int idTable = Convert.ToInt32(txbTableID.Text);
                    string name = txbTableName.Text;
                    string status = cbTableStatus.SelectedItem.ToString();
                    var rs = MessageBox.Show("Bạn muốn thêm bàn?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (rs == DialogResult.Yes)
                    {
                        if (TableDAO.Instance.UpdateTable(idTable, name, status))
                        {
                            MessageBox.Show("Thêm bàn thành công");
                            LoadListTable();
                            if (insertTable != null)
                                insertTable(this, new EventArgs());
                            ControlItemTable(isFlagTable);
                            btnAddTable.Text = "Thêm";
                            btnEditTable.Enabled = true;
                            isFlagTable = false;

                            LoadListTable();
                        }
                        else
                        {
                            MessageBox.Show("Có lỗi khi thêm");
                        }
                    }
                    else
                    {
                        ControlItemTable(isFlagTable);
                        btnAddTable.Text = "Thêm";
                        btnEditTable.Enabled = true;
                        isFlagTable = false;
                        TableDAO.Instance.DeleteTableEmpty(idTable);       // xóa bàn khỏi cơ sở dữ liệu
                        LoadListTable();
                    }
                    dtgvTable.Enabled = true;
                }
            }
        }

        private bool CheckDataEmptyTable()
        {
            if (txbTableName.Text == "")
            {
                MessageBox.Show("Không được để trống tên bàn.");
                txbTableName.Focus();
                return true;
            }
            return false;
        }

        private void btnDeleteTable_Click(object sender, EventArgs e)
        {
            int idTable = Convert.ToInt32(txbTableID.Text);
            if (!TableDAO.Instance.TableIsEmpty(idTable))       // Kiểm tra bàn có đang trống hay không
            {
                MessageBox.Show("Bàn ăn đang có khách nên không được xóa");
                return;
            }
            else
            {
                if (TableDAO.Instance.DeleteTable(idTable))
                {
                    MessageBox.Show("Xóa bàn thành công");
                    LoadListTable();
                    //if (deleteTable != null)
                    //    deleteTable(this, new EventArgs());
                }
                else
                {
                    MessageBox.Show("Có lỗi khi xóa");
                }
            }
        }

        private void btnEditTable_Click(object sender, EventArgs e)
        {

            if (isFlagTable == false)
            {
                ControlItemTable(isFlagTable);
                btnAddTable.Enabled = false;
                btnEditTable.Text = "Lưu";

                isFlagTable = true;

                string[] strStatus = { "Có người", "Trống" };
                cbTableStatus.DataSource = strStatus;
            }
            else
            {
                if (!CheckDataEmptyTable())
                {
                    var rs = MessageBox.Show("Bạn muốn chỉnh sửa bàn ăn?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (rs == DialogResult.Yes)
                    {
                        string name = txbTableName.Text;
                        string status = cbTableStatus.SelectedItem.ToString();

                        int idTable = Convert.ToInt32(txbTableID.Text);

                        if (TableDAO.Instance.UpdateTable(idTable, name, status))
                        {
                            MessageBox.Show("Sửa doanh mục thành công");
                            LoadListTable();
                            if (updateTable != null)
                                updateTable(this, new EventArgs());
                            ControlItemTable(isFlagTable);
                            btnAddTable.Enabled = true;
                            btnEditTable.Text = "Sửa";
                            isFlagTable = false;
                        }
                        else
                        {
                            MessageBox.Show("Có lỗi khi sửa");
                        }
                    }
                    else
                    {
                        LoadListTable();
                        ControlItemTable(isFlagTable);
                        btnAddTable.Enabled = true;
                        btnEditTable.Text = "Sửa";
                        isFlagTable = false;
                    }
                }
            }
        }

        private void btnShowTable_Click(object sender, EventArgs e)
        {
            if (isFlagTable == false)
                LoadListTable();
            else if (btnAddTable.Text.Equals("Lưu"))     // Xử lý cho trường hợp thêm
            {
                ControlItemTable(isFlagTable);
                btnEditTable.Enabled = true;
                btnAddTable.Text = "Thêm";
                isFlagTable = false;
                TableDAO.Instance.DeleteTableEmpty(Convert.ToInt32(txbTableID.Text));       // xóa món ăn khỏi cơ sở dữ liệu
                LoadListTable();

                dtgvTable.Enabled = true;
            }
            else if (btnEditTable.Text.Equals("Lưu"))     // xử lý cho trường hợp sửa
            {
                LoadListTable();
                ControlItemTable(isFlagTable);
                btnAddTable.Enabled = true;
                btnEditTable.Text = "Sửa";
                isFlagTable = false;
            }
        }

        private event EventHandler insertTable;
        public event EventHandler InsertTable
        {
            add { insertTable += value; }
            remove { insertTable -= value; }
        }

        private event EventHandler deleteTable;
        public event EventHandler DeleteTable
        {
            add { deleteTable += value; }
            remove { deleteTable -= value; }
        }

        private event EventHandler updateTable;
        public event EventHandler UpdateTable
        {
            add { updateTable += value; }
            remove { updateTable -= value; }
        }

        #endregion

        #region Event Bill

        private void btnFirst_Click(object sender, EventArgs e)
        {
            txbPage.Text = "1";
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            int page = Convert.ToInt32(txbPage.Text);

            if (page > 1)

                page--;
            txbPage.Text = page.ToString();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            int page = Convert.ToInt32(txbPage.Text);
            int sumRecord = BillDAO.Instance.GetNumBillListByDate(dtpkFromDate.Value, dtpkToDate.Value);

            if (page < sumRecord)
                page++;
            txbPage.Text = page.ToString();
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            int sumRecord = BillDAO.Instance.GetNumBillListByDate(dtpkFromDate.Value, dtpkToDate.Value);
            int lastPage = sumRecord / 10;
            if (sumRecord % 10 != 0)
                lastPage += 1;

            txbPage.Text = lastPage.ToString();
        }

        private void txbPage_TextChanged(object sender, EventArgs e)
        {
            dtgvBill.DataSource = BillDAO.Instance.GetBillListByDateAndPage(dtpkFromDate.Value, dtpkToDate.Value, Convert.ToInt32(txbPage.Text));
        }

        private void btnViewbill_Click(object sender, EventArgs e)
        {
            LoadListBillByDate(dtpkFromDate.Value, dtpkToDate.Value);
        }

        #endregion


        #endregion

        void DeleteAccount(string userName)
        {
            if (loginAccount.UserName.Equals(userName))
            {
                MessageBox.Show("Không được xóa tài khoản đang sử dụng");
                return;
            }
            if (AccountDAO.Instance.DeleteAccount(userName))
            {
                MessageBox.Show("Xóa tài khoản thành công");
            }
            else
            {
                MessageBox.Show("Xóa tài khoản thất bại");
            }

            LoadListAccount();
        }

        void ResetPassword(string userName)
        {
            AccountDAO.Instance.ResetPssword(userName);
        }

        //private void btnAddAccount_Click(object sender, EventArgs e)
        //{
        //    int idAccountNew = 0;
        //    if (isFlagAccount == false)
        //    {
        //        AccountDAO.getInstance.InsertAccount("", "", "");          // Tạo dữ liệu mặc định
        //        idAccountNew = AccountDAO.getInstance.GetMaxIdAccount();
        //        txbAccountID.Text = idAccountNew.ToString();
        //        txbAccountUsername.Text = "";
        //        txbDisplayName.Text = "";

        //        ControlItemAccount(isFlagAccount);
        //        btnEditAccount.Enabled = false;
        //        btnAddAccount.Text = "Lưu";
        //        isFlagAccount = true;

        //        string[] strType = { "Admin", "Staff" };
        //        cbAccountType.DataSource = strType;
        //    }
        //    else
        //    {
        //        if (!CheckDataEmptyAccount())
        //        {
        //            int idAccount = Convert.ToInt32(txbAccountID.Text);
        //            string username = txbAccountUsername.Text;
        //            string displayname = txbDisplayName.Text;
        //            string type = cbAccountType.SelectedValue.ToString();
        //            var rs = MessageBox.Show("Bạn muốn thêm tài khoản?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        //            if (rs == DialogResult.Yes)
        //            {
        //                if (AccountDAO.getInstance.UpdateAccount(idAccount, username, displayname, type))
        //                {
        //                    MessageBox.Show("Thêm bàn thành công");
        //                    LoadListAccount();
        //                    if (insertAccount != null)
        //                        insertAccount(this, new EventArgs());
        //                    ControlItemAccount(isFlagAccount);
        //                    btnAddAccount.Text = "Thêm";
        //                    btnEditAccount.Enabled = true;
        //                    isFlagAccount = false;

        //                    LoadListAccount();
        //                }
        //                else
        //                {
        //                    MessageBox.Show("Có lỗi khi thêm");
        //                }
        //            }
        //            else
        //            {
        //                ControlItemAccount(isFlagAccount);
        //                btnAddAccount.Text = "Thêm";
        //                btnEditAccount.Enabled = true;
        //                isFlagAccount = false;
        //                AccountDAO.getInstance.DeleteAccountEmpty(idAccount);       // xóa bàn khỏi cơ sở dữ liệu
        //                LoadListAccount();
        //            }
        //            dtgvAccount.Enabled = true;
        //        }
        //    }
        //}

        void Clear()
        {
            txbAccountUsername.Text = "";
            txbDisplayName.Text = "";
            cbAccountType.SelectedIndex = 1;
            txbPhone.Text = "";
            cbGioiTinh.SelectedIndex = 1;
            txbAddress.Text = "";
            txbCMND.Text = "";
            txbEmail.Text = "";
        }

        void GetDataFromUI(IAccount icust)
        {
            icust.Id = Convert.ToInt32(txbAccountID.Text);
            //icust.Id = Convert.ToInt32(acclist.Max(x => x.Id).ToString()) + 1;
            icust.UserName = txbAccountUsername.Text;
            icust.DisplayName = txbDisplayName.Text;
            icust.Password = "1962026656160185351301320480154111117132155";
            icust.IsUsed = true;
            icust.Phone = txbPhone.Text;
            if (cbGioiTinh.SelectedIndex == 1)
                icust.GioiTinh = true;
            else
                icust.GioiTinh = false;
            icust.Address = txbAddress.Text;
            icust.CMND = txbCMND.Text;
            icust.ImageID = "";
            icust.Birthday = DateTime.Now;
            icust.Email = txbEmail.Text;
        }

        private void btnShowAccount_Click(object sender, EventArgs e)
        {
            LoadListAccount();
            HiddenColumnsAccount();
        }

        bool IsExistUsername(string Username)
        {
            return acclist.Where(x => x.UserName == Username).Count() > 0;
        }
        private void btnAddAccount_Click(object sender, EventArgs e)
        {
            if (isFlagAccount == false)
            {
                Clear();
                ControlItemAccount(isFlagAccount);
                btnEditAccount.Enabled = false;
                btnAddAccount.Text = "Lưu";
                isFlagAccount = true;
                int newID = Convert.ToInt32(acclist.Max(x => x.Id).ToString()) + 1;
                txbAccountID.Text = newID.ToString(); 
            }
            else
            {
                var rs = MessageBox.Show("Bạn muốn thêm tài khoản mới?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (rs == DialogResult.Yes)
                {
                    if (IsExistUsername(txbAccountUsername.Text))
                    {
                        MessageBox.Show("Tên tài khoản đã tồn tại!");
                        return;
                    }

                    icust = Fac.Get(cbAccountType.Text);
                    GetDataFromUI(icust);
                    if (icust.Validate() == false)
                        return;
                    Fac.Save(icust);
                    dtgvAccount.DataSource = null;          // reset datasource datagridview
                    acclist.Add(icust);
                    isFlagAccount = true;
                    ControlItemAccount(isFlagAccount);
                    btnAddAccount.Text = "Thêm";
                    isFlagAccount = false;
                    btnEditAccount.Enabled = true;
                }
                else
                {
                    ControlItemAccount(isFlagAccount);
                    btnAddAccount.Text = "Thêm";
                    isFlagAccount = false;
                    btnEditAccount.Enabled = true;
                }
            }
            dtgvAccount.DataSource = acclist;
            HiddenColumnsAccount();
        }

        bool IsAccountUsing(int id, string username)
        {
            if (id == loginAccount.Id || username.Equals(loginAccount.UserName))
                return true;
            return false;
        }

        private void btnDeleteAccount_Click(object sender, EventArgs e)
        {
            if(IsAccountUsing(Convert.ToInt32(txbAccountID.Text), txbAccountUsername.Text))
            {
                MessageBox.Show("Không thể xóa tài khoản đang sử dụng!");
                return;
            }
            var rs = MessageBox.Show("Bạn muốn xóa tài khoản?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (rs == DialogResult.Yes)
            {
                icust = Fac.Get(cbAccountType.Text);
                GetDataFromUI(icust);
                icust.IsUsed = false;                   // Đổi cột IsUsed thành false
                Fac.Save(icust);
                dtgvAccount.DataSource = null;          // reset datasource datagridview
                acclist.Remove(acclist.Where(x=>x.Id == icust.Id).SingleOrDefault());                  // xóa khỏi danh sách 
                dtgvAccount.DataSource = acclist;
            }
        }

        void EditAccList(int id)
        {
            IAccount item = acclist.Where(x => x.Id == id).SingleOrDefault();
            item.Phone = txbPhone.Text;
            item.Birthday = DateTime.Now;
            item.DisplayName = txbDisplayName.Text;
            if (cbGioiTinh.SelectedIndex == 0)
                item.GioiTinh = true;
            else
                item.GioiTinh = false;
            item.CMND = txbCMND.Text;
            item.Email = txbEmail.Text;
            item.Address = txbAddress.Text;
        }

        private void btnEditAccount_Click(object sender, EventArgs e)
        {
            if (isFlagAccount == false)
            {
                ControlItemAccount(isFlagAccount);
                btnAddAccount.Enabled = false;
                btnEditAccount.Text = "Lưu";
                isFlagAccount = true;
                //pnType.Enabled = false;         // không cho phép thay đổi quyền 
            }
            else
            {
                var rs = MessageBox.Show("Bạn muốn chỉnh sửa tài khoản?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (rs == DialogResult.Yes)
                {
                    icust = Fac.Get(cbAccountType.Text);
                    GetDataFromUI(icust);
                    if (icust.Validate() == false)
                        return;
                    Fac.Save(icust);
                    //dtgvAccount.DataSource = null;                          // reset datasource datagridview
                    EditAccList(Convert.ToInt32(txbAccountID.Text));        // chỉnh sửa trong accList
                    ControlItemAccount(isFlagAccount);
                    btnEditAccount.Text = "Sửa";
                    btnAddAccount.Enabled = true;
                    isFlagAccount = false;
                }
                else
                {
                    ControlItemAccount(isFlagAccount);
                    btnAddAccount.Enabled = true;
                    btnEditAccount.Text = "Sửa";
                    isFlagAccount = false;
                    pnType.Enabled = true;
                }
            }
            dtgvAccount.DataSource = acclist;
            HiddenColumnsAccount();
        }
        private bool CheckDataEmptyAccount()
        {
            if (string.IsNullOrEmpty(txbAccountUsername.Text) ||
                string.IsNullOrEmpty(txbDisplayName.Text))
            {
                MessageBox.Show("Bạn không được bỏ trống trường dữ liệu");
                return true;
            }
            return false;
        }

        private void ControlItemAccount(bool flagAccount)
        {
            if (flagAccount == false)
            {
                txbAccountUsername.ReadOnly = false;
                txbDisplayName.ReadOnly = false;
                txbPhone.ReadOnly = false;
                txbAddress.ReadOnly = false;
                txbCMND.ReadOnly = false;
                txbEmail.ReadOnly = false;

                pndtgvAccount.Enabled = false;

                //btnEditAccount.Enabled = false;
                btnDeleteAccount.Enabled = false;
                btnShowAccount.Enabled = false;

                //pnSearchCategory.Enabled = false;
            }
            else
            {
                txbAccountUsername.ReadOnly = true;
                txbDisplayName.ReadOnly = true;
                txbPhone.ReadOnly = true;
                txbAddress.ReadOnly = true;
                txbCMND.ReadOnly = true;
                txbEmail.ReadOnly = true;

                pndtgvAccount.Enabled = true;

                //btnEditCategory.Enabled = true;
                btnDeleteAccount.Enabled = true;
                btnShowAccount.Enabled = true;

                //pnSearchCategory.Enabled = true;
            }
        }

        private void btnResetPassword_Click(object sender, EventArgs e)
        {
            icust = Fac.Get(cbAccountType.Text);
            GetDataFromUI(icust);
            if (icust.Validate() == false)
                return;
            icust.Password = "1962026656160185351301320480154111117132155";
            Fac.Save(icust);
            //dtgvAccount.DataSource = null;                          // reset datasource datagridview
            EditAccList(Convert.ToInt32(txbAccountID.Text));        // chỉnh sửa trong accList

        }

    }
}
