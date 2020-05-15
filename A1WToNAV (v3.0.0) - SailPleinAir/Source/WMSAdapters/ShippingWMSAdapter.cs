using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Reflection;
using Accellos.Interfaces.NAV.BusinessObjects;
using Accellos.InterfaceFramework.EventNotificationServer;
using Accellos.Integrations.CommonUtilities;

namespace Accellos.Interfaces.NAV.WMSAdapters
{
    public partial class WMSAdapter
    {
        //To WMS
        #region ParsePackslip
        public static DataTable ParsePackslip(Packslip packslip)
        {
            DataTable result = CompanyConfig.Instance.DownloadTableSchema;

            #region PH
            DataRow row = result.NewRow();

            row["FIELD001"] = "PH";
            row["FIELD002"] = packslip.RecordType;
            row["FIELD003"] = packslip.BatchControlNumber;
            row["FIELD004"] = packslip.CustomerNumber;
            row["FIELD005"] = packslip.OrderNumber;
            row["FIELD006"] = packslip.BackOrderNumber;
            row["FIELD007"] = packslip.CustomerPONumber;
            row["FIELD008"] = packslip.CustomerLabel;
            row["FIELD009"] = packslip.CrystalPackingSlip;
            row["FIELD010"] = StringUtils.Left(packslip.CustomerGroup, 10);
            row["FIELD011"] = packslip.LabelText;
            row["FIELD012"] = StringUtils.Left(packslip.ShipName, 35);
            row["FIELD013"] = StringUtils.Left(packslip.ShipAddressName1, 35);
            row["FIELD014"] = StringUtils.Left(packslip.ShipAddressName2, 35);
            row["FIELD015"] = StringUtils.Left(packslip.ShipCity, 35);
            row["FIELD016"] = StringUtils.Left(packslip.ShipProvinceState, 3);
            row["FIELD017"] = StringUtils.Left(packslip.ShipPostalZipCode, 10);
            row["FIELD018"] = StringUtils.Left(packslip.ShipCountry, 13);
            row["FIELD019"] = StringUtils.Left(packslip.ShipAttnTo, 35);
            row["FIELD020"] = StringUtils.Left(packslip.ShipTelephoneNumber, 25);
            row["FIELD021"] = StringUtils.Left(packslip.BillName, 35);
            row["FIELD022"] = StringUtils.Left(packslip.BillAddress1, 35);
            row["FIELD023"] = StringUtils.Left(packslip.BillAddress2, 35);
            row["FIELD024"] = StringUtils.Left(packslip.BillCity, 35);
            row["FIELD025"] = StringUtils.Left(packslip.BillProvinceState, 3);
            row["FIELD026"] = StringUtils.Left(packslip.BillPostalZip, 10);
			row["FIELD027"] = StringUtils.Left(packslip.BillCountry, 13);
            row["FIELD028"] = packslip.ShipDateRequired == DateTime.MinValue ? string.Empty : packslip.ShipDateRequired.ToString("yyyyMMdd");
            row["FIELD029"] = packslip.ShipTimeRequired;
            row["FIELD030"] = StringUtils.Left(packslip.TruckRoute, 10);
            row["FIELD031"] = StringUtils.Left(packslip.Priority, 1);

            #region row["FIELD032"] = packslip.ShipmentPaymentType
            switch (packslip.ShipmentPaymentType)
            {
                case ShipmentPaymentType.Collect:
                    row["FIELD032"] = "C";
                    break;
                case ShipmentPaymentType.BillRecipient:
                    row["FIELD032"] = "R";
                    break;
                case ShipmentPaymentType.Prepaid:
                    row["FIELD032"] = "P";
                    break;
                case ShipmentPaymentType.ThirdParty:
                    row["FIELD032"] = "T";
                    break;
                case ShipmentPaymentType.DutyPaidThirdParty:
                    row["FIELD032"] = "G";
                    break;
                default:
                    row["FIELD032"] = string.Empty;
                    break;
            }
            #endregion

            row["FIELD033"] = StringUtils.Left(packslip.ShipViaPreferred, 15);
            row["FIELD034"] = StringUtils.Left(packslip.ShipPayAccountNumber, 16);
            row["FIELD035"] = packslip.OrderDate == DateTime.MinValue ? string.Empty : packslip.OrderDate.ToString("yyyyMMdd");
            row["FIELD036"] = packslip.CancelDate == DateTime.MinValue ? string.Empty : packslip.CancelDate.ToString("yyyyMMdd");

			#region row["FIELD037"] = packslip.PurchaseOrderType
			switch (packslip.PurchaseOrderType)
			{
				case PurchaseOrderType.NotSpecified:
					row["FIELD037"] = string.Empty;
					break;
				case PurchaseOrderType.StandAlone:
					row["FIELD037"] = "SA";
					break;
				case PurchaseOrderType.Promotional:
					row["FIELD037"] = "PR";
					break;
				case PurchaseOrderType.CustomerSpecialOrder:
					row["FIELD037"] = "OS";
					break;
				case PurchaseOrderType.ReOrder:
					row["FIELD037"] = "RE";
					break;
				case PurchaseOrderType.Transfer:
					row["FIELD037"] = "TRANSFER";
					break;
			}
			#endregion

			row["FIELD038"] = StringUtils.Left(packslip.Shipper, 10); // Changed for A1W 64
            row["FIELD039"] = packslip.ShipCompleteOnly ? "Y" : "N";
            row["FIELD040"] = packslip.ClientMH10Number;
            row["FIELD041"] = StringUtils.Left(packslip.ClientName, 10);
            row["FIELD042"] = StringUtils.Left(packslip.SpecialInstructions1, 20);
            row["FIELD043"] = StringUtils.Left(packslip.SpecialInstructions2, 20);
            row["FIELD044"] = StringUtils.Left(packslip.SpecialInstructions3, 20);
            row["FIELD045"] = StringUtils.Left(packslip.SpecialInstructions4, 20);
            row["FIELD046"] = StringUtils.Left(packslip.SpecialInstructions5, 20);
            row["FIELD047"] = StringUtils.Left(packslip.SpecialInstructions6, 20);
            row["FIELD048"] = StringUtils.Left(packslip.SpecialInstructions7, 20);
            row["FIELD049"] = StringUtils.Left(packslip.CostCenter, 12);
            row["FIELD050"] = StringUtils.Left(packslip.ShipToNumber, 35);
            row["FIELD051"] = packslip.IsCustomerAcceptsBackorders ? "Y" : "N";
            row["FIELD052"] = StringUtils.Left(packslip.TruckStop, 5);
            row["FIELD053"] = packslip.TruckDeliveryTime;
            row["FIELD054"] = StringUtils.Left(packslip.ShipperName, 35);
            row["FIELD055"] = StringUtils.Left(packslip.ShipperAddress1, 35);
            row["FIELD056"] = StringUtils.Left(packslip.ShipperAddress2, 35);
            row["FIELD057"] = StringUtils.Left(packslip.ShipperCity, 35);
            row["FIELD058"] = StringUtils.Left(packslip.ShipperProvinceState, 3);
            row["FIELD059"] = StringUtils.Left(packslip.ShipperPostalZipCode, 10);
            row["FIELD060"] = StringUtils.Left(packslip.PackingSlip, 8);
            row["FIELD061"] = StringUtils.Left(packslip.IncludePickZones, 10);
            row["FIELD062"] = StringUtils.Left(packslip.ExcludePickZones, 10);
            row["FIELD063"] = StringUtils.Left(packslip.IncludeReplenishmentZones, 10);
            row["FIELD064"] = packslip.ShipWithOtherGoods ? "Y" : "N";
            row["FIELD065"] = packslip.DontSplitExpiryDates ? "Y" : "N";
            row["FIELD066"] = StringUtils.Left(packslip.ShipmentOptions, 5);
            row["FIELD067"] = StringUtils.Left(packslip.ShipmentActions, 1);
            row["FIELD068"] = StringUtils.Left(packslip.ShipmentMessage, 20);
            row["FIELD069"] = StringUtils.Left(packslip.ManifestReportName, 8);
            row["FIELD070"] = StringUtils.Left(packslip.COD, 1);
            row["FIELD071"] = StringUtils.Left(packslip.ShipmentConsolidation, 19);
            row["FIELD072"] = StringUtils.Left(packslip.PricingTicket, 10);
            row["FIELD073"] = packslip.DontShipBeforeDate == DateTime.MinValue ? string.Empty : packslip.DontShipBeforeDate.ToString("yyyyMMdd");
            row["FIELD074"] = StringUtils.Left(packslip.ShipToFaxNumber, 14);
            row["FIELD075"] = packslip.WaveNumber;
            row["FIELD076"] = StringUtils.Left(packslip.ReplenishmentGroup, 10);
            row["FIELD077"] = StringUtils.Left(packslip.CustomerUDF1, 30);
            row["FIELD078"] = StringUtils.Left(packslip.CustomerUDF2, 30);
            row["FIELD079"] = StringUtils.Left(packslip.CustomerUDF3, 30);
            row["FIELD080"] = StringUtils.Left(packslip.BusinessTelephoneNumber, 25);
            row["FIELD081"] = StringUtils.Left(packslip.Email, 40);
            row["FIELD082"] = StringUtils.Left(packslip.CustomerUDF4, 30);
            row["FIELD083"] = StringUtils.Left(packslip.CustomerUDF5, 30);
			row["FIELD084"] = StringUtils.Left(packslip.OrderSource, 10); // Added for A1W 64
			row["FIELD085"] = packslip.TotalInvoiceAmountCharge.ToString(CultureInfo.InvariantCulture);
            row["FIELD086"] = StringUtils.Left(packslip.ShipAddress3, 35);
            row["FIELD087"] = StringUtils.Left(packslip.BillAddress3, 35);
            row["FIELD088"] = StringUtils.Left(packslip.ShipperAddress3, 35);
            row["FIELD089"] = StringUtils.Left(packslip.ShipperCountry, 40);
            row["FIELD090"] = packslip.TotalFreightToCharge.ToString(CultureInfo.InvariantCulture);
            row["FIELD091"] = packslip.TotalShippingHandlingCharge.ToString(CultureInfo.InvariantCulture);
            row["FIELD092"] = packslip.PromoAmount.ToString(CultureInfo.InvariantCulture);
            row["FIELD093"] = packslip.PromoDiscount.ToString(CultureInfo.InvariantCulture);
            row["FIELD094"] = StringUtils.Left(packslip.EndOfLineProcess.ToString(CultureInfo.InvariantCulture), 1);
            row["FIELD095"] = StringUtils.Left(packslip.PurchaseOrderNumber, 20);
            row["FIELD096"] = StringUtils.Left(packslip.Special1, 20);
            row["FIELD097"] = StringUtils.Left(packslip.Special2, 20);
            row["FIELD098"] = StringUtils.Left(packslip.Special3, 20);
            row["FIELD099"] = packslip.MinimumDaysOfExpiry.ToString(CultureInfo.InvariantCulture);
            row["FIELD100"] = packslip.MixedLotIndicator ? "Y" : "N";
            row["FIELD101"] = StringUtils.Left(packslip.Extra1, 40);
            row["FIELD102"] = StringUtils.Left(packslip.Extra2, 40);
            row["FIELD103"] = StringUtils.Left(packslip.Extra3, 40);
            row["FIELD104"] = StringUtils.Left(packslip.Extra4, 40);
            row["FIELD105"] = StringUtils.Left(packslip.Extra5, 40);
            row["FIELD106"] = StringUtils.Left(packslip.Handle, 10);
            row["FIELD107"] = StringUtils.Left(packslip.CustomerCartonContentLabel, 8);

            #region row["FIELD108"] = packslip.CartonType
            switch (packslip.CartonType)
            {
                case CartonType.NotSpecified:
                    row["FIELD108"] = string.Empty;
                    break;
                case CartonType.CartonLabel:
                    row["FIELD108"] = "0";
                    break;
                case CartonType.PalletLabel:
                    row["FIELD108"] = "1";
                    break;
                case CartonType.TruckLoadLabel:
                    row["FIELD108"] = "2";
                    break;
            }
            #endregion

            row["FIELD109"] = packslip.WarehouseTransitLocation;
            row["FIELD110"] = packslip.WarehouseDestinationLocation;
            row["FIELD111"] = StringUtils.Left(packslip.DestinationBinLocation, 8);
            row["FIELD112"] = StringUtils.Left(packslip.PlannerID, 15);
            row["FIELD113"] = StringUtils.Left(packslip.ControlBuyerID, 15);
			row["FIELD114"] = packslip.ScheduledStartDate;
            row["FIELD115"] = packslip.FinishedGoodItemNumber;
            row["FIELD116"] = packslip.FinishedGoodDescription;
            row["FIELD117"] = packslip.FinishedGoodClassID;
            row["FIELD118"] = packslip.MOENdingQuantity;
            row["FIELD119"] = packslip.FirstWorkCenter;
            row["FIELD120"] = packslip.MODescription;
            row["FIELD121"] = packslip.PrimaryBillToAddress;
            row["FIELD122"] = packslip.WorkArea;
            row["FIELD123"] = StringUtils.Left(packslip.AssemblyNote1, 40);
            row["FIELD124"] = StringUtils.Left(packslip.AssemblyNote2, 40);
            row["FIELD125"] = StringUtils.Left(packslip.AssemblyNote3, 40);
            row["FIELD126"] = StringUtils.Left(packslip.AssemblyNote4, 40);
            row["FIELD127"] = StringUtils.Left(packslip.AssemblyNote5, 40);
			row["FIELD128"] = StringUtils.Left(packslip.ShipperEIN, 40);
			row["FIELD129"] = StringUtils.Left(packslip.ShipToEIN, 40);
			row["FIELD130"] = StringUtils.Left(packslip.IntegrationUDF1, 40); // Added for A1W 64
			row["FIELD131"] = StringUtils.Left(packslip.IntegrationUDF2, 40); // Added for A1W 64
			row["FIELD132"] = StringUtils.Left(packslip.IntegrationUDF3, 40); // Added for A1W 64
			row["FIELD133"] = StringUtils.Left(packslip.IntegrationUDF4, 40); // Added for A1W 64
			row["FIELD134"] = StringUtils.Left(packslip.IntegrationUDF5, 40); // Added for A1W 64
			row["FIELD135"] = StringUtils.Left(packslip.IntegrationUDF6, 40); // Added for A1W 64
			row["FIELD136"] = StringUtils.Left(packslip.IntegrationUDF7, 40); // Added for A1W 64
			row["FIELD137"] = StringUtils.Left(packslip.IntegrationUDF8, 40); // Added for A1W 64
			row["FIELD138"] = StringUtils.Left(packslip.IntegrationUDF9, 40); // Added for A1W 64
			row["FIELD139"] = StringUtils.Left(packslip.IntegrationUDF10, 40); // Added for A1W 64
			row["FIELD140"] = StringUtils.Left(packslip.IntegrationUDF11, 40); // Added for A1W 64
			row["FIELD141"] = StringUtils.Left(packslip.IntegrationUDF12, 40); // Added for A1W 64
			row["FIELD142"] = StringUtils.Left(packslip.IntegrationUDF13, 40); // Added for A1W 64
			row["FIELD143"] = StringUtils.Left(packslip.IntegrationUDF14, 40); // Added for A1W 64
			row["FIELD144"] = StringUtils.Left(packslip.IntegrationUDF15, 40); // Added for A1W 64
			row["FIELD145"] = StringUtils.Left(packslip.IntegrationUDF16, 40); // Added for A1W 64
			row["FIELD146"] = StringUtils.Left(packslip.IntegrationUDF17, 40); // Added for A1W 64
			row["FIELD147"] = StringUtils.Left(packslip.IntegrationUDF18, 40); // Added for A1W 64
			row["FIELD148"] = StringUtils.Left(packslip.IntegrationUDF19, 40); // Added for A1W 64
			row["FIELD149"] = StringUtils.Left(packslip.IntegrationUDF20, 40); // Added for A1W 64
			row["FIELD150"] = StringUtils.Left(packslip.IntegrationUDF21, 40); // Added for A1W 64
			row["FIELD151"] = StringUtils.Left(packslip.IntegrationUDF22, 40); // Added for A1W 64
			row["FIELD152"] = StringUtils.Left(packslip.IntegrationUDF23, 40); // Added for A1W 64
			row["FIELD153"] = StringUtils.Left(packslip.IntegrationUDF24, 40); // Added for A1W 64
			row["FIELD154"] = StringUtils.Left(packslip.IntegrationUDF25, 40); // Added for A1W 64

            result.Rows.Add(row);
            #endregion

            #region PD
            foreach (PackslipLine line in packslip.Lines)
            {
                row = result.NewRow();

                row["FIELD001"] = "PD";
                row["FIELD002"] = line.Warehouse;
                row["FIELD003"] = line.Location;
                row["FIELD004"] = line.CustomerNumber;
                row["FIELD005"] = line.OrderNumber;
                row["FIELD006"] = line.BackOrderNumber;
                row["FIELD007"] = StringUtils.Left(line.StoreNumber, 10);
                row["FIELD008"] = StringUtils.Left(line.DepartmentNumber, 10);
                row["FIELD009"] = StringUtils.Left(line.PromoNumber, 20);
                row["FIELD010"] = line.OrderSequenceNumber;
                row["FIELD011"] = StringUtils.Left(line.CustomerLineReference, 12);
                row["FIELD012"] = line.ProductCode;
                row["FIELD013"] = line.PrimaryLocation;
				row["FIELD014"] = StringUtils.Left(line.Description, 100); // Changed to 100 characters for A1W 64
                row["FIELD015"] = line.UnitOfMeasureMultiplier;
                row["FIELD016"] = StringUtils.Left(line.ProductClass, 6);
                row["FIELD017"] = StringUtils.Left(line.UPC, 14);
                row["FIELD018"] = line.QuantityOrdered.ToString(CultureInfo.InvariantCulture);
                row["FIELD019"] = line.QuantityToPick.ToString(CultureInfo.InvariantCulture);
                row["FIELD020"] = StringUtils.Left(line.HazmatCode, 6);
                row["FIELD021"] = line.CustomerXRef;
                row["FIELD022"] = StringUtils.Left(line.CommentIndicator, 1);
                row["FIELD023"] = line.UnitPrice.ToString(CultureInfo.InvariantCulture);
                row["FIELD024"] = StringUtils.Left(line.Weight.ToString(CultureInfo.InvariantCulture), 14);
                row["FIELD025"] = StringUtils.Left(line.Cube.ToString(CultureInfo.InvariantCulture), 20);
                row["FIELD026"] = StringUtils.Left(line.StoreName, 35);
                row["FIELD027"] = StringUtils.Left(line.StoreAddress1, 35);
                row["FIELD028"] = StringUtils.Left(line.StoreAddress2, 35);
                row["FIELD029"] = StringUtils.Left(line.StoreCity, 35);
                row["FIELD030"] = StringUtils.Left(line.StoreProvinceState, 3);
                row["FIELD031"] = StringUtils.Left(line.StorePostalZip, 10);
                row["FIELD032"] = StringUtils.Left(line.StoreCountry, 13);
                row["FIELD039"] = StringUtils.Left(line.SpecialInstructions1, 20);
                row["FIELD040"] = StringUtils.Left(line.SpecialInstructions2, 20);
                row["FIELD041"] = StringUtils.Left(line.SpecialInstructions3, 20);
                row["FIELD042"] = StringUtils.Left(line.SpecialInstructions4, 20);
                row["FIELD043"] = StringUtils.Left(line.SpecialInstructions5, 20);
                row["FIELD044"] = StringUtils.Left(line.SpecialInstructions6, 20);
                row["FIELD045"] = StringUtils.Left(line.SpecialInstructions7, 20);
                row["FIELD046"] = StringUtils.Left(line.Extra, 20);
				row["FIELD047"] = ParseTracking(line.PickAttributeTracking); // Do not send attributes, as they will be pulled from prodmstr
                row["FIELD048"] = line.CustomerPoNumber;
                row["FIELD049"] = line.Discount.ToString(CultureInfo.InvariantCulture);
                row["FIELD050"] = line.RetailPrice.ToString(CultureInfo.InvariantCulture);
                row["FIELD051"] = line.ValuePrice.ToString(CultureInfo.InvariantCulture);
                row["FIELD052"] = StringUtils.Left(line.PriceTicketDescription, 20);
                row["FIELD057"] = line.DaysToExpire.ToString();
                row["FIELD058"] = line.VendorNumber;
                row["FIELD059"] = StringUtils.Left(line.CountryOfOrigin, 6);
                row["FIELD060"] = line.SellPrice.ToString(CultureInfo.InvariantCulture);
                row["FIELD061"] = line.Tax1 ? "Y" : "N";
                row["FIELD062"] = line.Tax2 ? "Y" : "N";
                row["FIELD063"] = line.Tax3 ? "Y" : "N";
                row["FIELD064"] = line.Tax4 ? "Y" : "N";
                row["FIELD065"] = StringUtils.Left(line.StockItem, 1);
                row["FIELD066"] = line.PurchaseOrderNumber;
                row["FIELD067"] = line.PurchaseOrderLineNumber;
                row["FIELD068"] = StringUtils.Left(line.EDPNumber, 10);
                row["FIELD069"] = StringUtils.Left(line.Commitment.ToString(CultureInfo.InvariantCulture), 3);
                row["FIELD070"] = StringUtils.Left(line.Extra1, 40);
                row["FIELD071"] = StringUtils.Left(line.Extra2, 40);
                row["FIELD072"] = StringUtils.Left(line.Extra3, 40);
                row["FIELD073"] = StringUtils.Left(line.Extra4, 40);
                row["FIELD074"] = StringUtils.Left(line.Extra5, 40);

                #region row["FIELD075"] = line.VariableLengthProduct;
                switch (line.VariableLengthProduct)
                {
                    case VariableLengthType.NotVLP:
                        row["FIELD075"] = string.Empty;
                        break;
                    case VariableLengthType.VLP:
                        row["FIELD075"] = "Y";
                        break;
                    case VariableLengthType.ContinuousVLP:
                        row["FIELD075"] = "C";
                        break;
                }
                #endregion

                #region row["FIELD076"] = line.KitType;
                switch (line.KitType)
                {
                    case KitType.NotSpecified:
                        row["FIELD076"] = string.Empty;
                        break;
                    case KitType.BuildToStockWorkOrder:
                        row["FIELD076"] = "0";
                        break;
                    case KitType.BuildToOrderWorkOrder:
                        row["FIELD076"] = "1";
                        break;
                    case KitType.ExplodeForPicking:
                        row["FIELD076"] = "2";
                        break;
                    case KitType.ExplodeForPickingWithShortedComponents:
                        row["FIELD076"] = "3";
                        break;
                    case KitType.RecipeOnly:
                        row["FIELD076"] = "4";
                        break;
                    case KitType.ExplodeDownload:
                        row["FIELD076"] = "6";
                        break;
                }
                #endregion

                row["FIELD077"] = StringUtils.Left(line.InnerPacksize.ToString(CultureInfo.InvariantCulture), 6);
                row["FIELD078"] = StringUtils.Left(line.MiddlePacksize.ToString(CultureInfo.InvariantCulture), 6);
                row["FIELD079"] = StringUtils.Left(line.UnitOfMeasureTextDescription, 15);

                #region row["FIELD080"] = line.ReservationType;
                switch (line.ReservationType)
                {
					case ReservationType.NotSpecified:
                        row["FIELD080"] = string.Empty;
                        break;
                    case ReservationType.SeasonalReservation:
                        row["FIELD080"] = "S";
                        break;
                    case ReservationType.CoreReservation:
                        row["FIELD080"] = "C";
                        break;
                }
                #endregion

                row["FIELD081"] = StringUtils.Left(line.CommodityCode, 10);
                row["FIELD082"] = StringUtils.Left(line.LicensePlate, 22);
                row["FIELD083"] = StringUtils.Left(line.PackClass, 10);
                row["FIELD084"] = StringUtils.Left(line.StoreAddress3, 35);
                row["FIELD085"] = StringUtils.Left(line.CommodityDescription, 40);
                row["FIELD086"] = StringUtils.Left(line.NMFCNumber, 10);
                row["FIELD087"] = StringUtils.Left(line.TariffClass, 10);
                row["FIELD110"] = line.BackflushFlag ? "Y" : "N";
                row["FIELD111"] = StringUtils.Left(line.Salesman, 50);
				row["FIELD112"] = StringUtils.Left(line.IntegrationUDF1, 40); // Added for A1W 64
				row["FIELD113"] = StringUtils.Left(line.IntegrationUDF2, 40); // Added for A1W 64
				row["FIELD114"] = StringUtils.Left(line.IntegrationUDF3, 40); // Added for A1W 64
				row["FIELD115"] = StringUtils.Left(line.IntegrationUDF4, 40); // Added for A1W 64
				row["FIELD116"] = StringUtils.Left(line.IntegrationUDF5, 40); // Added for A1W 64
				row["FIELD117"] = StringUtils.Left(line.IntegrationUDF6, 40); // Added for A1W 64
				row["FIELD118"] = StringUtils.Left(line.IntegrationUDF7, 40); // Added for A1W 64
				row["FIELD119"] = StringUtils.Left(line.IntegrationUDF8, 40); // Added for A1W 64
				row["FIELD120"] = StringUtils.Left(line.IntegrationUDF9, 40); // Added for A1W 64
				row["FIELD121"] = StringUtils.Left(line.IntegrationUDF10, 40); // Added for A1W 64
				row["FIELD122"] = StringUtils.Left(line.IntegrationUDF11, 40); // Added for A1W 64
				row["FIELD123"] = StringUtils.Left(line.IntegrationUDF12, 40); // Added for A1W 64
				row["FIELD124"] = StringUtils.Left(line.IntegrationUDF13, 40); // Added for A1W 64
				row["FIELD125"] = StringUtils.Left(line.IntegrationUDF14, 40); // Added for A1W 64
				row["FIELD126"] = StringUtils.Left(line.IntegrationUDF15, 40); // Added for A1W 64
				row["FIELD127"] = StringUtils.Left(line.IntegrationUDF16, 40); // Added for A1W 64
				row["FIELD128"] = StringUtils.Left(line.IntegrationUDF17, 40); // Added for A1W 64
				row["FIELD129"] = StringUtils.Left(line.IntegrationUDF18, 40); // Added for A1W 64
				row["FIELD130"] = StringUtils.Left(line.IntegrationUDF19, 40); // Added for A1W 64
				row["FIELD131"] = StringUtils.Left(line.IntegrationUDF20, 40); // Added for A1W 64
				row["FIELD132"] = StringUtils.Left(line.IntegrationUDF21, 40); // Added for A1W 64
				row["FIELD133"] = StringUtils.Left(line.IntegrationUDF22, 40); // Added for A1W 64
				row["FIELD134"] = StringUtils.Left(line.IntegrationUDF23, 40); // Added for A1W 64
				row["FIELD135"] = StringUtils.Left(line.IntegrationUDF24, 40); // Added for A1W 64
				row["FIELD136"] = StringUtils.Left(line.IntegrationUDF25, 40); // Added for A1W 64

                #region Attributes
                if (line.Attributes.Count == 0)
                    result.Rows.Add(row);
                else
                {
                    foreach (AttributeSet attributes in line.Attributes)
                    {
                        DataRow attrRow = result.NewRow();
                        attrRow.ItemArray = row.ItemArray;

                        attrRow["FIELD033"] = attributes.Attribute1;
                        attrRow["FIELD034"] = attributes.Attribute2;
                        attrRow["FIELD035"] = attributes.Attribute3;
                        attrRow["FIELD036"] = attributes.Attribute4;
                        attrRow["FIELD037"] = attributes.Attribute5;
                        attrRow["FIELD038"] = attributes.Attribute6;
                        attrRow["FIELD053"] = attributes.Attribute7;
                        attrRow["FIELD054"] = attributes.Attribute8;
                        attrRow["FIELD055"] = attributes.Attribute9;
                        attrRow["FIELD056"] = attributes.Attribute10;

                        result.Rows.Add(attrRow);
                    }
                }
                #endregion

                #region Production recipies
                foreach (ProductionRecipe recipe in line.ProductionRecipes)
                {
                    row = result.NewRow();

                    row["FIELD001"] = "PW";
                    row["FIELD002"] = recipe.Warehouse;
                    row["FIELD003"] = recipe.CustomerNumber;
                    row["FIELD004"] = recipe.OrderNumber;
                    row["FIELD005"] = recipe.BackorderNumber;
                    row["FIELD006"] = recipe.OrderSequenceNumber;
                    row["FIELD007"] = recipe.WorkArea;
                    row["FIELD008"] = ParseTracking(recipe.ReceiveAttributeTracking);   //SVA Added ParseTracking
                    row["FIELD009"] = recipe.ProductCode;
                    row["FIELD010"] = recipe.PackQuantity;
                    row["FIELD011"] = StringUtils.Left(recipe.Description, 40);
                    row["FIELD012"] = recipe.QuantityToUse;
                    row["FIELD013"] = recipe.UnitOfMeasure;
                    row["FIELD014"] = StringUtils.Left(recipe.PrimaryLocation, 8);
                    row["FIELD015"] = ParseTracking(recipe.PickAttributeTracking);  //SVA Added ParseTracking
                    row["FIELD016"] = StringUtils.Left(recipe.CommentIndicator, 1);
                    row["FIELD027"] = StringUtils.Left(recipe.ClientName, 10);
                    row["FIELD028"] = StringUtils.Left(recipe.Extra1, 40);
                    row["FIELD029"] = StringUtils.Left(recipe.Extra2, 40);
                    row["FIELD030"] = StringUtils.Left(recipe.Extra3, 40);
                    row["FIELD031"] = StringUtils.Left(recipe.Extra4, 40);
                    row["FIELD032"] = StringUtils.Left(recipe.Extra5, 40);

                    #region row["FIELD033"] = recipe.ComponentType;
                    switch (recipe.ComponentType)
                    {
                        case ComponentType.CoProduct:
                            row["FIELD033"] = "C";
                            break;
                        case ComponentType.ServiceItem:
                            row["FIELD033"] = "A";
                            break;
                        default:
                            row["FIELD033"] = "S";
                            break;
                    }
                    #endregion

                    row["FIELD034"] = recipe.QuantityBuilt;
                    row["FIELD035"] = recipe.ComponentCost;
                    row["FIELD036"] = StringUtils.Left(recipe.ComponentSpecialInstructions, 40);

                    #region Attributes
                    if (recipe.Attributes.Count == 0)
                        result.Rows.Add(row);
                    else
                    {
                        foreach (AttributeSet attributes in recipe.Attributes)
                        {
                            DataRow attrRow = result.NewRow();
                            attrRow.ItemArray = row.ItemArray;

                            attrRow["FIELD017"] = attributes.Attribute1;
                            attrRow["FIELD018"] = attributes.Attribute2;
                            attrRow["FIELD019"] = attributes.Attribute3;
                            attrRow["FIELD020"] = attributes.Attribute4;
                            attrRow["FIELD021"] = attributes.Attribute5;
                            attrRow["FIELD022"] = attributes.Attribute6;
                            attrRow["FIELD023"] = attributes.Attribute7;
                            attrRow["FIELD024"] = attributes.Attribute8;
                            attrRow["FIELD025"] = attributes.Attribute9;
                            attrRow["FIELD026"] = attributes.Attribute10;

                            result.Rows.Add(attrRow);
                        }
                    }
                    #endregion
                }
                #endregion
            }
            #endregion

            return result;
        }
        #endregion

