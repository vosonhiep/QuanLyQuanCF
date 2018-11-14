using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyQuanCafe.DTO
{
    public class Account
    {

        public Account(int id, string userName, string displayName, string type, string password = null)
        {
            this.ID = id;
            this.UserName = userName;
            this.DisplayName = displayName;
            this.Type = type;
            this.Password = password;
            this.IsUsed = true;
        }

        public Account(DataRow row)
        {
            this.ID = (int)row["ID"];
            this.UserName = row["userName"].ToString();
            this.DisplayName = row["displayName"].ToString();
            this.Type = row["type"].ToString();
            this.Password = row["password"].ToString();
            this.IsUsed = (bool)row["isUsed"];
        }
        private string userName;

        private int iD;

        public int ID
        {
            get { return iD; }
            set { iD = value; }
        }
        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }
        private string displayName;

        public string DisplayName
        {
            get { return displayName; }
            set { displayName = value; }
        }
        private string password;

        public string Password
        {
            get { return password; }
            set { password = value; }
        }
        private string type;

        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        private bool isUsed;

        public bool IsUsed
        {
            get { return isUsed; }
            set { isUsed = value; }
        }
    }
}
