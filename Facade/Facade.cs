using FactoryAccount;
using FactoryDAO;
using IAccountInterface;
using IDAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facade
{
    public class AccountUiFacade
    {
        IDataLayer<IAccount> dao;
        IAccount iAcc;
        private AccountBase custOld; // Design pattern :- Memento pattern
        List<IAccount> custcoll;
        int SelectedIndex = 0;
        public AccountUiFacade(string DaoType)
        {
            dao = FactoryDAO<IAccount>.getDao(DaoType);
        }
        public IAccount Get(string Type)
        {
            return Factory<IAccount>.Create(Type);
        }
        public IAccount Revert()
        {
            return custcoll[SelectedIndex].Clone();
        }
        public IAccount Select(int Index)
        {
            SelectedIndex = Index;
            return custcoll[Index].Clone();

        }
        public List<IAccount> GetAccounts(string query)
        {
            custcoll = dao.Get(query);
            return custcoll;
        }
        public void Save(IAccount Base)
        {
            // Design pattern :- Facade pattern
            Base.Validate();
            dao.Add(Base);
            dao.Save();
        }
    }
}
