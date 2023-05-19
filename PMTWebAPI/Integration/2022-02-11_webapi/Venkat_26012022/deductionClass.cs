using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMTWebAPI.DAL
{
    public class deductionClass
    {
        public string deductionuid { get; set; }
        public string deductionMode { get; set; }
        public string Value { get; set; }
        
    }
    public class deductionListClass
    {
        public List<deductionClass> deductionList;
    }
    
}