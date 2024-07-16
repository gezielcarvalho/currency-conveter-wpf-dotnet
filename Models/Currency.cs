using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace currency_converter_wpf_dotnet.Models
{
    internal class Currency
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public double Rate { get; set; }
    }
}
