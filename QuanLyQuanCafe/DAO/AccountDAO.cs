using QuanLyQuanCafe.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyQuanCafe.DAO
{
    public class AccountDAO
    {
        private static AccountDAO instance;

        public static AccountDAO Instance
        {
            get
            {
                if (instance == null)
                    instance = new AccountDAO();
                return instance;
            }
            set { instance = value; }
        }

        private AccountDAO() { }
        public int Login(string userName, string passWord)
        {
            byte[] temp = ASCIIEncoding.ASCII.GetBytes(passWord);
            byte[] hasData = new MD5CryptoServiceProvider().ComputeHash(temp);

            string hasPass = "";
            foreach (byte item in hasData)
            {
                hasPass += item;
            }
            //var list = hasData.ToString();
            //list.Reverse();

            string query = "USP_Login @userName , @passWord";
            DataTable result = DataProvider.Instance.ExecuteQuery(query, new object[] { userName, hasPass });
            if (result.Rows.Count > 0)
            {
                Account itemp = GetAccountByUserName(userName);
                if (itemp.IsUsed == false)
                    return -1;
                else
                    return 1;
            }
            else
                return 0;
        }

        public bool UpdateAccount(int idAccount, string userName, string displayName, string password, string newPassword)
        {
            int result = DataProvider.Instance.ExecuteNonQuery("exec USP_UpdateAccount @userName , @displayName , @password , @newPassword", new object[] {idAccount, userName, displayName, password, newPassword });
            return result > 0;
        }

        public Account GetAccountByUserName(string userName)
        {
            DataTable data = DataProvider.Instance.ExecuteQuery("Select * from account where userName = '" + userName + "'");

            foreach (DataRow item in data.Rows)
            {
                return new Account(item);
            }

            return null;
        }

        public DataTable GetListAccount()
        {
            return DataProvider.Instance.ExecuteQuery("Select ID, UserName, DisplayName, Type From Account where isUsed = 'true'");
        }


        public bool InsertAccount(string name, string displayName, string type)
        {
            string query = string.Format("INSERT dbo.Account( UserName, DisplayName, Type, Password ) VALUES  ( N'{0}', N'{1}', N'{2}', N'{3}')", name, displayName, type, "1962026656160185351301320480154111117132155");
            int result = DataProvider.Instance.ExecuteNonQuery(query);
            return result > 0;
        }

        public bool UpdateAccount(int idAccount, string username, string displayName, string type)
        {
            string query = string.Format("UPDATE dbo.Account SET DisplayName = N'{1}', Type = N'{2}', UserName = N'{0}' WHERE ID = {3}", username, displayName, type, idAccount);
            int result = DataProvider.Instance.ExecuteNonQuery(query);
            return result > 0;
        }

        public bool DeleteAccount(string userName)
        {    
            string query = string.Format("Update Account set isUsed = 'false' where UserName = N'{0}'", userName);
            int result = DataProvider.Instance.ExecuteNonQuery(query);
            return result > 0;
        }

        public int DeleteAccountEmpty(int idAccount)
        {
            string query = string.Format("Delete dbo.Account WHERE id = " + idAccount);
            return (int)DataProvider.Instance.ExecuteNonQuery(query);
        }

        public bool ResetPssword(string userName)
        {
            string query = string.Format("update Account set password = N'1962026656160185351301320480154111117132155' where UserName = N'{0}'", userName);
            int result = DataProvider.Instance.ExecuteNonQuery(query);
            return result > 0;
        }


        internal int GetMaxIdAccount()
        {
            return (int)DataProvider.Instance.ExecuteScalar("select MAX(id) from Account ");
        }
    }
}
