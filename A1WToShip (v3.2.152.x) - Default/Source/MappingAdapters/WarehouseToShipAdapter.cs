using Accellos.Ship.MT.Common.Constants;
using Accellos.Ship.MT.Common.DTO;
using Accellos.Warehouse.MT.Client.DotNet;
using Accellos.Warehouse.MT.Common.Constants;
using Accellos.Warehouse.MT.Common.Criteria;
using Accellos.Warehouse.MT.Common.DTO;
using HighJump.Integrations.IntegrationUtils;
using HighJump.Integrations.ShipUtils;
using HighJump.Integrations.ShipUtils.Adapters;
using HighJump.Integrations.WarehouseUtils;
using HighJump.Integrations.WarehouseUtils.Adapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighJump.Integrations.WarehouseToShip.MappingAdapters
{
    public static class WarehouseToShipAdapter
    {
        #region ReadFromWarehouse
        public static IList<TOTMASTR> ReadFromWarehouse(string uniqueIdentifier, TotMastrActions totMastrAction, UniqueIdentifierType uniqueIdentifierType, bool isForWarehouseWriteBack)
        {
            IList<TOTMASTR> totMastrs = null;
            string uniqueIdentifierFieldName = string.Empty;
            WarehouseSyncClient warehouseSyncClient = WarehouseConnectionAdapter.GetNewWarehouseSyncClient();

            try
            {
                switch (uniqueIdentifierType)
                {
                    case UniqueIdentifierType.CartonASN:
                        uniqueIdentifierFieldName = "CARTON_ASN";
                        break;
                    case UniqueIdentifierType.OrderNumber:
                        uniqueIdentifierFieldName = "PACKSLIP";
                        break;
                    case UniqueIdentifierType.ShipmentNumber:
                        uniqueIdentifierFieldName = "SHP_CNSLDT";
                        break;
                    default:
                        throw new Exception(string.Format("No records could be loaded from TOTMASTR because the unique identifier type {0} is invalid!", uniqueIdentifierType.ToString("G")));
                }

                #region Handle Manual Shipment By Carton ASN
                if (totMastrAction == TotMastrActions.ShipmentDataSync && !isForWarehouseWriteBack)
                {
                    IList<TOTMASTR> tempTotMastrs = warehouseSyncClient.FindAllTOTMASTRPaged(TOTMASTRHydration.None,
                        string.Format("TOTMASTR.Where(item => item.{0} == \"{1}\" && item.ACTION == {2}).OrderBy(param => param.CARTON_ASN).Take(1)",
                        uniqueIdentifierFieldName, uniqueIdentifier, totMastrAction.ToString("D")));

                    if (tempTotMastrs == null || tempTotMastrs.Count == 0)
                        return null;

                    uniqueIdentifierFieldName = "SHP_CNSLDT";
                    uniqueIdentifier = tempTotMastrs[0].SHP_CNSLDT;
                }
                #endregion

                string queryString = null;

                if (totMastrAction == TotMastrActions.ShipmentDataSync)
                    queryString = string.Format("TOTMASTR.Where(item => item.{0} == \"{1}\" && item.ACTION == {2}).OrderBy(param => param.CARTON_ASN).Take({3})",
                        uniqueIdentifierFieldName, uniqueIdentifier, totMastrAction.ToString("D"), WarehouseIntegrationConfig.Instance.PageSize);
                else
                    queryString = string.Format("TOTMASTR.Where(item => item.{0} == \"{1}\" && item.ACTION == {2} && item.SHIP_SYS == \"A\").OrderBy(param => param.CARTON_ASN).Take({3})",
                        uniqueIdentifierFieldName, uniqueIdentifier, totMastrAction.ToString("D"), WarehouseIntegrationConfig.Instance.PageSize);

                totMastrs = warehouseSyncClient.FindAllTOTMASTRPaged(TOTMASTRHydration.None, queryString);

                int i = 0;

                while (totMastrs != null && totMastrs.Count > 0)
                {
                    i++;

                    IList<TOTMASTR> remainingTotMastrs = warehouseSyncClient.FindAllTOTMASTRPaged(TOTMASTRHydration.None,
                        string.Format("{0}.Skip({1})", queryString, (i * WarehouseIntegrationConfig.Instance.PageSize)));

                    if (remainingTotMastrs == null || remainingTotMastrs.Count == 0)
                        break;

                    foreach (TOTMASTR remainingTotMastr in remainingTotMastrs)
                        totMastrs.Add(remainingTotMastr);
                }

                #region Check All Picked
                if (totMastrAction == TotMastrActions.ShipmentDataSync && !isForWarehouseWriteBack)
                {
                    bool isAllPicked = false;
                    List<string> packslipNumbers = totMastrs.Select(t => t.PACKSLIP).ToList();

                    foreach (string packslipNumber in packslipNumbers)
                    {
                        IList<PickHead> tempPickHeads = warehouseSyncClient.FindAllPickHeadPaged(PickHeadHydration.None,
                            string.Format("PickHead.Where(item => item.PACKSLIP == \"{0}\" && item.ALL_PICKED == \"1\").Take(1)", packslipNumber));

                        isAllPicked = tempPickHeads != null && tempPickHeads.Count > 0;

                        if (!isAllPicked)
                            return null;
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                if (warehouseSyncClient != null)
                {
                    warehouseSyncClient.Close();
                    warehouseSyncClient = null;
                }
            }

            return totMastrs;
        }
        #endregion

        #region ReadUnusedFromWarehouse
        public static IList<TOTMASTR> ReadUnusedFromWarehouse(string shpCnsldtNumber)
        {
            IList<TOTMASTR> totMastrs = null;
            WarehouseSyncClient warehouseSyncClient = WarehouseConnectionAdapter.GetNewWarehouseSyncClient();

            try
            {
                totMastrs = warehouseSyncClient.FindAllTOTMASTRPaged(TOTMASTRHydration.None,
                    string.Format("TOTMASTR.Where(item => item.SHP_CNSLDT == \"{0}\" && item.ACTION == {1} && (item.ACCUM_QTY == null || item.ACCUM_QTY == 0)).OrderBy(param => param.ROWID).Take({2})",
                    shpCnsldtNumber, TotMastrActions.ShipmentDataSync.ToString("D"), WarehouseIntegrationConfig.Instance.PageSize));
                int i = 0;

                while (totMastrs != null && totMastrs.Count > 0)
                {
                    i++;

                    IList<TOTMASTR> remainingTotMastrs = warehouseSyncClient.FindAllTOTMASTRPaged(TOTMASTRHydration.None,
                        string.Format("TOTMASTR.Where(item => item.SHP_CNSLDT == \"{0}\" && item.ACTION == {1} && (item.ACCUM_QTY == null || item.ACCUM_QTY == 0)).OrderBy(param => param.ROWID).Take({2}).Skip({3})",
                        shpCnsldtNumber, TotMastrActions.ShipmentDataSync.ToString("D"), WarehouseIntegrationConfig.Instance.PageSize, (i * WarehouseIntegrationConfig.Instance.PageSize)));

                    if (remainingTotMastrs == null || remainingTotMastrs.Count == 0)
                        break;

                    foreach (TOTMASTR remainingTotMastr in remainingTotMastrs)
                        totMastrs.Add(remainingTotMastr);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                if (warehouseSyncClient != null)
                {
                    warehouseSyncClient.Close();
                    warehouseSyncClient = null;
                }
            }

            return totMastrs;
        }
        #endregion

        #region ConvertToShipDTO (Shipment)
        public static Shipment ConvertToShipDTO(TOTMASTR totMastr, IList<Container> containers)
        {
            #region Load External Data
            IList<PickHead> pickHeads = null;
            WarehouseSyncClient warehouseSyncClient = WarehouseConnectionAdapter.GetNewWarehouseSyncClient();

            try
            {
                pickHeads = warehouseSyncClient.FindPickHeadsByCriteriaPaged(new PickHeadsByCriteria() { PACKSLIP = totMastr.PACKSLIP }, PickHeadHydration.None, 0, 1);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                if (warehouseSyncClient != null)
                {
                    warehouseSyncClient.Close();
                    warehouseSyncClient = null;
                }
            }
            #endregion

            Shipment shipment = new Shipment();

            shipment.AccountNumber = StringUtils.Left(totMastr.SHIP_ACCT, 50);
            shipment.BOLNumber = StringUtils.Left(totMastr.PICKUP_NUM, 50);

            #region Containers
            if (containers == null || containers.Count == 0)
                throw new Exception(string.Format("There are no valid containers associated with Warehouse shipment {0}, so the shipment will not be sent to Ship!", totMastr.SHP_CNSLDT));

            shipment.ContainerCount = containers.Count;
            shipment.Containers = new List<Container>(containers);
            shipment.WeightUOM = StringUtils.Left(containers[0].UOM.Name, 10);

            foreach (Container container in containers)
                container.Shipment = shipment;
            #endregion

            shipment.DeliverBeforeDate = totMastr.DATE_REQD;

            if (containers[0].ContainerCIs != null && containers[0].ContainerCIs.Count > 0)
                shipment.Description = StringUtils.Left(containers[0].ContainerCIs[0].CIDescription, 50);

            shipment.DocumentNumber = StringUtils.Left(totMastr.PACKSLIP, 30);
            shipment.EmailAddresses = totMastr.E_MAIL; // varchar(max)
            shipment.IdentityField = StringUtils.Left(totMastr.SHP_CNSLDT, 255);
            shipment.IntegrationTypeId = (int)ShipIntegrationType.AccellosOneWarehouse;

            #region LTL Accessorials
            StringBuilder ltlAccessorials = new StringBuilder();

            foreach (char shipOption in totMastr.SHP_OPTION.ToCharArray())
            {
                string shipLTLAccessorial = ShipXMLMapsAdapter.GetShipLTLAccessorial(shipOption.ToString());

                if (!string.IsNullOrWhiteSpace(shipLTLAccessorial))
                    ltlAccessorials.AppendFormat("{0}||", shipLTLAccessorial);
            }

            if (ltlAccessorials.Length > 0)
                shipment.LtlAccessories = StringUtils.Left(ltlAccessorials.Remove(ltlAccessorials.Length - 2, 2).ToString(), 1024);
			#endregion

			if (IsAmazonOrder(totMastr.SHP_OPTION))
				shipment.OrderId = StringUtils.Left(totMastr.CUST_ORDER, 30);

			shipment.PaymentTerms = StringUtils.Left(ShipXMLMapsAdapter.GetShipPaymentTermsCode(string.IsNullOrWhiteSpace(totMastr.SHIP_PAY) ? string.Empty : totMastr.SHIP_PAY), 25);
            shipment.PrintCarrierLabel = true; // Currently always set to true
            shipment.ProNumber = StringUtils.Left(totMastr.SHIP_NUM, 50);
            shipment.ReasonForExport = StringUtils.Left(ShipIntegrationConfig.Instance.DefaultReasonForExport, 25);

            #region ReturnAddress
            if (WarehouseIntegrationConfig.Instance.UseCustomerAddressForReturns)
            {
                shipment.ReturnAddress = new ShipAddress();
                shipment.ReturnAddress.Address1 = StringUtils.Left(totMastr.CUST_ADD1, 100);
                shipment.ReturnAddress.Address2 = StringUtils.Left(totMastr.CUST_ADD2, 100);
                shipment.ReturnAddress.Address3 = StringUtils.Left(totMastr.CUST_ADD3, 100);
                shipment.ReturnAddress.City = StringUtils.Left(totMastr.CUST_CITY, 50);
                shipment.ReturnAddress.Company = StringUtils.Left(totMastr.CUST_NAME, 50);
                shipment.ReturnAddress.Contact = StringUtils.Left(totMastr.CUST_ATTN, 100);
                shipment.ReturnAddress.State = StringUtils.Left(totMastr.CUST_PROV, 3);
                shipment.ReturnAddress.ZipCode = StringUtils.Left(totMastr.CUST_ZIP, 50);
            }
            else //if (!string.IsNullOrWhiteSpace(totMastr.CUST_NUM))
                shipment.ReturnAddress =
                    ShipXMLMapsAdapter.GetShipAlternateReturnAddress(string.IsNullOrWhiteSpace(totMastr.CLIENTNAME) ? string.Empty : totMastr.CLIENTNAME,
                    string.IsNullOrWhiteSpace(totMastr.CUST_NUM) ? string.Empty : totMastr.CUST_NUM);
            #endregion

            #region Services
            if (!string.IsNullOrWhiteSpace(totMastr.SHIP_VIA))// && !string.IsNullOrWhiteSpace(totMastr.SHIP_SERVC))
            {
                shipment.CarrierServiceCode = StringUtils.Left(string.Format("{0};{1}", totMastr.SHIP_VIA, totMastr.SHIP_SERVC), 50);
                shipment.CarrierServiceCode = shipment.CarrierServiceCode.EndsWith(";") ? shipment.CarrierServiceCode.Substring(0, shipment.CarrierServiceCode.Length - 1) : shipment.CarrierServiceCode;
                shipment.Services = new List<string>();
                shipment.Services.Add(shipment.CarrierServiceCode);
            }
            #endregion

            #region Ship Date
            DateTime now = DateTime.Now;

            shipment.ShipDate = DateTime.SpecifyKind(totMastr.DATE_SHIP ?? now, DateTimeKind.Local);

            if (ShipIntegrationConfig.Instance.ShipDateCarrierWaiver && shipment.ShipDate < now)
                shipment.ShipDate = DateTime.SpecifyKind(now, DateTimeKind.Local);

            shipment.ShipDate = DateTime.SpecifyKind(shipment.ShipDate.Value.Date, DateTimeKind.Utc);
            #endregion

            shipment.ShipFromAddress = null;

            #region ShipmentLevelOptions
            shipment.ShipmentLevelOptions = new ShipmentLevelOptions();

			if (!string.IsNullOrWhiteSpace(totMastr.SHP_OPTION) && totMastr.SHP_OPTION.Contains("5"))
				shipment.ShipmentLevelOptions.AncillaryEndorsementType = (int)AncillaryEndorsementType.NoEndorsement;
			else if (!string.IsNullOrWhiteSpace(totMastr.SHP_OPTION) && totMastr.SHP_OPTION.Contains("6"))
				shipment.ShipmentLevelOptions.AncillaryEndorsementType = (int)AncillaryEndorsementType.CarrierLeaveIfNoResponse;
			else if (!string.IsNullOrWhiteSpace(totMastr.SHP_OPTION) && totMastr.SHP_OPTION.Contains("7"))
				shipment.ShipmentLevelOptions.AncillaryEndorsementType = (int)AncillaryEndorsementType.AddressServiceRequested;
			else if (!string.IsNullOrWhiteSpace(totMastr.SHP_OPTION) && totMastr.SHP_OPTION.Contains("8"))
				shipment.ShipmentLevelOptions.AncillaryEndorsementType = (int)AncillaryEndorsementType.ForwardingServiceRequested;
			else if (!string.IsNullOrWhiteSpace(totMastr.SHP_OPTION) && totMastr.SHP_OPTION.Contains("9"))
				shipment.ShipmentLevelOptions.AncillaryEndorsementType = (int)AncillaryEndorsementType.ReturnServiceRequested;
			else if (!string.IsNullOrWhiteSpace(totMastr.SHP_OPTION) && totMastr.SHP_OPTION.Contains("0"))
				shipment.ShipmentLevelOptions.AncillaryEndorsementType = (int)AncillaryEndorsementType.ChangeServiceRequested;

			shipment.ShipmentLevelOptions.CurrencyCode = ShipIntegrationConfig.Instance.DefaultCurrencyCode;
			shipment.ShipmentLevelOptions.DeclaredValueCustoms = (double)shipment.Containers
				.Where(c => c.ContainerCIs != null && c.ContainerCIs.Count > 0)
				.SelectMany(c => c.ContainerCIs)
				.Sum(c => (c.UnitValue ?? 0m) * (c.Quantity ?? 0m));
			shipment.ShipmentLevelOptions.DimensionUOM = ShipIntegrationConfig.Instance.DimensionUnitOfMeasure;
			shipment.ShipmentLevelOptions.InsideDelivery = (!string.IsNullOrWhiteSpace(totMastr.SHP_OPTION) && totMastr.SHP_OPTION.Contains("E")) ? true : false;

			shipment.ShipmentLevelOptions.IsAlcohol = (!string.IsNullOrWhiteSpace(totMastr.SHP_OPTION) && totMastr.SHP_OPTION.Contains("W")) ? true : false;

			if (shipment.ShipmentLevelOptions.IsAlcohol)
				shipment.ShipmentLevelOptions.AlcoholRecipientType = 1; // 1 = Consumer

			shipment.ShipmentLevelOptions.LiftgateDelivery = (!string.IsNullOrWhiteSpace(totMastr.SHP_OPTION) && totMastr.SHP_OPTION.Contains("L")) ? true : false;
            shipment.ShipmentLevelOptions.Residential = (!string.IsNullOrWhiteSpace(totMastr.SHP_OPTION) && totMastr.SHP_OPTION.Contains("R")) ? true : false;
            shipment.ShipmentLevelOptions.ReturnDeliveryMethod = (!string.IsNullOrWhiteSpace(totMastr.SHP_OPTION) && totMastr.SHP_OPTION.Contains("M")) ? 5 : 0; // 5 = print labels
            shipment.ShipmentLevelOptions.SaturdayDelivery = (!string.IsNullOrWhiteSpace(totMastr.SHP_OPTION) && totMastr.SHP_OPTION.Contains("B")) ? true : false;
            shipment.ShipmentLevelOptions.CardForPickup = (!string.IsNullOrWhiteSpace(totMastr.SHP_OPTION) && totMastr.SHP_OPTION.Contains("4")) ? true : false;
            // New 2.1 option - "Do Not Safe Drop" shipment.ShipmentLevelOptions. = (!string.IsNullOrWhiteSpace(totMastr.SHP_OPTION) && totMastr.SHP_OPTION.Contains("Q")) ? true : false;
            #endregion

            if (totMastr.ACTION.HasValue && totMastr.ACTION == (short)TotMastrActions.PreRateOrder)
                shipment.ShipmentNumber = "PRERATE"; // For other actions, shipment number is auto-generated by Ship
            else if (!string.IsNullOrWhiteSpace(totMastr.SHIPMENT))
                shipment.ShipmentNumber = totMastr.SHIPMENT;

			#region ShipmentReferenceFields
			//shipment.ShipmentReferenceFields = new ShipmentReferenceFields();
			//shipment.ShipmentReferenceFields.LocationReference = "";
			#endregion

			shipment.Shipper = StringUtils.Left(string.IsNullOrWhiteSpace(totMastr.SHIPPER) ? ShipIntegrationConfig.Instance.DefaultShipper : totMastr.SHIPPER, 50);

            #region ShipToAddress
            if (!string.IsNullOrWhiteSpace(totMastr.SHIP_ADD1))
            {
                shipment.ShipToAddress = new ShipAddress()
                {
                    Address1 = StringUtils.Left(totMastr.SHIP_ADD1, 100),
                    Address2 = StringUtils.Left(totMastr.SHIP_ADD2, 100),
                    Address3 = StringUtils.Left(totMastr.SHIP_ADD3, 100),
                    City = StringUtils.Left(totMastr.SHIP_CITY, 50),
                    Company = StringUtils.Left(totMastr.SHIP_NAME, 50),
                    Contact = StringUtils.Left(totMastr.SHIP_ATTN, 100),
                    Country = StringUtils.Left(totMastr.SHIP_CNTRY, 50),
                    EmailAddresses = totMastr.E_MAIL, // varchar(max) in Ship DB
                    Phone = StringUtils.Left(totMastr.SHIP_TELNO, 50),
                    State = StringUtils.Left(totMastr.SHIP_PROV, 3),
                    ZipCode = StringUtils.Left(totMastr.SHIP_ZIP, 50)
                };
            }
            #endregion

            shipment.ShipToHold = totMastr.ACTION.HasValue && totMastr.ACTION == (short)TotMastrActions.PrePrintCartonLabel ? true : false;

            #region ThirdPartyAddress
            if (!string.IsNullOrWhiteSpace(totMastr.BILL_ADD1))
            {
                shipment.ThirdPartyAddress = new ShipAddress()
                {
                    Address1 = StringUtils.Left(totMastr.BILL_ADD1, 100),
                    Address2 = StringUtils.Left(totMastr.BILL_ADD2, 100),
                    Address3 = StringUtils.Left(totMastr.BILL_ADD3, 100),
                    City = StringUtils.Left(totMastr.BILL_CITY, 50),
                    Company = StringUtils.Left(totMastr.BILL_NAME, 50),
                    State = StringUtils.Left(totMastr.BILL_PROV, 3),
                    ZipCode = StringUtils.Left(totMastr.BILL_ZIP, 50)
                };
            }
            #endregion

            #region PickHead Mappings
            if (pickHeads != null && pickHeads.Count > 0)
            {
                if (WarehouseIntegrationConfig.Instance.UseCustomerAddressForReturns)
                {
                    shipment.ReturnAddress.Country = StringUtils.Left(pickHeads[0].SHPR_CNTRY, 50);
                    shipment.ReturnAddress.Phone = StringUtils.Left(pickHeads[0].BUS_TELNO, 50);
                }

                shipment.SenderEIN = StringUtils.Left(pickHeads[0].SHIPPER_EIN, 20);
                shipment.ShipToEIN = StringUtils.Left(pickHeads[0].SHIP_TO_EIN, 20);
				shipment.SpecialInstructions = string.Concat(
                    pickHeads[0].INSTRUCT1 ?? string.Empty, " ",
                    pickHeads[0].INSTRUCT2 ?? string.Empty, " ",
                    pickHeads[0].INSTRUCT3 ?? string.Empty, " ",
                    pickHeads[0].INSTRUCT4 ?? string.Empty, " ",
                    pickHeads[0].INSTRUCT5 ?? string.Empty, " ",
                    pickHeads[0].INSTRUCT6 ?? string.Empty, " ",
                    pickHeads[0].INSTRUCT7 ?? string.Empty);
                shipment.SpecialInstructions = StringUtils.Left(shipment.SpecialInstructions, 250);

                if (shipment.ThirdPartyAddress != null)
                    shipment.ThirdPartyAddress.Country = StringUtils.Left(pickHeads[0].BILL_CNTRY, 50);

                shipment.TrailerNumber = StringUtils.Left(pickHeads[0].TRAILER_NO, 50);

                if (!string.IsNullOrWhiteSpace(pickHeads[0].HANDLE))
                {
                    shipment.ShipmentKeywordXReves = new List<ShipmentKeywordXRef>();
                    shipment.ShipmentKeywordXReves.Add(new ShipmentKeywordXRef() { Keyword = new Keyword() { WordContent = pickHeads[0].HANDLE }, Shipment = shipment });
                }
            }
            #endregion

            return shipment;
        }
        #endregion

        #region ConvertToShipDTO (Container)
        public static Container ConvertToShipDTO(TOTMASTR totMastr)
        {
            if (!string.IsNullOrWhiteSpace(totMastr.PALLET_ASN)) // Ship only needs to know about the pallet records if the given carton is on a pallet
                return null;

            if ((totMastr.ACCUM_QTY == null || totMastr.ACCUM_QTY == 0) &&
                (totMastr.ACTION != (short)TotMastrActions.PrePrintCartonLabel && totMastr.ACTION != (short)TotMastrActions.PreRateOrder)) // Don't send Ship records with no quantity associated, unless it is a pre-print or pre-rate
                return null;

            Container container = new Container();
			string ltlPackageType = null;
            WarehouseSyncClient warehouseSyncClient = WarehouseConnectionAdapter.GetNewWarehouseSyncClient();

            try
            {
                #region Load External Data
                IList<TOTEDETL> totDetls = null;
                List<string> toteLabelsOnPallet = new List<string>() { totMastr.TOTLABEL };

                if (!string.IsNullOrWhiteSpace(totMastr.IS_PALLET) && totMastr.IS_PALLET == "1")
                {
                    IList<TOTMASTR> totMastrsOnPallet = warehouseSyncClient.FindAllTOTMASTRPaged(TOTMASTRHydration.None,
                        string.Format("TOTMASTR.Where(item => item.PALLET_ASN == \"{0}\" && item.ACCUM_QTY > 0 && item.SHP_CNSLDT == \"{1}\")", totMastr.CARTON_ASN, totMastr.SHP_CNSLDT));

                    if (totMastrsOnPallet != null && totMastrsOnPallet.Count > 0)
                        toteLabelsOnPallet.AddRange(totMastrsOnPallet.Select(tmp => tmp.TOTLABEL).Distinct());
                }

                string totDetlQuery = "TOTEDETL.Where(item => ";

                foreach (string toteLabelOnPallet in toteLabelsOnPallet)
                    totDetlQuery += string.Format("item.TOTLABEL == \"{0}\" || ", toteLabelOnPallet);

                totDetlQuery = totDetlQuery.Substring(0, totDetlQuery.Length - 4) + ")";

                totDetls = warehouseSyncClient.FindAllTOTEDETLPaged(TOTEDETLHydration.None,
                    string.Format("{0}.Take({1})", totDetlQuery, WarehouseIntegrationConfig.Instance.PageSize));
                int i = 0;

                while (totDetls != null && totDetls.Count > 0)
                {
                    i++;

                    IList<TOTEDETL> remainingTotdetls = warehouseSyncClient.FindAllTOTEDETLPaged(TOTEDETLHydration.None,
                        string.Format("{0}.Take({1}).Skip({2})", totDetlQuery, WarehouseIntegrationConfig.Instance.PageSize, (i * WarehouseIntegrationConfig.Instance.PageSize)));

                    if (remainingTotdetls == null || remainingTotdetls.Count == 0)
                        break;

                    foreach (TOTEDETL remainingTotdetl in remainingTotdetls)
                        totDetls.Add(remainingTotdetl);
                }
                #endregion

                #region ContainerOption
                container.ContainerOption = new ContainerOption();
                container.ContainerOption.AdditionalHandling = (!string.IsNullOrWhiteSpace(totMastr.SHP_OPTION) && totMastr.SHP_OPTION.Contains("H")) ? true : false;
                container.ContainerOption.AdultSignatureRequired = (!string.IsNullOrWhiteSpace(totMastr.SHP_OPTION) && totMastr.SHP_OPTION.Contains("A")) ? true : false;
                container.ContainerOption.AppointmentDelivery = (!string.IsNullOrWhiteSpace(totMastr.SHP_OPTION) && totMastr.SHP_OPTION.Contains("D")) ? true : false;
                container.ContainerOption.DoNotSafeDrop = (!string.IsNullOrWhiteSpace(totMastr.SHP_OPTION) && totMastr.SHP_OPTION.Contains("1")) ? true : false;
                container.ContainerOption.ProofRequireSignatureAdult19 = (!string.IsNullOrWhiteSpace(totMastr.SHP_OPTION) && totMastr.SHP_OPTION.Contains("2")) ? true : false;
                container.ContainerOption.LeaveAtDoor = (!string.IsNullOrWhiteSpace(totMastr.SHP_OPTION) && totMastr.SHP_OPTION.Contains("3")) ? true : false;     

                if (!string.IsNullOrWhiteSpace(totMastr.COD) && totMastr.COD == "C")
                {
                    container.CODContainer = true;
                    container.ContainerOption.CODAmount = totMastr.SHIP_COD;

                    if (!string.IsNullOrWhiteSpace(totMastr.COD_PAY))
                        container.ContainerOption.CODPaymentType = ShipXMLMapsAdapter.GetShipCODPaymentTypeCode(totMastr.COD_PAY);
                }

                container.ContainerOption.DeliveryConfirmation = (!string.IsNullOrWhiteSpace(totMastr.SHP_OPTION) && totMastr.SHP_OPTION.Contains("C")) ? true : false;
                container.ContainerOption.FedExDirectSignatureRequired = (!string.IsNullOrWhiteSpace(totMastr.SHP_OPTION) && totMastr.SHP_OPTION.Contains("T")) ? true : false;

                if (!string.IsNullOrWhiteSpace(totMastr.IS_PALLET) && totMastr.IS_PALLET == "1")
                {
                    container.ContainerOption.InnerPackageCount = warehouseSyncClient.FindAllTOTMASTRCount(
                        string.Format("TOTMASTR.Where(item => item.PALLET_ASN == \"{0}\" && item.ACCUM_QTY > 0 && item.SHP_CNSLDT == \"{1}\")", totMastr.CARTON_ASN, totMastr.SHP_CNSLDT));

                    ltlPackageType = "Pallet";
                }
                else
                {
                    container.ContainerOption.InnerPackageCount = 1;
                    ltlPackageType = "Carton";
                }

				if (!string.IsNullOrWhiteSpace(totMastr.SHP_OPTION) && totMastr.SHP_OPTION.Contains("I"))
				{
					container.ContainerOption.InsuranceType = 1;
					container.TransmitDeclaredValue = true;
				}

				container.ContainerOption.SignatureRequired = (!string.IsNullOrWhiteSpace(totMastr.SHP_OPTION) && totMastr.SHP_OPTION.Contains("S")) ? true : false;
                #endregion

                #region ContainerReferenceFields
                container.ContainerReferenceFields = new ContainerReferenceFields()
                {
                    ReferenceField1 = StringUtils.Left(totMastr.MESSAGE1, 50),
                    ReferenceField2 = StringUtils.Left(totMastr.MESSAGE2, 50),
                    ReferenceField3 = StringUtils.Left(totMastr.MESSAGE3, 50),
                    ReferenceField4 = StringUtils.Left(totMastr.MESSAGE4, 50),
                    ReferenceField5 = StringUtils.Left(totMastr.MESSAGE5, 50)
                };
                #endregion

                container.DeclaredValueAmount = totMastr.SHIP_DV;
                container.DocumentNumber = StringUtils.Left(totMastr.PACKSLIP, 30);
                container.Height = totMastr.SHIP_HEIGHT;
                container.IdentityField = StringUtils.Left(totMastr.CARTON_ASN, 50);
                container.Length = totMastr.SHIP_LENGTH;

                #region PackageType
                if (totMastr.SHP_OPTION.Contains("F"))
                    container.PackageType = "LETTER";

                if (totMastr.SHP_OPTION.Contains("G"))
                    container.PackageType = "SBOX";

                if (totMastr.SHP_OPTION.Contains("K"))
                    container.PackageType = "MBOX";

                if (totMastr.SHP_OPTION.Contains("J"))
                    container.PackageType = "LBOX";

                if (totMastr.SHP_OPTION.Contains("X"))
                    container.PackageType = "BOX";

                if (totMastr.SHP_OPTION.Contains("N"))
                    container.PackageType = "USPSRRBA";

                if (totMastr.SHP_OPTION.Contains("O"))
                    container.PackageType = "USPSRRBB";

                if (totMastr.SHP_OPTION.Contains("P"))
                    container.PackageType = "USPSRRBC";

                if (totMastr.SHP_OPTION.Contains("U"))
                    container.PackageType = "PAK";
				
				if (totMastr.SHP_OPTION.Contains("V"))
                    container.PackageType = "MDEXBX";

                if (totMastr.SHP_OPTION.Contains("Y"))
                    container.PackageType = "LGEXBX";

                if (totMastr.SHP_OPTION.Contains("?"))
                    container.PackageType = "TUBE";

                if (totMastr.SHP_OPTION.Contains("!"))
                    container.PackageType = "FRE";

                if (totMastr.SHP_OPTION.Contains("Q"))
                    container.PackageType = "EXLGEXBX";
				
				if (totMastr.SHP_OPTION.Contains("$"))
                    container.PackageType = "SMEXBX";

                if (string.IsNullOrWhiteSpace(container.PackageType))
                    container.PackageType = "CUSTOM";
                #endregion

                container.PONumber = StringUtils.Left(totMastr.CUST_ORDER, 50);
				container.Shippable = true; // Currently hard-coded since this is a "ship" integration as of today, not a "pack" integration

                if (totMastr.ACTION == (short)TotMastrActions.RateManifestShipment || totMastr.ACTION == (short)TotMastrActions.DeleteCarton)
                {
                    if (ShipIntegrationConfig.Instance.UseLastMileTrackingNumber)
                        container.TrackingNumber = string.IsNullOrWhiteSpace(totMastr.CAPTUREDBC) ? StringUtils.Left(totMastr.TRACKTRACE, 50) : StringUtils.Left(totMastr.CAPTUREDBC, 50);
                    else
                        container.TrackingNumber = StringUtils.Left(totMastr.TRACKTRACE, 50);
                }

                container.UOM = new UOM() { Name = StringUtils.Left(ShipIntegrationConfig.Instance.WeightUnitOfMeasure, 50) };
                container.Weight = totMastr.WEIGHT == null || totMastr.WEIGHT == 0 ? ShipIntegrationConfig.Instance.DefaultContainerWeight : totMastr.WEIGHT;
                container.Width = totMastr.SHIP_WIDTH;

				#region AmazonItems, ContainerCIs, and LTLData
				if ((totDetls == null || totDetls.Count == 0) && totMastr.ACTION == (short)TotMastrActions.PreRateOrder)
				{
					LTLData ltlData = new LTLData() { CommodityClass = StringUtils.Left(ShipIntegrationConfig.Instance.ForcedPreRateCommodityClass, 50) };

					container.LTLDatas.Add(ltlData);
				}

				foreach (TOTEDETL totDetl in totDetls)
                {
                    IList<Config> configs = null;
                    IList<ProdMstr> ciLTLProdMstrs = null;
                    IList<CUBISCAN> cubiscans = null;
                    ContainerCI containerCI = new ContainerCI();
                    LTLData ltlData = new LTLData();

                    #region Load External Data
                    configs = warehouseSyncClient.FindAllConfigPaged(ConfigHydration.None, "Config.Where(item => item.ConfigKey.StartsWith(\"PACKSIZE_\"))");
                    ciLTLProdMstrs = warehouseSyncClient.FindProdMstrsBySearchCriteriaPaged(new ProdMstrsBySearchCriteria() { Extended = totDetl.EXTENDED },
                        ProdMstrHydration.None, 0, 1);
                    cubiscans = warehouseSyncClient.FindAllCUBISCANPaged(CUBISCANHydration.None,
                        string.Format("CUBISCAN.Where(item => item.PRODUCT == \"{0}\" && item.PACKSIZE == {1}).Take(1)", totDetl.PRODUCT, totDetl.PACKSIZE));
					#endregion

					#region AmazonItems
					if (IsAmazonOrder(totMastr.SHP_OPTION))
					{
						AmazonItem amazonItem = new AmazonItem();

						amazonItem.Container = container;

						var pickDetls = warehouseSyncClient.FindAllPickDetlPaged(PickDetlHydration.None, string.Format("PickDet.Where(item => item.PackSlip == \"{0}\" && item.LineNumber == \"{1}\")",
							totDetl.PACKSLIP, totDetl.ORDER_SEQ));

						if (pickDetls == null || pickDetls.Count == 0)
							throw new Exception("One or more items on the shipment did not provide an Amazon item identifier!");

						if (ShipIntegrationConfig.Instance.AmazonAssumeBoxPerLine && pickDetls[0].IS_CMPN == "E") // This is a kit component, thus the kit parent needs looking up for the Amazon item ID
						{
							string[] splitKitLineNum = totDetl.ORDER_SEQ.Split('.'); // Kit component line numbers look like this: 1.001
							string parentLineNum = splitKitLineNum[0]; // Putting this into a variable for code readability
							var parentPickDetls = warehouseSyncClient.FindAllPickDetlPaged(PickDetlHydration.None,
								string.Format("PickDet.Where(item => item.PackSlip == \"{0}\" && item.LineNumber == \"{1}\")", totDetl.PACKSLIP, parentLineNum));
							if (parentPickDetls == null || parentPickDetls.Count == 0 || string.IsNullOrWhiteSpace(parentPickDetls[0].CustomerCrossReference))
								throw new Exception("One or more items on the shipment did not provide an Amazon item identifier!");
							amazonItem.ItemId = StringUtils.Left(parentPickDetls[0].CustomerCrossReference, 128);
							amazonItem.Quantity = parentPickDetls[0].QuantityPicked;
						}
						else // This is a regular item, not a kit
						{
							if (string.IsNullOrWhiteSpace(pickDetls[0].CustomerCrossReference))
								throw new Exception("One or more items on the shipment did not provide an Amazon item identifier!");
							amazonItem.ItemId = StringUtils.Left(pickDetls[0].CustomerCrossReference, 128);
							amazonItem.Quantity = totDetl.QTY;
						}

						container.AmazonItems.Add(amazonItem);
					}
					#endregion

					#region ContainerCI
					containerCI.Container = container;
                    containerCI.CountryOfManufacture = StringUtils.Left(totDetl.CTRY_ORIG, 2);
                    containerCI.HarmonizedTariffCode = StringUtils.Left(totDetl.TariffClass, 10);
                    containerCI.Quantity = totDetl.QTY;
                    containerCI.UnitValue = totDetl.SELL_PRICE;

                    if (!string.IsNullOrWhiteSpace(totMastr.SHIP_VIA) && !string.IsNullOrWhiteSpace(totMastr.SHIP_SERVC) && 
						(totMastr.SHIP_VIA.StartsWith("DHLGM") || totMastr.SHIP_VIA.StartsWith("DHLAMP") || totMastr.SHIP_VIA.StartsWith("GLOBEG")))
                        containerCI.CIDescription = StringUtils.Left(string.Format("{0} - {1}", StringUtils.Left(totDetl.PRODUCT, 20), totDetl.DESCRIPT), 250);
                    else
                        containerCI.CIDescription = StringUtils.Left(totDetl.CommodityDescription, 250);

                    if (ciLTLProdMstrs != null && ciLTLProdMstrs.Count > 0)
                    {
                        containerCI.UnitWeight = ciLTLProdMstrs[0].Weight;

                        if (totDetl.PACKSIZE.HasValue && configs != null && configs.Count > 0)
                        {
                            if (ciLTLProdMstrs[0].Size1 == totDetl.PACKSIZE && configs.Any(item => item.ConfigKey == "PACKSIZE_1"))
                                containerCI.QuantityUOM = StringUtils.Left(configs.Where(item => item.ConfigKey == "PACKSIZE_1").First().CValue, 10);
                            else if (ciLTLProdMstrs[0].Size2 == totDetl.PACKSIZE && configs.Any(item => item.ConfigKey == "PACKSIZE_2"))
                                containerCI.QuantityUOM = StringUtils.Left(configs.Where(item => item.ConfigKey == "PACKSIZE_2").First().CValue, 10);
                            else if (ciLTLProdMstrs[0].Size3 == totDetl.PACKSIZE && configs.Any(item => item.ConfigKey == "PACKSIZE_3"))
                                containerCI.QuantityUOM = StringUtils.Left(configs.Where(item => item.ConfigKey == "PACKSIZE_3").First().CValue, 10);
                            else if (ciLTLProdMstrs[0].Size4 == totDetl.PACKSIZE && configs.Any(item => item.ConfigKey == "PACKSIZE_4"))
                                containerCI.QuantityUOM = StringUtils.Left(configs.Where(item => item.ConfigKey == "PACKSIZE_4").First().CValue, 10);
                            else if (ciLTLProdMstrs[0].Size5 == totDetl.PACKSIZE && configs.Any(item => item.ConfigKey == "PACKSIZE_5"))
                                containerCI.QuantityUOM = StringUtils.Left(configs.Where(item => item.ConfigKey == "PACKSIZE_5").First().CValue, 10);
                        }
                    }

                    if (cubiscans != null && cubiscans.Count > 0)
                        containerCI.UnitWeight = cubiscans[0].WEIGHT;

                    container.ContainerCIs.Add(containerCI);
                    #endregion

                    #region LTLDatas
                    ltlData.CommodityClassDescription = StringUtils.Left(totDetl.CommodityDescription, 100);
                    ltlData.Container = container;
                    ltlData.InnerPackageCount = 1;
                    ltlData.PackageType = ltlPackageType;

					if (totMastr.ACTION == (short)TotMastrActions.PreRateOrder && !string.IsNullOrWhiteSpace(ShipIntegrationConfig.Instance.ForcedPreRateCommodityClass))
						ltlData.CommodityClass = StringUtils.Left(ShipIntegrationConfig.Instance.ForcedPreRateCommodityClass, 50);
					else
						ltlData.CommodityClass = StringUtils.Left(!string.IsNullOrWhiteSpace(totDetl.FreightClass) ? totDetl.FreightClass : ShipIntegrationConfig.Instance.DefaultCommodityClass, 50);

                    if (!string.IsNullOrWhiteSpace(totDetl.NMFCNumber))
                    {
                        if (totDetl.NMFCNumber.Contains("-"))
                        {
                            string[] splitNMFC = totDetl.NMFCNumber.Split('-');

                            ltlData.NMFCCode = StringUtils.Left(splitNMFC[0], 50);
                            ltlData.NMFCItemSub = StringUtils.Left(splitNMFC[1], 10);
                        }
                        else
                            ltlData.NMFCCode = StringUtils.Left(totDetl.NMFCNumber, 50);
                    }

					if (ciLTLProdMstrs != null && ciLTLProdMstrs.Count > 0)
					{
						//if (totDetls.Count > 1)
						//    ltlData.Weight = ciLTLProdMstrs[0].Weight;
						//else
						//    ltlData.Weight = container.Weight;

						if (ciLTLProdMstrs[0].Weight != null && totDetl.QTY != null)
							ltlData.Weight = ciLTLProdMstrs[0].Weight * totDetl.QTY;
					}

					container.LTLDatas.Add(ltlData);
                    #endregion
                }
                #endregion
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                if (warehouseSyncClient != null)
                {
                    warehouseSyncClient.Close();
                    warehouseSyncClient = null;
                }
            }

            return container;
        }
        #endregion

        #region ConvertToShipDTO (Containers)
        public static List<Container> ConvertToShipDTO(IList<TOTMASTR> totMastrs)
        {
            List<Container> containers = new List<Container>();

			#region Build Containers for Pre-Rate
			if (totMastrs[0].ACTION == (short)TotMastrActions.PreRateOrder && !WarehouseIntegrationConfig.Instance.UseMultipleContainersForPreRate)
			{
				if (totMastrs.Count != 1)
					throw new Exception(string.Format("Pre-rating for order {0} failed because the number of containers specified by Warehouse is not 1!", totMastrs[0].PACKSLIP));

				decimal totalWeight = totMastrs[0].WEIGHT ?? 0m;
				short numberOfContainers = totMastrs[0].EST_TOT_PC ?? 0;

				if (numberOfContainers == 0)
					throw new Exception(string.Format("Pre-rating for order {0} failed because the number of containers specified by Warehouse is 0!", totMastrs[0].PACKSLIP));

				totMastrs[0].WEIGHT = totalWeight > 0 ? (totalWeight / numberOfContainers) : ShipIntegrationConfig.Instance.DefaultContainerWeight;

				for (int i = 0; i < numberOfContainers - 1; i++)
					totMastrs.Add(new TOTMASTR(totMastrs[0]));
			}
            #endregion

            foreach (TOTMASTR totMastr in totMastrs)
            {
                Container container = ConvertToShipDTO(totMastr);

                if (container != null)
                    containers.Add(container);
            }

            return containers;
        }
		#endregion

		#region IsAmazonOrder
		private static bool IsAmazonOrder(string shipOptions)
		{
			return !string.IsNullOrWhiteSpace(shipOptions) && shipOptions.Contains("Z");// SHP_OPTION Z indicates Amazon order
		}
		#endregion
	}
}
