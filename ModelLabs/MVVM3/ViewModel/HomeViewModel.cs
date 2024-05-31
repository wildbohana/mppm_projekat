using MVVM3.Commands;
using MVVM3.Helpers;
using MVVM3.Model;
using MVVMLight.Messaging;
using System.Windows.Media;
using System;
using System.IO;

namespace MVVM3.ViewModel
{
    public class HomeViewModel : BindableBase
    {
        private string applicationTitle;
        private string applicationSubtitle;

        public MyICommand ImportXml { get; set; }
        public MyICommand ResetAll { get; set; }

        public HomeViewModel()
        {
            ApplicationTitle = "GDA Client";
            ApplicationSubtitle = "Use navigation to access advanced options";

            ImportXml = new MyICommand(ImportXmlData);
            ResetAll = new MyICommand(ClearData);
        }

        private void ImportXmlData()
        {
            new ImportApplyRemoveDataCommands().ImportData();
        }

        private void ClearData()
        {
            try
            {
                string solutionDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)?.Parent?.Parent?.Parent?.FullName;
                string dataPath = Path.Combine(solutionDirectory, "NetworkModelData.data");

                if (File.Exists(dataPath))
                {
                    File.Delete(dataPath);
                    Messenger.Default.Send(new StatusMessage("NetworkModelData.data file deleted successfully.", "SteelBlue"));
                }
                else
                {
                    Messenger.Default.Send(new StatusMessage("NetworkModelData.data file does not exist.", "Firebrick"));
                }
            }
            catch (Exception ex)
            {
                Messenger.Default.Send(new StatusMessage($"An error occurred: {ex.Message}", "Firebrick"));
            }
        }

        public string ApplicationTitle
        {
            get { return applicationTitle; }
            set
            {
                applicationTitle = value;
                OnPropertyChanged("ApplicationTitle");
            }
        }

        public string ApplicationSubtitle
        {
            get { return applicationSubtitle; }
            set
            {
                applicationSubtitle = value;
                OnPropertyChanged("ApplicationSubtitle");
            }
        }
    }
}
