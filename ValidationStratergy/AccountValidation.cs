using IAccountInterface;
using Stratergy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValidationStratergy
{
    public class AdminAllValidation : IValidationStratergy<IAccount>
    {

        public bool Validate(IAccount obj)
        {
            if (obj.UserName.Length == 0)
            {
                return false;
                throw new Exception("UserName is required");
                
            }
            if (obj.DisplayName.Length == 0)
            {
                return false;
                throw new Exception("UserName is required");
            }
            if (obj.Email.Length == 0)
            {
                return false;
                throw new Exception("Phone number is required");
            }
            if (obj.Phone.Length == 0)
            {
                return false;
                throw new Exception("Phone number is required");
            }
            if (obj.CMND.Length == 0)
            {
                return false;
                throw new Exception("UserName is required");
            }
            if (obj.Address.Length == 0)
            {
                return false;
                throw new Exception("Bill Amount is required");
            }
            if (obj.Birthday > DateTime.Now)
            {
                return false;
                throw new Exception("Bill date  is not proper");
            }
            return true;
        }
    }

    public class CashierValidation : IValidationStratergy<IAccount>
    {
        public bool Validate(IAccount obj)
        {
            if (obj.UserName.Length == 0)
            {
                return false;
                throw new Exception("UserName is required");
            }
            if (obj.DisplayName.Length == 0)
            {
                return false;
                throw new Exception("UserName is required");
            }
            if (obj.Email.Length == 0)
            {
                return false;
                throw new Exception("Phone number is required");
            }
            if (obj.Phone.Length == 0)
            {
                return false;
                throw new Exception("Phone number is required");
            }
            if (obj.CMND.Length == 0)
            {
                return false;
                throw new Exception("UserName is required");
            }
            if (obj.Address.Length == 0)
            {
                return false;
                throw new Exception("Bill Amount is required");
            }
            if (obj.Birthday <= DateTime.Now)
            {
                return false;
                throw new Exception("Bill date  is not proper");
            }
            return true;
        }
    }

    public class StaffValidation : IValidationStratergy<IAccount>
    {
        public bool Validate(IAccount obj)
        {
            if (obj.UserName.Length == 0)
            {
                return false;
                throw new Exception("UserName is required");
            }
            if (obj.DisplayName.Length == 0)
            {
                return false;
                throw new Exception("DisplayName is required");
            }
            
            if (obj.Phone.Length == 0)
            {
                return false;
                throw new Exception("Phone number is required");
            }
            
            
            if (obj.Birthday > DateTime.Now)
            {
                return false;
                throw new Exception("Birthday is not proper");
            }
            return true; ;
        }
    }
}
