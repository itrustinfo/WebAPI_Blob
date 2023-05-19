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

    public class additionClass
    {
        public string additionuid { get; set; }
        public string additionMode { get; set; }
        public string Value { get; set; }

    }
    public class additionListClass
    {
        public List<additionClass> additionList;
    }

    public class deductionListClass
    {
        public List<deductionClass> deductionList;
    }
    
}