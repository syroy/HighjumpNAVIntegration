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
    public class PurchaseNAVAdaptor
    {
        #region SVA RetrieveFromNAV
        public static List<PurchaseOrder> RetrieveFromNAV()
        {
            NAVPurchaseOrderRoot myNAVPurchaseOrderRoot = new NAVPurchaseOrderRoot();
            List<PurchaseOrder> myWMSPurchaseOrders = new List<PurchaseOrder>();
            bool didFailureOccur;

            String myResult = String.Empty;

            try
            {
                PurchaseOrder myWMSPurchaseOrder;
                PurchaseOrderLine myWMSPurchaseOrderLine;

                // Call NAV Web Service | ExportPurchaseOrder
                myResult = CompanyConfig.Instance.NavWebServiceInstance.ExportPurchaseOrder(ref myNAVPurchaseOrderRoot);
                CompanyConfig.Instance.NavWebServiceInstance.Dispose();
                EventNotificationClient.Instance.PostMessage("NAV.Download.PurchaseOrders", CompanyConfig.InstanceId, "Exported NAV Data",
                    new Variable("data", Utils.SerializeToString(myNAVPurchaseOrderRoot)));

                //NAV Business Object | Code = RH
                NAVBusinessObjectRH myNAVBusinessObjectRH = new NAVBusinessObjectRH();
                myNAVBusinessObjectRH = myNAVPurchaseOrderRoot.NAVBusinessObjectRH;

                //NAV Receipt Headers | Processed = Ready to Send
                NAVReceiptHeaders myNAVReceiptHeaders = new NAVReceiptHeaders();
                myNAVReceiptHeaders = myNAVBusinessObjectRH.NAVReceiptHeaders;

                try
                {
                    if (myNAVReceiptHeaders.NAVReceiptHeader == null)
                    {
                        //There aren't any orders to process
                        myWMSPurchaseOrders = new List<PurchaseOrder>();
                        return myWMSPurchaseOrders;
                    }

                    // For each Receipt Header record in the queue
                    foreach (NAVReceiptHeader myNAVReceiptHeader in myNAVReceiptHeaders.NAVReceiptHeader)
                    {
                        NAVPurchaseOrders myNAVPurchaseOrders = new NAVPurchaseOrders();
                        myNAVPurchaseOrders = myNAVReceiptHeader.NAVPurchaseOrders;

                        NAVPurchaseOrder myNAVOrder = new NAVPurchaseOrder();
                        myNAVOrder = myNAVPurchaseOrders.NAVPurchaseOrder;

                        myWMSPurchaseOrder = new PurchaseOrder();

                        // Parse Purchase Order Header
                        myWMSPurchaseOrder.AllowBackOrder = NAVAdapterUtils.NAVFormatBoolean(myNAVOrder.AllowBackOrder.ToString());
                        myWMSPurchaseOrder.BackOrder = NAVAdapterUtils.NAVFormatString(myNAVOrder.BackOrder.ToString(), 40);
                        myWMSPurchaseOrder.BuyerName = NAVAdapterUtils.NAVFormatString(myNAVOrder.BuyerName.ToString(), 40);
                        myWMSPurchaseOrder.ClientName = NAVAdapterUtils.getClientName();
                        myWMSPurchaseOrder.ContainerBatchRef = NAVAdapterUtils.NAVFormatString(myNAVOrder.ContainerBatchRef.ToString(), 40);
                        myWMSPurchaseOrder.DueDate = NAVAdapterUtils.NAVFormatDateTime(myNAVOrder.DueDate.ToString());
                        myWMSPurchaseOrder.Email = NAVAdapterUtils.NAVFormatString(myNAVOrder.Email.ToString(), 40);
                        myWMSPurchaseOrder.Extra1 = NAVAdapterUtils.NAVFormatString(myNAVOrder.Extra1.ToString(), 40);
                        myWMSPurchaseOrder.Extra2 = NAVAdapterUtils.NAVFormatString(myNAVOrder.Extra2.ToString(), 40);
                        myWMSPurchaseOrder.Extra3 = NAVAdapterUtils.NAVFormatString(myNAVOrder.Extra3.ToString(), 40);
                        myWMSPurchaseOrder.Extra4 = NAVAdapterUtils.NAVFormatString(myNAVOrder.Extra4.ToString(), 40);
                        myWMSPurchaseOrder.Extra5 = NAVAdapterUtils.NAVFormatString(myNAVOrder.Extra5.ToString(), 40);
                        myWMSPurchaseOrder.IsRMA = NAVAdapterUtils.NAVFormatBoolean(myNAVOrder.IsRMA.ToString());
                        myWMSPurchaseOrder.NoSHReship = NAVAdapterUtils.NAVFormatString(myNAVOrder.NoSHReship.ToString(), 40);
                        myWMSPurchaseOrder.OrderSource = NAVAdapterUtils.NAVFormatString(myNAVOrder.OrderSource.ToString(), 40);
                        myWMSPurchaseOrder.Packslip = NAVAdapterUtils.NAVFormatString(myNAVOrder.Packslip.ToString(), 40);
                        myWMSPurchaseOrder.PoType = NAVAdapterUtils.NAVFormatPOType(myNAVOrder.PoType.ToString());
                        myWMSPurchaseOrder.PurchaseOrderNumber = NAVAdapterUtils.NAVFormatString(myNAVOrder.PurchaseOrderNumber.ToString(), 40);
                        myWMSPurchaseOrder.RequestPOD = NAVAdapterUtils.NAVFormatDateTime(myNAVOrder.RequestPOD.ToString()).ToString();
                        myWMSPurchaseOrder.RequestSH = NAVAdapterUtils.NAVFormatString(myNAVOrder.CreditSH.ToString(), 40);
                        myWMSPurchaseOrder.RequisitionNumber = NAVAdapterUtils.NAVFormatString(myNAVOrder.RequisitionNumber.ToString(), 40);
                        myWMSPurchaseOrder.SpecialInstructions1 = NAVAdapterUtils.NAVFormatString(myNAVOrder.SpecialIntructions1.ToString(), 40);
                        myWMSPurchaseOrder.SpecialInstructions2 = NAVAdapterUtils.NAVFormatString(myNAVOrder.SpecialIntructions2.ToString(), 40);
                        myWMSPurchaseOrder.SpecialInstructions3 = NAVAdapterUtils.NAVFormatString(myNAVOrder.SpecialIntructions3.ToString(), 40);
                        myWMSPurchaseOrder.SpecialInstructions4 = NAVAdapterUtils.NAVFormatString(myNAVOrder.SpecialIntructions4.ToString(), 40);
                        myWMSPurchaseOrder.SpecialInstructions5 = NAVAdapterUtils.NAVFormatString(myNAVOrder.SpecialIntructions5.ToString(), 40);
                        myWMSPurchaseOrder.SpecialInstructions6 = NAVAdapterUtils.NAVFormatString(myNAVOrder.SpecialIntructions6.ToString(), 40);
                        myWMSPurchaseOrder.SpecialInstructions7 = NAVAdapterUtils.NAVFormatString(myNAVOrder.SpecialIntructions7.ToString(), 40);
                        myWMSPurchaseOrder.SVAStagingID = myNAVOrder.StagingID.ToString();
                        myWMSPurchaseOrder.VendorName = NAVAdapterUtils.NAVFormatString(myNAVOrder.VendorName.ToString(), 40);
                        myWMSPurchaseOrder.VendorNumber = NAVAdapterUtils.NAVFormatString(myNAVOrder.VendorNumber.ToString(), 40);
                        myWMSPurchaseOrder.WarehouseTransferLocation = NAVAdapterUtils.NAVFormatString(myNAVOrder.WarehouseTransferLocation.ToString(), 40);
                        myWMSPurchaseOrder.IntegrationUDF1 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF1.ToString(), 40);
                        myWMSPurchaseOrder.IntegrationUDF2 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF2.ToString(), 40);
                        myWMSPurchaseOrder.IntegrationUDF3 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF3.ToString(), 40);
                        myWMSPurchaseOrder.IntegrationUDF4 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF4.ToString(), 40);
                        myWMSPurchaseOrder.IntegrationUDF5 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF5.ToString(), 40);
                        myWMSPurchaseOrder.IntegrationUDF6 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF6.ToString(), 40);
                        myWMSPurchaseOrder.IntegrationUDF7 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF7.ToString(), 40);
                        myWMSPurchaseOrder.IntegrationUDF8 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF8.ToString(), 40);
                        myWMSPurchaseOrder.IntegrationUDF9 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF9.ToString(), 40);
                        myWMSPurchaseOrder.IntegrationUDF10 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF10.ToString(), 40);
                        myWMSPurchaseOrder.IntegrationUDF11 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF11.ToString(), 40);
                        myWMSPurchaseOrder.IntegrationUDF12 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF12.ToString(), 40);
                        myWMSPurchaseOrder.IntegrationUDF13 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF13.ToString(), 40);
                        myWMSPurchaseOrder.IntegrationUDF14 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF14.ToString(), 40);
                        myWMSPurchaseOrder.IntegrationUDF15 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF15.ToString(), 40);
                        myWMSPurchaseOrder.IntegrationUDF16 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF16.ToString(), 40);
                        myWMSPurchaseOrder.IntegrationUDF17 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF17.ToString(), 40);
                        myWMSPurchaseOrder.IntegrationUDF18 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF18.ToString(), 40);
                        myWMSPurchaseOrder.IntegrationUDF19 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF19.ToString(), 40);
                        myWMSPurchaseOrder.IntegrationUDF20 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF20.ToString(), 40);
                        myWMSPurchaseOrder.IntegrationUDF21 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF21.ToString(), 40);
                        myWMSPurchaseOrder.IntegrationUDF22 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF22.ToString(), 40);
                        myWMSPurchaseOrder.IntegrationUDF23 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF23.ToString(), 40);
                        myWMSPurchaseOrder.IntegrationUDF24 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF24.ToString(), 40);
                        myWMSPurchaseOrder.IntegrationUDF25 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF25.ToString(), 40);

                        didFailureOccur = false;

                        try
                        {
                            NAVPurchaseOrderLines myNAVPurchaseOrderLines = myNAVOrder.NAVPurchaseOrderLines;

                            foreach (NAVPurchaseOrderLine myNAVLine in myNAVPurchaseOrderLines.NAVPurchaseOrderLine)
                            {
                                myWMSPurchaseOrderLine = new PurchaseOrderLine();

                                NAVWRLReceiptLines myNAVWRLRecLines = myNAVLine.NAVWRLReceiptLines;
                                if (myNAVWRLRecLines.NAVWRLReceiptLine != null)
                                {
                                    try
                                    {
                                        foreach (NAVWRLReceiptLine myNAVWRLine in myNAVWRLRecLines.NAVWRLReceiptLine)
                                        {
                                            myWMSPurchaseOrderLine = new PurchaseOrderLine();

                                            NAV.BusinessObjects.AttributeSet mySet = new NAV.BusinessObjects.AttributeSet();
                                            mySet.Attribute1 = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRAttribute1.ToString(), 40);
                                            mySet.Attribute2 = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRAttribute2.ToString(), 40);
                                            mySet.Attribute3 = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRAttribute3.ToString(), 40);
                                            mySet.Attribute4 = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRAttribute4.ToString(), 40);
                                            mySet.Attribute5 = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRAttribute5.ToString(), 40);
                                            mySet.Attribute6 = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRAttribute6.ToString(), 40);
                                            mySet.Attribute7 = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRAttribute7.ToString(), 40);
                                            mySet.Attribute8 = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRAttribute8.ToString(), 40);
                                            mySet.Attribute9 = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRAttribute9.ToString(), 40);
                                            mySet.Attribute10 = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRAttribute10.ToString(), 40);
                                            myWMSPurchaseOrderLine.Attributes.Add(mySet);

                                            myWMSPurchaseOrderLine.ContainerBatchRef = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRContainerBatchRef.ToString(), 40);
                                            myWMSPurchaseOrderLine.ContainerStatus = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRContainerStatus.ToString(), 40);
                                            myWMSPurchaseOrderLine.CostPrice = NAVAdapterUtils.NAVFormatDecimal(myNAVWRLine.WRCostPrice.ToString());
                                            myWMSPurchaseOrderLine.CountryOfOrigin = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRCountryOfOrigin.ToString(), 40);
                                            myWMSPurchaseOrderLine.CreditNow = NAVAdapterUtils.NAVFormatBoolean(myNAVWRLine.WRCreditNow.ToString());
                                            myWMSPurchaseOrderLine.CustomerName = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRCustomerName.ToString(), 40);
                                            myWMSPurchaseOrderLine.CustomerNumber = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRCustomerNumber.ToString(), 40);
                                            myWMSPurchaseOrderLine.Description = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRDescription.ToString(), 100); // Changed to 100 characters for A1W 64 - iCepts 10/6/15
                                            myWMSPurchaseOrderLine.Discount = NAVAdapterUtils.NAVFormatDecimal(myNAVWRLine.WRDiscount.ToString());
                                            myWMSPurchaseOrderLine.ExpectedRequiredDate = NAVAdapterUtils.NAVFormatDateTime(myNAVWRLine.WRExpectedRequiredDate.ToString());
                                            myWMSPurchaseOrderLine.ExpiryDate = NAVAdapterUtils.NAVFormatDateTime(myNAVWRLine.WRExpiryDate.ToString());
                                            myWMSPurchaseOrderLine.Extra1 = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRExtra1.ToString(), 40);
                                            myWMSPurchaseOrderLine.Extra2 = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRExtra2.ToString(), 40);
                                            myWMSPurchaseOrderLine.Extra3 = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRExtra3.ToString(), 40);
                                            myWMSPurchaseOrderLine.Extra4 = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRExtra4.ToString(), 40);
                                            myWMSPurchaseOrderLine.Extra5 = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRExtra5.ToString(), 40);
                                            myWMSPurchaseOrderLine.HostLineReference = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRHostLineReference.ToString(), 40);
                                            myWMSPurchaseOrderLine.IsSpecial = NAVAdapterUtils.NAVFormatBoolean(myNAVWRLine.WRIsSpecial.ToString());

                                            myWMSPurchaseOrderLine.LineNumber = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRLineNumber.ToString(), 40);
                                            myWMSPurchaseOrderLine.Location = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRLocation.ToString(), 40);
                                            myWMSPurchaseOrderLine.OverReceiptPercentage = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WROverReceivePercent.ToString(), 40);
                                            myWMSPurchaseOrderLine.PrimaryLocation = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRPrimaryBin.ToString(), 40);
                                            myWMSPurchaseOrderLine.ProductClass = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRProductClass.ToString(), 40);
                                            myWMSPurchaseOrderLine.ProductCode = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRProductCode.ToString(), 40);
                                            myWMSPurchaseOrderLine.PurchaseOrderNumber = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRPurchaseOrderNumber.ToString(), 40);
                                            myWMSPurchaseOrderLine.QuantityExpected = NAVAdapterUtils.NAVFormatDecimal(myNAVWRLine.WRQuantityExpected.ToString());

                                            myWMSPurchaseOrderLine.ReceiveAttributeTracking.Attribute1Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVWRLine.WRAttribute1Tracking.ToString());
                                            myWMSPurchaseOrderLine.ReceiveAttributeTracking.Attribute2Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVWRLine.WRAttribute2Tracking.ToString());
                                            myWMSPurchaseOrderLine.ReceiveAttributeTracking.Attribute3Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVWRLine.WRAttribute3Tracking.ToString());
                                            myWMSPurchaseOrderLine.ReceiveAttributeTracking.Attribute4Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVWRLine.WRAttribute4Tracking.ToString());
                                            myWMSPurchaseOrderLine.ReceiveAttributeTracking.Attribute5Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVWRLine.WRAttribute5Tracking.ToString());
                                            myWMSPurchaseOrderLine.ReceiveAttributeTracking.Attribute6Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVWRLine.WRAttribute6Tracking.ToString());
                                            myWMSPurchaseOrderLine.ReceiveAttributeTracking.Attribute7Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVWRLine.WRAttribute7Tracking.ToString());
                                            myWMSPurchaseOrderLine.ReceiveAttributeTracking.Attribute8Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVWRLine.WRAttribute8Tracking.ToString());
                                            myWMSPurchaseOrderLine.ReceiveAttributeTracking.Attribute9Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVWRLine.WRAttribute9Tracking.ToString());
                                            myWMSPurchaseOrderLine.ReceiveAttributeTracking.Attribute10Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVWRLine.WRAttribute10Tracking.ToString());
                                            myWMSPurchaseOrderLine.ReInvoice = NAVAdapterUtils.NAVFormatBoolean(myNAVWRLine.WRReInvoice.ToString());
                                            myWMSPurchaseOrderLine.Reship = NAVAdapterUtils.NAVFormatInt(myNAVWRLine.WRReship.ToString());
                                            myWMSPurchaseOrderLine.RMAReason = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRRMAReason.ToString(), 40);
                                            myWMSPurchaseOrderLine.RMARestockCharge = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRRMARestockCharge.ToString(), 40);                               //myWMSPurchaseOrderLine = NAVAdapterUtils.NAVFormatString(myNAVLine
                                            myWMSPurchaseOrderLine.SalesOrderLineNumber = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRSalesOrderLineNumber.ToString(), 40);
                                            myWMSPurchaseOrderLine.SalesOrderNumber = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRSalesOrderNumber.ToString(), 40);
                                            myWMSPurchaseOrderLine.SpecialInstructions1 = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRSpecialInstructions1.ToString(), 40);
                                            myWMSPurchaseOrderLine.SpecialInstructions2 = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRSpecialInstructions2.ToString(), 40);
                                            myWMSPurchaseOrderLine.SpecialInstructions3 = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRSpecialInstructions3.ToString(), 40);
                                            myWMSPurchaseOrderLine.SpecialInstructions4 = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRSpecialInstructions4.ToString(), 40);
                                            myWMSPurchaseOrderLine.SpecialInstructions5 = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRSpecialInstructions5.ToString(), 40);
                                            myWMSPurchaseOrderLine.SpecialInstructions6 = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRSpecialInstructions6.ToString(), 40);
                                            myWMSPurchaseOrderLine.SpecialInstructions7 = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRSpecialInstructions7.ToString(), 40);
                                            myWMSPurchaseOrderLine.StockItemIndicator = NAVAdapterUtils.NAVFormatStockItemIndicatorType(myNAVWRLine.WRStockItemIndicator.ToString());
                                            myWMSPurchaseOrderLine.UnitOfMeasureMultiplier = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRUnitOfMeasureMultiplier.ToString(), 40);
                                            myWMSPurchaseOrderLine.UnitOfMeasureText = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRUnitOfMeasureText.ToString(), 40);
                                            myWMSPurchaseOrderLine.UnitPrice = NAVAdapterUtils.NAVFormatDecimal(myNAVWRLine.WRUnitPrice.ToString());
                                            myWMSPurchaseOrderLine.UPC = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRUPC.ToString(), 40);
                                            myWMSPurchaseOrderLine.VendorProductNumber = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRVendorProductNumber.ToString(), 40);
                                            myWMSPurchaseOrderLine.Warehouse = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRWarehouse.ToString(), 40);
                                            myWMSPurchaseOrderLine.IntegrationUDF1 = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRIntegrationUDF1.ToString(), 40);
                                            myWMSPurchaseOrderLine.IntegrationUDF2 = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRIntegrationUDF2.ToString(), 40);
                                            myWMSPurchaseOrderLine.IntegrationUDF3 = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRIntegrationUDF3.ToString(), 40);
                                            myWMSPurchaseOrderLine.IntegrationUDF4 = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRIntegrationUDF4.ToString(), 40);
                                            myWMSPurchaseOrderLine.IntegrationUDF5 = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRIntegrationUDF5.ToString(), 40);
                                            myWMSPurchaseOrderLine.IntegrationUDF6 = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRIntegrationUDF6.ToString(), 40);
                                            myWMSPurchaseOrderLine.IntegrationUDF7 = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRIntegrationUDF7.ToString(), 40);
                                            myWMSPurchaseOrderLine.IntegrationUDF8 = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRIntegrationUDF8.ToString(), 40);
                                            myWMSPurchaseOrderLine.IntegrationUDF9 = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRIntegrationUDF9.ToString(), 40);
                                            myWMSPurchaseOrderLine.IntegrationUDF10 = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRIntegrationUDF10.ToString(), 40);
                                            myWMSPurchaseOrderLine.IntegrationUDF11 = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRIntegrationUDF11.ToString(), 40);
                                            myWMSPurchaseOrderLine.IntegrationUDF12 = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRIntegrationUDF12.ToString(), 40);
                                            myWMSPurchaseOrderLine.IntegrationUDF13 = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRIntegrationUDF13.ToString(), 40);
                                            myWMSPurchaseOrderLine.IntegrationUDF14 = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRIntegrationUDF14.ToString(), 40);
                                            myWMSPurchaseOrderLine.IntegrationUDF15 = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRIntegrationUDF15.ToString(), 40);
                                            myWMSPurchaseOrderLine.IntegrationUDF16 = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRIntegrationUDF16.ToString(), 40);
                                            myWMSPurchaseOrderLine.IntegrationUDF17 = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRIntegrationUDF17.ToString(), 40);
                                            myWMSPurchaseOrderLine.IntegrationUDF18 = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRIntegrationUDF18.ToString(), 40);
                                            myWMSPurchaseOrderLine.IntegrationUDF19 = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRIntegrationUDF19.ToString(), 40);
                                            myWMSPurchaseOrderLine.IntegrationUDF20 = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRIntegrationUDF20.ToString(), 40);
                                            myWMSPurchaseOrderLine.IntegrationUDF21 = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRIntegrationUDF21.ToString(), 40);
                                            myWMSPurchaseOrderLine.IntegrationUDF22 = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRIntegrationUDF22.ToString(), 40);
                                            myWMSPurchaseOrderLine.IntegrationUDF23 = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRIntegrationUDF23.ToString(), 40);
                                            myWMSPurchaseOrderLine.IntegrationUDF24 = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRIntegrationUDF24.ToString(), 40);
                                            myWMSPurchaseOrderLine.IntegrationUDF25 = NAVAdapterUtils.NAVFormatString(myNAVWRLine.WRIntegrationUDF25.ToString(), 40);

                                            myWMSPurchaseOrder.Lines.Add(myWMSPurchaseOrderLine);
                                        }
                                    }
                                    catch (Exception exceptionWRL)
                                    {
                                        //Error in Warehouse Receipt Line
                                        EventNotificationClient.Instance.PostMessage("NAV.Download.PurchaseOrders.Error", CompanyConfig.InstanceId, "Error", new Variable("Error parsing warehouse receipt line", exceptionWRL));
                                        didFailureOccur = true;
                                        break;
                                    }
                                }
                                else
                                {
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

                                    myWMSPurchaseOrder.Lines.Add(myWMSPurchaseOrderLine);
                                }
                            }
                        }
                        catch (Exception exceptionOrdLinesLvl)
                        {
                            //Error at Purchase Order Lines Level
                            EventNotificationClient.Instance.PostMessage("NAV.Download.PurchaseOrders.Error", CompanyConfig.InstanceId, "Error: Purchase Order Lines Level", new Variable("Error Sending to WMS", exceptionOrdLinesLvl));
                            didFailureOccur = true;
                        }

                        //Add Purchase Order (if succeeded) to List of Purchase Orders
                        if (!didFailureOccur)
                        {
                            myWMSPurchaseOrders.Add(myWMSPurchaseOrder);
                            EventNotificationClient.Instance.PostMessage("NAV.Download.PurchaseOrders", CompanyConfig.InstanceId, "A1I -> Status Update -> NAV", new Variable("Status", "Sent to WMS"), new Variable("Purchase Order Number", NAVAdapterUtils.NAVFormatString(myNAVOrder.PurchaseOrderNumber.ToString(), 40)), new Variable("Entry No", NAVAdapterUtils.NAVFormatString(myNAVOrder.StagingID.ToString(), 40)));
                            //try
                            //{
                            //    // Call NAV Web Service | SentStatus
                            //    CompanyConfig.Instance.NavWebServiceInstance.SentStatus(NAVAdapterUtils.NAVFormatString(myNAVOrder.StagingID.ToString(), 40), "RH");
                            //    CompanyConfig.Instance.NavWebServiceInstance.Dispose();
                            //}
                            //catch (Exception ex1)
                            //{
                            //    EventNotificationClient.Instance.PostMessage("NAV.Download.PurchaseOrders.Error", CompanyConfig.InstanceId, "Error: A1I Entry No -> NAV", new Variable("Error Sending to WMS", ex1));
                            //}
                        }
                    }
                }
                catch (Exception exceptionRHLvl)
                {
                    //Error at Receipt Header Level
                    EventNotificationClient.Instance.PostMessage("NAV.Download.PurchaseOrders.Error", CompanyConfig.InstanceId, "Error: Receipt Header Level", new Variable("Error Sending to WMS", exceptionRHLvl));
                }
            }
            catch (Exception exceptionPORootLvl)
            {
                //Error at Purchase Order Root Level
                EventNotificationClient.Instance.PostMessage("NAV.Download.PurchaseOrders.Error", CompanyConfig.InstanceId, "Error: Purchase Order Root Level", new Variable("Error Sending to WMS", exceptionPORootLvl), new Variable("NAV Return Data", myNAVPurchaseOrderRoot), new Variable("My Result", myResult));
            }

            return myWMSPurchaseOrders;
        }
        #endregion

        #region SVA WriteToNAV
        public static List<UploadConfirmation> WriteToNAV(List<ReceiptConfirmation> receiptConfirmations)
        {
            //EventNotificationClient.Instance.PostMessage("NAV.Upload.PurchaseOrder.WriteToNAV", CompanyConfig.InstanceId, "Entering Function");   

            List<UploadConfirmation> uploadConfirmations = new List<UploadConfirmation>();
            UploadConfirmation myUploadConfirmation = new UploadConfirmation();

            try
            {
                NAVReceiptConfirmationRoot myNAVReceiptConfirmationRoot = new NAVReceiptConfirmationRoot();
                List<NAVReceiptConfirmation> myNAVReceiptConfirmations = new List<NAVReceiptConfirmation>();
                NAVReceiptConfirmation myNAVReceiptConfirmation;
                String myRowID = String.Empty;
                String myResult = String.Empty;
              try
              {
                foreach (ReceiptConfirmation receiptConfirmation in receiptConfirmations)
                {
                        myUploadConfirmation = new UploadConfirmation();
                        myNAVReceiptConfirmation = new NAVReceiptConfirmation();

                        //EventNotificationClient.Instance.PostMessage("NAV.Upload.PurchaseOrder.WriteToNAV", CompanyConfig.InstanceId, "Parsing Receipts");
                    
                        //NAV XMLPort Note:  Items must be assigned in the Column Order that is defined in the XMLPort object
                        //Parse Receipt Confirmation Header
                        myNAVReceiptConfirmation.RecordType = "RC";
                        myNAVReceiptConfirmation.UploadType = "RC";
                        myNAVReceiptConfirmation.ContainerReceivingBatchRef = receiptConfirmation.ContainerReceivingBatchRef.ToString();
                        myNAVReceiptConfirmation.PONumber = receiptConfirmation.PurchaseOrderId.ToString();
                        myNAVReceiptConfirmation.PackingSlipNumber = receiptConfirmation.PackingSlipId.ToString();
                        myNAVReceiptConfirmation.Carrier = receiptConfirmation.Carrier.ToString();
                        myNAVReceiptConfirmation.ClientName = receiptConfirmation.ClientName.ToString();
                        myNAVReceiptConfirmation.AssignedContainerCost = receiptConfirmation.AssignedContainerCost.ToString();
                        myNAVReceiptConfirmation.ShipmentPaymentType = receiptConfirmation.ShipmentPaymentType.ToString();
                        myNAVReceiptConfirmation.FreightStatus = receiptConfirmation.FreightStatus.ToString();
                        myNAVReceiptConfirmation.Extra1 = receiptConfirmation.Extra1.ToString();
                        myNAVReceiptConfirmation.Extra2 = receiptConfirmation.Extra2.ToString();
                        myNAVReceiptConfirmation.Extra3 = receiptConfirmation.Extra3.ToString();
                        myNAVReceiptConfirmation.Extra4 = receiptConfirmation.Extra4.ToString();
                        myNAVReceiptConfirmation.Extra5 = receiptConfirmation.Extra5.ToString();
                        myNAVReceiptConfirmation.BackOrderNum = receiptConfirmation.BackOrderNum.ToString();
                        myNAVReceiptConfirmation.VesselNum = receiptConfirmation.VesselNum.ToString();
                        myNAVReceiptConfirmation.DateLeftPort = receiptConfirmation.DateLeftPort.ToString();
                        myNAVReceiptConfirmation.EstArriveDate = receiptConfirmation.ArriveDate.ToString();
                        myNAVReceiptConfirmation.VendorNumber = receiptConfirmation.VendorId.ToString();
                        myNAVReceiptConfirmation.LPTrackNumASNReceipts = receiptConfirmation.LicensePlate.ToString();
                        myNAVReceiptConfirmation.POHeaderUDF1 = receiptConfirmation.WMSUDF1;
                        myNAVReceiptConfirmation.POHeaderUDF2 = receiptConfirmation.WMSUDF2;
                        myNAVReceiptConfirmation.POHeaderUDF3 = receiptConfirmation.WMSUDF3;
                        myNAVReceiptConfirmation.POHeaderUDF4 = receiptConfirmation.WMSUDF4;

                        myNAVReceiptConfirmation.CompletionCode = "";
                        myNAVReceiptConfirmation.CartonsReceivedForRecord = "";
                        myNAVReceiptConfirmation.WaveNumber = "";
                        myNAVReceiptConfirmation.SizeOfCartonsReceivedForRecord = "";
                        myNAVReceiptConfirmation.UnallocatedQtyOnHand = "";
                        myNAVReceiptConfirmation.TotQtyForLineAndAttribute = "";

                        foreach (ReceiptConfirmationLine line in receiptConfirmation.Lines)
                        {
                            //Parse Receipt Confirmation Line
                            myNAVReceiptConfirmation.LineNum = line.LineNum.ToString();
                            myNAVReceiptConfirmation.CostPrice = line.CostPrice.ToString();
                            myNAVReceiptConfirmation.Discount = line.Discount.ToString();
                            myNAVReceiptConfirmation.ExpiryDate = line.ExpiryDate.ToString();
                            myNAVReceiptConfirmation.LineExtra1 = line.Extra1.ToString();
                            myNAVReceiptConfirmation.LineExtra2 = line.Extra2.ToString();
                            myNAVReceiptConfirmation.LineExtra3 = line.Extra3.ToString();
                            myNAVReceiptConfirmation.LineExtra4 = line.Extra4.ToString();
                            myNAVReceiptConfirmation.LineExtra5 = line.Extra5.ToString();
                            myNAVReceiptConfirmation.ProductCode = line.ProductCode.ToString();
                            myNAVReceiptConfirmation.QuantityExpected = line.QuantityExpected.ToString();
                            myNAVReceiptConfirmation.TotQtyReceivedForLine = line.QuantityReceived.ToString();
                            myNAVReceiptConfirmation.ReceivedBy = line.ReceivedBy.ToString();
                            myNAVReceiptConfirmation.ReceivedOnDate = line.ReceivedOnDate.ToString();
                            myNAVReceiptConfirmation.ReceivePutawayBin = line.ReceivePutawayLocation.ToString();
                            myNAVReceiptConfirmation.RowId = line.RowId.ToString();
                            myRowID = line.RowId.ToString();
                            myNAVReceiptConfirmation.RMACode = line.RMACode.ToString();
                            myNAVReceiptConfirmation.SellingPrice = line.SellingPrice.ToString();
                            myNAVReceiptConfirmation.Tiebreaker = line.Tiebreaker.ToString();
                            myNAVReceiptConfirmation.Warehouse = line.Warehouse.ToString();
                            myNAVReceiptConfirmation.POLineUDF1 = line.WMSUDF1;
                            myNAVReceiptConfirmation.POLineUDF2 = line.WMSUDF2;

                            if ((line.Attributes != null) && (line.Attributes.Count > 0))
                            {
                                int tmpIndex = line.Attributes.Count - 1;
                                myNAVReceiptConfirmation.AttributeQuantity = line.Attributes[tmpIndex].Quantity.ToString();
                                myNAVReceiptConfirmation.Attribute1 = line.Attributes[tmpIndex].Attribute1.ToString();
                                myNAVReceiptConfirmation.Attribute2 = line.Attributes[tmpIndex].Attribute2.ToString();
                                myNAVReceiptConfirmation.Attribute3 = line.Attributes[tmpIndex].Attribute3.ToString();
                                myNAVReceiptConfirmation.Attribute4 = line.Attributes[tmpIndex].Attribute4.ToString();
                                myNAVReceiptConfirmation.Attribute5 = line.Attributes[tmpIndex].Attribute5.ToString();
                                myNAVReceiptConfirmation.Attribute6 = line.Attributes[tmpIndex].Attribute6.ToString();
                                myNAVReceiptConfirmation.Attribute7 = line.Attributes[tmpIndex].Attribute7.ToString();
                                myNAVReceiptConfirmation.Attribute8 = line.Attributes[tmpIndex].Attribute8.ToString();
                                myNAVReceiptConfirmation.Attribute9 = line.Attributes[tmpIndex].Attribute9.ToString();
                                myNAVReceiptConfirmation.Attribute10 = line.Attributes[tmpIndex].Attribute10.ToString();
                            }
                            else
                            {
                                myNAVReceiptConfirmation.AttributeQuantity = "0";
                                myNAVReceiptConfirmation.Attribute1 = " ";
                                myNAVReceiptConfirmation.Attribute2 = " ";
                                myNAVReceiptConfirmation.Attribute3 = " ";
                                myNAVReceiptConfirmation.Attribute4 = " ";
                                myNAVReceiptConfirmation.Attribute5 = " ";
                                myNAVReceiptConfirmation.Attribute6 = " ";
                                myNAVReceiptConfirmation.Attribute7 = " ";
                                myNAVReceiptConfirmation.Attribute8 = " ";
                                myNAVReceiptConfirmation.Attribute9 = " ";
                                myNAVReceiptConfirmation.Attribute10 = " ";
                            }
                        }

                        //Add Receipt Confirmation to the List of Receipt Confirmations
                        myNAVReceiptConfirmations.Add(myNAVReceiptConfirmation);
                    }
                   
                       //EventNotificationClient.Instance.PostMessage("NAV.Upload.PurchaseOrder.WriteToNAV", CompanyConfig.InstanceId, "Submitting Receipt to NAV");   

                        //Send Receipt Confirmation to NAV 
                        myNAVReceiptConfirmationRoot.NAVReceiptConfirmation = myNAVReceiptConfirmations.ToArray();

                        EventNotificationClient.Instance.PostMessage("NAV.Upload.PurchaseOrder", CompanyConfig.InstanceId,
                            "Submitting Pick to NAV", new Variable("data", myNAVReceiptConfirmationRoot.ToString()),
                            new Variable("xml data", Utils.SerializeToString(myNAVReceiptConfirmationRoot.NAVReceiptConfirmation[0])));

                        myResult = CompanyConfig.Instance.NavWebServiceInstance.ImportReceiptConfirmation(myNAVReceiptConfirmationRoot);
                        CompanyConfig.Instance.NavWebServiceInstance.Dispose();

                        if (myResult == "OK")
                        {
                            //EventNotificationClient.Instance.PostMessage("NAV.Upload.PurchaseOrder.WriteToNAV", CompanyConfig.InstanceId,
                            //"Error",
                            //    new Variable ("Receipt Uploaded", myRowID));
                            myUploadConfirmation.IsSuccess = true;
                            myUploadConfirmation.RowId = myRowID;
                            myUploadConfirmation.ErrorMessage = String.Empty;
                            uploadConfirmations.Add(myUploadConfirmation);
                        }
                        else
                        {
                            //EventNotificationClient.Instance.PostMessage("NAV.Upload.PurchaseOrder.WriteToNAV", CompanyConfig.InstanceId, 
                            //"Error",
                            //    new Variable ("Receipt Failed [A]", myRowID));
                            myUploadConfirmation.IsSuccess = false;
                            myUploadConfirmation.RowId = myRowID;
                            myUploadConfirmation.ErrorMessage = "Upload to NAV failed.";
                            uploadConfirmations.Add(myUploadConfirmation);
                        }
                    }
                    catch (Exception ex)
                    {
                        //EventNotificationClient.Instance.PostMessage("NAV.Upload.PurchaseOrder.WriteToNAV", CompanyConfig.InstanceId,
                        //"Error",
                        //    new Variable("Receipt Failed [B]", ex.ToString()));
                        myUploadConfirmation.IsSuccess = false;
                        myUploadConfirmation.RowId = myRowID;
                        myUploadConfirmation.ErrorMessage = ex.ToString();
                        uploadConfirmations.Add(myUploadConfirmation);
                    }
            }
            catch
            {
                //EventNotificationClient.Instance.PostMessage("NAV.Upload.PurchaseOrder.WriteToNAV", CompanyConfig.InstanceId,
                //"Error",
                //    new Variable("Receipt Failed [C]", ex.ToString()));
                //Error 
                uploadConfirmations = new List<UploadConfirmation>();
                return uploadConfirmations;
            }

            //EventNotificationClient.Instance.PostMessage("NAV.Upload.PurchaseOrder.WriteToNAV", CompanyConfig.InstanceId,
            //    string.Format("Existing WriteToNAV", ""));
            return uploadConfirmations;
        }
        #endregion

        #region SVA RetrievalRequestFromNAV
        public static List<PurchaseOrder> RetrievalRequestFromNAV()
        {
            NAVPurchaseOrderRetrievalRoot myNAVPurchaseOrderRetrievalRoot = new NAVPurchaseOrderRetrievalRoot();
            List<PurchaseOrder> myWMSPurchaseOrders = new List<PurchaseOrder>();

            String myResult = String.Empty;
            try
            {
                PurchaseOrder myWMSPurchaseOrder;

                // Call NAV Web Service | ExportPurchaseOrder
                myResult = CompanyConfig.Instance.NavWebServiceInstance.RetrievePurchaseOrder(ref myNAVPurchaseOrderRetrievalRoot);
                CompanyConfig.Instance.NavWebServiceInstance.Dispose();

                //NAV Business Object | Code = RH
                NAVBusinessObjectRHR myNAVBusinessObjectRHR = new NAVBusinessObjectRHR();
                myNAVBusinessObjectRHR = myNAVPurchaseOrderRetrievalRoot.NAVBusinessObjectRHR;

                //NAV Receipt Headers | Processed = Ready to Send
                NAVReceiptHeaderRetrievals myNAVReceiptHeaderRetrievals = new NAVReceiptHeaderRetrievals();
                myNAVReceiptHeaderRetrievals = myNAVBusinessObjectRHR.NAVReceiptHeaderRetrievals;

                try
                {
                    // For each Receipt Header Retrieval record in the queue
                    foreach (NAVReceiptHeaderRetrieval myNAVReceiptHeaderRetrieval in myNAVReceiptHeaderRetrievals.NAVReceiptHeaderRetrieval)
                    {
                        myWMSPurchaseOrder = new PurchaseOrder();

                        // Parse Packslip Header
                        myWMSPurchaseOrder.SVAStagingID = myNAVReceiptHeaderRetrieval.StagingID.ToString();
                        myWMSPurchaseOrder.ClientName = NAVAdapterUtils.getClientName();
                        myWMSPurchaseOrder.PurchaseOrderNumber = NAVAdapterUtils.NAVFormatString(myNAVReceiptHeaderRetrieval.PurchaseOrderNumber.ToString(), 40);

                        //Add Packslips to List of Packslips
                        if (myNAVReceiptHeaderRetrieval.PurchaseOrderNumber.ToString() != "")
                        {
                            myWMSPurchaseOrders.Add(myWMSPurchaseOrder);
                            EventNotificationClient.Instance.PostMessage("NAV.Download.PurchaseOrderRetrieval", CompanyConfig.InstanceId, 
                                "A1I -> Status Update -> NAV", new Variable("Status", "Sent to WMS"), 
                                new Variable("Purchase Order Number", 
                                    NAVAdapterUtils.NAVFormatString(myNAVReceiptHeaderRetrieval.PurchaseOrderNumber.ToString(), 40)),
                                    new Variable("Entry No", NAVAdapterUtils.NAVFormatString(myNAVReceiptHeaderRetrieval.StagingID.ToString(), 40)));
                            try
                            {
                                // Call NAV Web Service | SentStatus
                                CompanyConfig.Instance.NavWebServiceInstance.SentStatus(NAVAdapterUtils.NAVFormatString(myNAVReceiptHeaderRetrieval.StagingID.ToString(), 40), "RH");
                                CompanyConfig.Instance.NavWebServiceInstance.Dispose();
                            }
                            catch (Exception ex1)
                            {
                                EventNotificationClient.Instance.PostMessage("NAV.Download.PurchaseOrderRetrieval.Error", CompanyConfig.InstanceId, "Error: A1I Entry No -> NAV", new Variable("Error Sending to WMS", ex1));
                            }
                        }
                    }
                }
                catch (Exception exceptionPOLvl)
                {
                    //Error at Purchase Order Level
                    EventNotificationClient.Instance.PostMessage("NAV.Download.PurchaseOrderRetrieval.Error", CompanyConfig.InstanceId, "Error: Purchase Order Level", new Variable("Error Sending to WMS", exceptionPOLvl));
                }
            }
            catch (Exception exceptionPORetRootLvl)
            {
                //Error at Purchase Order Retrieval Root Level
                EventNotificationClient.Instance.PostMessage("NAV.Download.PurchaseOrderRetrieval.Error", CompanyConfig.InstanceId, "Error: Purchase Order Retrieval Root Level", new Variable("Error Sending to WMS", exceptionPORetRootLvl));
            }

            return myWMSPurchaseOrders;
        }
        #endregion
    }
}

