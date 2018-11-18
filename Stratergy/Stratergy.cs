using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratergy
{
    public interface IValidationStratergy<AnyType>
    {
        bool Validate(AnyType obj);
    }
}