		#region Packslip to A1W Middle Tier DTO
        //public static List<DNLOAD> PackslipToDTO(Packslip packslip)
        //{
        //    List<DNLOAD> dnloads = new List<DNLOAD>();

        //    #region Lines
        //    foreach (PackslipLine line in packslip.Lines)
        //    {
        //        DNLOAD lineDnload = new DNLOAD();

        //        lineDnload.FIELD001 = "PD";
        //        lineDnload.FIELD002 = line.Warehouse;
        //        lineDnload.FIELD003 = line.Location;
        //        lineDnload.FIELD004 = line.CustomerNumber;
        //        lineDnload.FIELD005 = line.OrderNumber;
        //        lineDnload.FIELD006 = line.BackOrderNumber;
        //        lineDnload.FIELD007 = StringUtils.Left(line.StoreNumber, 10);
        //        lineDnload.FIELD008 = StringUtils.Left(line.DepartmentNumber, 10);
        //        lineDnload.FIELD009 = StringUtils.Left(line.PromoNumber, 20);
        //        lineDnload.FIELD010 = line.OrderSequenceNumber;
        //        lineDnload.FIELD011 = StringUtils.Left(line.CustomerLineReference, 12);
        //        lineDnload.FIELD012 = line.ProductCode;
        //        lineDnload.FIELD013 = line.PrimaryLocation;
        //        lineDnload.FIELD014 = StringUtils.Left(line.Description, 100); // Changed to 100 characters for A1W 64
        //        lineDnload.FIELD015 = line.UnitOfMeasureMultiplier;
        //        lineDnload.FIELD016 = StringUtils.Left(line.ProductClass, 6);
        //        lineDnload.FIELD017 = StringUtils.Left(line.UPC, 14);
        //        lineDnload.FIELD018 = line.QuantityOrdered.ToString(CultureInfo.InvariantCulture);
        //        lineDnload.FIELD019 = line.QuantityToPick.ToString(CultureInfo.InvariantCulture);
        //        lineDnload.FIELD020 = StringUtils.Left(line.HazmatCode, 6);
        //        lineDnload.FIELD021 = line.CustomerXRef;
        //        lineDnload.FIELD022 = StringUtils.Left(line.CommentIndicator, 1);
        //        lineDnload.FIELD023 = line.UnitPrice.ToString(CultureInfo.InvariantCulture);
        //        lineDnload.FIELD024 = StringUtils.Left(line.Weight.ToString(CultureInfo.InvariantCulture), 14);
        //        lineDnload.FIELD025 = StringUtils.Left(line.Cube.ToString(CultureInfo.InvariantCulture), 20);
        //        lineDnload.FIELD026 = StringUtils.Left(line.StoreName, 35);
        //        lineDnload.FIELD027 = StringUtils.Left(line.StoreAddress1, 35);
        //        lineDnload.FIELD028 = StringUtils.Left(line.StoreAddress2, 35);
        //        lineDnload.FIELD029 = StringUtils.Left(line.StoreCity, 35);
        //        lineDnload.FIELD030 = StringUtils.Left(line.StoreProvinceState, 3);
        //        lineDnload.FIELD031 = StringUtils.Left(line.StorePostalZip, 10);
        //        lineDnload.FIELD032 = StringUtils.Left(line.StoreCountry, 13);

