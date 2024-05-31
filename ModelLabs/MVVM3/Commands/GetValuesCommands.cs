using FTN.Common;
using FTN.ServiceContracts;
using MVVM3.Model;
using MVVMLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml;

namespace MVVM3.Commands
{
    public class GetValuesCommands
    {
        private ModelResourcesDesc modelResourcesDesc = new ModelResourcesDesc();

        public GetValuesCommands() { }

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
        public ObservableCollection<PropertyView> GetValues(long globalId, List<ModelCode> props)
        {
            try
            {
                ResourceDescription rd = Proxy.GetValues(globalId, props);
                ObservableCollection<PropertyView> data = new ObservableCollection<PropertyView>();
                StringBuilder sb;
                List<Property> Properties = rd.Properties;

                for (int i = 0; i < Properties.Count; i++)
                {
                    switch (Properties[i].Type)
                    {
                        case PropertyType.Float:
                            data.Add(new PropertyView(Properties[i].Id, Properties[i].AsFloat().ToString()));
                            break;
                        case PropertyType.Byte:
                        case PropertyType.Int32:
                        case PropertyType.Int64:
                        case PropertyType.TimeSpan:
                            if (Properties[i].Id == ModelCode.IDOBJ_GID)
                                data.Add(new PropertyView(Properties[i].Id, Properties[i].AsLong().ToString()));
                            else
                                data.Add(new PropertyView(Properties[i].Id, Properties[i].AsLong().ToString()));
                            break;
                        case PropertyType.Bool:
                            data.Add(new PropertyView(Properties[i].Id, Properties[i].AsBool() ? "✓" : "X"));
                            break;
                        case PropertyType.DateTime:
                                data.Add(new PropertyView(Properties[i].Id, Properties[i].AsDateTime().ToString()));
                            break;
                        case PropertyType.Enum:
                            try
                            {
                                data.Add(new PropertyView(Properties[i].Id, new EnumDescs().GetStringFromEnum(Properties[i].Id, Properties[i].AsEnum())));
                            }
                            catch (Exception)
                            {
                                data.Add(new PropertyView(Properties[i].Id, Properties[i].AsEnum().ToString()));
                            }
                            break;
                        case PropertyType.Reference:
                            data.Add(new PropertyView(Properties[i].Id, Properties[i].AsReference().ToString()));
                            break;
                        case PropertyType.String:
                            if (Properties[i].PropertyValue.StringValue == null || Properties[i].PropertyValue.StringValue == "")
                            {
                                Properties[i].PropertyValue.StringValue = " ";
                            }
                            data.Add(new PropertyView(Properties[i].Id, Properties[i].AsString().ToString()));
                            break;
                        case PropertyType.Int64Vector:
                        case PropertyType.ReferenceVector:
                            if (Properties[i].AsLongs().Count > 0)
                            {
                                sb = new StringBuilder(100);
                                for (int j = 0; j < Properties[i].AsLongs().Count; j++)
                                {
                                    sb.Append(String.Format("0x{0:x16}", Properties[i].AsLongs()[j])).Append(", ");
                                }
                                data.Add(new PropertyView(Properties[i].Id, sb.ToString(0, sb.Length - 2)));
                            }
                            else
                            {
                                data.Add(new PropertyView(Properties[i].Id, "empty long/reference vector"));
                            }

                            break;
                        case PropertyType.TimeSpanVector:
                            if (Properties[i].AsLongs().Count > 0)
                            {
                                sb = new StringBuilder(100);
                                for (int j = 0; j < Properties[i].AsLongs().Count; j++)
                                {
                                    sb.Append(String.Format("0x{0:x16}", Properties[i].AsTimeSpans()[j])).Append(", ");
                                }
                                data.Add(new PropertyView(Properties[i].Id, sb.ToString(0, sb.Length - 2)));
                            }
                            else
                            {
                                data.Add(new PropertyView(Properties[i].Id, "empty long/reference vector"));
                            }

                            break;
                        case PropertyType.Int32Vector:
                            if (Properties[i].AsInts().Count > 0)
                            {
                                sb = new StringBuilder(100);
                                for (int j = 0; j < Properties[i].AsInts().Count; j++)
                                {
                                    sb.Append(String.Format("{0}", Properties[i].AsInts()[j])).Append(", ");
                                }
                                data.Add(new PropertyView(Properties[i].Id, sb.ToString(0, sb.Length - 2)));
                            }
                            else
                            {
                                data.Add(new PropertyView(Properties[i].Id, "empty int vector"));
                            }

                            break;
                        case PropertyType.DateTimeVector:
                            if (Properties[i].AsDateTimes().Count > 0)
                            {
                                sb = new StringBuilder(100);
                                for (int j = 0; j < Properties[i].AsDateTimes().Count; j++)
                                {
                                    sb.Append(String.Format("{0}", Properties[i].AsDateTimes()[j])).Append(", ");
                                }

                                data.Add(new PropertyView(Properties[i].Id, sb.ToString(0, sb.Length - 2)));
                            }
                            else
                            {
                                data.Add(new PropertyView(Properties[i].Id, "empty DateTime vector"));
                            }

                            break;
                        case PropertyType.BoolVector:
                            if (Properties[i].AsBools().Count > 0)
                            {
                                sb = new StringBuilder(100);
                                for (int j = 0; j < Properties[i].AsBools().Count; j++)
                                {
                                    sb.Append(String.Format("{0}", Properties[i].AsBools()[j])).Append(", ");
                                }

                                data.Add(new PropertyView(Properties[i].Id, sb.ToString(0, sb.Length - 2)));
                            }
                            else
                            {
                                data.Add(new PropertyView(Properties[i].Id, "empty bool vector"));
                            }

                            break;
                        case PropertyType.FloatVector:
                            if (Properties[i].AsFloats().Count > 0)
                            {
                                sb = new StringBuilder(100);
                                for (int j = 0; j < Properties[i].AsFloats().Count; j++)
                                {
                                    sb.Append(Properties[i].AsFloats()[j]).Append(", ");
                                }

                                data.Add(new PropertyView(Properties[i].Id, sb.ToString(0, sb.Length - 2)));
                            }
                            else
                            {
                                data.Add(new PropertyView(Properties[i].Id, "empty float vector"));
                            }

                            break;
                        case PropertyType.StringVector:
                            if (Properties[i].AsStrings().Count > 0)
                            {
                                sb = new StringBuilder(100);
                                for (int j = 0; j < Properties[i].AsStrings().Count; j++)
                                {
                                    sb.Append(Properties[i].AsStrings()[j]).Append(", ");
                                }

                                data.Add(new PropertyView(Properties[i].Id, sb.ToString(0, sb.Length - 2)));
                            }
                            else
                            {
                                data.Add(new PropertyView(Properties[i].Id, "empty string vector"));
                            }

                            break;
                        case PropertyType.EnumVector:
                            if (Properties[i].AsEnums().Count > 0)
                            {
                                sb = new StringBuilder(100);
                                EnumDescs enumDescs = new EnumDescs();

                                for (int j = 0; j < Properties[i].AsEnums().Count; j++)
                                {
                                    try
                                    {
                                        sb.Append(String.Format("{0}", enumDescs.GetStringFromEnum(Properties[i].Id, Properties[i].AsEnums()[j]))).Append(", ");
                                    }
                                    catch (Exception)
                                    {
                                        sb.Append(String.Format("{0}", Properties[i].AsEnums()[j])).Append(", ");
                                    }
                                }

                                data.Add(new PropertyView(Properties[i].Id, sb.ToString(0, sb.Length - 2)));
                            }
                            else
                            {
                                data.Add(new PropertyView(Properties[i].Id, "empty enum vector"));
                            }

                            break;
                        default:
                            Messenger.Default.Send(new StatusMessage("Service can't fetch values right now!", "Firebrick"));
                            break;
                    }
                }

                Messenger.Default.Send(new StatusMessage("Get values method fetched " + data.Count + " " +
                                         (data.Count <= 1 ? "property" : "properties") + " from service.", "SteelBlue"));
                return data;
            }
            catch (Exception)
            {
                Messenger.Default.Send(new StatusMessage("Service can't fetch values right now!", "Firebrick"));
                return new ObservableCollection<PropertyView>();
            }
        }
    }
}
