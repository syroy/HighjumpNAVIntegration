using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Accellos.InterfaceFramework;
using Accellos.InterfaceFramework.EventNotificationServer;
using Accellos.InterfaceFramework.ScriptManagementServer;
using Accellos.Interfaces.NAV.BusinessObjects;
using Accellos.Interfaces.NAV.NAVWebServices;
using Accellos.Interfaces.NAV.WMSAdapters;

namespace Accellos.Interfaces.NAV.NAVAdapters
{
    public class RmaNAVAdapter
    {
        #region SVA RetrieveFromNAV
        public static List<PurchaseOrder> RetrieveFromNAV()
        {
            NAVRMARoot myNAVRMARoot = new NAVRMARoot();
            List<PurchaseOrder> myWMSRMAOrders = new List<PurchaseOrder>();
            bool didFailureOccur;

            String myResult = String.Empty;
            try
            {
                PurchaseOrder myWMSRMAOrder;
                PurchaseOrderLine myWMSPurchaseOrderLine;

                // Call NAV Web Service | ExportRMA
                myResult = CompanyConfig.Instance.NavWebServiceInstance.ExportRMAOrder(ref myNAVRMARoot);
                CompanyConfig.Instance.NavWebServiceInstance.Dispose();
                EventNotificationClient.Instance.PostMessage("NAV.Download.RMAOrders", CompanyConfig.InstanceId, "Exported NAV Data",
                    new Variable("data", Utils.SerializeToString(myNAVRMARoot)));

                //NAV Business Object | Code = RH
                NAVBusinessObjectRMA myNAVBusinessObjectRMA = new NAVBusinessObjectRMA();
                myNAVBusinessObjectRMA = myNAVRMARoot.NAVBusinessObjectRMA;

                //NAV Customer Returns | Processed = Ready to Send
                NAVCustomerReturns myNAVCustomerReturns = new NAVCustomerReturns();
                myNAVCustomerReturns = myNAVBusinessObjectRMA.NAVCustomerReturns;

                try
                {
                    if (myNAVCustomerReturns.NAVCustomerReturn == null)
                    {
                        //There aren't any orders to process
                        myWMSRMAOrders = new List<PurchaseOrder>();
                        return myWMSRMAOrders;
                    }

                    // For each Customer Return record in the queue
                    foreach (NAVCustomerReturn myNAVCustomerReturn in myNAVCustomerReturns.NAVCustomerReturn)
                    {
                        NAVRmaOrders myNAVRMAOrders = new NAVRmaOrders();
                        myNAVRMAOrders = myNAVCustomerReturn.NAVRmaOrders;

                        NAVRmaOrder myNAVOrder = new NAVRmaOrder();
                        myNAVOrder = myNAVRMAOrders.NAVRmaOrder;

                        myWMSRMAOrder = new PurchaseOrder();

                        // Parse Purchase Order Header
                        myWMSRMAOrder.AllowBackOrder = NAVAdapterUtils.NAVFormatBoolean(myNAVOrder.AllowBackOrder.ToString());
                        myWMSRMAOrder.BackOrder = NAVAdapterUtils.NAVFormatString(myNAVOrder.BackOrder.ToString(), 40);
                        myWMSRMAOrder.BuyerName = NAVAdapterUtils.NAVFormatString(myNAVOrder.BuyerName.ToString(), 40);
                        myWMSRMAOrder.ClientName = NAVAdapterUtils.getClientName();
                        myWMSRMAOrder.ContainerBatchRef = NAVAdapterUtils.NAVFormatString(myNAVOrder.ContainerBatchRef.ToString(), 40);
                        myWMSRMAOrder.DueDate = NAVAdapterUtils.NAVFormatDateTime(myNAVOrder.DueDate.ToString());
                        myWMSRMAOrder.Email = NAVAdapterUtils.NAVFormatString(myNAVOrder.Email.ToString(), 40);
                        myWMSRMAOrder.Extra1 = NAVAdapterUtils.NAVFormatString(myNAVOrder.Extra1.ToString(), 40);
                        myWMSRMAOrder.Extra2 = NAVAdapterUtils.NAVFormatString(myNAVOrder.Extra2.ToString(), 40);
                        myWMSRMAOrder.Extra3 = NAVAdapterUtils.NAVFormatString(myNAVOrder.Extra3.ToString(), 40);
                        myWMSRMAOrder.Extra4 = NAVAdapterUtils.NAVFormatString(myNAVOrder.Extra4.ToString(), 40);
                        myWMSRMAOrder.Extra5 = NAVAdapterUtils.NAVFormatString(myNAVOrder.Extra5.ToString(), 40);
                        myWMSRMAOrder.IsRMA = NAVAdapterUtils.NAVFormatBoolean(myNAVOrder.IsRMA.ToString());
                        myWMSRMAOrder.NoSHReship = NAVAdapterUtils.NAVFormatString(myNAVOrder.NoSHReship.ToString(), 40);
                        myWMSRMAOrder.OrderSource = NAVAdapterUtils.NAVFormatString(myNAVOrder.OrderSource.ToString(), 40);
                        myWMSRMAOrder.Packslip = NAVAdapterUtils.NAVFormatString(myNAVOrder.Packslip.ToString(), 40);
                        myWMSRMAOrder.PoType = NAVAdapterUtils.NAVFormatPOType(myNAVOrder.PoType.ToString());
                        myWMSRMAOrder.PurchaseOrderNumber = NAVAdapterUtils.NAVFormatString(myNAVOrder.PurchaseOrderNumber.ToString(), 40);
                        myWMSRMAOrder.RequestPOD = NAVAdapterUtils.NAVFormatString(myNAVOrder.RequestPOD.ToString(), 40);
                        myWMSRMAOrder.RequestSH = NAVAdapterUtils.NAVFormatString(myNAVOrder.CreditSH.ToString(), 40);
                        myWMSRMAOrder.RequisitionNumber = NAVAdapterUtils.NAVFormatString(myNAVOrder.RequisitionNumber.ToString(), 40);
                        myWMSRMAOrder.SpecialInstructions1 = NAVAdapterUtils.NAVFormatString(myNAVOrder.SpecialIntructions1.ToString(), 40);
                        myWMSRMAOrder.SpecialInstructions2 = NAVAdapterUtils.NAVFormatString(myNAVOrder.SpecialIntructions2.ToString(), 40);
                        myWMSRMAOrder.SpecialInstructions3 = NAVAdapterUtils.NAVFormatString(myNAVOrder.SpecialIntructions3.ToString(), 40);
                        myWMSRMAOrder.SpecialInstructions4 = NAVAdapterUtils.NAVFormatString(myNAVOrder.SpecialIntructions4.ToString(), 40);
                        myWMSRMAOrder.SpecialInstructions5 = NAVAdapterUtils.NAVFormatString(myNAVOrder.SpecialIntructions5.ToString(), 40);
                        myWMSRMAOrder.SpecialInstructions6 = NAVAdapterUtils.NAVFormatString(myNAVOrder.SpecialIntructions6.ToString(), 40);
                        myWMSRMAOrder.SpecialInstructions7 = NAVAdapterUtils.NAVFormatString(myNAVOrder.SpecialIntructions7.ToString(), 40);
                        myWMSRMAOrder.SVAStagingID = myNAVOrder.StagingID.ToString();
                        myWMSRMAOrder.VendorName = NAVAdapterUtils.NAVFormatString(myNAVOrder.VendorName.ToString(), 40);
                        myWMSRMAOrder.VendorNumber = NAVAdapterUtils.NAVFormatString(myNAVOrder.VendorNumber.ToString(), 40);
                        myWMSRMAOrder.WarehouseTransferLocation = NAVAdapterUtils.NAVFormatString(myNAVOrder.WarehouseTransferLocation.ToString(), 40);
                        myWMSRMAOrder.IntegrationUDF1 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF1.ToString(), 40);
                        myWMSRMAOrder.IntegrationUDF2 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF2.ToString(), 40);
                        myWMSRMAOrder.IntegrationUDF3 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF3.ToString(), 40);
                        myWMSRMAOrder.IntegrationUDF4 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF4.ToString(), 40);
                        myWMSRMAOrder.IntegrationUDF5 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF5.ToString(), 40);
                        myWMSRMAOrder.IntegrationUDF6 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF6.ToString(), 40);
                        myWMSRMAOrder.IntegrationUDF7 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF7.ToString(), 40);
                        myWMSRMAOrder.IntegrationUDF8 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF8.ToString(), 40);
                        myWMSRMAOrder.IntegrationUDF9 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF9.ToString(), 40);
                        myWMSRMAOrder.IntegrationUDF10 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF10.ToString(), 40);
                        myWMSRMAOrder.IntegrationUDF11 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF11.ToString(), 40);
                        myWMSRMAOrder.IntegrationUDF12 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF12.ToString(), 40);
                        myWMSRMAOrder.IntegrationUDF13 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF13.ToString(), 40);
                        myWMSRMAOrder.IntegrationUDF14 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF14.ToString(), 40);
                        myWMSRMAOrder.IntegrationUDF15 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF15.ToString(), 40);
                        myWMSRMAOrder.IntegrationUDF16 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF16.ToString(), 40);
                        myWMSRMAOrder.IntegrationUDF17 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF17.ToString(), 40);
                        myWMSRMAOrder.IntegrationUDF18 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF18.ToString(), 40);
                        myWMSRMAOrder.IntegrationUDF19 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF19.ToString(), 40);
                        myWMSRMAOrder.IntegrationUDF20 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF20.ToString(), 40);
                        myWMSRMAOrder.IntegrationUDF21 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF21.ToString(), 40);
                        myWMSRMAOrder.IntegrationUDF22 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF22.ToString(), 40);
                        myWMSRMAOrder.IntegrationUDF23 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF23.ToString(), 40);
                        myWMSRMAOrder.IntegrationUDF24 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF24.ToString(), 40);
                        myWMSRMAOrder.IntegrationUDF25 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF25.ToString(), 40);

                        didFailureOccur = false;

                        try
                        {
                            NAVRmaLines myNAVRMALines = myNAVOrder.NAVRmaLines;

                            foreach (NAVRmaLine myNAVLine in myNAVRMALines.NAVRmaLine)
                            {
                                myWMSPurchaseOrderLine = new PurchaseOrderLine();

                                // Parse Purchase Order Lines
                                NAV.BusinessObjects.AttributeSet mySet = new NAV.BusinessObjects.AttributeSet();
                                mySet.Attribute1 = NAVAdapterUtils.NAVFormatString(myNAVLine.Attribute1.ToString(), 40);
                                mySet.Attribute2 = NAVAdapterUtils.NAVFormatString(myNAVLine.Attribute2.ToString(), 40);
                                mySet.Attribute3 = NAVAdapterUtils.NAVFormatString(myNAVLine.Attribute3.ToString(), 40);
                                mySet.Attribute4 = NAVAdapterUtils.NAVFormatString(myNAVLine.Attribute4.ToString(), 40);
                                mySet.Attribute5 = NAVAdapterUtils.NAVFormatString(myNAVLine.Attribute5.ToString(), 40);
                                mySet.Attribute6 = NAVAdapterUtils.NAVFormatString(myNAVLine.Attribute6.ToString(), 40);
                                mySet.Attribute7 = NAVAdapterUtils.NAVFormatString(myNAVLine.Attribute7.ToString(), 40);
                                mySet.Attribute8 = NAVAdapterUtils.NAVFormatString(myNAVLine.Attribute8.ToString(), 40);
                                mySet.Attribute9 = NAVAdapterUtils.NAVFormatString(myNAVLine.Attribute9.ToString(), 40);
                                mySet.Attribute10 = NAVAdapterUtils.NAVFormatString(myNAVLine.Attribute10.ToString(), 40);
                                myWMSPurchaseOrderLine.Attributes.Add(mySet);

                                myWMSPurchaseOrderLine.ContainerBatchRef = NAVAdapterUtils.NAVFormatString(myNAVLine.ContainerBatchRef.ToString(), 40);
                                myWMSPurchaseOrderLine.ContainerStatus = NAVAdapterUtils.NAVFormatString(myNAVLine.ContainerStatus.ToString(), 40);
                                myWMSPurchaseOrderLine.CostPrice = NAVAdapterUtils.NAVFormatDecimal(myNAVLine.CostPrice.ToString());
                                myWMSPurchaseOrderLine.CountryOfOrigin = NAVAdapterUtils.NAVFormatString(myNAVLine.CountryOfOrigin.ToString(), 40);
                                myWMSPurchaseOrderLine.CreditNow = NAVAdapterUtils.NAVFormatBoolean(myNAVLine.CreditNow.ToString());
                                myWMSPurchaseOrderLine.CustomerName = NAVAdapterUtils.NAVFormatString(myNAVLine.CustomerName.ToString(), 40);
                                myWMSPurchaseOrderLine.CustomerNumber = NAVAdapterUtils.NAVFormatString(myNAVLine.CustomerNumber.ToString(), 40);
                                myWMSPurchaseOrderLine.Description = NAVAdapterUtils.NAVFormatString(myNAVLine.Description.ToString(), 100); // Changed to 100 characters for A1W 64 - iCepts 10/6/15
                                myWMSPurchaseOrderLine.Discount = NAVAdapterUtils.NAVFormatDecimal(myNAVLine.Discount.ToString());
                                myWMSPurchaseOrderLine.ExpectedRequiredDate = NAVAdapterUtils.NAVFormatDateTime(myNAVLine.ExpectedRequiredDate.ToString());
                                myWMSPurchaseOrderLine.ExpiryDate = NAVAdapterUtils.NAVFormatDateTime(myNAVLine.ExpiryDate.ToString());
                                myWMSPurchaseOrderLine.Extra1 = NAVAdapterUtils.NAVFormatString(myNAVLine.Extra1.ToString(), 40);
                                myWMSPurchaseOrderLine.Extra2 = NAVAdapterUtils.NAVFormatString(myNAVLine.Extra2.ToString(), 40);
                                myWMSPurchaseOrderLine.Extra3 = NAVAdapterUtils.NAVFormatString(myNAVLine.Extra3.ToString(), 40);
                                myWMSPurchaseOrderLine.Extra4 = NAVAdapterUtils.NAVFormatString(myNAVLine.Extra4.ToString(), 40);
                                myWMSPurchaseOrderLine.Extra5 = NAVAdapterUtils.NAVFormatString(myNAVLine.Extra5.ToString(), 40);
                                myWMSPurchaseOrderLine.HostLineReference = NAVAdapterUtils.NAVFormatString(myNAVLine.HostLineReference.ToString(), 40);
                                myWMSPurchaseOrderLine.IsSpecial = NAVAdapterUtils.NAVFormatBoolean(myNAVLine.IsSpecial.ToString());
                                myWMSPurchaseOrderLine.LineNumber = NAVAdapterUtils.NAVFormatString(myNAVLine.LineNumber.ToString(), 40);
                                myWMSPurchaseOrderLine.Location = NAVAdapterUtils.NAVFormatString(myNAVLine.Location.ToString(), 40);
                                myWMSPurchaseOrderLine.OverReceiptPercentage = NAVAdapterUtils.NAVFormatString(myNAVLine.OverReceivePercent.ToString(), 40);
                                myWMSPurchaseOrderLine.PrimaryLocation = NAVAdapterUtils.NAVFormatString(myNAVLine.PrimaryBin.ToString(), 40);
                                myWMSPurchaseOrderLine.ProductClass = NAVAdapterUtils.NAVFormatString(myNAVLine.ProductClass.ToString(), 40);
                                myWMSPurchaseOrderLine.ProductCode = NAVAdapterUtils.NAVFormatString(myNAVLine.ProductCode.ToString(), 40);
                                myWMSPurchaseOrderLine.PurchaseOrderNumber = NAVAdapterUtils.NAVFormatString(myNAVLine.PurchaseOrderNumber.ToString(), 40);
                                myWMSPurchaseOrderLine.QuantityExpected = NAVAdapterUtils.NAVFormatDecimal(myNAVLine.QuantityExpected.ToString());
                                myWMSPurchaseOrderLine.ReceiveAttributeTracking.Attribute1Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVLine.Attribute1Tracking.ToString());
                                myWMSPurchaseOrderLine.ReceiveAttributeTracking.Attribute2Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVLine.Attribute2Tracking.ToString());
                                myWMSPurchaseOrderLine.ReceiveAttributeTracking.Attribute3Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVLine.Attribute3Tracking.ToString());
                                myWMSPurchaseOrderLine.ReceiveAttributeTracking.Attribute4Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVLine.Attribute4Tracking.ToString());
                                myWMSPurchaseOrderLine.ReceiveAttributeTracking.Attribute5Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVLine.Attribute5Tracking.ToString());
                                myWMSPurchaseOrderLine.ReceiveAttributeTracking.Attribute6Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVLine.Attribute6Tracking.ToString());
                                myWMSPurchaseOrderLine.ReceiveAttributeTracking.Attribute7Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVLine.Attribute7Tracking.ToString());
                                myWMSPurchaseOrderLine.ReceiveAttributeTracking.Attribute8Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVLine.Attribute8Tracking.ToString());
                                myWMSPurchaseOrderLine.ReceiveAttributeTracking.Attribute9Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVLine.Attribute9Tracking.ToString());
                                myWMSPurchaseOrderLine.ReceiveAttributeTracking.Attribute10Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVLine.Attribute10Tracking.ToString());
                                myWMSPurchaseOrderLine.ReInvoice = NAVAdapterUtils.NAVFormatBoolean(myNAVLine.ReInvoice.ToString());
                                myWMSPurchaseOrderLine.Reship = NAVAdapterUtils.NAVFormatInt(myNAVLine.Reship.ToString());
                                myWMSPurchaseOrderLine.RMAReason = NAVAdapterUtils.NAVFormatString(myNAVLine.RMAReason.ToString(), 40);
                                myWMSPurchaseOrderLine.RMARestockCharge = NAVAdapterUtils.NAVFormatString(myNAVLine.RMARestockCharge.ToString(), 40);                               //myWMSPurchaseOrderLine = NAVAdapterUtils.NAVFormatString(myNAVLine
                                myWMSPurchaseOrderLine.SalesOrderLineNumber = NAVAdapterUtils.NAVFormatString(myNAVLine.SalesOrderLineNumber.ToString(), 40);
                                myWMSPurchaseOrderLine.SalesOrderNumber = NAVAdapterUtils.NAVFormatString(myNAVLine.SalesOrderNumber.ToString(), 40);
                                myWMSPurchaseOrderLine.SpecialInstructions1 = NAVAdapterUtils.NAVFormatString(myNAVLine.SpecialInstructions1.ToString(), 40);
                                myWMSPurchaseOrderLine.SpecialInstructions2 = NAVAdapterUtils.NAVFormatString(myNAVLine.SpecialInstructions2.ToString(), 40);
                                myWMSPurchaseOrderLine.SpecialInstructions3 = NAVAdapterUtils.NAVFormatString(myNAVLine.SpecialInstructions3.ToString(), 40);
                                myWMSPurchaseOrderLine.SpecialInstructions4 = NAVAdapterUtils.NAVFormatString(myNAVLine.SpecialInstructions4.ToString(), 40);
                                myWMSPurchaseOrderLine.SpecialInstructions5 = NAVAdapterUtils.NAVFormatString(myNAVLine.SpecialInstructions5.ToString(), 40);
                                myWMSPurchaseOrderLine.SpecialInstructions6 = NAVAdapterUtils.NAVFormatString(myNAVLine.SpecialInstructions6.ToString(), 40);
                                myWMSPurchaseOrderLine.SpecialInstructions7 = NAVAdapterUtils.NAVFormatString(myNAVLine.SpecialInstructions7.ToString(), 40);
                                myWMSPurchaseOrderLine.StockItemIndicator = NAVAdapterUtils.NAVFormatStockItemIndicatorType(myNAVLine.StockItemIndicator.ToString());
                                myWMSPurchaseOrderLine.UnitOfMeasureMultiplier = NAVAdapterUtils.NAVFormatString(myNAVLine.UnitOfMeasureMultiplier.ToString(), 40);
                                myWMSPurchaseOrderLine.UnitOfMeasureText = NAVAdapterUtils.NAVFormatString(myNAVLine.UnitOfMeasureText.ToString(), 40);
                                myWMSPurchaseOrderLine.UnitPrice = NAVAdapterUtils.NAVFormatDecimal(myNAVLine.UnitPrice.ToString());
                                myWMSPurchaseOrderLine.UPC = NAVAdapterUtils.NAVFormatString(myNAVLine.UPC.ToString(), 40);
                                myWMSPurchaseOrderLine.VendorProductNumber = NAVAdapterUtils.NAVFormatString(myNAVLine.VendorProductNumber.ToString(), 40);
                                myWMSPurchaseOrderLine.Warehouse = NAVAdapterUtils.NAVFormatString(myNAVLine.Warehouse.ToString(), 40);
                                myWMSPurchaseOrderLine.IntegrationUDF1 = NAVAdapterUtils.NAVFormatString(myNAVLine.IntegrationUDF1.ToString(), 40);
                                myWMSPurchaseOrderLine.IntegrationUDF2 = NAVAdapterUtils.NAVFormatString(myNAVLine.IntegrationUDF2.ToString(), 40);
                                myWMSPurchaseOrderLine.IntegrationUDF3 = NAVAdapterUtils.NAVFormatString(myNAVLine.IntegrationUDF3.ToString(), 40);
                                myWMSPurchaseOrderLine.IntegrationUDF4 = NAVAdapterUtils.NAVFormatString(myNAVLine.IntegrationUDF4.ToString(), 40);
                                myWMSPurchaseOrderLine.IntegrationUDF5 = NAVAdapterUtils.NAVFormatString(myNAVLine.IntegrationUDF5.ToString(), 40);
                                myWMSPurchaseOrderLine.IntegrationUDF6 = NAVAdapterUtils.NAVFormatString(myNAVLine.IntegrationUDF6.ToString(), 40);
                                myWMSPurchaseOrderLine.IntegrationUDF7 = NAVAdapterUtils.NAVFormatString(myNAVLine.IntegrationUDF7.ToString(), 40);
                                myWMSPurchaseOrderLine.IntegrationUDF8 = NAVAdapterUtils.NAVFormatString(myNAVLine.IntegrationUDF8.ToString(), 40);
                                myWMSPurchaseOrderLine.IntegrationUDF9 = NAVAdapterUtils.NAVFormatString(myNAVLine.IntegrationUDF9.ToString(), 40);
                                myWMSPurchaseOrderLine.IntegrationUDF10 = NAVAdapterUtils.NAVFormatString(myNAVLine.IntegrationUDF10.ToString(), 40);
                                myWMSPurchaseOrderLine.IntegrationUDF11 = NAVAdapterUtils.NAVFormatString(myNAVLine.IntegrationUDF11.ToString(), 40);
                                myWMSPurchaseOrderLine.IntegrationUDF12 = NAVAdapterUtils.NAVFormatString(myNAVLine.IntegrationUDF12.ToString(), 40);
                                myWMSPurchaseOrderLine.IntegrationUDF13 = NAVAdapterUtils.NAVFormatString(myNAVLine.IntegrationUDF13.ToString(), 40);
                                myWMSPurchaseOrderLine.IntegrationUDF14 = NAVAdapterUtils.NAVFormatString(myNAVLine.IntegrationUDF14.ToString(), 40);
                                myWMSPurchaseOrderLine.IntegrationUDF15 = NAVAdapterUtils.NAVFormatString(myNAVLine.IntegrationUDF15.ToString(), 40);
                                myWMSPurchaseOrderLine.IntegrationUDF16 = NAVAdapterUtils.NAVFormatString(myNAVLine.IntegrationUDF16.ToString(), 40);
                                myWMSPurchaseOrderLine.IntegrationUDF17 = NAVAdapterUtils.NAVFormatString(myNAVLine.IntegrationUDF17.ToString(), 40);
                                myWMSPurchaseOrderLine.IntegrationUDF18 = NAVAdapterUtils.NAVFormatString(myNAVLine.IntegrationUDF18.ToString(), 40);
                                myWMSPurchaseOrderLine.IntegrationUDF19 = NAVAdapterUtils.NAVFormatString(myNAVLine.IntegrationUDF19.ToString(), 40);
                                myWMSPurchaseOrderLine.IntegrationUDF20 = NAVAdapterUtils.NAVFormatString(myNAVLine.IntegrationUDF20.ToString(), 40);
                                myWMSPurchaseOrderLine.IntegrationUDF21 = NAVAdapterUtils.NAVFormatString(myNAVLine.IntegrationUDF21.ToString(), 40);
                                myWMSPurchaseOrderLine.IntegrationUDF22 = NAVAdapterUtils.NAVFormatString(myNAVLine.IntegrationUDF22.ToString(), 40);
                                myWMSPurchaseOrderLine.IntegrationUDF23 = NAVAdapterUtils.NAVFormatString(myNAVLine.IntegrationUDF23.ToString(), 40);
                                myWMSPurchaseOrderLine.IntegrationUDF24 = NAVAdapterUtils.NAVFormatString(myNAVLine.IntegrationUDF24.ToString(), 40);
                                myWMSPurchaseOrderLine.IntegrationUDF25 = NAVAdapterUtils.NAVFormatString(myNAVLine.IntegrationUDF25.ToString(), 40);


                                myWMSRMAOrder.Lines.Add(myWMSPurchaseOrderLine);
                            }
                        }
                        catch (Exception exceptionRMALinesLvl)
                        {
                            //Error at RMA Lines Level
                            EventNotificationClient.Instance.PostMessage("NAV.Download.RMAOrders.Error", CompanyConfig.InstanceId, "Error: RMA Order Lines Level", new Variable("Error Sending to WMS", exceptionRMALinesLvl));
                            didFailureOccur = true;
                        }

                        //Add RMA (if succeeded) to List of RMAs
                        if (!didFailureOccur)
                        {
                            myWMSRMAOrders.Add(myWMSRMAOrder);
                            EventNotificationClient.Instance.PostMessage("NAV.Download.RMAOrders", CompanyConfig.InstanceId, "A1I -> Status Update -> NAV", new Variable("Status", "Sent to WMS"), new Variable("Purchase Order Number", NAVAdapterUtils.NAVFormatString(myNAVOrder.PurchaseOrderNumber.ToString(), 40)), new Variable("Entry No", NAVAdapterUtils.NAVFormatString(myNAVOrder.StagingID.ToString(), 40)));
                            //try
                            //{
                            //    // Call NAV Web Service | SentStatus
                            //    CompanyConfig.Instance.NavWebServiceInstance.SentStatus(NAVAdapterUtils.NAVFormatString(myNAVOrder.StagingID.ToString(), 40), "RH");
                            //    CompanyConfig.Instance.NavWebServiceInstance.Dispose();
                            //}
                            //catch (Exception ex1)
                            //{
                            //    EventNotificationClient.Instance.PostMessage("NAV.Download.RMAOrders.Error", CompanyConfig.InstanceId, "Error: A1I Entry No -> NAV", new Variable("Error Sending to WMS", ex1));
                            //}
                        }
                    }
                }
                catch (Exception exceptionCRLvl)
                {
                    //Error at Customer Return Level
                    EventNotificationClient.Instance.PostMessage("NAV.Download.RMAOrders.Error", CompanyConfig.InstanceId, "Error: Customer Return Level", new Variable("Error Sending to WMS", exceptionCRLvl));
                }
            }
            catch (Exception exceptionRMARootLvl)
            {
                //Error at RMA Root Level
                EventNotificationClient.Instance.PostMessage("NAV.Download.RMAOrders.Error", CompanyConfig.InstanceId, "Error: RMA Root Level", new Variable("Error Sending to WMS", exceptionRMARootLvl));
            }

            return myWMSRMAOrders;
        }
        #endregion

        #region SVA RetrievalRequestFromNAV
        public static List<PurchaseOrder> RetrievalRequestFromNAV()
        {
            NAVRMARetrievalRoot myNAVRMAOrderRetrievalRoot = new NAVRMARetrievalRoot();
            List<PurchaseOrder> myWMSRMAOrders = new List<PurchaseOrder>();

            String myResult = String.Empty;
            try
            {
                PurchaseOrder myWMSRMAOrder;

                // Call NAV Web Service | ExportPurchaseOrder
                myResult = CompanyConfig.Instance.NavWebServiceInstance.RetrieveRMAOrder(ref myNAVRMAOrderRetrievalRoot);
                CompanyConfig.Instance.NavWebServiceInstance.Dispose();

                //NAV Business Object | Code = RH
                NAVBusinessObjectRMAR myNAVBusinessObjectRMAR = new NAVBusinessObjectRMAR();
                myNAVBusinessObjectRMAR = myNAVRMAOrderRetrievalRoot.NAVBusinessObjectRMAR;

                //NAV RMA Headers | Processed = Ready to Send
                NAVRMARetrievals myNAVRMARetrievals = new NAVRMARetrievals();
                myNAVRMARetrievals = myNAVBusinessObjectRMAR.NAVRMARetrievals;

                try
                {
                    // For each RMA Retrieval record in the queue
                    foreach (NAVRMARetrieval myNAVOrder in myNAVRMARetrievals.NAVRMARetrieval)
                    {
                        myWMSRMAOrder = new PurchaseOrder();

                        // Parse RMA Header
                        myWMSRMAOrder.SVAStagingID = myNAVOrder.StagingID.ToString();
                        myWMSRMAOrder.ClientName = NAVAdapterUtils.getClientName();
                        myWMSRMAOrder.PurchaseOrderNumber = NAVAdapterUtils.NAVFormatString(myNAVOrder.OrderNumber.ToString(), 40);

                        //Add RMA to List of RMAs
                        if (myNAVOrder.OrderNumber.ToString() != "")
                        {
                            myWMSRMAOrders.Add(myWMSRMAOrder);
                            EventNotificationClient.Instance.PostMessage("NAV.Download.RMAOrderRetrieval", CompanyConfig.InstanceId, "A1I -> Status Update -> NAV", new Variable("Status", "Sent to WMS"), new Variable("Purchase Order Number", NAVAdapterUtils.NAVFormatString(myNAVOrder.OrderNumber.ToString(), 40)), new Variable("Entry No", NAVAdapterUtils.NAVFormatString(myNAVOrder.StagingID.ToString(), 40)));
                            try
                            {
                                // Call NAV Web Service | SentStatus
                                CompanyConfig.Instance.NavWebServiceInstance.SentStatus(NAVAdapterUtils.NAVFormatString(myNAVOrder.StagingID.ToString(), 40), "RH");
                                CompanyConfig.Instance.NavWebServiceInstance.Dispose();
                            }
                            catch (Exception ex1)
                            {
                                EventNotificationClient.Instance.PostMessage("NAV.Download.RMAOrderRetrieval.Error", CompanyConfig.InstanceId, "Error: A1I Entry No -> NAV", new Variable("Error Sending to WMS", ex1));
                            }
                        }
                    }
                }
                catch (Exception exceptionPOLvl)
                {
                    //Error at RMA Order Level
                    EventNotificationClient.Instance.PostMessage("NAV.Download.RMAOrderRetrieval.Error", CompanyConfig.InstanceId, "Error: RMA Order Level", new Variable("Error Sending to WMS", exceptionPOLvl));
                }
            }
            catch (Exception exceptionPORetRootLvl)
            {
                //Error at RMA Order Retrieval Root Level
                EventNotificationClient.Instance.PostMessage("NAV.Download.RMAOrderRetrieval.Error", CompanyConfig.InstanceId, "Error: RMA Order Retrieval Root Level", new Variable("Error Sending to WMS", exceptionPORetRootLvl));
            }

            return myWMSRMAOrders;
        }
        #endregion
    }
}

