using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Middlewares.Exceptionfeature
{
    public class CustomExecption : Exception
    {
        public CustomExecption(string message) : base(message) { }
    }
}
