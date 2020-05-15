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
	public class SalesNAVAdapter
    {
        #region SVA RetrieveFromNAV
        public static List<Packslip> RetrieveFromNAV()
        {
            NAVPackslipRoot myNAVPackslipRoot = new NAVPackslipRoot();
            List<Packslip> myWMSPackslips = new List<Packslip>();
            bool didFailureOccur;

            String myResult = String.Empty;

            try
            {
                Packslip myWMSPackslip;
                PackslipLine myWMSPackslipLine;

                // Call NAV Web Service | ExportPurchaseOrder
                myResult = CompanyConfig.Instance.NavWebServiceInstance.ExportPackslip(ref myNAVPackslipRoot);
                CompanyConfig.Instance.NavWebServiceInstance.Dispose();

                EventNotificationClient.Instance.PostMessage("NAV.Download.SalesOrders", CompanyConfig.InstanceId, "Exported NAV Data",
                    new Variable("data", Utils.SerializeToString(myNAVPackslipRoot)));

                //NAV Business Object | Code = PH
                NAVBusinessObjectPH myNAVBusinessObjectPH = new NAVBusinessObjectPH();
                myNAVBusinessObjectPH = myNAVPackslipRoot.NAVBusinessObjectPH;

                //NAV Pick Headers | Processed = Ready to Send
                NAVPickHeaders myNAVPickHeaders = new NAVPickHeaders();
                myNAVPickHeaders = myNAVBusinessObjectPH.NAVPickHeaders;

                try
                {
                    if (myNAVPickHeaders.NAVPickHeader == null)
                    {
                        //There are no orders to process
                        myWMSPackslips = new List<Packslip>();
                        return myWMSPackslips;
                    }

                    // For each Pick Header record in the queue
                    foreach (NAVPickHeader myNAVPickHeader in myNAVPickHeaders.NAVPickHeader)
                    {
                        NAVPackslips myNAVPackslips = new NAVPackslips();
                        myNAVPackslips = myNAVPickHeader.NAVPackslips;

                        NAVPackslip myNAVOrder = new NAVPackslip();
                        myNAVOrder = myNAVPackslips.NAVPackslip;

                        if (myNAVOrder != null)
                        {
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
                           myWMSPackslip.ControlBuyerID = NAVAdapterUtils.NAVFormatString(myNAVOrder.ControlBuyerID.ToString(), 40);
                           myWMSPackslip.CostCenter = NAVAdapterUtils.NAVFormatString(myNAVOrder.CostCenter.ToString(), 40);
                           myWMSPackslip.CustomerUDF5 = NAVAdapterUtils.NAVFormatString(myNAVOrder.CustomerUDF5.ToString(), 40);
                           myWMSPackslip.CustomerUDF4 = NAVAdapterUtils.NAVFormatString(myNAVOrder.CustomerUDF4.ToString(), 40);
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
                           //myWMSPackslip.ShipAddressName1 = NAVAdapterUtils.NAVFormatString(myNAVOrder.ShipAddress1.ToString(), 40);Createch--20191211 NY Split address1 in add1 & add2
                           //myWMSPackslip.ShipAddressName2 = NAVAdapterUtils.NAVFormatString(myNAVOrder.ShipAddress2.ToString(), 40);Createch--20191211 NY Split address1 in add1 & add2
						   myWMSPackslip.ShipAddressName1 = NAVAdapterUtils.CreatechNAVFormatSubString(myNAVOrder.ShipAddress1.ToString(), 35, 0);//Createch++20191211 NY Split address1 in add1 & add2
                           myWMSPackslip.ShipAddressName2 = NAVAdapterUtils.CreatechNAVFormatSubString(myNAVOrder.ShipAddress1.ToString(), 35, 35);//Createch++20191211 NY Split address1 in add1 & add2
						   myWMSPackslip.ShipAddressName2 = NAVAdapterUtils.CreatechNAVFormatSubString((myWMSPackslip.ShipAddressName2+" "+myNAVOrder.ShipAddress2.ToString()).Trim(), 35, 0);//Createch++20191211 NY Split address1 in add1 & add2                           
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
                           myWMSPackslip.Shipper = NAVAdapterUtils.NAVFormatString(myNAVOrder.Shipper.ToString(), 10);
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
                               NAVPackslipLines myNAVPackslipLines = myNAVOrder.NAVPackslipLines;

                               foreach (NAVPackslipLine myNAVLine in myNAVPackslipLines.NAVPackslipLine)
                               {
                                   myWMSPackslipLine = new PackslipLine();

                                   NAVWSLPackslipLines myNAVWSLPSLines = myNAVLine.NAVWSLPackslipLines;
                                   if (myNAVWSLPSLines.NAVWSLPackslipLine != null)
                                   {
                                       try
                                       {
                                           foreach (NAVWSLPackslipLine myNAVWSLLine in myNAVWSLPSLines.NAVWSLPackslipLine)
                                           {
                                               myWMSPackslipLine = new PackslipLine();

                                               //Parse Warehouse Shipment Line Packslip Attributes
                                               NAV.BusinessObjects.AttributeSet tempSet = new Accellos.Interfaces.NAV.BusinessObjects.AttributeSet();
                                               tempSet.Attribute1 = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSAttribute1.ToString(), 40);
                                               tempSet.Attribute2 = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSAttribute2.ToString(), 40);
                                               tempSet.Attribute3 = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSAttribute3.ToString(), 40);
                                               tempSet.Attribute4 = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSAttribute4.ToString(), 40);
                                               tempSet.Attribute5 = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSAttribute5.ToString(), 40);
                                               tempSet.Attribute6 = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSAttribute6.ToString(), 40);
                                               tempSet.Attribute7 = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSAttribute7.ToString(), 40);
                                               tempSet.Attribute8 = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSAttribute8.ToString(), 40);
                                               tempSet.Attribute9 = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSAttribute9.ToString(), 40);
                                               tempSet.Attribute10 = NAVAdapterUtils.NAVFormatString(myNAVWSLLine.WSAttribute10.ToString(), 40);
                                               myWMSPackslipLine.Attributes.Add(tempSet);

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
                                           EventNotificationClient.Instance.PostMessage("NAV.Download.SalesOrders.Error", CompanyConfig.InstanceId, "Error", new Variable("Error parsing warehouse shipment line", exceptionWSL));
                                           didFailureOccur = true;
                                           break;
                                       }
                                   }
                                   else
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
                                        
                                       // Parse Packslip Lines
                                       myWMSPackslipLine.BackflushFlag = NAVAdapterUtils.NAVFormatBoolean(myNAVLine.BackflushFlag.ToString());
                                       myWMSPackslipLine.BackOrderNumber = NAVAdapterUtils.NAVFormatString(myNAVLine.LineBackOrderNumber.ToString(), 40);
                                       myWMSPackslipLine.CommentIndicator = NAVAdapterUtils.NAVFormatString(myNAVLine.CommentIndicator.ToString(), 40);
                                       myWMSPackslipLine.Commitment = NAVAdapterUtils.NAVFormatInt(myNAVLine.Commitment.ToString());
                                       myWMSPackslipLine.CommodityCode = NAVAdapterUtils.NAVFormatString(myNAVLine.CommodityCode.ToString(), 40);
                                       myWMSPackslipLine.CommodityDescription = NAVAdapterUtils.NAVFormatString(myNAVLine.CommodityDescription.ToString(), 40);
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
                                       myWMSPackslipLine.NMFCNumber =  NAVAdapterUtils.NAVFormatString(myNAVLine.NMFCNumber.ToString(), 40);
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
                                       myWMSPackslipLine.SpecialInstructions7 = NAVAdapterUtils.NAVFormatString(myNAVLine.SpecialInstructions7.ToString(), 35);
                                       myWMSPackslipLine.StockItem = NAVAdapterUtils.NAVFormatString(myNAVLine.StockItem.ToString(), 40);
                                       myWMSPackslipLine.StoreAddress1 = NAVAdapterUtils.NAVFormatString(myNAVLine.StoreAddress1.ToString(), 40);
                                       myWMSPackslipLine.StoreAddress2 = NAVAdapterUtils.NAVFormatString(myNAVLine.StoreAddress2.ToString(), 40);
                                       myWMSPackslipLine.StoreAddress3 = NAVAdapterUtils.NAVFormatString(myNAVLine.StoreAddress3.ToString(), 40);
                                       myWMSPackslipLine.StoreCity = NAVAdapterUtils.NAVFormatString(myNAVLine.StoreCity.ToString(), 40);
                                       myWMSPackslipLine.StoreCountry = NAVAdapterUtils.NAVFormatString(myNAVLine.StoreCountry.ToString(), 40);
                                       myWMSPackslipLine.StoreName = NAVAdapterUtils.NAVFormatString(myNAVLine.StoreName.ToString(), 40);
                                       myWMSPackslipLine.StoreNumber = NAVAdapterUtils.NAVFormatString(myNAVLine.StoreNumber.ToString(), 40);
                                       myWMSPackslipLine.StorePostalZip = NAVAdapterUtils.NAVFormatString(myNAVLine.StorePostalZipCode.ToString(), 40);
                                       myWMSPackslipLine.StoreProvinceState = NAVAdapterUtils.NAVFormatString(myNAVLine.StoreProvinceState.ToString(), 40);
                                       myWMSPackslipLine.TariffClass = NAVAdapterUtils.NAVFormatString(myNAVLine.TariffClass.ToString(), 40);
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

                                   NAVAssemblyLinks navAssemblyLinks = myNAVLine.NAVAssemblyLinks;

                                   try
                                   {
                                       if (navAssemblyLinks.NAVAssemblyLink != null)
                                       {
                                           foreach (NAVAssemblyLink navAssemblyLink in navAssemblyLinks.NAVAssemblyLink)
                                           {
                                               NAVAssemblyLines myNAVRecipes = navAssemblyLink.NAVAssemblyLines;

                                               try
                                               {
                                                   if (myNAVRecipes.NAVAssemblyLine != null)
                                                   {
                                                       foreach (NAVAssemblyLine myNAVRecipe in myNAVRecipes.NAVAssemblyLine)
                                                       {
                                                           ProductionRecipe myWMSProductionRecipe = new ProductionRecipe();

                                                           // Parse Sales Kit (assembly) Order Recipe
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

                                                           //Add Kit Recipe to Sales Order Line
                                                           myWMSPackslipLine.ProductionRecipes.Add(myWMSProductionRecipe);
                                                       }
                                                   }
                                               }
                                               catch (Exception exKitRec)
                                               {
                                                   //Error in Production Receipe
                                                   EventNotificationClient.Instance.PostMessage("NAV.Download.SalesOrders.Error", CompanyConfig.InstanceId,
                                                       "Error: Kit Receipe Level", new Variable("Error Sending to WMS", exKitRec));
                                                   didFailureOccur = true;
                                                   break; //error occurred, no need to continue loop if we're not adding this
                                               }
                                           }
                                       }
                                   }
                                   catch (Exception exAssemblyLink)
                                   {
                                       //Error in Kit Assembly link
                                       EventNotificationClient.Instance.PostMessage("NAV.Download.SalesOrders.Error", CompanyConfig.InstanceId,
                                           "Error: Kit Assembly Link Level", new Variable("Error Sending to WMS", exAssemblyLink));
                                       didFailureOccur = true;
                                       break; //error occurred, no need to continue loop if we're not adding this
                                   }
                               }
                           }
                           catch (Exception exceptionPSLineLvl)
                           {
                               //Error at Packslip Line Level
                               EventNotificationClient.Instance.PostMessage("NAV.Download.SalesOrders.Error", CompanyConfig.InstanceId, "Error", new Variable("Error parsing packslip line", exceptionPSLineLvl));
                               didFailureOccur = true;
                           }

                           //Add Packslips (if successful) to List of Packslips
                           if (!didFailureOccur)
                           {
                               myWMSPackslips.Add(myWMSPackslip);
                               EventNotificationClient.Instance.PostMessage("NAV.Download.SalesOrders", CompanyConfig.InstanceId, "A1I -> Status Update -> NAV", new Variable("Status", "Sent to WMS"), new Variable("Sales Order Number", NAVAdapterUtils.NAVFormatString(myNAVOrder.OrderNumber.ToString(), 40)), new Variable("Entry No", NAVAdapterUtils.NAVFormatString(myNAVOrder.StagingID.ToString(), 40)));
                               //try
                               //{
                               //    // Call NAV Web Service | SentStatus
                               //    CompanyConfig.Instance.NavWebServiceInstance.SentStatus(NAVAdapterUtils.NAVFormatString(myNAVOrder.StagingID.ToString(), 40), "PH");
                               //    CompanyConfig.Instance.NavWebServiceInstance.Dispose();
                               //}
                               //catch (Exception ex1)
                               //{
                               //    EventNotificationClient.Instance.PostMessage("NAV.Download.SalesOrders.Error", CompanyConfig.InstanceId, "Error: A1I Entry No -> NAV", new Variable("Error Sending to WMS", ex1));
                               //}
                           }
                       } // if myOrder != null
                     } // for each Navpickheader
                }
                catch (Exception exceptionPSLvl)
                {
                    //Error at Packslip Level
                    EventNotificationClient.Instance.PostMessage("NAV.Download.SalesOrders.Error", CompanyConfig.InstanceId, "Error: Packslip Level", new Variable("Error Sending to WMS", exceptionPSLvl));
                }
            }
            catch (Exception exceptionPSRootLvl)
            {
                //Error at Sales Order Root Level
                EventNotificationClient.Instance.PostMessage("NAV.Download.SalesOrders.Error", CompanyConfig.InstanceId, "Error: Packslip Root Level", new Variable("Error Sending to WMS", exceptionPSRootLvl));
            }

            return myWMSPackslips;
        }
        #endregion

        #region SVA WriteToNAV
        public static List<UploadConfirmation> WriteToNAV(List<PickConfirmation> pickConfirmations)
        {
            //EventNotificationClient.Instance.PostMessage("NAV.Upload.SalesOrder.WriteToNAV", CompanyConfig.InstanceId, 
            //    "Entering Function", new Variable("pickConfirmation",pickConfirmation.OrderId));

            List<UploadConfirmation> uploadConfirmations = new List<UploadConfirmation>();
            UploadConfirmation myUploadConfirmation  = new UploadConfirmation();

            try
            {
                NAVPickConfirmationRoot myNAVPickConfirmationRoot = new NAVPickConfirmationRoot();
                List<NAVPickConfirmation> myNAVPickConfirmations = new List<NAVPickConfirmation>();
                NAVPickConfirmation myNAVPickConfirmation;
                String myRowID = String.Empty;
                String myResult = String.Empty;
                
                try
                {

                 foreach (PickConfirmation pickConfirmation in pickConfirmations)
                  { 
                    myUploadConfirmation = new UploadConfirmation();
                        //Start with new NAV Pick Confirmation
                        myNAVPickConfirmation = new NAVPickConfirmation();

                        EventNotificationClient.Instance.PostMessage("NAV.Upload.SalesOrder.WriteToNAV", CompanyConfig.InstanceId, "Parsing Next pickConfirmation");

                        //NAV XMLPort Note:  Items must be assigned in the Column Order that is defined in the XMLPort object
                        // Parse Pick Confirmation Header Record
                        myNAVPickConfirmation.RecordType = "PC";
                        myNAVPickConfirmation.UploadType = "PC";
                        myNAVPickConfirmation.OrderNumber = pickConfirmation.OrderId.ToString();
                        myNAVPickConfirmation.BackOrderNum = pickConfirmation.BackOrderNum.ToString();
                        myNAVPickConfirmation.CustomerNumber = pickConfirmation.CustomerId.ToString();
                        myNAVPickConfirmation.StoreNum = pickConfirmation.StoreNum.ToString();
                        myNAVPickConfirmation.HostOrder = pickConfirmation.HostOrder.ToString();
                        myNAVPickConfirmation.SpecialsSelected = pickConfirmation.SpecialsSelected.ToString();
                        myNAVPickConfirmation.CostCenter = pickConfirmation.CostCenter.ToString();
                        myNAVPickConfirmation.ShipToNum = pickConfirmation.ShipToNum.ToString();
                        myNAVPickConfirmation.ClientName = pickConfirmation.ClientName.ToString();
                        myNAVPickConfirmation.ShippedName = pickConfirmation.ShippedName.ToString();
                        myNAVPickConfirmation.ShippedAddress1 = pickConfirmation.ShippedAddress1.ToString();
                        myNAVPickConfirmation.ShippedAddress2 = pickConfirmation.ShippedAddress2.ToString();
                        myNAVPickConfirmation.ShippedAddress3 = pickConfirmation.ShippedAddress3.ToString();
                        myNAVPickConfirmation.ShippedCity = pickConfirmation.ShippedCity.ToString();
                        myNAVPickConfirmation.ShippedProvinceState = pickConfirmation.ShippedProvinceState.ToString();
                        myNAVPickConfirmation.ShippedPostalZipCode = pickConfirmation.ShippedPostalZip.ToString();
                        myNAVPickConfirmation.ShippedCountry = pickConfirmation.ShippedCountry.ToString();
                        myNAVPickConfirmation.ShippedAttnTo = pickConfirmation.ShippedAttnTo.ToString();
                        myNAVPickConfirmation.ShippedPhone = pickConfirmation.ShippedPhone.ToString();
                        myNAVPickConfirmation.CustomerPoNumber = pickConfirmation.CustomerPoNumber.ToString();
                        myNAVPickConfirmation.Extra1 = pickConfirmation.Extra1.ToString();
                        myNAVPickConfirmation.Extra2 = pickConfirmation.Extra2.ToString();
                        myNAVPickConfirmation.Extra3 = pickConfirmation.Extra3.ToString();
                        myNAVPickConfirmation.Extra4 = pickConfirmation.Extra4.ToString();
                        myNAVPickConfirmation.Extra5 = pickConfirmation.Extra5.ToString();
                        myNAVPickConfirmation.Special1 = pickConfirmation.Special1.ToString();
                        myNAVPickConfirmation.Special2 = pickConfirmation.Special2.ToString();
                        myNAVPickConfirmation.Special3 = pickConfirmation.Special3.ToString();
                        myNAVPickConfirmation.POType = pickConfirmation.POType.ToString();
                        myNAVPickConfirmation.InvoicePrinter = pickConfirmation.InvoicePrinter.ToString();
                        myNAVPickConfirmation.BatchControlNum = pickConfirmation.BatchControlNum.ToString();
                        myNAVPickConfirmation.OrderDate = pickConfirmation.OrderDate.ToString();
                        myNAVPickConfirmation.RequiredDate = pickConfirmation.RequiredDate.ToString();
                        myNAVPickConfirmation.MasterBOLNumber = pickConfirmation.MasterBOLNumber.ToString();
                        myNAVPickConfirmation.ShipmentConsolidationNumber = pickConfirmation.ShipmentConsolidationNumber.ToString();

                        myNAVPickConfirmation.QtyPackedForPackslip = "";
                        myNAVPickConfirmation.IsComplete = "";
                        myNAVPickConfirmation.Unused = "";
                        myNAVPickConfirmation.Extra = "";
                        myNAVPickConfirmation.PIN = "";
                        myNAVPickConfirmation.ZoneShippedTo = "";
                        myNAVPickConfirmation.ShipmentTiebreaker = "";
                        myNAVPickConfirmation.Nuked = "";
                        myNAVPickConfirmation.AllocatedShipmentCost = "";
                        myNAVPickConfirmation.FIFODate = "";
                        myNAVPickConfirmation.TotAmtInvoiced = "";
                        myNAVPickConfirmation.TaxAmt1 = "";
                        myNAVPickConfirmation.TaxAmt2 = ""; 
                        myNAVPickConfirmation.TaxAmt3 = ""; 
                        myNAVPickConfirmation.TaxAmt4 = "";
                        myNAVPickConfirmation.GiftCertNumber = "";
                        myNAVPickConfirmation.GiftCertAmt = "";
                        myNAVPickConfirmation.TotQtyForItem = "";
                        myNAVPickConfirmation.TotQtyForLineAndAttribute = "";
                        myNAVPickConfirmation.A1WPackslipNumber = "";

                        foreach (Shipment shipment in pickConfirmation.Shipments)
                        {
                            //EventNotificationClient.Instance.PostMessage("NAV.Upload.SalesOrder.WriteToNAV", CompanyConfig.InstanceId, "Parsing Shipments", new Variable("Shipment ID",shipment.ShipmentId));

                            //Parse Shipment Record
                            myNAVPickConfirmation.ShipmentNumber = shipment.ShipmentId.ToString();
                            myNAVPickConfirmation.ShipmentCarrier = shipment.ShipmentCarrier.ToString();
                            myNAVPickConfirmation.PickupNumber = shipment.PickupId.ToString();
                            myNAVPickConfirmation.ShipmentServiceUsed = shipment.ShipmentServiceUsed.ToString();
                            myNAVPickConfirmation.DestinationWarehouse = shipment.DestinationWarehouse.ToString();
                            myNAVPickConfirmation.ShipPayAccount = shipment.ShipPayAccount.ToString();
                            myNAVPickConfirmation.ShipPaymentType = shipment.ShipPaymentType.ToString();
                            myNAVPickConfirmation.ShippingUDF1 = shipment.ShippingUDF1.ToString();
                            myNAVPickConfirmation.ShippingUDF2 = shipment.ShippingUDF2.ToString();
                            myNAVPickConfirmation.TrailerNum = shipment.TrailerNum.ToString();
                            myNAVPickConfirmation.TruckRoute = shipment.TruckRoute.ToString();
                            myNAVPickConfirmation.TruckStop = shipment.TruckStop.ToString();
                            myNAVPickConfirmation.TotalWeight = shipment.TotalWeight.ToString();
                            myNAVPickConfirmation.TotalShipmentCharge = shipment.TotalShipmentCharge.ToString();
                            myNAVPickConfirmation.ShipRateCharged = shipment.ShipRateCharged.ToString();
                            myNAVPickConfirmation.DateShipped = shipment.DateShipped.ToString();
                            myNAVPickConfirmation.NumberOfPiecesShipped = shipment.NumberOfPiecesShipped.ToString();
                            myNAVPickConfirmation.SOHeaderUDF1 = shipment.WMSUDF1;
                            myNAVPickConfirmation.SOHeaderUDF2 = shipment.WMSUDF2;
                            myNAVPickConfirmation.SOHeaderUDF3 = shipment.WMSUDF3;
                            myNAVPickConfirmation.SOHeaderUDF4 = shipment.WMSUDF4;
                            myNAVPickConfirmation.IntegrationSOHeaderUDF1 = shipment.IntegrationUDF1;
                            myNAVPickConfirmation.IntegrationSOHeaderUDF2 = shipment.IntegrationUDF2;
                            myNAVPickConfirmation.IntegrationSOHeaderUDF3 = shipment.IntegrationUDF3;
                            myNAVPickConfirmation.IntegrationSOHeaderUDF4 = shipment.IntegrationUDF4;
                            myNAVPickConfirmation.IntegrationSOHeaderUDF5 = shipment.IntegrationUDF5;
                            myNAVPickConfirmation.IntegrationSOHeaderUDF6 = shipment.IntegrationUDF6;
                            myNAVPickConfirmation.IntegrationSOHeaderUDF7 = shipment.IntegrationUDF7;
                            myNAVPickConfirmation.IntegrationSOHeaderUDF8 = shipment.IntegrationUDF8;
                            myNAVPickConfirmation.IntegrationSOHeaderUDF9 = shipment.IntegrationUDF9;
                            myNAVPickConfirmation.IntegrationSOHeaderUDF10 = shipment.IntegrationUDF10;
                            myNAVPickConfirmation.IntegrationSOHeaderUDF11 = shipment.IntegrationUDF11;
                            myNAVPickConfirmation.IntegrationSOHeaderUDF12 = shipment.IntegrationUDF12;
                            myNAVPickConfirmation.IntegrationSOHeaderUDF13 = shipment.IntegrationUDF13;
                            myNAVPickConfirmation.IntegrationSOHeaderUDF14 = shipment.IntegrationUDF14;
                            myNAVPickConfirmation.IntegrationSOHeaderUDF15 = shipment.IntegrationUDF15;
                            myNAVPickConfirmation.IntegrationSOHeaderUDF16 = shipment.IntegrationUDF16;
                            myNAVPickConfirmation.IntegrationSOHeaderUDF17 = shipment.IntegrationUDF17;
                            myNAVPickConfirmation.IntegrationSOHeaderUDF18 = shipment.IntegrationUDF18;
                            myNAVPickConfirmation.IntegrationSOHeaderUDF19= shipment.IntegrationUDF19;
                            myNAVPickConfirmation.IntegrationSOHeaderUDF20 = shipment.IntegrationUDF20;
                            myNAVPickConfirmation.IntegrationSOHeaderUDF21 = shipment.IntegrationUDF21;
                            myNAVPickConfirmation.IntegrationSOHeaderUDF22 = shipment.IntegrationUDF22;
                            myNAVPickConfirmation.IntegrationSOHeaderUDF23 = shipment.IntegrationUDF23;
                            myNAVPickConfirmation.IntegrationSOHeaderUDF24 = shipment.IntegrationUDF24;
                            myNAVPickConfirmation.IntegrationSOHeaderUDF25 = shipment.IntegrationUDF25;
                            
                            foreach (Carton carton in shipment.Cartons)
                            {
                                //EventNotificationClient.Instance.PostMessage("NAV.Upload.SalesOrder.WriteToNAV", CompanyConfig.InstanceId, "Parsing Cartons", new Variable("Carton ASN", carton.CartonASN));

                                //Parse Carton Record
                                myNAVPickConfirmation.TrackTraceNumber = carton.TrackTraceNumber.ToString();
                                myNAVPickConfirmation.Warehouse = carton.Warehouse.ToString();
                                myNAVPickConfirmation.CartonASN = carton.CartonASN.ToString();
                                myNAVPickConfirmation.PalletASN = carton.PalletASN.ToString();
                                myNAVPickConfirmation.FreightHandlingAmount = carton.FreightAndHandling.ToString();
                                myNAVPickConfirmation.BoxSize = carton.BoxSize.ToString();
                                myNAVPickConfirmation.SCC18 = carton.SCC18.ToString();
                                myNAVPickConfirmation.Dunnage = carton.Dunnage.ToString();
                                myNAVPickConfirmation.AdditionalBarcodeReference = carton.AdditionalBarcodeReference.ToString();
                                myNAVPickConfirmation.TotalCube = carton.TotalCube.ToString();

                                foreach (CartonLine cartonLine in carton.Lines)
                                {
                                    //EventNotificationClient.Instance.PostMessage("NAV.Upload.SalesOrder.WriteToNAV", CompanyConfig.InstanceId, "Parsing Carton Lines", new Variable("Carton Lines ASN", cartonLine.RowId));

                                    //01/16/2016 iCepts DRS - Allow 0 Quantity Packed to goto NAV
                                    //if (cartonLine.QuantityPacked == 0)
                                    //{
                                    //    //NAV doesn't like a line of 0 quantity
                                    //    EventNotificationClient.Instance.PostMessage("NAV.Upload.SalesOrder", CompanyConfig.InstanceId,
                                    //        "Removed Line of 0", new Variable("Order", cartonLine.HostOrder), new Variable("Line", cartonLine.LineNum));
                                    //    continue;
                                    //}

                                    //Parse Carton Line Record
                                    myNAVPickConfirmation.RowId = cartonLine.RowId.ToString();
                                    myRowID = cartonLine.RowId.ToString();
                                    myNAVPickConfirmation.ProductCode = cartonLine.ProductCode.ToString();
                                    myNAVPickConfirmation.OrderSequence = cartonLine.LineNum.ToString();
                                    myNAVPickConfirmation.UPC = cartonLine.UPC.ToString();
                                    myNAVPickConfirmation.CartonLineExtra1 = cartonLine.Extra1.ToString();
                                    myNAVPickConfirmation.CartonLineExtra2 = cartonLine.Extra2.ToString();
                                    myNAVPickConfirmation.CartonLineExtra3 = cartonLine.Extra3.ToString();
                                    myNAVPickConfirmation.CartonLineExtra4 = cartonLine.Extra4.ToString();
                                    myNAVPickConfirmation.CartonLineExtra5 = cartonLine.Extra5.ToString();
                                    myNAVPickConfirmation.DropShipWarehouse = cartonLine.DropShipWarehouse.ToString();
                                    myNAVPickConfirmation.PickerId = cartonLine.PickerId.ToString();
                                    myNAVPickConfirmation.CustomerPartNumber = cartonLine.CustomerPartNumer.ToString();
                                    myNAVPickConfirmation.Zone = cartonLine.Zone.ToString();
                                    myNAVPickConfirmation.PurchaseOrderNumber = cartonLine.PurchaseOrderNumber.ToString();
                                    myNAVPickConfirmation.TieBreaker = cartonLine.TieBreaker;
                                    myNAVPickConfirmation.PickBin = cartonLine.BinLocation;
                                    myNAVPickConfirmation.QuantityPackedASN = cartonLine.QuantityPacked.ToString();
                                    myNAVPickConfirmation.QuantityOrdered = cartonLine.QuantityOrdered.ToString();
                                    myNAVPickConfirmation.QtyPackedForPackslip = cartonLine.QuantityForPackslip.ToString();//DRS 05/02/16
                                    myNAVPickConfirmation.IsComplete = cartonLine.IsComplete.ToString(); //DRS 05/02/16
                                    myNAVPickConfirmation.Packsize = cartonLine.Packsize.ToString();
                                    myNAVPickConfirmation.TransportMethod = cartonLine.ProductSubstitute.ToString();
                                    myNAVPickConfirmation.POLineNumber = cartonLine.PurchaseOrderLine.ToString();
                                    myNAVPickConfirmation.SOLineUDF1 = cartonLine.WMSUDF1.ToString();
                                    myNAVPickConfirmation.SOLineUDF2 = cartonLine.WMSUDF2.ToString();
                                    myNAVPickConfirmation.IntegrationSOLineUDF1 = cartonLine.IntegrationUDF1;
                                    myNAVPickConfirmation.IntegrationSOLineUDF2 = cartonLine.IntegrationUDF2;
                                    myNAVPickConfirmation.IntegrationSOLineUDF3 = cartonLine.IntegrationUDF3;
                                    myNAVPickConfirmation.IntegrationSOLineUDF4 = cartonLine.IntegrationUDF4;
                                    myNAVPickConfirmation.IntegrationSOLineUDF5 = cartonLine.IntegrationUDF5;
                                    myNAVPickConfirmation.IntegrationSOLineUDF6 = cartonLine.IntegrationUDF6;
                                    myNAVPickConfirmation.IntegrationSOLineUDF7 = cartonLine.IntegrationUDF7;
                                    myNAVPickConfirmation.IntegrationSOLineUDF8 = cartonLine.IntegrationUDF8;
                                    myNAVPickConfirmation.IntegrationSOLineUDF9 = cartonLine.IntegrationUDF9;
                                    myNAVPickConfirmation.IntegrationSOLineUDF10 = cartonLine.IntegrationUDF10;
                                    myNAVPickConfirmation.IntegrationSOLineUDF11 = cartonLine.IntegrationUDF11;
                                    myNAVPickConfirmation.IntegrationSOLineUDF12 = cartonLine.IntegrationUDF12;
                                    myNAVPickConfirmation.IntegrationSOLineUDF13 = cartonLine.IntegrationUDF13;
                                    myNAVPickConfirmation.IntegrationSOLineUDF14 = cartonLine.IntegrationUDF14;
                                    myNAVPickConfirmation.IntegrationSOLineUDF15 = cartonLine.IntegrationUDF15;
                                    myNAVPickConfirmation.IntegrationSOLineUDF16 = cartonLine.IntegrationUDF16;
                                    myNAVPickConfirmation.IntegrationSOLineUDF17 = cartonLine.IntegrationUDF17;
                                    myNAVPickConfirmation.IntegrationSOLineUDF18 = cartonLine.IntegrationUDF18;
                                    myNAVPickConfirmation.IntegrationSOLineUDF19 = cartonLine.IntegrationUDF19;
                                    myNAVPickConfirmation.IntegrationSOLineUDF20 = cartonLine.IntegrationUDF20;
                                    myNAVPickConfirmation.IntegrationSOLineUDF21 = cartonLine.IntegrationUDF21;
                                    myNAVPickConfirmation.IntegrationSOLineUDF22 = cartonLine.IntegrationUDF22;
                                    myNAVPickConfirmation.IntegrationSOLineUDF23 = cartonLine.IntegrationUDF23;
                                    myNAVPickConfirmation.IntegrationSOLineUDF24 = cartonLine.IntegrationUDF24;
                                    myNAVPickConfirmation.IntegrationSOLineUDF25 = cartonLine.IntegrationUDF25;

                                    if ((cartonLine.Attributes != null) && (cartonLine.Attributes.Count > 0))
                                    {
                                        int tmpIndex = cartonLine.Attributes.Count - 1;
                                        myNAVPickConfirmation.Attribute1 = cartonLine.Attributes[tmpIndex].Attribute1.ToString();
                                        myNAVPickConfirmation.Attribute2 = cartonLine.Attributes[tmpIndex].Attribute2.ToString();
                                        myNAVPickConfirmation.Attribute3 = cartonLine.Attributes[tmpIndex].Attribute3.ToString();
                                        myNAVPickConfirmation.Attribute4 = cartonLine.Attributes[tmpIndex].Attribute4.ToString();
                                        myNAVPickConfirmation.Attribute5 = cartonLine.Attributes[tmpIndex].Attribute5.ToString();
                                        myNAVPickConfirmation.Attribute6 = cartonLine.Attributes[tmpIndex].Attribute6.ToString();
                                        myNAVPickConfirmation.Attribute7 = cartonLine.Attributes[tmpIndex].Attribute7.ToString();
                                        myNAVPickConfirmation.Attribute8 = cartonLine.Attributes[tmpIndex].Attribute8.ToString();
                                        myNAVPickConfirmation.Attribute9 = cartonLine.Attributes[tmpIndex].Attribute9.ToString();
                                        myNAVPickConfirmation.Attribute10 = cartonLine.Attributes[tmpIndex].Attribute10.ToString();
                                    }
                                    else
                                    {
                                        myNAVPickConfirmation.Attribute1 = " ";
                                        myNAVPickConfirmation.Attribute2 = " ";
                                        myNAVPickConfirmation.Attribute3 = " ";
                                        myNAVPickConfirmation.Attribute4 = " ";
                                        myNAVPickConfirmation.Attribute5 = " ";
                                        myNAVPickConfirmation.Attribute6 = " ";
                                        myNAVPickConfirmation.Attribute7 = " ";
                                        myNAVPickConfirmation.Attribute8 = " ";
                                        myNAVPickConfirmation.Attribute9 = " ";
                                        myNAVPickConfirmation.Attribute10 = " ";
                                    }

                                    //Add Receipt Confirmation to the List of Receipt Confirmations
                                    myNAVPickConfirmations.Add(myNAVPickConfirmation);
                                }
                            }
                        }
                  }
                         
                    //Send Pick Confirmation to NAV 
                                   myNAVPickConfirmationRoot.NAVPickConfirmation = myNAVPickConfirmations.ToArray();

                                    EventNotificationClient.Instance.PostMessage("NAV.Upload.SalesOrder", CompanyConfig.InstanceId,
                                        "Submitting Picks to NAV", new Variable("data", myNAVPickConfirmationRoot.ToString()),
                                        new Variable("xml data", Utils.SerializeToString(myNAVPickConfirmationRoot.NAVPickConfirmation)));

                                    myResult = CompanyConfig.Instance.NavWebServiceInstance.ImportPickConfirmation(myNAVPickConfirmationRoot);
                                    CompanyConfig.Instance.NavWebServiceInstance.Dispose();
                              
                                    if (myResult == "OK")
                                    {
                                        //EventNotificationClient.Instance.PostMessage("NAV.Upload.SalesOrder.WriteToNAV", CompanyConfig.InstanceId,
                                        //    myResult.ToString(), new Variable("Receipt Uploaded", myRowID));

                                        myUploadConfirmation.IsSuccess = true;
                                        myUploadConfirmation.RowId = myRowID;
                                        myUploadConfirmation.ErrorMessage = myResult.ToString();
                                        uploadConfirmations.Add(myUploadConfirmation);
                                    }
                                    else
                                    {
                                        EventNotificationClient.Instance.PostMessage("NAV.Upload.SalesOrder.Error", CompanyConfig.InstanceId,
                                            "Error | " + myResult.ToString(), new Variable("Pick Confirmation Failed | 1000", myRowID));

                                        myUploadConfirmation.IsSuccess = false;
                                        myUploadConfirmation.RowId = myRowID;
                                        myUploadConfirmation.ErrorMessage = "Upload to NAV failed |" + myResult.ToString();
                                        uploadConfirmations.Add(myUploadConfirmation);
                                    }
                         
                }
                    catch (Exception ex)
                    {
                        CompanyConfig.Instance.NavWebServiceInstance.Dispose();

                        EventNotificationClient.Instance.PostMessage("NAV.Upload.SalesOrder.Error", CompanyConfig.InstanceId,
                            "Error | " + ex, new Variable("Pick Confirmation | 2000", ex));

                        myUploadConfirmation.IsSuccess = false;
                        myUploadConfirmation.RowId = myRowID;
                        myUploadConfirmation.ErrorMessage = ex.ToString();
                        uploadConfirmations.Add(myUploadConfirmation);
                    }
            }
            catch
            {
                //Error at Shipment Level
                uploadConfirmations = new List<UploadConfirmation>();
                return uploadConfirmations;
            }

            //EventNotificationClient.Instance.PostMessage("NAV.Upload.PurchaseOrder.WriteToNAV", CompanyConfig.InstanceId,
            //    string.Format("Existing WriteToNAV", ""));
            
            return uploadConfirmations;
        }
        #endregion

        #region SVA RetrievalRequestFromNAV
        public static List<Packslip> RetrievalRequestFromNAV()
        {
            NAVPackslipRetrievalRoot myNAVPackslipRetrievalRoot = new NAVPackslipRetrievalRoot();
            List<Packslip> myWMSPackslips = new List<Packslip>();

            String myResult = String.Empty;
            try
            {

                Packslip myWMSPackslip;

                // Call NAV Web Service | ExportSalesOrder
                myResult = CompanyConfig.Instance.NavWebServiceInstance.RetrievePackslip(ref myNAVPackslipRetrievalRoot);
                CompanyConfig.Instance.NavWebServiceInstance.Dispose();

                //NAV Business Object | Code = PH
                NAVBusinessObjectPHR myNAVBusinessObjectPHR = new NAVBusinessObjectPHR();
                myNAVBusinessObjectPHR = myNAVPackslipRetrievalRoot.NAVBusinessObjectPHR;

                //NAV Pick Headers | Processed = Ready to Send | Header Only
                NAVPickHeaderRetrievals myNAVPickHeaderRetrievals = new NAVPickHeaderRetrievals();
                myNAVPickHeaderRetrievals = myNAVBusinessObjectPHR.NAVPickHeaderRetrievals;

                try
                {

                    // For each Pick Header record in the queue
                    foreach (NAVPickHeaderRetrieval myNAVOrder in myNAVPickHeaderRetrievals.NAVPickHeaderRetrieval)
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
                            EventNotificationClient.Instance.PostMessage("NAV.Download.SalesOrderRetrieval", CompanyConfig.InstanceId, "A1I -> Status Update -> NAV", new Variable("Status", "Sent to WMS"), new Variable("Sales Order Number", NAVAdapterUtils.NAVFormatString(myNAVOrder.OrderNumber.ToString(), 40)), new Variable("Entry No", NAVAdapterUtils.NAVFormatString(myNAVOrder.StagingID.ToString(), 40)));
                            try
                            {
                                // Call NAV Web Service | SentStatus
                                CompanyConfig.Instance.NavWebServiceInstance.SentStatus(NAVAdapterUtils.NAVFormatString(myNAVOrder.StagingID.ToString(), 40), "PH");
                                CompanyConfig.Instance.NavWebServiceInstance.Dispose();
                            }
                            catch (Exception ex1)
                            {
                                EventNotificationClient.Instance.PostMessage("NAV.Download.SalesOrderRetrieval.Error", CompanyConfig.InstanceId, "Error: A1I Entry No -> NAV", new Variable("Error Sending to WMS", ex1));
                            }
                        }
                    }
                }
                catch (Exception exceptionPSLvl)
                {
                    //Error at Packslip Level
                    EventNotificationClient.Instance.PostMessage("NAV.Download.SalesOrderRetrieval.Error", CompanyConfig.InstanceId, "Error: Packslip Level", new Variable("Error Sending to WMS", exceptionPSLvl));
                }
            }
            catch (Exception exceptionPSRetLvl)
            {
                //Error at Packslip Retrieval Level
                EventNotificationClient.Instance.PostMessage("NAV.Download.SalesOrderRetrieval.Error", CompanyConfig.InstanceId, "Error: Packslip Retrieval Level", new Variable("Error Sending to WMS", exceptionPSRetLvl));
            }

            return myWMSPackslips;
        }
        #endregion
    }
}

