﻿using FactoryAccount;
using IAccountInterface;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoDAO
{
    public abstract class AdoAbstractTemplate<IBo> : AbstractDAO.AbstractDal<IBo>
    {

        SqlConnection objConnection;
        protected SqlCommand objCommand;

        void Open()
        {

            objConnection = new SqlConnection(@"Data Source=DESKTOP-ME1AATN\SQL;Initial Catalog=QuanLyQuanCafe2;Integrated Security=True");
            objConnection.Open();
            objCommand = new SqlCommand();
            objCommand.Connection = objConnection;
        }
        void Close()
        {
            objConnection.Close();

        }
        protected abstract void ExecuteQuery(IBo obj);
        protected abstract List<IBo> ExecuteNonQuery(string query);
        // Design pattern :- Template pattern
        public void Execute(IBo obj)
        {
            Open();
            ExecuteQuery(obj);
            Close();
        }
        public List<IBo> ExecuteRead(string query)
        {
            List<IBo> obj = new List<IBo>();
            Open();
            obj = ExecuteNonQuery(query);
            Close();
            return obj;
        }
        public override void Save()
        {
            foreach (IBo i in Collection)
            {
                Execute(i);
            }
        }
    }

    public class AccountDAO : AdoAbstractTemplate<IAccount>
    {

        public override List<IAccount> Get(string query)
        {
            return ExecuteRead(query);
        }
        protected override List<IAccount> ExecuteNonQuery(string query)
        {
            List<IAccount> listIAccount = new List<IAccount>();
            //objCommand.CommandText = "select * from Account";
            objCommand.CommandText = query;
            SqlDataReader rd = objCommand.ExecuteReader();
            while (rd.Read())
            {
                IAccount i = Factory<IAccount>.Create(rd["Type"].ToString());
                i.Type = rd["Type"].ToString();
                i.UserName = rd["UserName"].ToString();
                i.Password = rd["Password"].ToString();
                i.DisplayName = rd["DisplayName"].ToString();
                i.GioiTinh = (bool)rd["GioiTinh"];
                i.Address = rd["Address"].ToString();
                i.Phone = rd["Phone"].ToString();
                i.CMND = rd["CMND"].ToString();
                i.ImageID = rd["ImageID"].ToString();
                i.Birthday = Convert.ToDateTime(rd["Birthday"].ToString());
                i.Id = Convert.ToInt16(rd["Id"].ToString());
                i.Email = rd["Email"].ToString();
                i.IsUsed = (bool)rd["IsUsed"];
                listIAccount.Add(i);
            }
            return listIAccount;
        }
        protected override void ExecuteQuery(IAccount obj)
        {
            //objCommand.CommandText = string.Format("INSERT dbo.Account( UserName , PassWord , DisplayName , Phone, ImageID, Address, GioiTinh, CMND, Birthday, Email, Type , IsUsed ) " +
            //                                                            " VALUES  ( N'{0}' , N'{1}' , N'{2}' , N'{3}' , N'{4}', N'{5}', N'{6}', N'{7}', N'{8}', N'{9}', N'{10}', N'{11}' ) ",
            //                                                            obj.UserName, obj.Password, obj.DisplayName, obj.Phone, obj.ImageID, obj.Address, obj.GioiTinh, obj.CMND, obj.Birthday, obj.Email, obj.Type, obj.IsUsed);

            objCommand.CommandText = string.Format("EXEC dbo.USP_UpdateAccount @idAccount = {0} , @userName = N'{1}' , @displayName = N'{2}' , @password = N'{3}' , @newPassword = N'{4}' , @type = N'{5}' , @phone = '{6}' , @GioiTinh = {7} , @Address = N'{8}' , @CMND = '{9}' , @ImageId = N'{10}' , @Birthday = '{11}' , @Email = N'{12}' , @IsUsed = {13} ",
                                                                                         obj.Id, obj.UserName, obj.DisplayName, obj.Password, obj.Password, obj.Type, obj.Phone, obj.GioiTinh, obj.Address, obj.CMND, obj.ImageID, obj.Birthday, obj.Email, obj.IsUsed);
            objCommand.ExecuteNonQuery();
        }
    }
}
