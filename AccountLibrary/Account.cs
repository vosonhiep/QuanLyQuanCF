using IAccountInterface;
using Stratergy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountLibrary
{
    public class Admin : AccountBase
    {
        public Admin(IValidationStratergy<IAccount> obj)  : base(obj)
        {
            
        }
        public override string Type
        {
            get
            {
                return "Admin";
            }
            set
            {
                _type = value;
            }
        }
    }
    public class Cashier : AccountBase
    {
        public Cashier(IValidationStratergy<IAccount> obj): base(obj)
        {

        }
        public override string Type
        {
            get
            {
                return "Cashier";
            }
            set
            {
                _type = value;
            }
        }
       
    }
    public class Staff : AccountBase
    {
        public Staff(IValidationStratergy<IAccount> obj): base(obj)
        {

        }
        public override string Type
        {
            get
            {
                return "Staff";
            }
            set
            {
                _type = value;
            }
        }
       
    }
}
