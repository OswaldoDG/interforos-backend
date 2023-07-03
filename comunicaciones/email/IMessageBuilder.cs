using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace comunicaciones.email
{
    public interface IMessageBuilder
    {
        string FromTemplate(string template, string jsonData);
    }
}
