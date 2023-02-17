using ClientUIApp.Models;
using FTN.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientUIApp.ViewModels
{
    public class PropertyViewModel : ObservableModel
    {
        private Property _property;
        private string _name;
        private bool _isSelected;


        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                OnPropertyChanged("IsSelected");
                _isSelected = value;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public PropertyViewModel(Property property, bool isSelected)
        {
            _property = property;
            Name = _property.Id.ToString();
            IsSelected = isSelected;
        }

        public ModelCode GetModelCode()
        {
            return _property.Id;
        }
    }
}