        //        foreach (AttributeSet attributes in line.Attributes)
        //        {
        //            lineDnload.FIELD033 = attributes.Attribute1;
        //            lineDnload.FIELD034 = attributes.Attribute2;
        //            lineDnload.FIELD035 = attributes.Attribute3;
        //            lineDnload.FIELD036 = attributes.Attribute4;
        //            lineDnload.FIELD037 = attributes.Attribute5;
        //            lineDnload.FIELD038 = attributes.Attribute6;
        //            lineDnload.FIELD053 = attributes.Attribute7;
        //            lineDnload.FIELD054 = attributes.Attribute8;
        //            lineDnload.FIELD055 = attributes.Attribute9;
        //            lineDnload.FIELD056 = attributes.Attribute10;
        //        }

        //        lineDnload.FIELD039 = StringUtils.Left(line.SpecialInstructions1, 20);
        //        lineDnload.FIELD040 = StringUtils.Left(line.SpecialInstructions2, 20);
        //        lineDnload.FIELD041 = StringUtils.Left(line.SpecialInstructions3, 20);
        //        lineDnload.FIELD042 = StringUtils.Left(line.SpecialInstructions4, 20);
        //        lineDnload.FIELD043 = StringUtils.Left(line.SpecialInstructions5, 20);
        //        lineDnload.FIELD044 = StringUtils.Left(line.SpecialInstructions6, 20);
        //        lineDnload.FIELD045 = StringUtils.Left(line.SpecialInstructions7, 20);
        //        lineDnload.FIELD046 = StringUtils.Left(line.Extra, 20);
        //        lineDnload.FIELD048 = line.CustomerPoNumber;
        //        lineDnload.FIELD049 = line.Discount.ToString(CultureInfo.InvariantCulture);
        //        lineDnload.FIELD050 = line.RetailPrice.ToString(CultureInfo.InvariantCulture);
        //        lineDnload.FIELD051 = line.ValuePrice.ToString(CultureInfo.InvariantCulture);
        //        lineDnload.FIELD052 = StringUtils.Left(line.PriceTicketDescription, 20);
        //        lineDnload.FIELD057 = line.DaysToExpire.ToString();
        //        lineDnload.FIELD058 = line.VendorNumber;
        //        lineDnload.FIELD059 = StringUtils.Left(line.CountryOfOrigin, 6);
        //        lineDnload.FIELD060 = line.SellPrice.ToString(CultureInfo.InvariantCulture);
        //        lineDnload.FIELD061 = line.Tax1 ? "Y" : "N";
        //        lineDnload.FIELD062 = line.Tax2 ? "Y" : "N";
        //        lineDnload.FIELD063 = line.Tax3 ? "Y" : "N";
        //        lineDnload.FIELD064 = line.Tax4 ? "Y" : "N";
        //        lineDnload.FIELD065 = StringUtils.Left(line.StockItem, 1);
        //        lineDnload.FIELD066 = line.PurchaseOrderNumber;
        //        lineDnload.FIELD067 = line.PurchaseOrderLineNumber;
        //        lineDnload.FIELD068 = StringUtils.Left(line.EDPNumber, 10);
        //        lineDnload.FIELD069 = StringUtils.Left(line.Commitment.ToString(CultureInfo.InvariantCulture), 3);
        //        lineDnload.FIELD070 = StringUtils.Left(line.Extra1, 40);
        //        lineDnload.FIELD071 = StringUtils.Left(line.Extra2, 40);
        //        lineDnload.FIELD072 = StringUtils.Left(line.Extra3, 40);
        //        lineDnload.FIELD073 = StringUtils.Left(line.Extra4, 40);
        //        lineDnload.FIELD074 = StringUtils.Left(line.Extra5, 40);

