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
	public class AssemblyNAVAdapter
    {
        #region SVA RetrieveFromNAV
        public static List<Packslip> RetrieveFromNAV()
        {
            NAVAssemblyOrderRoot myNAVAssemblyOrderRoot = new NAVAssemblyOrderRoot();
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
                myResult = CompanyConfig.Instance.NavWebServiceInstance.ExportAssemblyOrder(ref myNAVAssemblyOrderRoot);
                CompanyConfig.Instance.NavWebServiceInstance.Dispose();

                EventNotificationClient.Instance.PostMessage("NAV.Download.AssemblyOrders", CompanyConfig.InstanceId, "Exported NAV Data",
                    new Variable("data", Utils.SerializeToString(myNAVAssemblyOrderRoot)));

                //NAV Business Object | Code = O
                var myNAVBusinessObjectAO = myNAVAssemblyOrderRoot.NAVBusinessObjectAO;

                //NAV Pick Headers | Processed = Ready to Send
                var myNAVAssemblyOrders = myNAVBusinessObjectAO.NAVAssemblyOrders2;

                try
                {
                    if (myNAVAssemblyOrders.NAVAssemblyOrder2 == null)
                    {
                        //There are no orders to process
                        myWMSProductionOrders = new List<Packslip>();
                        return myWMSProductionOrders;
                    }

                    // For each Assembly Order record in the queue
                    foreach (NAVAssemblyOrder2 myNAVAssemblyOrder2 in myNAVAssemblyOrders.NAVAssemblyOrder2)
                    { 
                        var myNAVAssemblyOrder = myNAVAssemblyOrder2.NAVAssemblyOrders3.NAVAssemblyOrder3;
                        myWMSPackslip = new Packslip();

                        // Parse Production Order Header
                        myWMSPackslip.RecordType = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.RecordType.ToString(), 40);
                        myWMSPackslip.BatchControlNumber = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.BatchControlNumber.ToString(), 40);
                        myWMSPackslip.CustomerNumber = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.CustomerNumber.ToString(), 40);
                        myWMSPackslip.OrderNumber = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.OrderNumber.ToString(), 40);
                        myWMSPackslip.BackOrderNumber = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.BackOrderNumber.ToString(), 40);
                        myWMSPackslip.CustomerPONumber = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.CustomerPONumber.ToString(), 40);
                        myWMSPackslip.CustomerLabel = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.CustomerLabel.ToString(), 40);
                        myWMSPackslip.CrystalPackingSlip = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.CrystalPackingSlip.ToString(), 40);
                        myWMSPackslip.CustomerGroup = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.CustomerGroup.ToString(), 40);
                        myWMSPackslip.LabelText = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.LabelText.ToString(), 40);
                        myWMSPackslip.ShipName = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.ShipName.ToString(), 40);
                        myWMSPackslip.ShipAddressName1 = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.ShipAddress1.ToString(), 40);
                        myWMSPackslip.ShipAddressName2 = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.ShipAddress2.ToString(), 40);
                        myWMSPackslip.ShipCity = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.ShipCity.ToString(), 40);
                        myWMSPackslip.ShipProvinceState = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.ShipProvinceState.ToString(), 40);
                        myWMSPackslip.ShipPostalZipCode = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.ShipPostalZipCode.ToString(), 40);
                        myWMSPackslip.ShipCountry = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.ShipCountry.ToString(), 40);
                        myWMSPackslip.ShipAttnTo = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.ShipAttnTo.ToString(), 40);
                        myWMSPackslip.ShipTelephoneNumber = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.ShipTelephoneNumber.ToString(), 40);
                        myWMSPackslip.BillName = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.BillName.ToString(), 40);
                        myWMSPackslip.BillAddress1 = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.BillAddress1.ToString(), 40);
                        myWMSPackslip.BillAddress2 = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.BillAddress2.ToString(), 40);
                        myWMSPackslip.BillCity = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.BillCity.ToString(), 40);
                        myWMSPackslip.BillProvinceState = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.BillProvinceState.ToString(), 40);
                        myWMSPackslip.BillPostalZip = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.BillPostalZipCode.ToString(), 40);
                        myWMSPackslip.BillCountry = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.BillCountry.ToString(), 40);
                        myWMSPackslip.ShipDateRequired = NAVAdapterUtils.NAVFormatDateTime(myNAVAssemblyOrder.ShipDateRequired.ToString());
                        myWMSPackslip.ShipTimeRequired = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.ShipTimeRequired.ToString(), 40);
                        myWMSPackslip.TruckRoute = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.TruckRoute.ToString(), 40);
                        myWMSPackslip.Priority = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.Priority.ToString(), 40);
                        myWMSPackslip.ShipmentPaymentType = NAVAdapterUtils.NAVFormatShipmentPaymentType(myNAVAssemblyOrder.ShipmentPaymentType.ToString());
                        myWMSPackslip.ShipViaPreferred = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.ShipViaPreferred.ToString(), 40);
                        myWMSPackslip.ShipPayAccountNumber = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.ShipPayAccountNumber.ToString(), 40);
                        myWMSPackslip.OrderDate = NAVAdapterUtils.NAVFormatDateTime(myNAVAssemblyOrder.OrderDate.ToString());
                        myWMSPackslip.CancelDate = NAVAdapterUtils.NAVFormatDateTime(myNAVAssemblyOrder.CancelDate.ToString());
                        myWMSPackslip.PurchaseOrderType = NAVAdapterUtils.NAVFormatPurchaseOrderType(myNAVAssemblyOrder.PurchaseOrderType.ToString());
                        myWMSPackslip.Consolidate = NAVAdapterUtils.NAVFormatBoolean(myNAVAssemblyOrder.Consolidate.ToString());
                        myWMSPackslip.ShipCompleteOnly = NAVAdapterUtils.NAVFormatBoolean(myNAVAssemblyOrder.ShipCompleteOnly.ToString());
                        myWMSPackslip.ClientMH10Number = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.ClientMH10Number.ToString(), 40);
                        myWMSPackslip.ClientName = NAVAdapterUtils.getClientName();
                        myWMSPackslip.SpecialInstructions1 = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.SpecialInstructions1.ToString(), 40);
                        myWMSPackslip.SpecialInstructions2 = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.SpecialInstructions2.ToString(), 40);
                        myWMSPackslip.SpecialInstructions3 = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.SpecialInstructions3.ToString(), 40);
                        myWMSPackslip.SpecialInstructions4 = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.SpecialInstructions4.ToString(), 40);
                        myWMSPackslip.SpecialInstructions5 = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.SpecialInstructions5.ToString(), 40);
                        myWMSPackslip.SpecialInstructions6 = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.SpecialInstructions6.ToString(), 40);
                        myWMSPackslip.SpecialInstructions7 = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.SpecialInstructions7.ToString(), 40);
                        myWMSPackslip.CostCenter = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.CostCenter.ToString(), 40);
                        myWMSPackslip.ShipToNumber = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.ShipToNumber.ToString(), 40);
                        myWMSPackslip.IsCustomerAcceptsBackorders = NAVAdapterUtils.NAVFormatBoolean(myNAVAssemblyOrder.IsCustomerAcceptsBackorders.ToString());
                        myWMSPackslip.TruckStop = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.TruckStop.ToString(), 40);
                        myWMSPackslip.TruckDeliveryTime = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.TruckDeliveryTime.ToString(), 40);
                        myWMSPackslip.ShipperName = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.ShipperName.ToString(), 40);
                        myWMSPackslip.ShipperAddress1 = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.ShipperAddress1.ToString(), 40);
                        myWMSPackslip.ShipperAddress2 = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.ShipperAddress2.ToString(), 40);
                        myWMSPackslip.ShipperCity = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.ShipperCity.ToString(), 40);
                        myWMSPackslip.ShipperProvinceState = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.ShipperProvinceState.ToString(), 40);
                        myWMSPackslip.ShipperPostalZipCode = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.ShipperPostalZipCode.ToString(), 40);
                        myWMSPackslip.PackingSlip = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.PackingSlip.ToString(), 40);
                        myWMSPackslip.IncludePickZones = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.IncludePickZones.ToString(), 40);
                        myWMSPackslip.ExcludePickZones = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.ExcludePickZones.ToString(), 40);
                        myWMSPackslip.IncludeReplenishmentZones = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.IncludeReplenishmentZones.ToString(), 40);
                        myWMSPackslip.ShipWithOtherGoods = NAVAdapterUtils.NAVFormatBoolean(myNAVAssemblyOrder.ShipWithOtherGoods.ToString());
                        myWMSPackslip.DontSplitExpiryDates = NAVAdapterUtils.NAVFormatBoolean(myNAVAssemblyOrder.DontSplitExpiryDates.ToString());
                        myWMSPackslip.ShipmentOptions = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.ShipmentOptions.ToString(), 40);
                        myWMSPackslip.ShipmentActions = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.ShipmentActions.ToString(), 40);
                        myWMSPackslip.ShipmentMessage = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.ShipmentMessage.ToString(), 40);
                        myWMSPackslip.ManifestReportName = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.ManifestReportName.ToString(), 40);
                        myWMSPackslip.COD = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.COD.ToString(), 40);
                        myWMSPackslip.ShipmentConsolidation = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.ShipmentConsolidation.ToString(), 40);
                        myWMSPackslip.PricingTicket = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.PricingTicket.ToString(), 40);
                        myWMSPackslip.DontShipBeforeDate = NAVAdapterUtils.NAVFormatDateTime(myNAVAssemblyOrder.DontShipBeforeDate.ToString());
                        myWMSPackslip.ShipToFaxNumber = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.ShipToFaxNumber.ToString(), 40);
                        myWMSPackslip.WaveNumber = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.WaveNumber.ToString(), 40);
                        myWMSPackslip.ReplenishmentGroup = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.ReplenishmentGroup.ToString(), 40);
                        myWMSPackslip.CustomerUDF1 = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.CustomerUDF1.ToString(), 30);
                        myWMSPackslip.CustomerUDF2 = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.CustomerUDF2.ToString(), 30);
                        myWMSPackslip.CustomerUDF3 = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.CustomerUDF3.ToString(), 30);
                        myWMSPackslip.BusinessTelephoneNumber = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.BusinessTelephoneNumber.ToString(), 40);
                        myWMSPackslip.Email = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.Email.ToString(), 40);
                        myWMSPackslip.CustomerUDF4 = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.CustomerUDF4.ToString(), 30);
                        myWMSPackslip.CustomerUDF5 = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.CustomerUDF5.ToString(), 30);
                        myWMSPackslip.OrderSource = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.OrderSource.ToString(), 40);
                        //myWMSPackslip.TotalInvoiceAmountCharge = NAVAdapterUtils.NAVFormatDecimal(myNAVProdOrder.TotalInvoiceAmountCharge.ToString());
                        myWMSPackslip.ShipAddress3 = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.ShipAddress3.ToString(), 35);
                        myWMSPackslip.BillAddress3 = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.BillAddress3.ToString(), 35);
                        myWMSPackslip.ShipperAddress3 = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.ShipperAddress3.ToString(), 35);
                        myWMSPackslip.ShipperCountry = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.ShipperCountry.ToString(), 40);
                        myWMSPackslip.TotalFreightToCharge = NAVAdapterUtils.NAVFormatDecimal(myNAVAssemblyOrder.TotalFreightToCharge.ToString());
                        myWMSPackslip.TotalShippingHandlingCharge = NAVAdapterUtils.NAVFormatDecimal(myNAVAssemblyOrder.TotalShippingHandlingCharge.ToString());
                        myWMSPackslip.PromoAmount = NAVAdapterUtils.NAVFormatDecimal(myNAVAssemblyOrder.PromoAmount.ToString());
                        myWMSPackslip.PromoDiscount = NAVAdapterUtils.NAVFormatDecimal(myNAVAssemblyOrder.PromoDiscount.ToString());
                        myWMSPackslip.EndOfLineProcess = NAVAdapterUtils.NAVFormatInt(myNAVAssemblyOrder.EndOfLineProcess.ToString());
                        myWMSPackslip.PurchaseOrderNumber = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.PurchaseOrderNumber.ToString(), 40);
                        myWMSPackslip.Special1 = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.Special1.ToString(), 40);
                        myWMSPackslip.Special2 = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.Special2.ToString(), 40);
                        myWMSPackslip.Special3 = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.Special3.ToString(), 40);
                        myWMSPackslip.MinimumDaysOfExpiry = NAVAdapterUtils.NAVFormatInt(myNAVAssemblyOrder.MinimumDaysOfExpiry.ToString());
                        myWMSPackslip.MixedLotIndicator = NAVAdapterUtils.NAVFormatBoolean(myNAVAssemblyOrder.MixedLotIndicator.ToString());
                        myWMSPackslip.Extra1 = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.Extra1.ToString(), 40);
                        myWMSPackslip.Extra2 = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.Extra2.ToString(), 40);
                        myWMSPackslip.Extra3 = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.Extra3.ToString(), 40);
                        myWMSPackslip.Extra4 = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.Extra4.ToString(), 40);
                        myWMSPackslip.Extra5 = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.Extra5.ToString(), 40);
                        myWMSPackslip.Handle = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.Handle.ToString(), 40);
                        myWMSPackslip.CustomerCartonContentLabel = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.CustomerCartonContentLabel.ToString(), 40);
                        myWMSPackslip.CartonType = NAVAdapterUtils.NAVFormatCartonType(myNAVAssemblyOrder.CartonType.ToString());
                        myWMSPackslip.WarehouseTransitLocation = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.WarehouseTransitLocation.ToString(), 40);
                        myWMSPackslip.WarehouseDestinationLocation = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.WarehouseDestinationLocation.ToString(), 40);
                        myWMSPackslip.DestinationBinLocation = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.DestinationBin.ToString(), 40);
                        myWMSPackslip.PlannerID = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.PlannerID.ToString(), 40);
                        myWMSPackslip.ControlBuyerID = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.ControlBuyerID.ToString(), 40);
                        myWMSPackslip.ScheduledStartDate = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.ScheduledStartDate.ToString(), 40);
                        myWMSPackslip.FinishedGoodItemNumber = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.FinishedGoodItemNumber.ToString(), 40);
                        myWMSPackslip.FinishedGoodDescription = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.FinishedGoodDescription.ToString(), 40);
                        myWMSPackslip.FinishedGoodClassID = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.FinishedGoodClassID.ToString(), 40);
                        myWMSPackslip.MOENdingQuantity = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.MOEndingQuantity.ToString(), 40);
                        myWMSPackslip.FirstWorkCenter = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.FirstWorkCenter.ToString(), 40);
                        myWMSPackslip.MODescription = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.MODescription.ToString(), 40);
                        myWMSPackslip.PrimaryBillToAddress = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.PrimaryBillToAddress.ToString(), 40);
                        myWMSPackslip.WorkArea = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.WorkArea.ToString(), 40);
                        myWMSPackslip.AssemblyNote1 = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.AssemblyNote1.ToString(), 40);
                        myWMSPackslip.AssemblyNote2 = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.AssemblyNote2.ToString(), 40);
                        myWMSPackslip.AssemblyNote3 = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.AssemblyNote3.ToString(), 40);
                        myWMSPackslip.AssemblyNote4 = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.AssemblyNote4.ToString(), 40);
                        myWMSPackslip.AssemblyNote5 = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.AssemblyNote5.ToString(), 40);
                        myWMSPackslip.ShipperEIN = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.ShipperEmployerIdNumber.ToString(), 40);
                        myWMSPackslip.ShipToEIN = NAVAdapterUtils.NAVFormatString(myNAVAssemblyOrder.ShipToEmployerIdNumber.ToString(), 40);
                        myWMSPackslip.SVAStagingID = myNAVAssemblyOrder.StagingID.ToString();

                        didFailureOccur = false;

                        try
                        {
                            NAVAssemblyOrderLines2 myNAVProductionOrderLines = myNAVAssemblyOrder.NAVAssemblyOrderLines2;

                            foreach (NAVAssemblyOrderLine2 myNAVLine in myNAVProductionOrderLines.NAVAssemblyOrderLine2)
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
                                    NAVAssemblyRecipes2 myNAVRecipes = myNAVLine.NAVAssemblyRecipes2;

                                    try
                                    {
                                        foreach (NAVAssemblyRecipe2 myNAVRecipe in myNAVRecipes.NAVAssemblyRecipe2)
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
                                        EventNotificationClient.Instance.PostMessage("NAV.Download.AssemblyOrders.Error", CompanyConfig.InstanceId,
                                            "Error: Assembly Recipe Level", new Variable("Error Sending to WMS", exceptionProdRec));
                                        didFailureOccur = true;
                                        break; //error occurred, no need to continue loop if we're not adding this
                                    }

                                    //Add Assembly Order Line to Assembly Order
                                    myWMSPackslip.Lines.Add(myWMSPackslipLine);
                                }
                                catch (Exception exceptionProdOrdLineLvl)
                                {
                                    //Error at Assembly Order Line Level
                                    EventNotificationClient.Instance.PostMessage("NAV.Download.AssemblyOrders.Error", CompanyConfig.InstanceId, "Error: Assembly Order Line Level", new Variable("Error retrieving from NAV", exceptionProdOrdLineLvl));
                                    didFailureOccur = true;
                                    break; //error occurred, no need to continue loop if we're not adding this
                                }
                            }
                        }
                        catch (Exception exceptionProdOrdLinesLvl)
                        {
                            //Error at Assembly Order Lines Level
                            EventNotificationClient.Instance.PostMessage("NAV.Download.AssemblyOrders.Error", CompanyConfig.InstanceId, "Error: Assembly Order Lines Level", new Variable("Error retrieving from NAV", exceptionProdOrdLinesLvl));
                            didFailureOccur = true;
                        }

                        //Add Assembly Order (if succeeded) to List of Assembly Orders
                        if (!didFailureOccur)
                        {
                            myWMSProductionOrders.Add(myWMSPackslip);

                            EventNotificationClient.Instance.PostMessage("NAV.Download.AssemblyOrders", CompanyConfig.InstanceId,
                                "A1I -> Status Update -> NAV", new Variable("Status", "Sent to WMS"),
                                new Variable("Assembly Order Number", myNAVAssemblyOrder.OrderNumber),
                                new Variable("Entry No", myNAVAssemblyOrder.StagingID));

                            //try
                            //{
                            //    // Call NAV Web Service | SentStatus
                            //    CompanyConfig.Instance.NavWebServiceInstance.SentStatus(myWMSPackslip.SVAStagingID, "AO");
                            //    CompanyConfig.Instance.NavWebServiceInstance.Dispose();
                            //}
                            //catch (Exception ex1)
                            //{
                            //    EventNotificationClient.Instance.PostMessage("NAV.Download.AssemblyOrders.Error", CompanyConfig.InstanceId, "Error: A1I Entry No -> NAV", new Variable("Error updating Sent Status in NAV", ex1));
                            //}
                        }
                    }
                }
                catch (Exception exceptionPSLvl)
                {
                    //Error at Packslip Level
                    EventNotificationClient.Instance.PostMessage("NAV.Download.AssemblyOrders.Error", CompanyConfig.InstanceId, "Error: Packslip Level", new Variable("Error Parsing NAV data", exceptionPSLvl));
                }
            }
            catch (Exception exceptionProdOrdRootLvl)
            {
                //Error at Assembly Order Root Level
                EventNotificationClient.Instance.PostMessage("NAV.Download.AssemblyOrders.Error", CompanyConfig.InstanceId, "Error: Assembly Order Root Level", new Variable("Error retrieving XML from NAV", exceptionProdOrdRootLvl));
            }

            return myWMSProductionOrders;
        }
        #endregion

        #region SVA RetrievalRequestFromNAV
        public static List<Packslip> RetrievalRequestFromNAV()
        {
            NAVAORetrievalRoot myNAVAORetrievalRoot = new NAVAORetrievalRoot();
            List<Packslip> myWMSPackslips = new List<Packslip>();

            String myResult = String.Empty;
            try
            {
                Packslip myWMSPackslip;

                // Call NAV Web Service | RetrieveProductionOrder
                myResult = CompanyConfig.Instance.NavWebServiceInstance.RetrieveAssemblyOrder(ref myNAVAORetrievalRoot);
                CompanyConfig.Instance.NavWebServiceInstance.Dispose();

                //NAV Business Object | Code = AO
                NAVBusinessObjectAOR myNAVBusinessObjectAOR = new NAVBusinessObjectAOR();
                myNAVBusinessObjectAOR = myNAVAORetrievalRoot.NAVBusinessObjectAOR;

                //NAV Work Orders | Processed = Ready to Send | Header Only
                NAVAORetrievals myNAVAORetrievals = new NAVAORetrievals();
                myNAVAORetrievals = myNAVBusinessObjectAOR.NAVAORetrievals;

                try
                {
                    // For each Pick Header record in the queue
                    foreach (NAVAORetrieval myNAVOrder in myNAVAORetrievals.NAVAORetrieval)
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
                            EventNotificationClient.Instance.PostMessage("NAV.Download.AssemblyOrderRetrieval", 
                                CompanyConfig.InstanceId, "A1I -> Status Update -> NAV", new Variable("Status", "Sent to WMS"), 
                                new Variable("Assembly Order Number", NAVAdapterUtils.NAVFormatString(myNAVOrder.OrderNumber.ToString(), 40)), 
                                new Variable("Entry No", NAVAdapterUtils.NAVFormatString(myNAVOrder.StagingID.ToString(), 40)));

                            try
                            {
                                // Call NAV Web Service | SentStatus
                                CompanyConfig.Instance.NavWebServiceInstance.SentStatus(
                                    NAVAdapterUtils.NAVFormatString(myNAVOrder.StagingID.ToString(), 40), "AO");
                                CompanyConfig.Instance.NavWebServiceInstance.Dispose();
                            }
                            catch (Exception ex1)
                            {
                                EventNotificationClient.Instance.PostMessage("NAV.Download.AssemblyOrderRetrieval.Error", CompanyConfig.InstanceId, 
                                    "Error: A1I Entry No -> NAV", new Variable("Error Sending to WMS", ex1));
                            }
                        }
                    }
                }
                catch (Exception exceptionAOLvl)
                {
                    //Error at Packslip Level
                    EventNotificationClient.Instance.PostMessage("NAV.Download.AssemblyOrderRetrieval.Error", CompanyConfig.InstanceId,
                        "Error: Production Order Level", new Variable("Error Sending to WMS", exceptionAOLvl));
                }
            }
            catch (Exception exceptionAORetLvl)
            {
                //Error at Packslip Retrieval Level
                EventNotificationClient.Instance.PostMessage("NAV.Download.AssemblyOrderRetrieval.Error", CompanyConfig.InstanceId, 
                    "Error: Assembly Order Retrieval Level", new Variable("Error Sending to WMS", exceptionAORetLvl));
            }

            return myWMSPackslips;
        }
        #endregion
    }
}
    
