using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToyCommunication.Domain.Exceptions.Networks
{
    public class PortNotOpenException : Exception
    {
        public PortNotOpenException(string message) : base(message)
        {
        }
    }
}
