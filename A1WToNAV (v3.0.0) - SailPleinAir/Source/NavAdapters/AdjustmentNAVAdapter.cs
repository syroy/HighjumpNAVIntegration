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
	public class AdjustmentNAVAdapter
    {
        #region SVA WriteToNAV
        public static List<UploadConfirmation> WriteToNAV(List<Adjustment> adjustments)
        {
            //EventNotificationClient.Instance.PostMessage("NAV.Upload.Adjustment.WriteToNAV", CompanyConfig.InstanceId, "Entering Function");   

            List<UploadConfirmation> uploadConfirmations = new List<UploadConfirmation>();
            UploadConfirmation myUploadConfirmation;
            List<string> myRowIDs = new List<string>();

            try
            {
                NAVAdjustmentRoot myNAVAdjustmentRoot = new NAVAdjustmentRoot();
                List<NAVAdjustment> myNAVAdjustments = new List<NAVAdjustment>();
                NAVAdjustment myNAVAdjustment;
                String myResult = String.Empty;

                foreach (Adjustment adjustment in adjustments)
                {
                    myUploadConfirmation = new UploadConfirmation();

                    try
                    {
                        //NAV XMLPort Note:  Items must be assigned in the Column Order that is defined in the XMLPort object

                        //Start with new NAV Adjustment
                        myNAVAdjustment = new NAVAdjustment();

                        //EventNotificationClient.Instance.PostMessage("NAV.Upload.Adjustment.WriteToNAV", CompanyConfig.InstanceId, "Parsing Transfer Confirmations");

                        //Parse Adjustment Record
                        //myNAVAdjustment = adjustment.ReplaceText.ToString();
                        myNAVAdjustment.RecordType = "MT";
                        myNAVAdjustment.UploadType = "MT";
                        myNAVAdjustment.TransactionCode = adjustment.TransactionCode.ToString();
                        myNAVAdjustment.Warehouse = adjustment.Warehouse.ToString();
                        myNAVAdjustment.ProductCode = adjustment.ProductCode.ToString();
                        myNAVAdjustment.Bin = adjustment.BinLocation.ToString();
                        myNAVAdjustment.InOrOut = adjustment.Quantity < 0 ? "-" : "+";
                        myNAVAdjustment.Quantity = Math.Abs(adjustment.Quantity).ToString();
                        myNAVAdjustment.CustomerVendor = adjustment.CustomerVendor.ToString();
                        myNAVAdjustment.OrderNum = adjustment.OrderNum.ToString();
                        myNAVAdjustment.RMA = adjustment.RMA.ToString();
                        myNAVAdjustment.ReasonCode = adjustment.ReasonCode.ToString();
                        myNAVAdjustment.Comment = adjustment.Comment.ToString();
                        myNAVAdjustment.OperatorName = adjustment.OperatorName.ToString();
                        myNAVAdjustment.AdjustmentDate = adjustment.AdjustmentDate.ToString();
                        myNAVAdjustment.UPC = adjustment.UPC.ToString();

                        Accellos.Interfaces.NAV.NAVWebServices.AttributeSet tempSet = new Accellos.Interfaces.NAV.NAVWebServices.AttributeSet();
                        tempSet.Attribute1 = adjustment.Attribute1.ToString();
                        tempSet.Attribute2 = adjustment.Attribute2.ToString();
                        tempSet.Attribute3 = adjustment.Attribute3.ToString();
                        tempSet.Attribute4 = adjustment.Attribute4.ToString();
                        tempSet.Attribute5 = adjustment.Attribute5.ToString();
                        tempSet.Attribute6 = adjustment.Attribute6.ToString();
                        tempSet.Attribute7 = adjustment.Attribute7.ToString();
                        tempSet.Attribute8 = adjustment.Attribute8.ToString();
                        tempSet.Attribute9 = adjustment.Attribute9.ToString();
                        tempSet.Attribute10 = adjustment.Attribute10.ToString();
                        myNAVAdjustment.AttributeSet = tempSet;

                        myNAVAdjustment.Zone = adjustment.Zone.ToString();
                        myNAVAdjustment.ExpiryDate = adjustment.ExpiryDate.ToString();
                        myNAVAdjustment.OrderReference = adjustment.OrderReference.ToString();
                        myNAVAdjustment.Special1 = adjustment.Special1.ToString();
                        myNAVAdjustment.Special2 = adjustment.Special2.ToString();
                        myNAVAdjustment.Special3 = adjustment.Special3.ToString();
                        myNAVAdjustment.ClientName = adjustment.ClientName.ToString();
                        myNAVAdjustment.Extra1 = adjustment.Extra1.ToString();
                        myNAVAdjustment.Extra2 = adjustment.Extra2.ToString();
                        myNAVAdjustment.Extra3 = adjustment.Extra3.ToString();
                        myNAVAdjustment.Extra4 = adjustment.Extra4.ToString();
                        myNAVAdjustment.Extra5 = adjustment.Extra5.ToString();
                        myNAVAdjustment.Special4 = adjustment.Special4.ToString();
                        myNAVAdjustment.Special5 = adjustment.Special5.ToString();
                        myNAVAdjustment.Special6 = adjustment.Special6.ToString();
                        myNAVAdjustment.TotWarehouseQty = "";  //6.4 - added, no wms field for this
                        myNAVAdjustment.TotReservedQty = "";   //6.4 - added, no wms field for this
                        myNAVAdjustment.TotUnpickedOrders = "";    //6.4 - added, no wms field for this
                        myNAVAdjustment.LineNum = adjustment.LineNum.ToString();
                        myNAVAdjustment.AddTime = adjustment.AddTime.ToString();
                        myNAVAdjustment.ItemCost = adjustment.ItemCost.ToString();
                        myNAVAdjustment.TransferOrderReference = adjustment.TransferOrderReference.ToString();
                        myNAVAdjustment.TransferOrderType = adjustment.TransferOrderType.ToString();
                        myNAVAdjustment.Packsize = adjustment.Packsize.ToString();
                        myNAVAdjustment.LicensePlate = adjustment.LicensePlate.ToString();
                        myNAVAdjustment.Reserved = adjustment.Reserved.ToString();
                        myNAVAdjustment.RowId = adjustment.RowId.ToString();

                        //Add Adjustment to the list of Adjustments
                        myNAVAdjustments.Add(myNAVAdjustment);

                        myRowIDs.Add(adjustment.RowId);
                    }
                    catch (Exception ex)
                    {
                        EventNotificationClient.Instance.PostMessage("NAV.Upload.Adjustment.Error", CompanyConfig.InstanceId,
                            "Error | " + ex.Message, new Variable("Adjustment [B]", ex.ToString()));

                        myUploadConfirmation.IsSuccess = false;
                        myUploadConfirmation.RowId = adjustment.RowId;
                        myUploadConfirmation.ErrorMessage = ex.ToString();
                        uploadConfirmations.Add(myUploadConfirmation);
                    }
                }

                //Send Adjustment to NAV 
                myNAVAdjustmentRoot.NAVAdjustment = myNAVAdjustments.ToArray();

                EventNotificationClient.Instance.PostMessage("NAV.Upload.Adjustment", CompanyConfig.InstanceId,
                    "Submitting Pick to NAV", new Variable("data", myNAVAdjustmentRoot.ToString()),
                    new Variable("xml data", Utils.SerializeToString(myNAVAdjustmentRoot.NAVAdjustment[0])));

                myResult = CompanyConfig.Instance.NavWebServiceInstance.ImportAdjustment(myNAVAdjustmentRoot);
                CompanyConfig.Instance.NavWebServiceInstance.Dispose();

                if (myResult == "OK")
                {
                    foreach (string rowId in myRowIDs)
                    {
                        EventNotificationClient.Instance.PostMessage("NAV.Upload.Adjustment", CompanyConfig.InstanceId,
                            "Adjustment Uploaded", new Variable("Adjustment Uploaded", rowId));

                        myUploadConfirmation = new UploadConfirmation();
                        myUploadConfirmation.IsSuccess = true;
                        myUploadConfirmation.RowId = rowId;
                        myUploadConfirmation.ErrorMessage = String.Empty;
                        uploadConfirmations.Add(myUploadConfirmation);
                    }
                }
                else
                {

                    foreach (string rowId in myRowIDs)
                    {
                        EventNotificationClient.Instance.PostMessage("NAV.Upload.Adjustment.Error", CompanyConfig.InstanceId,
                            "Error", new Variable("Adjustment [A]", rowId));

                        myUploadConfirmation = new UploadConfirmation();
                        myUploadConfirmation.IsSuccess = false;
                        myUploadConfirmation.RowId = rowId;
                        myUploadConfirmation.ErrorMessage = "Upload to NAV failed.";
                        uploadConfirmations.Add(myUploadConfirmation);
                    }
                }
            }
            catch (Exception ex)
            {
                EventNotificationClient.Instance.PostMessage("NAV.Upload.Adjustment.Error", CompanyConfig.InstanceId,
                    "Error | " + ex.Message, new Variable("Adjustment [C]", ex.ToString()));

                myUploadConfirmation = new UploadConfirmation();
                myUploadConfirmation.IsSuccess = false;
                myUploadConfirmation.RowId = "";
                myUploadConfirmation.ErrorMessage = ex.ToString();
                uploadConfirmations.Add(myUploadConfirmation);
                //return uploadConfirmations;
            }

            //EventNotificationClient.Instance.PostMessage("NAV.Upload.Adjustment.WriteToNAV", CompanyConfig.InstanceId,
            //    string.Format("Existing WriteToNAV", ""));
            return uploadConfirmations;
        }
        # endregion
	}
}