        //        switch (line.VariableLengthProduct)
        //        {
        //            case VariableLengthType.NotVLP:
        //                lineDnload.FIELD075 = string.Empty;
        //                break;
        //            case VariableLengthType.VLP:
        //                lineDnload.FIELD075 = "Y";
        //                break;
        //            case VariableLengthType.ContinuousVLP:
        //                lineDnload.FIELD075 = "C";
        //                break;
        //        }

        //        switch (line.KitType)
        //        {
        //            case KitType.NotSpecified:
        //                lineDnload.FIELD076 = string.Empty;
        //                break;
        //            case KitType.BuildToStockWorkOrder:
        //                lineDnload.FIELD076 = "0";
        //                break;
        //            case KitType.BuildToOrderWorkOrder:
        //                lineDnload.FIELD076 = "1";
        //                break;
        //            case KitType.ExplodeForPicking:
        //                lineDnload.FIELD076 = "2";
        //                break;
        //            case KitType.ExplodeForPickingWithShortedComponents:
        //                lineDnload.FIELD076 = "3";
        //                break;
        //            case KitType.RecipeOnly:
        //                lineDnload.FIELD076 = "4";
        //                break;
        //            case KitType.ExplodeDownload:
        //                lineDnload.FIELD076 = "6";
        //                break;
        //        }

        //        lineDnload.FIELD077 = StringUtils.Left(line.InnerPacksize.ToString(CultureInfo.InvariantCulture), 6);
        //        lineDnload.FIELD078 = StringUtils.Left(line.MiddlePacksize.ToString(CultureInfo.InvariantCulture), 6);
        //        lineDnload.FIELD079 = StringUtils.Left(line.UnitOfMeasureTextDescription, 15);

        //        switch (line.ReservationType)
        //        {
        //            case ReservationType.NotSpeficied:
        //                lineDnload.FIELD080 = string.Empty;
        //                break;
        //            case ReservationType.SeasonalReservation:
        //                lineDnload.FIELD080 = "S";
        //                break;
        //            case ReservationType.CoreReservation:
        //                lineDnload.FIELD080 = "C";
        //                break;
        //        }

        //        lineDnload.FIELD081 = StringUtils.Left(line.CommodityCode, 10);
        //        lineDnload.FIELD082 = StringUtils.Left(line.LicensePlate, 22);
        //        lineDnload.FIELD083 = StringUtils.Left(line.PackClass, 10);
        //        lineDnload.FIELD110 = line.BackflushFlag ? "Y" : "N";
        //        lineDnload.FIELD111 = StringUtils.Left(line.Salesman, 50);
        //        lineDnload.FIELD112 = StringUtils.Left(line.IntegrationUDF1, 40); // Added for A1W 64
        //        lineDnload.FIELD113 = StringUtils.Left(line.IntegrationUDF2, 40); // Added for A1W 64
        //        lineDnload.FIELD114 = StringUtils.Left(line.IntegrationUDF3, 40); // Added for A1W 64
        //        lineDnload.FIELD115 = StringUtils.Left(line.IntegrationUDF4, 40); // Added for A1W 64
        //        lineDnload.FIELD116 = StringUtils.Left(line.IntegrationUDF5, 40); // Added for A1W 64
        //        lineDnload.FIELD117 = StringUtils.Left(line.IntegrationUDF6, 40); // Added for A1W 64
        //        lineDnload.FIELD118 = StringUtils.Left(line.IntegrationUDF7, 40); // Added for A1W 64
        //        lineDnload.FIELD119 = StringUtils.Left(line.IntegrationUDF8, 40); // Added for A1W 64
        //        lineDnload.FIELD120 = StringUtils.Left(line.IntegrationUDF9, 40); // Added for A1W 64
        //        lineDnload.FIELD121 = StringUtils.Left(line.IntegrationUDF10, 40); // Added for A1W 64
        //        lineDnload.FIELD122 = StringUtils.Left(line.IntegrationUDF11, 40); // Added for A1W 64
        //        lineDnload.FIELD123 = StringUtils.Left(line.IntegrationUDF12, 40); // Added for A1W 64
        //        lineDnload.FIELD124 = StringUtils.Left(line.IntegrationUDF13, 40); // Added for A1W 64
        //        lineDnload.FIELD125 = StringUtils.Left(line.IntegrationUDF14, 40); // Added for A1W 64
        //        lineDnload.FIELD126 = StringUtils.Left(line.IntegrationUDF15, 40); // Added for A1W 64
        //        lineDnload.FIELD127 = StringUtils.Left(line.IntegrationUDF16, 40); // Added for A1W 64
        //        lineDnload.FIELD128 = StringUtils.Left(line.IntegrationUDF17, 40); // Added for A1W 64
        //        lineDnload.FIELD129 = StringUtils.Left(line.IntegrationUDF18, 40); // Added for A1W 64
        //        lineDnload.FIELD130 = StringUtils.Left(line.IntegrationUDF19, 40); // Added for A1W 64
        //        lineDnload.FIELD131 = StringUtils.Left(line.IntegrationUDF20, 40); // Added for A1W 64
        //        lineDnload.FIELD132 = StringUtils.Left(line.IntegrationUDF21, 40); // Added for A1W 64
        //        lineDnload.FIELD133 = StringUtils.Left(line.IntegrationUDF22, 40); // Added for A1W 64
        //        lineDnload.FIELD134 = StringUtils.Left(line.IntegrationUDF23, 40); // Added for A1W 64
        //        lineDnload.FIELD135 = StringUtils.Left(line.IntegrationUDF24, 40); // Added for A1W 64
        //        lineDnload.FIELD136 = StringUtils.Left(line.IntegrationUDF25, 40); // Added for A1W 64

        //        #region Finished Good Recipes
        //        foreach (ProductionRecipe recipe in line.ProductionRecipes)
        //        {
        //            DNLOAD recipeDnload = new DNLOAD();

        //            recipeDnload.FIELD001 = "PW";
        //            recipeDnload.FIELD002 = recipe.Warehouse;
        //            recipeDnload.FIELD003 = recipe.CustomerNumber;
        //            recipeDnload.FIELD004 = recipe.OrderNumber;
        //            recipeDnload.FIELD005 = recipe.BackorderNumber;
        //            recipeDnload.FIELD006 = recipe.OrderSequenceNumber;
        //            recipeDnload.FIELD007 = recipe.WorkArea;
        //            recipeDnload.FIELD009 = recipe.ProductCode;
        //            recipeDnload.FIELD010 = recipe.PackQuantity.ToString(CultureInfo.InvariantCulture);
        //            recipeDnload.FIELD011 = StringUtils.Left(recipe.Description, 40);
        //            recipeDnload.FIELD012 = recipe.QuantityToUse.ToString(CultureInfo.InvariantCulture);
        //            recipeDnload.FIELD013 = recipe.UnitOfMeasure;
        //            recipeDnload.FIELD014 = StringUtils.Left(recipe.PrimaryLocation, 8);
        //            recipeDnload.FIELD016 = StringUtils.Left(recipe.CommentIndicator, 1);
        //            recipeDnload.FIELD027 = StringUtils.Left(recipe.ClientName, 10);
        //            recipeDnload.FIELD028 = StringUtils.Left(recipe.Extra1, 40);
        //            recipeDnload.FIELD029 = StringUtils.Left(recipe.Extra2, 40);
        //            recipeDnload.FIELD030 = StringUtils.Left(recipe.Extra3, 40);
        //            recipeDnload.FIELD031 = StringUtils.Left(recipe.Extra4, 40);
        //            recipeDnload.FIELD032 = StringUtils.Left(recipe.Extra5, 40);

        //            switch (recipe.ComponentType)
        //            {
        //                case ComponentType.CoProduct:
        //                    recipeDnload.FIELD033 = "C";
        //                    break;
        //                case ComponentType.ServiceItem:
        //                    recipeDnload.FIELD033 = "A";
        //                    break;
        //                default:
        //                    recipeDnload.FIELD033 = "S";
        //                    break;
        //            }

        //            recipeDnload.FIELD034 = recipe.QuantityBuilt.ToString(CultureInfo.InvariantCulture);
        //            recipeDnload.FIELD035 = recipe.ComponentCost.ToString(CultureInfo.InvariantCulture);
        //            recipeDnload.FIELD036 = StringUtils.Left(recipe.ComponentSpecialInstructions, 40);

        //            foreach (AttributeSet attributes in recipe.Attributes)
        //            {
        //                recipeDnload.FIELD017 = attributes.Attribute1;
        //                recipeDnload.FIELD018 = attributes.Attribute2;
        //                recipeDnload.FIELD019 = attributes.Attribute3;
        //                recipeDnload.FIELD020 = attributes.Attribute4;
        //                recipeDnload.FIELD021 = attributes.Attribute5;
        //                recipeDnload.FIELD022 = attributes.Attribute6;
        //                recipeDnload.FIELD023 = attributes.Attribute7;
        //                recipeDnload.FIELD024 = attributes.Attribute8;
        //                recipeDnload.FIELD025 = attributes.Attribute9;
        //                recipeDnload.FIELD026 = attributes.Attribute10;
        //            }

        //            dnloads.Add(recipeDnload);
        //        }
        //        #endregion

        //        dnloads.Add(lineDnload);
        //    }
        //    #endregion

        //    #region Header
        //    DNLOAD headerDnload = new DNLOAD();

