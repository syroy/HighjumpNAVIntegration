using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Xml;
using Accellos.InterfaceFramework;
using Accellos.InterfaceFramework.EventNotificationServer;
using Accellos.InterfaceFramework.ScriptManagementServer;
using Accellos.Interfaces.NAV.BusinessObjects;
using Accellos.Interfaces.NAV.NAVWebServices;
using Accellos.Interfaces.NAV.WMSAdapters;

namespace Accellos.Interfaces.NAV.NAVAdapters
{
    public class ComputerAdjustmentNAVAdapter
    {
        #region SVA RetrieveFromNAV
        public static List<AdjustmentsCA> RetrieveFromNAV()
        {
            NAVComputerAdjustmentRoot myNAVComputerAdjustmentRoot = new NAVComputerAdjustmentRoot();
            List<AdjustmentsCA> myWMSComputerAdjustments = new List<AdjustmentsCA>();

            String myResult = String.Empty;

            try
            {
                AdjustmentsCA myWMSComputerAdjustment;

                // Call NAV Web Service | ExportComputerAdjustment
                myResult = CompanyConfig.Instance.NavWebServiceInstance.ExportComputerAdjustment(ref myNAVComputerAdjustmentRoot);
                EventNotificationClient.Instance.PostMessage("NAV.Download.ComputerAdjustments", CompanyConfig.InstanceId, "Exported NAV Data",
                    new Variable("data", Utils.SerializeToString(myNAVComputerAdjustmentRoot)));

                //NAV Business Object | Code = CA
                NAVBusinessObjectCA myNAVBusinessObjectCA = myNAVComputerAdjustmentRoot.NAVBusinessObjectCA;

                //NAV Computer Adjustments | Processed = Ready to Send
                NAVComputerAdjustments myNAVComputerAdjustments = myNAVBusinessObjectCA.NAVComputerAdjustments;

                try
                {
                    if (myNAVComputerAdjustments.NAVCAInventory == null)
                    {
                        //There aren't any orders to process
                        myWMSComputerAdjustments = new List<AdjustmentsCA>();
                        return myWMSComputerAdjustments;
                    }

                    // For each Receipt Header record in the queue
                    foreach (NAVCAInventory iNavCAInventory in myNAVComputerAdjustments.NAVCAInventory)
                    {
                        NAVCAItem iItem = new NAVCAItem();
                        iItem = iNavCAInventory.NAVCAItems.NAVCAItem;

                        myWMSComputerAdjustment = new AdjustmentsCA();

                        // Parse Computer Adjustment
                        myWMSComputerAdjustment.SVAStagingID = iItem.StagingID.ToString();
                        myWMSComputerAdjustment.Attribute1 = NAVAdapterUtils.NAVFormatString(iItem.Attribute1, 40);
                        myWMSComputerAdjustment.Attribute2 = NAVAdapterUtils.NAVFormatString(iItem.Attribute2, 40);
                        myWMSComputerAdjustment.Attribute3 = NAVAdapterUtils.NAVFormatString(iItem.Attribute3, 40);
                        myWMSComputerAdjustment.Attribute4 = NAVAdapterUtils.NAVFormatString(iItem.Attribute4, 40);
                        myWMSComputerAdjustment.Attribute5 = NAVAdapterUtils.NAVFormatString(iItem.Attribute5, 40);
                        myWMSComputerAdjustment.Attribute6 = NAVAdapterUtils.NAVFormatString(iItem.Attribute6, 40);
                        myWMSComputerAdjustment.Attribute7 = NAVAdapterUtils.NAVFormatString(iItem.Attribute7, 40);
                        myWMSComputerAdjustment.Attribute8 = NAVAdapterUtils.NAVFormatString(iItem.Attribute8, 40);
                        myWMSComputerAdjustment.Attribute9 = NAVAdapterUtils.NAVFormatString(iItem.Attribute9, 40);
                        myWMSComputerAdjustment.Attribute10 = NAVAdapterUtils.NAVFormatString(iItem.Attribute10, 40);

                        myWMSComputerAdjustment.BinLocation = NAVAdapterUtils.NAVFormatString(iItem.BinLocation, 40);
                        myWMSComputerAdjustment.ClientName = NAVAdapterUtils.NAVFormatString(iItem.Clientname, 40);
                        myWMSComputerAdjustment.Comment = NAVAdapterUtils.NAVFormatString(iItem.Comment, 40);
                        myWMSComputerAdjustment.Description = NAVAdapterUtils.NAVFormatString(iItem.Description, 100); // Changed to 100 characters for A1W 64 - iCepts 10/6/15
                    //    myWMSComputerAdjustment.FIFODate = NAVAdapterUtils.NAVFormatDateTime(iItem.FIFODate);
                        myWMSComputerAdjustment.Packsize = NAVAdapterUtils.NAVFormatInt(iItem.Innerpack);
                        myWMSComputerAdjustment.PlusMinus = (NAVAdapterUtils.NAVFormatDecimal(iItem.Quantity) < 0 ? "-" : "+");
                        myWMSComputerAdjustment.LicensePlate = NAVAdapterUtils.NAVFormatString(iItem.LicensePlate, 40);
                        myWMSComputerAdjustment.MaximumLevelOfReplenishment = NAVAdapterUtils.NAVFormatInt(iItem.MaximumLevelOfReplenishment);
                        myWMSComputerAdjustment.MinimumLevelOfReplenishment = NAVAdapterUtils.NAVFormatInt(iItem.MinimumLevelOfReplenishment);
                        myWMSComputerAdjustment.Packsize = NAVAdapterUtils.NAVFormatInt(iItem.Packsize);
                        myWMSComputerAdjustment.PONumber = NAVAdapterUtils.NAVFormatString(iItem.PONumber, 40);
                        myWMSComputerAdjustment.ProductClass = NAVAdapterUtils.NAVFormatString(iItem.ProductClass, 40);
                        myWMSComputerAdjustment.ProductCode = NAVAdapterUtils.NAVFormatString(iItem.ProductCode, 40);
                        myWMSComputerAdjustment.Quantity = Math.Abs(NAVAdapterUtils.NAVFormatDecimal(iItem.Quantity));
                        myWMSComputerAdjustment.ReceivingAttributeControl = NAVAdapterUtils.NAVFormatString(iItem.ReceivingAttributeControl, 40);
                        myWMSComputerAdjustment.ReservedStockFlag = NAVAdapterUtils.NAVFormatString(iItem.ReservedStockFlag, 40);
                    //    myWMSComputerAdjustment.Subcode = NAVAdapterUtils.NAVFormatString(iItem.Subcode, 40);
                        myWMSComputerAdjustment.UnitOfMeasure = NAVAdapterUtils.NAVFormatString(iItem.UnitOfMeasure, 40);
                        myWMSComputerAdjustment.UPC = NAVAdapterUtils.NAVFormatString(iItem.UPC, 40);

                        //Add Computer Adjustment (if succeeded) to List of Computer Adjustments
                        myWMSComputerAdjustments.Add(myWMSComputerAdjustment);
                        EventNotificationClient.Instance.PostMessage("NAV.Download.ComputerAdjustments", CompanyConfig.InstanceId,
                            "A1I -> Status Update -> NAV", new Variable("Status", "Sent to WMS"),
                            new Variable("Product Code", myWMSComputerAdjustment.ProductCode));
                        //try
                        //{
                        //    // Call NAV Web Service | SentStatus
                        //    CompanyConfig.Instance.NavWebServiceInstance.SentStatus(NAVAdapterUtils.NAVFormatString(iItem.StagingID.ToString(), 40), "CA");
                        //    CompanyConfig.Instance.NavWebServiceInstance.Dispose();
                        //}
                        //catch (Exception ex1)
                        //{
                        //    EventNotificationClient.Instance.PostMessage("NAV.Download.InventoryItems.Error", CompanyConfig.InstanceId, "Error: A1I Entry No -> NAV", new Variable("Error Updating Status to NAV", ex1));
                        //}
                    }
                }
                catch (Exception exceptionCALvl)
                {
                    //Error with Computer Adjustment
                    EventNotificationClient.Instance.PostMessage("NAV.Download.ComputerAdjustments.Error", CompanyConfig.InstanceId,
                        "Error: Computer Adjustment Level", new Variable("Error Sending to WMS", exceptionCALvl));
                }
            }
            catch (Exception exceptionCARootLvl)
            {
                //Error at Computer Adjustment Root Level
                EventNotificationClient.Instance.PostMessage("NAV.Download.ComputerAdjustments.Error", CompanyConfig.InstanceId,
                    "Error: Computer Adjustment Root Level", new Variable("Error Sending to WMS", exceptionCARootLvl),
                    new Variable("NAV Return Data", myNAVComputerAdjustmentRoot), new Variable("My Result", myResult));
            }

            return myWMSComputerAdjustments;
        }
        #endregion

