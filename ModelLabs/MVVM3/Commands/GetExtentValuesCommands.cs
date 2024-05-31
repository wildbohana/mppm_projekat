using FTN.Common;
using FTN.ServiceContracts;
using MVVM3.Model;
using MVVMLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MVVM3.Commands
{
    public class GetExtentValuesCommands
    {
        private ModelResourcesDesc modelResourcesDesc = new ModelResourcesDesc();

        public GetExtentValuesCommands() { }

        protected INetworkModelGDAContract Proxy
        {
            get { return ProxyConnector.Instance.GetProxy(); }
        }

        public ObservableCollection<PropertiesView> GetExtentValues(DMSType dmsModelCode, List<ModelCode> props)
        {
            Messenger.Default.Send(new StatusMessage("Getting extent values method started", "SteelBlue"));

            int iteratorId = 0;
            List<long> ids = new List<long>();
            ObservableCollection<PropertiesView> data = new ObservableCollection<PropertiesView>();
            ModelCode modelCode = modelResourcesDesc.GetModelCodeFromType(dmsModelCode);

            try
            {
                int numberOfResources = 2;
                int resourcesLeft = 0;

                // List<ModelCode> properties = modelResourcesDesc.GetAllPropertyIds(modelCode);

                iteratorId = Proxy.GetExtentValues(modelCode, props);
                resourcesLeft = Proxy.IteratorResourcesLeft(iteratorId);

                while (resourcesLeft > 0)
                {
                    List<ResourceDescription> rds = Proxy.IteratorNext(numberOfResources, iteratorId);

                    for (int i = 0; i < rds.Count; i++)
                    {
                        ids.Add(rds[i].Id);
                    }

                    resourcesLeft = Proxy.IteratorResourcesLeft(iteratorId);
                }

                Proxy.IteratorClose(iteratorId);

                foreach (long gid in ids)
                {

                    List<PropertyView> entity = new GetValuesCommands().GetValues(gid, props).ToList();
                    data.Add(new PropertiesView() { ParentElementName = modelCode, Properties = entity });
                }

                Messenger.Default.Send(new StatusMessage("Getting extent values for " + modelCode + " method successfully finished. Fetched " + ids.Count + " samples.", "SeaGreen"));

            }
            catch (Exception)
            {
                Messenger.Default.Send(new StatusMessage("Service can't fetch extent values right now!", "Firebrick"));
            }

            return data;
        }
    }
}
