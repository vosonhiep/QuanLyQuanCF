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
    public partial class fAccountProfile : Form
    {
        private IAccount loginAccount;
        AccountUiFacade Fac = new AccountUiFacade("AdoAccDAO");
        public IAccount LoginAccount
        {
            get { return loginAccount; }
            set { loginAccount = value; ChangeAccount(loginAccount); }
        }
        public fAccountProfile(IAccount acc)
        {
            InitializeComponent();
            LoginAccount = acc;
        }

        void ChangeAccount(IAccount acc)
        {
            txbAccountID.Text = acc.Id.ToString();
            txbUsername.Text = acc.UserName;
            txbDisPlayName.Text = acc.DisplayName;
            txbPhone.Text = acc.Phone;
            if (acc.GioiTinh == true)
                cbGioiTinh.SelectedIndex = 0;
            else
                cbGioiTinh.SelectedIndex = 1;
            dateBirthday.Value = acc.Birthday;
            txbEmail.Text = acc.Email;
            txbCMND.Text = acc.CMND;
            txbAddress.Text = acc.Address;
        }


        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        bool UpdateAcc()
        {
            IAccount icust = Fac.Get(LoginAccount.Type);
            icust.Id = Convert.ToInt32(txbAccountID.Text);
            icust.UserName = txbUsername.Text;
            icust.DisplayName = txbDisPlayName.Text;
            
            //icust.Password = txbNewPassword.Text;
            //icust.newpass = txbNewPassword.Text;
            //icust.reenterPass = txbReEnterPass.Text;
            icust.Phone = txbPhone.Text;
            if (cbGioiTinh.SelectedIndex == 0)
                icust.GioiTinh = true;
            else
                icust.GioiTinh = false;
            icust.Birthday = dateBirthday.Value;
            icust.Email = txbEmail.Text;
            icust.CMND = txbCMND.Text;
            icust.Address = txbAddress.Text;
            icust.Password = LoginAccount.Password;
            icust.Type = LoginAccount.Type;
            icust.ImageID = LoginAccount.ImageID;
            icust.IsUsed = LoginAccount.IsUsed;
            if (icust.Validate() == false)
                return false;
            Fac.Save(icust);
            return true;
        }


        private event EventHandler<AccountEvent> updateAccount;
        public event EventHandler<AccountEvent> UpdateAccount
        {
            add { updateAccount += value; }
            remove { updateAccount -= value; }
        }
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if(UpdateAcc())
                MessageBox.Show("Cập nhật thông tin thành công!");
        }

        string ValidatePass()
        {
            byte[] temp = ASCIIEncoding.ASCII.GetBytes(txbPassword.Text);
            byte[] hasData = new MD5CryptoServiceProvider().ComputeHash(temp);

            string hasPass = "";
            foreach (byte item in hasData)
            {
                hasPass += item;
            }
            if (!hasPass.Equals(LoginAccount.Password))
                return "Mật khẩu cũ không chính xác!\n";
            if (!txbNewPassword.Text.Equals(txbReEnterPass.Text))
                return "Xác nhận mật khẩu không chinh xác";
            return string.Empty;
        }
        private void btnChangePass_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(ValidatePass()))
            {
                IAccount icust = Fac.Get(LoginAccount.Type);
                icust.Id = Convert.ToInt32(txbAccountID.Text);
                icust.UserName = txbUsername.Text;
                icust.DisplayName = txbDisPlayName.Text;

                byte[] temp = ASCIIEncoding.ASCII.GetBytes(txbNewPassword.Text);
                byte[] hasData = new MD5CryptoServiceProvider().ComputeHash(temp);

                string hasPass = "";
                foreach (byte item in hasData)
                {
                    hasPass += item;
                }

                icust.Password = hasPass;
                //icust.newpass = txbNewPassword.Text;
                //icust.reenterPass = txbReEnterPass.Text;
                icust.Phone = txbPhone.Text;
                if (cbGioiTinh.SelectedIndex == 0)
                    icust.GioiTinh = true;
                else
                    icust.GioiTinh = false;
                icust.Birthday = dateBirthday.Value;
                icust.Email = txbEmail.Text;
                icust.CMND = txbCMND.Text;
                icust.Address = txbAddress.Text;
                //icust.Password = LoginAccount.Password;
                icust.Type = LoginAccount.Type;
                icust.ImageID = LoginAccount.ImageID;
                icust.IsUsed = LoginAccount.IsUsed;
                if (icust.Validate() == false)
                    return;
                Fac.Save(icust);

                MessageBox.Show("Đổi mật khẩu thành công!");
            }
        }
    }

    public class AccountEvent : EventArgs
    {
        private Account acc;

        public Account Acc
        {
            get { return acc; }
            set { acc = value; }
        }

        public AccountEvent(Account acc)
        {
            this.Acc = acc;  
        }
    }
}
