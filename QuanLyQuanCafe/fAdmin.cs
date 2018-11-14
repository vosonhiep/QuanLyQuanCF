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
        BindingSource foodList = new BindingSource();
        BindingSource accountList = new BindingSource();
        BindingSource categoryList = new BindingSource();
        BindingSource tableList = new BindingSource();

        private bool isFlagFood = false;
        private bool isFlagCategory = false;
        private bool isFlagTable = false;
        private bool isFlagAccount = false;

        public Account loginAccount;
        public fAdmin()
        {
            InitializeComponent();
            Load();
        }

        #region Load and Binding

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
            dtgvAccount.DataSource = accountList;
            dtgvCategory.DataSource = categoryList;
            dtgvTable.DataSource = tableList;

            dtgvCategory.Columns["IsUsed"].Visible = false;             // ẩn cột IsUsed

            AddFoodBinding();
            AddAccountBinding();
            AddCategoryBinding();
            AddTableBinding();
        }


        void AddAccountBinding()
        {
            txbAccountUsername.DataBindings.Add(new Binding("Text", dtgvAccount.DataSource, "UserName", true, DataSourceUpdateMode.Never));
            txbDisplayName.DataBindings.Add(new Binding("Text", dtgvAccount.DataSource, "DisplayName", true, DataSourceUpdateMode.Never));
            cbAccountType.DataBindings.Add(new Binding("Text", dtgvAccount.DataSource, "Type", true, DataSourceUpdateMode.Never));

            //nmAccountType.DataBindings.Add(new Binding("Text", dtgvAccount.DataSource, "Type", true, DataSourceUpdateMode.Never));

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
            txbTableID.DataBindings.Add(new Binding("Text", dtgvTable.DataSource, "Name", true, DataSourceUpdateMode.Never));
            txbTableName.DataBindings.Add(new Binding("Text", dtgvTable.DataSource, "ID", true, DataSourceUpdateMode.Never));
            cbTableStatus.DataBindings.Add(new Binding("Text", dtgvTable.DataSource, "Status", true, DataSourceUpdateMode.Never));
        }

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
            accountList.DataSource = AccountDAO.Instance.GetListAccount();
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
            tableList.DataSource = TableDAO.Instance.LoadTableList();
        }

        void LoadCategoryIntoCombobox(ComboBox cb)
        {
            cb.DataSource = CategoryDAO.Instance.GetListCategory();
            cb.DisplayMember = "Name";
        }
        #endregion

        #region Event


        #region Event Food
        private void btnShowFood_Click(object sender, EventArgs e)
        {
            LoadListFood();
        }

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


                pndtgvFood.Enabled = false;

                //btnEditFood.Enabled = false;
                btnDeleteFood.Enabled = false;
                btnShowFood.Enabled = false;

                pnSearchFood.Enabled = false;
            }
            else
            {


                pndtgvFood.Enabled = true;

                //btnEditFood.Enabled = true;
                btnDeleteFood.Enabled = true;
                btnShowFood.Enabled = true;

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
                pndtgvCategory.Enabled = false;

                //btnEditCategory.Enabled = false;
                btnDeleteCategory.Enabled = false;
                btnShowFood.Enabled = false;

                //pnSearchCategory.Enabled = false;
            }
            else
            {


                pndtgvCategory.Enabled = true;

                //btnEditCategory.Enabled = true;
                btnDeleteCategory.Enabled = true;
                btnShowCategory.Enabled = true;

                //pnSearchCategory.Enabled = true;
            }
        }

        private void btnShowCategory_Click(object sender, EventArgs e)
        {
            LoadListCategory();
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

        #endregion


        #region Event Table


        private void ControlItemTable(bool flagTable)
        {
            if (flagTable == false)
            {
                pndtgvTable.Enabled = false;

                //btnEditTable.Enabled = false;
                btnDeleteTable.Enabled = false;
                btnShowTable.Enabled = false;

                //pnSearchCategory.Enabled = false;
            }
            else
            {
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

                    var rs = MessageBox.Show("Bạn muốn thêm bàn?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (rs == DialogResult.Yes)
                    {
                        if (TableDAO.Instance.UpdateTable(idTable, name, "Trống"))
                        {
                            MessageBox.Show("Thêm bàn thành công");
                            LoadListTable();
                            //if (insertTable != null)
                            //    insertTable(this, new EventArgs());
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
            }
            else
            {
                if (!CheckDataEmptyTable())
                {
                    var rs = MessageBox.Show("Bạn muốn chỉnh sửa bàn ăn?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (rs == DialogResult.Yes)
                    {
                        string name = txbTableName.Text;
                        string status = cbTableStatus.SelectedValue.ToString();

                        int idTable = Convert.ToInt32(txbTableID.Text);

                        if (TableDAO.Instance.UpdateTable(idTable, name, status))
                        {
                            MessageBox.Show("Sửa doanh mục thành công");
                            LoadListTable();
                            //if (updateTable != null)
                            //    updateTable(this, new EventArgs());
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
            LoadListTable();
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



        private void btnViewbill_Click(object sender, EventArgs e)
        {
            LoadListBillByDate(dtpkFromDate.Value, dtpkToDate.Value);
        }

        #endregion







        void GetFoodById(int id)
        {

        }

        /// <summary>
        /// Kiểm tra xem món ăn này có trong danh sách bàn ăn không
        /// </summary>
        /// <param name="idFood"></param>
        //bool IsFoodExistInTable(int idFood)
        //{
        //    Food itemFood = FoodDAO.Instance.GetFoodByID(idFood);
        //    List<Table> tableList = TableDAO.Instance.LoadTableList();

        //    foreach (Table item in tableList)
        //    {
        //        List<QuanLyQuanCafe.DTO.Menu> listBillInfo = MenuDAO.Instance.GetListMenuByTable(item.ID);
        //        foreach (QuanLyQuanCafe.DTO.Menu itemMenu in listBillInfo)
        //        {
        //            if (itemFood.Name.Equals(itemMenu.FoodName))
        //                return true;

        //        }
        //    }
        //    return false;
        //}




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





        private void btnShowAccount_Click(object sender, EventArgs e)
        {
            LoadListAccount();
        }


        //void AddAccount(string userName, string displayName, int type)
        //{
        //    foreach (Account item in accountList)
        //    {
        //        if (item.UserName == userName)
        //        {
        //            MessageBox.Show("Tên tài khoản đã tồn tại.");
        //            return;
        //        }
        //    }
        //    if (AccountDAO.Instance.InsertAccount(userName, displayName, type))
        //    {
        //        MessageBox.Show("Thêm tài khoản thành công");
        //    }
        //    else
        //    {
        //        MessageBox.Show("Thêm tài khoản thất bại");
        //    }

        //    LoadAccount();
        //}

        //void EditAccount(string userName, string displayName, int type)
        //{
        //    if (AccountDAO.Instance.UpdateAccount(userName, displayName, type))
        //    {
        //        MessageBox.Show("Sửa tài khoản thành công");
        //    }
        //    else
        //    {
        //        MessageBox.Show("Sửa tài khoản thất bại");
        //    }

        //    LoadListAccount();
        //}


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

        private void btnAddAccount_Click(object sender, EventArgs e)
        {
            string[] strType = {"Admin","Staff"};
            cbAccountType.DataSource = strType;

            int idAccountNew = 0;
            if (isFlagAccount == false)
            {
                AccountDAO.Instance.InsertAccount("","","");          // Tạo dữ liệu mặc định
                idAccountNew = AccountDAO.Instance.GetMaxIdAccount();
                txbAccountID.Text = idAccountNew.ToString();
                txbAccountUsername.Text = "";
                txbDisplayName.Text = "";

                ControlItemAccount(isFlagAccount);
                btnEditAccount.Enabled = false;
                btnAddAccount.Text = "Lưu";
                isFlagAccount = true;

            }
            else
            {
                if (!CheckDataEmptyAccount())
                {
                    int idAccount = Convert.ToInt32(txbAccountID.Text);
                    string username = txbAccountUsername.Text;
                    string displayname = txbDisplayName.Text;
                    string type = cbAccountType.SelectedValue.ToString();
                    var rs = MessageBox.Show("Bạn muốn thêm tài khoản?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (rs == DialogResult.Yes)
                    {
                        if (AccountDAO.Instance.UpdateAccount(idAccount, username, displayname,type))
                        {
                            MessageBox.Show("Thêm bàn thành công");
                            LoadListAccount();
                            //if (insertAccount != null)
                            //    insertAccount(this, new EventArgs());
                            ControlItemAccount(isFlagAccount);
                            btnAddAccount.Text = "Thêm";
                            btnEditAccount.Enabled = true;
                            isFlagAccount = false;

                            LoadListAccount();
                        }
                        else
                        {
                            MessageBox.Show("Có lỗi khi thêm");
                        }
                    }
                    else
                    {
                        ControlItemAccount(isFlagAccount);
                        btnAddAccount.Text = "Thêm";
                        btnEditAccount.Enabled = true;
                        isFlagAccount = false;
                        AccountDAO.Instance.DeleteAccountEmpty(idAccount);       // xóa bàn khỏi cơ sở dữ liệu
                        LoadListAccount();
                    }
                    dtgvAccount.Enabled = true;
                }
            }
        }

        private bool CheckDataEmptyAccount()
        {
            if(string.IsNullOrEmpty(txbAccountUsername.Text) || 
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
                pndtgvAccount.Enabled = false;

                //btnEditAccount.Enabled = false;
                btnDeleteAccount.Enabled = false;
                btnShowAccount.Enabled = false;

                //pnSearchCategory.Enabled = false;
            }
            else
            {
                pndtgvAccount.Enabled = true;

                //btnEditCategory.Enabled = true;
                btnDeleteAccount.Enabled = true;
                btnShowAccount.Enabled = true;

                //pnSearchCategory.Enabled = true;
            }
        }

        private void btnDeleteAccount_Click(object sender, EventArgs e)
        {
            string userName = txbAccountUsername.Text;

            DeleteAccount(userName);
        }

        private void btnEditAccount_Click(object sender, EventArgs e)
        {
            if (isFlagAccount == false)
            {
                ControlItemAccount(isFlagAccount);
                btnAddAccount.Enabled = false;
                btnEditAccount.Text = "Lưu";

                isFlagAccount = true;
            }
            else
            {
                if (!CheckDataEmptyAccount())
                {
                    var rs = MessageBox.Show("Bạn muốn chỉnh sửa tài khoản?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (rs == DialogResult.Yes)
                    {
                        string username = txbAccountUsername.Text;
                        string type = cbAccountType.SelectedValue.ToString();
                        string displayName = txbDisplayName.Text;
                        int idAccount = Convert.ToInt32(txbAccountID.Text);

                        if (AccountDAO.Instance.UpdateAccount(idAccount, username, displayName, type))
                        {
                            MessageBox.Show("Sửa tài khoản thành công");
                            LoadListAccount();
                            //if (updateAccount != null)
                            //    updateAccount(this, new EventArgs());
                            ControlItemAccount(isFlagAccount);
                            btnAddAccount.Enabled = true;
                            btnEditAccount.Text = "Sửa";
                            isFlagAccount = false;
                        }
                        else
                        {
                            MessageBox.Show("Có lỗi khi sửa");
                        }
                    }
                    else
                    {
                        LoadListAccount();
                        ControlItemAccount(isFlagAccount);
                        btnAddAccount.Enabled = true;
                        btnEditAccount.Text = "Sửa";
                        isFlagAccount = false;
                    }
                }
            }
        }

        private void btnResetPassword_Click(object sender, EventArgs e)
        {
            string userName = txbAccountUsername.Text;

            ResetPassword(userName);
        }

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



    }
}
