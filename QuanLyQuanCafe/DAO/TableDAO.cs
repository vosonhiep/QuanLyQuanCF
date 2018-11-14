using QuanLyQuanCafe.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyQuanCafe.DAO
{
    public class TableDAO
    {
        private static TableDAO instance;

        public static TableDAO Instance
        {
            get { if (instance == null) instance = new TableDAO(); return TableDAO.instance; }
            private set { TableDAO.instance = value; }
        }

        public static int TableWidth = 90;
        public static int TableHeight = 90;

        private TableDAO() { }

        public void SwitchTable(int id1, int id2)
        {
            DataProvider.Instance.ExecuteQuery("USP_SwitchTable @idTable1 , @idTabel2", new object[] { id1, id2 });
        }

        public List<Table> LoadTableList()
        {
            List<Table> tableList = new List<Table>();

            DataTable data = DataProvider.Instance.ExecuteQuery("Select * from TableFood where isUsed = 'true'");

            foreach (DataRow item in data.Rows)
            {
                Table table = new Table(item);
                tableList.Add(table);
            }

            return tableList;
        }

        public int GetMaxIdTable()
        {
            return (int)DataProvider.Instance.ExecuteScalar("select MAX(id) from TableFood ");
        }

        public bool InsertTable(string name, string status, bool isUsed)
        {
            string query = string.Format("INSERT dbo.TableFood( name, status, isUsed ) VALUES  ( N'{0}', N'{1}', '{2}')", name, status, isUsed);
            int result = DataProvider.Instance.ExecuteNonQuery(query);
            return result > 0;
        }

        public bool UpdateTable(int idTable, string name, string status)
        {
            string query = string.Format("UPDATE dbo.TableFood SET	name = N'{0}', status = N'{1}' WHERE id = {2}", name, status, idTable);
            int result = DataProvider.Instance.ExecuteNonQuery(query);
            return result > 0;
        }

        public Table GetTableById(int idTable)
        {
            string query = "select * from TableFood where id = " + idTable;

            DataTable data = DataProvider.Instance.ExecuteQuery(query);

            foreach (DataRow item in data.Rows)
            {
                return new Table(item);
            }
            return null;
        }

        public bool TableIsEmpty(int idTable)
        {
            Table item = GetTableById(idTable);
            return item.Status.Equals("Trống") == true ? true : false;
        }

        public bool DeleteTable(int idTable)
        {
            if(TableIsEmpty(idTable))
            {
                string query = string.Format("UPDATE dbo.TableFood SET IsUsed = 'false' WHERE id = " + idTable);
                int result = DataProvider.Instance.ExecuteNonQuery(query); 
                return result > 0;
            }
            return false;
        }

        public int DeleteTableEmpty(int idTable)
        {
            string query = string.Format("Delete dbo.TableFood WHERE id = " + idTable);
            return (int)DataProvider.Instance.ExecuteNonQuery(query);
        }
    }
}
