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
    public partial class fAddFood : Form
    {
        
        public fAddFood()
        {
            InitializeComponent();
            LoadData();
        }

        void LoadData()
        {
            cbFoodCategory.DataSource = CategoryDAO.Instance.GetListCategory();
            cbFoodCategory.DisplayMember = "Name";

            FoodDAO.Instance.InsertFood("", 1, 0);          // Tạo dữ liệu mặc định
            int idFoodNew = 0;
            idFoodNew = FoodDAO.Instance.GetMaxIdFood();
            txbFoodID.Text = idFoodNew.ToString();          // load idNewFood vào textbox ID
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {

            int idFood = Convert.ToInt32(txbFoodID.Text);
            string name = txbFoodName.Text;
            int categoryID = (cbFoodCategory.SelectedItem as Category).ID;
            float price = (float)nmFoodPrice.Value;

            //if (FoodDAO.Instance.UpdateFood(idFood, name, categoryID, price))
            //{
            //    MessageBox.Show("Thêm món thành công");
            //    LoadListFood();
            //    if (insertFood != null)
            //        insertFood(this, new EventArgs());
            //    isAddFood = false;

            //}
            //else
            //{
            //    MessageBox.Show("Có lỗi khi thêm");
            //}
            //dtgvFood.Enabled = false;
        }

    }
}
