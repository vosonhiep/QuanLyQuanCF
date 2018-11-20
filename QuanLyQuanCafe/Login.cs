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
            //return Fac.GetAccounts(string.Format("select * from Account where Username = N'{0}' and IsUsed = 'true'", userName));
            listAcc = Fac.GetAccounts("select * from Account where IsUsed = 'true'");
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

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