        //    headerDnload.FIELD001 = "PH";
        //    headerDnload.FIELD002 = packslip.RecordType;
        //    headerDnload.FIELD003 = packslip.BatchControlNumber;
        //    headerDnload.FIELD004 = packslip.CustomerNumber;
        //    headerDnload.FIELD005 = packslip.OrderNumber;
        //    headerDnload.FIELD006 = packslip.BackOrderNumber;
        //    headerDnload.FIELD007 = packslip.CustomerPONumber;
        //    headerDnload.FIELD008 = packslip.CustomerLabel;
        //    headerDnload.FIELD009 = packslip.CrystalPackingSlip;
        //    headerDnload.FIELD010 = StringUtils.Left(packslip.CustomerGroup, 10);
        //    headerDnload.FIELD011 = packslip.LabelText;
        //    headerDnload.FIELD012 = StringUtils.Left(packslip.ShipName, 35);
        //    headerDnload.FIELD013 = StringUtils.Left(packslip.ShipAddressName1, 35);
        //    headerDnload.FIELD014 = StringUtils.Left(packslip.ShipAddressName2, 35);
        //    headerDnload.FIELD015 = StringUtils.Left(packslip.ShipCity, 35);
        //    headerDnload.FIELD016 = StringUtils.Left(packslip.ShipProviceState, 3);
        //    headerDnload.FIELD017 = StringUtils.Left(packslip.ShipPostalZipCode, 10);
        //    headerDnload.FIELD018 = StringUtils.Left(packslip.ShipCountry, 13);
        //    headerDnload.FIELD019 = StringUtils.Left(packslip.ShipAttnTo, 35);
        //    headerDnload.FIELD020 = StringUtils.Left(packslip.ShipTelephoneNumber, 25);
        //    headerDnload.FIELD021 = StringUtils.Left(packslip.BillName, 35);
        //    headerDnload.FIELD022 = StringUtils.Left(packslip.BillAddress1, 35);
        //    headerDnload.FIELD023 = StringUtils.Left(packslip.BillAddress2, 35);
        //    headerDnload.FIELD024 = StringUtils.Left(packslip.BillCity, 35);
        //    headerDnload.FIELD025 = StringUtils.Left(packslip.BillProvinceState, 3);
        //    headerDnload.FIELD026 = StringUtils.Left(packslip.BillPostalZip, 10);
        //    headerDnload.FIELD027 = StringUtils.Left(packslip.BillCoutry, 13);
        //    headerDnload.FIELD028 = packslip.ShipDateRequired == DateTime.MinValue ? string.Empty : packslip.ShipDateRequired.ToString("yyyyMMdd");
        //    headerDnload.FIELD029 = packslip.ShipTimeRequired;
        //    headerDnload.FIELD030 = StringUtils.Left(packslip.TruckRoute, 10);
        //    headerDnload.FIELD031 = StringUtils.Left(packslip.Priority, 1);

        //    switch (packslip.ShipmentPaymentType)
        //    {
        //        case ShipmentPaymentType.Collect:
        //            headerDnload.FIELD032 = "C";
        //            break;
        //        case ShipmentPaymentType.BillRecipient:
        //            headerDnload.FIELD032 = "R";
        //            break;
        //        case ShipmentPaymentType.Prepaid:
        //            headerDnload.FIELD032 = "P";
        //            break;
        //        case ShipmentPaymentType.ThirdParty:
        //            headerDnload.FIELD032 = "T";
        //            break;
        //        default:
        //            headerDnload.FIELD032 = string.Empty;
        //            break;
        //    }

        //    headerDnload.FIELD033 = StringUtils.Left(packslip.ShipViaPreferred, 15);
        //    headerDnload.FIELD034 = StringUtils.Left(packslip.ShipPayAccountNumber, 16);
        //    headerDnload.FIELD035 = packslip.OrderDate == DateTime.MinValue ? string.Empty : packslip.OrderDate.ToString("yyyyMMdd");
        //    headerDnload.FIELD036 = packslip.CancelDate == DateTime.MinValue ? string.Empty : packslip.CancelDate.ToString("yyyyMMdd");

        //    switch (packslip.PurchaseOrderType)
        //    {
        //        case PurchaseOrderType.NotSpecified:
        //            headerDnload.FIELD037 = string.Empty;
        //            break;
        //        case PurchaseOrderType.StandAlone:
        //            headerDnload.FIELD037 = "SA";
        //            break;
        //        case PurchaseOrderType.Promotional:
        //            headerDnload.FIELD037 = "PR";
        //            break;
        //        case PurchaseOrderType.CustomerSpecialOrder:
        //            headerDnload.FIELD037 = "OS";
        //            break;
        //        case PurchaseOrderType.ReOrder:
        //            headerDnload.FIELD037 = "RE";
        //            break;
        //        case PurchaseOrderType.Transfer:
        //            headerDnload.FIELD037 = "TRANSFER";
        //            break;
        //    }

        //    headerDnload.FIELD038 = StringUtils.Left(packslip.Shipper, 10); // Changed for A1W 64
        //    headerDnload.FIELD039 = packslip.ShipCompleteOnly ? "Y" : "N";
        //    headerDnload.FIELD040 = packslip.ClientMH10Number;
        //    headerDnload.FIELD041 = StringUtils.Left(packslip.ClientName, 10);
        //    headerDnload.FIELD042 = StringUtils.Left(packslip.SpecialInstructions1, 20);
        //    headerDnload.FIELD043 = StringUtils.Left(packslip.SpecialInstructions2, 20);
        //    headerDnload.FIELD044 = StringUtils.Left(packslip.SpecialInstructions3, 20);
        //    headerDnload.FIELD045 = StringUtils.Left(packslip.SpecialInstructions4, 20);
        //    headerDnload.FIELD046 = StringUtils.Left(packslip.SpecialInstructions5, 20);
        //    headerDnload.FIELD047 = StringUtils.Left(packslip.SpecialInstructions6, 20);
        //    headerDnload.FIELD048 = StringUtils.Left(packslip.SpecialInstructions7, 20);
        //    headerDnload.FIELD049 = StringUtils.Left(packslip.CostCenter, 12);
        //    headerDnload.FIELD050 = StringUtils.Left(packslip.ShipToNumber, 35);
        //    headerDnload.FIELD051 = packslip.IsCustomerAcceptsBackorders ? "Y" : "N";
        //    headerDnload.FIELD052 = StringUtils.Left(packslip.TruckStop, 5);
        //    headerDnload.FIELD053 = packslip.TruckDeliveryTime;
        //    headerDnload.FIELD054 = StringUtils.Left(packslip.ShipperName, 35);
        //    headerDnload.FIELD055 = StringUtils.Left(packslip.ShipperAddress1, 35);
        //    headerDnload.FIELD056 = StringUtils.Left(packslip.ShipperAddress2, 35);
        //    headerDnload.FIELD057 = StringUtils.Left(packslip.ShipperCity, 35);
        //    headerDnload.FIELD058 = StringUtils.Left(packslip.ShipperProvinceState, 3);
        //    headerDnload.FIELD059 = StringUtils.Left(packslip.ShipperPostalZipCode, 10);
        //    headerDnload.FIELD060 = StringUtils.Left(packslip.PackingSlip, 8);
        //    headerDnload.FIELD061 = StringUtils.Left(packslip.IncludePickZones, 10);
        //    headerDnload.FIELD062 = StringUtils.Left(packslip.ExcludePickZones, 10);
        //    headerDnload.FIELD063 = StringUtils.Left(packslip.IncludeReplenishmentZones, 10);
        //    headerDnload.FIELD064 = packslip.ShipWithOtherGoods ? "Y" : "N";
        //    headerDnload.FIELD065 = packslip.DontSplitExpiryDates ? "Y" : "N";
        //    headerDnload.FIELD066 = StringUtils.Left(packslip.ShipmentOptions, 5);
        //    headerDnload.FIELD067 = StringUtils.Left(packslip.ShipmentActions, 1);
        //    headerDnload.FIELD068 = StringUtils.Left(packslip.ShipmentMessage, 20);
        //    headerDnload.FIELD069 = StringUtils.Left(packslip.ManifestReportName, 8);
        //    headerDnload.FIELD070 = StringUtils.Left(packslip.COD, 1);
        //    headerDnload.FIELD071 = StringUtils.Left(packslip.ShipmentConsolidation, 19);
        //    headerDnload.FIELD072 = StringUtils.Left(packslip.PricingTicket, 10);
        //    headerDnload.FIELD073 = packslip.DontShipBeforeDate == DateTime.MinValue ? string.Empty : packslip.DontShipBeforeDate.ToString("yyyyMMdd");
        //    headerDnload.FIELD074 = StringUtils.Left(packslip.ShipToFaxNumber, 14);
        //    headerDnload.FIELD075 = packslip.WaveNumber;
        //    headerDnload.FIELD076 = StringUtils.Left(packslip.ReplenishmentGroup, 10);
        //    headerDnload.FIELD077 = StringUtils.Left(packslip.GiftCertificateNumber, 20);
        //    headerDnload.FIELD078 = packslip.GiftCertificateAmount.ToString(CultureInfo.InvariantCulture);
        //    headerDnload.FIELD079 = packslip.GiftCertificateAmountUsed.ToString(CultureInfo.InvariantCulture);
        //    headerDnload.FIELD080 = StringUtils.Left(packslip.BusinessTelephoneNumber, 25);
        //    headerDnload.FIELD081 = StringUtils.Left(packslip.Email, 40);
        //    headerDnload.FIELD082 = packslip.CreditCardNumber;
        //    headerDnload.FIELD083 = packslip.CreditCardExpiry;
        //    headerDnload.FIELD084 = StringUtils.Left(packslip.OrderSource, 10); // Added for A1W 64
        //    headerDnload.FIELD085 = packslip.TotalInvoiceAmmountCharge.ToString(CultureInfo.InvariantCulture);
        //    headerDnload.FIELD086 = packslip.TotalTaxToCharge1.ToString(CultureInfo.InvariantCulture);
        //    headerDnload.FIELD087 = packslip.TotalTaxToCharge2.ToString(CultureInfo.InvariantCulture);
        //    headerDnload.FIELD088 = packslip.TotalTaxToCharge3.ToString(CultureInfo.InvariantCulture);
        //    headerDnload.FIELD089 = packslip.TotalTaxToCharge4.ToString(CultureInfo.InvariantCulture);
        //    headerDnload.FIELD090 = packslip.TotalFreightToCharge.ToString(CultureInfo.InvariantCulture);
        //    headerDnload.FIELD091 = packslip.TotalShippingHandlingCharge.ToString(CultureInfo.InvariantCulture);
        //    headerDnload.FIELD092 = packslip.PromoAmount.ToString(CultureInfo.InvariantCulture);
        //    headerDnload.FIELD093 = packslip.PromoDiscount.ToString(CultureInfo.InvariantCulture);
        //    headerDnload.FIELD094 = StringUtils.Left(packslip.EndOfLineProcess.ToString(CultureInfo.InvariantCulture), 1);
        //    headerDnload.FIELD095 = StringUtils.Left(packslip.PurchaseOrderNumber, 20);
        //    headerDnload.FIELD096 = StringUtils.Left(packslip.Special1, 20);
        //    headerDnload.FIELD097 = StringUtils.Left(packslip.Special2, 20);
        //    headerDnload.FIELD098 = StringUtils.Left(packslip.Special3, 20);
        //    headerDnload.FIELD099 = packslip.MinimumDaysOfExpiry.ToString(CultureInfo.InvariantCulture);
        //    headerDnload.FIELD100 = packslip.MixedLotIndicator ? "Y" : "N";
        //    headerDnload.FIELD101 = StringUtils.Left(packslip.Extra1, 40);
        //    headerDnload.FIELD102 = StringUtils.Left(packslip.Extra2, 40);
        //    headerDnload.FIELD103 = StringUtils.Left(packslip.Extra3, 40);
        //    headerDnload.FIELD104 = StringUtils.Left(packslip.Extra4, 40);
        //    headerDnload.FIELD105 = StringUtils.Left(packslip.Extra5, 40);
        //    headerDnload.FIELD106 = StringUtils.Left(packslip.Handle, 10);
        //    headerDnload.FIELD107 = StringUtils.Left(packslip.CustomerCartonContentLabel, 8);

