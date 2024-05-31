using FTN.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM3.Model
{
    public class PropertyView
    {
        public ModelCode ModelCode { get; set; }
        public string Value { get; set; }

        public PropertyView(ModelCode modelCode, string value)
        {
            ModelCode = modelCode;
            Value = value;
        }
    }
}
