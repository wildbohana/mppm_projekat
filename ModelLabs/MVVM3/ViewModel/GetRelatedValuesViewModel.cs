using FTN.Common;
using MVVM3.Commands;
using MVVM3.Helpers;
using MVVM3.Model;
using MVVMLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Markup;

namespace MVVM3.ViewModel
{
    public class GetRelatedValuesViewModel : BindableBase
    {
        public MyICommand ClearProperties { get; set; }
        public MyICommand ResetAll { get; set; }
        public MyICommand GetRelatedValuesCriteria { get; set; }
        private GetRelatedValuesCommands commands = new GetRelatedValuesCommands();

        private List<DMSType> types = new List<DMSType>();
        private ObservableCollection<long> gids = new ObservableCollection<long>();
        private ObservableCollection<ModelCode> typeModelCodes = new ObservableCollection<ModelCode>();
        private ObservableCollection<PropertiesView> listedProperties = new ObservableCollection<PropertiesView>();
        private ObservableCollection<ModelCode> properties = new ObservableCollection<ModelCode>();

        private ObservableCollection<ModelCode> references = new ObservableCollection<ModelCode>();
        private ModelCode selectedReference;
        private ModelCode selectedTypeModelCode = 0;

        private DMSType selectedType;
        private long selectedGid = -1;

        public ObservableCollection<ModelCode> ModelList { get; set; }
        public ObservableCollection<ModelCode> SelectedModels { get; set; }

        public GetRelatedValuesViewModel()
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

            RemoveDMSTypesWithoutReferences();

            GetRelatedValuesCriteria = new MyICommand(GetValuesFromNMSCriteria);

            ResetAllForm();
        }

        public void GetValuesFromNMSCriteria()
        {
            if (SelectedType == DMSType.MASK_TYPE)
            {
                Messenger.Default.Send(new StatusMessage("You didn't choose a DMS type!", "Firebrick"));
                return;
            }
            if (SelectedGid == -1)
            {
                Messenger.Default.Send(new StatusMessage("You didn't choose a Global ID!", "Firebrick"));
                return;
            }
            if(SelectedReferenceModelCode == 0)
            {
                Messenger.Default.Send(new StatusMessage("You must choose a valid reference!", "Firebrick"));
                return;
            }
            if (SelectedModels.Count == 0)
            {
                Messenger.Default.Send(new StatusMessage("You must choose atleast one property!", "Firebrick"));
                return;
            }

            Messenger.Default.Send(new StatusMessage("Executing query. Please wait...", "SteelBlue"));
            Association association = new Association() { PropertyId = SelectedReferenceModelCode, Type = 0 };
            ListedProperties = commands.GetRelatedValues(selectedGid, SelectedModels.ToList(), association, selectedReference);
        }

        private void ClearPropertiesCollection()
        {
            SelectedModels.Clear();
        }

        private void RemoveDMSTypesWithoutReferences()
        {
            List<DMSType> types = MainWindowViewModel.modelResourcesDesc.AllDMSTypes.ToList();
            types.Remove(DMSType.MASK_TYPE);

            foreach (DMSType type in types)
            {
                int numberOfReferences = 0;
                foreach (var mc in MainWindowViewModel.modelResourcesDesc.GetAllPropertyIds(type))
                {
                    if ((((long)mc & 0x0000000000000009) == 0x0000000000000009)
                        || ((long)mc & 0x0000000000000019) == 0x0000000000000019)
                    {
                        numberOfReferences++;
                    }
                }

                if (numberOfReferences == 0) // dms type has no references by schema
                {
                    Types.Remove(type);
                }
            }
        }

        private void ResetAllForm()
        {
            SelectedModels.Clear();
            ListedProperties.Clear();
            References.Clear();
            TypeModelCodes.Clear();
            Properties.Clear();
            Gids.Clear();
            SelectedType = (DMSType)1;
            Gids = commands.GetGIDs(SelectedType);
            References = GetReferences();
            SelectedGid = 0;
            SelectedTypeModelCode = 0;
            SelectedReferenceModelCode = 0;
            Messenger.Default.Send(new StatusMessage("Criteria has been resetted.", "SteelBlue"));
        }