        //    switch (packslip.CartonType)
        //    {
        //        case CartonType.NotSpecified:
        //            headerDnload.FIELD108 = string.Empty;
        //            break;
        //        case CartonType.CartonLabel:
        //            headerDnload.FIELD108 = "0";
        //            break;
        //        case CartonType.PalletLabel:
        //            headerDnload.FIELD108 = "1";
        //            break;
        //        case CartonType.TruckLoadLabel:
        //            headerDnload.FIELD108 = "2";
        //            break;
        //    }

        //    headerDnload.FIELD109 = packslip.WarehouseTransitLocation;
        //    headerDnload.FIELD110 = packslip.WarehouseDestinationLocation;
        //    headerDnload.FIELD111 = StringUtils.Left(packslip.DestinationBinLocation, 8);
        //    headerDnload.FIELD112 = StringUtils.Left(packslip.PlannerID, 15);
        //    headerDnload.FIELD113 = StringUtils.Left(packslip.ControlBuyerID, 15);
        //    headerDnload.FIELD114 = packslip.SchedulledStartDate;
        //    headerDnload.FIELD115 = packslip.FinishedGoodItemNumber;
        //    headerDnload.FIELD116 = packslip.FinishedGoodDescription;
        //    headerDnload.FIELD117 = packslip.FinishedGoodClassID;
        //    headerDnload.FIELD118 = packslip.MOENdingQuantity;
        //    headerDnload.FIELD119 = packslip.FirstWorkCenter;
        //    headerDnload.FIELD120 = packslip.MODescription;
        //    headerDnload.FIELD121 = packslip.PrimaryBillToAddress;
        //    headerDnload.FIELD122 = packslip.WorkArea;
        //    headerDnload.FIELD123 = StringUtils.Left(packslip.AssemblyNote1, 40);
        //    headerDnload.FIELD124 = StringUtils.Left(packslip.AssemblyNote2, 40);
        //    headerDnload.FIELD125 = StringUtils.Left(packslip.AssemblyNote3, 40);
        //    headerDnload.FIELD126 = StringUtils.Left(packslip.AssemblyNote4, 40);
        //    headerDnload.FIELD127 = StringUtils.Left(packslip.AssemblyNote5, 40);
        //    headerDnload.FIELD128 = StringUtils.Left(packslip.ShipperEIN, 40);
        //    headerDnload.FIELD129 = StringUtils.Left(packslip.ShipToEIN, 40);
        //    headerDnload.FIELD130 = StringUtils.Left(packslip.IntegrationUDF1, 40); // Added for A1W 64
        //    headerDnload.FIELD131 = StringUtils.Left(packslip.IntegrationUDF2, 40); // Added for A1W 64
        //    headerDnload.FIELD132 = StringUtils.Left(packslip.IntegrationUDF3, 40); // Added for A1W 64
        //    headerDnload.FIELD133 = StringUtils.Left(packslip.IntegrationUDF4, 40); // Added for A1W 64
        //    headerDnload.FIELD134 = StringUtils.Left(packslip.IntegrationUDF5, 40); // Added for A1W 64
        //    headerDnload.FIELD135 = StringUtils.Left(packslip.IntegrationUDF6, 40); // Added for A1W 64
        //    headerDnload.FIELD136 = StringUtils.Left(packslip.IntegrationUDF7, 40); // Added for A1W 64
        //    headerDnload.FIELD137 = StringUtils.Left(packslip.IntegrationUDF8, 40); // Added for A1W 64
        //    headerDnload.FIELD138 = StringUtils.Left(packslip.IntegrationUDF9, 40); // Added for A1W 64
        //    headerDnload.FIELD139 = StringUtils.Left(packslip.IntegrationUDF10, 40); // Added for A1W 64
        //    headerDnload.FIELD140 = StringUtils.Left(packslip.IntegrationUDF11, 40); // Added for A1W 64
        //    headerDnload.FIELD141 = StringUtils.Left(packslip.IntegrationUDF12, 40); // Added for A1W 64
        //    headerDnload.FIELD142 = StringUtils.Left(packslip.IntegrationUDF13, 40); // Added for A1W 64
        //    headerDnload.FIELD143 = StringUtils.Left(packslip.IntegrationUDF14, 40); // Added for A1W 64
        //    headerDnload.FIELD144 = StringUtils.Left(packslip.IntegrationUDF15, 40); // Added for A1W 64
        //    headerDnload.FIELD145 = StringUtils.Left(packslip.IntegrationUDF16, 40); // Added for A1W 64
        //    headerDnload.FIELD146 = StringUtils.Left(packslip.IntegrationUDF17, 40); // Added for A1W 64
        //    headerDnload.FIELD147 = StringUtils.Left(packslip.IntegrationUDF18, 40); // Added for A1W 64
        //    headerDnload.FIELD148 = StringUtils.Left(packslip.IntegrationUDF19, 40); // Added for A1W 64
        //    headerDnload.FIELD149 = StringUtils.Left(packslip.IntegrationUDF20, 40); // Added for A1W 64
        //    headerDnload.FIELD150 = StringUtils.Left(packslip.IntegrationUDF21, 40); // Added for A1W 64
        //    headerDnload.FIELD151 = StringUtils.Left(packslip.IntegrationUDF22, 40); // Added for A1W 64
        //    headerDnload.FIELD152 = StringUtils.Left(packslip.IntegrationUDF23, 40); // Added for A1W 64
        //    headerDnload.FIELD153 = StringUtils.Left(packslip.IntegrationUDF24, 40); // Added for A1W 64
        //    headerDnload.FIELD154 = StringUtils.Left(packslip.IntegrationUDF25, 40); // Added for A1W 64

        //    dnloads.Add(headerDnload);
        //    #endregion

        //    return dnloads;
        //}
		#endregion

