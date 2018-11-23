using QuanLyQuanCafe.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyQuanCafe.DAO
{
    public class FoodDAO
    {
        private static FoodDAO instance;

        public static FoodDAO Instance
        {
            get { if (instance == null) instance = new FoodDAO(); return FoodDAO.instance; }
            set { FoodDAO.instance = value; }
        }

        private FoodDAO() { }

        public List<Food> GetFoodByCategoryID(int id)
        {
            List<Food> list = new List<Food>();
            string query = "select * from Food where idCategory = " + id + " and isUsed = 'true'";

            DataTable data = DataProvider.Instance.ExecuteQuery(query);

            foreach (DataRow item in data.Rows)
            {
                Food food = new Food(item);
                list.Add(food);
            }
            return list;
        }

        public List<Food> GetListFood()
        {
            List<Food> list = new List<Food>();
            string query = "select * from Food where isUsed = 'true'";

            DataTable data = DataProvider.Instance.ExecuteQuery(query);

            foreach (DataRow item in data.Rows)
            {
                Food food = new Food(item);
                list.Add(food);
            }
            return list;
        }

        public int GetMaxIdFood()
        {
            return (int)DataProvider.Instance.ExecuteScalar("select MAX(id) from Food ");
            
        }

        public int GetCountFoodByCategoryID(int idCategory)
        {
            return (int)DataProvider.Instance.ExecuteScalar("select COUNT(*) from Food where idCategory = " + idCategory);
        }

        public bool InsertFood(string name, int id, float price)
        {
            string query = string.Format("INSERT dbo.Food( name, idCategory, price ) VALUES  ( N'{0}', {1}, {2})",name, id, price);
            int result = DataProvider.Instance.ExecuteNonQuery(query);
            return result > 0;
        }

        public bool UpdateFood(int idFood, string name, int id, float price)
        {
            string query = string.Format("UPDATE dbo.Food SET	name = N'{0}', idCategory = {1}, price = {2} WHERE id = {3}", name, id, price, idFood);
            int result = DataProvider.Instance.ExecuteNonQuery(query);
            return result > 0;
        }

        public bool DeleteFood(int idFood)
        {
            BillInfoDAO.Instance.DeleteBillInfoByFoodID(idFood);

            string query = string.Format("UPDATE dbo.Food SET IsUsed = 'false' WHERE id = " + idFood);
            int result = DataProvider.Instance.ExecuteNonQuery(query);
            return result > 0;
        }

        public List<Food> SearchFoodByName(string name)
        {
            List<Food> list = new List<Food>();
            string query = string.Format("SELECT * FROM dbo.Food WHERE dbo.fuConvertToUnsign1(name) LIKE N'%' + dbo.fuConvertToUnsign1(N'{0}') + '%'", name);

            DataTable data = DataProvider.Instance.ExecuteQuery(query);

            foreach (DataRow item in data.Rows)
            {
                Food food = new Food(item);
                list.Add(food);
            }
            return list;
        }

        public Food GetFoodByID(int id)
        {
            string query = "select * from Food where id = " + id;

            DataTable data = DataProvider.Instance.ExecuteQuery(query);

            foreach (DataRow item in data.Rows)
            {
                return new Food(item);
            }
            return null;
        }

        public int DeleteFoodEmpty(int idFood)
        {
            string query = string.Format("Delete dbo.Food WHERE id = " + idFood);
            return (int)DataProvider.Instance.ExecuteNonQuery(query);
        }

        /// <summary>
        /// Kiểm tra món ăn có đã được khách gọi chưa
        /// </summary>
        /// <param name="idFood"></param>
        /// <returns></returns>
        public bool IsFoodExistInTable(int idFood)
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
    }
}
