using Facade;
using IAccountInterface;
using QuanLyQuanCafe.DAO;
using QuanLyQuanCafe.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyQuanCafe
{
    public partial class fLogin : Form
    {
        AccountUiFacade Fac = new AccountUiFacade("AdoAccDAO");
        List<IAccount> listAcc = null;

        public fLogin()
        {
            InitializeComponent();
        }

        private void fLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(MessageBox.Show("Bạn có thật sự muốn thoát chương trình?", "Thông báo", MessageBoxButtons.OKCancel) != System.Windows.Forms.DialogResult.OK)
            {
                e.Cancel = true;
            }
        }

        public bool Login(string userName, string passWord)
        {
            byte[] temp = ASCIIEncoding.ASCII.GetBytes(passWord);
            byte[] hasData = new MD5CryptoServiceProvider().ComputeHash(temp);

            string hasPass = "";
            foreach (byte item in hasData)
            {
                hasPass += item;
            }

            listAcc = Fac.GetAccounts(string.Format("Exec USP_Login @userName = N'{0}' , @passWord = N'{1}'",userName, hasPass));
           return listAcc.Where(x => x.UserName == userName).SingleOrDefault() != null ? true : false;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string userName = txbUsername.Text;
            string passWord = txbPassword.Text;
            if(Login(userName, passWord))
            {
                IAccount loginAccount = listAcc.Where(x => x.UserName == userName).SingleOrDefault();

                fTableManager f = new fTableManager(loginAccount);
                this.Hide();
                f.ShowDialog();
                this.Show();
            }
            else 
            {
                MessageBox.Show("Sai tên tài khoản hoặc mật khẩu!");
            }
        }
    }
}