        //From WMS
        #region ParsePickConfirmation
        public static List<PickConfirmation> ParsePickConfirmation(DataTable pcRecords)
        {
            if (pcRecords.Rows.Count == 0)
                return null;

            List<PickConfirmation> PickConfirmations = new List<PickConfirmation>();

            foreach (DataRow row in pcRecords.Rows)
            {
                #region PickConfirmation mapping
                PickConfirmation result = new PickConfirmation();
                
                //3. Customer Number (16C)
                result.CustomerId = row["FIELD003"] as string;

                //4. Order Number (20C)
                result.OrderId = row["FIELD004"] as string;

                //5. Backorder Number (16C)
                result.BackOrderNum = row["FIELD005"] as string;

                //6. Store Number (10C)
                result.StoreNum = row["FIELD006"] as string;

                //26. Host Order (20C)
                result.HostOrder = row["FIELD026"] as string;

                //40. Specials Selected (20C)
                result.SpecialsSelected = row["FIELD040"] as string;

                //41. Cost Center (4C)
                result.CostCenter = row["FIELD041"] as string;

                //48. Ship to Number (35C)
                result.ShipToNum = row["FIELD048"] as string;

                //56. Client Name (16C)
                result.ClientName = row["FIELD056"] as string;

                //59. Special 1 (20C)
                result.Special1 = row["FIELD059"] as string;

                //60. Special 2 (20C)
                result.Special2 = row["FIELD060"] as string;

                //61. Special 3 (20C)
                result.Special3 = row["FIELD061"] as string;

                //64. Batch Control Number (12C)
                result.BatchControlNum = row["FIELD064"] as string;

                //72. Shipped Name (35C)
                result.ShippedName = row["FIELD072"] as string;

                //73. Shipped Address 1 (35C)
                result.ShippedAddress1 = row["FIELD073"] as string;

                //74. Shipped Address 2 (35C)
                result.ShippedAddress2 = row["FIELD074"] as string;

                //168. Shipped Address 3 (35C)
                result.ShippedAddress3 = row["FIELD168"] as string;

                //75. Shipped City (35C)
                result.ShippedCity = row["FIELD075"] as string;

                //76. Shipped Province/State (3C)
                result.ShippedProvinceState = row["FIELD076"] as string;

                //77. Shipped Postal/Zip Code (10C)
                result.ShippedPostalZip = row["FIELD077"] as string;

                //78. Shipped Country (13C)
                result.ShippedCountry = row["FIELD078"] as string;

                //79. Shipped Attention To: (35C)
                result.ShippedAttnTo = row["FIELD079"] as string;

                //80. Shipped Telephone Number (25C)
                result.ShippedPhone = row["FIELD080"] as string;

                //85. Order Date (D)
                if (row["FIELD085"].ToString().Length > 0)
                    result.RequiredDate = DateTime.ParseExact(row["FIELD085"].ToString(), "yyyyMMdd", CultureInfo.InvariantCulture);

                //86. Required Date (D)
                if (row["FIELD086"].ToString().Length > 0)
                    result.OrderDate = DateTime.ParseExact(row["FIELD086"] as string, "yyyyMMdd", CultureInfo.InvariantCulture);

                //88. Customer PO Number (18C)
                result.CustomerPoNumber = row["FIELD088"] as string;

                //89. Extra1 (40C)
                result.Extra1 = row["FIELD089"] as string;

                //90. Extra2 (40C)
                result.Extra2 = row["FIELD090"] as string;

                //91. Extra3 (40C)
                result.Extra3 = row["FIELD091"] as string;

                //92. Extra4 (40C)
                result.Extra4 = row["FIELD092"] as string;

                //93. Extra5 (40C)
                result.Extra5 = row["FIELD093"] as string;

                //103. Invoice Printer  (30C)
                result.InvoicePrinter = row["FIELD103"] as string;

                //108. Purchase Order Type (8C)
                result.POType = row["FIELD108"] as string;

                //112. Master Bill of Lading (BOL) Number (20C)
                result.MasterBOLNumber = row["FIELD112"] as string;

                //170. Shipment Consoslidation Number (20C)
                result.ShipmentConsolidationNumber = row["FIELD170"] as string;
                #endregion

                #region Shipment mapping
                Shipment shipment = new Shipment();
                //33. Shipment Number (20C)
                shipment.ShipmentId = row["FIELD033"] as string;
                if (shipment.ShipmentId == null)
                    shipment.ShipmentId = string.Empty;

                //Retrieve existent shipment or add new one if doesn't exist
                if (!result.Shipments.Contains(shipment))
                    result.Shipments.Add(shipment);
                else
                    shipment = result.Shipments[result.Shipments.IndexOf(shipment)];

                //8. Destination Warehouse (6C)
                shipment.DestinationWarehouse = row["FIELD008"] as string;

                //31. Date Shipped (D)
                if (row["FIELD031"].ToString().Length > 0)
                    shipment.DateShipped = DateTime.ParseExact(row["FIELD031"].ToString(), "yyyyMMdd", CultureInfo.InvariantCulture);

                //32. Total Weight (10.3N)
                if (row["FIELD032"].ToString().Length > 0)
                    shipment.TotalWeight = Decimal.Parse(row["FIELD032"] as string);

                //34. Shipment Carrier Used (15C)
                shipment.ShipmentCarrier = row["FIELD034"] as string;

                //36. Number of Pieces Shipped (6N)
                if (row["FIELD036"].ToString().Length > 0)
                    shipment.NumberOfPiecesShipped = int.Parse(row["FIELD036"] as string);

                //37. Ship Total Charge (9.2N)
                if (row["FIELD037"].ToString().Length > 0)
                    shipment.TotalShipmentCharge = Decimal.Parse(row["FIELD037"] as string);

                //38. Ship Payment Type (1C)
                shipment.ShipPaymentType = row["FIELD038"] as string;

                //39. Ship Rate Charged (9.2N)
                if (row["FIELD039"].ToString().Length > 0)
                    shipment.ShipRateCharged = Decimal.Parse(row["FIELD039"] as string);

                //42. Pickup Number (20C)
                shipment.PickupId = row["FIELD042"] as string;

                //45. Ship Pay Account Number (16C)
                shipment.ShipPayAccount = row["FIELD045"] as string;

                //49. Shipment Service Used (4C)
                shipment.ShipmentServiceUsed = row["FIELD049"] as string;

                //62. Truck Route (10C)
                shipment.TruckRoute = row["FIELD062"] as string;

                //63. Truck Stop (5C)
                shipment.TruckStop = row["FIELD063"] as string;

                //106. 1st User-Defined Shipping Field (20C)
                shipment.ShippingUDF1 = row["FIELD106"] as string;

                //107. 2nd User-Defined Shipping Field (20C)
                shipment.ShippingUDF2 = row["FIELD107"] as string;

                //110. Trailer # (20C)
                shipment.TrailerNum = row["FIELD110"] as string;

				// 113 - 116. WMS UDFs (20C)
				shipment.WMSUDF1 = row["FIELD113"] as string; // Added for A1W 64
				shipment.WMSUDF2 = row["FIELD114"] as string; // Added for A1W 64
				shipment.WMSUDF3 = row["FIELD115"] as string; // Added for A1W 64
				shipment.WMSUDF4 = row["FIELD116"] as string; // Added for A1W 64

				// 119 - 143. Integration UDFs (40C)
				shipment.IntegrationUDF1 = row["FIELD119"] as string; // Added for A1W 64
				shipment.IntegrationUDF2 = row["FIELD120"] as string; // Added for A1W 64
				shipment.IntegrationUDF3 = row["FIELD121"] as string; // Added for A1W 64
				shipment.IntegrationUDF4 = row["FIELD122"] as string; // Added for A1W 64
				shipment.IntegrationUDF5 = row["FIELD123"] as string; // Added for A1W 64
				shipment.IntegrationUDF6 = row["FIELD124"] as string; // Added for A1W 64
				shipment.IntegrationUDF7 = row["FIELD125"] as string; // Added for A1W 64
				shipment.IntegrationUDF8 = row["FIELD126"] as string; // Added for A1W 64
				shipment.IntegrationUDF9 = row["FIELD127"] as string; // Added for A1W 64
				shipment.IntegrationUDF10 = row["FIELD128"] as string; // Added for A1W 64
				shipment.IntegrationUDF11 = row["FIELD129"] as string; // Added for A1W 64
				shipment.IntegrationUDF12 = row["FIELD130"] as string; // Added for A1W 64
				shipment.IntegrationUDF13 = row["FIELD131"] as string; // Added for A1W 64
				shipment.IntegrationUDF14 = row["FIELD132"] as string; // Added for A1W 64
				shipment.IntegrationUDF15 = row["FIELD133"] as string; // Added for A1W 64
				shipment.IntegrationUDF16 = row["FIELD134"] as string; // Added for A1W 64
				shipment.IntegrationUDF17 = row["FIELD135"] as string; // Added for A1W 64
				shipment.IntegrationUDF18 = row["FIELD136"] as string; // Added for A1W 64
				shipment.IntegrationUDF19 = row["FIELD137"] as string; // Added for A1W 64
				shipment.IntegrationUDF20 = row["FIELD138"] as string; // Added for A1W 64
				shipment.IntegrationUDF21 = row["FIELD139"] as string; // Added for A1W 64
				shipment.IntegrationUDF22 = row["FIELD140"] as string; // Added for A1W 64
				shipment.IntegrationUDF23 = row["FIELD141"] as string; // Added for A1W 64
				shipment.IntegrationUDF24 = row["FIELD142"] as string; // Added for A1W 64
				shipment.IntegrationUDF25 = row["FIELD143"] as string; // Added for A1W 64
                #endregion

                #region Carton mapping
                Carton carton = new Carton();

                //7. Carton ASN (9C)
                carton.CartonASN = row["FIELD007"] as string;

                //Retrieve existent carton or add new one if doesn't exist
                if (!shipment.Cartons.Contains(carton))
                    shipment.Cartons.Add(carton);
                else
                    carton = shipment.Cartons[shipment.Cartons.IndexOf(carton)];

                //2. Warehouse (6C)
                carton.Warehouse = row["FIELD002"] as string;

                //9. Pallet ASN (20C)
                carton.PalletASN = row["FIELD009"] as string;

                //43. Total Cube (12.3N)
                if (row["FIELD043"].ToString().Length > 0)
                    carton.TotalCube = Decimal.Parse(row["FIELD043"] as string);

                //46. Track & Trace Number (20C)
                carton.TrackTraceNumber = row["FIELD046"] as string;

                //67. Freight & Handling Amount (11.2N)
                if (row["FIELD067"].ToString().Length > 0)
                    carton.FreightAndHandling = Decimal.Parse(row["FIELD067"] as string);

                //99. Box Size (20C)
                carton.BoxSize = row["FIELD099"] as string;

                //100. Dunnage (20C)
                carton.Dunnage = row["FIELD099"] as string;

                //101. SCC-18  (20C)
                carton.SCC18 = row["FIELD101"] as string;

                //65. Additional Barcode/Reference (30C)
                carton.AdditionalBarcodeReference = row["FIELD065"] as string;
                #endregion

                #region Carton lines
                CartonLine line = new CartonLine();
                line.RowId = row["ROWID"].ToString();
                
                //DRS 05/02/16
                //21. Complete(Y/N) (1C)
                line.IsComplete = row["FIELD021"] as string;

                //23. Tiebreaker (10N)
                line.TieBreaker = row["FIELD023"] as string;

                //10. Order Sequence (20C)
                line.LineNum = row["FIELD010"] as string;

                if (!carton.Lines.Contains(line))
                    carton.Lines.Add(line);
                else
                    line = carton.Lines[carton.Lines.IndexOf(line)];

                //11. Product Code (20C)
                line.ProductCode = row["FIELD011"] as string;

                //12. Quantity Packed - This ASN (8N)
                if (row["FIELD012"].ToString().Length > 0)
                    line.QuantityPacked += Decimal.Parse(row["FIELD012"] as string);

                //DRS 05/02/16
                //13. Quantity Packed - This Packslip (8N)
                if (row["FIELD013"].ToString().Length > 0)
                    line.QuantityForPackslip = Decimal.Parse(row["FIELD013"] as string);

                //14. UPC (14C)
                line.UPC = row["FIELD014"] as string;

                //22. Picker ID (8C)
                line.PickerId = row["FIELD022"] as string;

                //25. Product Substitute (20C)
                line.ProductSubstitute = row["FIELD025"] as string;

                //27. Quantity Ordered (8N)
                if (row["FIELD027"].ToString().Length > 0)
                    line.QuantityOrdered = Decimal.Parse(row["FIELD027"] as string);

                //28. Bin Location (8C)
                line.BinLocation = row["FIELD028"] as string;

                //54. Zone (1C)
                line.Zone = row["FIELD053"] as string;

				//55. Packsize (6N)
				if (row["FIELD055"].ToString().Length > 0)
					line.Packsize = decimal.Parse(row["FIELD055"] as string); // Changed to decimal for A1W 64

                //83. Purchase Order Number (20C)
                line.PurchaseOrderNumber = row["FIELD083"] as string;

                //84. Purchase Order Line Number (6C)
                line.PurchaseOrderLine = row["FIELD084"] as string;

                //87. Customer Part Number (15C)
                line.CustomerPartNumer = row["FIELD087"] as string;

                //94. Extra1 (40C)
                line.Extra1 = row["FIELD094"] as string;

                //95. Extra2  40C)
                line.Extra2 = row["FIELD095"] as string;

                //96. Extra3 (40C)
                line.Extra3 = row["FIELD096"] as string;

                //97. Extra4 (40C)
                line.Extra4 = row["FIELD097"] as string;

                //98. Extra5 (40C)
                line.Extra5 = row["FIELD098"] as string;

                //102. Drop-Ship Warehouse  (10C)
                line.DropShipWarehouse = row["FIELD102"] as string;

				// 117 - 118. WMS UDFs (20C)
				line.WMSUDF1 = row["FIELD117"] as string; // Added for A1W 64
				line.WMSUDF2 = row["FIELD118"] as string; // Added for A1W 64

				// 144 - 168. Integration UDFs (40C)
				line.IntegrationUDF1 = row["FIELD144"] as string; // Added for A1W 64
				line.IntegrationUDF2 = row["FIELD145"] as string; // Added for A1W 64
				line.IntegrationUDF3 = row["FIELD146"] as string; // Added for A1W 64
				line.IntegrationUDF4 = row["FIELD147"] as string; // Added for A1W 64
				line.IntegrationUDF5 = row["FIELD148"] as string; // Added for A1W 64
				line.IntegrationUDF6 = row["FIELD149"] as string; // Added for A1W 64
				line.IntegrationUDF7 = row["FIELD150"] as string; // Added for A1W 64
				line.IntegrationUDF8 = row["FIELD151"] as string; // Added for A1W 64
				line.IntegrationUDF9 = row["FIELD152"] as string; // Added for A1W 64
				line.IntegrationUDF10 = row["FIELD153"] as string; // Added for A1W 64
				line.IntegrationUDF11 = row["FIELD154"] as string; // Added for A1W 64
				line.IntegrationUDF12 = row["FIELD155"] as string; // Added for A1W 64
				line.IntegrationUDF13 = row["FIELD156"] as string; // Added for A1W 64
				line.IntegrationUDF14 = row["FIELD157"] as string; // Added for A1W 64
				line.IntegrationUDF15 = row["FIELD158"] as string; // Added for A1W 64
				line.IntegrationUDF16 = row["FIELD159"] as string; // Added for A1W 64
				line.IntegrationUDF17 = row["FIELD160"] as string; // Added for A1W 64
				line.IntegrationUDF18 = row["FIELD161"] as string; // Added for A1W 64
				line.IntegrationUDF19 = row["FIELD162"] as string; // Added for A1W 64
				line.IntegrationUDF20 = row["FIELD163"] as string; // Added for A1W 64
				line.IntegrationUDF21 = row["FIELD164"] as string; // Added for A1W 64
				line.IntegrationUDF22 = row["FIELD165"] as string; // Added for A1W 64
				line.IntegrationUDF23 = row["FIELD166"] as string; // Added for A1W 64
				line.IntegrationUDF24 = row["FIELD167"] as string; // Added for A1W 64
				line.IntegrationUDF25 = row["FIELD168"] as string; // Added for A1W 64
                #endregion

                #region Attributes
                AttributeSet set = new AttributeSet();
                set.RowId = row["ROWID"].ToString();

                //15. Attribute 1 (25C)
                set.Attribute1 = row["FIELD015"] as string;

                //16. Attribute 2 (25C)
                set.Attribute2 = row["FIELD016"] as string;

                //17. Attribute 3 (25C)
                set.Attribute3 = row["FIELD017"] as string;

                //18. Attribute 4 (25C)
                set.Attribute4 = row["FIELD018"] as string;

                //19. Attribute 5 (25C)
                set.Attribute5 = row["FIELD019"] as string;

                //20. Attribute 6 (25C)
                set.Attribute6 = row["FIELD020"] as string;

                //50. Attribute 7 (10C)
                set.Attribute7 = row["FIELD050"] as string;

                //51. Attribute 8 (10C)
                set.Attribute8 = row["FIELD051"] as string;

                //52. Attribute 9 (10C)
                set.Attribute9 = row["FIELD052"] as string;

                //53. Attribute 10 (10C)
                set.Attribute10 = row["FIELD053"] as string;

                if (!set.Empy && !line.Attributes.Contains(set))
                    line.Attributes.Add(set);
                else if (!set.Empy)
                    set = line.Attributes[line.Attributes.IndexOf(set)];

                //12. Quantity (N)
				if (row["FIELD012"].ToString().Length > 0)
					set.Quantity += Decimal.Parse(row["FIELD012"] as string);
                #endregion

               PickConfirmations.Add(result);
            }

            return PickConfirmations;
        }
        #endregion

		#region A1W Middle Tier DTO to PickConfirmation
        //public static PickConfirmation DTOToPickConfirmation(IList<UPLOAD> uploads)
        //{
        //    PickConfirmation pick = new PickConfirmation();