        private ObservableCollection<ModelCode> GetConcreteClassesModelCodes()
        {
            if(((short)selectedReference) == 0)
            {
                return new ObservableCollection<ModelCode>();
            }

            ObservableCollection<ModelCode> availableTypeModelCodes = new ObservableCollection<ModelCode>
            {
                MainWindowViewModel.modelResourcesDesc.GetModelCodeFromModelCodeName(SelectedReferenceModelCode.ToString().Split('_')[1])
            };

            return availableTypeModelCodes;
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

        private ObservableCollection<ModelCode> GetReferences()
        {
            ObservableCollection<ModelCode> references = new ObservableCollection<ModelCode>();
            
            foreach (var mc in MainWindowViewModel.modelResourcesDesc.GetAllPropertyIds(SelectedType))
            {
                if ((((long)mc & 0x0000000000000009) == 0x0000000000000009)
                    || ((long)mc & 0x0000000000000019) == 0x0000000000000019)
                {
                    references.Add(mc);
                }
            }
            return references;
        }

        public ModelCode SelectedReferenceModelCode
        {
            get => selectedReference;
            set
            {
                if (selectedReference != value)
                {
                    selectedReference = value;
                    TypeModelCodes.Clear();
                    TypeModelCodes = GetConcreteClassesModelCodes();
                    SelectedModels.Clear();

                    try
                    {
                        // Populate properties for selected reference
                        Properties = new ObservableCollection<ModelCode>(MainWindowViewModel.modelResourcesDesc.GetAllPropertyIds(TypeModelCodes[0]).ToList());
                    }
                    catch
                    {
                        Properties.Clear();
                    }

                    OnPropertyChanged("SelectedReferenceModelCode");
                }
            }
        }

        public ObservableCollection<ModelCode> References
        {
            get => references;
            set
            {
                if (value != references)
                {
                    references = value;
                    OnPropertyChanged("References");
                }
            }
        }

        public ModelCode SelectedTypeModelCode
        {
            get => selectedTypeModelCode;
            set
            {
                if (value != selectedTypeModelCode)
                {
                    selectedTypeModelCode = value;
                    SelectedGid = -1;
                    ListedProperties.Clear();
                    References.Clear();
                    TypeModelCodes.Clear();
                    References = GetReferences();
                    SelectedTypeModelCode = 0;
                    Properties.Clear();
                    SelectedReferenceModelCode = 0;
                    OnPropertyChanged("SelectedTypeModelCode");
                }
            }
        }

        public ObservableCollection<PropertiesView> ListedProperties
        {
            get => listedProperties;
            set
            {
                if (value != listedProperties)
                {
                    listedProperties = value;
                    OnPropertyChanged("ListedProperties");
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

                    // Get all global ids from service via proxy
                    try
                    {
                        Gids.Clear();
                        Gids = commands.GetGIDs(selectedType);
                        TypeModelCodes = new ObservableCollection<ModelCode>(
                            MainWindowViewModel.modelResourcesDesc.GetAllPropertyIds(SelectedType));
                        SelectedModels.Clear();
                        SelectedGid = -1;
                        ListedProperties.Clear();
                        Properties.Clear();
                        References.Clear();
                        References = GetReferences();
                    }
                    catch
                    {
                        if (Gids != null)
                        {
                            Gids.Clear();
                            SelectedGid = -1;
                        }
                    }

                    OnPropertyChanged("SelectedType");
                    OnPropertyChanged("References");
                }
            }
        }

        public long SelectedGid
        {
            get => selectedGid;
            set
            {
                if (selectedGid != value)
                {
                    selectedGid = value;
                    ListedProperties.Clear();
                    OnPropertyChanged("SelectedGid");
                }
            }
        }

        public ObservableCollection<long> Gids
        {
            get => gids;
            private set
            {
                if (gids != value)
                {
                    gids = value;
                    OnPropertyChanged("Gids");
                }
            }
        }

        public ObservableCollection<ModelCode> TypeModelCodes
        {
            get => typeModelCodes;
            set
            {
                if (typeModelCodes != value)
                {
                    typeModelCodes = value;
                    OnPropertyChanged("TypeModelCodes");
                }
            }
        }
    }
}
