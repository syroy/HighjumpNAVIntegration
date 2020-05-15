using System;
using System.Collections.Generic;
using System.Text;
using Accellos.InterfaceFramework;
using Accellos.InterfaceFramework.EventNotificationServer;
using Accellos.InterfaceFramework.ScriptManagementServer;
using Accellos.Interfaces.NAV.BusinessObjects;
using Accellos.Interfaces.NAV.NAVWebServices;
using Accellos.Interfaces.NAV.WMSAdapters;

namespace Accellos.Interfaces.NAV.NAVAdapters
{
    public class StockCountNAVAdapter
    {
        #region SVA WriteToNAV
        public static List<UploadConfirmation> WriteToNAV(List<StockCount> stockCounts)
        {
            //EventNotificationClient.Instance.PostMessage("NAV.Upload.StockCount.WriteToNAV", CompanyConfig.InstanceId, "Entering Function");   

            List<UploadConfirmation> uploadConfirmations = new List<UploadConfirmation>();
            UploadConfirmation myUploadConfirmation;
            String myRowID = String.Empty;

            try
            {
                NAVStockCountRoot myNAVStockCountRoot = new NAVStockCountRoot();
                List<NAVStockCount> myNAVStockCounts = new List<NAVStockCount>();
                NAVStockCount myNAVStockCount;
                String myResult = String.Empty;

                foreach (StockCount stockCount in stockCounts)
                {
                    myUploadConfirmation = new UploadConfirmation();

                    try
                    {
                        //NAV XMLPort Note:  Items must be assigned in the Column Order that is defined in the XMLPort object

                        //Start with new NAV Stock Count
                        myNAVStockCount = new NAVStockCount();

                        //EventNotificationClient.Instance.PostMessage("NAV.Upload.StockCount.WriteToNAV", CompanyConfig.InstanceId, "Parsing Transfer Confirmations");

                        //Parse Adjustment Record
                        //myNAVStockCount = adjustment.ReplaceText.ToString();
                        myNAVStockCount.RecordType = "SC";
                        myNAVStockCount.UploadType = stockCount.TransactionCode.ToString();
                        myNAVStockCount.Warehouse = stockCount.Warehouse.ToString();
                        myNAVStockCount.ProductCode = stockCount.ProductCode.ToString();
                        myNAVStockCount.Bin = stockCount.BinLocation.ToString();
                        myNAVStockCount.BinOnHandQty = stockCount.BinQty.ToString();
                        myNAVStockCount.ProdExtendedOnHandQty = stockCount.WarehouseQty.ToString();
                        myNAVStockCount.TotalOnHandQty = stockCount.TotalQty.ToString();
                        myNAVStockCount.UPC = stockCount.UPC.ToString();
                        myNAVStockCount.FIFODate = stockCount.ExpiryDate.ToString();
                        myNAVStockCount.Packsize = stockCount.UnitOfMeasureMultiplier.ToString();
                        myNAVStockCount.OnHandQtyPackSize = stockCount.QtyInPacksize.ToString();
                        myNAVStockCount.ClientName = stockCount.ClientName.ToString();
                        myNAVStockCount.IsReserved = stockCount.IsReserved.ToString();
                        myNAVStockCount.AtShipQty = stockCount.AtShipQty.ToString();
                        myNAVStockCount.AtReceiveQty = stockCount.AtReceiveQty.ToString();
                        myNAVStockCount.Attribute1 = stockCount.Attribute1.ToString();
                        myNAVStockCount.Attribute2 = stockCount.Attribute2.ToString();
                        myNAVStockCount.Attribute3 = stockCount.Attribute3.ToString();
                        myNAVStockCount.Attribute4 = stockCount.Attribute4.ToString();
                        myNAVStockCount.Attribute5 = stockCount.Attribute5.ToString();
                        myNAVStockCount.Attribute6 = stockCount.Attribute6.ToString();
                        myNAVStockCount.Attribute7 = stockCount.Attribute7.ToString();
                        myNAVStockCount.Attribute8 = stockCount.Attribute8.ToString();
                        myNAVStockCount.Attribute9 = stockCount.Attribute9.ToString();
                        myNAVStockCount.Attribute10 = stockCount.Attribute10.ToString();
                        myNAVStockCount.RowId = stockCount.RowID.ToString();

                        //Add Stock Count to the list of Stock Counts
                        myNAVStockCounts.Add(myNAVStockCount);

                        myRowID = stockCount.RowID.ToString();

                        //EventNotificationClient.Instance.PostMessage("NAV.Upload.StockCount.WriteToNAV", CompanyConfig.InstanceId, "Submitting XC to NAV");   

                        //Send Stock Count to NAV 
                        myNAVStockCountRoot.NAVStockCount = myNAVStockCounts.ToArray();

                        EventNotificationClient.Instance.PostMessage("NAV.Upload.StockCount", CompanyConfig.InstanceId,
                            "Submitting Count to NAV", new Variable("data", myNAVStockCountRoot.ToString()),
                            new Variable("xml data", Utils.SerializeToString(myNAVStockCountRoot.NAVStockCount[0])));

                        myResult = CompanyConfig.Instance.NavWebServiceInstance.ImportStockCount(myNAVStockCountRoot);

                        if (myResult == "OK")
                        {
                            EventNotificationClient.Instance.PostMessage("NAV.Upload.StockCount", CompanyConfig.InstanceId,
                                "Stock Count Uploaded", new Variable ("Stock Count Uploaded (Row ID)", myRowID));

                            myUploadConfirmation.IsSuccess = true;
                            myUploadConfirmation.RowId = myRowID;
                            myUploadConfirmation.ErrorMessage = String.Empty;
                            uploadConfirmations.Add(myUploadConfirmation);
                        }
                        else
                        {
                            EventNotificationClient.Instance.PostMessage("NAV.Upload.StockCount.Error", CompanyConfig.InstanceId, 
                                "Error", new Variable ("StockCount [A]", myRowID));

                            myUploadConfirmation.IsSuccess = false;
                            myUploadConfirmation.RowId = myRowID;
                            myUploadConfirmation.ErrorMessage = "Upload to NAV failed.";
                            uploadConfirmations.Add(myUploadConfirmation);
                        }
                    }
                    catch (Exception ex)
                    {
                        EventNotificationClient.Instance.PostMessage("NAV.Upload.StockCount.Error", CompanyConfig.InstanceId,
                            "Error", new Variable("StockCount [B]", ex.ToString()));

                        myUploadConfirmation.IsSuccess = false;
                        myUploadConfirmation.RowId = myRowID;
                        myUploadConfirmation.ErrorMessage = ex.ToString();
                        uploadConfirmations.Add(myUploadConfirmation);
                    }
                }
            }
            catch (Exception ex)
            {
                EventNotificationClient.Instance.PostMessage("NAV.Upload.StockCount.Error", CompanyConfig.InstanceId,
                    "Error", new Variable("StockCount [C]", ex.ToString()));

                myUploadConfirmation = new UploadConfirmation();
                myUploadConfirmation.IsSuccess = false;
                myUploadConfirmation.RowId = myRowID;
                myUploadConfirmation.ErrorMessage = ex.ToString();
                uploadConfirmations.Add(myUploadConfirmation);
                return uploadConfirmations;
            }

            //EventNotificationClient.Instance.PostMessage("NAV.Upload.StockCount.WriteToNAV", CompanyConfig.InstanceId,
            //    string.Format("Existing WriteToNAV", ""));
            return uploadConfirmations;
        }
        # endregion
    }
}

