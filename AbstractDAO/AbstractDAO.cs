using IDAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDAO
{
    // Design pattern :- Generic design pattern
    public abstract class AbstractDal<Anytype> : IDataLayer<Anytype>
    {

        protected List<Anytype> Collection = new List<Anytype>();
        public void Add(Anytype obj)
        {
            Collection.Add(obj);
        }
        public void Delete(Anytype obj)
        {
            Collection.Remove(obj);
        }

        public virtual void Save()
        {
            throw new NotImplementedException();
        }

        public virtual List<Anytype> Get(string query)
        {
            return Collection;
        }



    }
}
