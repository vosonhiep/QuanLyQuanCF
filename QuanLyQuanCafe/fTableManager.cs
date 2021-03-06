﻿using IAccountInterface;
using QuanLyQuanCafe.DAO;
using QuanLyQuanCafe.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyQuanCafe
{
    public partial class fTableManager : Form
    {
        private IAccount loginAccount;
        List<Table> tableList = null;
        List<Button> btnList = new List<Button>();
        private bool isTextChanged = true;
        public IAccount LoginAccount
        {
            get { return loginAccount; }
            set 
            {
                loginAccount = value;
                ChangeAccount(loginAccount.Type);
            }
        }
        public fTableManager(IAccount acc)
        {
            InitializeComponent();

            this.LoginAccount = acc;        // truyền tài khoản đăng nhập

            LoadTable();
            LoadCategory();
            LoadComboboxTable(cbSwitchTable);
        }

        #region Method

        void ChangeAccount(string type)
        {
            if (type.Equals("Admin"))
            {
                //btnAddFood.Enabled = false;
                //btnSwitchTable.Enabled = false;
                //btnCheckOut.Enabled = false;
                pnAdmin_Cashier.Visible = true;
            }
            else if (type.Equals("Staff"))
            {
                //adminToolStripMenuItem.Visible = false;
                //btnCheckOut.Enabled = false;
                pnStaff.Visible = true;
            }
            else
            {
                //adminToolStripMenuItem.Visible = false;
                //btnAddFood.Enabled = false;
                pnAdmin_Cashier.Visible = true;
            }
        }

        void LoadCategory()
        {
            List<Category> listCategory = CategoryDAO.Instance.GetListCategory();
            cbCategory.DataSource = listCategory;
            cbCategory.DisplayMember = "Name";
        }

        void LoadFoodListByCategoryID(int id)
        {
            List<Food> listFood = FoodDAO.Instance.GetFoodByCategoryID(id);
            cbFood.DataSource = listFood;
            cbFood.DisplayMember = "Name";
        }

        void LoadTable()
        {
            flpTable.Controls.Clear();

            if(tableList == null)
                tableList = TableDAO.Instance.LoadTableList();

            foreach (Table item in tableList)
            {
                Button btn = new Button() { Width = TableDAO.TableWidth, Height = TableDAO.TableHeight };
                btn.Text = item.Name + Environment.NewLine + item.Status;
                btn.Click += btn_Click;
                btn.Tag = item;
                btn.ForeColor = Color.White;
                btn.Font = new Font("Segoe UI", 12, FontStyle.Bold);
                btn.Name = "btn" + item.ID;         // name của btn = "btn" + Name(table)
                switch (item.Status)
                {
                    case "Trống":
                        btn.BackColor = Color.LimeGreen;
                        break;
                    default:
                        btn.BackColor = Color.DodgerBlue;
                        break;
                }
                btnList.Add(btn);
                flpTable.Controls.Add(btn);
            }
        }

        /// <summary>
        /// Sau khi thêm món hoặc thanh toán hóa đơn thì chỉ load lại table nào cần load
        /// </summary>
        void LoadTableId(int idTable)
        {
            Table item = tableList.Where(x => x.ID == idTable).SingleOrDefault();
            Button btn = btnList.Where(x => x.Name == ("btn" + item.ID)).SingleOrDefault();
            switch (item.Status)
            {
                case "Trống":
                    btn.BackColor = Color.LimeGreen;
                    break;
                default:
                    btn.BackColor = Color.DodgerBlue;
                    break;
            }
        }

        void ShowBill(int id)
        {
            lsvBill.Items.Clear();
            List<QuanLyQuanCafe.DTO.Menu> listBillInfo = MenuDAO.Instance.GetListMenuByTable(id);
            float totalPrice = 0;
            foreach (QuanLyQuanCafe.DTO.Menu item in listBillInfo)
            {
                ListViewItem lsvItem = new ListViewItem(item.FoodName.ToString());
                lsvItem.SubItems.Add(item.Count.ToString());
                lsvItem.SubItems.Add(item.Price.ToString());
                lsvItem.SubItems.Add(item.TotalPrice.ToString());
                totalPrice += item.TotalPrice;
                lsvBill.Items.Add(lsvItem);
            }
            //CultureInfo culture = new CultureInfo("vi-VN");

            //Thread.CurrentThread.CurrentCulture = culture;

            txtTotalPrice.Text = totalPrice.ToString();

        }

        void LoadComboboxTable(ComboBox cb)
        {
            cb.DataSource = TableDAO.Instance.LoadTableList();
            cb.DisplayMember = "Name";
        }

        #endregion


        #region Events
        void btn_Click(object sender, EventArgs e)
        {
            int tableID = ((sender as Button).Tag as Table).ID;
            lsvBill.Tag = (sender as Button).Tag;
            ShowBill(tableID);
        }
        private void đăngXuấtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pnAdmin_Cashier.Visible = false;
            pnStaff.Visible = false;
            this.Close();
        }

        private void thôngTinCáNhânToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fAccountProfile f = new fAccountProfile(LoginAccount);
            f.UpdateAccount += f_UpdateAccount;
            f.ShowDialog();
        }

        void f_UpdateAccount(object sender, AccountEvent e)
        {
            thToolStripMenuItem.Text = "Thông tin tài khoản (" + e.Acc.DisplayName + ")";
        }

        private void adminToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fAdmin f = new fAdmin();
            f.loginAccount = LoginAccount;
            f.tableList = tableList;

            f.InsertFood += f_InsertFood;
            f.UpdateFood += f_UpdateFood;
            f.DeleteFood += f_DeleteFood;

            f.InsertCategory += f_InsertCategory;
            f.UpdateCategory += f_UpdateCategory;
            f.DeleteCategory += f_DeleteCategory;

            f.InsertTable += f_InsertTable;
            f.UpdateTable += f_UpdateTable;
            f.DeleteTable += f_DeleteTable;

            
            f.ShowDialog();
        }

        void f_DeleteCategory(object sender, EventArgs e)
        {
            LoadCategory();
        }

        void f_UpdateCategory(object sender, EventArgs e)
        {
            LoadCategory();
        }

        void f_InsertCategory(object sender, EventArgs e)
        {
            LoadCategory();
        }

        void f_DeleteFood(object sender, EventArgs e)
        {
            LoadFoodListByCategoryID((cbCategory.SelectedItem as Category).ID);
            if (lsvBill.Tag != null)
                ShowBill((lsvBill.Tag as Table).ID);
            LoadTable();
        }

        void f_UpdateFood(object sender, EventArgs e)
        {
            LoadFoodListByCategoryID((cbCategory.SelectedItem as Category).ID);
            if (lsvBill.Tag != null)
                ShowBill((lsvBill.Tag as Table).ID);
        }

        void f_InsertFood(object sender, EventArgs e)
        {
            LoadFoodListByCategoryID((cbCategory.SelectedItem as Category).ID);
            if (lsvBill.Tag != null)
                ShowBill((lsvBill.Tag as Table).ID);
        }

        void f_DeleteTable(object sender, EventArgs e)
        {
            LoadTable();
        }

        void f_UpdateTable(object sender, EventArgs e)
        {
            LoadTable();
        }

        void f_InsertTable(object sender, EventArgs e)
        {
            LoadTable();
        }


        private void cbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            int id = 0;

            ComboBox cb = sender as ComboBox;

            if (cb.SelectedItem == null)
                return;

            Category selected = cb.SelectedItem as Category;
            id = selected.ID;

            LoadFoodListByCategoryID(id);
        }

        void UpdadteListTable(int idTable, string status)
        {
            Table item = tableList.Where(x => x.ID == idTable).SingleOrDefault();
            item.Status = status;
        }

        private void btnAddFood_Click(object sender, EventArgs e)
        {
            Table table = lsvBill.Tag as Table;
            if (table == null)
            {
                MessageBox.Show("Bạn chưa chọn bàn!");
                return;
            }

            int idBill = BillDAO.Instance.GetUncheckBillIDByTableID(table.ID);
            int foodID = (cbFood.SelectedItem as Food).ID;
            int count = (int)nmFoodCount.Value;

            if (idBill == -1)
            {
                BillDAO.Instance.InsertBill(table.ID);
                BillInfoDAO.Instance.InsertBillInfo(BillDAO.Instance.GetMaxIDBill(), foodID, count);
            }
            else
            {
                BillInfoDAO.Instance.InsertBillInfo(idBill, foodID, count);
            }
            UpdadteListTable(table.ID, "Có người");
            ShowBill(table.ID);
            LoadTableId(table.ID);
            //LoadTable();
        }
        private void btnCheckOut_Click(object sender, EventArgs e)
        {
            Table table = lsvBill.Tag as Table;

            int idBill = BillDAO.Instance.GetUncheckBillIDByTableID(table.ID);
            int discount = (int)nmDiscount.Value;

            double totalPrice = Convert.ToDouble(txtTotalPrice.Text.Split(',')[0]);
            // string item = txtTotalPrice.Text.Substring(0, txtTotalPrice.Text.Length - 5);     xử lý tổng tiền
            double finalTotalPrice = totalPrice - (totalPrice / 100) * discount;

            if (idBill != -1)
            {
                if (MessageBox.Show(string.Format("Bạn có chắc thanh toán hóa đơn cho bàn {0}\nTổng tiền - (Tổng tiền / 100) x Giảm giá\n=> {1} - ({1} / 100) x {2} = {3}", table.Name, totalPrice, discount, finalTotalPrice), "Thông báo", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                {
                    BillDAO.Instance.CheckOut(idBill, discount, (float)finalTotalPrice);
                    UpdadteListTable(table.ID, "Trống");
                    ShowBill(table.ID);
                    LoadTableId(table.ID);
                    //LoadTable();
                }
            }
        }
        private void btnSwitchTable_Click(object sender, EventArgs e)
        {

            int id1 = (lsvBill.Tag as Table).ID;

            int id2 = (cbSwitchTable.SelectedItem as Table).ID;
            if (MessageBox.Show(string.Format("Bạn có thật sự muốn chuyển bàn {0} qua bàn {1}", (lsvBill.Tag as Table).Name, (cbSwitchTable.SelectedItem as Table).Name), "Thông báo", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                TableDAO.Instance.SwitchTable(id1, id2);

                LoadTable();
            }
        }

        #endregion

        private void thanhToánToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnCheckOut_Click(this, new EventArgs());
        }

        private void thêmMónĂnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnAddFood_Click(this, new EventArgs());
        }

        private void txtTotalPrice_TextChanged(object sender, EventArgs e)
        {
            
                txtTotalPrice.Text = txtTotalPrice.Text.Replace(" ", "");
                String temp = txtTotalPrice.Text;

                if (temp.Length == 0)
                {
                    return;
                }

                if (temp.Length <= 4)
                {
                    temp = temp.Replace(",", "");
                }

                if (temp.Length > 3 && isTextChanged)
                {
                    temp = String.Format("{0:0,000}", Double.Parse(temp));
                    isTextChanged = false;
                    txtTotalPrice.Text = temp;
                }
                else
                {
                    txtTotalPrice.Text = temp;
                    isTextChanged = true;
                }

                txtTotalPrice.SelectionStart = txtTotalPrice.Text.Length;
            
            
        }



    }
}
