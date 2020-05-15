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
    class TransferOrderNAVAdapter
    {
        #region RetrievePHFromNAV
        public static List<Packslip> RetrievePHFromNAV()
        {
            NAVTransferOrderRoot myNAVTransferRoot = new NAVTransferOrderRoot();
            List<Packslip> myWMSTransfers = new List<Packslip>();
            bool didFailureOccur;

            String myResult = String.Empty;

            try
            {
                // Populate fields of Packslip object
                Packslip myWMSPackslip;
                PackslipLine myWMSPackslipLine;

                myResult = CompanyConfig.Instance.NavWebServiceInstance.ExportTransferOrderPH(ref myNAVTransferRoot);
                CompanyConfig.Instance.NavWebServiceInstance.Dispose();
                EventNotificationClient.Instance.PostMessage("NAV.Download.TransferOrders.PickHeaders", CompanyConfig.InstanceId, "Exported NAV Data",
                    new Variable("data", Utils.SerializeToString(myNAVTransferRoot)));

                //NAV Business Object | Code = TO
                NAVBusinessObjectTO myNAVBusinessObjectTO = new NAVBusinessObjectTO();
                myNAVBusinessObjectTO = myNAVTransferRoot.NAVBusinessObjectTO;

                //NAV Transfer Orders | Processed = Ready to Send
                NAVTransferOrders myNAVTransferOrders = new NAVTransferOrders();
                myNAVTransferOrders = myNAVBusinessObjectTO.NAVTransferOrders;

                try
                {
                    if (myNAVTransferOrders.NAVTransferOrder == null)
                    {
                        //There aren't any orders to process
                        myWMSTransfers = new List<Packslip>();
                        return myWMSTransfers;
                    }

                    //For each Transfer Order in the queue
                    foreach (NAVTransferOrder myNAVTransferOrder in myNAVTransferOrders.NAVTransferOrder)
                    {
                        NAVPackslips2 myNAVPackslips = new NAVPackslips2();
                        myNAVPackslips = myNAVTransferOrder.NAVPackslips2;

                        NAVPackslip2 myNAVOrder = new NAVPackslip2();
                        myNAVOrder = myNAVPackslips.NAVPackslip2;

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
                        myWMSPackslip.ShipAddress3 = NAVAdapterUtils.NAVFormatString(myNAVOrder.ShipAddress3, 35);
                        myWMSPackslip.BillAddress3 = NAVAdapterUtils.NAVFormatString(myNAVOrder.BillAddress3.ToString(), 35);
                        myWMSPackslip.ShipperAddress3 = NAVAdapterUtils.NAVFormatString(myNAVOrder.ShipperAddress3.ToString(), 35);
                        myWMSPackslip.ShipperCountry = NAVAdapterUtils.NAVFormatString(myNAVOrder.ShipperCountry.ToString(), 40);
                        myWMSPackslip.TruckDeliveryTime = NAVAdapterUtils.NAVFormatString(myNAVOrder.TruckDeliveryTime.ToString(), 40);
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
                            NAVPackslipLines2 myNAVPackslipLines = myNAVOrder.NAVPackslipLines2;

                            foreach (NAVPackslipLine2 myNAVLine in myNAVPackslipLines.NAVPackslipLine2)
                            {
                                myWMSPackslipLine = new PackslipLine();

                                NAVWSLPackslipLines2 myNAVWSLPSLines = myNAVLine.NAVWSLPackslipLines2;
                                if (myNAVWSLPSLines.NAVWSLPackslipLine2 != null)
                                {
                                    try
                                    {
                                        foreach (NAVWSLPackslipLine2 myNAVWSLLine in myNAVWSLPSLines.NAVWSLPackslipLine2)
                                        {
                                            try
                                            {
                                                //Parse Packslip Attributes
                                                NAV.BusinessObjects.AttributeSet mySet = new NAV.BusinessObjects.AttributeSet();
                                                mySet.Attribute1 = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSAttribute1.ToString(), 40);
                                                mySet.Attribute2 = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSAttribute2.ToString(), 40);
                                                mySet.Attribute3 = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSAttribute3.ToString(), 40);
                                                mySet.Attribute4 = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSAttribute4.ToString(), 40);
                                                mySet.Attribute5 = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSAttribute5.ToString(), 40);
                                                mySet.Attribute6 = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSAttribute6.ToString(), 40);
                                                mySet.Attribute7 = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSAttribute7.ToString(), 40);
                                                mySet.Attribute8 = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSAttribute8.ToString(), 40);
                                                mySet.Attribute9 = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSAttribute9.ToString(), 40);
                                                mySet.Attribute10 = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSAttribute10.ToString(), 40);

                                                myWMSPackslipLine.Attributes.Add(mySet);
                                            }
                                            catch (Exception ex)
                                            {
                                                //Error at Warehouse Shipment Line Level
                                                EventNotificationClient.Instance.PostMessage("NAV.Download.TransferOrders.PickHeaders.Error", CompanyConfig.InstanceId, "Error", new Variable("Error parsing warehouse shipment line", ex));
                                                didFailureOccur = true;
                                                break; //Error occurred, no need to continue as this Packslip will not be added
                                            }

                                            // Parse Packslip Lines
                                            myWMSPackslipLine.BackflushFlag = NAVAdapterUtils.NAVFormatBoolean(myNAVWSLLine.WSBackflushFlag.ToString());
                                            myWMSPackslipLine.BackOrderNumber = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSLineBackOrderNumber.ToString(), 40);
                                            myWMSPackslipLine.CommentIndicator = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSCommentIndicator.ToString(), 40);
                                            myWMSPackslipLine.Commitment = NAVAdapterUtils.NAVFormatInt(myNAVWSLLine.WSCommitment.ToString());
                                            myWMSPackslipLine.CommodityCode = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSCommodityCode.ToString(), 40);
                                            myWMSPackslipLine.CountryOfOrigin = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSCountryOfOrigin.ToString(), 40);
                                            myWMSPackslipLine.Cube = NAVAdapterUtils.NAVFormatDecimal(myNAVWSLLine.WSCube.ToString());
                                            myWMSPackslipLine.CustomerLineReference = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSCustomerLineReference.ToString(), 40);
                                            myWMSPackslipLine.CustomerNumber = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSCustomerNumber.ToString(), 40);
                                            myWMSPackslipLine.CustomerPoNumber = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSCustomerPoNumber.ToString(), 40);
                                            myWMSPackslipLine.CustomerXRef = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSCustomerXRef.ToString(), 40);
                                            myWMSPackslipLine.DepartmentNumber = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSDepartmentNumber.ToString(), 40);
                                            myWMSPackslipLine.Description = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSDescription.ToString(), 100); // Changed to 100 characters for A1W 64 - iCepts 10/6/15
                                            myWMSPackslipLine.Discount = NAVAdapterUtils.NAVFormatDecimal(myNAVWSLLine.WSDiscount.ToString());
                                            myWMSPackslipLine.EDPNumber = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSEDPNumber.ToString(), 40);
                                            myWMSPackslipLine.Extra = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSExtra.ToString(), 40);
                                            myWMSPackslipLine.Extra1 = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSExtra1.ToString(), 40);
                                            myWMSPackslipLine.Extra2 = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSExtra2.ToString(), 40);
                                            myWMSPackslipLine.Extra3 = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSExtra3.ToString(), 40);
                                            myWMSPackslipLine.Extra4 = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSExtra4.ToString(), 40);
                                            myWMSPackslipLine.Extra5 = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSExtra5.ToString(), 40);
                                            myWMSPackslipLine.HazmatCode = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSHazmatCode.ToString(), 40);
                                            myWMSPackslipLine.InnerPacksize = NAVAdapterUtils.NAVFormatInt(myNAVWSLLine.WSInnerPacksize.ToString());
                                            myWMSPackslipLine.KitType = NAVAdapterUtils.NAVFormatKitType(myNAVWSLLine.WSKitType.ToString());
                                            myWMSPackslipLine.LicensePlate = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSLicensePlate.ToString(), 40);
                                            myWMSPackslipLine.Location = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSLocation.ToString(), 40);
                                            myWMSPackslipLine.MiddlePacksize = NAVAdapterUtils.NAVFormatInt(myNAVWSLLine.WSMiddlePacksize.ToString());
                                            myWMSPackslipLine.MinimumDaysOfExpiry = NAVAdapterUtils.NAVFormatInt(myNAVWSLLine.WSMinimumDaysOfExpiry.ToString());
                                            myWMSPackslipLine.OrderNumber = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSOrderNumber.ToString(), 40);
                                            myWMSPackslipLine.OrderSequenceNumber = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSOrderSequenceNumber.ToString(), 40);
                                            myWMSPackslipLine.PackClass = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSPackClass.ToString(), 40);
                                            myWMSPackslipLine.PickAttributeTracking.Attribute1Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVWSLLine.WSPickAttributeTracking.WSAttribute1Tracking.ToString());
                                            myWMSPackslipLine.PickAttributeTracking.Attribute2Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVWSLLine.WSPickAttributeTracking.WSAttribute2Tracking.ToString());
                                            myWMSPackslipLine.PickAttributeTracking.Attribute3Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVWSLLine.WSPickAttributeTracking.WSAttribute3Tracking.ToString());
                                            myWMSPackslipLine.PickAttributeTracking.Attribute4Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVWSLLine.WSPickAttributeTracking.WSAttribute4Tracking.ToString());
                                            myWMSPackslipLine.PickAttributeTracking.Attribute5Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVWSLLine.WSPickAttributeTracking.WSAttribute5Tracking.ToString());
                                            myWMSPackslipLine.PickAttributeTracking.Attribute6Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVWSLLine.WSPickAttributeTracking.WSAttribute6Tracking.ToString());
                                            myWMSPackslipLine.PickAttributeTracking.Attribute7Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVWSLLine.WSPickAttributeTracking.WSAttribute7Tracking.ToString());
                                            myWMSPackslipLine.PickAttributeTracking.Attribute8Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVWSLLine.WSPickAttributeTracking.WSAttribute8Tracking.ToString());
                                            myWMSPackslipLine.PickAttributeTracking.Attribute9Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVWSLLine.WSPickAttributeTracking.WSAttribute9Tracking.ToString());
                                            myWMSPackslipLine.PickAttributeTracking.Attribute10Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVWSLLine.WSPickAttributeTracking.WSAttribute10Tracking.ToString());
                                            myWMSPackslipLine.PriceTicketDescription = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSPriceTicketDescription.ToString(), 40);
                                            myWMSPackslipLine.PrimaryLocation = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSPrimaryBin.ToString(), 40);
                                            myWMSPackslipLine.ProductClass = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSProductClass.ToString(), 40);
                                            myWMSPackslipLine.ProductCode = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSProductCode.ToString(), 40);
                                            myWMSPackslipLine.PromoNumber = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSPromoNumber.ToString(), 40);
                                            myWMSPackslipLine.PurchaseOrderLineNumber = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSPurchaseOrderLineNumber.ToString(), 40);
                                            myWMSPackslipLine.PurchaseOrderNumber = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSPurchaseOrderNumber.ToString(), 40);
                                            myWMSPackslipLine.QuantityOrdered = NAVAdapterUtils.NAVFormatDecimal(myNAVWSLLine.WSQuantityOrdered.ToString());
                                            myWMSPackslipLine.QuantityToPick = NAVAdapterUtils.NAVFormatDecimal(myNAVWSLLine.WSQuantityToPick.ToString());
                                            myWMSPackslipLine.ReservationType = NAVAdapterUtils.NAVFormatReservationType(myNAVWSLLine.WSReservationType.ToString());
                                            myWMSPackslipLine.RetailPrice = NAVAdapterUtils.NAVFormatDecimal(myNAVWSLLine.WSRetailPrice.ToString());
                                            myWMSPackslipLine.Salesman = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSSalesman.ToString(), 40);
                                            myWMSPackslipLine.SellPrice = NAVAdapterUtils.NAVFormatDecimal(myNAVWSLLine.WSSellPrice.ToString());
                                            myWMSPackslipLine.SpecialInstructions1 = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSSpecialInstructions1.ToString(), 40);
                                            myWMSPackslipLine.SpecialInstructions2 = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSSpecialInstructions2.ToString(), 40);
                                            myWMSPackslipLine.SpecialInstructions3 = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSSpecialInstructions3.ToString(), 40);
                                            myWMSPackslipLine.SpecialInstructions4 = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSSpecialInstructions4.ToString(), 40);
                                            myWMSPackslipLine.SpecialInstructions5 = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSSpecialInstructions5.ToString(), 40);
                                            myWMSPackslipLine.SpecialInstructions6 = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSSpecialInstructions6.ToString(), 40);
                                            myWMSPackslipLine.SpecialInstructions7 = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSSpecialInstructions7.ToString(), 40);
                                            myWMSPackslipLine.StockItem = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSStockItem.ToString(), 40);
                                            myWMSPackslipLine.StoreAddress1 = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSStoreAddress1.ToString(), 40);
                                            myWMSPackslipLine.StoreAddress2 = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSStoreAddress2.ToString(), 40);
                                            myWMSPackslipLine.StoreCity = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSStoreCity.ToString(), 40);
                                            myWMSPackslipLine.StoreCountry = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSStoreCountry.ToString(), 40);
                                            myWMSPackslipLine.StoreName = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSStoreName.ToString(), 40);
                                            myWMSPackslipLine.StoreNumber = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSStoreNumber.ToString(), 40);
                                            myWMSPackslipLine.StorePostalZip = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSStorePostalZipCode.ToString(), 40);
                                            myWMSPackslipLine.StoreProvinceState = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSStoreProvinceState.ToString(), 40);
                                            myWMSPackslipLine.Tax1 = NAVAdapterUtils.NAVFormatBoolean(myNAVWSLLine.WSTax1.ToString());
                                            myWMSPackslipLine.Tax2 = NAVAdapterUtils.NAVFormatBoolean(myNAVWSLLine.WSTax2.ToString());
                                            myWMSPackslipLine.Tax3 = NAVAdapterUtils.NAVFormatBoolean(myNAVWSLLine.WSTax3.ToString());
                                            myWMSPackslipLine.Tax4 = NAVAdapterUtils.NAVFormatBoolean(myNAVWSLLine.WSTax4.ToString());
                                            myWMSPackslipLine.UnitOfMeasureMultiplier = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSUnitOfMeasureMultiplier.ToString(), 40);
                                            myWMSPackslipLine.UnitOfMeasureTextDescription = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSUnitOfMeasureTextDescription.ToString(), 40);
                                            myWMSPackslipLine.UnitPrice = NAVAdapterUtils.NAVFormatDecimal(myNAVWSLLine.WSUnitPrice.ToString());
                                            myWMSPackslipLine.UPC = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSUPC.ToString(), 40);
                                            myWMSPackslipLine.ValuePrice = NAVAdapterUtils.NAVFormatDecimal(myNAVWSLLine.WSValuePrice.ToString());
                                            myWMSPackslipLine.VariableLengthProduct = NAVAdapterUtils.NAVFormatVariableLengthType(myNAVWSLLine.WSVariableLengthProduct.ToString());
                                            myWMSPackslipLine.VendorNumber = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSVendorNumber.ToString(), 40);
                                            myWMSPackslipLine.Warehouse = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSWarehouse.ToString(), 40);
                                            myWMSPackslipLine.Weight = NAVAdapterUtils.NAVFormatDecimal(myNAVWSLLine.WSWeight.ToString());
                                            myWMSPackslipLine.IntegrationUDF1 = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSIntegrationUDF1.ToString(), 40);
                                            myWMSPackslipLine.IntegrationUDF2 = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSIntegrationUDF2.ToString(), 40);
                                            myWMSPackslipLine.IntegrationUDF3 = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSIntegrationUDF3.ToString(), 40);
                                            myWMSPackslipLine.IntegrationUDF4 = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSIntegrationUDF4.ToString(), 40);
                                            myWMSPackslipLine.IntegrationUDF5 = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSIntegrationUDF5.ToString(), 40);
                                            myWMSPackslipLine.IntegrationUDF6 = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSIntegrationUDF6.ToString(), 40);
                                            myWMSPackslipLine.IntegrationUDF7 = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSIntegrationUDF7.ToString(), 40);
                                            myWMSPackslipLine.IntegrationUDF8 = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSIntegrationUDF8.ToString(), 40);
                                            myWMSPackslipLine.IntegrationUDF9 = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSIntegrationUDF9.ToString(), 40);
                                            myWMSPackslipLine.IntegrationUDF10 = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSIntegrationUDF10.ToString(), 40);
                                            myWMSPackslipLine.IntegrationUDF11 = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSIntegrationUDF11.ToString(), 40);
                                            myWMSPackslipLine.IntegrationUDF12 = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSIntegrationUDF12.ToString(), 40);
                                            myWMSPackslipLine.IntegrationUDF13 = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSIntegrationUDF13.ToString(), 40);
                                            myWMSPackslipLine.IntegrationUDF14 = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSIntegrationUDF14.ToString(), 40);
                                            myWMSPackslipLine.IntegrationUDF15 = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSIntegrationUDF15.ToString(), 40);
                                            myWMSPackslipLine.IntegrationUDF16 = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSIntegrationUDF16.ToString(), 40);
                                            myWMSPackslipLine.IntegrationUDF17 = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSIntegrationUDF17.ToString(), 40);
                                            myWMSPackslipLine.IntegrationUDF18 = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSIntegrationUDF18.ToString(), 40);
                                            myWMSPackslipLine.IntegrationUDF19 = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSIntegrationUDF19.ToString(), 40);
                                            myWMSPackslipLine.IntegrationUDF20 = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSIntegrationUDF20.ToString(), 40);
                                            myWMSPackslipLine.IntegrationUDF21 = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSIntegrationUDF21.ToString(), 40);
                                            myWMSPackslipLine.IntegrationUDF22 = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSIntegrationUDF22.ToString(), 40);
                                            myWMSPackslipLine.IntegrationUDF23 = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSIntegrationUDF23.ToString(), 40);
                                            myWMSPackslipLine.IntegrationUDF24 = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSIntegrationUDF24.ToString(), 40);
                                            myWMSPackslipLine.IntegrationUDF25 = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSIntegrationUDF25.ToString(), 40);

                                            myWMSPackslip.Lines.Add(myWMSPackslipLine);
                                        }
                                    }
                                    catch (Exception exceptionWSL)
                                    {
                                        //Error in Warehouse Shipment Line
                                        EventNotificationClient.Instance.PostMessage("NAV.Download.TransferOrders.PickHeaders.Error", CompanyConfig.InstanceId, "Error", new Variable("Error parsing warehouse shipment line", exceptionWSL));
                                        didFailureOccur = true;
                                        break;
                                    }
                                }
                                else
                                {
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
                                        EventNotificationClient.Instance.PostMessage("NAV.Download.TransferOrders.PickHeaders.Error", CompanyConfig.InstanceId, "Error", new Variable("Error parsing packslip attributes", ex));
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
                        }
                        catch (Exception exceptionPSLineLvl)
                        {
                            //Error at Packslip Line Level
                            EventNotificationClient.Instance.PostMessage("NAV.Download.TransferOrders.PickHeaders.Error", CompanyConfig.InstanceId, "Error", new Variable("Error parsing packslip line", exceptionPSLineLvl));
                            didFailureOccur = true;
                        }

                        //Add Packslips (if successful) to List of Transfers
                        if (!didFailureOccur)
                        {
                            myWMSTransfers.Add(myWMSPackslip);
                            EventNotificationClient.Instance.PostMessage("NAV.Download.TransferOrders.PickHeaders", CompanyConfig.InstanceId, "A1I -> Status Update -> NAV", new Variable("Status", "Sent to WMS"), new Variable("Sales Order Number", NAVAdapterUtils.NAVFormatString(myNAVOrder.OrderNumber.ToString(), 40)), new Variable("Entry No", NAVAdapterUtils.NAVFormatString(myNAVOrder.StagingID.ToString(), 40)));
                            try
                            {
                                // Call NAV Web Service | SentStatus
                                CompanyConfig.Instance.NavWebServiceInstance.ReadyToSendTORHStatus(NAVAdapterUtils.NAVFormatString(myNAVOrder.StagingID.ToString(), 40), "TO");
                                CompanyConfig.Instance.NavWebServiceInstance.Dispose();
                            }
                            catch (Exception ex1)
                            {
                                EventNotificationClient.Instance.PostMessage("NAV.Download.TransferOrders.PickHeaders.Error", CompanyConfig.InstanceId, "Error: A1I Entry No -> NAV", new Variable("Error Sending to WMS", ex1));
                            }
                        }
                    }
                }
                catch (Exception exceptionPSLvl)
                {
                    //Error at Packslip Level
                    EventNotificationClient.Instance.PostMessage("NAV.Download.TransferOrders.PickHeaders.Error", CompanyConfig.InstanceId, "Error: Packslip Level", new Variable("Error Sending to WMS", exceptionPSLvl));
                }
            }
            catch (Exception exceptionTORootLvl)
            {
                //Error at Transfer Order Root Level
                EventNotificationClient.Instance.PostMessage("NAV.Download.TransferOrders.PickHeaders.Error", CompanyConfig.InstanceId, "Error: Transfer Order Root Level", new Variable("Error Sending to WMS", exceptionTORootLvl));
            }

            return myWMSTransfers;
        }
        #endregion

        #region RetrieveRHFromNAV
        public static List<PurchaseOrder> RetrieveRHFromNAV()
        {
            NAVTransferOrderRoot2 myNAVTransferRoot = new NAVTransferOrderRoot2();
            List<PurchaseOrder> myWMSTransfers = new List<PurchaseOrder>();
            bool didFailureOccur;

            String myResult = String.Empty;

            try
            {
                // Populate fields of PurchaseOrder object
                PurchaseOrder myWMSPurchaseOrder;
                PurchaseOrderLine myWMSPurchaseOrderLine;


                myResult = CompanyConfig.Instance.NavWebServiceInstance.ExportTransferOrderRH(ref myNAVTransferRoot);
                CompanyConfig.Instance.NavWebServiceInstance.Dispose();
                EventNotificationClient.Instance.PostMessage("NAV.Download.TransferOrders.ReceiptHeaders", CompanyConfig.InstanceId, "Exported NAV Data",
                    new Variable("data", Utils.SerializeToString(myNAVTransferRoot)));


                //NAV Business Object | Code = TO
                NAVBusinessObjectTO2 myNAVBusinessObjectTO = new NAVBusinessObjectTO2();
                myNAVBusinessObjectTO = myNAVTransferRoot.NAVBusinessObjectTO2;

                //NAV Transfer Headers| Processed = Ready to Send
                NAVTransferHeaders2 myNAVTransferHeaders = new NAVTransferHeaders2();
                myNAVTransferHeaders = myNAVBusinessObjectTO.NAVTransferHeaders2;

                try
                {
                    if (myNAVTransferHeaders.NAVTransferHeader2 == null)
                    {
                        //There aren't any orders to process
                        myWMSTransfers = new List<PurchaseOrder>();
                        return myWMSTransfers;
                    }

                    //For each Transfer Order in the queue
                    foreach (NAVTransferHeader2 myNAVTransferHeader in myNAVTransferHeaders.NAVTransferHeader2)
                    {
                        NAVPurchaseOrders2 myNAVPurchaseOrders = new NAVPurchaseOrders2();
                        myNAVPurchaseOrders = myNAVTransferHeader.NAVPurchaseOrders2;

                        NAVPurchaseOrder2 myNAVOrder = new NAVPurchaseOrder2();
                        myNAVOrder = myNAVPurchaseOrders.NAVPurchaseOrder2;

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
                        myWMSPurchaseOrder.RequestPOD = NAVAdapterUtils.NAVFormatString(myNAVOrder.RequestPOD.ToString(), 40);
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
                            NAVPurchaseOrderLines2 myNAVPurchaseOrderLines = myNAVOrder.NAVPurchaseOrderLines2;

                            foreach (NAVPurchaseOrderLine2 myNAVLine in myNAVPurchaseOrderLines.NAVPurchaseOrderLine2)
                            {
                                myWMSPurchaseOrderLine = new PurchaseOrderLine();

                                NAVWRLPurchaseOrderLines2 myNAVWRLPOLines = myNAVLine.NAVWRLPurchaseOrderLines2;
                                if (myNAVWRLPOLines.NAVWRLPurchaseOrderLine2 != null)
                                {
                                    try
                                    {
                                        foreach (NAVWRLPurchaseOrderLine2 myNAVWRLLine in myNAVWRLPOLines.NAVWRLPurchaseOrderLine2)
                                        {
                                            // Parse Purchase Order Lines
                                            NAV.BusinessObjects.AttributeSet mySet = new NAV.BusinessObjects.AttributeSet();
                                            mySet.Attribute1 = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRAttribute1.ToString(), 40);
                                            mySet.Attribute2 = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRAttribute2.ToString(), 40);
                                            mySet.Attribute3 = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRAttribute3.ToString(), 40);
                                            mySet.Attribute4 = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRAttribute4.ToString(), 40);
                                            mySet.Attribute5 = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRAttribute5.ToString(), 40);
                                            mySet.Attribute6 = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRAttribute6.ToString(), 40);
                                            mySet.Attribute7 = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRAttribute7.ToString(), 40);
                                            mySet.Attribute8 = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRAttribute8.ToString(), 40);
                                            mySet.Attribute9 = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRAttribute9.ToString(), 40);
                                            mySet.Attribute10 = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRAttribute10.ToString(), 40);
                                            myWMSPurchaseOrderLine.Attributes.Add(mySet);

                                            myWMSPurchaseOrderLine.ContainerBatchRef = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRContainerBatchRef.ToString(), 40);
                                            myWMSPurchaseOrderLine.ContainerStatus = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRContainerStatus.ToString(), 40);
                                            myWMSPurchaseOrderLine.CostPrice = NAVAdapterUtils.NAVFormatDecimal(myNAVWRLLine.WRCostPrice.ToString());
                                            myWMSPurchaseOrderLine.CountryOfOrigin = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRCountryOfOrigin.ToString(), 40);
                                            myWMSPurchaseOrderLine.CreditNow = NAVAdapterUtils.NAVFormatBoolean(myNAVWRLLine.WRCreditNow.ToString());
                                            myWMSPurchaseOrderLine.CustomerName = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRCustomerName.ToString(), 40);
                                            myWMSPurchaseOrderLine.CustomerNumber = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRCustomerNumber.ToString(), 40);
                                            myWMSPurchaseOrderLine.Description = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRDescription.ToString(), 100); // Changed to 100 characters for A1W 64 - iCepts 10/6/15
                                            myWMSPurchaseOrderLine.Discount = NAVAdapterUtils.NAVFormatDecimal(myNAVWRLLine.WRDiscount.ToString());
                                            myWMSPurchaseOrderLine.ExpectedRequiredDate = NAVAdapterUtils.NAVFormatDateTime(myNAVWRLLine.WRExpectedRequiredDate.ToString());
                                            myWMSPurchaseOrderLine.ExpiryDate = NAVAdapterUtils.NAVFormatDateTime(myNAVWRLLine.WRExpiryDate.ToString());
                                            myWMSPurchaseOrderLine.Extra1 = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRExtra1.ToString(), 40);
                                            myWMSPurchaseOrderLine.Extra2 = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRExtra2.ToString(), 40);
                                            myWMSPurchaseOrderLine.Extra3 = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRExtra3.ToString(), 40);
                                            myWMSPurchaseOrderLine.Extra4 = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRExtra4.ToString(), 40);
                                            myWMSPurchaseOrderLine.Extra5 = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRExtra5.ToString(), 40);
                                            myWMSPurchaseOrderLine.HostLineReference = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRHostLineReference.ToString(), 40);
                                            myWMSPurchaseOrderLine.IsSpecial = NAVAdapterUtils.NAVFormatBoolean(myNAVWRLLine.WRIsSpecial.ToString());
                                            myWMSPurchaseOrderLine.LineNumber = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRLineNumber.ToString(), 40);
                                            myWMSPurchaseOrderLine.Location = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRLocation.ToString(), 40);
                                            myWMSPurchaseOrderLine.OverReceiptPercentage = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WROverReceivePercent.ToString(), 40);
                                            myWMSPurchaseOrderLine.PrimaryLocation = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRPrimaryBin.ToString(), 40);
                                            myWMSPurchaseOrderLine.ProductClass = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRProductClass.ToString(), 40);
                                            myWMSPurchaseOrderLine.ProductCode = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRProductCode.ToString(), 40);
                                            myWMSPurchaseOrderLine.PurchaseOrderNumber = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRPurchaseOrderNumber.ToString(), 40);
                                            myWMSPurchaseOrderLine.QuantityExpected = NAVAdapterUtils.NAVFormatDecimal(myNAVWRLLine.WRQuantityExpected.ToString());
                                            myWMSPurchaseOrderLine.ReceiveAttributeTracking.Attribute1Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVWRLLine.WRAttribute1Tracking.ToString());
                                            myWMSPurchaseOrderLine.ReceiveAttributeTracking.Attribute2Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVWRLLine.WRAttribute2Tracking.ToString());
                                            myWMSPurchaseOrderLine.ReceiveAttributeTracking.Attribute3Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVWRLLine.WRAttribute3Tracking.ToString());
                                            myWMSPurchaseOrderLine.ReceiveAttributeTracking.Attribute4Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVWRLLine.WRAttribute4Tracking.ToString());
                                            myWMSPurchaseOrderLine.ReceiveAttributeTracking.Attribute5Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVWRLLine.WRAttribute5Tracking.ToString());
                                            myWMSPurchaseOrderLine.ReceiveAttributeTracking.Attribute6Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVWRLLine.WRAttribute6Tracking.ToString());
                                            myWMSPurchaseOrderLine.ReceiveAttributeTracking.Attribute7Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVWRLLine.WRAttribute7Tracking.ToString());
                                            myWMSPurchaseOrderLine.ReceiveAttributeTracking.Attribute8Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVWRLLine.WRAttribute8Tracking.ToString());
                                            myWMSPurchaseOrderLine.ReceiveAttributeTracking.Attribute9Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVWRLLine.WRAttribute9Tracking.ToString());
                                            myWMSPurchaseOrderLine.ReceiveAttributeTracking.Attribute10Tracking = NAVAdapterUtils.NAVFormatTrackingType(myNAVWRLLine.WRAttribute10Tracking.ToString());
                                            myWMSPurchaseOrderLine.ReInvoice = NAVAdapterUtils.NAVFormatBoolean(myNAVWRLLine.WRReInvoice.ToString());
                                            myWMSPurchaseOrderLine.Reship = NAVAdapterUtils.NAVFormatInt(myNAVWRLLine.WRReship.ToString());
                                            myWMSPurchaseOrderLine.RMAReason = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRRMAReason.ToString(), 40);
                                            myWMSPurchaseOrderLine.RMARestockCharge = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRRMARestockCharge.ToString(), 40);                               //myWMSPurchaseOrderLine = NAVAdapterUtils.NAVFormatString(myNAVLine
                                            myWMSPurchaseOrderLine.SalesOrderLineNumber = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRSalesOrderLineNumber.ToString(), 40);
                                            myWMSPurchaseOrderLine.SalesOrderNumber = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRSalesOrderNumber.ToString(), 40);
                                            myWMSPurchaseOrderLine.SpecialInstructions1 = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRSpecialInstructions1.ToString(), 40);
                                            myWMSPurchaseOrderLine.SpecialInstructions2 = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRSpecialInstructions2.ToString(), 40);
                                            myWMSPurchaseOrderLine.SpecialInstructions3 = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRSpecialInstructions3.ToString(), 40);
                                            myWMSPurchaseOrderLine.SpecialInstructions4 = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRSpecialInstructions4.ToString(), 40);
                                            myWMSPurchaseOrderLine.SpecialInstructions5 = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRSpecialInstructions5.ToString(), 40);
                                            myWMSPurchaseOrderLine.SpecialInstructions6 = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRSpecialInstructions6.ToString(), 40);
                                            myWMSPurchaseOrderLine.SpecialInstructions7 = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRSpecialInstructions7.ToString(), 40);
                                            myWMSPurchaseOrderLine.StockItemIndicator = NAVAdapterUtils.NAVFormatStockItemIndicatorType(myNAVWRLLine.WRStockItemIndicator.ToString());
                                            myWMSPurchaseOrderLine.UnitOfMeasureMultiplier = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRUnitOfMeasureMultiplier.ToString(), 40);
                                            myWMSPurchaseOrderLine.UnitOfMeasureText = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRUnitOfMeasureText.ToString(), 40);
                                            myWMSPurchaseOrderLine.UnitPrice = NAVAdapterUtils.NAVFormatDecimal(myNAVWRLLine.WRUnitPrice.ToString());
                                            myWMSPurchaseOrderLine.UPC = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRUPC.ToString(), 40);
                                            myWMSPurchaseOrderLine.VendorProductNumber = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRVendorProductNumber.ToString(), 40);
                                            myWMSPurchaseOrderLine.Warehouse = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRWarehouse.ToString(), 40);
                                            myWMSPurchaseOrderLine.IntegrationUDF1 = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRIntegrationUDF1.ToString(), 40);
                                            myWMSPurchaseOrderLine.IntegrationUDF2 = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRIntegrationUDF2.ToString(), 40);
                                            myWMSPurchaseOrderLine.IntegrationUDF3 = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRIntegrationUDF3.ToString(), 40);
                                            myWMSPurchaseOrderLine.IntegrationUDF4 = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRIntegrationUDF4.ToString(), 40);
                                            myWMSPurchaseOrderLine.IntegrationUDF5 = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRIntegrationUDF5.ToString(), 40);
                                            myWMSPurchaseOrderLine.IntegrationUDF6 = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRIntegrationUDF6.ToString(), 40);
                                            myWMSPurchaseOrderLine.IntegrationUDF7 = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRIntegrationUDF7.ToString(), 40);
                                            myWMSPurchaseOrderLine.IntegrationUDF8 = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRIntegrationUDF8.ToString(), 40);
                                            myWMSPurchaseOrderLine.IntegrationUDF9 = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRIntegrationUDF9.ToString(), 40);
                                            myWMSPurchaseOrderLine.IntegrationUDF10 = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRIntegrationUDF10.ToString(), 40);
                                            myWMSPurchaseOrderLine.IntegrationUDF11 = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRIntegrationUDF11.ToString(), 40);
                                            myWMSPurchaseOrderLine.IntegrationUDF12 = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRIntegrationUDF12.ToString(), 40);
                                            myWMSPurchaseOrderLine.IntegrationUDF13 = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRIntegrationUDF13.ToString(), 40);
                                            myWMSPurchaseOrderLine.IntegrationUDF14 = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRIntegrationUDF14.ToString(), 40);
                                            myWMSPurchaseOrderLine.IntegrationUDF15 = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRIntegrationUDF15.ToString(), 40);
                                            myWMSPurchaseOrderLine.IntegrationUDF16 = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRIntegrationUDF16.ToString(), 40);
                                            myWMSPurchaseOrderLine.IntegrationUDF17 = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRIntegrationUDF17.ToString(), 40);
                                            myWMSPurchaseOrderLine.IntegrationUDF18 = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRIntegrationUDF18.ToString(), 40);
                                            myWMSPurchaseOrderLine.IntegrationUDF19 = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRIntegrationUDF19.ToString(), 40);
                                            myWMSPurchaseOrderLine.IntegrationUDF20 = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRIntegrationUDF20.ToString(), 40);
                                            myWMSPurchaseOrderLine.IntegrationUDF21 = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRIntegrationUDF21.ToString(), 40);
                                            myWMSPurchaseOrderLine.IntegrationUDF22 = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRIntegrationUDF22.ToString(), 40);
                                            myWMSPurchaseOrderLine.IntegrationUDF23 = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRIntegrationUDF23.ToString(), 40);
                                            myWMSPurchaseOrderLine.IntegrationUDF24 = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRIntegrationUDF24.ToString(), 40);
                                            myWMSPurchaseOrderLine.IntegrationUDF25 = NAVAdapterUtils.NAVFormatString(myNAVWRLLine.WRIntegrationUDF25.ToString(), 40);

                                            myWMSPurchaseOrder.Lines.Add(myWMSPurchaseOrderLine);
                                        }
                                    }
                                    catch (Exception exceptionWRL)
                                    {
                                        //Error in Warehouse Receipt Line
                                        EventNotificationClient.Instance.PostMessage("NAV.Download.TransferOrders.ReceiptHeaders.Error", CompanyConfig.InstanceId, "Error", new Variable("Error parsing warehouse receipt line", exceptionWRL));
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

                                    myWMSPurchaseOrder.Lines.Add(myWMSPurchaseOrderLine);
                                }
                            }
                        }
                        catch (Exception exceptionOrdLinesLvl)
                        {
                            //Error at Purchase Order Lines Level
                            EventNotificationClient.Instance.PostMessage("NAV.Download.TransferOrders.ReceiptHeaders.Error", CompanyConfig.InstanceId, "Error: Purchase Order Lines Level", new Variable("Error Sending to WMS", exceptionOrdLinesLvl));
                            didFailureOccur = true;
                        }

                        //Add Purchase Order (if succeeded) to List of Purchase Orders
                        if (!didFailureOccur)
                        {
                            myWMSTransfers.Add(myWMSPurchaseOrder);
                            EventNotificationClient.Instance.PostMessage("NAV.Download.TransferOrders.ReceiptHeaders", CompanyConfig.InstanceId, "A1I -> Status Update -> NAV", new Variable("Status", "Sent to WMS"), new Variable("Purchase Order Number", NAVAdapterUtils.NAVFormatString(myNAVOrder.PurchaseOrderNumber.ToString(), 40)), new Variable("Entry No", NAVAdapterUtils.NAVFormatString(myNAVOrder.StagingID.ToString(), 40)));
                            //try
                            //{
                            //    // Call NAV Web Service | SentStatus
                            //    CompanyConfig.Instance.NavWebServiceInstance.SentStatus(NAVAdapterUtils.NAVFormatString(myNAVOrder.StagingID.ToString(), 40), "TO");
                            //    CompanyConfig.Instance.NavWebServiceInstance.Dispose();
                            //}
                            //catch (Exception ex1)
                            //{
                            //    EventNotificationClient.Instance.PostMessage("NAV.Download.TransferOrders.ReceiptHeaders.Error", CompanyConfig.InstanceId, "Error: A1I Entry No -> NAV", new Variable("Error Sending to WMS", ex1));
                            //}
                        }
                    }
                }
                catch (Exception exceptionPSLvl)
                {
                    //Error at Transfer Header Level
                    EventNotificationClient.Instance.PostMessage("NAV.Download.TransferOrders.ReceiptHeaders.Error", CompanyConfig.InstanceId, "Error: Transfer Header Level", new Variable("Error Sending to WMS", exceptionPSLvl));
                }
            }
            catch (Exception exceptionTORootLvl)
            {
                //Error at Transfer Order Root Level
                EventNotificationClient.Instance.PostMessage("NAV.Download.TransferOrders.ReceiptHeaders.Error", CompanyConfig.InstanceId, "Error: Transfer Header Root Level", new Variable("Error Sending to WMS", exceptionTORootLvl));
            }

            return myWMSTransfers;
        }
        #endregion

        #region RetrievalRequestFromNAVPH
        public static List<Packslip> RetrievalRequestFromNAVPH()
        {
            NAVTORetrievalPHRoot myNAVTOPHRetrievalRoot = new NAVTORetrievalPHRoot();
            List<Packslip> myWMSPackslips = new List<Packslip>();

            String myResult = String.Empty;

            try
            {
                Packslip myWMSPackslip;

                // Call NAV Web Service | ExportSalesOrder
                myResult = CompanyConfig.Instance.NavWebServiceInstance.RetrieveTransferOrderPH(ref myNAVTOPHRetrievalRoot);
                CompanyConfig.Instance.NavWebServiceInstance.Dispose();

                //NAV Business Object | Code = PH
                NAVBusinessObjectTOPHR myNAVBusinessObjectTOPHR = myNAVTOPHRetrievalRoot.NAVBusinessObjectTOPHR;

                //NAV Pick Headers | Processed = Ready to Send | Header Only
                NAVTOPHRetrievals myNAVTOPHRetrievals = myNAVBusinessObjectTOPHR.NAVTOPHRetrievals;

                try
                {
                    // For each Pick Header record in the queue
                    foreach (NAVTOPHRetrieval myNAVOrder in myNAVTOPHRetrievals.NAVTOPHRetrieval)
                    {
                        myWMSPackslip = new Packslip();

                        // Parse Packslip Header
                        myWMSPackslip.SVAStagingID = myNAVOrder.StagingID.ToString();
                        myWMSPackslip.RecordType = NAVAdapterUtils.NAVFormatString(myNAVOrder.RecordType.ToString(), 40);
                        myWMSPackslip.ClientName = NAVAdapterUtils.getClientName();
                        myWMSPackslip.OrderNumber = NAVAdapterUtils.NAVFormatString(myNAVOrder.OrderNumber.ToString(), 40);
                        myWMSPackslip.BackOrderNumber = myNAVOrder.BackOrderNumber.ToString();

                        //Add Packslips to List of Packslips
                        if (!string.IsNullOrEmpty(myNAVOrder.OrderNumber.ToString()))
                        {
                            myWMSPackslips.Add(myWMSPackslip);
                            EventNotificationClient.Instance.PostMessage("NAV.Download.TransferOrderRetrieval.Packslips", CompanyConfig.InstanceId, 
                                "A1I -> Status Update -> NAV", new Variable("Status", "Sent to WMS"), 
                                new Variable("Packslip Number", NAVAdapterUtils.NAVFormatString(myNAVOrder.OrderNumber.ToString(), 40)), 
                                new Variable("Entry No", NAVAdapterUtils.NAVFormatString(myNAVOrder.StagingID.ToString(), 40)));
                        }
                    }
                }
                catch (Exception exceptionPSLvl)
                {
                    //Error at Packslip Level
                    EventNotificationClient.Instance.PostMessage("NAV.Download.TransferOrderRetrieval.Packslips.Error", CompanyConfig.InstanceId, "Error: Packslip Level", new Variable("Error Sending to WMS", exceptionPSLvl));
                }
            }
            catch (Exception exceptionPSRetLvl)
            {
                //Error at Packslip Retrieval Level
                EventNotificationClient.Instance.PostMessage("NAV.Download.TransferOrderRetrieval.Packslips.Error", CompanyConfig.InstanceId, "Error: Packslip Retrieval Level", new Variable("Error Sending to WMS", exceptionPSRetLvl));
            }

            return myWMSPackslips;
        }
        #endregion

        #region RetrievalRequestFromNAVRH
        public static List<PurchaseOrder> RetrievalRequestFromNAVRH()
        {
            NAVTORetrievalRHRoot myNAVTORHRetrievalRoot = new NAVTORetrievalRHRoot();
            List<PurchaseOrder> myWMSPurchaseOrders = new List<PurchaseOrder>();

            String myResult = String.Empty;

            try
            {
                PurchaseOrder myWMSPurchaseOrder;

                // Call NAV Web Service | ExportPurchaseOrder
                myResult = CompanyConfig.Instance.NavWebServiceInstance.RetrieveTransferOrderRH(ref myNAVTORHRetrievalRoot);
                CompanyConfig.Instance.NavWebServiceInstance.Dispose();

                //NAV Business Object | Code = RH
                NAVBusinessObjectTORHR myNAVBusinessObjectTORHR = myNAVTORHRetrievalRoot.NAVBusinessObjectTORHR;

                //NAV Receipt Headers | Processed = Ready to Send
                NAVTORHRetrievals myNAVTORHRetrievals = myNAVBusinessObjectTORHR.NAVTORHRetrievals;

                try
                {
                    // For each Receipt Header Retrieval record in the queue
                    foreach (NAVTORHRetrieval myNAVOrder in myNAVTORHRetrievals.NAVTORHRetrieval)
                    {
                        myWMSPurchaseOrder = new PurchaseOrder();

                        // Parse Packslip Header
                        myWMSPurchaseOrder.SVAStagingID = myNAVOrder.StagingID.ToString();
                        myWMSPurchaseOrder.ClientName = NAVAdapterUtils.getClientName();
                        myWMSPurchaseOrder.PurchaseOrderNumber = NAVAdapterUtils.NAVFormatString(myNAVOrder.OrderNumber.ToString(), 40);

                        //Add Packslips to List of Packslips
                        if (myNAVOrder.OrderNumber.ToString() != "")
                        {
                            myWMSPurchaseOrders.Add(myWMSPurchaseOrder);
                            
                            EventNotificationClient.Instance.PostMessage("NAV.Download.TransferOrderRetrieval.PurchaseOrders", CompanyConfig.InstanceId, 
                                "A1I -> Status Update -> NAV", new Variable("Status", "Sent to WMS"), 
                                new Variable("Purchase Order Number", NAVAdapterUtils.NAVFormatString(myNAVOrder.OrderNumber.ToString(), 40)), 
                                new Variable("Entry No", NAVAdapterUtils.NAVFormatString(myNAVOrder.StagingID.ToString(), 40)));

                            try
                            {
                                // Call NAV Web Service | SentStatus
                                CompanyConfig.Instance.NavWebServiceInstance.SentStatus(NAVAdapterUtils.NAVFormatString(myNAVOrder.StagingID.ToString(), 40), "TO");
                                CompanyConfig.Instance.NavWebServiceInstance.Dispose();
                            }
                            catch (Exception ex1)
                            {
                                EventNotificationClient.Instance.PostMessage("NAV.Download.TransferOrderRetrieval.PurchaseOrders.Error", CompanyConfig.InstanceId, "Error: A1I Entry No -> NAV", new Variable("Error Sending to WMS", ex1));
                            }
                        }
                    }
                }
                catch (Exception exceptionPOLvl)
                {
                    //Error at Purchase Order Level
                    EventNotificationClient.Instance.PostMessage("NAV.Download.TransferOrderRetrieval.PurchaseOrders.Error", CompanyConfig.InstanceId, 
                        "Error: Purchase Order Level", new Variable("Error Sending to WMS", exceptionPOLvl));
                }
            }
            catch (Exception exceptionPORetRootLvl)
            {
                //Error at Purchase Order Retrieval Root Level
                EventNotificationClient.Instance.PostMessage("NAV.Download.TransferOrderRetrieval.PurchaseOrders.Error", CompanyConfig.InstanceId, 
                    "Error: Purchase Order Retrieval Root Level", new Variable("Error Sending to WMS", exceptionPORetRootLvl));
            }

            return myWMSPurchaseOrders;
        }
        #endregion
    }
}

