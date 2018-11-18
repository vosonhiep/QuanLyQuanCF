using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDAO
{
    public interface IDataLayer<AnyType>
    {

        void Add(AnyType obj);
        void Delete(AnyType obj);
        void Save();
        List<AnyType> Get();

    }
}
