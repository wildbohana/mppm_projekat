using MVVM3.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM3.Model
{
    public class StatusMessage : BindableBase
    {
        private string message;
        private string background_color;

        public StatusMessage(string message, string background_color)
        {
            this.message = message;
            this.background_color = background_color;
        }

        public string Message
        {
            get => message;
            set
            {
                if(message != value)
                {
                    message = value;
                    OnPropertyChanged("Message");
                }
            }
        }

        public string Background_Color
        {
            get => background_color;
            set
            {
                if(background_color != value)
                {
                    background_color = value;
                    OnPropertyChanged("Background_Color");
                }
            }
        }
    }
}