        #region SVA RetrieveFromFile
        public static List<AdjustmentsCA> RetrieveFromFile()
        {
            return RetrieveFromFile("\\\\" + CompanyConfig.Instance.Url.Split('/')[2].Split(':')[0] + "\\SVATemp\\CA.xml");
        }

        public static List<AdjustmentsCA> RetrieveFromFile(string filepath)
        {
            List<AdjustmentsCA> myWMSComputerAdjustments = new List<AdjustmentsCA>();
            AdjustmentsCA myWMSComputerAdjustment;

            XmlTextReader xmlReadCA = new XmlTextReader(filepath);
            xmlReadCA.WhitespaceHandling = WhitespaceHandling.None;

            try
            {
                xmlReadCA.Read();
                //EventNotificationClient.Instance.PostMessage("NAV.Download.ComputerAdjustments", CompanyConfig.InstanceId, "Node Information",
                //        new Variable("Name", xmlReadCA.Name), new Variable("Value", xmlReadCA.Value), new Variable("Type", xmlReadCA.NodeType));
                if (xmlReadCA.NodeType != XmlNodeType.XmlDeclaration)
                {
                    throw new Exception("Xml Declaration Node Missing!");
                }

                xmlReadCA.Read();
                //EventNotificationClient.Instance.PostMessage("NAV.Download.ComputerAdjustments", CompanyConfig.InstanceId, "Node Information",
                //        new Variable("Name", xmlReadCA.Name), new Variable("Value", xmlReadCA.Value), new Variable("Type", xmlReadCA.NodeType));
                if (xmlReadCA.Name != "NAVComputerAdjustmentRoot")
                {
                    throw new Exception("NAVComputerAdjustmentRoot Node Missing!");
                }

                xmlReadCA.Read();
                //EventNotificationClient.Instance.PostMessage("NAV.Download.ComputerAdjustments", CompanyConfig.InstanceId, "Node Information",
                //        new Variable("Name", xmlReadCA.Name), new Variable("Value", xmlReadCA.Value), new Variable("Type", xmlReadCA.NodeType));
                if (xmlReadCA.Name != "NAVBusinessObjectCA")
                {
                    throw new Exception("NAVBusinessObjectCA Node Missing!");
                }

                xmlReadCA.Read();
                //EventNotificationClient.Instance.PostMessage("NAV.Download.ComputerAdjustments", CompanyConfig.InstanceId, "Node Information",
                //        new Variable("Name", xmlReadCA.Name), new Variable("Value", xmlReadCA.Value), new Variable("Type", xmlReadCA.NodeType));
                if (xmlReadCA.Name != "NAVComputerAdjustments")
                {
                    throw new Exception("NAVComputerAdjustments Node Missing!");
                }

                if (xmlReadCA.IsEmptyElement)
                {
                    //There aren't any orders to process
                    xmlReadCA.Close();

                    myWMSComputerAdjustments = new List<AdjustmentsCA>();
                    return myWMSComputerAdjustments;
                }

                xmlReadCA.Read();
                //EventNotificationClient.Instance.PostMessage("NAV.Download.ComputerAdjustments", CompanyConfig.InstanceId, "Node Information",
                //        new Variable("Name", xmlReadCA.Name), new Variable("Value", xmlReadCA.Value), new Variable("Type", xmlReadCA.NodeType));
                if (xmlReadCA.Name != "NAVCAInventory")
                {
                    throw new Exception("NAVCAInventory Node Missing!");
                }

                while (xmlReadCA.Name == "NAVCAInventory")
                {
                    xmlReadCA.Read();
                    //EventNotificationClient.Instance.PostMessage("NAV.Download.ComputerAdjustments", CompanyConfig.InstanceId, "Node Information",
                    //        new Variable("Name", xmlReadCA.Name), new Variable("Value", xmlReadCA.Value), new Variable("Type", xmlReadCA.NodeType));
                    if (xmlReadCA.Name != "NAVCAItems")
                    {
                        throw new Exception("NAVCAItems Node Missing!");
                    }

                    xmlReadCA.Read();
                    //EventNotificationClient.Instance.PostMessage("NAV.Download.ComputerAdjustments", CompanyConfig.InstanceId, "Node Information",
                    //        new Variable("Name", xmlReadCA.Name), new Variable("Value", xmlReadCA.Value), new Variable("Type", xmlReadCA.NodeType));
                    if (xmlReadCA.Name != "NAVCAItem")
                    {
                        throw new Exception("NAVCAItem Node Missing!");
                    }

                    myWMSComputerAdjustment = new AdjustmentsCA();

                    xmlReadCA.Read();
                    //EventNotificationClient.Instance.PostMessage("NAV.Download.ComputerAdjustments", CompanyConfig.InstanceId, "Element Node Info",
                    //    new Variable("Name", xmlReadCA.Name), new Variable("Value", xmlReadCA.Value), new Variable("Type", xmlReadCA.NodeType));

                    while (xmlReadCA.Name != "NAVCAItem")
                    {
                        if (xmlReadCA.NodeType == XmlNodeType.Element)
                        {
                            string elementName = xmlReadCA.Name;

                            xmlReadCA.Read();
                            //EventNotificationClient.Instance.PostMessage("NAV.Download.ComputerAdjustments", CompanyConfig.InstanceId, "Text Node Info",
                            //    new Variable("Name", xmlReadCA.Name), new Variable("Value", xmlReadCA.Value), new Variable("Type", xmlReadCA.NodeType));

                            if (xmlReadCA.NodeType == XmlNodeType.Text)
                            {
                                switch (elementName.ToLower())  //6.4 Added ToLower
                                {
                                    case "adjustmentsubcode":   //6.4 Subcode --> adjustmentsubcode
                                        myWMSComputerAdjustment.AdjustmentSubcode = NAVAdapterUtils.NAVFormatString(xmlReadCA.Value, 40);   //6.4 Subcode --> AdjustmentSubcode
                                        break;
                                    case "attribute1":  //6.4 Attribute1 --> attribute1
                                        myWMSComputerAdjustment.Attribute1 = NAVAdapterUtils.NAVFormatString(xmlReadCA.Value, 40);
                                        break;
                                    case "attribute2":  //6.4 Attribute2 --> attribute2
                                        myWMSComputerAdjustment.Attribute2 = NAVAdapterUtils.NAVFormatString(xmlReadCA.Value, 40);
                                        break;
                                    case "attribute3":  //6.4 Attribute3 --> attribute3
                                        myWMSComputerAdjustment.Attribute3 = NAVAdapterUtils.NAVFormatString(xmlReadCA.Value, 40);
                                        break;
                                    case "attribute4":  //6.4 Attribute4 --> attribute4
                                        myWMSComputerAdjustment.Attribute4 = NAVAdapterUtils.NAVFormatString(xmlReadCA.Value, 40);
                                        break;
                                    case "attribute5":  //6.4 Attribute5 --> attribute5
                                        myWMSComputerAdjustment.Attribute5 = NAVAdapterUtils.NAVFormatString(xmlReadCA.Value, 40);
                                        break;
                                    case "attribute6":  //6.4 Attribute6 --> attribute6
                                        myWMSComputerAdjustment.Attribute6 = NAVAdapterUtils.NAVFormatString(xmlReadCA.Value, 40);
                                        break;
                                    case "attribute7":  //6.4 Attribute7 --> attribute7
                                        myWMSComputerAdjustment.Attribute7 = NAVAdapterUtils.NAVFormatString(xmlReadCA.Value, 40);
                                        break;
                                    case "attribute8":  //6.4 Attribute8 --> attribute8
                                        myWMSComputerAdjustment.Attribute8 = NAVAdapterUtils.NAVFormatString(xmlReadCA.Value, 40);
                                        break;
                                    case "attribute9":  //6.4 Attribute9 --> attribute9
                                        myWMSComputerAdjustment.Attribute9 = NAVAdapterUtils.NAVFormatString(xmlReadCA.Value, 40);
                                        break;
                                    case "attribute10": //6.4 Attribute10 --> attribute10
                                        myWMSComputerAdjustment.Attribute10 = NAVAdapterUtils.NAVFormatString(xmlReadCA.Value, 40);
                                        break;
                                    case "bin": //6.4 BinLocation --> bin
                                        myWMSComputerAdjustment.BinLocation = NAVAdapterUtils.NAVFormatString(xmlReadCA.Value, 40);
                                        break;
                                    case "clientname":  //6.4 Clientname --> clientname
                                        myWMSComputerAdjustment.ClientName = NAVAdapterUtils.getClientName();  //6.4 Clientname --> ClientName, NAVFormatString --> NAVAdapterUtils.getClientName()
                                        break;
                                    case "comment": //6.4 Comment --> comment
                                        myWMSComputerAdjustment.Comment = NAVAdapterUtils.NAVFormatString(xmlReadCA.Value, 40);
                                        break;
                                    case "description": //6.4 Description --> description
                                        myWMSComputerAdjustment.Description = NAVAdapterUtils.NAVFormatString(xmlReadCA.Value, 40);
                                        break;
                                    case "fifodate":    //6.4 FIFODate --> fifodate
                                        myWMSComputerAdjustment.FIFODate = NAVAdapterUtils.NAVFormatDateTime(xmlReadCA.Value);
                                        break;
                                    case "innerpack":   //6.4 Innerpack --> innerpack
                                        myWMSComputerAdjustment.InnerPack = NAVAdapterUtils.NAVFormatInt(xmlReadCA.Value);  //6.4 Innerpack --> InnerPack
                                        break;
                                    case "licenseplate":    //6.4 LicensePlate --> licenseplate
                                        myWMSComputerAdjustment.LicensePlate = NAVAdapterUtils.NAVFormatString(xmlReadCA.Value, 40);
                                        break;
                                    case "maximumlevelofreplenishment": //6.4 MaximumLevelOfReplenishment --> maximumlevelofreplenishment
                                        myWMSComputerAdjustment.MaximumLevelOfReplenishment = NAVAdapterUtils.NAVFormatInt(xmlReadCA.Value);
                                        break;
                                    case "minimumlevelofreplenishment": //6.4 MinimumLevelOfReplenishment --> minimumlevelofreplenishment
                                        myWMSComputerAdjustment.MinimumLevelOfReplenishment = NAVAdapterUtils.NAVFormatInt(xmlReadCA.Value);
                                        break;
                                    case "packsize":    //6.4 Packsize --> packsize
                                        myWMSComputerAdjustment.Packsize = NAVAdapterUtils.NAVFormatInt(xmlReadCA.Value);
                                        break;
                                    case "ponumber":    //6.4 PONumber --> ponumber
                                        myWMSComputerAdjustment.PONumber = NAVAdapterUtils.NAVFormatString(xmlReadCA.Value, 40);
                                        break;
                                    case "productclass":    //6.4 ProductClass --> productclass
                                        myWMSComputerAdjustment.ProductClass = NAVAdapterUtils.NAVFormatString(xmlReadCA.Value, 40);
                                        break;
                                    case "productcode": //6.4 ProductCode --> productcode
                                        myWMSComputerAdjustment.ProductCode = NAVAdapterUtils.NAVFormatString(xmlReadCA.Value, 40);
                                        break;
                                    case "quantity":    //6.4 Quantity --> quantity
                                        myWMSComputerAdjustment.Quantity = NAVAdapterUtils.NAVFormatDecimal(xmlReadCA.Value);
                                     //   myWMSComputerAdjustment.PlusMinus = (myWMSComputerAdjustment.Quantity < 0 ? '-' : '+'); //6.4 InOrOut --> PlusMinus, "-" --> '-', "+" --> '+'
                                        myWMSComputerAdjustment.Quantity = Math.Abs(myWMSComputerAdjustment.Quantity);
                                        break;
                                    case "receivingattributecontrol":   //6.4 ReceivingAttributeControl --> receivingattributecontrol
                                        myWMSComputerAdjustment.ReceivingAttributeControl = NAVAdapterUtils.NAVFormatString(xmlReadCA.Value, 40);
                                        break;
                                    case "reservedstockflag":   //6.4 ReservedStockFlag --> reservedstockflag
                                       // myWMSComputerAdjustment.ReservedStockFlag = NAVAdapterUtils.NAVFormatChar(xmlReadCA.Value); //6.4 NAVFormatString --> NAVFormatChar
                                        break;
                                    case "unitofmeasure":   //6.4 UnitOfMeasure --> unitofmeasure
                                        myWMSComputerAdjustment.UnitOfMeasure = NAVAdapterUtils.NAVFormatString(xmlReadCA.Value, 40);
                                        break;
                                    case "upc": //6.4 UPC --> upc
                                        myWMSComputerAdjustment.UPC = NAVAdapterUtils.NAVFormatString(xmlReadCA.Value, 40);
                                        break;
                                    default:
                                        throw new Exception("Not a supported tag type!");
                                }
                            }
                            else
                            {
                                //Continue the loop because the data we have may be the next element
                                //and we don't want to read the next line of data till we test the 
                                //current element
                                continue;
                            }
                        }

                        //Get next value (if there is one)
                        xmlReadCA.Read();
                        //EventNotificationClient.Instance.PostMessage("NAV.Download.ComputerAdjustments", CompanyConfig.InstanceId, "Node information",
                        //    new Variable("Name", xmlReadCA.Name), new Variable("Value", xmlReadCA.Value), new Variable("Type", xmlReadCA.NodeType));
                    }

                    if (myWMSComputerAdjustment.Quantity != 0)  //If no quantity to adjust, ignore. (NAV doesn't handle this filter well)
                    {
                        //Add Computer Adjustment to List of Computer Adjustments
                        myWMSComputerAdjustments.Add(myWMSComputerAdjustment);
                        EventNotificationClient.Instance.PostMessage("NAV.Download.ComputerAdjustments", CompanyConfig.InstanceId,
                            "A1I -> Status Update -> NAV", new Variable("Status", "Sent to WMS"),
                            new Variable("Product Code", myWMSComputerAdjustment.ProductCode));
                    }
                    else
                    {
                        EventNotificationClient.Instance.PostMessage("NAV.Download.ComputerAdjustments", CompanyConfig.InstanceId,
                            "Quantity 0, adjustment omitted.", new Variable("Bin", myWMSComputerAdjustment.BinLocation),
                            new Variable("Product Code", myWMSComputerAdjustment.ProductCode));
                    }

                    //Get next item (if there is one)
                    while ((!(xmlReadCA.EOF)) && (xmlReadCA.NodeType == XmlNodeType.EndElement))
                    {
                        xmlReadCA.Read();
                        //EventNotificationClient.Instance.PostMessage("NAV.Download.ComputerAdjustments", CompanyConfig.InstanceId, "Node information",
                        //    new Variable("Name", xmlReadCA.Name), new Variable("Value", xmlReadCA.Value), new Variable("Type", xmlReadCA.NodeType));
                    }
                }
            }
            catch (Exception exception)
            {
                //Error with Computer Adjustment
                EventNotificationClient.Instance.PostMessage("NAV.Download.ComputerAdjustments.Error", CompanyConfig.InstanceId,
                    exception.Message, new Variable("Error Sending to WMS", exception));
            }

            xmlReadCA.Close();

            return myWMSComputerAdjustments;
        }
        #endregion
    }
}

