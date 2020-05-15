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
	public class ProductionNAVAdapter
    {
        #region SVA RetrieveFromNAV
        public static List<Packslip> RetrieveFromNAV()
        {
            NAVProductionOrderRoot myNAVProductionOrderRoot = new NAVProductionOrderRoot();
            List<Packslip> myWMSProductionOrders = new List<Packslip>();
            List<ProductionRecipe> myWMSProductionRecipes = new List<ProductionRecipe>();
            bool didFailureOccur;

            String myResult = String.Empty;
            try
            {
                Packslip myWMSPackslip;
                PackslipLine myWMSPackslipLine;
                ProductionRecipe myWMSProductionRecipe;

                // Call NAV Web Service | ExportPurchaseOrder
                myResult = CompanyConfig.Instance.NavWebServiceInstance.ExportProductionOrder(ref myNAVProductionOrderRoot);
                CompanyConfig.Instance.NavWebServiceInstance.Dispose();

                EventNotificationClient.Instance.PostMessage("NAV.Download.ProductionOrders", CompanyConfig.InstanceId, "Exported NAV Data",
                    new Variable("data", Utils.SerializeToString(myNAVProductionOrderRoot)));

                //NAV Business Object | Code = WO
                NAVBusinessObjectWO myNAVBusinessObjectWO = myNAVProductionOrderRoot.NAVBusinessObjectWO;

                //NAV Pick Headers | Processed = Ready to Send
                NAVWorkOrders myNAVWorkOrders = myNAVBusinessObjectWO.NAVWorkOrders;

                try
                {
                    if (myNAVWorkOrders.NAVWorkOrder == null)
                    {
                        //There are no orders to process
                        myWMSProductionOrders = new List<Packslip>();
                        return myWMSProductionOrders;
                    }

                    // For each Pick Header record in the queue
                    foreach (NAVWorkOrder myNAVWorkOrder in myNAVWorkOrders.NAVWorkOrder)
                    {
                        NAVAssemblyOrders myNAVProductionOrders = myNAVWorkOrder.NAVAssemblyOrders;

                        NAVAssemblyOrder myNAVProdOrder = myNAVProductionOrders.NAVAssemblyOrder;

                        myWMSPackslip = new Packslip();

                        // Parse Production Order Header
                        myWMSPackslip.RecordType = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.RecordType.ToString(), 40);
                        myWMSPackslip.BatchControlNumber = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.BatchControlNumber.ToString(), 40);
                        myWMSPackslip.CustomerNumber = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.CustomerNumber.ToString(), 40);
                        myWMSPackslip.OrderNumber = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.OrderNumber.ToString(), 40);
                        myWMSPackslip.BackOrderNumber = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.BackOrderNumber.ToString(), 40);
                        myWMSPackslip.CustomerPONumber = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.CustomerPONumber.ToString(), 40);
                        myWMSPackslip.CustomerLabel = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.CustomerLabel.ToString(), 40);
                        myWMSPackslip.CrystalPackingSlip = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.CrystalPackingSlip.ToString(), 40);
                        myWMSPackslip.CustomerGroup = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.CustomerGroup.ToString(), 40);
                        myWMSPackslip.LabelText = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.LabelText.ToString(), 40);
                        myWMSPackslip.ShipName = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.ShipName.ToString(), 40);
                        myWMSPackslip.ShipAddressName1 = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.ShipAddress1.ToString(), 40);
                        myWMSPackslip.ShipAddressName2 = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.ShipAddress2.ToString(), 40);
                        myWMSPackslip.ShipCity = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.ShipCity.ToString(), 40);
                        myWMSPackslip.ShipProvinceState = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.ShipProvinceState.ToString(), 40);
                        myWMSPackslip.ShipPostalZipCode = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.ShipPostalZipCode.ToString(), 40);
                        myWMSPackslip.ShipCountry = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.ShipCountry.ToString(), 40);
                        myWMSPackslip.ShipAttnTo = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.ShipAttnTo.ToString(), 40);
                        myWMSPackslip.ShipTelephoneNumber = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.ShipTelephoneNumber.ToString(), 40);
                        myWMSPackslip.BillName = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.BillName.ToString(), 40);
                        myWMSPackslip.BillAddress1 = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.BillAddress1.ToString(), 40);
                        myWMSPackslip.BillAddress2 = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.BillAddress2.ToString(), 40);
                        myWMSPackslip.BillCity = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.BillCity.ToString(), 40);
                        myWMSPackslip.BillProvinceState = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.BillProvinceState.ToString(), 40);
                        myWMSPackslip.BillPostalZip = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.BillPostalZipCode.ToString(), 40);
                        myWMSPackslip.BillCountry = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.BillCountry.ToString(), 40);
                        myWMSPackslip.ShipDateRequired = NAVAdapterUtils.NAVFormatDateTime(myNAVProdOrder.ShipDateRequired.ToString());
                        myWMSPackslip.ShipTimeRequired = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.ShipTimeRequired.ToString(), 40);
                        myWMSPackslip.TruckRoute = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.TruckRoute.ToString(), 40);
                        myWMSPackslip.Priority = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.Priority.ToString(), 40);
                        myWMSPackslip.ShipmentPaymentType = NAVAdapterUtils.NAVFormatShipmentPaymentType(myNAVProdOrder.ShipmentPaymentType.ToString());
                        myWMSPackslip.ShipViaPreferred = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.ShipViaPreferred.ToString(), 40);
                        myWMSPackslip.ShipPayAccountNumber = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.ShipPayAccountNumber.ToString(), 40);
                        myWMSPackslip.OrderDate = NAVAdapterUtils.NAVFormatDateTime(myNAVProdOrder.OrderDate.ToString());
                        myWMSPackslip.CancelDate = NAVAdapterUtils.NAVFormatDateTime(myNAVProdOrder.CancelDate.ToString());
                        myWMSPackslip.PurchaseOrderType = NAVAdapterUtils.NAVFormatPurchaseOrderType(myNAVProdOrder.PurchaseOrderType.ToString());
                        myWMSPackslip.Consolidate = NAVAdapterUtils.NAVFormatBoolean(myNAVProdOrder.Consolidate.ToString());
                        myWMSPackslip.ShipCompleteOnly = NAVAdapterUtils.NAVFormatBoolean(myNAVProdOrder.ShipCompleteOnly.ToString());
                        myWMSPackslip.ClientMH10Number = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.ClientMH10Number.ToString(), 40);
                        myWMSPackslip.ClientName = NAVAdapterUtils.getClientName();
                        myWMSPackslip.SpecialInstructions1 = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.SpecialInstructions1.ToString(), 40);
                        myWMSPackslip.SpecialInstructions2 = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.SpecialInstructions2.ToString(), 40);
                        myWMSPackslip.SpecialInstructions3 = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.SpecialInstructions3.ToString(), 40);
                        myWMSPackslip.SpecialInstructions4 = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.SpecialInstructions4.ToString(), 40);
                        myWMSPackslip.SpecialInstructions5 = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.SpecialInstructions5.ToString(), 40);
                        myWMSPackslip.SpecialInstructions6 = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.SpecialInstructions6.ToString(), 40);
                        myWMSPackslip.SpecialInstructions7 = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.SpecialInstructions7.ToString(), 40);
                        myWMSPackslip.CostCenter = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.CostCenter.ToString(), 40);
                        myWMSPackslip.ShipToNumber = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.ShipToNumber.ToString(), 40);
                        myWMSPackslip.IsCustomerAcceptsBackorders = NAVAdapterUtils.NAVFormatBoolean(myNAVProdOrder.IsCustomerAcceptsBackorders.ToString());
                        myWMSPackslip.TruckStop = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.TruckStop.ToString(), 40);
                        myWMSPackslip.TruckDeliveryTime = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.TruckDeliveryTime.ToString(), 40);
                        myWMSPackslip.ShipperName = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.ShipperName.ToString(), 40);
                        myWMSPackslip.ShipperAddress1 = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.ShipperAddress1.ToString(), 40);
                        myWMSPackslip.ShipperAddress2 = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.ShipperAddress2.ToString(), 40);
                        myWMSPackslip.ShipperCity = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.ShipperCity.ToString(), 40);
                        myWMSPackslip.ShipperProvinceState = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.ShipperProvinceState.ToString(), 40);
                        myWMSPackslip.ShipperPostalZipCode = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.ShipperPostalZipCode.ToString(), 40);
                        myWMSPackslip.PackingSlip = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.PackingSlip.ToString(), 40);
                        myWMSPackslip.IncludePickZones = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.IncludePickZones.ToString(), 40);
                        myWMSPackslip.ExcludePickZones = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.ExcludePickZones.ToString(), 40);
                        myWMSPackslip.IncludeReplenishmentZones = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.IncludeReplenishmentZones.ToString(), 40);
                        myWMSPackslip.ShipWithOtherGoods = NAVAdapterUtils.NAVFormatBoolean(myNAVProdOrder.ShipWithOtherGoods.ToString());
                        myWMSPackslip.DontSplitExpiryDates = NAVAdapterUtils.NAVFormatBoolean(myNAVProdOrder.DontSplitExpiryDates.ToString());
                        myWMSPackslip.ShipmentOptions = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.ShipmentOptions.ToString(), 40);
                        myWMSPackslip.ShipmentActions = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.ShipmentActions.ToString(), 40);
                        myWMSPackslip.ShipmentMessage = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.ShipmentMessage.ToString(), 40);
                        myWMSPackslip.ManifestReportName = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.ManifestReportName.ToString(), 40);
                        myWMSPackslip.COD = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.COD.ToString(), 40);
                        myWMSPackslip.ShipmentConsolidation = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.ShipmentConsolidation.ToString(), 40);
                        myWMSPackslip.PricingTicket = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.PricingTicket.ToString(), 40);
                        myWMSPackslip.DontShipBeforeDate = NAVAdapterUtils.NAVFormatDateTime(myNAVProdOrder.DontShipBeforeDate.ToString());
                        myWMSPackslip.ShipToFaxNumber = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.ShipToFaxNumber.ToString(), 40);
                        myWMSPackslip.WaveNumber = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.WaveNumber.ToString(), 40);
                        myWMSPackslip.ReplenishmentGroup = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.ReplenishmentGroup.ToString(), 40);
                        myWMSPackslip.CustomerUDF1 = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.CustomerUDF1.ToString(), 30);
                        myWMSPackslip.CustomerUDF2 = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.CustomerUDF2.ToString(), 30);
                        myWMSPackslip.CustomerUDF3 = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.CustomerUDF3.ToString(), 30);
                        myWMSPackslip.BusinessTelephoneNumber = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.BusinessTelephoneNumber.ToString(), 40);
                        myWMSPackslip.Email = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.Email.ToString(), 40);
                        myWMSPackslip.CustomerUDF4 = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.CustomerUDF4.ToString(), 30);
                        myWMSPackslip.CustomerUDF5 = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.CustomerUDF5.ToString(), 30);
                        myWMSPackslip.OrderSource = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.OrderSource.ToString(), 40);
                        //myWMSPackslip.TotalInvoiceAmountCharge = NAVAdapterUtils.NAVFormatDecimal(myNAVProdOrder.TotalInvoiceAmountCharge.ToString());
                        myWMSPackslip.ShipAddress3 = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.ShipAddress3.ToString(), 35);
                        myWMSPackslip.BillAddress3 = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.BillAddress3.ToString(), 35);
                        myWMSPackslip.ShipperAddress3 = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.ShipperAddress3.ToString(), 35);
                        myWMSPackslip.ShipperCountry = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.ShipperCountry.ToString(), 40);
                        myWMSPackslip.TotalFreightToCharge = NAVAdapterUtils.NAVFormatDecimal(myNAVProdOrder.TotalFreightToCharge.ToString());
                        myWMSPackslip.TotalShippingHandlingCharge = NAVAdapterUtils.NAVFormatDecimal(myNAVProdOrder.TotalShippingHandlingCharge.ToString());
                        myWMSPackslip.PromoAmount = NAVAdapterUtils.NAVFormatDecimal(myNAVProdOrder.PromoAmount.ToString());
                        myWMSPackslip.PromoDiscount = NAVAdapterUtils.NAVFormatDecimal(myNAVProdOrder.PromoDiscount.ToString());
                        myWMSPackslip.EndOfLineProcess = NAVAdapterUtils.NAVFormatInt(myNAVProdOrder.EndOfLineProcess.ToString());
                        myWMSPackslip.PurchaseOrderNumber = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.PurchaseOrderNumber.ToString(), 40);
                        myWMSPackslip.Special1 = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.Special1.ToString(), 40);
                        myWMSPackslip.Special2 = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.Special2.ToString(), 40);
                        myWMSPackslip.Special3 = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.Special3.ToString(), 40);
                        myWMSPackslip.MinimumDaysOfExpiry = NAVAdapterUtils.NAVFormatInt(myNAVProdOrder.MinimumDaysOfExpiry.ToString());
                        myWMSPackslip.MixedLotIndicator = NAVAdapterUtils.NAVFormatBoolean(myNAVProdOrder.MixedLotIndicator.ToString());
                        myWMSPackslip.Extra1 = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.Extra1.ToString(), 40);
                        myWMSPackslip.Extra2 = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.Extra2.ToString(), 40);
                        myWMSPackslip.Extra3 = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.Extra3.ToString(), 40);
                        myWMSPackslip.Extra4 = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.Extra4.ToString(), 40);
                        myWMSPackslip.Extra5 = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.Extra5.ToString(), 40);
                        myWMSPackslip.Handle = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.Handle.ToString(), 40);
                        myWMSPackslip.CustomerCartonContentLabel = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.CustomerCartonContentLabel.ToString(), 40);
                        myWMSPackslip.CartonType = NAVAdapterUtils.NAVFormatCartonType(myNAVProdOrder.CartonType.ToString());
                        myWMSPackslip.WarehouseTransitLocation = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.WarehouseTransitLocation.ToString(), 40);
                        myWMSPackslip.WarehouseDestinationLocation = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.WarehouseDestinationLocation.ToString(), 40);
                        myWMSPackslip.DestinationBinLocation = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.DestinationBin.ToString(), 40);
                        myWMSPackslip.PlannerID = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.PlannerID.ToString(), 40);
                        myWMSPackslip.ControlBuyerID = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.ControlBuyerID.ToString(), 40);
                        myWMSPackslip.ScheduledStartDate = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.ScheduledStartDate.ToString(), 40);
                        myWMSPackslip.FinishedGoodItemNumber = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.FinishedGoodItemNumber.ToString(), 40);
                        myWMSPackslip.FinishedGoodDescription = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.FinishedGoodDescription.ToString(), 40);
                        myWMSPackslip.FinishedGoodClassID = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.FinishedGoodClassID.ToString(), 40);
                        myWMSPackslip.MOENdingQuantity = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.MOEndingQuantity.ToString(), 40);
                        myWMSPackslip.FirstWorkCenter = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.FirstWorkCenter.ToString(), 40);
                        myWMSPackslip.MODescription = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.MODescription.ToString(), 40);
                        myWMSPackslip.PrimaryBillToAddress = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.PrimaryBillToAddress.ToString(), 40);
                        myWMSPackslip.WorkArea = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.WorkArea.ToString(), 40);
                        myWMSPackslip.AssemblyNote1 = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.AssemblyNote1.ToString(), 40);
                        myWMSPackslip.AssemblyNote2 = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.AssemblyNote2.ToString(), 40);
                        myWMSPackslip.AssemblyNote3 = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.AssemblyNote3.ToString(), 40);
                        myWMSPackslip.AssemblyNote4 = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.AssemblyNote4.ToString(), 40);
                        myWMSPackslip.AssemblyNote5 = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.AssemblyNote5.ToString(), 40);
                        myWMSPackslip.ShipperEIN = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.ShipperEmployerIdNumber.ToString(), 40);
                        myWMSPackslip.ShipToEIN = NAVAdapterUtils.NAVFormatString(myNAVProdOrder.ShipToEmployerIdNumber.ToString(), 40);
                        myWMSPackslip.SVAStagingID = myNAVProdOrder.StagingID.ToString();

                        didFailureOccur = false;

                        try
                        {
                            NAVAssemblyOrderLines myNAVProductionOrderLines = myNAVProdOrder.NAVAssemblyOrderLines;

                            foreach (NAVAssemblyOrderLine myNAVLine in myNAVProductionOrderLines.NAVAssemblyOrderLine)
                            {
                                myWMSPackslipLine = new PackslipLine();

                                // Parse Production Order Lines
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

                                myWMSPackslipLine.Attributes.Add(mySet);
                                myWMSPackslipLine.BackflushFlag = NAVAdapterUtils.NAVFormatBoolean(myNAVLine.BackflushFlag.ToString());
                                myWMSPackslipLine.BackOrderNumber = NAVAdapterUtils.NAVFormatString(myNAVLine.BackOrderNumber.ToString(), 40);
                                myWMSPackslipLine.CommentIndicator = NAVAdapterUtils.NAVFormatString(myNAVLine.CommentIndicator.ToString(), 40);
                                myWMSPackslipLine.Commitment = NAVAdapterUtils.NAVFormatInt(myNAVLine.Commitment.ToString());
                                myWMSPackslipLine.CommodityCode = NAVAdapterUtils.NAVFormatString(myNAVLine.CommodityCode.ToString(), 40);
                                myWMSPackslipLine.CountryOfOrigin = NAVAdapterUtils.NAVFormatString(myNAVLine.CountryOfOrigin.ToString(), 40);
                                myWMSPackslipLine.Cube = NAVAdapterUtils.NAVFormatDecimal(myNAVLine.Cube.ToString());
                                myWMSPackslipLine.CustomerLineReference = NAVAdapterUtils.NAVFormatString(myNAVLine.CustomerLineReference.ToString(), 40);
                                myWMSPackslipLine.CustomerNumber = NAVAdapterUtils.NAVFormatString(myNAVLine.CustomerNumber.ToString(), 40);
                                myWMSPackslipLine.CustomerPoNumber = NAVAdapterUtils.NAVFormatString(myNAVLine.CustomerPoNumber.ToString(), 40);
                                myWMSPackslipLine.CustomerXRef = NAVAdapterUtils.NAVFormatString(myNAVLine.CustomerXRef.ToString(), 40);
                                myWMSPackslipLine.DepartmentNumber = NAVAdapterUtils.NAVFormatString(myNAVLine.DepartmentNumber.ToString(), 40);
                                myWMSPackslipLine.Description = NAVAdapterUtils.NAVFormatString(myNAVLine.Description.ToString(), 100); // Changed to 100 characters for A1W 64 - iCepts 10/6/15
                                myWMSPackslipLine.Discount = NAVAdapterUtils.NAVFormatDecimal(myNAVLine.Discount.ToString());
                                myWMSPackslipLine.EDPNumber = NAVAdapterUtils.NAVFormatString(myNAVLine.EDPNumber.ToString(), 40);
                                myWMSPackslipLine.Extra = NAVAdapterUtils.NAVFormatString(myNAVLine.Extra.ToString(), 40);
                                myWMSPackslipLine.Extra1 = NAVAdapterUtils.NAVFormatString(myNAVLine.Extra1.ToString(), 40);
                                myWMSPackslipLine.Extra2 = NAVAdapterUtils.NAVFormatString(myNAVLine.Extra2.ToString(), 40);
                                myWMSPackslipLine.Extra3 = NAVAdapterUtils.NAVFormatString(myNAVLine.Extra3.ToString(), 40);
                                myWMSPackslipLine.Extra4 = NAVAdapterUtils.NAVFormatString(myNAVLine.Extra4.ToString(), 40);
                                myWMSPackslipLine.Extra5 = NAVAdapterUtils.NAVFormatString(myNAVLine.Extra5.ToString(), 40);
                                myWMSPackslipLine.HazmatCode = NAVAdapterUtils.NAVFormatString(myNAVLine.HazmatCode.ToString(), 40);
                                myWMSPackslipLine.InnerPacksize = NAVAdapterUtils.NAVFormatInt(myNAVLine.InnerPacksize.ToString());
                                myWMSPackslipLine.KitType = NAVAdapterUtils.NAVFormatKitType(myNAVLine.KitType.ToString());
                                myWMSPackslipLine.LicensePlate = NAVAdapterUtils.NAVFormatString(myNAVLine.LicensePlate.ToString(), 40);
                                myWMSPackslipLine.Location = NAVAdapterUtils.NAVFormatString(myNAVLine.Location.ToString(), 40);
                                myWMSPackslipLine.MiddlePacksize = NAVAdapterUtils.NAVFormatInt(myNAVLine.MiddlePacksize.ToString());
                                myWMSPackslipLine.MinimumDaysOfExpiry = NAVAdapterUtils.NAVFormatInt(myNAVLine.MinimumDaysOfExpiry.ToString());
                                myWMSPackslipLine.OrderNumber = NAVAdapterUtils.NAVFormatString(myNAVLine.OrderNumber.ToString(), 40);
                                myWMSPackslipLine.OrderSequenceNumber = NAVAdapterUtils.NAVFormatString(myNAVLine.OrderSequenceNumber.ToString(), 40);
                                myWMSPackslipLine.PackClass = NAVAdapterUtils.NAVFormatString(myNAVLine.PackClass.ToString(), 40);
                                myWMSPackslipLine.PickAttributeTracking.Attribute1Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVLine.PickAttributeTracking.Attribute1Tracking.ToString());
                                myWMSPackslipLine.PickAttributeTracking.Attribute2Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVLine.PickAttributeTracking.Attribute2Tracking.ToString());
                                myWMSPackslipLine.PickAttributeTracking.Attribute3Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVLine.PickAttributeTracking.Attribute3Tracking.ToString());
                                myWMSPackslipLine.PickAttributeTracking.Attribute4Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVLine.PickAttributeTracking.Attribute4Tracking.ToString());
                                myWMSPackslipLine.PickAttributeTracking.Attribute5Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVLine.PickAttributeTracking.Attribute5Tracking.ToString());
                                myWMSPackslipLine.PickAttributeTracking.Attribute6Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVLine.PickAttributeTracking.Attribute6Tracking.ToString());
                                myWMSPackslipLine.PickAttributeTracking.Attribute7Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVLine.PickAttributeTracking.Attribute7Tracking.ToString());
                                myWMSPackslipLine.PickAttributeTracking.Attribute8Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVLine.PickAttributeTracking.Attribute8Tracking.ToString());
                                myWMSPackslipLine.PickAttributeTracking.Attribute9Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVLine.PickAttributeTracking.Attribute9Tracking.ToString());
                                myWMSPackslipLine.PickAttributeTracking.Attribute10Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVLine.PickAttributeTracking.Attribute10Tracking.ToString());
                                myWMSPackslipLine.PriceTicketDescription = NAVAdapterUtils.NAVFormatString(myNAVLine.PriceTicketDescription.ToString(), 40);
                                myWMSPackslipLine.PrimaryLocation = NAVAdapterUtils.NAVFormatString(myNAVLine.PrimaryBin.ToString(), 40);
                                myWMSPackslipLine.ProductClass = NAVAdapterUtils.NAVFormatString(myNAVLine.ProductClass.ToString(), 40);
                                myWMSPackslipLine.ProductCode = NAVAdapterUtils.NAVFormatString(myNAVLine.ProductCode.ToString(), 40);
                                myWMSPackslipLine.PromoNumber = NAVAdapterUtils.NAVFormatString(myNAVLine.PromoNumber.ToString(), 40);
                                myWMSPackslipLine.PurchaseOrderLineNumber = NAVAdapterUtils.NAVFormatString(myNAVLine.PurchaseOrderLineNumber.ToString(), 40);
                                myWMSPackslipLine.PurchaseOrderNumber = NAVAdapterUtils.NAVFormatString(myNAVLine.PurchaseOrderNumber.ToString(), 40);
                                myWMSPackslipLine.QuantityOrdered = NAVAdapterUtils.NAVFormatDecimal(myNAVLine.QuantityOrdered.ToString());
                                myWMSPackslipLine.QuantityToPick = NAVAdapterUtils.NAVFormatDecimal(myNAVLine.QuantityToPick.ToString());
                                myWMSPackslipLine.ReservationType = NAVAdapterUtils.NAVFormatReservationType(myNAVLine.ReservationType.ToString());
                                myWMSPackslipLine.RetailPrice = NAVAdapterUtils.NAVFormatDecimal(myNAVLine.RetailPrice.ToString());
                                myWMSPackslipLine.Salesman = NAVAdapterUtils.NAVFormatString(myNAVLine.Salesman.ToString(), 40);
                                myWMSPackslipLine.SellPrice = NAVAdapterUtils.NAVFormatDecimal(myNAVLine.SellPrice.ToString());
                                myWMSPackslipLine.SpecialInstructions1 = NAVAdapterUtils.NAVFormatString(myNAVLine.SpecialInstructions1.ToString(), 40);
                                myWMSPackslipLine.SpecialInstructions2 = NAVAdapterUtils.NAVFormatString(myNAVLine.SpecialInstructions2.ToString(), 40);
                                myWMSPackslipLine.SpecialInstructions3 = NAVAdapterUtils.NAVFormatString(myNAVLine.SpecialInstructions3.ToString(), 40);
                                myWMSPackslipLine.SpecialInstructions4 = NAVAdapterUtils.NAVFormatString(myNAVLine.SpecialInstructions4.ToString(), 40);
                                myWMSPackslipLine.SpecialInstructions5 = NAVAdapterUtils.NAVFormatString(myNAVLine.SpecialInstructions5.ToString(), 40);
                                myWMSPackslipLine.SpecialInstructions6 = NAVAdapterUtils.NAVFormatString(myNAVLine.SpecialInstructions6.ToString(), 40);
                                myWMSPackslipLine.SpecialInstructions7 = NAVAdapterUtils.NAVFormatString(myNAVLine.SpecialInstructions7.ToString(), 40);
                                myWMSPackslipLine.StockItem = NAVAdapterUtils.NAVFormatString(myNAVLine.StockItem.ToString(), 40);
                                myWMSPackslipLine.StoreAddress1 = NAVAdapterUtils.NAVFormatString(myNAVLine.StoreAddress1.ToString(), 40);
                                myWMSPackslipLine.StoreAddress2 = NAVAdapterUtils.NAVFormatString(myNAVLine.StoreAddress2.ToString(), 40);
                                myWMSPackslipLine.StoreCity = NAVAdapterUtils.NAVFormatString(myNAVLine.StoreCity.ToString(), 40);
                                myWMSPackslipLine.StoreCountry = NAVAdapterUtils.NAVFormatString(myNAVLine.StoreCountry.ToString(), 40);
                                myWMSPackslipLine.StoreName = NAVAdapterUtils.NAVFormatString(myNAVLine.StoreName.ToString(), 40);
                                myWMSPackslipLine.StoreNumber = NAVAdapterUtils.NAVFormatString(myNAVLine.StoreNumber.ToString(), 40);
                                myWMSPackslipLine.StorePostalZip = NAVAdapterUtils.NAVFormatString(myNAVLine.StorePostalZipCode.ToString(), 40);
                                myWMSPackslipLine.StoreProvinceState = NAVAdapterUtils.NAVFormatString(myNAVLine.StoreProvinceState.ToString(), 40);
                                myWMSPackslipLine.Tax1 = NAVAdapterUtils.NAVFormatBoolean(myNAVLine.Tax1.ToString());
                                myWMSPackslipLine.Tax2 = NAVAdapterUtils.NAVFormatBoolean(myNAVLine.Tax2.ToString());
                                myWMSPackslipLine.Tax3 = NAVAdapterUtils.NAVFormatBoolean(myNAVLine.Tax3.ToString());
                                myWMSPackslipLine.Tax4 = NAVAdapterUtils.NAVFormatBoolean(myNAVLine.Tax4.ToString());
                                myWMSPackslipLine.UnitOfMeasureMultiplier = NAVAdapterUtils.NAVFormatString(myNAVLine.UnitOfMeasureMultiplier.ToString(), 40);
                                myWMSPackslipLine.UnitOfMeasureTextDescription = NAVAdapterUtils.NAVFormatString(myNAVLine.UnitOfMeasureTextDescription.ToString(), 40);
                                myWMSPackslipLine.UnitPrice = NAVAdapterUtils.NAVFormatDecimal(myNAVLine.UnitPrice.ToString());
                                myWMSPackslipLine.UPC = NAVAdapterUtils.NAVFormatString(myNAVLine.UPC.ToString(), 40);
                                myWMSPackslipLine.ValuePrice = NAVAdapterUtils.NAVFormatDecimal(myNAVLine.ValuePrice.ToString());
                                myWMSPackslipLine.VariableLengthProduct = NAVAdapterUtils.NAVFormatVariableLengthType(myNAVLine.VariableLengthProduct.ToString());
                                myWMSPackslipLine.VendorNumber = NAVAdapterUtils.NAVFormatString(myNAVLine.VendorNumber.ToString(), 40);
                                myWMSPackslipLine.Warehouse = NAVAdapterUtils.NAVFormatString(myNAVLine.Warehouse.ToString(), 40);
                                myWMSPackslipLine.Weight = NAVAdapterUtils.NAVFormatDecimal(myNAVLine.Weight.ToString());

                                try
                                {
                                    NAVAssemblyRecipes myNAVRecipes = myNAVLine.NAVAssemblyRecipes;

                                    try
                                    {
                                        foreach (NAVAssemblyRecipe myNAVRecipe in myNAVRecipes.NAVAssemblyRecipe)
                                        {
                                            myWMSProductionRecipe = new ProductionRecipe();

                                            // Parse Production Order Recipe
                                            myWMSProductionRecipe.Attributes = new List<Accellos.Interfaces.NAV.BusinessObjects.AttributeSet>();

                                            Accellos.Interfaces.NAV.BusinessObjects.AttributeSet tempSet = new Accellos.Interfaces.NAV.BusinessObjects.AttributeSet();

                                            tempSet.Attribute1 = NAVAdapterUtils.NAVFormatString(myNAVRecipe.AttributeSet.Attribute1.ToString(), 40);
                                            tempSet.Attribute2 = NAVAdapterUtils.NAVFormatString(myNAVRecipe.AttributeSet.Attribute2.ToString(), 40);
                                            tempSet.Attribute3 = NAVAdapterUtils.NAVFormatString(myNAVRecipe.AttributeSet.Attribute3.ToString(), 40);
                                            tempSet.Attribute4 = NAVAdapterUtils.NAVFormatString(myNAVRecipe.AttributeSet.Attribute4.ToString(), 40);
                                            tempSet.Attribute5 = NAVAdapterUtils.NAVFormatString(myNAVRecipe.AttributeSet.Attribute5.ToString(), 40);
                                            tempSet.Attribute6 = NAVAdapterUtils.NAVFormatString(myNAVRecipe.AttributeSet.Attribute6.ToString(), 40);
                                            tempSet.Attribute7 = NAVAdapterUtils.NAVFormatString(myNAVRecipe.AttributeSet.Attribute7.ToString(), 40);
                                            tempSet.Attribute8 = NAVAdapterUtils.NAVFormatString(myNAVRecipe.AttributeSet.Attribute8.ToString(), 40);
                                            tempSet.Attribute9 = NAVAdapterUtils.NAVFormatString(myNAVRecipe.AttributeSet.Attribute9.ToString(), 40);
                                            tempSet.Attribute10 = NAVAdapterUtils.NAVFormatString(myNAVRecipe.AttributeSet.Attribute10.ToString(), 40);
                                            myWMSProductionRecipe.Attributes.Add(tempSet);

                                            myWMSProductionRecipe.BackorderNumber = NAVAdapterUtils.NAVFormatString(myNAVRecipe.BackorderNumber.ToString(), 40);
                                            myWMSProductionRecipe.ClientName = NAVAdapterUtils.NAVFormatString(myNAVRecipe.ClientName.ToString(), 40);
                                            myWMSProductionRecipe.CommentIndicator = NAVAdapterUtils.NAVFormatString(myNAVRecipe.CommentIndicator.ToString(), 40);
                                            myWMSProductionRecipe.ComponentCost = NAVAdapterUtils.NAVFormatDecimal(myNAVRecipe.ComponentCost.ToString());
                                            myWMSProductionRecipe.ComponentSpecialInstructions = NAVAdapterUtils.NAVFormatString(myNAVRecipe.ComponentSpecialInstructions.ToString(), 40);
                                            myWMSProductionRecipe.ComponentType = NAVAdapterUtils.NAVFormatComponentType(myNAVRecipe.ComponentType.ToString());
                                            myWMSProductionRecipe.CustomerNumber = NAVAdapterUtils.NAVFormatString(myNAVRecipe.CustomerNumber.ToString(), 40);
                                            myWMSProductionRecipe.Description = NAVAdapterUtils.NAVFormatString(myNAVRecipe.Description.ToString(), 100); // Changed to 100 characters for A1W 64 - iCepts 10/6/15
                                            myWMSProductionRecipe.Extra1 = NAVAdapterUtils.NAVFormatString(myNAVRecipe.Extra1.ToString(), 40);
                                            myWMSProductionRecipe.Extra2 = NAVAdapterUtils.NAVFormatString(myNAVRecipe.Extra2.ToString(), 40);
                                            myWMSProductionRecipe.Extra3 = NAVAdapterUtils.NAVFormatString(myNAVRecipe.Extra3.ToString(), 40);
                                            myWMSProductionRecipe.Extra4 = NAVAdapterUtils.NAVFormatString(myNAVRecipe.Extra4.ToString(), 40);
                                            myWMSProductionRecipe.Extra5 = NAVAdapterUtils.NAVFormatString(myNAVRecipe.Extra5.ToString(), 40);
                                            myWMSProductionRecipe.OrderNumber = NAVAdapterUtils.NAVFormatString(myNAVRecipe.OrderNumber.ToString(), 40);
                                            myWMSProductionRecipe.OrderSequenceNumber = NAVAdapterUtils.NAVFormatString(myNAVRecipe.OrderSequenceNumber.ToString(), 40);
                                            myWMSProductionRecipe.PackQuantity = NAVAdapterUtils.NAVFormatInt(myNAVRecipe.PackQuantity.ToString());
                                            myWMSProductionRecipe.PickAttributeTracking.Attribute1Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVRecipe.PickAttributeTracking.Attribute1Tracking.ToString());
                                            myWMSProductionRecipe.PickAttributeTracking.Attribute2Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVRecipe.PickAttributeTracking.Attribute2Tracking.ToString());
                                            myWMSProductionRecipe.PickAttributeTracking.Attribute3Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVRecipe.PickAttributeTracking.Attribute3Tracking.ToString());
                                            myWMSProductionRecipe.PickAttributeTracking.Attribute4Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVRecipe.PickAttributeTracking.Attribute4Tracking.ToString());
                                            myWMSProductionRecipe.PickAttributeTracking.Attribute5Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVRecipe.PickAttributeTracking.Attribute5Tracking.ToString());
                                            myWMSProductionRecipe.PickAttributeTracking.Attribute6Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVRecipe.PickAttributeTracking.Attribute6Tracking.ToString());
                                            myWMSProductionRecipe.PickAttributeTracking.Attribute7Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVRecipe.PickAttributeTracking.Attribute7Tracking.ToString());
                                            myWMSProductionRecipe.PickAttributeTracking.Attribute8Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVRecipe.PickAttributeTracking.Attribute8Tracking.ToString());
                                            myWMSProductionRecipe.PickAttributeTracking.Attribute9Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVRecipe.PickAttributeTracking.Attribute9Tracking.ToString());
                                            myWMSProductionRecipe.PickAttributeTracking.Attribute10Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVRecipe.PickAttributeTracking.Attribute10Tracking.ToString());
                                            myWMSProductionRecipe.PrimaryLocation = NAVAdapterUtils.NAVFormatString(myNAVRecipe.PrimaryLocation.ToString(), 40);
                                            myWMSProductionRecipe.ProductCode = NAVAdapterUtils.NAVFormatString(myNAVRecipe.ProductCode.ToString(), 40);
                                            myWMSProductionRecipe.QuantityBuilt = NAVAdapterUtils.NAVFormatDecimal(myNAVRecipe.QuantityBuilt.ToString());
                                            myWMSProductionRecipe.QuantityToUse = NAVAdapterUtils.NAVFormatDecimal(myNAVRecipe.QuantityToUse.ToString());
                                            myWMSProductionRecipe.ReceiveAttributeTracking.Attribute1Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVRecipe.PickAttributeTracking.Attribute1Tracking.ToString());
                                            myWMSProductionRecipe.ReceiveAttributeTracking.Attribute2Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVRecipe.PickAttributeTracking.Attribute2Tracking.ToString());
                                            myWMSProductionRecipe.ReceiveAttributeTracking.Attribute3Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVRecipe.PickAttributeTracking.Attribute3Tracking.ToString());
                                            myWMSProductionRecipe.ReceiveAttributeTracking.Attribute4Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVRecipe.PickAttributeTracking.Attribute4Tracking.ToString());
                                            myWMSProductionRecipe.ReceiveAttributeTracking.Attribute5Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVRecipe.PickAttributeTracking.Attribute5Tracking.ToString());
                                            myWMSProductionRecipe.ReceiveAttributeTracking.Attribute6Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVRecipe.PickAttributeTracking.Attribute6Tracking.ToString());
                                            myWMSProductionRecipe.ReceiveAttributeTracking.Attribute7Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVRecipe.PickAttributeTracking.Attribute7Tracking.ToString());
                                            myWMSProductionRecipe.ReceiveAttributeTracking.Attribute8Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVRecipe.PickAttributeTracking.Attribute8Tracking.ToString());
                                            myWMSProductionRecipe.ReceiveAttributeTracking.Attribute9Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVRecipe.PickAttributeTracking.Attribute9Tracking.ToString());
                                            myWMSProductionRecipe.ReceiveAttributeTracking.Attribute10Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVRecipe.PickAttributeTracking.Attribute10Tracking.ToString());
                                            myWMSProductionRecipe.UnitOfMeasure = NAVAdapterUtils.NAVFormatString(myNAVRecipe.UnitOfMeasure.ToString(), 40);
                                            myWMSProductionRecipe.Warehouse = NAVAdapterUtils.NAVFormatString(myNAVRecipe.Warehouse.ToString(), 40);
                                            myWMSProductionRecipe.WorkArea = NAVAdapterUtils.NAVFormatString(myNAVRecipe.WorkArea.ToString(), 40);

                                            //Add Production Recipe to Production Order Line
                                            myWMSPackslipLine.ProductionRecipes.Add(myWMSProductionRecipe);
                                        }
                                    }
                                    catch (Exception exceptionProdRec)
                                    {
                                        //Error in Production Receipe
                                        EventNotificationClient.Instance.PostMessage("NAV.Download.ProductionOrders.Error", CompanyConfig.InstanceId,
                                            "Error: Production Receipe Level", new Variable("Error Sending to WMS", exceptionProdRec));
                                        didFailureOccur = true;
                                        break; //error occurred, no need to continue loop if we're not adding this
                                    }

                                    //Add Production Order Line to Production Order
                                    myWMSPackslip.Lines.Add(myWMSPackslipLine);
                                }
                                catch (Exception exceptionProdOrdLineLvl)
                                {
                                    //Error at Production Order Line Level
                                    EventNotificationClient.Instance.PostMessage("NAV.Download.ProductionOrders.Error", CompanyConfig.InstanceId, "Error: Production Order Line Level", new Variable("Error Sending to WMS", exceptionProdOrdLineLvl));
                                    didFailureOccur = true;
                                    break; //error occurred, no need to continue loop if we're not adding this
                                }
                            }
                        }
                        catch (Exception exceptionProdOrdLinesLvl)
                        {
                            //Error at Production Order Lines Level
                            EventNotificationClient.Instance.PostMessage("NAV.Download.ProductionOrders.Error", CompanyConfig.InstanceId, "Error: Production Order Lines Level", new Variable("Error Sending to WMS", exceptionProdOrdLinesLvl));
                            didFailureOccur = true;
                        }

                        //Add Production Order (if succeeded) to List of Production Orders
                        if (!didFailureOccur)
                        {
                            myWMSProductionOrders.Add(myWMSPackslip);

                            EventNotificationClient.Instance.PostMessage("NAV.Download.ProductionOrders", CompanyConfig.InstanceId,
                                "A1I -> Status Update -> NAV", new Variable("Status", "Sent to WMS"),
                                new Variable("Production Order Number", myNAVProdOrder.OrderNumber),
                                new Variable("Entry No", myNAVProdOrder.StagingID));

                            //try
                            //{
                            //    // Call NAV Web Service | SentStatus
                            //    CompanyConfig.Instance.NavWebServiceInstance.SentStatus(myWMSPackslip.SVAStagingID, "WO");
                            //    CompanyConfig.Instance.NavWebServiceInstance.Dispose();
                            //}
                            //catch (Exception ex1)
                            //{
                            //    EventNotificationClient.Instance.PostMessage("NAV.Download.ProductionOrders.Error", CompanyConfig.InstanceId, "Error: A1I Entry No -> NAV", new Variable("Error Sending to WMS", ex1));
                            //}
                        }
                    }
                }
                catch (Exception exceptionPSLvl)
                {
                    //Error at Packslip Level
                    EventNotificationClient.Instance.PostMessage("NAV.Download.ProductionOrders.Error", CompanyConfig.InstanceId, "Error: Packslip Level", new Variable("Error Sending to WMS", exceptionPSLvl));
                }
            }
            catch (Exception exceptionProdOrdRootLvl)
            {
                //Error at Production Order Root Level
                EventNotificationClient.Instance.PostMessage("NAV.Download.ProductionOrders.Error", CompanyConfig.InstanceId, "Error: Prodcution Order Root Level", new Variable("Error Sending to WMS", exceptionProdOrdRootLvl));
            }

            return myWMSProductionOrders;
        }
        #endregion

        #region SVA RetrievalRequestFromNAV
        public static List<Packslip> RetrievalRequestFromNAV()
        {
            NAVWORetrievalRoot myNAVWORetrievalRoot = new NAVWORetrievalRoot();
            List<Packslip> myWMSPackslips = new List<Packslip>();

            String myResult = String.Empty;
            try
            {
                Packslip myWMSPackslip;

                // Call NAV Web Service | RetrieveProductionOrder
                myResult = CompanyConfig.Instance.NavWebServiceInstance.RetrieveProductionOrder(ref myNAVWORetrievalRoot);
                CompanyConfig.Instance.NavWebServiceInstance.Dispose();

                //NAV Business Object | Code = WO
                NAVBusinessObjectWOR myNAVBusinessObjectWOR = new NAVBusinessObjectWOR();
                myNAVBusinessObjectWOR = myNAVWORetrievalRoot.NAVBusinessObjectWOR;

                //NAV Work Orders | Processed = Ready to Send | Header Only
                NAVWORetrievals myNAVWORetrievals = new NAVWORetrievals();
                myNAVWORetrievals = myNAVBusinessObjectWOR.NAVWORetrievals;

                try
                {
                    // For each Pick Header record in the queue
                    foreach (NAVWORetrieval myNAVOrder in myNAVWORetrievals.NAVWORetrieval)
                    {
                        myWMSPackslip = new Packslip();

                        // Parse Packslip Header
                        myWMSPackslip.SVAStagingID = myNAVOrder.StagingID.ToString();
                        myWMSPackslip.ClientName = NAVAdapterUtils.getClientName();
                        myWMSPackslip.OrderNumber = NAVAdapterUtils.NAVFormatString(myNAVOrder.OrderNumber.ToString(), 40);
                        myWMSPackslip.BackOrderNumber = myNAVOrder.BackOrderNumber.ToString();

                        //Add Packslips to List of Packslips
                        if (!string.IsNullOrEmpty(myNAVOrder.OrderNumber.ToString()))
                        {
                            myWMSPackslips.Add(myWMSPackslip);
                            EventNotificationClient.Instance.PostMessage("NAV.Download.ProductionOrderRetrieval", 
                                CompanyConfig.InstanceId, "A1I -> Status Update -> NAV", new Variable("Status", "Sent to WMS"), 
                                new Variable("Production Order Number", NAVAdapterUtils.NAVFormatString(myNAVOrder.OrderNumber.ToString(), 40)), 
                                new Variable("Entry No", NAVAdapterUtils.NAVFormatString(myNAVOrder.StagingID.ToString(), 40)));

                            try
                            {
                                // Call NAV Web Service | SentStatus
                                CompanyConfig.Instance.NavWebServiceInstance.SentStatus(
                                    NAVAdapterUtils.NAVFormatString(myNAVOrder.StagingID.ToString(), 40), "WO");
                                CompanyConfig.Instance.NavWebServiceInstance.Dispose();
                            }
                            catch (Exception ex1)
                            {
                                EventNotificationClient.Instance.PostMessage("NAV.Download.ProductionOrderRetrieval.Error", CompanyConfig.InstanceId, 
                                    "Error: A1I Entry No -> NAV", new Variable("Error Sending to WMS", ex1));
                            }
                        }
                    }
                }
                catch (Exception exceptionWOLvl)
                {
                    //Error at Packslip Level
                    EventNotificationClient.Instance.PostMessage("NAV.Download.ProductionOrderRetrieval.Error", CompanyConfig.InstanceId,
                        "Error: Production Order Level", new Variable("Error Sending to WMS", exceptionWOLvl));
                }
            }
            catch (Exception exceptionWORetLvl)
            {
                //Error at Packslip Retrieval Level
                EventNotificationClient.Instance.PostMessage("NAV.Download.ProductionOrderRetrieval.Error", CompanyConfig.InstanceId, 
                    "Error: Production Order Retrieval Level", new Variable("Error Sending to WMS", exceptionWORetLvl));
            }

            return myWMSPackslips;
        }
        #endregion
    }
}
    
