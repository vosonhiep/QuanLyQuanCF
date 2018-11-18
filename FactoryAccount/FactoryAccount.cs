using AccountLibrary;
using IAccountInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Injection;
using ValidationStratergy;

namespace FactoryAccount
{
    public static class Factory<AnyType>
    {
        static IUnityContainer container = null;

        public static AnyType Create(string Type)
        {
            if (container == null)
            {
                container = new UnityContainer();

                container.RegisterType<IAccount, Admin>("Admin",
                                new InjectionConstructor(new AdminAllValidation()));
                container.RegisterType<IAccount, Cashier>("Cashier",
                                    new InjectionConstructor(new CashierValidation()));
                container.RegisterType<IAccount, Staff>("Staff",
                                    new InjectionConstructor(new StaffValidation()));

            }
            return container.Resolve<AnyType>(Type.ToString());
        }

    }
}
