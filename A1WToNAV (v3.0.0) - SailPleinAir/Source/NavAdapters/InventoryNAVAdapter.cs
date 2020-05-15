using System;
using System.Collections.Generic;
using Accellos.InterfaceFramework;
using Accellos.InterfaceFramework.EventNotificationServer;
using Accellos.InterfaceFramework.ScriptManagementServer;
using Accellos.Interfaces.NAV.BusinessObjects;
using Accellos.Interfaces.NAV.WMSAdapters;
using Accellos.Interfaces.NAV.NAVWebServices;

namespace Accellos.Interfaces.NAV.NAVAdapters
{
	public class InventoryNAVAdapter
    {
        #region SVA RetrieveFromNAV
        public static List<InventoryItem> RetrieveFromNAV()
        {
            List<InventoryItem> myWMSItems = new List<InventoryItem>();
            String myResult = String.Empty;

            try
            {
                NAVInventoryItemRoot myNAVInventoryItemRoot = new NAVInventoryItemRoot();
                InventoryItem myWMSItem;
                myWMSItem = new InventoryItem();

                // Call NAV Web Service | ExportInventoryItem
                myResult = CompanyConfig.Instance.NavWebServiceInstance.ExportInventoryItem(ref myNAVInventoryItemRoot);
                CompanyConfig.Instance.NavWebServiceInstance.Dispose();
                EventNotificationClient.Instance.PostMessage("NAV.Download.InventoryItems", CompanyConfig.InstanceId, "Exported NAV Data",
                    new Variable("data", Utils.SerializeToString(myNAVInventoryItemRoot)));


                //NAV Business Object | Code = VA
                NAVBusinessObjectVA myNAVBusinessObjectVA = new NAVBusinessObjectVA();
                myNAVBusinessObjectVA = myNAVInventoryItemRoot.NAVBusinessObjectVA;

                //NAV Vendor Adjustments | Processed = Ready to Send
                NAVVendorAdjustments myNAVVendorAdjustments = new NAVVendorAdjustments();
                myNAVVendorAdjustments = myNAVBusinessObjectVA.NAVVendorAdjustments;

                if (myNAVVendorAdjustments.NAVVendorAdjustment == null)
                {
                    //There aren't any items to process
                    myWMSItems = new List<InventoryItem>();
                    return myWMSItems;
                }

                // For each Vendor Adjustment record in the queue
                foreach (NAVVendorAdjustment myNAVVendorAdjustment in myNAVVendorAdjustments.NAVVendorAdjustment)
                {
                    NAVInventoryItems myNAVInventoryItems = new NAVInventoryItems();
                    myNAVInventoryItems = myNAVVendorAdjustment.NAVInventoryItems;

                    NAVInventoryItem myNAVItem = new NAVInventoryItem();
                    myNAVItem = myNAVInventoryItems.NAVInventoryItem;

	                EventNotificationClient.Instance.PostMessage("NAV.Download.InventoryItems", CompanyConfig.InstanceId, "NAVVendorAdjustment",
        	            new Variable("data", Utils.SerializeToString(myNAVItem)), new Variable("desc", myNAVItem.Description.ToString()));

                    myWMSItem = new InventoryItem();

                    if (myNAVItem.ProductCode.Length > 0)
                    {
                        NAV.BusinessObjects.AttributeSet mySet = new NAV.BusinessObjects.AttributeSet();
                        mySet.Attribute1 = NAVAdapterUtils.NAVFormatString(myNAVItem.Attributes.Attribute1.ToString(), 40);
                        mySet.Attribute2 = NAVAdapterUtils.NAVFormatString(myNAVItem.Attributes.Attribute2.ToString(), 40);
                        mySet.Attribute3 = NAVAdapterUtils.NAVFormatString(myNAVItem.Attributes.Attribute3.ToString(), 40);
                        mySet.Attribute4 = NAVAdapterUtils.NAVFormatString(myNAVItem.Attributes.Attribute4.ToString(), 40);
                        mySet.Attribute5 = NAVAdapterUtils.NAVFormatString(myNAVItem.Attributes.Attribute5.ToString(), 40);
                        mySet.Attribute6 = NAVAdapterUtils.NAVFormatString(myNAVItem.Attributes.Attribute6.ToString(), 40);
                        mySet.Attribute7 = NAVAdapterUtils.NAVFormatString(myNAVItem.Attributes.Attribute7.ToString(), 40);
                        mySet.Attribute8 = NAVAdapterUtils.NAVFormatString(myNAVItem.Attributes.Attribute8.ToString(), 40);
                        mySet.Attribute9 = NAVAdapterUtils.NAVFormatString(myNAVItem.Attributes.Attribute9.ToString(), 40);
                        mySet.Attribute10 = NAVAdapterUtils.NAVFormatString(myNAVItem.Attributes.Attribute10.ToString(), 40);
                        myWMSItem.Attributes.Add(mySet);

                        myWMSItem.AverageUnits = NAVAdapterUtils.NAVFormatDecimal(myNAVItem.AverageUnits.ToString());
                        myWMSItem.BeeLine = NAVAdapterUtils.NAVFormatString(myNAVItem.BeeLine.ToString(), 40);
                        myWMSItem.BinLocation = NAVAdapterUtils.NAVFormatString(myNAVItem.Bin.ToString(), 40);
                        myWMSItem.Buyer = NAVAdapterUtils.NAVFormatString(myNAVItem.Buyer.ToString(), 40);
                        myWMSItem.BypassQuantity = NAVAdapterUtils.NAVFormatInt(myNAVItem.BypassQuantity.ToString());
                        myWMSItem.CartonDef = NAVAdapterUtils.NAVFormatInt(myNAVItem.CartonDef.ToString());
                        myWMSItem.CartonSize = NAVAdapterUtils.NAVFormatString(myNAVItem.CartonSize.ToString(), 40);
                        myWMSItem.ClientName = NAVAdapterUtils.getClientName();
                        myWMSItem.CommodityCode = NAVAdapterUtils.NAVFormatString(myNAVItem.CommodityCode.ToString(), 6);
                        myWMSItem.CountryofOrigin = NAVAdapterUtils.NAVFormatString(myNAVItem.CountryOfOrigin.ToString(), 6);
                        myWMSItem.Cost = NAVAdapterUtils.NAVFormatDecimal(myNAVItem.Cost.ToString());
                        myWMSItem.Cubage = NAVAdapterUtils.NAVFormatDecimal(myNAVItem.Cubage.ToString());
                        myWMSItem.Description = NAVAdapterUtils.NAVFormatString(myNAVItem.Description.ToString(), 100); // Changed to 100 characters for A1W 64 - iCepts 10/6/15
                        myWMSItem.EDPNum = NAVAdapterUtils.NAVFormatString(myNAVItem.EDPNum.ToString(), 40);
                        myWMSItem.ExpiryDateRequired = NAVAdapterUtils.NAVFormatBoolean(myNAVItem.ExpiryDateRequired.ToString());
                        myWMSItem.FormalSize1.Height = NAVAdapterUtils.NAVFormatDecimal(myNAVItem.FormalSize1.Height.ToString());
                        myWMSItem.FormalSize1.Length = NAVAdapterUtils.NAVFormatDecimal(myNAVItem.FormalSize1.Length.ToString());
                        myWMSItem.FormalSize1.Weight = NAVAdapterUtils.NAVFormatDecimal(myNAVItem.FormalSize1.Weight.ToString());
                        myWMSItem.FormalSize1.Width = NAVAdapterUtils.NAVFormatDecimal(myNAVItem.FormalSize1.Width.ToString());
                        myWMSItem.FormalSize2.Height = NAVAdapterUtils.NAVFormatDecimal(myNAVItem.FormalSize2.Height.ToString());
                        myWMSItem.FormalSize2.Length = NAVAdapterUtils.NAVFormatDecimal(myNAVItem.FormalSize2.Length.ToString());
                        myWMSItem.FormalSize2.Weight = NAVAdapterUtils.NAVFormatDecimal(myNAVItem.FormalSize2.Weight.ToString());
                        myWMSItem.FormalSize2.Width = NAVAdapterUtils.NAVFormatDecimal(myNAVItem.FormalSize2.Width.ToString());
                        myWMSItem.FormalSize3.Height = NAVAdapterUtils.NAVFormatDecimal(myNAVItem.FormalSize3.Height.ToString());
                        myWMSItem.FormalSize3.Length = NAVAdapterUtils.NAVFormatDecimal(myNAVItem.FormalSize3.Length.ToString());
                        myWMSItem.FormalSize3.Weight = NAVAdapterUtils.NAVFormatDecimal(myNAVItem.FormalSize3.Weight.ToString());
                        myWMSItem.FormalSize3.Width = NAVAdapterUtils.NAVFormatDecimal(myNAVItem.FormalSize3.Width.ToString());
                        myWMSItem.FormalSize4.Height = NAVAdapterUtils.NAVFormatDecimal(myNAVItem.FormalSize4.Height.ToString());
                        myWMSItem.FormalSize4.Length = NAVAdapterUtils.NAVFormatDecimal(myNAVItem.FormalSize4.Length.ToString());
                        myWMSItem.FormalSize4.Weight = NAVAdapterUtils.NAVFormatDecimal(myNAVItem.FormalSize4.Weight.ToString());
                        myWMSItem.FormalSize4.Width = NAVAdapterUtils.NAVFormatDecimal(myNAVItem.FormalSize4.Width.ToString());
                        myWMSItem.FormalSize5.Height = NAVAdapterUtils.NAVFormatDecimal(myNAVItem.FormalSize5.Height.ToString());
                        myWMSItem.FormalSize5.Length = NAVAdapterUtils.NAVFormatDecimal(myNAVItem.FormalSize5.Length.ToString());
                        myWMSItem.FormalSize5.Weight = NAVAdapterUtils.NAVFormatDecimal(myNAVItem.FormalSize5.Weight.ToString());
                        myWMSItem.FormalSize5.Width = NAVAdapterUtils.NAVFormatDecimal(myNAVItem.FormalSize5.Width.ToString());
                        myWMSItem.GlobalCoreReservation = NAVAdapterUtils.NAVFormatInt(myNAVItem.GlobalCoreReservation.ToString());
                        myWMSItem.HandleCode = NAVAdapterUtils.NAVFormatString(myNAVItem.HandleCode.ToString(), 40);
                        myWMSItem.Height = NAVAdapterUtils.NAVFormatDecimal(myNAVItem.Height.ToString());
                        myWMSItem.Length = NAVAdapterUtils.NAVFormatDecimal(myNAVItem.Length.ToString());
                        myWMSItem.LicensePlate = NAVAdapterUtils.NAVFormatString(myNAVItem.LicensePlate.ToString(), 40);
                        myWMSItem.MaximumReplenishmentLevel1 = NAVAdapterUtils.NAVFormatInt(myNAVItem.MaximumReplenishmentLevel1.ToString());
                        myWMSItem.MaximumReplenishmentLevel2 = NAVAdapterUtils.NAVFormatInt(myNAVItem.MaximumReplenishmentLevel2.ToString());
                        myWMSItem.MaximumReplenishmentLevel3 = NAVAdapterUtils.NAVFormatInt(myNAVItem.MaximumReplenishmentLevel3.ToString());
                        myWMSItem.MaximumReplenishmentLevel4 = NAVAdapterUtils.NAVFormatInt(myNAVItem.MaximumReplenishmentLevel4.ToString());
                        myWMSItem.MaximumReplenishmentLevel5 = NAVAdapterUtils.NAVFormatInt(myNAVItem.MaximumReplenishmentLevel5.ToString());
                        myWMSItem.MinimumDaysToExpiry = NAVAdapterUtils.NAVFormatString(myNAVItem.MinimumDaysToExpiry.ToString(), 40);
                        myWMSItem.MinimumReplenishmentLevel1 = NAVAdapterUtils.NAVFormatInt(myNAVItem.MinimumReplenishmentLevel1.ToString());
                        myWMSItem.MinimumReplenishmentLevel2 = NAVAdapterUtils.NAVFormatInt(myNAVItem.MinimumReplenishmentLevel2.ToString());
                        myWMSItem.MinimumReplenishmentLevel3 = NAVAdapterUtils.NAVFormatInt(myNAVItem.MinimumReplenishmentLevel3.ToString());
                        myWMSItem.MinimumReplenishmentLevel4 = NAVAdapterUtils.NAVFormatInt(myNAVItem.MinimumReplenishmentLevel4.ToString());
                        myWMSItem.MinimumReplenishmentLevel5 = NAVAdapterUtils.NAVFormatInt(myNAVItem.MinimumReplenishmentLevel5.ToString());
                        myWMSItem.OrderGroup = NAVAdapterUtils.NAVFormatString(myNAVItem.OrderGroup.ToString(), 40);
                        myWMSItem.OverReceiptPercentage = NAVAdapterUtils.NAVFormatString(myNAVItem.OverReceivePercent.ToString(), 40);
                        myWMSItem.PackClass = NAVAdapterUtils.NAVFormatString(myNAVItem.PackClass.ToString(), 40);
                        myWMSItem.PacksizePrecision = NAVAdapterUtils.NAVFormatInt(myNAVItem.DecimalPacksizePrecision.ToString());
                        myWMSItem.PickAttributeTracking.Attribute1Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVItem.PickAttributeTracking.Attribute1Tracking.ToString());
                        myWMSItem.PickAttributeTracking.Attribute2Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVItem.PickAttributeTracking.Attribute2Tracking.ToString());
                        myWMSItem.PickAttributeTracking.Attribute3Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVItem.PickAttributeTracking.Attribute3Tracking.ToString());
                        myWMSItem.PickAttributeTracking.Attribute4Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVItem.PickAttributeTracking.Attribute4Tracking.ToString());
                        myWMSItem.PickAttributeTracking.Attribute5Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVItem.PickAttributeTracking.Attribute5Tracking.ToString());
                        myWMSItem.PickAttributeTracking.Attribute6Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVItem.PickAttributeTracking.Attribute6Tracking.ToString());
                        myWMSItem.PickAttributeTracking.Attribute7Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVItem.PickAttributeTracking.Attribute7Tracking.ToString());
                        myWMSItem.PickAttributeTracking.Attribute8Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVItem.PickAttributeTracking.Attribute8Tracking.ToString());
                        myWMSItem.PickAttributeTracking.Attribute9Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVItem.PickAttributeTracking.Attribute9Tracking.ToString());
                        myWMSItem.PickAttributeTracking.Attribute10Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVItem.PickAttributeTracking.Attribute10Tracking.ToString());
                        myWMSItem.ProductClass = NAVAdapterUtils.NAVFormatString(myNAVItem.ProductClass.ToString(), 40);
                        myWMSItem.ProductCode = NAVAdapterUtils.NAVFormatString(myNAVItem.ProductCode.ToString(), 40);
                        myWMSItem.ProductUDF1 = NAVAdapterUtils.NAVFormatString(myNAVItem.ProductUDF1.ToString(), 40);
                        myWMSItem.ProductUDF2 = NAVAdapterUtils.NAVFormatString(myNAVItem.ProductUDF2.ToString(), 40);
                        myWMSItem.ProductUDF3 = NAVAdapterUtils.NAVFormatString(myNAVItem.ProductUDF3.ToString(), 40);
                        myWMSItem.ProductUDF4 = NAVAdapterUtils.NAVFormatString(myNAVItem.ProductUDF4.ToString(), 40);
                        myWMSItem.ProductUDF5 = NAVAdapterUtils.NAVFormatString(myNAVItem.ProductUDF5.ToString(), 40);
                        myWMSItem.QuantityAvailable = NAVAdapterUtils.NAVFormatInt(myNAVItem.QuantityAvailable.ToString());
                        myWMSItem.QuantityPrecision = NAVAdapterUtils.NAVFormatInt(myNAVItem.DecimalQtyPrecision.ToString());
                        myWMSItem.ReceiveAttributeTracking.Attribute1Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVItem.ReceiveAttributeTracking.Attribute1Tracking.ToString());
                        myWMSItem.ReceiveAttributeTracking.Attribute2Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVItem.ReceiveAttributeTracking.Attribute2Tracking.ToString());
                        myWMSItem.ReceiveAttributeTracking.Attribute3Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVItem.ReceiveAttributeTracking.Attribute3Tracking.ToString());
                        myWMSItem.ReceiveAttributeTracking.Attribute4Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVItem.ReceiveAttributeTracking.Attribute4Tracking.ToString());
                        myWMSItem.ReceiveAttributeTracking.Attribute5Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVItem.ReceiveAttributeTracking.Attribute5Tracking.ToString());
                        myWMSItem.ReceiveAttributeTracking.Attribute6Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVItem.ReceiveAttributeTracking.Attribute6Tracking.ToString());
                        myWMSItem.ReceiveAttributeTracking.Attribute7Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVItem.ReceiveAttributeTracking.Attribute7Tracking.ToString());
                        myWMSItem.ReceiveAttributeTracking.Attribute8Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVItem.ReceiveAttributeTracking.Attribute8Tracking.ToString());
                        myWMSItem.ReceiveAttributeTracking.Attribute9Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVItem.ReceiveAttributeTracking.Attribute9Tracking.ToString());
                        myWMSItem.ReceiveAttributeTracking.Attribute10Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVItem.ReceiveAttributeTracking.Attribute10Tracking.ToString());
                        myWMSItem.ReorderPoint = NAVAdapterUtils.NAVFormatInt(myNAVItem.ReorderPoint.ToString());
                        myWMSItem.RetailPrice = NAVAdapterUtils.NAVFormatDecimal(myNAVItem.RetailPrice.ToString());
                        myWMSItem.Size1 = NAVAdapterUtils.NAVFormatInt(myNAVItem.Size1.ToString());
                        myWMSItem.Size1ItemType = NAVAdapterUtils.NAVFormatString(myNAVItem.Size1ItemType.ToString(), 40);
                        myWMSItem.Size2 = NAVAdapterUtils.NAVFormatInt(myNAVItem.Size2.ToString());
                        myWMSItem.Size2ItemType = NAVAdapterUtils.NAVFormatString(myNAVItem.Size2ItemType.ToString(), 40);
                        myWMSItem.Size3 = NAVAdapterUtils.NAVFormatInt(myNAVItem.Size3.ToString());
                        myWMSItem.Size3ItemType = NAVAdapterUtils.NAVFormatString(myNAVItem.Size3ItemType.ToString(), 40);
                        myWMSItem.Size4 = NAVAdapterUtils.NAVFormatInt(myNAVItem.Size4.ToString());
                        myWMSItem.Size4ItemType = NAVAdapterUtils.NAVFormatString(myNAVItem.Size4ItemType.ToString(), 40);
                        myWMSItem.Size5 = NAVAdapterUtils.NAVFormatInt(myNAVItem.Size5.ToString());
                        myWMSItem.Size5ItemType = NAVAdapterUtils.NAVFormatString(myNAVItem.Size5ItemType.ToString(), 40);
                        myWMSItem.SVAStagingID = myNAVItem.StagingID.ToString();
                        myWMSItem.SupplierType = NAVAdapterUtils.NAVFormatString(myNAVItem.SupplierType.ToString(), 40);
                        myWMSItem.TariffCode = NAVAdapterUtils.NAVFormatString(myNAVItem.TariffCode.ToString(), 10);
                        myWMSItem.UPC = NAVAdapterUtils.NAVFormatString(myNAVItem.UPC.ToString(), 40);
                        myWMSItem.VendorNumber = NAVAdapterUtils.NAVFormatString(myNAVItem.VendorNumber.ToString(), 40);
                        myWMSItem.VendorProductNumber = NAVAdapterUtils.NAVFormatString(myNAVItem.VendorProductCode.ToString(), 40);
                        myWMSItem.Weight = NAVAdapterUtils.NAVFormatDecimal(myNAVItem.Weight.ToString());
                        myWMSItem.WholesalePacksize = NAVAdapterUtils.NAVFormatInt(myNAVItem.WholesalePackSize.ToString());
                        myWMSItem.Width = NAVAdapterUtils.NAVFormatDecimal(myNAVItem.Width.ToString());

                        myWMSItems.Add(myWMSItem);
                        EventNotificationClient.Instance.PostMessage("NAV.Download.InventoryItems", CompanyConfig.InstanceId, "A1I -> Status Update -> NAV", new Variable("Status", "Sent to WMS"), new Variable("Product Code", NAVAdapterUtils.NAVFormatString(myNAVItem.ProductCode.ToString(),40)), new Variable("Entry No", NAVAdapterUtils.NAVFormatString(myNAVItem.StagingID.ToString(),40)));
                        try
                        {
                            // Call NAV Web Service | SentStatus
                            CompanyConfig.Instance.NavWebServiceInstance.SentStatus(NAVAdapterUtils.NAVFormatString(myNAVItem.StagingID.ToString(), 40),"VA");
                            CompanyConfig.Instance.NavWebServiceInstance.Dispose();
                        }
                        catch (Exception ex1)
                        {
                            EventNotificationClient.Instance.PostMessage("NAV.Download.InventoryItems.Error", CompanyConfig.InstanceId, "Error: A1I Entry No -> NAV", new Variable("Error Sending to WMS", ex1));
                        }
                    }
                }
            }
            catch (Exception ex1)
            {
                EventNotificationClient.Instance.PostMessage("NAV.Download.InventoryItems.Error", CompanyConfig.InstanceId, "Error", new Variable("Error Message", ex1));

                try
                {
                    if (myWMSItems.Count == 0)
                    {
                        myWMSItems = new List<InventoryItem>();
                    }
                }
                catch (Exception ex)
                {
                    EventNotificationClient.Instance.PostMessage("NAV.Download.InventoryItems.Error", CompanyConfig.InstanceId, "Error", new Variable("XMLPort Result Message", myResult), new Variable("Error Message", ex));
                    myWMSItems = new List<InventoryItem>();
                }
                return myWMSItems;
            }
            return myWMSItems;
        }
        #endregion
    }
}

