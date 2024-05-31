using FTN.Common;
using MVVM3.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM3.Model
{
    public class PropertiesView : BindableBase
    {
        private ModelCode parentElementName;
        private List<PropertyView> properties = new List<PropertyView>();

        public PropertiesView() { }

        public ModelCode ParentElementName
        {
            get => parentElementName;
            set
            {
                if (parentElementName != value)
                {
                    parentElementName = value;
                    OnPropertyChanged("ParentElementName");
                }
            }
        }

        public List<PropertyView> Properties
        {
            get => properties;
            set
            {
                if(properties != value)
                {
                    properties = value;
                    OnPropertyChanged("Properties");
                }
            }
        }
    }
}
