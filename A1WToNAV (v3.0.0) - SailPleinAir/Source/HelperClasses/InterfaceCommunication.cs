using System;
using System.Web.Services;
using System.Data;
using System.IO;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.Serialization;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Accellos.InterfaceFramework.JobServer;
using Accellos.InterfaceFramework.EventNotificationServer;
using Accellos.Interfaces.NAV.BusinessObjects;
using Accellos.Interfaces.NAV.Controllers;
using Accellos.Interfaces.NAV.WMSAdapters;
using Accellos.Interfaces.NAV.NAVWebServices;

namespace Accellos.Interfaces.NAV
{
    [System.Web.Services.WebServiceBinding (ConformsTo=WsiProfiles.BasicProfile1_1 )] 
    //Note: !!! Do not modify name or namespace of this class, NAV API relies on it !!!
    [WebService(Namespace = "http://accellos/interface/nav")]
    public class InterfaceCommunication : WebService
    {
        #region SVA Push Methods
        [WebMethod]
        public String DownloadComputerAdjustmentFromFile()
        {
            try
            {
                DownloadController.ComputerAdjustmentDownloadDataSource(DataSourceType.File);

                return "OK";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        [WebMethod]
        public String DownloadComputerAdjustment()
        {
            try
            {
              //  JobClient.Instance.PostJob(new Job("Accellos.Interfaces.NAV.Controllers.DownloadController",
              //                                     new FunctionCall("ComputerAdjustmentDownload")) { InstanceId = CompanyConfig.InstanceId });
                DownloadController.ComputerAdjustmentDownloadDataSource(DataSourceType.NAVWebService);

                return "OK";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        [WebMethod]
        public String DownloadInventoryItem()
        {
            try
            {
                //JobClient.Instance.PostJob(new Job("Accellos.Interfaces.NAV.Controllers.DownloadController",
                //                                   new FunctionCall("InventoryItemDownload")) { InstanceId = CompanyConfig.InstanceId });
                DownloadController.InventoryItemDownload();

                return "OK";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        [WebMethod]
        public String DownloadPurchaseOrder()
        {
            try
            {
                //JobClient.Instance.PostJob(new Job("Accellos.Interfaces.NAV.Controllers.DownloadController",
                //                                   new FunctionCall("PurchaseOrderDownload")) { InstanceId = CompanyConfig.InstanceId });
                DownloadController.PurchaseOrderDownload();

                return "OK";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        
        [WebMethod]
        public String DownloadSalesOrder()
        {
            try
            {
                //JobClient.Instance.PostJob(new Job("Accellos.Interfaces.NAV.Controllers.DownloadController",
                //                                   new FunctionCall("SalesOrderDownload")) { InstanceId = CompanyConfig.InstanceId });
                DownloadController.SalesOrderDownload();

                return "OK";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        [WebMethod]
        public String DownloadPurchaseReturnOrder()
        {
            try
            {
                //JobClient.Instance.PostJob(new Job("Accellos.Interfaces.NAV.Controllers.DownloadController",
                //                                   new FunctionCall("PurchaseReturnOrderDownload")) { InstanceId = CompanyConfig.InstanceId });
                DownloadController.PurchaseReturnOrderDownload();

                return "OK";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        [WebMethod]
        public String DownloadSalesReturnOrder()
        {
            try
            {
                //JobClient.Instance.PostJob(new Job("Accellos.Interfaces.NAV.Controllers.DownloadController",
                //                                   new FunctionCall("RmaOrderDownload")) { InstanceId = CompanyConfig.InstanceId });
                DownloadController.RmaOrderDownload();

                return "OK";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        [WebMethod]
        public String DownloadTransferOrder()
        {
            try
            {
                //JobClient.Instance.PostJob(new Job("Accellos.Interfaces.NAV.Controllers.DownloadController",
                //                                   new FunctionCall("TransferOrderDownload")) { InstanceId = CompanyConfig.InstanceId });
                DownloadController.TransferOrderDownload();

                return "OK";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        [WebMethod]
        public String DownloadProductionOrder()
        {
            try
            {
                //JobClient.Instance.PostJob(new Job("Accellos.Interfaces.NAV.Controllers.DownloadController",
                //                                   new FunctionCall("ProductionOrderDownload")) { InstanceId = CompanyConfig.InstanceId });
                DownloadController.ProductionOrderDownload();

                return "OK";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        [WebMethod]
        public String DownloadPurchaseOrderRetrieval()
        {
            try
            {
                //JobClient.Instance.PostJob(new Job("Accellos.Interfaces.NAV.Controllers.DownloadController",
                //                                   new FunctionCall("PurchaseOrderRetrieval")) { InstanceId = CompanyConfig.InstanceId });
                DownloadController.PurchaseOrderRetrieval();

                return "OK";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        [WebMethod]
        public String DownloadSalesOrderRetrieval()
        {
            try
            {
                //JobClient.Instance.PostJob(new Job("Accellos.Interfaces.NAV.Controllers.DownloadController",
                //                                   new FunctionCall("SalesOrderRetrieval")) { InstanceId = CompanyConfig.InstanceId });
                DownloadController.SalesOrderRetrieval();

                return "OK";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        [WebMethod]
        public String DownloadPurchaseReturnOrderRetrieval()
        {
            try
            {
                //JobClient.Instance.PostJob(new Job("Accellos.Interfaces.NAV.Controllers.DownloadController",
                //                                   new FunctionCall("PurchaseReturnOrderRetrieval")) { InstanceId = CompanyConfig.InstanceId });
                DownloadController.PurchaseReturnOrderRetrieval();

                return "OK";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        [WebMethod]
        public String DownloadSalesReturnOrderRetrieval()
        {
            try
            {
                //JobClient.Instance.PostJob(new Job("Accellos.Interfaces.NAV.Controllers.DownloadController",
                //                                   new FunctionCall("RmaOrderRetrieval")) { InstanceId = CompanyConfig.InstanceId });
                DownloadController.RMAOrderRetrieval();

                return "OK";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        [WebMethod]
        public String DownloadTransferOrderRetrieval()
        {
            try
            {
                //JobClient.Instance.PostJob(new Job("Accellos.Interfaces.NAV.Controllers.DownloadController",
                //                                   new FunctionCall("TransferOrderRetrieval")) { InstanceId = CompanyConfig.InstanceId });
                DownloadController.TransferOrderRetrieval();

                return "OK";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        [WebMethod]
        public String DownloadProductionOrderRetrieval()
        {
            try
            {
                //JobClient.Instance.PostJob(new Job("Accellos.Interfaces.NAV.Controllers.DownloadController",
                //                                   new FunctionCall("RmaOrderRetrieval")) { InstanceId = CompanyConfig.InstanceId });
                DownloadController.ProductionOrderRetrieval();

                return "OK";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }


        //[WebMethod]
        //public String CheckOrderStatus(String documentType, String orderNumber, String clientName)
        //{
        //    String returnValue = "Unlocked";

        //    String is3pl = CompanyConfig.Instance.GetWarehouseDBConnection().ExecuteScalar("select TOP 1 C_VALUE from config nolock where config = '3PL_EDITION'").ToString();

        //    if (is3pl != "Y")
        //    {
        //        clientName = String.Empty;
        //    }

        //    switch (documentType)
        //    {
        //        // Sales Order / Pick Header
        //        case "PH":
        //            returnValue = WMSAdapterUtils.CheckSalesOrderLockStatus(orderNumber, clientName);
        //            break;

        //        // Transfer Order / Pick Header
        //        case "TO":
        //            returnValue = WMSAdapterUtils.CheckTransferOrderLockStatus(orderNumber, clientName);
        //            break;

        //        // Warehouse Transfer Receipt / Receipt Header
        //        case "TR":
        //            returnValue = WMSAdapterUtils.CheckPurchaseOrderLockStatus(orderNumber, clientName);
        //            break;
                    
        //        // Purchase Order / Receipt Header
        //        case "RH":
        //            returnValue = WMSAdapterUtils.CheckPurchaseOrderLockStatus(orderNumber, clientName);
        //            break;

        //        // Customer Return - RMA / Receipt Header
        //        case "CR":
        //            returnValue = WMSAdapterUtils.CheckRMAOrderLockStatus(orderNumber, clientName);
        //            break;

        //        // Vendor Return / Pick Header
        //        case "VR":
        //            returnValue = WMSAdapterUtils.CheckSalesOrderLockStatus(orderNumber, clientName);
        //            break;

        //        default:
        //            returnValue = "Unlocked";
        //            break;
        //    }

        //    return returnValue;
        //}

        #endregion

        #region Test Methods

        //These Web Methods were created to print the ELEMENTs of a Business Object - so that matching ELEMENTS could be created in NAV's XMLPorts
        //[WebMethod]
        //public List<InventoryItem> ReadInventoryItem()
        //{
        //    List<InventoryItem> myResult = new List<InventoryItem>();
        //    return myResult;
        //}

        //[WebMethod]
        //public List<Packslip> ReadPackslip()
        //{
        //    List<Packslip> myResult = new List<Packslip>();
        //    return myResult;
        //}

        //[WebMethod]
        //public List<PackslipLine> ReadPackslipLine()
        //{
        //    List<PackslipLine> myResult = new List<PackslipLine>();
        //    return myResult;
        //}

        //[WebMethod]
        //public List<PickConfirmation> ReadPickConfirmation()
        //{
        //    List<PickConfirmation> myResult = new List<PickConfirmation>();
        //    return myResult;
        //}

        //[WebMethod]
        //public List<Shipment> ReadShipment()
        //{
        //    List<Shipment> myResult = new List<Shipment>();
        //    return myResult;
        //}

        //[WebMethod]
        //public List<Carton> ReadCarton()
        //{
        //    List<Carton> myResult = new List<Carton>();
        //    return myResult;
        //}

        //[WebMethod]
        //public List<CartonLine> ReadCartonLine()
        //{
        //    List<CartonLine> myResult = new List<CartonLine>();
        //    return myResult;
        //}

        //[WebMethod]
        //public List<AttributeSet> ReadAttributeSet()
        //{
        //    List<AttributeSet> myResult = new List<AttributeSet>();
        //    return myResult;
        //}

        //[WebMethod]
        //public List<AttributeTracking> ReadAttributeTracking()
        //{
        //    List<AttributeTracking> myResult = new List<AttributeTracking>();
        //    return myResult;
        //}

        //[WebMethod]
        //public List<PurchaseOrder> ReadPurchaseOrder()
        //{
        //    List<PurchaseOrder> myResult = new List<PurchaseOrder>();
        //    return myResult;
        //}

        //[WebMethod]
        //public List<PurchaseOrderLine> ReadPurchaseOrderLine()
        //{
        //    List<PurchaseOrderLine> myResult = new List<PurchaseOrderLine>();
        //    return myResult;
        //}

        //[WebMethod]
        //public List<ReceiptConfirmation> ReadReceiptConfirmation()
        //{
        //    List<ReceiptConfirmation> myResult = new List<ReceiptConfirmation>();
        //    return myResult;
        //}


        //[WebMethod]
        //public List<ReceiptConfirmationLine> ReadReceiptConfirmationLine()
        //{
        //    List<ReceiptConfirmationLine> myResult = new List<ReceiptConfirmationLine>();
        //    return myResult;
        //}

        //[WebMethod]
        //public List<TransferConfirmation> ReadTransferConfirmation()
        //{
        //    List<TransferConfirmation> myResult = new List<TransferConfirmation>();
        //    return myResult;
        //}


        //[WebMethod]
        //public TestReceiptBO UploadTestReceipts()
        //{
        //    TestReceiptBO returnValue = new TestReceiptBO();
        //    Guid myGuid;
        //    myGuid = Guid.NewGuid();
        //    String myRowID = myGuid.ToString();
        //    String[] myReceiptArray = new String[9];

        //    try
        //    {
        //        TestReceiptBO myReceipt = new TestReceiptBO();
        //        myReceipt.UploadType = "RC";
        //        myReceipt.ProductCode = "ABCITEM";
        //        myReceipt.RowID = myRowID;
        //        myReceipt.Warehouse = "01";
        //        myReceipt.LineNum = "1";
        //        myReceipt.QuantityExpected = 100;
        //        myReceipt.QuantityReceived = 75;
        //        myReceipt.ReceivedBy = "DWJ";

        //        myReceiptArray[1] = "RC";
        //        myReceiptArray[2] = myRowID;
        //        myReceiptArray[3] = "01";
        //        myReceiptArray[4] = "1";
        //        myReceiptArray[5] = "ABCITEM";
        //        myReceiptArray[6] = "100";
        //        myReceiptArray[7] = "DWJ";
        //        myReceiptArray[8] = "75";

        //        return myReceipt;

        //    }
        //    catch (Exception ex)
        //    {
        //        returnValue.ProductCode = ex.Message.ToString();
        //    }

        //    return returnValue;
        //}



        //[WebMethod]
        //public String DownloadTestItem(List<TestItem> TestItems)
        //{
        //    String returnValue = String.Empty;
        //    DataTable downloadTable = new DataTable();
        //    String postResult = String.Empty;

        //    try
        //    {
        //        foreach (TestItem item in TestItems)
        //        {

        //            InventoryItem inventoryItem = new InventoryItem();


        //            inventoryItem.ProductCode = item.ProductCode;
        //            inventoryItem.Description = item.Description;
        //            inventoryItem.ClientName = item.ClientName;

        //            EventNotificationClient.Instance.PostMessage("NAV.TestXMLDownload." + item.ClientName.ToString(),
        //                             CompanyConfig.InstanceId,
        //                             "Download started",
        //                             new Variable("itemId", item.ProductCode));

        //            downloadTable = WMSAdapter.ParseInventoryItem(inventoryItem);
        //            postResult = CompanyConfig.Instance.PostDownloadRecords(downloadTable);

        //        }

        //        if (!string.IsNullOrEmpty(postResult))
        //        {
        //            returnValue = postResult;
        //        }
        //        else
        //        {
        //            returnValue = "OK";
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        returnValue = ex.Message.ToString();
        //    }

        //    return returnValue;
        //}



        //[WebMethod]
        //public List<InventoryItem> CheckOrderStatusStronglyTyped(String documentType, String uniqueIdentifier)
        //{

        //    InventoryItem myItem = new InventoryItem();
        //    List<InventoryItem> myInventoryItems = new List<InventoryItem>();

        //    myItem.ProductCode = "MYFIRSTITEM";
        //    myItem.Description = "My First Inventory Item";
        //    myInventoryItems.Add(myItem);

        //    myItem = new InventoryItem();
        //    myItem.ProductCode = "MYSECONDITEM";
        //    myItem.Description = "My Second Inventory Item";
        //    myInventoryItems.Add(myItem);
        //    return myInventoryItems;
        //}

        //[WebMethod]
        //public List<String> CheckOrderStatusLooselyTyped(String documentType, String uniqueIdentifier)
        //{

        //    String myTemp = String.Empty;
        //    List<String> myTemps = new List<String>();

        //    myTemp = "FirstItem";
        //    myTemps.Add(myTemp);

        //    myTemp = "SecondITem";
        //    myTemps.Add(myTemp);

        //    return myTemps;
        //}


        //Note: !!! Do not modify signature of this call, NAV API relies on it !!!
 
        #endregion

        #region Accellos Original HandleEvent
        //[WebMethod]
        //public String HandleEvent(string eventType, String objectType, String uniqueIdentifier)
        //{
        //    try
        //    {
        //        EventNotificationClient.Instance.PostMessage("NAV.Download." + objectType,
        //                                                     CompanyConfig.InstanceId,
        //                                                     eventType,
        //                                                     new Variable("eventType", eventType),
        //                                                     new Variable("objectType", objectType),
        //                                                     new Variable("uniqueIdentifier", uniqueIdentifier));
        //        switch (eventType)
        //        {
        //            //Download
        //            case "Download":

        //                switch (objectType)
        //                {
        //                    case "InventTable":
        //                        JobClient.Instance.PostJob(new Job("Accellos.Interfaces.NAV.Controllers.DownloadController",
        //                                                           new FunctionCall("InventoryItemDownload", objectType, uniqueIdentifier)) { InstanceId = CompanyConfig.InstanceId });
        //                        break;
        //                    case "SalesTable":
        //                        JobClient.Instance.PostJob(new Job("Accellos.Interfaces.NAV.Controllers.DownloadController",
        //                                                           new FunctionCall("SalesOrderDownload", objectType, uniqueIdentifier)) { InstanceId = CompanyConfig.InstanceId });
        //                        break;
        //                    case "PurchTable":
        //                        JobClient.Instance.PostJob(new Job("Accellos.Interfaces.NAV.Controllers.DownloadController",
        //                                                           new FunctionCall("PurchaseOrderDownload", objectType, uniqueIdentifier)) { InstanceId = CompanyConfig.InstanceId });
        //                        break;
        //                    case "ProdTable":
        //                        JobClient.Instance.PostJob(new Job("Accellos.Interfaces.NAV.Controllers.DownloadController",
        //                                                           new FunctionCall("ProductionOrderDownload", objectType, uniqueIdentifier)) { InstanceId = CompanyConfig.InstanceId });
        //                        break;
        //                }
        //                break;
        //            //Record locking
        //            case "Verify":
        //                switch (objectType)
        //                {
        //                    case "SalesTable":
        //                        WMSAdapter.VerifyPackslip(uniqueIdentifier);
        //                        break;
        //                    case "PurchTable":
        //                        WMSAdapter.VerifyPurchaseOrder(uniqueIdentifier);
        //                        break;
        //                    case "ProdTable":
        //                        WMSAdapter.VerifyProductionOrder(uniqueIdentifier);
        //                        break;
        //                }
        //                break;
        //            //Invalid parameter
        //            default:
        //                throw new Exception(string.Format("Invalid Event type [{0}]", eventType));
        //        }
        //        //Successful call
        //        return string.Empty;
        //    }
        //    catch (Exception ex)
        //    {
        //        //Return whatever error message came up back to AX to show error message
        //        return ex.Message;
        //    }
        //}
        
        #endregion

    }
}
