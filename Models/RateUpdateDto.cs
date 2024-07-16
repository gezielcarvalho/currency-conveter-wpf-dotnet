﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace currency_converter_wpf_dotnet.Models
{
    public class RateUpdateDto
    {
        public string Disclaimer { get; set; }
        public string License { get; set; }
        public int Timestamp { get; set; }
        public string Base { get; set; }
        public Dictionary<string, double> Rates { get; set; }
    }
}
