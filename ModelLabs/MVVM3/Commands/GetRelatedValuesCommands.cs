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

namespace MVVM3.Commands
{
    public class GetRelatedValuesCommands
    {
        private ModelResourcesDesc modelResourcesDesc = new ModelResourcesDesc();

        public GetRelatedValuesCommands() { }

        protected INetworkModelGDAContract Proxy
        {
            get { return ProxyConnector.Instance.GetProxy(); }
        }

        // Method to get gids per dms model code
        public ObservableCollection<long> GetGIDs(DMSType modelCode)
        {
            Messenger.Default.Send(new StatusMessage("Getting global identificators method started", "SteelBlue"));

            int iteratorId = 0;
            List<long> ids = new List<long>();

            try
            {
                int numberOfResources = 2;
                int resourcesLeft = 0;

                List<ModelCode> properties = modelResourcesDesc.GetAllPropertyIds(modelCode);

                iteratorId = Proxy.GetExtentValues(modelResourcesDesc.GetModelCodeFromType(modelCode), properties);
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

                Messenger.Default.Send(new StatusMessage("Getting extent values for " + modelCode + " method successfully finished. Fetched " + ids.Count + " samples.", "SeaGreen"));
            }
            catch (Exception e)
            {
                string message = string.Format("Getting extent values method failed for {0}. {1}", modelCode, e.Message);
                Messenger.Default.Send(new StatusMessage(message, "Firebrick"));
            }

            ObservableCollection<long> gids = new ObservableCollection<long>(ids);

            return gids;
        }

        // Method to get by values
        public ObservableCollection<PropertiesView> GetRelatedValues(long globalId, List<ModelCode> props, Association association, ModelCode requestedEntityType)
        {
            Messenger.Default.Send(new StatusMessage("Getting related values method started", "SteelBlue"));

            int iteratorId = 0;
            List<long> ids = new List<long>();
            ObservableCollection<PropertiesView> data = new ObservableCollection<PropertiesView>();

            try
            {
                int numberOfResources = 2;
                int resourcesLeft = 0;

                iteratorId = Proxy.GetRelatedValues(globalId, props, association);
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
                    data.Add(new PropertiesView() { ParentElementName = requestedEntityType, Properties = entity });
                }

                Messenger.Default.Send(new StatusMessage("Getting related values for " + requestedEntityType.ToString() + " method successfully finished. Fetched " + ids.Count + " samples.", "SeaGreen"));

            }
            catch (Exception)
            {
                Messenger.Default.Send(new StatusMessage("Service can't fetch related values right now!", "Firebrick"));
            }

            return data;
        }
    }
}