        //    foreach (UPLOAD upload in uploads)
        //    {
        //        #region PickConfirmation mapping
        //        pick.CustomerId = upload.FIELD003;
        //        pick.OrderId = upload.FIELD004;
        //        pick.BackOrderNum = upload.FIELD005;
        //        pick.StoreNum = upload.FIELD006;
        //        pick.HostOrder = upload.FIELD026;
        //        pick.SpecialsSelected = upload.FIELD040;
        //        pick.CostCenter = upload.FIELD041;
        //        pick.ShipToNum = upload.FIELD048;
        //        pick.ClientName = upload.FIELD056;
        //        pick.Special1 = upload.FIELD059;
        //        pick.Special2 = upload.FIELD060;
        //        pick.Special3 = upload.FIELD061;
        //        pick.BatchControlNum = upload.FIELD064;
        //        pick.ShippedName = upload.FIELD072;
        //        pick.ShippedAddress1 = upload.FIELD073;
        //        pick.ShippedAddress2 = upload.FIELD074;
        //        pick.ShippedCity = upload.FIELD075;
        //        pick.ShippedProvinceState = upload.FIELD076;
        //        pick.ShippedPostalZip = upload.FIELD077;
        //        pick.ShippedCountry = upload.FIELD078;
        //        pick.ShippedAttnTo = upload.FIELD079;
        //        pick.ShippedPhone = upload.FIELD080;

        //        if (upload.FIELD085.Length > 0)
        //            pick.RequiredDate = DateTime.ParseExact(upload.FIELD085, "yyyyMMdd", CultureInfo.InvariantCulture);

        //        if (upload.FIELD086.Length > 0)
        //            pick.OrderDate = DateTime.ParseExact(upload.FIELD086, "yyyyMMdd", CultureInfo.InvariantCulture);

        //        pick.CustomerPoNumber = upload.FIELD088;
        //        pick.Extra1 = upload.FIELD089;
        //        pick.Extra2 = upload.FIELD090;
        //        pick.Extra3 = upload.FIELD091;
        //        pick.Extra4 = upload.FIELD092;
        //        pick.Extra5 = upload.FIELD093;
        //        pick.InvoicePrinter = upload.FIELD103;
        //        pick.POType = upload.FIELD108;
        //        #endregion

        //        #region Shipment mapping
        //        Shipment shipment = new Shipment();

        //        shipment.ShipmentId = upload.FIELD033;

        //        if (shipment.ShipmentId == null)
        //            shipment.ShipmentId = string.Empty;

        //        if (!pick.Shipments.Contains(shipment))
        //            pick.Shipments.Add(shipment);
        //        else
        //            shipment = pick.Shipments[pick.Shipments.IndexOf(shipment)];

        //        shipment.DestinationWarehouse = upload.FIELD008;

        //        if (upload.FIELD031.Length > 0)
        //            shipment.DateShipped = DateTime.ParseExact(upload.FIELD031, "yyyyMMdd", CultureInfo.InvariantCulture);

        //        if (upload.FIELD032.Length > 0)
        //            shipment.TotalWeight = Decimal.Parse(upload.FIELD032);

        //        shipment.ShipmentCarrier = upload.FIELD034;

        //        if (upload.FIELD037.Length > 0)
        //            shipment.TotalShipmentCharge = Decimal.Parse(upload.FIELD037);

        //        shipment.ShipPaymentType = upload.FIELD038;

        //        if (upload.FIELD039.Length > 0)
        //            shipment.ShipRateCharged = Decimal.Parse(upload.FIELD039);

        //        shipment.PickupId = upload.FIELD042;
        //        shipment.ShipPayAccount = upload.FIELD045;
        //        shipment.ShipmentServiceUsed = upload.FIELD049;
        //        shipment.TruckRoute = upload.FIELD062;
        //        shipment.TruckStop = upload.FIELD063;
        //        shipment.ShippingUDF1 = upload.FIELD106;
        //        shipment.ShippingUDF2 = upload.FIELD107;
        //        shipment.TrailerNum = upload.FIELD110;
        //        shipment.WMSUDF1 = upload.FIELD113; // Added for A1W 64
        //        shipment.WMSUDF2 = upload.FIELD114; // Added for A1W 64
        //        shipment.WMSUDF3 = upload.FIELD115; // Added for A1W 64
        //        shipment.WMSUDF4 = upload.FIELD116; // Added for A1W 64
        //        shipment.IntegrationUDF1 = upload.FIELD119; // Added for A1W 64
        //        shipment.IntegrationUDF2 = upload.FIELD120; // Added for A1W 64
        //        shipment.IntegrationUDF3 = upload.FIELD121; // Added for A1W 64
        //        shipment.IntegrationUDF4 = upload.FIELD122; // Added for A1W 64
        //        shipment.IntegrationUDF5 = upload.FIELD123; // Added for A1W 64
        //        shipment.IntegrationUDF6 = upload.FIELD124; // Added for A1W 64
        //        shipment.IntegrationUDF7 = upload.FIELD125; // Added for A1W 64
        //        shipment.IntegrationUDF8 = upload.FIELD126; // Added for A1W 64
        //        shipment.IntegrationUDF9 = upload.FIELD127; // Added for A1W 64
        //        shipment.IntegrationUDF10 = upload.FIELD128; // Added for A1W 64
        //        shipment.IntegrationUDF11 = upload.FIELD129; // Added for A1W 64
        //        shipment.IntegrationUDF12 = upload.FIELD130; // Added for A1W 64
        //        shipment.IntegrationUDF13 = upload.FIELD131; // Added for A1W 64
        //        shipment.IntegrationUDF14 = upload.FIELD132; // Added for A1W 64
        //        shipment.IntegrationUDF15 = upload.FIELD133; // Added for A1W 64
        //        shipment.IntegrationUDF16 = upload.FIELD134; // Added for A1W 64
        //        shipment.IntegrationUDF17 = upload.FIELD135; // Added for A1W 64
        //        shipment.IntegrationUDF18 = upload.FIELD136; // Added for A1W 64
        //        shipment.IntegrationUDF19 = upload.FIELD137; // Added for A1W 64
        //        shipment.IntegrationUDF20 = upload.FIELD138; // Added for A1W 64
        //        shipment.IntegrationUDF21 = upload.FIELD139; // Added for A1W 64
        //        shipment.IntegrationUDF22 = upload.FIELD140; // Added for A1W 64
        //        shipment.IntegrationUDF23 = upload.FIELD141; // Added for A1W 64
        //        shipment.IntegrationUDF24 = upload.FIELD142; // Added for A1W 64
        //        shipment.IntegrationUDF25 = upload.FIELD143; // Added for A1W 64
        //        #endregion

        //        #region Carton mapping
        //        Carton carton = new Carton();

        //        carton.CartonASN = upload.FIELD007;

        //        if (!shipment.Cartons.Contains(carton))
        //            shipment.Cartons.Add(carton);
        //        else
        //            carton = shipment.Cartons[shipment.Cartons.IndexOf(carton)];

        //        carton.Warehouse = upload.FIELD002;
        //        carton.PalletASN = upload.FIELD009;

        //        if (upload.FIELD043.Length > 0)
        //            carton.TotalCube = Decimal.Parse(upload.FIELD043);

        //        carton.TrackTraceNumber = upload.FIELD046;

        //        if (upload.FIELD067.Length > 0)
        //            carton.FreightAndHandling = Decimal.Parse(upload.FIELD067);

        //        carton.BoxSize = upload.FIELD099;
        //        carton.Dunnage = upload.FIELD099;
        //        carton.SCC18 = upload.FIELD101;
        //        carton.AdditionalBarcodeReference = upload.FIELD065;
        //        #endregion

        //        #region Carton lines
        //        CartonLine line = new CartonLine();

        //        line.RowId = upload.ROWID.ToString();
        //        line.TieBreaker = upload.FIELD023;
        //        line.LineNum = upload.FIELD010;

        //        if (!carton.Lines.Contains(line))
        //            carton.Lines.Add(line);
        //        else
        //            line = carton.Lines[carton.Lines.IndexOf(line)];

        //        line.ProductCode = upload.FIELD011;

        //        if (upload.FIELD012.Length > 0)
        //            line.QuantityPacked += Decimal.Parse(upload.FIELD012);

        //        line.UPC = upload.FIELD014;
        //        line.PickerId = upload.FIELD022;
        //        line.ProductSubstitute = upload.FIELD025;

        //        if (upload.FIELD027.Length > 0)
        //            line.QuantityOrdered = Decimal.Parse(upload.FIELD027);

        //        line.BinLocation = upload.FIELD028;
        //        line.Zone = upload.FIELD053;

        //        if (upload.FIELD055.Length > 0)
        //            line.Packsize = decimal.Parse(upload.FIELD055); // Changed to decimal for A1W 64

        //        line.PurchaseOrderNumber = upload.FIELD083;
        //        line.PurchaseOrderNumber = upload.FIELD084;
        //        line.CustomerPartNumer = upload.FIELD087;
        //        line.Extra1 = upload.FIELD094;
        //        line.Extra2 = upload.FIELD095;
        //        line.Extra3 = upload.FIELD096;
        //        line.Extra4 = upload.FIELD097;
        //        line.Extra5 = upload.FIELD098;
        //        line.DropShipWarehouse = upload.FIELD102;
        //        line.WMSUDF1 = upload.FIELD117; // Added for A1W 64
        //        line.WMSUDF2 = upload.FIELD118; // Added for A1W 64
        //        line.IntegrationUDF1 = upload.FIELD144; // Added for A1W 64
        //        line.IntegrationUDF2 = upload.FIELD145; // Added for A1W 64
        //        line.IntegrationUDF3 = upload.FIELD146; // Added for A1W 64
        //        line.IntegrationUDF4 = upload.FIELD147; // Added for A1W 64
        //        line.IntegrationUDF5 = upload.FIELD148; // Added for A1W 64
        //        line.IntegrationUDF6 = upload.FIELD149; // Added for A1W 64
        //        line.IntegrationUDF7 = upload.FIELD150; // Added for A1W 64
        //        line.IntegrationUDF8 = upload.FIELD151; // Added for A1W 64
        //        line.IntegrationUDF9 = upload.FIELD152; // Added for A1W 64
        //        line.IntegrationUDF10 = upload.FIELD153; // Added for A1W 64
        //        line.IntegrationUDF11 = upload.FIELD154; // Added for A1W 64
        //        line.IntegrationUDF12 = upload.FIELD155; // Added for A1W 64
        //        line.IntegrationUDF13 = upload.FIELD156; // Added for A1W 64
        //        line.IntegrationUDF14 = upload.FIELD157; // Added for A1W 64
        //        line.IntegrationUDF15 = upload.FIELD158; // Added for A1W 64
        //        line.IntegrationUDF16 = upload.FIELD159; // Added for A1W 64
        //        line.IntegrationUDF17 = upload.FIELD160; // Added for A1W 64
        //        line.IntegrationUDF18 = upload.FIELD161; // Added for A1W 64
        //        line.IntegrationUDF19 = upload.FIELD162; // Added for A1W 64
        //        line.IntegrationUDF20 = upload.FIELD163; // Added for A1W 64
        //        line.IntegrationUDF21 = upload.FIELD164; // Added for A1W 64
        //        line.IntegrationUDF22 = upload.FIELD165; // Added for A1W 64
        //        line.IntegrationUDF23 = upload.FIELD166; // Added for A1W 64
        //        line.IntegrationUDF24 = upload.FIELD167; // Added for A1W 64
        //        line.IntegrationUDF25 = upload.FIELD168; // Added for A1W 64
        //        #endregion

        //        #region Attributes
        //        AttributeSet set = new AttributeSet();

        //        set.RowId = upload.ROWID.ToString();

        //        set.Attribute1 = upload.FIELD015;
        //        set.Attribute2 = upload.FIELD016;
        //        set.Attribute3 = upload.FIELD017;
        //        set.Attribute4 = upload.FIELD018;
        //        set.Attribute5 = upload.FIELD019;
        //        set.Attribute6 = upload.FIELD020;
        //        set.Attribute7 = upload.FIELD050;
        //        set.Attribute8 = upload.FIELD051;
        //        set.Attribute9 = upload.FIELD052;
        //        set.Attribute10 = upload.FIELD053;

        //        if (!set.Empy && !line.Attributes.Contains(set))
        //            line.Attributes.Add(set);
        //        else if (!set.Empy)
        //            set = line.Attributes[line.Attributes.IndexOf(set)];

        //        if (upload.FIELD012.Length > 0)
        //            set.Quantity += Decimal.Parse(upload.FIELD012);
        //        #endregion
        //    }

        //    return pick;
        //}
		#endregion
    }
}
