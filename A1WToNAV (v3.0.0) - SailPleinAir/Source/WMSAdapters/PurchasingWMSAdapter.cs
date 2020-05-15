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
        #region ParsePurchaseOrder
        public static DataTable ParsePurchaseOrder(PurchaseOrder purchaseOrder)
        {
            DataTable result = CompanyConfig.Instance.DownloadTableSchema;

            #region RH
            DataRow row = result.NewRow();

            row["FIELD001"] = "RH";
            row["FIELD003"] = purchaseOrder.ContainerBatchRef;
            row["FIELD004"] = purchaseOrder.PurchaseOrderNumber;
            row["FIELD005"] = purchaseOrder.VendorNumber;
            row["FIELD006"] = StringUtils.Left(purchaseOrder.VendorName, 35);
            row["FIELD007"] = StringUtils.Left(purchaseOrder.BuyerName, 15);
            row["FIELD008"] = purchaseOrder.AllowBackOrder ? "Y" : "N";
            row["FIELD009"] = StringUtils.Left(purchaseOrder.ClientName, 10);
            row["FIELD010"] = StringUtils.Left(purchaseOrder.SpecialInstructions1, 40); //Increased from 20 03/12/2018
            row["FIELD011"] = StringUtils.Left(purchaseOrder.SpecialInstructions2, 40);
            row["FIELD012"] = StringUtils.Left(purchaseOrder.SpecialInstructions3, 40);
            row["FIELD013"] = StringUtils.Left(purchaseOrder.SpecialInstructions4, 40);
            row["FIELD014"] = StringUtils.Left(purchaseOrder.SpecialInstructions5, 40);
            row["FIELD015"] = StringUtils.Left(purchaseOrder.SpecialInstructions6, 40);
            row["FIELD016"] = StringUtils.Left(purchaseOrder.SpecialInstructions7, 40);
            row["FIELD017"] = purchaseOrder.BackOrder;
            row["FIELD018"] = purchaseOrder.IsRMA ? "Y" : "N";
            row["FIELD019"] = purchaseOrder.DueDate == DateTime.MinValue ? string.Empty : purchaseOrder.DueDate.ToString("yyyyMMdd");
            row["FIELD020"] = StringUtils.Left(purchaseOrder.Extra1, 40);
            row["FIELD021"] = StringUtils.Left(purchaseOrder.Extra2, 40);
            row["FIELD022"] = StringUtils.Left(purchaseOrder.Extra3, 40);
            row["FIELD023"] = StringUtils.Left(purchaseOrder.Extra4, 40);
            row["FIELD024"] = StringUtils.Left(purchaseOrder.Extra5, 40);
            //row["FIELD024"] =  StringUtils.Left(purchaseOrder.StagingID, 40);
            row["FIELD025"] = StringUtils.Left(purchaseOrder.WarehouseTransferLocation, 10);

            #region row["FIELD026"] = purchaseOrder.PoType;
            switch (purchaseOrder.PoType)
            {
                case PoType.NotSpecified:
                    row["FIELD026"] = string.Empty;
                    break;
                case PoType.Transfer:
					row["FIELD026"] = "TRANSFER";
                    break;
                case PoType.Requisition:
                    row["FIELD026"] = "REQUISITION";
                    break;
            }
            #endregion

            row["FIELD027"] = StringUtils.Left(purchaseOrder.RequisitionNumber, 20);
            row["FIELD028"] = StringUtils.Left(purchaseOrder.Email, 40);
            row["FIELD029"] = StringUtils.Left(purchaseOrder.Packslip, 20);
            row["FIELD030"] = StringUtils.Left(purchaseOrder.RequestPOD, 1);
            row["FIELD031"] = StringUtils.Left(purchaseOrder.RequestSH, 1);
            row["FIELD032"] = StringUtils.Left(purchaseOrder.NoSHReship, 1);
			row["FIELD033"] = StringUtils.Left(purchaseOrder.OrderSource, 10); // Added for A1W 64
			row["FIELD034"] = StringUtils.Left(purchaseOrder.IntegrationUDF1, 40); // Added for A1W 64
			row["FIELD035"] = StringUtils.Left(purchaseOrder.IntegrationUDF2, 40); // Added for A1W 64
			row["FIELD036"] = StringUtils.Left(purchaseOrder.IntegrationUDF3, 40); // Added for A1W 64
			row["FIELD037"] = StringUtils.Left(purchaseOrder.IntegrationUDF4, 40); // Added for A1W 64
			row["FIELD038"] = StringUtils.Left(purchaseOrder.IntegrationUDF5, 40); // Added for A1W 64
			row["FIELD039"] = StringUtils.Left(purchaseOrder.IntegrationUDF6, 40); // Added for A1W 64
			row["FIELD040"] = StringUtils.Left(purchaseOrder.IntegrationUDF7, 40); // Added for A1W 64
			row["FIELD041"] = StringUtils.Left(purchaseOrder.IntegrationUDF8, 40); // Added for A1W 64
			row["FIELD042"] = StringUtils.Left(purchaseOrder.IntegrationUDF9, 40); // Added for A1W 64
			row["FIELD043"] = StringUtils.Left(purchaseOrder.IntegrationUDF10, 40); // Added for A1W 64
			row["FIELD044"] = StringUtils.Left(purchaseOrder.IntegrationUDF11, 40); // Added for A1W 64
			row["FIELD045"] = StringUtils.Left(purchaseOrder.IntegrationUDF12, 40); // Added for A1W 64
			row["FIELD046"] = StringUtils.Left(purchaseOrder.IntegrationUDF13, 40); // Added for A1W 64
			row["FIELD047"] = StringUtils.Left(purchaseOrder.IntegrationUDF14, 40); // Added for A1W 64
			row["FIELD048"] = StringUtils.Left(purchaseOrder.IntegrationUDF15, 40); // Added for A1W 64
			row["FIELD049"] = StringUtils.Left(purchaseOrder.IntegrationUDF16, 40); // Added for A1W 64
			row["FIELD050"] = StringUtils.Left(purchaseOrder.IntegrationUDF17, 40); // Added for A1W 64
			row["FIELD051"] = StringUtils.Left(purchaseOrder.IntegrationUDF18, 40); // Added for A1W 64
			row["FIELD052"] = StringUtils.Left(purchaseOrder.IntegrationUDF19, 40); // Added for A1W 64
			row["FIELD053"] = StringUtils.Left(purchaseOrder.IntegrationUDF20, 40); // Added for A1W 64
			row["FIELD054"] = StringUtils.Left(purchaseOrder.IntegrationUDF21, 40); // Added for A1W 64
			row["FIELD055"] = StringUtils.Left(purchaseOrder.IntegrationUDF22, 40); // Added for A1W 64
			row["FIELD056"] = StringUtils.Left(purchaseOrder.IntegrationUDF23, 40); // Added for A1W 64
			row["FIELD057"] = StringUtils.Left(purchaseOrder.IntegrationUDF24, 40); // Added for A1W 64
			row["FIELD058"] = StringUtils.Left(purchaseOrder.IntegrationUDF25, 40); // Added for A1W 64

            result.Rows.Add(row);
            #endregion

            #region RD
            foreach (PurchaseOrderLine line in purchaseOrder.Lines)
            {
                row = result.NewRow();

                row["FIELD001"] = "RD";
                row["FIELD002"] = line.Warehouse;
                row["FIELD003"] = line.Location;
                row["FIELD004"] = line.ContainerBatchRef;
                row["FIELD005"] = line.PurchaseOrderNumber;
                row["FIELD006"] = line.LineNumber;
                row["FIELD007"] = line.ProductCode;
                row["FIELD008"] = StringUtils.Left(line.PrimaryLocation, 8);
				row["FIELD009"] = StringUtils.Left(line.Description, 100); // Changed to 100 characters for A1W 64
                row["FIELD010"] = line.VendorProductNumber;
                row["FIELD011"] = line.UnitOfMeasureMultiplier;
				row["FIELD012"] = StringUtils.Left(line.ProductClass, 10); // Changed to 10 characters for A1W 64
                row["FIELD013"] = StringUtils.Left(line.UPC, 14);
                row["FIELD014"] = line.QuantityExpected.ToString(CultureInfo.InvariantCulture);
				row["FIELD015"] = StringUtils.Left(line.OverReceiptPercentage, 6); // Added for A1W 64
                row["FIELD016"] = line.IsSpecial ? "Y" : "N";
                row["FIELD017"] = line.CustomerNumber;
                row["FIELD018"] = StringUtils.Left(line.CustomerName, 35);
                row["FIELD019"] = line.SalesOrderNumber;
                row["FIELD020"] = line.SalesOrderLineNumber;
				row["FIELD028"] = ParseTracking(line.ReceiveAttributeTracking); // Do not send attributes, as they will be pulled from prodmstr
                row["FIELD033"] = line.ExpiryDate == DateTime.MinValue ? string.Empty : line.ExpiryDate.ToString("yyyyMMdd");

                #region row["FIELD034"] = line.StockItemIndicator;
                switch (line.StockItemIndicator)
                {
                    case StockItemIndicatorType.NotSpecified:
                        row["FIELD034"] = string.Empty;
                        break;
                    case StockItemIndicatorType.StockItem:
                        row["FIELD034"] = "0";
                        break;
                    case StockItemIndicatorType.NonStockItem:
                        row["FIELD034"] = "1";
                        break;
                }
                #endregion

                row["FIELD035"] = line.UnitPrice.ToString(CultureInfo.InvariantCulture);
                row["FIELD036"] = line.Discount.ToString(CultureInfo.InvariantCulture);
                row["FIELD037"] = line.CostPrice.ToString(CultureInfo.InvariantCulture);
                row["FIELD039"] = StringUtils.Left(line.Extra1, 40);
                row["FIELD040"] = StringUtils.Left(line.Extra2, 40);
                row["FIELD041"] = StringUtils.Left(line.Extra3, 40);
                row["FIELD042"] = StringUtils.Left(line.Extra4, 40);
                row["FIELD043"] = StringUtils.Left(line.Extra5, 40);
                row["FIELD044"] = line.ExpectedRequiredDate == DateTime.MinValue ? string.Empty : line.ExpectedRequiredDate.ToString("yyyyMMdd");
                row["FIELD045"] = StringUtils.Left(line.SpecialInstructions1, 40); //03/09/18 Increased from 20
                row["FIELD046"] = StringUtils.Left(line.SpecialInstructions2, 40);
                row["FIELD047"] = StringUtils.Left(line.SpecialInstructions3, 40);
                row["FIELD048"] = StringUtils.Left(line.SpecialInstructions4, 40);
                row["FIELD049"] = StringUtils.Left(line.SpecialInstructions5, 40);
                row["FIELD050"] = StringUtils.Left(line.SpecialInstructions6, 40);
                row["FIELD051"] = StringUtils.Left(line.SpecialInstructions7, 40);
                row["FIELD052"] = StringUtils.Left(line.UnitOfMeasureText, 15);
                row["FIELD053"] = StringUtils.Left(line.CountryOfOrigin, 15);
                row["FIELD054"] = StringUtils.Left(line.ContainerStatus, 1);
                row["FIELD055"] = StringUtils.Left(line.HostLineReference, 20);
                row["FIELD056"] = StringUtils.Left(line.RMAReason, 30);
                row["FIELD057"] = line.RMARestockCharge;
                row["FIELD058"] = line.CreditNow ? "Y" : "N";
                row["FIELD059"] = StringUtils.Left(line.Reship.ToString(CultureInfo.InvariantCulture), 6);
                row["FIELD060"] = line.ReInvoice ? "Y" : "N";
				row["FIELD062"] = StringUtils.Left(line.IntegrationUDF1, 40); // Added for A1W 64
				row["FIELD063"] = StringUtils.Left(line.IntegrationUDF2, 40); // Added for A1W 64
				row["FIELD064"] = StringUtils.Left(line.IntegrationUDF3, 40); // Added for A1W 64
				row["FIELD065"] = StringUtils.Left(line.IntegrationUDF4, 40); // Added for A1W 64
				row["FIELD066"] = StringUtils.Left(line.IntegrationUDF5, 40); // Added for A1W 64
				row["FIELD067"] = StringUtils.Left(line.IntegrationUDF6, 40); // Added for A1W 64
				row["FIELD068"] = StringUtils.Left(line.IntegrationUDF7, 40); // Added for A1W 64
				row["FIELD069"] = StringUtils.Left(line.IntegrationUDF8, 40); // Added for A1W 64
				row["FIELD070"] = StringUtils.Left(line.IntegrationUDF9, 40); // Added for A1W 64
				row["FIELD071"] = StringUtils.Left(line.IntegrationUDF10, 40); // Added for A1W 64
				row["FIELD072"] = StringUtils.Left(line.IntegrationUDF11, 40); // Added for A1W 64
				row["FIELD073"] = StringUtils.Left(line.IntegrationUDF12, 40); // Added for A1W 64
				row["FIELD074"] = StringUtils.Left(line.IntegrationUDF13, 40); // Added for A1W 64
				row["FIELD075"] = StringUtils.Left(line.IntegrationUDF14, 40); // Added for A1W 64
				row["FIELD076"] = StringUtils.Left(line.IntegrationUDF15, 40); // Added for A1W 64
				row["FIELD077"] = StringUtils.Left(line.IntegrationUDF16, 40); // Added for A1W 64
				row["FIELD078"] = StringUtils.Left(line.IntegrationUDF17, 40); // Added for A1W 64
				row["FIELD079"] = StringUtils.Left(line.IntegrationUDF18, 40); // Added for A1W 64
				row["FIELD080"] = StringUtils.Left(line.IntegrationUDF19, 40); // Added for A1W 64
				row["FIELD081"] = StringUtils.Left(line.IntegrationUDF20, 40); // Added for A1W 64
				row["FIELD082"] = StringUtils.Left(line.IntegrationUDF21, 40); // Added for A1W 64
				row["FIELD083"] = StringUtils.Left(line.IntegrationUDF22, 40); // Added for A1W 64
				row["FIELD084"] = StringUtils.Left(line.IntegrationUDF23, 40); // Added for A1W 64
				row["FIELD085"] = StringUtils.Left(line.IntegrationUDF24, 40); // Added for A1W 64
				row["FIELD086"] = StringUtils.Left(line.IntegrationUDF25, 40); // Added for A1W 64

                #region Attributes
                if (line.Attributes.Count == 0)
                    result.Rows.Add(row);
                else
                {
                    foreach (AttributeSet attributes in line.Attributes)
                    {
                        DataRow attrRow = result.NewRow();
                        attrRow.ItemArray = row.ItemArray;

                        attrRow["FIELD022"] = attributes.Attribute1;
                        attrRow["FIELD023"] = attributes.Attribute2;
                        attrRow["FIELD024"] = attributes.Attribute3;
                        attrRow["FIELD025"] = attributes.Attribute4;
                        attrRow["FIELD026"] = attributes.Attribute5;
                        attrRow["FIELD027"] = attributes.Attribute6;
                        attrRow["FIELD029"] = attributes.Attribute7;
                        attrRow["FIELD030"] = attributes.Attribute8;
                        attrRow["FIELD031"] = attributes.Attribute9;
                        attrRow["FIELD032"] = attributes.Attribute10;

                        result.Rows.Add(attrRow);
                    }
                }
                #endregion
            }
            #endregion

            return result;
        }
        #endregion

		#region PurchaseOrder to A1W Middle Tier DTO
        //public static List<DNLOAD> PurchaseOrderToDTO(PurchaseOrder purchaseOrder)
        //{
        //    List<DNLOAD> dnloads = new List<DNLOAD>();
			
        //    #region Lines
        //    foreach (PurchaseOrderLine line in purchaseOrder.Lines)
        //    {
        //        DNLOAD lineDnload = new DNLOAD();

        //        lineDnload.FIELD001 = "RD";
        //        lineDnload.FIELD002 = line.Warehouse;
        //        lineDnload.FIELD003 = line.Location;
        //        lineDnload.FIELD004 = line.ContainerBatchRef;
        //        lineDnload.FIELD005 = line.PurchaseOrderNumber;
        //        lineDnload.FIELD006 = line.LineNumber;
        //        lineDnload.FIELD007 = line.ProductCode;
        //        lineDnload.FIELD008 = StringUtils.Left(line.PrimaryLocation, 8);
        //        lineDnload.FIELD009 = StringUtils.Left(line.Description, 100); // Changed to 100 characters for A1W 64
        //        lineDnload.FIELD010 = line.VendorProductNumber;
        //        lineDnload.FIELD011 = line.UnitOfMeasureMultiplier;
        //        lineDnload.FIELD012 = StringUtils.Left(line.ProductClass, 10); // Changed to 10 characters for A1W 64
        //        lineDnload.FIELD013 = StringUtils.Left(line.UPC, 14);
        //        lineDnload.FIELD014 = line.QuantityExpected.ToString(CultureInfo.InvariantCulture);
        //        lineDnload.FIELD015 = StringUtils.Left(line.OverReceiptPercentage, 6); // Added for A1W 64
        //        lineDnload.FIELD016 = line.IsSpecial ? "Y" : "N";
        //        lineDnload.FIELD017 = line.CustomerNumber;
        //        lineDnload.FIELD018 = StringUtils.Left(line.CustomerName, 35);
        //        lineDnload.FIELD019 = line.SalesOrderNumber;
        //        lineDnload.FIELD020 = line.SalesOrderLineNumber;

        //        foreach (AttributeSet attributes in line.Attributes)
        //        {
        //            lineDnload.FIELD022 = attributes.Attribute1;
        //            lineDnload.FIELD023 = attributes.Attribute2;
        //            lineDnload.FIELD024 = attributes.Attribute3;
        //            lineDnload.FIELD025 = attributes.Attribute4;
        //            lineDnload.FIELD026 = attributes.Attribute5;
        //            lineDnload.FIELD027 = attributes.Attribute6;
        //            lineDnload.FIELD029 = attributes.Attribute7;
        //            lineDnload.FIELD030 = attributes.Attribute8;
        //            lineDnload.FIELD031 = attributes.Attribute9;
        //            lineDnload.FIELD032 = attributes.Attribute10;
        //        }

        //        lineDnload.FIELD033 = line.ExpiryDate == DateTime.MinValue ? string.Empty : line.ExpiryDate.ToString("yyyyMMdd");

        //        switch (line.StockItemIndicator)
        //        {
        //            case StockItemIndicatorType.NotSpecified:
        //                lineDnload.FIELD034 = string.Empty;
        //                break;
        //            case StockItemIndicatorType.StockItem:
        //                lineDnload.FIELD034 = "0";
        //                break;
        //            case StockItemIndicatorType.NonStockItem:
        //                lineDnload.FIELD034 = "1";
        //                break;
        //        }

        //        lineDnload.FIELD035 = line.UnitPrice.ToString(CultureInfo.InvariantCulture);
        //        lineDnload.FIELD036 = line.Discount.ToString(CultureInfo.InvariantCulture);
        //        lineDnload.FIELD037 = line.CostPrice.ToString(CultureInfo.InvariantCulture);
        //        lineDnload.FIELD039 = StringUtils.Left(line.Extra1, 40);
        //        lineDnload.FIELD040 = StringUtils.Left(line.Extra2, 40);
        //        lineDnload.FIELD041 = StringUtils.Left(line.Extra3, 40);
        //        lineDnload.FIELD042 = StringUtils.Left(line.Extra4, 40);
        //        lineDnload.FIELD043 = StringUtils.Left(line.Extra5, 40);
        //        lineDnload.FIELD044 = line.ExpectedRequiredDate == DateTime.MinValue ? string.Empty : line.ExpectedRequiredDate.ToString("yyyyMMdd");
        //        lineDnload.FIELD045 = StringUtils.Left(line.SpecialInstructions1, 20);
        //        lineDnload.FIELD046 = StringUtils.Left(line.SpecialInstructions2, 20);
        //        lineDnload.FIELD047 = StringUtils.Left(line.SpecialInstructions3, 20);
        //        lineDnload.FIELD048 = StringUtils.Left(line.SpecialInstructions4, 20);
        //        lineDnload.FIELD049 = StringUtils.Left(line.SpecialInstructions5, 20);
        //        lineDnload.FIELD050 = StringUtils.Left(line.SpecialInstructions6, 20);
        //        lineDnload.FIELD051 = StringUtils.Left(line.SpecialInstructions7, 20);
        //        lineDnload.FIELD052 = StringUtils.Left(line.UnitOfMeasureText, 15);
        //        lineDnload.FIELD053 = StringUtils.Left(line.CountryOfOrigin, 15);
        //        lineDnload.FIELD054 = StringUtils.Left(line.ContainerStatus, 1);
        //        lineDnload.FIELD055 = StringUtils.Left(line.HostLineReference, 20);
        //        lineDnload.FIELD056 = StringUtils.Left(line.RMAReason, 30);
        //        lineDnload.FIELD057 = line.RMARestockCharge;
        //        lineDnload.FIELD058 = line.CreditNow ? "Y" : "N";
        //        lineDnload.FIELD059 = StringUtils.Left(line.Reship.ToString(CultureInfo.InvariantCulture), 6);
        //        lineDnload.FIELD060 = line.ReInvoice ? "Y" : "N";
        //        lineDnload.FIELD062 = StringUtils.Left(line.IntegrationUDF1, 40); // Added for A1W 64
        //        lineDnload.FIELD063 = StringUtils.Left(line.IntegrationUDF2, 40); // Added for A1W 64
        //        lineDnload.FIELD064 = StringUtils.Left(line.IntegrationUDF3, 40); // Added for A1W 64
        //        lineDnload.FIELD065 = StringUtils.Left(line.IntegrationUDF4, 40); // Added for A1W 64
        //        lineDnload.FIELD066 = StringUtils.Left(line.IntegrationUDF5, 40); // Added for A1W 64
        //        lineDnload.FIELD067 = StringUtils.Left(line.IntegrationUDF6, 40); // Added for A1W 64
        //        lineDnload.FIELD068 = StringUtils.Left(line.IntegrationUDF7, 40); // Added for A1W 64
        //        lineDnload.FIELD069 = StringUtils.Left(line.IntegrationUDF8, 40); // Added for A1W 64
        //        lineDnload.FIELD070 = StringUtils.Left(line.IntegrationUDF9, 40); // Added for A1W 64
        //        lineDnload.FIELD071 = StringUtils.Left(line.IntegrationUDF10, 40); // Added for A1W 64
        //        lineDnload.FIELD072 = StringUtils.Left(line.IntegrationUDF11, 40); // Added for A1W 64
        //        lineDnload.FIELD073 = StringUtils.Left(line.IntegrationUDF12, 40); // Added for A1W 64
        //        lineDnload.FIELD074 = StringUtils.Left(line.IntegrationUDF13, 40); // Added for A1W 64
        //        lineDnload.FIELD075 = StringUtils.Left(line.IntegrationUDF14, 40); // Added for A1W 64
        //        lineDnload.FIELD076 = StringUtils.Left(line.IntegrationUDF15, 40); // Added for A1W 64
        //        lineDnload.FIELD077 = StringUtils.Left(line.IntegrationUDF16, 40); // Added for A1W 64
        //        lineDnload.FIELD078 = StringUtils.Left(line.IntegrationUDF17, 40); // Added for A1W 64
        //        lineDnload.FIELD079 = StringUtils.Left(line.IntegrationUDF18, 40); // Added for A1W 64
        //        lineDnload.FIELD080 = StringUtils.Left(line.IntegrationUDF19, 40); // Added for A1W 64
        //        lineDnload.FIELD081 = StringUtils.Left(line.IntegrationUDF20, 40); // Added for A1W 64
        //        lineDnload.FIELD082 = StringUtils.Left(line.IntegrationUDF21, 40); // Added for A1W 64
        //        lineDnload.FIELD083 = StringUtils.Left(line.IntegrationUDF22, 40); // Added for A1W 64
        //        lineDnload.FIELD084 = StringUtils.Left(line.IntegrationUDF23, 40); // Added for A1W 64
        //        lineDnload.FIELD085 = StringUtils.Left(line.IntegrationUDF24, 40); // Added for A1W 64
        //        lineDnload.FIELD086 = StringUtils.Left(line.IntegrationUDF25, 40); // Added for A1W 64

        //        dnloads.Add(lineDnload);
        //    }
        //    #endregion

        //    #region Header
        //    DNLOAD headerDnload = new DNLOAD();

        //    headerDnload.FIELD001 = "RH";
        //    headerDnload.FIELD003 = purchaseOrder.ContainerBatchRef;
        //    headerDnload.FIELD004 = purchaseOrder.PurchaseOrderNumber;
        //    headerDnload.FIELD005 = purchaseOrder.VendorNumber;
        //    headerDnload.FIELD006 = StringUtils.Left(purchaseOrder.VendorName, 35);
        //    headerDnload.FIELD007 = StringUtils.Left(purchaseOrder.BuyerName, 15);
        //    headerDnload.FIELD008 = purchaseOrder.AllowBackOrder ? "Y" : "N";
        //    headerDnload.FIELD009 = StringUtils.Left(purchaseOrder.ClientName, 10);
        //    headerDnload.FIELD010 = StringUtils.Left(purchaseOrder.SpecialInstructions1, 20);
        //    headerDnload.FIELD011 = StringUtils.Left(purchaseOrder.SpecialInstructions2, 20);
        //    headerDnload.FIELD012 = StringUtils.Left(purchaseOrder.SpecialInstructions3, 20);
        //    headerDnload.FIELD013 = StringUtils.Left(purchaseOrder.SpecialInstructions4, 20);
        //    headerDnload.FIELD014 = StringUtils.Left(purchaseOrder.SpecialInstructions5, 20);
        //    headerDnload.FIELD015 = StringUtils.Left(purchaseOrder.SpecialInstructions6, 20);
        //    headerDnload.FIELD016 = StringUtils.Left(purchaseOrder.SpecialInstructions7, 20);
        //    headerDnload.FIELD017 = purchaseOrder.BackOrder;
        //    headerDnload.FIELD018 = purchaseOrder.IsRMA ? "Y" : "N";
        //    headerDnload.FIELD019 = purchaseOrder.DueDate == DateTime.MinValue ? string.Empty : purchaseOrder.DueDate.ToString("yyyyMMdd");
        //    headerDnload.FIELD020 = StringUtils.Left(purchaseOrder.Extra1, 40);
        //    headerDnload.FIELD021 = StringUtils.Left(purchaseOrder.Extra2, 40);
        //    headerDnload.FIELD022 = StringUtils.Left(purchaseOrder.Extra3, 40);
        //    headerDnload.FIELD023 = StringUtils.Left(purchaseOrder.Extra4, 40);
        //    headerDnload.FIELD024 = StringUtils.Left(purchaseOrder.Extra5, 40);
        //    headerDnload.FIELD025 = StringUtils.Left(purchaseOrder.WarehouseTransferLocation, 10);

        //    switch (purchaseOrder.PoType)
        //    {
        //        case PoType.NotSpecified:
        //            headerDnload.FIELD026 = string.Empty;
        //            break;
        //        case PoType.Transfer:
        //            headerDnload.FIELD026 = "TRASNSFER";
        //            break;
        //        case PoType.Requisition:
        //            headerDnload.FIELD026 = "REQUISITION";
        //            break;
        //    }

        //    headerDnload.FIELD027 = StringUtils.Left(purchaseOrder.RequisitionNumber, 20);
        //    headerDnload.FIELD028 = StringUtils.Left(purchaseOrder.Email, 40);
        //    headerDnload.FIELD029 = StringUtils.Left(purchaseOrder.Packslip, 20);
        //    headerDnload.FIELD030 = StringUtils.Left(purchaseOrder.RequestPOD, 1);
        //    headerDnload.FIELD031 = StringUtils.Left(purchaseOrder.RequestSH, 1);
        //    headerDnload.FIELD032 = StringUtils.Left(purchaseOrder.NoSHReship, 1);
        //    headerDnload.FIELD033 = StringUtils.Left(purchaseOrder.OrderSource, 10); // Added for A1W 64
        //    headerDnload.FIELD034 = StringUtils.Left(purchaseOrder.IntegrationUDF1, 40); // Added for A1W 64
        //    headerDnload.FIELD035 = StringUtils.Left(purchaseOrder.IntegrationUDF2, 40); // Added for A1W 64
        //    headerDnload.FIELD036 = StringUtils.Left(purchaseOrder.IntegrationUDF3, 40); // Added for A1W 64
        //    headerDnload.FIELD037 = StringUtils.Left(purchaseOrder.IntegrationUDF4, 40); // Added for A1W 64
        //    headerDnload.FIELD038 = StringUtils.Left(purchaseOrder.IntegrationUDF5, 40); // Added for A1W 64
        //    headerDnload.FIELD039 = StringUtils.Left(purchaseOrder.IntegrationUDF6, 40); // Added for A1W 64
        //    headerDnload.FIELD040 = StringUtils.Left(purchaseOrder.IntegrationUDF7, 40); // Added for A1W 64
        //    headerDnload.FIELD041 = StringUtils.Left(purchaseOrder.IntegrationUDF8, 40); // Added for A1W 64
        //    headerDnload.FIELD042 = StringUtils.Left(purchaseOrder.IntegrationUDF9, 40); // Added for A1W 64
        //    headerDnload.FIELD043 = StringUtils.Left(purchaseOrder.IntegrationUDF10, 40); // Added for A1W 64
        //    headerDnload.FIELD044 = StringUtils.Left(purchaseOrder.IntegrationUDF11, 40); // Added for A1W 64
        //    headerDnload.FIELD045 = StringUtils.Left(purchaseOrder.IntegrationUDF12, 40); // Added for A1W 64
        //    headerDnload.FIELD046 = StringUtils.Left(purchaseOrder.IntegrationUDF13, 40); // Added for A1W 64
        //    headerDnload.FIELD047 = StringUtils.Left(purchaseOrder.IntegrationUDF14, 40); // Added for A1W 64
        //    headerDnload.FIELD048 = StringUtils.Left(purchaseOrder.IntegrationUDF15, 40); // Added for A1W 64
        //    headerDnload.FIELD049 = StringUtils.Left(purchaseOrder.IntegrationUDF16, 40); // Added for A1W 64
        //    headerDnload.FIELD050 = StringUtils.Left(purchaseOrder.IntegrationUDF17, 40); // Added for A1W 64
        //    headerDnload.FIELD051 = StringUtils.Left(purchaseOrder.IntegrationUDF18, 40); // Added for A1W 64
        //    headerDnload.FIELD052 = StringUtils.Left(purchaseOrder.IntegrationUDF19, 40); // Added for A1W 64
        //    headerDnload.FIELD053 = StringUtils.Left(purchaseOrder.IntegrationUDF20, 40); // Added for A1W 64
        //    headerDnload.FIELD054 = StringUtils.Left(purchaseOrder.IntegrationUDF21, 40); // Added for A1W 64
        //    headerDnload.FIELD055 = StringUtils.Left(purchaseOrder.IntegrationUDF22, 40); // Added for A1W 64
        //    headerDnload.FIELD056 = StringUtils.Left(purchaseOrder.IntegrationUDF23, 40); // Added for A1W 64
        //    headerDnload.FIELD057 = StringUtils.Left(purchaseOrder.IntegrationUDF24, 40); // Added for A1W 64
        //    headerDnload.FIELD058 = StringUtils.Left(purchaseOrder.IntegrationUDF25, 40); // Added for A1W 64

        //    dnloads.Add(headerDnload);
        //    #endregion

        //    return dnloads;
        //}
		#endregion

        //From WMS
        #region ParseReceiptConfirmation
        public static List<ReceiptConfirmation> ParseReceiptConfirmation(DataTable rcRecords)
        {
            if (rcRecords.Rows.Count == 0)
                return null;

            List<ReceiptConfirmation> receiptConfirmations = new List<ReceiptConfirmation>();
           
            foreach (DataRow row in rcRecords.Rows)
            {
                ReceiptConfirmation receipt = new ReceiptConfirmation();

                #region ReceiptConfirmation mapping
                //3. Client Name (10C)
                receipt.ClientName = row["FIELD003"] as string;

                //4. Container / Receiving Batch Reference (16C)
                receipt.ContainerReceivingBatchRef = row["FIELD004"] as string;

                //5. PO Number (16C)
                receipt.PurchaseOrderId = row["FIELD005"] as string;

                //13. Packing Slip Number (20C)
                receipt.PackingSlipId = row["FIELD013"] as string;

                //14. Carrier (16C)
                receipt.Carrier = row["FIELD014"] as string;

                //15. Assigned Container Cost
                receipt.AssignedContainerCost = row["FIELD015"] as string;

                //19. Shipment Payment Type (1C)
                receipt.ShipmentPaymentType = row["FIELD019"] as string;

                //26. Freight Status (1C)
                receipt.FreightStatus = row["FIELD026"] as string;

                //44. Extra1 (40C)
                receipt.Extra1 = row["FIELD044"] as string;

                //45. Extra2 (40C)
                receipt.Extra2 = row["FIELD045"] as string;

                //46. Extra3 (40C)
                receipt.Extra3 = row["FIELD046"] as string;

                //47. Extra4 (40C)
                receipt.Extra4 = row["FIELD047"] as string;

                //48. Extra5 (40C)
                receipt.Extra5 = row["FIELD048"] as string;

                //54. Receiving Backorder # (1C)
                receipt.BackOrderNum = row["FIELD054"] as string;

                //57. Vessel No (20C)
                receipt.VesselNum = row["FIELD057"] as string;

                //58. Date Left Port (D)
                if (row["FIELD058"].ToString().Length > 0)
                    receipt.DateLeftPort = DateTime.ParseExact(row["FIELD058"].ToString(), "yyyyMMdd", CultureInfo.InvariantCulture);

                //59. Est. Arrive Date(D)
                if (row["FIELD059"].ToString().Length > 0)
                    receipt.ArriveDate = DateTime.ParseExact(row["FIELD059"].ToString(), "yyyyMMdd", CultureInfo.InvariantCulture);

                //60. Vendor Number (16C)
                receipt.VendorId = row["FIELD060"] as string;

				//61. License Plate/Tracking Number for ASN Receipts (34C)
				receipt.LicensePlate = row["FIELD061"] as string;

				// 62 - 65. WMS UDFs (20C)
				receipt.WMSUDF1 = row["FIELD062"] as string; // Added/changed for A1W 64
				receipt.WMSUDF2 = row["FIELD063"] as string; // Added/changed for A1W 64
				receipt.WMSUDF3 = row["FIELD064"] as string; // Added/changed for A1W 64
				receipt.WMSUDF4 = row["FIELD065"] as string; // Added/changed for A1W 64

				// 68 - 92. Integration UDFs (40C)
				receipt.IntegrationUDF1 = row["FIELD068"] as string; // Added/changed for A1W 64
				receipt.IntegrationUDF2 = row["FIELD069"] as string; // Added/changed for A1W 64
				receipt.IntegrationUDF3 = row["FIELD070"] as string; // Added/changed for A1W 64
				receipt.IntegrationUDF4 = row["FIELD071"] as string; // Added/changed for A1W 64
				receipt.IntegrationUDF5 = row["FIELD072"] as string; // Added/changed for A1W 64
				receipt.IntegrationUDF6 = row["FIELD073"] as string; // Added/changed for A1W 64
				receipt.IntegrationUDF7 = row["FIELD074"] as string; // Added/changed for A1W 64
				receipt.IntegrationUDF8 = row["FIELD075"] as string; // Added/changed for A1W 64
				receipt.IntegrationUDF9 = row["FIELD076"] as string; // Added/changed for A1W 64
				receipt.IntegrationUDF10 = row["FIELD077"] as string; // Added/changed for A1W 64
				receipt.IntegrationUDF11 = row["FIELD078"] as string; // Added/changed for A1W 64
				receipt.IntegrationUDF12 = row["FIELD079"] as string; // Added/changed for A1W 64
				receipt.IntegrationUDF13 = row["FIELD080"] as string; // Added/changed for A1W 64
				receipt.IntegrationUDF14 = row["FIELD081"] as string; // Added/changed for A1W 64
				receipt.IntegrationUDF15 = row["FIELD082"] as string; // Added/changed for A1W 64
				receipt.IntegrationUDF16 = row["FIELD083"] as string; // Added/changed for A1W 64
				receipt.IntegrationUDF17 = row["FIELD084"] as string; // Added/changed for A1W 64
				receipt.IntegrationUDF18 = row["FIELD085"] as string; // Added/changed for A1W 64
				receipt.IntegrationUDF19 = row["FIELD086"] as string; // Added/changed for A1W 64
				receipt.IntegrationUDF20 = row["FIELD087"] as string; // Added/changed for A1W 64
				receipt.IntegrationUDF21 = row["FIELD088"] as string; // Added/changed for A1W 64
				receipt.IntegrationUDF22 = row["FIELD089"] as string; // Added/changed for A1W 64
				receipt.IntegrationUDF23 = row["FIELD090"] as string; // Added/changed for A1W 64
				receipt.IntegrationUDF24 = row["FIELD091"] as string; // Added/changed for A1W 64
				receipt.IntegrationUDF25 = row["FIELD092"] as string; // Added/changed for A1W 64
                #endregion

                #region ReceiptConfirmation line mapping
                ReceiptConfirmationLine line = new ReceiptConfirmationLine();
                line.RowId = row["ROWID"].ToString();

                //32. Tiebreaker (10N)
                line.Tiebreaker = row["FIELD032"] as string;

                //2. Warehouse (6C)
                line.Warehouse = row["FIELD002"] as string;

                //6. Line Number (20C)
                line.LineNum = row["FIELD006"] as string;

                if (!receipt.Lines.Contains(line))
                    receipt.Lines.Add(line);
                else
                    line = receipt.Lines[receipt.Lines.IndexOf(line)];

                //7. Product Code (20C)				
                line.ProductCode = row["FIELD007"] as string;

                //8. Quantity Expected (10N)
                if (row["FIELD008"].ToString().Length > 0)
                    line.QuantityExpected = Decimal.Parse(row["FIELD008"] as string);

                //9. Total Quantity Received, in Stock unit of measure (10N)
                if (row["FIELD009"].ToString().Length > 0)
                    line.QuantityReceived = Decimal.Parse(row["FIELD009"] as string);

                //11. Received on date (D)
                if (row["FIELD011"].ToString().Length > 0)
                    line.ReceivedOnDate = DateTime.ParseExact(row["FIELD011"].ToString(), "yyyyMMdd", CultureInfo.InvariantCulture);

                //12. Received by (16C)
                line.ReceivedBy = row["FIELD012"] as string;

                //33. RMA Code (2C)
                line.RMACode = row["FIELD033"] as string;

				//39. Expiry Date (D)
				if (row["FIELD039"].ToString().Length > 0)
					line.ExpiryDate = DateTime.ParseExact(row["FIELD039"].ToString(), "yyyyMMdd", CultureInfo.InvariantCulture);

                //40. Receive Putaway Location (8C)
                line.ReceivePutawayLocation = row["FIELD040"] as string;

                //41. Selling Price (9.2N)
                if (row["FIELD041"].ToString().Length > 0)
                    line.SellingPrice = Decimal.Parse(row["FIELD041"] as string);

                //42. Discount (8.2N)
                if (row["FIELD042"].ToString().Length > 0)
                    line.Discount = Decimal.Parse(row["FIELD042"] as string);

                //43. Cost Price (9.2N)
                if (row["FIELD043"].ToString().Length > 0)
                    line.CostPrice = Decimal.Parse(row["FIELD043"] as string);

                //49. Extra1 (40C)
                line.Extra1 = row["FIELD049"] as string;

                //50. Extra2 (40C)
                line.Extra2 = row["FIELD050"] as string;

                //51. Extra3 (40C)
                line.Extra3 = row["FIELD051"] as string;

                //52. Extra4 (40C)
                line.Extra4 = row["FIELD052"] as string;

                //53. Extra5 (40C)
                line.Extra5 = row["FIELD053"] as string;

				// 66 - 67. WMS UDFs (20C)
				line.WMSUDF1 = row["FIELD066"] as string; // Added/changed for A1W 64
				line.WMSUDF2 = row["FIELD067"] as string; // Added/changed for A1W 64

				// 93 - 117. Integration UDFs (40C)
				line.IntegrationUDF1 = row["FIELD093"] as string; // Added/changed for A1W 64
				line.IntegrationUDF2 = row["FIELD094"] as string; // Added/changed for A1W 64
				line.IntegrationUDF3 = row["FIELD095"] as string; // Added/changed for A1W 64
				line.IntegrationUDF4 = row["FIELD096"] as string; // Added/changed for A1W 64
				line.IntegrationUDF5 = row["FIELD097"] as string; // Added/changed for A1W 64
				line.IntegrationUDF6 = row["FIELD098"] as string; // Added/changed for A1W 64
				line.IntegrationUDF7 = row["FIELD099"] as string; // Added/changed for A1W 64
				line.IntegrationUDF8 = row["FIELD100"] as string; // Added/changed for A1W 64
				line.IntegrationUDF9 = row["FIELD101"] as string; // Added/changed for A1W 64
				line.IntegrationUDF10 = row["FIELD102"] as string; // Added/changed for A1W 64
				line.IntegrationUDF11 = row["FIELD103"] as string; // Added/changed for A1W 64
				line.IntegrationUDF12 = row["FIELD104"] as string; // Added/changed for A1W 64
				line.IntegrationUDF13 = row["FIELD105"] as string; // Added/changed for A1W 64
				line.IntegrationUDF14 = row["FIELD106"] as string; // Added/changed for A1W 64
				line.IntegrationUDF15 = row["FIELD107"] as string; // Added/changed for A1W 64
				line.IntegrationUDF16 = row["FIELD108"] as string; // Added/changed for A1W 64
				line.IntegrationUDF17 = row["FIELD109"] as string; // Added/changed for A1W 64
				line.IntegrationUDF18 = row["FIELD110"] as string; // Added/changed for A1W 64
				line.IntegrationUDF19 = row["FIELD111"] as string; // Added/changed for A1W 64
				line.IntegrationUDF20 = row["FIELD112"] as string; // Added/changed for A1W 64
				line.IntegrationUDF21 = row["FIELD113"] as string; // Added/changed for A1W 64
				line.IntegrationUDF22 = row["FIELD114"] as string; // Added/changed for A1W 64
				line.IntegrationUDF23 = row["FIELD115"] as string; // Added/changed for A1W 64
				line.IntegrationUDF24 = row["FIELD116"] as string; // Added/changed for A1W 64
				line.IntegrationUDF25 = row["FIELD117"] as string; // Added/changed for A1W 64
                #endregion

                #region Attributes
                AttributeSet set = new AttributeSet();
                set.RowId = row["ROWID"].ToString();

                //20. Attribute 1 (25C)
                set.Attribute1 = row["FIELD020"] as string;

                //21. Attribute 2 (25C)
                set.Attribute2 = row["FIELD021"] as string;

                //22. Attribute 3 (25C)
                set.Attribute3 = row["FIELD022"] as string;

                //23. Attribute 4 (6C)
                set.Attribute4 = row["FIELD023"] as string;

                //24. Attribute 5 (6C)
                set.Attribute5 = row["FIELD024"] as string;

                //25. Attribute 6 (6C)
                set.Attribute6 = row["FIELD025"] as string;

                //35. Attribute 7 (6C)
                set.Attribute7 = row["FIELD035"] as string;

                //36. Attribute 8 (6C)
                set.Attribute8 = row["FIELD036"] as string;

                //37. Attribute 9 (6C)
                set.Attribute9 = row["FIELD037"] as string;

                //38. Attribute 10 (6C)
                set.Attribute10 = row["FIELD038"] as string;

                if (row["FIELD039"].ToString().Length > 0)
                    set.ExpiryDate = DateTime.ParseExact(row["FIELD039"].ToString(), "yyyyMMdd", CultureInfo.InvariantCulture);

                //27. Quantity (N) This record
                decimal decQty;     //SVA Added
                Decimal.TryParse(row["FIELD027"].ToString(), out decQty);   //SVA Converted from IF/Parse to TryParse
                set.Quantity = decQty;

                if (!set.Empy && !line.Attributes.Contains(set))
                {
                    line.Attributes.Add(set);
                }
                else if (!set.Empy)
                {
                    set = line.Attributes[line.Attributes.IndexOf(set)];
                    set.Quantity += decQty;
                }
                #endregion

                receiptConfirmations.Add(receipt);
            }

            return receiptConfirmations;
        }
        #endregion

		#region A1W Middle Tier DTO to ReceiptConfirmation
        //public static ReceiptConfirmation DTOToReceiptConfirmation(IList<UPLOAD> uploads)
        //{
        //    ReceiptConfirmation receipt = new ReceiptConfirmation();

        //    foreach (UPLOAD upload in uploads)
        //    {
        //        #region ReceiptConfirmation mapping
        //        receipt.ClientName = upload.FIELD003;
        //        receipt.ContainerReceivingBatchRef = upload.FIELD004;
        //        receipt.PurchaseOrderId = upload.FIELD005;
        //        receipt.PackingSlipId = upload.FIELD013;
        //        receipt.Carrier = upload.FIELD014;
        //        receipt.AssignedContainerCost = upload.FIELD015;
        //        receipt.ShipmentPaymentType = upload.FIELD019;
        //        receipt.FreightStatus = upload.FIELD026;
        //        receipt.Extra1 = upload.FIELD044;
        //        receipt.Extra2 = upload.FIELD045;
        //        receipt.Extra3 = upload.FIELD046;
        //        receipt.Extra4 = upload.FIELD047;
        //        receipt.Extra5 = upload.FIELD048;
        //        receipt.BackOrderNum = upload.FIELD054;
        //        receipt.VesselNum = upload.FIELD057;

        //        if (upload.FIELD058.Length > 0)
        //            receipt.DateLeftPort = DateTime.ParseExact(upload.FIELD058, "yyyyMMdd", CultureInfo.InvariantCulture);

        //        if (upload.FIELD059.Length > 0)
        //            receipt.ArriveDate = DateTime.ParseExact(upload.FIELD059, "yyyyMMdd", CultureInfo.InvariantCulture);

        //        receipt.VendorId = upload.FIELD060;
        //        receipt.LicensePlate = upload.FIELD061;
        //        receipt.WMSUDF1 = upload.FIELD062; // Added/changed for A1W 64
        //        receipt.WMSUDF2 = upload.FIELD063; // Added/changed for A1W 64
        //        receipt.WMSUDF3 = upload.FIELD064; // Added/changed for A1W 64
        //        receipt.WMSUDF4 = upload.FIELD065; // Added/changed for A1W 64
        //        receipt.IntegrationUDF1 = upload.FIELD068; // Added/changed for A1W 64
        //        receipt.IntegrationUDF2 = upload.FIELD069; // Added/changed for A1W 64
        //        receipt.IntegrationUDF3 = upload.FIELD070; // Added/changed for A1W 64
        //        receipt.IntegrationUDF4 = upload.FIELD071; // Added/changed for A1W 64
        //        receipt.IntegrationUDF5 = upload.FIELD072; // Added/changed for A1W 64
        //        receipt.IntegrationUDF6 = upload.FIELD073; // Added/changed for A1W 64
        //        receipt.IntegrationUDF7 = upload.FIELD074; // Added/changed for A1W 64
        //        receipt.IntegrationUDF8 = upload.FIELD075; // Added/changed for A1W 64
        //        receipt.IntegrationUDF9 = upload.FIELD076; // Added/changed for A1W 64
        //        receipt.IntegrationUDF10 = upload.FIELD077; // Added/changed for A1W 64
        //        receipt.IntegrationUDF11 = upload.FIELD078; // Added/changed for A1W 64
        //        receipt.IntegrationUDF12 = upload.FIELD079; // Added/changed for A1W 64
        //        receipt.IntegrationUDF13 = upload.FIELD080; // Added/changed for A1W 64
        //        receipt.IntegrationUDF14 = upload.FIELD081; // Added/changed for A1W 64
        //        receipt.IntegrationUDF15 = upload.FIELD082; // Added/changed for A1W 64
        //        receipt.IntegrationUDF16 = upload.FIELD083; // Added/changed for A1W 64
        //        receipt.IntegrationUDF17 = upload.FIELD084; // Added/changed for A1W 64
        //        receipt.IntegrationUDF18 = upload.FIELD085; // Added/changed for A1W 64
        //        receipt.IntegrationUDF19 = upload.FIELD086; // Added/changed for A1W 64
        //        receipt.IntegrationUDF20 = upload.FIELD087; // Added/changed for A1W 64
        //        receipt.IntegrationUDF21 = upload.FIELD088; // Added/changed for A1W 64
        //        receipt.IntegrationUDF22 = upload.FIELD089; // Added/changed for A1W 64
        //        receipt.IntegrationUDF23 = upload.FIELD090; // Added/changed for A1W 64
        //        receipt.IntegrationUDF24 = upload.FIELD091; // Added/changed for A1W 64
        //        receipt.IntegrationUDF25 = upload.FIELD092; // Added/changed for A1W 64
        //        #endregion

        //        #region ReceiptConfirmation line mapping
        //        ReceiptConfirmationLine line = new ReceiptConfirmationLine();

        //        line.RowId = upload.ROWID.ToString();
        //        line.Tiebreaker = upload.FIELD032;
        //        line.Warehouse = upload.FIELD002;
        //        line.LineNum = upload.FIELD006;

        //        if (!receipt.Lines.Contains(line))
        //            receipt.Lines.Add(line);
        //        else
        //            line = receipt.Lines[receipt.Lines.IndexOf(line)];

        //        line.ProductCode = upload.FIELD007;

        //        if (upload.FIELD008.Length > 0)
        //            line.QuantityExpected = Decimal.Parse(upload.FIELD008);

        //        if (upload.FIELD009.Length > 0)
        //            line.QuantityReceived = Decimal.Parse(upload.FIELD009);

        //        if (upload.FIELD011.Length > 0)
        //            line.ReceivedOnDate = DateTime.ParseExact(upload.FIELD011, "yyyyMMdd", CultureInfo.InvariantCulture);

        //        line.ReceivedBy = upload.FIELD012;
        //        line.RMACode = upload.FIELD033;

        //        if (upload.FIELD039.Length > 0)
        //            line.ExpiryDate = DateTime.ParseExact(upload.FIELD039, "yyyyMMdd", CultureInfo.InvariantCulture);

        //        line.ReceivePutawayLocation = upload.FIELD040;

        //        if (upload.FIELD041.Length > 0)
        //            line.SellingPrice = Decimal.Parse(upload.FIELD041);

        //        if (upload.FIELD042.Length > 0)
        //            line.Discount = Decimal.Parse(upload.FIELD042);

        //        if (upload.FIELD043.Length > 0)
        //            line.CostPrice = Decimal.Parse(upload.FIELD043);

        //        line.Extra1 = upload.FIELD049;
        //        line.Extra2 = upload.FIELD050;
        //        line.Extra3 = upload.FIELD051;
        //        line.Extra4 = upload.FIELD052;
        //        line.Extra5 = upload.FIELD053;
        //        line.WMSUDF1 = upload.FIELD066; // Added/changed for A1W 64
        //        line.WMSUDF2 = upload.FIELD067; // Added/changed for A1W 64
        //        line.IntegrationUDF1 = upload.FIELD093; // Added/changed for A1W 64
        //        line.IntegrationUDF2 = upload.FIELD094; // Added/changed for A1W 64
        //        line.IntegrationUDF3 = upload.FIELD095; // Added/changed for A1W 64
        //        line.IntegrationUDF4 = upload.FIELD096; // Added/changed for A1W 64
        //        line.IntegrationUDF5 = upload.FIELD097; // Added/changed for A1W 64
        //        line.IntegrationUDF6 = upload.FIELD098; // Added/changed for A1W 64
        //        line.IntegrationUDF7 = upload.FIELD099; // Added/changed for A1W 64
        //        line.IntegrationUDF8 = upload.FIELD100; // Added/changed for A1W 64
        //        line.IntegrationUDF9 = upload.FIELD101; // Added/changed for A1W 64
        //        line.IntegrationUDF10 = upload.FIELD102; // Added/changed for A1W 64
        //        line.IntegrationUDF11 = upload.FIELD103; // Added/changed for A1W 64
        //        line.IntegrationUDF12 = upload.FIELD104; // Added/changed for A1W 64
        //        line.IntegrationUDF13 = upload.FIELD105; // Added/changed for A1W 64
        //        line.IntegrationUDF14 = upload.FIELD106; // Added/changed for A1W 64
        //        line.IntegrationUDF15 = upload.FIELD107; // Added/changed for A1W 64
        //        line.IntegrationUDF16 = upload.FIELD108; // Added/changed for A1W 64
        //        line.IntegrationUDF17 = upload.FIELD109; // Added/changed for A1W 64
        //        line.IntegrationUDF18 = upload.FIELD110; // Added/changed for A1W 64
        //        line.IntegrationUDF19 = upload.FIELD111; // Added/changed for A1W 64
        //        line.IntegrationUDF20 = upload.FIELD112; // Added/changed for A1W 64
        //        line.IntegrationUDF21 = upload.FIELD113; // Added/changed for A1W 64
        //        line.IntegrationUDF22 = upload.FIELD114; // Added/changed for A1W 64
        //        line.IntegrationUDF23 = upload.FIELD115; // Added/changed for A1W 64
        //        line.IntegrationUDF24 = upload.FIELD116; // Added/changed for A1W 64
        //        line.IntegrationUDF25 = upload.FIELD117; // Added/changed for A1W 64
        //        #endregion

        //        #region Attributes
        //        AttributeSet set = new AttributeSet();

        //        set.RowId = upload.ROWID.ToString();
        //        set.Attribute1 = upload.FIELD020;
        //        set.Attribute2 = upload.FIELD021;
        //        set.Attribute3 = upload.FIELD022;
        //        set.Attribute4 = upload.FIELD023;
        //        set.Attribute5 = upload.FIELD024;
        //        set.Attribute6 = upload.FIELD025;
        //        set.Attribute7 = upload.FIELD035;
        //        set.Attribute8 = upload.FIELD036;
        //        set.Attribute9 = upload.FIELD037;
        //        set.Attribute10 = upload.FIELD038;

        //        if (upload.FIELD039.Length > 0)
        //            set.ExpiryDate = DateTime.ParseExact(upload.FIELD039, "yyyyMMdd", CultureInfo.InvariantCulture);

        //        if (!set.Empy && !line.Attributes.Contains(set))
        //            line.Attributes.Add(set);
        //        else if (!set.Empy)
        //            set = line.Attributes[line.Attributes.IndexOf(set)];

        //        //27. Quantity (N) This record
        //        if (upload.FIELD027.Length > 0)
        //            set.Quantity += Decimal.Parse(upload.FIELD027);
        //        #endregion
        //    }

        //    return receipt;
        //}
		#endregion
    }
}
