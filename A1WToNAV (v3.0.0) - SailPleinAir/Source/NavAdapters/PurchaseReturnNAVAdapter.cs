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
    class PurchaseReturnNAVAdapter
    {
        #region SVA RetrieveFromNAV
        public static List<Packslip> RetrieveFromNAV()
        {
            NAVPOReturnRoot myNAVPOReturnRoot = new NAVPOReturnRoot();
            List<Packslip> myWMSPurchaseReturns = new List<Packslip>();
            bool didFailureOccur;

            String myResult = String.Empty;

            try
            {
                Packslip myWMSPackslip;
                PackslipLine myWMSPackslipLine;

                // Call NAV Web Service | ExportPurchaseOrder
                myResult = CompanyConfig.Instance.NavWebServiceInstance.ExportPurchaseReturnOrder(ref myNAVPOReturnRoot);
                CompanyConfig.Instance.NavWebServiceInstance.Dispose();
                EventNotificationClient.Instance.PostMessage("NAV.Download.PurchaseReturnOrders", CompanyConfig.InstanceId, "Exported NAV Data",
                    new Variable("data", Utils.SerializeToString(myNAVPOReturnRoot)));

                //NAV Business Object | Code = PH
                NAVBusinessObjectPOReturn myNAVBusinessObjectPOReturn = new NAVBusinessObjectPOReturn();
                myNAVBusinessObjectPOReturn = myNAVPOReturnRoot.NAVBusinessObjectPOReturn;

                //NAV Pick Headers | Processed = Ready to Send
                NAVPickHeaderReturns myNAVPickHeaderReturns = new NAVPickHeaderReturns();
                myNAVPickHeaderReturns = myNAVBusinessObjectPOReturn.NAVPickHeaderReturns;

                try
                {
                    if (myNAVPickHeaderReturns.NAVPickHeaderReturn == null)
                    {
                        //There are no orders to process
                        myWMSPurchaseReturns = new List<Packslip>();
                        return myWMSPurchaseReturns;
                    }

                    // For each Pick Header record in the queue
                    foreach (NAVPickHeaderReturn myNAVPickHeaderReturn in myNAVPickHeaderReturns.NAVPickHeaderReturn)
                    {
                        NAVPOReturns myNAVPORturns = new NAVPOReturns();
                        myNAVPORturns = myNAVPickHeaderReturn.NAVPOReturns;

                        NAVPOReturn myNAVOrder = new NAVPOReturn();
                        myNAVOrder = myNAVPORturns.NAVPOReturn;

                        myWMSPackslip = new Packslip();

                        // Parse Packslip Header
                        myWMSPackslip.AssemblyNote1 = NAVAdapterUtils.NAVFormatString(myNAVOrder.AssemblyNote1.ToString(), 40);
                        myWMSPackslip.AssemblyNote2 = NAVAdapterUtils.NAVFormatString(myNAVOrder.AssemblyNote2.ToString(), 40);
                        myWMSPackslip.AssemblyNote3 = NAVAdapterUtils.NAVFormatString(myNAVOrder.AssemblyNote3.ToString(), 40);
                        myWMSPackslip.AssemblyNote4 = NAVAdapterUtils.NAVFormatString(myNAVOrder.AssemblyNote4.ToString(), 40);
                        myWMSPackslip.AssemblyNote5 = NAVAdapterUtils.NAVFormatString(myNAVOrder.AssemblyNote5.ToString(), 40);
                        myWMSPackslip.BackOrderNumber = NAVAdapterUtils.NAVFormatString(myNAVOrder.BackOrderNumber.ToString(), 40);
                        myWMSPackslip.BatchControlNumber = NAVAdapterUtils.NAVFormatString(myNAVOrder.BatchControlNumber.ToString(), 40);
                        myWMSPackslip.BillAddress1 = NAVAdapterUtils.NAVFormatString(myNAVOrder.BillAddress1.ToString(), 40);
                        myWMSPackslip.BillAddress2 = NAVAdapterUtils.NAVFormatString(myNAVOrder.BillAddress2.ToString(), 40);
                        myWMSPackslip.BillCity = NAVAdapterUtils.NAVFormatString(myNAVOrder.BillCity.ToString(), 40);
                        myWMSPackslip.BillCountry = NAVAdapterUtils.NAVFormatString(myNAVOrder.BillCountry.ToString(), 40);
                        myWMSPackslip.BillName = NAVAdapterUtils.NAVFormatString(myNAVOrder.BillName.ToString(), 40);
                        myWMSPackslip.BillPostalZip = NAVAdapterUtils.NAVFormatString(myNAVOrder.BillPostalZipCode.ToString(), 40);
                        myWMSPackslip.BillProvinceState = NAVAdapterUtils.NAVFormatString(myNAVOrder.BillProvinceState.ToString(), 40);
                        myWMSPackslip.BusinessTelephoneNumber = NAVAdapterUtils.NAVFormatString(myNAVOrder.BusinessTelephoneNumber.ToString(), 40);
                        myWMSPackslip.CancelDate = NAVAdapterUtils.NAVFormatDateTime(myNAVOrder.CancelDate.ToString());
                        myWMSPackslip.CartonType = NAVAdapterUtils.NAVFormatCartonType(myNAVOrder.CartonType.ToString());
                        myWMSPackslip.ClientMH10Number = NAVAdapterUtils.NAVFormatString(myNAVOrder.ClientMH10Number.ToString(), 40);
                        myWMSPackslip.ClientName = NAVAdapterUtils.getClientName();
                        myWMSPackslip.COD = NAVAdapterUtils.NAVFormatString(myNAVOrder.COD.ToString(), 40);
                        myWMSPackslip.Consolidate = NAVAdapterUtils.NAVFormatBoolean(myNAVOrder.Consolidate.ToString());
                        myWMSPackslip.ControlBuyerID = NAVAdapterUtils.NAVFormatString(myNAVOrder.ControlBuyerID.ToString(), 40);
                        myWMSPackslip.CostCenter = NAVAdapterUtils.NAVFormatString(myNAVOrder.CostCenter.ToString(), 40);
                        myWMSPackslip.CustomerUDF5 = NAVAdapterUtils.NAVFormatString(myNAVOrder.CustomerUDF5.ToString(), 30);
                        myWMSPackslip.CustomerUDF4 = NAVAdapterUtils.NAVFormatString(myNAVOrder.CustomerUDF4.ToString(), 30);
                        myWMSPackslip.CrystalPackingSlip = NAVAdapterUtils.NAVFormatString(myNAVOrder.CrystalPackingSlip.ToString(), 40);
                        myWMSPackslip.CustomerCartonContentLabel = NAVAdapterUtils.NAVFormatString(myNAVOrder.CustomerCartonContentLabel.ToString(), 40);
                        myWMSPackslip.CustomerGroup = NAVAdapterUtils.NAVFormatString(myNAVOrder.CustomerGroup.ToString(), 40);
                        myWMSPackslip.CustomerLabel = NAVAdapterUtils.NAVFormatString(myNAVOrder.CustomerLabel.ToString(), 40);
                        myWMSPackslip.CustomerNumber = NAVAdapterUtils.NAVFormatString(myNAVOrder.CustomerNumber.ToString(), 40);
                        myWMSPackslip.CustomerPONumber = NAVAdapterUtils.NAVFormatString(myNAVOrder.CustomerPONumber.ToString(), 40);
                        myWMSPackslip.DestinationBinLocation = NAVAdapterUtils.NAVFormatString(myNAVOrder.DestinationBin.ToString(), 40);
                        myWMSPackslip.DontShipBeforeDate = NAVAdapterUtils.NAVFormatDateTime(myNAVOrder.DontShipBeforeDate.ToString());
                        myWMSPackslip.DontSplitExpiryDates = NAVAdapterUtils.NAVFormatBoolean(myNAVOrder.DontSplitExpiryDates.ToString());
                        myWMSPackslip.Email = NAVAdapterUtils.NAVFormatString(myNAVOrder.Email.ToString(), 40);
                        myWMSPackslip.EndOfLineProcess = NAVAdapterUtils.NAVFormatInt(myNAVOrder.EndOfLineProcess.ToString());
                        myWMSPackslip.ExcludePickZones = NAVAdapterUtils.NAVFormatString(myNAVOrder.ExcludePickZones.ToString(), 40);
                        myWMSPackslip.Extra1 = NAVAdapterUtils.NAVFormatString(myNAVOrder.Extra1.ToString(), 40);
                        myWMSPackslip.Extra2 = NAVAdapterUtils.NAVFormatString(myNAVOrder.Extra2.ToString(), 40);
                        myWMSPackslip.Extra3 = NAVAdapterUtils.NAVFormatString(myNAVOrder.Extra3.ToString(), 40);
                        myWMSPackslip.Extra4 = NAVAdapterUtils.NAVFormatString(myNAVOrder.Extra4.ToString(), 40);
                        myWMSPackslip.Extra5 = NAVAdapterUtils.NAVFormatString(myNAVOrder.Extra5.ToString(), 40);
                        myWMSPackslip.FinishedGoodClassID = NAVAdapterUtils.NAVFormatString(myNAVOrder.FinishedGoodClassID.ToString(), 40);
                        myWMSPackslip.FinishedGoodDescription = NAVAdapterUtils.NAVFormatString(myNAVOrder.FinishedGoodDescription.ToString(), 100); // Changed to 100 characters for A1W 64 - iCepts 10/6/15
                        myWMSPackslip.FinishedGoodItemNumber = NAVAdapterUtils.NAVFormatString(myNAVOrder.FinishedGoodItemNumber.ToString(), 40);
                        myWMSPackslip.FirstWorkCenter = NAVAdapterUtils.NAVFormatString(myNAVOrder.FirstWorkCenter.ToString(), 40);
                        myWMSPackslip.CustomerUDF2 = NAVAdapterUtils.NAVFormatString(myNAVOrder.CustomerUDF2.ToString(), 30);
                        myWMSPackslip.CustomerUDF3 = NAVAdapterUtils.NAVFormatString(myNAVOrder.CustomerUDF3.ToString(), 30);
                        myWMSPackslip.CustomerUDF1 = NAVAdapterUtils.NAVFormatString(myNAVOrder.CustomerUDF1.ToString(), 30);
                        myWMSPackslip.Handle = NAVAdapterUtils.NAVFormatString(myNAVOrder.Handle.ToString(), 40);
                        myWMSPackslip.IncludePickZones = NAVAdapterUtils.NAVFormatString(myNAVOrder.IncludePickZones.ToString(), 40);
                        myWMSPackslip.IncludeReplenishmentZones = NAVAdapterUtils.NAVFormatString(myNAVOrder.IncludeReplenishmentZones.ToString(), 40);
                        myWMSPackslip.IsCustomerAcceptsBackorders = NAVAdapterUtils.NAVFormatBoolean(myNAVOrder.IsCustomerAcceptsBackorders.ToString());
                        myWMSPackslip.LabelText = NAVAdapterUtils.NAVFormatString(myNAVOrder.LabelText.ToString(), 40);
                        myWMSPackslip.ManifestReportName = NAVAdapterUtils.NAVFormatString(myNAVOrder.ManifestReportName.ToString(), 40);
                        myWMSPackslip.MinimumDaysOfExpiry = NAVAdapterUtils.NAVFormatInt(myNAVOrder.MinimumDaysOfExpiry.ToString());
                        myWMSPackslip.MixedLotIndicator = NAVAdapterUtils.NAVFormatBoolean(myNAVOrder.MixedLotIndicator.ToString());
                        myWMSPackslip.MODescription = NAVAdapterUtils.NAVFormatString(myNAVOrder.MODescription.ToString(), 100); // Changed to 100 characters for A1W 64 - iCepts 10/6/15
                        myWMSPackslip.MOENdingQuantity = NAVAdapterUtils.NAVFormatString(myNAVOrder.MOEndingQuantity.ToString(), 40);
                        myWMSPackslip.OrderDate = NAVAdapterUtils.NAVFormatDateTime(myNAVOrder.OrderDate.ToString());
                        myWMSPackslip.OrderNumber = NAVAdapterUtils.NAVFormatString(myNAVOrder.OrderNumber.ToString(), 40);
                        myWMSPackslip.OrderSource = NAVAdapterUtils.NAVFormatString(myNAVOrder.OrderSource.ToString(), 40);
                        myWMSPackslip.PackingSlip = NAVAdapterUtils.NAVFormatString(myNAVOrder.PackingSlip.ToString(), 40);
                        myWMSPackslip.PlannerID = NAVAdapterUtils.NAVFormatString(myNAVOrder.PlannerID.ToString(), 40);
                        myWMSPackslip.PricingTicket = NAVAdapterUtils.NAVFormatString(myNAVOrder.PricingTicket.ToString(), 40);
                        myWMSPackslip.PrimaryBillToAddress = NAVAdapterUtils.NAVFormatString(myNAVOrder.PrimaryBillToAddress.ToString(), 40);
                        myWMSPackslip.Priority = NAVAdapterUtils.NAVFormatString(myNAVOrder.Priority.ToString(), 40);
                        myWMSPackslip.PromoAmount = NAVAdapterUtils.NAVFormatDecimal(myNAVOrder.PromoAmount.ToString());
                        myWMSPackslip.PromoDiscount = NAVAdapterUtils.NAVFormatDecimal(myNAVOrder.PromoDiscount.ToString());
                        myWMSPackslip.PurchaseOrderNumber = NAVAdapterUtils.NAVFormatString(myNAVOrder.PurchaseOrderNumber.ToString(), 40);
                        myWMSPackslip.PurchaseOrderType = NAVAdapterUtils.NAVFormatPurchaseOrderType(myNAVOrder.PurchaseOrderType.ToString());
                        myWMSPackslip.RecordType = NAVAdapterUtils.NAVFormatString(myNAVOrder.RecordType.ToString(), 40);
                        myWMSPackslip.ReplenishmentGroup = NAVAdapterUtils.NAVFormatString(myNAVOrder.ReplenishmentGroup.ToString(), 40);
                        myWMSPackslip.ScheduledStartDate = NAVAdapterUtils.NAVFormatString(myNAVOrder.ScheduledStartDate.ToString(), 40);
                        myWMSPackslip.ShipAddressName1 = NAVAdapterUtils.NAVFormatString(myNAVOrder.ShipAddress1.ToString(), 40);
                        myWMSPackslip.ShipAddressName2 = NAVAdapterUtils.NAVFormatString(myNAVOrder.ShipAddress2.ToString(), 40);
                        myWMSPackslip.ShipAttnTo = NAVAdapterUtils.NAVFormatString(myNAVOrder.ShipAttnTo.ToString(), 40);
                        myWMSPackslip.ShipCity = NAVAdapterUtils.NAVFormatString(myNAVOrder.ShipCity.ToString(), 40);
                        myWMSPackslip.ShipCompleteOnly = NAVAdapterUtils.NAVFormatBoolean(myNAVOrder.ShipCompleteOnly.ToString());
                        myWMSPackslip.ShipCountry = NAVAdapterUtils.NAVFormatString(myNAVOrder.ShipCountry.ToString(), 40);
                        myWMSPackslip.ShipDateRequired = NAVAdapterUtils.NAVFormatDateTime(myNAVOrder.ShipDateRequired.ToString());
                        myWMSPackslip.ShipmentActions = NAVAdapterUtils.NAVFormatString(myNAVOrder.ShipmentActions.ToString(), 40);
                        myWMSPackslip.ShipmentConsolidation = NAVAdapterUtils.NAVFormatString(myNAVOrder.ShipmentConsolidation.ToString(), 40);
                        myWMSPackslip.ShipmentMessage = NAVAdapterUtils.NAVFormatString(myNAVOrder.ShipmentMessage.ToString(), 40);
                        myWMSPackslip.ShipmentOptions = NAVAdapterUtils.NAVFormatString(myNAVOrder.ShipmentOptions.ToString(), 40);
                        myWMSPackslip.ShipmentPaymentType = NAVAdapterUtils.NAVFormatShipmentPaymentType(myNAVOrder.ShipmentPaymentType.ToString());
                        myWMSPackslip.ShipName = NAVAdapterUtils.NAVFormatString(myNAVOrder.ShipName.ToString(), 40);
                        myWMSPackslip.ShipPayAccountNumber = NAVAdapterUtils.NAVFormatString(myNAVOrder.ShipPayAccountNumber.ToString(), 40);
                        myWMSPackslip.ShipperAddress1 = NAVAdapterUtils.NAVFormatString(myNAVOrder.ShipperAddress1.ToString(), 40);
                        myWMSPackslip.ShipperAddress2 = NAVAdapterUtils.NAVFormatString(myNAVOrder.ShipperAddress2.ToString(), 40);
                        myWMSPackslip.ShipperCity = NAVAdapterUtils.NAVFormatString(myNAVOrder.ShipperCity.ToString(), 40);
                        myWMSPackslip.ShipperEIN = NAVAdapterUtils.NAVFormatString(myNAVOrder.ShipperEmployerIdNumber.ToString(), 40);
                        myWMSPackslip.ShipperName = NAVAdapterUtils.NAVFormatString(myNAVOrder.ShipperName.ToString(), 40);
                        myWMSPackslip.ShipperPostalZipCode = NAVAdapterUtils.NAVFormatString(myNAVOrder.ShipperPostalZipCode.ToString(), 40);
                        myWMSPackslip.ShipperProvinceState = NAVAdapterUtils.NAVFormatString(myNAVOrder.ShipperProvinceState.ToString(), 40);
                        myWMSPackslip.ShipPostalZipCode = NAVAdapterUtils.NAVFormatString(myNAVOrder.ShipPostalZipCode.ToString(), 40);
                        myWMSPackslip.ShipProvinceState = NAVAdapterUtils.NAVFormatString(myNAVOrder.ShipProvinceState.ToString(), 40);
                        myWMSPackslip.ShipTelephoneNumber = NAVAdapterUtils.NAVFormatString(myNAVOrder.ShipTelephoneNumber.ToString(), 40);
                        myWMSPackslip.ShipTimeRequired = NAVAdapterUtils.NAVFormatString(myNAVOrder.ShipTimeRequired.ToString(), 40);
                        myWMSPackslip.ShipToEIN = NAVAdapterUtils.NAVFormatString(myNAVOrder.ShipToEmployerIdNumber.ToString(), 40);
                        myWMSPackslip.ShipToFaxNumber = NAVAdapterUtils.NAVFormatString(myNAVOrder.ShipToFaxNumber.ToString(), 40);
                        myWMSPackslip.ShipToNumber = NAVAdapterUtils.NAVFormatString(myNAVOrder.ShipToNumber.ToString(), 40);
                        myWMSPackslip.ShipViaPreferred = NAVAdapterUtils.NAVFormatString(myNAVOrder.ShipViaPreferred.ToString(), 40);
                        myWMSPackslip.ShipWithOtherGoods = NAVAdapterUtils.NAVFormatBoolean(myNAVOrder.ShipWithOtherGoods.ToString());
                        myWMSPackslip.Special1 = NAVAdapterUtils.NAVFormatString(myNAVOrder.Special1.ToString(), 40);
                        myWMSPackslip.Special2 = NAVAdapterUtils.NAVFormatString(myNAVOrder.Special2.ToString(), 40);
                        myWMSPackslip.Special3 = NAVAdapterUtils.NAVFormatString(myNAVOrder.Special3.ToString(), 40);
                        myWMSPackslip.SpecialInstructions1 = NAVAdapterUtils.NAVFormatString(myNAVOrder.SpecialInstructions1.ToString(), 40);
                        myWMSPackslip.SpecialInstructions2 = NAVAdapterUtils.NAVFormatString(myNAVOrder.SpecialInstructions2.ToString(), 40);
                        myWMSPackslip.SpecialInstructions3 = NAVAdapterUtils.NAVFormatString(myNAVOrder.SpecialInstructions3.ToString(), 40);
                        myWMSPackslip.SpecialInstructions4 = NAVAdapterUtils.NAVFormatString(myNAVOrder.SpecialInstructions4.ToString(), 40);
                        myWMSPackslip.SpecialInstructions5 = NAVAdapterUtils.NAVFormatString(myNAVOrder.SpecialInstructions5.ToString(), 40);
                        myWMSPackslip.SpecialInstructions6 = NAVAdapterUtils.NAVFormatString(myNAVOrder.SpecialInstructions6.ToString(), 40);
                        myWMSPackslip.SpecialInstructions7 = NAVAdapterUtils.NAVFormatString(myNAVOrder.SpecialInstructions7.ToString(), 40);
                        myWMSPackslip.SVAStagingID = myNAVOrder.StagingID.ToString();
                        myWMSPackslip.TotalFreightToCharge = NAVAdapterUtils.NAVFormatDecimal(myNAVOrder.TotalFreightToCharge.ToString());
                        myWMSPackslip.TotalInvoiceAmountCharge = NAVAdapterUtils.NAVFormatDecimal(myNAVOrder.TotalInvoiceAmountCharge.ToString());
                        myWMSPackslip.TotalShippingHandlingCharge = NAVAdapterUtils.NAVFormatDecimal(myNAVOrder.TotalShippingHandlingCharge.ToString());
                        myWMSPackslip.ShipAddress3 = NAVAdapterUtils.NAVFormatString(myNAVOrder.ShipAddress3.ToString(), 35);
                        myWMSPackslip.BillAddress3 = NAVAdapterUtils.NAVFormatString(myNAVOrder.BillAddress3.ToString(), 35);
                        myWMSPackslip.ShipperAddress3 = NAVAdapterUtils.NAVFormatString(myNAVOrder.ShipperAddress3.ToString(), 35);
                        myWMSPackslip.ShipperCountry = NAVAdapterUtils.NAVFormatString(myNAVOrder.ShipperCountry.ToString(), 40);
                        myWMSPackslip.TruckDeliveryTime = NAVAdapterUtils.NAVFormatString(myNAVOrder.TruckDeliveryTime.ToString(), 35);
                        myWMSPackslip.TruckRoute = NAVAdapterUtils.NAVFormatString(myNAVOrder.TruckRoute.ToString(), 40);
                        myWMSPackslip.TruckStop = NAVAdapterUtils.NAVFormatString(myNAVOrder.TruckStop.ToString(), 40);
                        myWMSPackslip.WarehouseDestinationLocation = NAVAdapterUtils.NAVFormatString(myNAVOrder.WarehouseDestinationLocation.ToString(), 40);
                        myWMSPackslip.WarehouseTransitLocation = NAVAdapterUtils.NAVFormatString(myNAVOrder.WarehouseTransitLocation.ToString(), 40);
                        myWMSPackslip.WaveNumber = NAVAdapterUtils.NAVFormatString(myNAVOrder.WaveNumber.ToString(), 40);
                        myWMSPackslip.WorkArea = NAVAdapterUtils.NAVFormatString(myNAVOrder.WorkArea.ToString(), 40);
                        myWMSPackslip.IntegrationUDF1 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF1.ToString(), 40);
                        myWMSPackslip.IntegrationUDF2 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF2.ToString(), 40);
                        myWMSPackslip.IntegrationUDF3 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF3.ToString(), 40);
                        myWMSPackslip.IntegrationUDF4 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF4.ToString(), 40);
                        myWMSPackslip.IntegrationUDF5 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF5.ToString(), 40);
                        myWMSPackslip.IntegrationUDF6 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF6.ToString(), 40);
                        myWMSPackslip.IntegrationUDF7 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF7.ToString(), 40);
                        myWMSPackslip.IntegrationUDF8 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF8.ToString(), 40);
                        myWMSPackslip.IntegrationUDF9 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF9.ToString(), 40);
                        myWMSPackslip.IntegrationUDF10 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF10.ToString(), 40);
                        myWMSPackslip.IntegrationUDF11 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF11.ToString(), 40);
                        myWMSPackslip.IntegrationUDF12 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF12.ToString(), 40);
                        myWMSPackslip.IntegrationUDF13 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF13.ToString(), 40);
                        myWMSPackslip.IntegrationUDF14 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF14.ToString(), 40);
                        myWMSPackslip.IntegrationUDF15 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF15.ToString(), 40);
                        myWMSPackslip.IntegrationUDF16 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF16.ToString(), 40);
                        myWMSPackslip.IntegrationUDF17 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF17.ToString(), 40);
                        myWMSPackslip.IntegrationUDF18 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF18.ToString(), 40);
                        myWMSPackslip.IntegrationUDF19 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF19.ToString(), 40);
                        myWMSPackslip.IntegrationUDF20 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF20.ToString(), 40);
                        myWMSPackslip.IntegrationUDF21 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF21.ToString(), 40);
                        myWMSPackslip.IntegrationUDF22 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF22.ToString(), 40);
                        myWMSPackslip.IntegrationUDF23 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF23.ToString(), 40);
                        myWMSPackslip.IntegrationUDF24 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF24.ToString(), 40);
                        myWMSPackslip.IntegrationUDF25 = NAVAdapterUtils.NAVFormatString(myNAVOrder.IntegrationUDF25.ToString(), 40);

                        didFailureOccur = false;
                        try
                        {
                            NAVPOReturnLines myNAVPOReturnLines = myNAVOrder.NAVPOReturnLines;

                            foreach (NAVPOReturnLine myNAVLine in myNAVPOReturnLines.NAVPOReturnLine)
                            {
                                myWMSPackslipLine = new PackslipLine();

                                try
                                {
                                    //Parse Packslip Attributes
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
                                }
                                catch (Exception ex)
                                {
                                    //Error at Packslip Line Level
                                    EventNotificationClient.Instance.PostMessage("NAV.Download.PurchaseReturnOrders.Error", CompanyConfig.InstanceId, "Error", new Variable("Error parsing purchase return attributes", ex));
                                    didFailureOccur = true;
                                    break; //Error occurred, no need to continue as this Packslip will not be added
                                }

                                // Parse Packslip Lines
                                myWMSPackslipLine.BackflushFlag = NAVAdapterUtils.NAVFormatBoolean(myNAVLine.BackflushFlag.ToString());
                                myWMSPackslipLine.BackOrderNumber = NAVAdapterUtils.NAVFormatString(myNAVLine.LineBackOrderNumber.ToString(), 40);
                                myWMSPackslipLine.CommentIndicator = NAVAdapterUtils.NAVFormatString(myNAVLine.CommentIndicator.ToString(), 40);
                                myWMSPackslipLine.Commitment = NAVAdapterUtils.NAVFormatInt(myNAVLine.Commitment.ToString());
                                myWMSPackslipLine.CommodityCode = NAVAdapterUtils.NAVFormatString(myNAVLine.CommodityCode.ToString(), 40);
                                myWMSPackslipLine.CountryOfOrigin = NAVAdapterUtils.NAVFormatString(myNAVLine.CountryOfOrigin.ToString(), 40);
                                myWMSPackslipLine.Cube = NAVAdapterUtils.NAVFormatDecimal(myNAVLine.Cube.ToString());
                                myWMSPackslipLine.CustomerLineReference = NAVAdapterUtils.NAVFormatString(myNAVLine.CustomerLineReference.ToString(), 40);
                                myWMSPackslipLine.CustomerNumber = NAVAdapterUtils.NAVFormatString(myNAVLine.CustomerNumber.ToString(), 40);
                                myWMSPackslipLine.CustomerPoNumber = NAVAdapterUtils.NAVFormatString(myNAVLine.LineCustomerPoNumber.ToString(), 40);
                                myWMSPackslipLine.CustomerXRef = NAVAdapterUtils.NAVFormatString(myNAVLine.CustomerXRef.ToString(), 40);
                                myWMSPackslipLine.DepartmentNumber = NAVAdapterUtils.NAVFormatString(myNAVLine.DepartmentNumber.ToString(), 40);
                                myWMSPackslipLine.Description = NAVAdapterUtils.NAVFormatString(myNAVLine.Description.ToString(), 100); // Changed to 100 characters for A1W 64 - iCepts 10/6/15
                                myWMSPackslipLine.Discount = NAVAdapterUtils.NAVFormatDecimal(myNAVLine.Discount.ToString());
                                myWMSPackslipLine.EDPNumber = NAVAdapterUtils.NAVFormatString(myNAVLine.EDPNumber.ToString(), 40);
                                myWMSPackslipLine.Extra = NAVAdapterUtils.NAVFormatString(myNAVLine.Extra.ToString(), 40);
                                myWMSPackslipLine.Extra1 = NAVAdapterUtils.NAVFormatString(myNAVLine.Extra1Line.ToString(), 40);
                                myWMSPackslipLine.Extra2 = NAVAdapterUtils.NAVFormatString(myNAVLine.Extra2Line.ToString(), 40);
                                myWMSPackslipLine.Extra3 = NAVAdapterUtils.NAVFormatString(myNAVLine.Extra3Line.ToString(), 40);
                                myWMSPackslipLine.Extra4 = NAVAdapterUtils.NAVFormatString(myNAVLine.Extra4Line.ToString(), 40);
                                myWMSPackslipLine.Extra5 = NAVAdapterUtils.NAVFormatString(myNAVLine.Extra5Line.ToString(), 40);
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
                                myWMSPackslipLine.IntegrationUDF1 = NAVAdapterUtils.NAVFormatString(myNAVLine.IntegrationUDF1.ToString(), 40);
                                myWMSPackslipLine.IntegrationUDF2 = NAVAdapterUtils.NAVFormatString(myNAVLine.IntegrationUDF2.ToString(), 40);
                                myWMSPackslipLine.IntegrationUDF3 = NAVAdapterUtils.NAVFormatString(myNAVLine.IntegrationUDF3.ToString(), 40);
                                myWMSPackslipLine.IntegrationUDF4 = NAVAdapterUtils.NAVFormatString(myNAVLine.IntegrationUDF4.ToString(), 40);
                                myWMSPackslipLine.IntegrationUDF5 = NAVAdapterUtils.NAVFormatString(myNAVLine.IntegrationUDF5.ToString(), 40);
                                myWMSPackslipLine.IntegrationUDF6 = NAVAdapterUtils.NAVFormatString(myNAVLine.IntegrationUDF6.ToString(), 40);
                                myWMSPackslipLine.IntegrationUDF7 = NAVAdapterUtils.NAVFormatString(myNAVLine.IntegrationUDF7.ToString(), 40);
                                myWMSPackslipLine.IntegrationUDF8 = NAVAdapterUtils.NAVFormatString(myNAVLine.IntegrationUDF8.ToString(), 40);
                                myWMSPackslipLine.IntegrationUDF9 = NAVAdapterUtils.NAVFormatString(myNAVLine.IntegrationUDF9.ToString(), 40);
                                myWMSPackslipLine.IntegrationUDF10 = NAVAdapterUtils.NAVFormatString(myNAVLine.IntegrationUDF10.ToString(), 40);
                                myWMSPackslipLine.IntegrationUDF11 = NAVAdapterUtils.NAVFormatString(myNAVLine.IntegrationUDF11.ToString(), 40);
                                myWMSPackslipLine.IntegrationUDF12 = NAVAdapterUtils.NAVFormatString(myNAVLine.IntegrationUDF12.ToString(), 40);
                                myWMSPackslipLine.IntegrationUDF13 = NAVAdapterUtils.NAVFormatString(myNAVLine.IntegrationUDF13.ToString(), 40);
                                myWMSPackslipLine.IntegrationUDF14 = NAVAdapterUtils.NAVFormatString(myNAVLine.IntegrationUDF14.ToString(), 40);
                                myWMSPackslipLine.IntegrationUDF15 = NAVAdapterUtils.NAVFormatString(myNAVLine.IntegrationUDF15.ToString(), 40);
                                myWMSPackslipLine.IntegrationUDF16 = NAVAdapterUtils.NAVFormatString(myNAVLine.IntegrationUDF16.ToString(), 40);
                                myWMSPackslipLine.IntegrationUDF17 = NAVAdapterUtils.NAVFormatString(myNAVLine.IntegrationUDF17.ToString(), 40);
                                myWMSPackslipLine.IntegrationUDF18 = NAVAdapterUtils.NAVFormatString(myNAVLine.IntegrationUDF18.ToString(), 40);
                                myWMSPackslipLine.IntegrationUDF19 = NAVAdapterUtils.NAVFormatString(myNAVLine.IntegrationUDF19.ToString(), 40);
                                myWMSPackslipLine.IntegrationUDF20 = NAVAdapterUtils.NAVFormatString(myNAVLine.IntegrationUDF20.ToString(), 40);
                                myWMSPackslipLine.IntegrationUDF21 = NAVAdapterUtils.NAVFormatString(myNAVLine.IntegrationUDF21.ToString(), 40);
                                myWMSPackslipLine.IntegrationUDF22 = NAVAdapterUtils.NAVFormatString(myNAVLine.IntegrationUDF22.ToString(), 40);
                                myWMSPackslipLine.IntegrationUDF23 = NAVAdapterUtils.NAVFormatString(myNAVLine.IntegrationUDF23.ToString(), 40);
                                myWMSPackslipLine.IntegrationUDF24 = NAVAdapterUtils.NAVFormatString(myNAVLine.IntegrationUDF24.ToString(), 40);
                                myWMSPackslipLine.IntegrationUDF25 = NAVAdapterUtils.NAVFormatString(myNAVLine.IntegrationUDF25.ToString(), 40);

                                myWMSPackslip.Lines.Add(myWMSPackslipLine);
                            }
                        }
                        catch (Exception exceptionPSLineLvl)
                        {
                            //Error at Purchase Return Line Level
                            EventNotificationClient.Instance.PostMessage("NAV.Download.PurchaseReturnOrders.Error", CompanyConfig.InstanceId, "Error", new Variable("Error parsing purchase return line", exceptionPSLineLvl));
                            didFailureOccur = true;
                        }

                        //Add Purchase Return (if successful) to List of Purchase Returns
                        if (!didFailureOccur)
                        {
                            myWMSPurchaseReturns.Add(myWMSPackslip);
                            EventNotificationClient.Instance.PostMessage("NAV.Download.PurchaseReturnOrders", CompanyConfig.InstanceId, "A1I -> Status Update -> NAV", new Variable("Status", "Sent to WMS"), new Variable("Purchase Return Order Number", NAVAdapterUtils.NAVFormatString(myNAVOrder.OrderNumber.ToString(), 40)), new Variable("Entry No", NAVAdapterUtils.NAVFormatString(myNAVOrder.StagingID.ToString(), 40)));
                            //try
                            //{
                            //    // Call NAV Web Service | SentStatus
                            //    CompanyConfig.Instance.NavWebServiceInstance.SentStatus(NAVAdapterUtils.NAVFormatString(myNAVOrder.StagingID.ToString(), 40), "PH");
                            //    CompanyConfig.Instance.NavWebServiceInstance.Dispose();
                            //}
                            //catch (Exception ex1)
                            //{
                            //    EventNotificationClient.Instance.PostMessage("NAV.Download.PurchaseReturnOrders.Error", CompanyConfig.InstanceId, "Error: A1I Entry No -> NAV", new Variable("Error Sending to WMS", ex1));
                            //}
                        }
                    }
                }
                catch (Exception exceptionPSLvl)
                {
                    //Error at Purchase Return Level
                    EventNotificationClient.Instance.PostMessage("NAV.Download.PurchaseReturnOrders.Error", CompanyConfig.InstanceId, "Error: Purchase Return Level", new Variable("Error Sending to WMS", exceptionPSLvl));
                }
            }
            catch (Exception exceptionPSRootLvl)
            {
                //Error at Purchase Return Order Root Level
                EventNotificationClient.Instance.PostMessage("NAV.Download.PurchaseReturnOrders.Error", CompanyConfig.InstanceId, "Error: Purchase Return Root Level", new Variable("Error Sending to WMS", exceptionPSRootLvl));
            }

            return myWMSPurchaseReturns;
        }
        #endregion

        #region SVA RetrievalRequestFromNAV
        public static List<Packslip> RetrievalRequestFromNAV()
        {
            NAVPORetrievalRoot myNAVPORetrievalRoot = new NAVPORetrievalRoot();
            List<Packslip> myWMSPackslips = new List<Packslip>();

            String myResult = String.Empty;
            try
            {
                Packslip myWMSPackslip;

                // Call NAV Web Service | ExportPurchaseReturnOrder
                myResult = CompanyConfig.Instance.NavWebServiceInstance.RetrievePurchaseReturnOrder(ref myNAVPORetrievalRoot);
                CompanyConfig.Instance.NavWebServiceInstance.Dispose();

                //NAV Business Object | Code = PH
                NAVBusinessObjectPOR myNAVBusinessObjectPOR = new NAVBusinessObjectPOR();
                myNAVBusinessObjectPOR = myNAVPORetrievalRoot.NAVBusinessObjectPOR;

                //NAV Pick Headers | Processed = Ready to Send | Header Only
                NAVPOReturnRetrievals myNAVPOReturnRetrievals = new NAVPOReturnRetrievals();
                myNAVPOReturnRetrievals = myNAVBusinessObjectPOR.NAVPOReturnRetrievals;

                try
                {
                    // For each Purchase Return record in the queue
                    foreach (NAVPOReturnRetrieval myNAVOrder in myNAVPOReturnRetrievals.NAVPOReturnRetrieval)
                    {
                        myWMSPackslip = new Packslip();

                        // Parse Purchase Return Header
                        myWMSPackslip.SVAStagingID = myNAVOrder.StagingID.ToString();
                        myWMSPackslip.ClientName = NAVAdapterUtils.getClientName();
                        myWMSPackslip.OrderNumber = NAVAdapterUtils.NAVFormatString(myNAVOrder.OrderNumber.ToString(), 40);

                        //Add Purchase Returns to List of Purchase Returns
                        if (!string.IsNullOrEmpty(myNAVOrder.OrderNumber.ToString()))
                        {
                            myWMSPackslips.Add(myWMSPackslip);
                            EventNotificationClient.Instance.PostMessage("NAV.Download.PurchaseReturnOrderRetrieval", CompanyConfig.InstanceId, 
                                "A1I -> Status Update -> NAV", new Variable("Status", "Sent to WMS"), 
                                new Variable("Purchase Return Order Number", NAVAdapterUtils.NAVFormatString(myNAVOrder.OrderNumber.ToString(), 40)), 
                                new Variable("Entry No", NAVAdapterUtils.NAVFormatString(myNAVOrder.StagingID.ToString(), 40)));

                            try
                            {
                                // Call NAV Web Service | SentStatus
                                CompanyConfig.Instance.NavWebServiceInstance.SentStatus(
                                    NAVAdapterUtils.NAVFormatString(myNAVOrder.StagingID.ToString(), 40), "PH");
                                CompanyConfig.Instance.NavWebServiceInstance.Dispose();
                            }
                            catch (Exception ex1)
                            {
                                EventNotificationClient.Instance.PostMessage("NAV.Download.PurchaseReturnOrderRetrieval.Error", CompanyConfig.InstanceId, 
                                    "Error: A1I Entry No -> NAV", new Variable("Error Sending to WMS", ex1));
                            }
                        }
                    }
                }
                catch (Exception exceptionPSLvl)
                {
                    //Error at Purchase Return Level
                    EventNotificationClient.Instance.PostMessage("NAV.Download.PurchaseReturnOrderRetrieval.Error", CompanyConfig.InstanceId, "Error: Purchase Return Level", new Variable("Error Sending to WMS", exceptionPSLvl));
                }
            }
            catch (Exception exceptionPSRetLvl)
            {
                //Error at Purchase Return Retrieval Level
                EventNotificationClient.Instance.PostMessage("NAV.Download.PurchaseReturnOrderRetrieval.Error", CompanyConfig.InstanceId, "Error: Purchase Return Retrieval Level", new Variable("Error Sending to WMS", exceptionPSRetLvl));
            }

            return myWMSPackslips;
        }
        #endregion
    }
}

