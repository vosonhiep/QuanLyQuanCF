using Stratergy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;

namespace IAccountInterface
{
    public interface IBo // Design pattern :- Composite Pattern
    {
        int Id { get; set; }
        bool Validate();
    }
    public abstract class BoBase : IBo
    {
        private int _Id;

        public int Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        public virtual bool Validate()
        {
            throw new NotImplementedException();
            
        }
    }
    public interface IAccount : IBo
    {
        int Id { get; set; }
        string UserName { get; set; }
        string Password { get; set; }
        string DisplayName { get; set; }
        bool GioiTinh { get; set; }
        string Phone { get; set; }
        string Address { get; set; }
        string CMND { get; set; }
        string Email { get; set; }
        DateTime Birthday { get; set; }
        string Type { get; set; }
        string ImageID { get; set; }
        bool IsUsed { get; set; }
        IAccount Clone();
        IValidationStratergy<IAccount> ValidationType { get; }
    }

    public abstract class AccountBase : BoBase, IAccount
    {
        public override bool Validate()
        {
            return ValidationType.Validate(this);
        }
        public void init()
        {
            UserName = "";
            Phone = "";
            DisplayName = "";
            Birthday = DateTime.Now;
            GioiTinh = true;
            IsUsed = true;
            ImageID = "";
            CMND = "";
            Address = "";
        }
        public AccountBase(IValidationStratergy<IAccount> _Validate)
        {
            _ValidationType = _Validate;
            init();
        }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
        public string Phone { get; set; }
        public bool GioiTinh { get; set; }
        public DateTime Birthday { get; set; }
        public string Address { get; set; }
        public bool IsUsed { get; set; }
        public string ImageID { get; set; }
        public string CMND { get; set; }
        public string Email { get; set; }
        
        [Key]
        public int Id { get; set; }
        protected string _type = "B";
        public virtual string Type
        {
            get
            {
                return "B";
            }
            set
            {
                _type = value;
            }
        }
        public IAccount Clone()
        {
            return (AccountBase)this.MemberwiseClone();
            // Design pattern :- Prototype pattern
        }
        private IValidationStratergy<IAccount> _ValidationType = null;
        public IValidationStratergy<IAccount> ValidationType
        {
            get { return _ValidationType; }

        }
    }
}
