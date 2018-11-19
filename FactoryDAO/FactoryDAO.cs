using AdoDAO;
using IAccountInterface;
using IDAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace FactoryDAO
{
    public static class FactoryDAO<AnyType>
    {
        static IUnityContainer container;
        public static IDataLayer<AnyType> getDao(string Daotype)
        {
            // Design pattern :- Lazy Loading ( Improve the below code using Lazy keyword).
            if (container == null)
            {
                container = new UnityContainer();
                container.RegisterType<IDataLayer<IAccount>, AccountDAO>("AdoAccDAO");
                //container.RegisterType<IDataLayer<CustomerBase>, CustomerEfDal>("EfCustDal");

                // Design pattern :- RIP ( Replace IF with Polymorphism)
            }
            return (IDataLayer<AnyType>)container.Resolve<IDataLayer<AnyType>>(Daotype);
        }
    }
}
