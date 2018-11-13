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
        private bool isAddTable = false;
        private bool isAddAccount = false;

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
            LoadAccount();
            LoadCategoryIntoCombobox(cbFoodCategory);
            LoadListCategory();


            dtgvFood.DataSource = foodList;
            dtgvAccount.DataSource = accountList;
            dtgvCategory.DataSource = categoryList;
            dtgvCategory.Columns["IsUsed"].Visible = false;             // ẩn cột IsUsed


            AddFoodBinding();
            AddAccountBinding();
            AddCategoryBinding();
        }


        void AddAccountBinding()
        {
            txbUsername.DataBindings.Add(new Binding("Text", dtgvAccount.DataSource, "UserName", true, DataSourceUpdateMode.Never));
            txbDisplayName.DataBindings.Add(new Binding("Text", dtgvAccount.DataSource, "DisplayName", true, DataSourceUpdateMode.Never));
            foreach (DataGridViewColumn col in dtgvAccount.Columns)
            {
                if (col.Name == "Type")
                {
                    //DO your Stuff here..
                }
            }

            nmAccountType.DataBindings.Add(new Binding("Value", dtgvAccount.DataSource, "Type", true, DataSourceUpdateMode.Never));

            //if (nmAccountType.Value == 0)
            //    nmAccountType.Value = "abc";

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
            // binding cbTableStatus
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

        void LoadAccount()
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
                btnAddFood.Text = "Lưu";

                pndtgvFood.Enabled = false;

                btnEditFood.Enabled = false;
                btnDeleteFood.Enabled = false;
                btnShowFood.Enabled = false;

                pnSearchFood.Enabled = false;
            }
            else
            {
                btnAddFood.Text = "Thêm";

                pndtgvFood.Enabled = true;

                btnEditFood.Enabled = true;
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
                btnEditFood.Text = "Lưu";
                dtgvFood.Enabled = false;
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
                            dtgvFood.Enabled = true;
                            btnEditFood.Text = "Sửa";
                        }
                        else
                        {
                            MessageBox.Show("Có lỗi khi sửa");
                        }
                    }
                    else
                    {
                        isFlagFood = false;
                        LoadListFood();
                        btnEditFood.Text = "Sửa";
                        dtgvFood.Enabled = true;
                    }
                }
            }

        }

        private void btnDeleteFood_Click(object sender, EventArgs e)
        {
            int idFood = Convert.ToInt32(txbFoodID.Text);
            if (IsFoodExistInTable(idFood))
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
                btnAddCategory.Text = "Lưu";

                pndtgvCategory.Enabled = false;

                btnEditCategory.Enabled = false;
                btnDeleteCategory.Enabled = false;
                btnShowFood.Enabled = false;

                //pnSearchCategory.Enabled = false;
            }
            else
            {
                btnAddCategory.Text = "Thêm";

                pndtgvCategory.Enabled = true;

                btnEditCategory.Enabled = true;
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
                isFlagCategory = true;

            }
            else
            {
                if (!CheckEmptyCategory())
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
                        isFlagCategory = false;
                        CategoryDAO.Instance.DeleteCategoryEmpty(idCategory);       // xóa doanh mục khỏi cơ sở dữ liệu
                        LoadListCategory();
                    }
                    dtgvCategory.Enabled = true;
                }
            }
        }

        private bool CheckEmptyCategory()
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
            string name = txbCategoryName.Text;
            int idCategory = Convert.ToInt32(txbCategoryID.Text);

            if (CategoryDAO.Instance.UpdateCategory(idCategory, name))
            {
                MessageBox.Show("Sửa doanh mục thành công");
                LoadListCategory();
                if (updateCategory != null)
                    updateCategory(this, new EventArgs());
            }
            else
            {
                MessageBox.Show("Có lỗi khi sửa");
            }
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
        bool IsFoodExistInTable(int idFood)
        {
            Food itemFood = FoodDAO.Instance.GetFoodByID(idFood);
            List<Table> tableList = TableDAO.Instance.LoadTableList();

            foreach (Table item in tableList)
            {
                List<QuanLyQuanCafe.DTO.Menu> listBillInfo = MenuDAO.Instance.GetListMenuByTable(item.ID);
                foreach (QuanLyQuanCafe.DTO.Menu itemMenu in listBillInfo)
                {
                    if (itemFood.Name.Equals(itemMenu.FoodName))
                        return true;

                }
            }
            return false;
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





        private void btnShowAccount_Click(object sender, EventArgs e)
        {
            LoadAccount();
        }


        void AddAccount(string userName, string displayName, int type)
        {
            foreach (Account item in accountList)
            {
                if (item.UserName == userName)
                {
                    MessageBox.Show("Tên tài khoản đã tồn tại.");
                    return;
                }
            }
            if (AccountDAO.Instance.InsertAccount(userName, displayName, type))
            {
                MessageBox.Show("Thêm tài khoản thành công");
            }
            else
            {
                MessageBox.Show("Thêm tài khoản thất bại");
            }

            LoadAccount();
        }

        void EditAccount(string userName, string displayName, int type)
        {
            if (AccountDAO.Instance.UpdateAccount(userName, displayName, type))
            {
                MessageBox.Show("Sửa tài khoản thành công");
            }
            else
            {
                MessageBox.Show("Sửa tài khoản thất bại");
            }

            LoadAccount();
        }


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

            LoadAccount();
        }


        void ResetPassword(string userName)
        {
            AccountDAO.Instance.ResetPssword(userName);
        }

        private void btnAddAccount_Click(object sender, EventArgs e)
        {
            string userName = txbUsername.Text;
            string displayName = txbDisplayName.Text;
            int type = (int)nmAccountType.Value;

            AddAccount(userName, displayName, type);
        }

        private void btnDeleteAccount_Click(object sender, EventArgs e)
        {
            string userName = txbUsername.Text;

            DeleteAccount(userName);
        }

        private void btnEditAccount_Click(object sender, EventArgs e)
        {
            string userName = txbUsername.Text;
            string displayName = txbDisplayName.Text;
            int type = (int)nmAccountType.Value;

            EditAccount(userName, displayName, type);
        }

        private void btnResetPassword_Click(object sender, EventArgs e)
        {
            string userName = txbUsername.Text;

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
