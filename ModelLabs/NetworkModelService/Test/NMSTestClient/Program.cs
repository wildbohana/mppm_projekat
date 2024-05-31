using FTN.Common;
using System;
using System.Collections.Generic;
using System.Text;
using TelventDMS.Services.NetworkModelService.TestClient.Tests;

namespace FTN.Services.NetworkModelService.TestClient
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            ModelResourcesDesc resDesc = new ModelResourcesDesc();

            try
            {
                // Set NMSTestInterfaceClient console layout
                Console.SetWindowPosition(0, 0);
                Console.SetBufferSize(250, 1000);
                Console.SetWindowSize((int)(Console.LargestWindowWidth * 0.7), (int)(Console.LargestWindowHeight * 0.8));
                Console.Title = "Network Model Service Test Client";
            }
            catch (Exception ex)
            {
                string errPositionMessage = string.Format("Error: Could not set Network Model Service Test Client's window size and position. {0}", ex.Message);
                Console.WriteLine(errPositionMessage);
                CommonTrace.WriteTrace(CommonTrace.TraceError, errPositionMessage);
            }

            string message = string.Format("Network Model Service Test Client is up and running...");
            Console.WriteLine(message);
            CommonTrace.WriteTrace(CommonTrace.TraceInfo, message);

            message = string.Format("Result directory: {0}", Config.Instance.ResultDirecotry);
            Console.WriteLine(message);
            CommonTrace.WriteTrace(CommonTrace.TraceInfo, message);

            try
            {
                TestGda tgda = new TestGda();


                string str = string.Empty;
                do
                {
                    PrintMenu();
                    str = Console.ReadLine();

                    if (str == "1")
                    {
                        try
                        {
                            tgda.GetValues(InputGlobalId());
                        }
                        catch (Exception ex)
                        {
                            message = string.Format("GetValues failed. {0}", ex.Message);
                            Console.WriteLine(message);
                            CommonTrace.WriteTrace(CommonTrace.TraceError, message);
                        }

                    }
                    else if (str == "2")
                    {
                        try
                        {
                            tgda.GetExtentValues(InputModelCode());
                        }
                        catch (Exception ex)
                        {
                            message = string.Format("GetExtentValues failed. {0}", ex.Message);
                            Console.WriteLine(message);
                            CommonTrace.WriteTrace(CommonTrace.TraceError, message);
                        }
                    }
                    else if (str == "3")
                    {
                        try
                        {
                            tgda.GetRelatedValues(InputGlobalId(), InputAssociation());
                        }
                        catch (Exception ex)
                        {
                            message = string.Format("GetRelatedValues failed. {0}", ex.Message);
                            Console.WriteLine(message);
                            CommonTrace.WriteTrace(CommonTrace.TraceError, message);
                        }
                    }
                    else if (str == "4")
                    {
                        try
                        {
                            tgda.TestApplyDeltaInsert();
                        }
                        catch (Exception ex)
                        {
                            message = string.Format("Test ApplyUpdate: Insert - Update - Delte failed. {0}", ex.Message);
                            Console.WriteLine(message);
                            CommonTrace.WriteTrace(CommonTrace.TraceError, message);
                        }
                    }
                    else if (str != "q")
                    {
                        PrintUnknownOption();
                    }

                    Console.WriteLine();
                }
                while (str != "q");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                CommonTrace.WriteTrace(CommonTrace.TraceError, ex.Message);
            }

            message = "Network Model Service Test Client stopped.";
            Console.WriteLine(message);
            CommonTrace.WriteTrace(CommonTrace.TraceError, message);
        }

        private static void PrintMenu()
        {
            Console.WriteLine("\nChoose tests type:");
            Console.WriteLine("\t1) Get values");
            Console.WriteLine("\t2) Get extent values");
            Console.WriteLine("\t3) Get related values");
            Console.WriteLine("\t4) Test apply update");
            Console.WriteLine("\tq) Quit");
        }

        #region Help methods

        private static void PrintUnknownOption()
        {
            Console.WriteLine("\nUnknown option entered. Please try again.");
        }

        private static long InputGlobalId()
        {
            CommonTrace.WriteTrace(CommonTrace.TraceVerbose, "Entering globalId started.");

            try
            {
                Console.Write("Enter global Id: ");
                string strId = Console.ReadLine();

                if (strId.StartsWith("0x", StringComparison.Ordinal))
                {
                    strId = strId.Remove(0, 2);
                    CommonTrace.WriteTrace(CommonTrace.TraceVerbose, "Entering globalId successfully ended.");

                    return Convert.ToInt64(Int64.Parse(strId, System.Globalization.NumberStyles.HexNumber));
                }
                else
                {
                    CommonTrace.WriteTrace(CommonTrace.TraceVerbose, "Entering globalId successfully ended.");
                    return Convert.ToInt64(strId);
                }
            }
            catch (FormatException ex)
            {
                string message = "Entering entity id failed. Please use hex (0x) or decimal format.";
                CommonTrace.WriteTrace(CommonTrace.TraceError, message);
                Console.WriteLine(message);
                throw ex;
            }
        }

        private static ModelCode InputModelCode()
        {
            CommonTrace.WriteTrace(CommonTrace.TraceVerbose, "Entering Model Code started.");

            try
            {
                Console.Write("Enter Model Code: ");
                string userModelCode = Console.ReadLine();
                ModelCode modelCode = 0;

                if (!ModelCodeHelper.GetModelCodeFromString(userModelCode, out modelCode))
                {
                    if (userModelCode.StartsWith("0x", StringComparison.Ordinal))
                    {
                        modelCode = (ModelCode)long.Parse(userModelCode.Substring(2), System.Globalization.NumberStyles.HexNumber);
                    }
                    else
                    {
                        modelCode = (ModelCode)long.Parse(userModelCode);
                    }
                }

                return modelCode;
            }
            catch (Exception ex)
            {
                string message = string.Format("Entering Model Code failed. {0}", ex);
                CommonTrace.WriteTrace(CommonTrace.TraceError, message);
                Console.WriteLine(message);
                throw ex;
            }
        }

        private static Association InputAssociation()
        {
            CommonTrace.WriteTrace(CommonTrace.TraceVerbose, "Entering association started.");
            Association association = new Association();

            try
            {
                Console.Write("Entering  association\n");

                Console.Write("Enter propertyId: ");

                string userModelCode = Console.ReadLine();
                ModelCode modelCode = 0;

                if (!ModelCodeHelper.GetModelCodeFromString(userModelCode, out modelCode))
                {
                    if (userModelCode.StartsWith("0x", StringComparison.Ordinal))
                    {
                        modelCode = (ModelCode)long.Parse(userModelCode.Substring(2), System.Globalization.NumberStyles.HexNumber);
                    }
                    else
                    {
                        modelCode = (ModelCode)long.Parse(userModelCode);
                    }
                }

                association.PropertyId = modelCode;

                Console.Write("Enter type model code: ");

                userModelCode = Console.ReadLine();
                modelCode = 0;

                if (!ModelCodeHelper.GetModelCodeFromString(userModelCode, out modelCode))
                {
                    if (userModelCode.StartsWith("0x", StringComparison.Ordinal))
                    {
                        modelCode = (ModelCode)long.Parse(userModelCode.Substring(2), System.Globalization.NumberStyles.HexNumber);
                    }
                    else
                    {
                        modelCode = (ModelCode)long.Parse(userModelCode);
                    }
                }

                association.Type = modelCode;

                return association;
            }
            catch (Exception ex)
            {
                string message = string.Format("Entering association failed. {0}", ex);
                CommonTrace.WriteTrace(CommonTrace.TraceError, message);
                Console.WriteLine(message);
                throw ex;
            }
        }

        private static string GetListAsCommaSeparatedString(List<string> stringValues)
        {
            StringBuilder sb = new StringBuilder();

            foreach (string stringValue in stringValues)
            {
                sb.Append("'").Append(stringValue).Append("'").Append(", ");
            }

            sb.Remove(sb.Length - 2, 2);

            return sb.ToString();
        }

        #endregion Help methods
    }
}
