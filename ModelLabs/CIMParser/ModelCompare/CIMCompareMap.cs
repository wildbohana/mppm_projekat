﻿using CIM.Model;
using CIMParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

// Ovo je sve vec bilo ovako

namespace CIM.ModelCompare
{
    public class CIMCompareMap
    {
        #region constants
        /// <summary> "http://iec.ch/TC57/2008/CIM-schema-cim13#" </summary>
        public const string CIM13 = "http://iec.ch/TC57/2008/CIM-schema-cim13#";
        /// <summary> "http://iec.ch/TC57/2009/CIM-schema-cim14#" </summary>
        public const string CIM14 = "http://iec.ch/TC57/2009/CIM-schema-cim14#";
        /// <summary> "http://iec.ch/TC57/2010/CIM-schema-cim15#" </summary>
        public const string CIM15 = "http://iec.ch/TC57/2010/CIM-schema-cim15#";

        private const string singleTab = "\t";
        private const string doubleTab = "\t\t";
        private const string tripleTab = "\t\t\t";
        /// <summary> "&lt;" </summary>
        private const string tagOpen = "<";
        /// <summary> "&gt;" </summary>
        private const string tagClose = ">";
        /// <summary> "&lt;/" </summary>
        private const string closingTagOpen = "</";
        /// <summary> "/&gt;" </summary>
        private const string closingTagClose = "/>";
        /// <summary> "\"" </summary>
        private const string quote = "\"";
        /// <summary> "\"&gt;" </summary>
        private const string quoteAndCloseTag = "\">";
        /// <summary> "=" </summary>
        private const string equalAndQuote = "=\"";
        /// <summary> " rdf:resource=\"" </summary>
        private const string rdfResource = " rdf:resource=\"";
        /// <summary> " rdf:ID=\"" </summary>
        private const string rdfID = " rdf:ID=\"";
        /// <summary> "&lt;!-- " </summary>
        private const string commentBegin = "<!-- ";
        /// <summary> " --&gt;" </summary>
        private const string commentEnd = " -->";
        private const byte dummy = 0;
        #endregion constants
        private Dictionary<string, string> rdfNamespaces = new Dictionary<string, string>();
        private CIMModelSets beforeModel = new CIMModelSets();
        private CIMModelSets afterModel = new CIMModelSets();

        private List<CIMEntity> added = new List<CIMEntity>();
        private List<CIMEntity> removed = new List<CIMEntity>();
        ////                        old        new
        private List<KeyValuePair<CIMEntity, CIMEntity>> updated = new List<KeyValuePair<CIMEntity, CIMEntity>>();

        private List<CIMEntityDifference> modified = new List<CIMEntityDifference>();


        #region Properties
        public CIMModelSets BeforeModel
        {
            get
            {
                return beforeModel;
            }
            set
            {
                beforeModel = value;
            }
        }

        public CIMModelSets AfterModel
        {
            get
            {
                return afterModel;
            }
            set
            {
                afterModel = value;
            }
        }

        public string BeforeModelSourcePath
        {
            get
            {
                if (beforeModel != null)
                {
                    return beforeModel.SourcePath;
                }
                return string.Empty;
            }
            set
            {
                if (beforeModel == null)
                {
                    beforeModel = new CIMModelSets();
                }
                beforeModel.SourcePath = value;
            }
        }

        public string AfterModelSourcePath
        {
            get
            {
                if (afterModel != null)
                {
                    return afterModel.SourcePath;
                }
                return string.Empty;
            }
            set
            {
                if (afterModel == null)
                {
                    afterModel = new CIMModelSets();
                }
                afterModel.SourcePath = value;
            }
        }
        #endregion Properties


        public void Compare()
        {
            added = new List<CIMEntity>();
            removed = new List<CIMEntity>();
            updated = new List<KeyValuePair<CIMEntity, CIMEntity>>();
            FindMissingIDs(beforeModel.ModelMRID, afterModel.ModelMRID, ref removed);
            FindMissingIDs(afterModel.ModelMRID, beforeModel.ModelMRID, ref added);

            //posto posmatramo da je rdf:ID jedinstven gledamo i druge dve mape...
            FindMissingIDs(beforeModel.ModelRDFID, afterModel.ModelRDFID, ref removed);
            FindMissingIDs(afterModel.ModelRDFID, beforeModel.ModelRDFID, ref added);

            FindUpdatedEntities(beforeModel.ModelMRID, afterModel.ModelMRID, ref updated);
            //posto posmatramo da je rdf:ID jedinstven gledamo i druge dve mape...
            FindUpdatedEntities(beforeModel.ModelRDFID, afterModel.ModelRDFID, ref updated);
            FindAttributeDifferences();
            PrintDifferenceModel();
            Console.WriteLine("Differences saved to DifferenceModel.xml file.");
        }

