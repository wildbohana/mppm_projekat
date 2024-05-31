using FTN.Common;
using MVVM3.Commands;
using MVVM3.Helpers;
using MVVM3.Model;
using MVVMLight.Messaging;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MVVM3.ViewModel
{
    public class GetExtentValuesViewModel : BindableBase
    {
        public MyICommand ClearProperties { get; set; }
        public MyICommand ResetAll { get; set; }
        public MyICommand GetExtentValuesCriteria { get; set; }
        private GetExtentValuesCommands commands = new GetExtentValuesCommands();

        private List<DMSType> types = new List<DMSType>();
        private ObservableCollection<ModelCode> properties = new ObservableCollection<ModelCode>();
        private ObservableCollection<PropertiesView> listedEntities = new ObservableCollection<PropertiesView>();

        private DMSType selectedType;

        public ObservableCollection<ModelCode> ModelList { get; set; }
        public ObservableCollection<ModelCode> SelectedModels { get; set; }

        public GetExtentValuesViewModel()
        {
            // Show all dms types in combobox
            if (types != null && types.Count == 0)
            {
                Types = MainWindowViewModel.modelResourcesDesc.AllDMSTypes.ToList();
                Types.Remove(DMSType.MASK_TYPE);
            }

            ModelList = new ObservableCollection<ModelCode>();
            SelectedModels = new ObservableCollection<ModelCode>();

            ClearProperties = new MyICommand(ClearPropertiesCollection);
            ResetAll = new MyICommand(ResetAllForm);
            GetExtentValuesCriteria = new MyICommand(GetValuesFromNMSCriteria);

            ResetAllForm();
        }

        private void ClearPropertiesCollection()
        {
            SelectedModels.Clear();
        }

        private void ResetAllForm()
        {
            SelectedModels.Clear();
            SelectedType = (DMSType)1;
            ListedEntities.Clear();
            Messenger.Default.Send(new StatusMessage("Criteria has been resetted.", "SteelBlue"));
        }

        public void GetValuesFromNMSCriteria()
        {
            if (SelectedType == DMSType.MASK_TYPE)
            {
                Messenger.Default.Send(new StatusMessage("You didn't choose a DMS type!", "Firebrick"));
                return;
            }
            if (SelectedModels.Count == 0)
            {
                Messenger.Default.Send(new StatusMessage("You must choose atleast one property!", "Firebrick"));
                return;
            }

            Messenger.Default.Send(new StatusMessage("Executing query. Please wait...", "SteelBlue"));
            ListedEntities = commands.GetExtentValues(SelectedType, SelectedModels.ToList());
        }

        public ObservableCollection<PropertiesView> ListedEntities
        {
            get => listedEntities;
            set
            {
                if (value != listedEntities)
                {
                    listedEntities = value;
                    OnPropertyChanged("ListedEntities");
                }
            }
        }

        public System.Collections.IList SelectedItems
        {
            get
            {
                return SelectedModels;
            }
            set
            {
                foreach (ModelCode model in value)
                {
                    if (!SelectedModels.Contains(model))
                    {
                        SelectedModels.Add(model);
                    }
                }
            }
        }

        public List<DMSType> Types
        {
            get => types;
            set
            {
                if (types != value)
                {
                    types = value;
                    OnPropertyChanged("Types");
                }

            }
        }

        public DMSType SelectedType
        {
            get => selectedType;
            set
            {
                if (selectedType != value)
                {
                    selectedType = value;
                    OnPropertyChanged("SelectedType");

                    // Get all global ids from service via proxy
                    try
                    {
                        Properties = new ObservableCollection<ModelCode>(
                            MainWindowViewModel.modelResourcesDesc.GetAllPropertyIds(SelectedType));
                        SelectedModels.Clear();
                        ListedEntities.Clear();
                    }
                    catch { }
                }
            }
        }

        public ObservableCollection<ModelCode> Properties
        {
            get => properties;
            set
            {
                if (properties != value)
                {
                    properties = value;
                    OnPropertyChanged("Properties");
                }
            }
        }
    }
}
