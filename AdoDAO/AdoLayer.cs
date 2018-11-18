using FactoryAccount;
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

            objConnection = new SqlConnection(@"Data Source=DESKTOP-ME1AATN\SQL;Initial Catalog=QuanLyQuanCafe;Integrated Security=True");
            objConnection.Open();
            objCommand = new SqlCommand();
            objCommand.Connection = objConnection;
        }
        void Close()
        {
            objConnection.Close();

        }
        protected abstract void ExecuteQuery(IBo obj);
        protected abstract List<IBo> ExecuteNonQuery();
        // Design pattern :- Template pattern
        public void Execute(IBo obj)
        {
            Open();
            ExecuteQuery(obj);
            Close();
        }
        public List<IBo> ExecuteRead()
        {
            List<IBo> obj = new List<IBo>();
            Open();
            obj = ExecuteNonQuery();
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

        public override List<IAccount> Get()
        {
            return ExecuteRead();
        }
        protected override List<IAccount> ExecuteNonQuery()
        {
            List<IAccount> o = new List<IAccount>();
            objCommand.CommandText = "select * from Account";
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
                o.Add(i);
            }
            return o;
        }
        protected override void ExecuteQuery(IAccount obj)
        {
            objCommand.CommandText = string.Format("INSERT dbo.Account( UserName , PassWord , DisplayName , Phone, ImageID, Address, GioiTinh, CMND, Birthday, Email, Type , IsUsed ) " +
                                                                        " VALUES  ( N'{0}' , N'{1}' , N'{2}' , N'{3}' , N'{4}', N'{5}', N'{6}', N'{7}', N'{8}', N'{9}', N'{10}', N'{11}' ) ",
                                                                        obj.UserName, obj.Password, obj.DisplayName, obj.Phone, obj.ImageID, obj.Address, obj.GioiTinh, obj.CMND, obj.Birthday, obj.Email, obj.Type, obj.IsUsed);
            objCommand.ExecuteNonQuery();
        }
    }
}