        public void PrintDifferenceModel()
        {
            FileInfo fileInfo = new FileInfo(".//DifferenceModel.xml");
            //// delete the file if it exists.
            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }
            StringBuilder text = new StringBuilder();
            //fileInfo.Create();
            using (StreamWriter streamWriter = fileInfo.CreateText())
            {

                try
                {

                    streamWriter.Write(BuildDefaultCIMRDFDocumentHeaderSnippet());
                    streamWriter.Write(BuildXMLComment("generated by: 'CIM Comparer' application"));

                    streamWriter.WriteLine("<dm:DifferenceModel rdf:about=\"\">");
                    streamWriter.WriteLine("\t<dm:preconditions rdf:parseType=\"Statements\">\n</dm:preconditions>");

                    //// print forward differences
                    streamWriter.WriteLine("<dm:forwardDifferences rdf:parseType=\"Statements\">");
                    foreach (CIMEntity cimEntity in added)
                    {
                        streamWriter.WriteLine("\r\n" + cimEntity.Source.Trim());
                    }
                    foreach (CIMEntityDifference difference in modified)
                    {
                        //// TODO - print samo razlike
                        text.Clear();
                        foreach (string attName in difference.Attributes.Keys)
                        {
                            KeyValuePair<List<ObjectAttribute>, List<ObjectAttribute>> atts = difference.Attributes[attName];
                            foreach (ObjectAttribute att in atts.Value)
                            {
                                ////ako je referenca
                                if (att.IsReference)
                                {
                                    text.AppendFormat("\r\n<{0}:{1} rdf:resource=\"#{2}\"/>", att.NamespaceString, att.FullName, att.ValueOfReference);
                                }
                                else
                                {
                                    text.AppendFormat("\r\n<{0}:{1}>{2}</{0}:{3}>", att.NamespaceString, att.FullName, att.Value, att.FullName);
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(text.ToString()))
                        {
                            text.Insert(0, "\r\n<rdf:Description rdf:about=\"#" + difference.RdfId + "\">");
                            text.AppendFormat("\r\n</rdf:Description>");
                            streamWriter.WriteLine(text.ToString());
                        }
                    }
                    streamWriter.WriteLine("</dm:forwardDifferences>");

                    //// print reverse differences
                    streamWriter.WriteLine("<dm:reverseDifferences rdf:parseType=\"Statements\">");
                    string[] splitter = new string[] { "\r\n" };
                    foreach (CIMEntity cimEntity in removed)
                    {
                        //// TODO - print only rdfid reverseDiffBuilder.AppendLine(cimEntity.Source); 
                        ////<cim:Terminal rdf:ID="ID_TERMINALA"></cim:Terminal> za svaki...
                        string[] sourceLines = cimEntity.Source.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                        streamWriter.WriteLine("\r\n" + sourceLines[0] + (sourceLines.Length == 1 ? string.Empty : sourceLines[sourceLines.Length - 1]));
                    }
                    foreach (CIMEntityDifference difference in modified)
                    {
                        //// TODO - print samo razlike
                        text.Clear();
                        foreach (string attName in difference.Attributes.Keys)
                        {
                            KeyValuePair<List<ObjectAttribute>, List<ObjectAttribute>> atts = difference.Attributes[attName];
                            foreach (ObjectAttribute att in atts.Key)
                            {
                                ////ako je referenca
                                if (att.IsReference)
                                {
                                    text.AppendFormat("\r\n<{0}:{1} rdf:resource=\"#{2}\"/>", att.NamespaceString, att.FullName, att.ValueOfReference);
                                }
                                else
                                {
                                    text.AppendFormat("\r\n<{0}:{1}>{2}</{0}:{3}>", att.NamespaceString, att.FullName, att.Value, att.FullName);
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(text.ToString()))
                        {
                            text.Insert(0, "\r\n<rdf:Description rdf:about=\"#" + difference.RdfId + "\">");
                            text.AppendFormat("\r\n</rdf:Description>");
                            streamWriter.WriteLine(text.ToString());
                        }
                    }
                    streamWriter.WriteLine("</dm:reverseDifferences>");

                    streamWriter.WriteLine("</dm:DifferenceModel>");
                    streamWriter.Write(BuildCIMRDFDocumentFooter());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                }
                finally
                {
                    streamWriter.Close();
                }
            }
        }

        public void AddRDFNamespace(string prefixAndNamespace, string uri)
        {
            if (!string.IsNullOrEmpty(prefixAndNamespace) && !string.IsNullOrEmpty(uri))
            {
                if (!rdfNamespaces.ContainsKey(prefixAndNamespace))
                {
                    rdfNamespaces.Add(prefixAndNamespace, uri);
                }
                else
                {
                    rdfNamespaces[prefixAndNamespace] = uri;
                }
            }
        }

        #region private: Find differences
        /// <summary>
        /// kojih ima u begin a nema u end idu u missing
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="missing"></param>
        private void FindMissingIDs(SortedList<string, CIMEntity> begin, SortedList<string, CIMEntity> end, ref List<CIMEntity> missing)
        {
            foreach (KeyValuePair<string, CIMEntity> pair in begin)
            {
                if (!end.ContainsKey(pair.Key))
                {
                    missing.Add(pair.Value);
                }
            }
        }

        private void FindUpdatedEntities(SortedList<string, CIMEntity> begin, SortedList<string, CIMEntity> end, ref List<KeyValuePair<CIMEntity, CIMEntity>> updated)
        {
            foreach (KeyValuePair<string, CIMEntity> pair in begin)
            {
                if (end.ContainsKey(pair.Key))
                {
                    if (string.Compare(pair.Value.Hash, end[pair.Key].Hash) != 0)
                    {
                        updated.Add(new KeyValuePair<CIMEntity, CIMEntity>(pair.Value, end[pair.Key]));
                    }
                }
            }
        }

        private void FindAttributeDifferences()
        {

            modified = new List<CIMEntityDifference>();
            if (updated != null)
            {
                CIMModelContext context = new CIMModelContext();
                foreach (KeyValuePair<CIMEntity, CIMEntity> pair in updated)
                {
                    CIMObject oldVersion = null;
                    CIMModelLoader.LoadCIMObject(ref oldVersion, context, string.Format("{0}{1}\n</rdf:RDF>", beforeModel.Header, pair.Key.Source));
                    CIMObject newVersion = null;
                    CIMModelLoader.LoadCIMObject(ref newVersion, context, string.Format("{0}{1}\n</rdf:RDF>", afterModel.Header, pair.Value.Source));

                    if ((oldVersion != null) && (newVersion != null))
                    {
                        CIMEntityDifference diff = new CIMEntityDifference();
                        diff.RdfId = oldVersion.ID;
                        //// TODO uporediti atribute u oba objekta i spakovati ih u CIMEntityDifference instancu
                        //PROVERA ZA OLD
                        if (oldVersion.HasAttributes)
                        {
                            foreach (ObjectAttribute oldAtt in oldVersion.MyAttributesAsList)
                            {
                                if (string.Compare(oldAtt.FullName, CIMConstants.AttributeNameIdentifiedObjectMRID) == 0)
                                {
                                    //postavi mrid
                                    diff.MRID = oldAtt.Value;
                                }

                                //uzmem sve sa istim imenom - ako nema dodajem ako ima proveravam
                                if (newVersion.GetAttributeList(oldAtt.FullName) != null)
                                {
                                    if (newVersion.GetAttributeList(oldAtt.FullName).Count == 0)
                                    {
                                        //dodajem
                                        diff.AddAttribute(oldAtt, null);
                                    }
                                    else
                                    {
                                        bool found = false;
                                        if (newVersion.HasAttributes)
                                        {
                                            foreach (ObjectAttribute newAtt in newVersion.GetAttributeList(oldAtt.FullName))
                                            {
                                                //proveravam da li se javlja ista vrednost
                                                if (string.Compare(oldAtt.Value, newAtt.Value) == 0)
                                                {
                                                    //ako se javlja dodajem ga kao nadjen i ne treba da se pojavi
                                                    found = true;
                                                    break;
                                                }
                                            }
                                        }
                                        //ako je found false, znaci da je dodat
                                        if (!found)
                                        {
                                            diff.AddAttribute(oldAtt, null);
                                        }
                                    }
                                }
                                else
                                {
                                    diff.AddAttribute(oldAtt, null);
                                }
                            }
                        }
                        //PROVERA ZA NEW
                        if (newVersion.HasAttributes)
                        {
                            if (newVersion.MyAttributesAsList != null)
                            {
                                foreach (ObjectAttribute newAtt in newVersion.MyAttributesAsList)
                                {
                                    if (string.Compare(newAtt.FullName, CIMConstants.AttributeNameIdentifiedObjectMRID) == 0)
                                    {
                                        //postavi mrid
                                        if (string.IsNullOrEmpty(diff.MRID))
                                        {
                                            diff.MRID = newAtt.Value;
                                        }
                                    }

                                    //uzmem sve sa istim imenom - ako nema dodajem ako ima proveravam
                                    if (oldVersion.GetAttributeList(newAtt.FullName) != null)
                                    {
                                        if (oldVersion.GetAttributeList(newAtt.FullName).Count == 0)
                                        {
                                            //dodajem
                                            diff.AddAttribute(newAtt, null);
                                        }
                                        else
                                        {
                                            bool found = false;
                                            if (oldVersion.HasAttributes)
                                            {
                                                foreach (ObjectAttribute oldAtt in oldVersion.GetAttributeList(newAtt.FullName))
                                                {
                                                    //proveravam da li se javlja ista vrednost
                                                    if (string.Compare(newAtt.Value, oldAtt.Value) == 0)
                                                    {
                                                        //ako se javlja i isti je obelezavam ga kao nadjen 
                                                        found = true;
                                                        break;
                                                    }
                                                }
                                            }
                                            //ako je found false, znaci da je dodat
                                            if (!found)
                                            {
                                                diff.AddAttribute(null, newAtt);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        diff.AddAttribute(null, newAtt);
                                    }
                                }
                            }
                        }
                        if (diff.Attributes.Count > 0)
                        {
                            this.modified.Add(diff);
                        }
                    }
                }
            }

        }
        #endregion private: Find differences

        /* 
		//// Method creates text of CIM/RDF element for given attribute of a CIM object             
        //// isMemberOfEmbedded - indication of whether or not given attribute belongs to an embedded child object        
        //// returns : CIM/RDF definition of given CIM attribute
        private static string BuildCIMRDFAttribute(ObjectAttribute attribute, bool isMemberOfEmbedded)
        {
            StringBuilder cimAttributeBuilder = new StringBuilder();
            if (attribute != null)
            {
                cimAttributeBuilder.Append(doubleTab);
                if (isMemberOfEmbedded)
                {
                    cimAttributeBuilder.Append(doubleTab);
                }
                cimAttributeBuilder.Append(tagOpen);
                cimAttributeBuilder.Append(attribute.NamespacePrintString);
                cimAttributeBuilder.Append(attribute.FullName);
                if (attribute.IsReference)
                {
                    cimAttributeBuilder.Append(rdfResource);
                    cimAttributeBuilder.Append(StringManipulationManager.PrepareStringForXMLFormat(attribute.Value));
                    cimAttributeBuilder.Append(quote);
                    cimAttributeBuilder.AppendLine(closingTagClose);
                }
                else
                {
                    cimAttributeBuilder.Append(tagClose);
                    cimAttributeBuilder.Append(StringManipulationManager.PrepareStringForXMLFormat(attribute.Value));
                    cimAttributeBuilder.Append(closingTagOpen);
                    cimAttributeBuilder.Append(attribute.NamespacePrintString);
                    cimAttributeBuilder.Append(attribute.FullName);
                    cimAttributeBuilder.AppendLine(tagClose);
                }
            }
            return cimAttributeBuilder.ToString();
        } 
		*/

        #region public static: Prepare XML text for print
        public string BuildDefaultCIMRDFDocumentHeaderSnippet()
        {
            StringBuilder headerBuilder = new StringBuilder();
            headerBuilder.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            headerBuilder.Append("<rdf:RDF");
            foreach (KeyValuePair<string, string> pair in rdfNamespaces)
            {
                headerBuilder.AppendFormat("\n\txmlns:{0}=\"{1}\"", pair.Key, pair.Value);
            }
            headerBuilder.Append("\n\txmlns:dm=\"http://iec.ch/2002/schema/CIM_difference_model#\"");
            headerBuilder.AppendLine(">");
            return headerBuilder.ToString();
        }

        /// <summary>
        /// Method returns footer for CIM/RDF document i.e. closing rdf tag.
        /// </summary>
        /// <returns>footer of RDF document, ie. "/rdf:RDF"</returns>
        public static string BuildCIMRDFDocumentFooter()
        {
            return "</rdf:RDF>";
        }

        //// Method returns xml comment paragraph.        
        //// returns : string - xml comment paragraph with given text
        private static string BuildXMLComment(string comment)
        {
            StringBuilder commentBuilder = new StringBuilder();
            commentBuilder.Append(commentBegin);

            string subString = comment;
            int maxLength = 150;
            while (subString.Length > maxLength)
            {
                subString = subString.Substring(0, maxLength);
                commentBuilder.AppendLine(subString);
            }
            commentBuilder.Append(subString);
            commentBuilder.AppendLine(commentEnd);

            return commentBuilder.ToString();
        }
        #endregion public static: Prepare XML text for print

        internal void AddRDFNamespaces(Dictionary<string, string> namespaces)
        {
            foreach (KeyValuePair<string, string> pair in namespaces)
            {
                string prefixAndNamespace = pair.Key;
                string uri = pair.Value;
                if (!string.IsNullOrEmpty(prefixAndNamespace) && !string.IsNullOrEmpty(uri))
                {
                    if (!rdfNamespaces.ContainsKey(prefixAndNamespace))
                    {
                        rdfNamespaces.Add(prefixAndNamespace, uri);
                    }
                    else
                    {
                        rdfNamespaces[prefixAndNamespace] = uri;
                    }
                }
            }
        }
    }
}
