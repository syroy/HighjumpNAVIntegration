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
        #region ParseInventoryItem
        public static DataTable ParseInventoryItem(InventoryItem item)
        {
            DataTable result = CompanyConfig.Instance.DownloadTableSchema;

            DataRow row = result.NewRow();
            row["FIELD001"] = "VA";
            row["FIELD002"] = item.ProductCode;
            row["FIELD003"] = item.VendorProductNumber;
			row["FIELD004"] = StringUtils.Left(item.Description, 100); // Changed to 100 characters for A1W 64
            row["FIELD005"] = item.VendorNumber;
            row["FIELD006"] = item.QuantityAvailable.ToString(CultureInfo.InvariantCulture);
			row["FIELD007"] = StringUtils.Left(item.OverReceiptPercentage, 6); // Added for A1W 64
            row["FIELD008"] = StringUtils.Left(item.UPC, 14);
            row["FIELD009"] = item.Cost.ToString(CultureInfo.InvariantCulture);
            row["FIELD010"] = item.RetailPrice.ToString(CultureInfo.InvariantCulture);
            row["FIELD011"] = item.WholesalePacksize.ToString(CultureInfo.InvariantCulture);
			row["FIELD012"] = item.QuantityPrecision.ToString(CultureInfo.InvariantCulture); // Added for A1W 64
			row["FIELD013"] = item.PacksizePrecision.ToString(CultureInfo.InvariantCulture); // Added for A1W 64
            row["FIELD014"] = item.MinimumDaysToExpiry;
            row["FIELD015"] = StringUtils.Left(item.SupplierType, 1);
            row["FIELD016"] = StringUtils.Left(item.BinLocation, 8);
            row["FIELD019"] = StringUtils.Left(item.ClientName, 10);
            row["FIELD020"] = ParseTracking(item.PickAttributeTracking);
            row["FIELD021"] = ParseTracking(item.ReceiveAttributeTracking);
            row["FIELD032"] = item.ExpiryDateRequired ? "Y" : "N";
            row["FIELD033"] = StringUtils.Left(item.Weight.ToString(CultureInfo.InvariantCulture), 14);
            row["FIELD034"] = StringUtils.Left(item.Cubage.ToString(CultureInfo.InvariantCulture), 20);
            row["FIELD035"] = StringUtils.Left(item.Height.ToString(CultureInfo.InvariantCulture), 11);
            row["FIELD036"] = StringUtils.Left(item.Width.ToString(CultureInfo.InvariantCulture), 11);
            row["FIELD037"] = StringUtils.Left(item.Length.ToString(CultureInfo.InvariantCulture), 11);
            row["FIELD038"] = StringUtils.Left(item.Size1.ToString(CultureInfo.InvariantCulture), 6);
            row["FIELD039"] = StringUtils.Left(item.Size2.ToString(CultureInfo.InvariantCulture), 6);
            row["FIELD040"] = StringUtils.Left(item.Size3.ToString(CultureInfo.InvariantCulture), 6);
            row["FIELD041"] = StringUtils.Left(item.Size4.ToString(CultureInfo.InvariantCulture), 6);
            row["FIELD042"] = StringUtils.Left(item.Size5.ToString(CultureInfo.InvariantCulture), 6);
            row["FIELD042"] = StringUtils.Left(item.Size5.ToString(CultureInfo.InvariantCulture), 6);
            row["FIELD043"] = StringUtils.Left(item.ProductClass, 6);
            row["FIELD044"] = StringUtils.Left(item.OrderGroup, 20);
            row["FIELD045"] = StringUtils.Left(item.ReorderPoint.ToString(), 6);
            row["FIELD046"] = StringUtils.Left(item.Buyer, 10);
			//row["FIELD047"] = WMSAdapterUtils.Left(item.GroupLimit1.ToString(CultureInfo.InvariantCulture), 6); Removed for A1W 64
			//row["FIELD048"] = WMSAdapterUtils.Left(item.GroupLimit2.ToString(CultureInfo.InvariantCulture), 6); Removed for A1W 64
			//row["FIELD049"] = WMSAdapterUtils.Left(item.GroupLimit3.ToString(CultureInfo.InvariantCulture), 6); Removed for A1W 64
			//row["FIELD050"] = WMSAdapterUtils.Left(item.GroupLimit4.ToString(CultureInfo.InvariantCulture), 6); Removed for A1W 64
			//row["FIELD051"] = WMSAdapterUtils.Left(item.GroupLimit5.ToString(CultureInfo.InvariantCulture), 6); Removed for A1W 64
			//row["FIELD052"] = WMSAdapterUtils.Left(item.GroupLimit6.ToString(CultureInfo.InvariantCulture), 6); Removed for A1W 64
			//row["FIELD053"] = WMSAdapterUtils.Left(item.GroupLimit7.ToString(CultureInfo.InvariantCulture), 6); Removed for A1W 64
			//row["FIELD054"] = WMSAdapterUtils.Left(item.GroupLimit8.ToString(CultureInfo.InvariantCulture), 6); Removed for A1W 64
			//row["FIELD055"] = WMSAdapterUtils.Left(item.GroupLimit9.ToString(CultureInfo.InvariantCulture), 6); Removed for A1W 64
            row["FIELD056"] = StringUtils.Left(item.EDPNum, 10);
            row["FIELD057"] = StringUtils.Left(item.MinimumReplenishmentLevel1.ToString(CultureInfo.InvariantCulture), 6);
            row["FIELD058"] = StringUtils.Left(item.MinimumReplenishmentLevel2.ToString(CultureInfo.InvariantCulture), 6);
            row["FIELD059"] = StringUtils.Left(item.MinimumReplenishmentLevel3.ToString(CultureInfo.InvariantCulture), 6);
            row["FIELD060"] = StringUtils.Left(item.MinimumReplenishmentLevel4.ToString(CultureInfo.InvariantCulture), 6);
            row["FIELD061"] = StringUtils.Left(item.MinimumReplenishmentLevel5.ToString(CultureInfo.InvariantCulture), 6);
            row["FIELD062"] = StringUtils.Left(item.MaximumReplenishmentLevel1.ToString(CultureInfo.InvariantCulture), 6);
            row["FIELD063"] = StringUtils.Left(item.MaximumReplenishmentLevel2.ToString(CultureInfo.InvariantCulture), 6);
            row["FIELD064"] = StringUtils.Left(item.MaximumReplenishmentLevel3.ToString(CultureInfo.InvariantCulture), 6);
            row["FIELD065"] = StringUtils.Left(item.MaximumReplenishmentLevel4.ToString(CultureInfo.InvariantCulture), 6);
            row["FIELD066"] = StringUtils.Left(item.MaximumReplenishmentLevel5.ToString(CultureInfo.InvariantCulture), 6);
            row["FIELD067"] = StringUtils.Left(item.CartonDef.ToString(CultureInfo.InvariantCulture), 8);
            row["FIELD068"] = StringUtils.Left(item.TariffCode.ToString(CultureInfo.InvariantCulture), 10);
            row["FIELD069"] = StringUtils.Left(item.CommodityCode.ToString(CultureInfo.InvariantCulture), 6);
            //row["FIELD070"] = WMSAdapterUtils.Left(item.InnerSize3.ToString(CultureInfo.InvariantCulture), 6); Removed for A1W 64
			//row["FIELD071"] = WMSAdapterUtils.Left(item.InnerSize4.ToString(CultureInfo.InvariantCulture), 6); Removed for A1W 64
			//row["FIELD072"] = WMSAdapterUtils.Left(item.InnerSize5.ToString(CultureInfo.InvariantCulture), 6); Removed for A1W 64
            row["FIELD073"] = StringUtils.Left(item.BeeLine, 8);
            row["FIELD074"] = StringUtils.Left(item.HandleCode, 1);
            row["FIELD075"] = StringUtils.Left(item.GlobalCoreReservation.ToString(CultureInfo.InvariantCulture), 8);
            row["FIELD076"] = item.BypassQuantity.ToString(CultureInfo.InvariantCulture);
           // row["FIELD077"] = item.CommodityCode;
            row["FIELD078"] = StringUtils.Left(item.CartonSize, 20);
            row["FIELD079"] = StringUtils.Left(item.AverageUnits.ToString(CultureInfo.InvariantCulture), 12);
            row["FIELD080"] = StringUtils.Left(item.ProductUDF1, 30);
            row["FIELD081"] = StringUtils.Left(item.ProductUDF2, 30);
            row["FIELD082"] = StringUtils.Left(item.ProductUDF3, 30);
            row["FIELD083"] = StringUtils.Left(item.ProductUDF4, 30);
            row["FIELD084"] = StringUtils.Left(item.ProductUDF5, 30);
            row["FIELD089"] = StringUtils.Left(item.FormalSize1.Length.ToString(), 11);
            row["FIELD090"] = StringUtils.Left(item.FormalSize1.Width.ToString(), 11);
            row["FIELD091"] = StringUtils.Left(item.FormalSize1.Height.ToString(), 11);
            row["FIELD092"] = StringUtils.Left(item.FormalSize1.Weight.ToString(), 20);
            row["FIELD093"] = StringUtils.Left(item.FormalSize2.Length.ToString(), 11);
            row["FIELD094"] = StringUtils.Left(item.FormalSize2.Width.ToString(), 11);
            row["FIELD095"] = StringUtils.Left(item.FormalSize2.Height.ToString(), 11);
            row["FIELD096"] = StringUtils.Left(item.FormalSize2.Weight.ToString(), 20);
            row["FIELD097"] = StringUtils.Left(item.FormalSize3.Length.ToString(), 11);
            row["FIELD098"] = StringUtils.Left(item.FormalSize3.Width.ToString(), 11);
            row["FIELD099"] = StringUtils.Left(item.FormalSize3.Height.ToString(), 11);
            row["FIELD100"] = StringUtils.Left(item.FormalSize3.Weight.ToString(), 20);
            row["FIELD101"] = StringUtils.Left(item.FormalSize4.Length.ToString(), 11);
            row["FIELD102"] = StringUtils.Left(item.FormalSize4.Width.ToString(), 11);
            row["FIELD103"] = StringUtils.Left(item.FormalSize4.Height.ToString(), 11);
            row["FIELD104"] = StringUtils.Left(item.FormalSize4.Weight.ToString(), 20);
            row["FIELD105"] = StringUtils.Left(item.FormalSize5.Length.ToString(), 11);
            row["FIELD106"] = StringUtils.Left(item.FormalSize5.Width.ToString(), 11);
            row["FIELD107"] = StringUtils.Left(item.FormalSize5.Height.ToString(), 11);
            row["FIELD108"] = StringUtils.Left(item.FormalSize5.Weight.ToString(), 20);
            row["FIELD109"] = StringUtils.Left(item.PackClass, 10);
            row["FIELD110"] = StringUtils.Left(item.LicensePlate, 22);
            row["FIELD111"] = StringUtils.Left(item.Size1ItemType, 20);
            row["FIELD112"] = StringUtils.Left(item.Size2ItemType, 20);
            row["FIELD113"] = StringUtils.Left(item.Size3ItemType, 20);
            row["FIELD114"] = StringUtils.Left(item.Size4ItemType, 20);
            row["FIELD115"] = StringUtils.Left(item.Size5ItemType, 20);
            row["FIELD116"] = StringUtils.Left(item.CountryofOrigin, 20);

            if (item.Attributes.Count == 0)
                result.Rows.Add(row);
            else
            {
                foreach (AttributeSet attributes in item.Attributes)
                {
                    DataRow attrRow = result.NewRow();
                    attrRow.ItemArray = row.ItemArray;

                    attrRow["FIELD022"] = attributes.Attribute1;
                    attrRow["FIELD023"] = attributes.Attribute2;
                    attrRow["FIELD024"] = attributes.Attribute3;
                    attrRow["FIELD025"] = attributes.Attribute4;
                    attrRow["FIELD026"] = attributes.Attribute5;
                    attrRow["FIELD027"] = attributes.Attribute6;
                    attrRow["FIELD028"] = attributes.Attribute7;
                    attrRow["FIELD029"] = attributes.Attribute8;
                    attrRow["FIELD030"] = attributes.Attribute9;
                    attrRow["FIELD031"] = attributes.Attribute10;

                    result.Rows.Add(attrRow);
                }
            }
            return result;
        }
        #endregion

		#region InventoryItem to A1W Middle Tier DTO
		//public static DNLOAD InventoryItemToDTO(InventoryItem item)
        //{
        //    DNLOAD dnload = new DNLOAD();

        //    dnload.FIELD001 = "VA";
        //    dnload.FIELD002 = item.ProductCode;
        //    dnload.FIELD003 = item.VendorProductNumber;
        //    dnload.FIELD004 = StringUtils.Left(item.Description, 100); // Changed to 100 characters for A1W 64
        //    dnload.FIELD005 = item.VendorNumber;
        //    dnload.FIELD006 = item.QuantityAvailable.ToString(CultureInfo.InvariantCulture);
        //    dnload.FIELD007 = StringUtils.Left(item.OverReceiptPercentage, 6); // Added for A1W 64
        //    dnload.FIELD008 = StringUtils.Left(item.UPC, 14);
        //    dnload.FIELD009 = item.Cost.ToString(CultureInfo.InvariantCulture);
        //    dnload.FIELD010 = item.RetailPrice.ToString(CultureInfo.InvariantCulture);
        //    dnload.FIELD011 = item.WholesalePacksize.ToString(CultureInfo.InvariantCulture);
        //    dnload.FIELD012 = item.QuantityPrecision.ToString(CultureInfo.InvariantCulture); // Added for A1W 64
        //    dnload.FIELD013 = item.PacksizePrecision.ToString(CultureInfo.InvariantCulture); // Added for A1W 64
        //    dnload.FIELD014 = item.MinimumDaysToExpiry;
        //    dnload.FIELD015 = StringUtils.Left(item.SupplierType, 1);
        //    dnload.FIELD016 = StringUtils.Left(item.BinLocation, 8);
        //    dnload.FIELD019 = StringUtils.Left(item.ClientName, 10);
        //    dnload.FIELD020 = ParseTracking(item.PickAttributeTracking);
        //    dnload.FIELD021 = ParseTracking(item.ReceiveAttributeTracking);

        //    foreach (AttributeSet attributes in item.Attributes)
        //    {
        //        dnload.FIELD022 = attributes.Attribute1;
        //        dnload.FIELD023 = attributes.Attribute2;
        //        dnload.FIELD024 = attributes.Attribute3;
        //        dnload.FIELD025 = attributes.Attribute4;
        //        dnload.FIELD026 = attributes.Attribute5;
        //        dnload.FIELD027 = attributes.Attribute6;
        //        dnload.FIELD028 = attributes.Attribute7;
        //        dnload.FIELD029 = attributes.Attribute8;
        //        dnload.FIELD030 = attributes.Attribute9;
        //        dnload.FIELD031 = attributes.Attribute10;
        //    }

        //    dnload.FIELD032 = item.ExpiryDateRequired ? "Y" : "N";
        //    dnload.FIELD033 = StringUtils.Left(item.Weight.ToString(CultureInfo.InvariantCulture), 14);
        //    dnload.FIELD034 = StringUtils.Left(item.Cubage.ToString(CultureInfo.InvariantCulture), 20);
        //    dnload.FIELD035 = StringUtils.Left(item.Height.ToString(CultureInfo.InvariantCulture), 11);
        //    dnload.FIELD036 = StringUtils.Left(item.Width.ToString(CultureInfo.InvariantCulture), 11);
        //    dnload.FIELD037 = StringUtils.Left(item.Length.ToString(CultureInfo.InvariantCulture), 11);
        //    dnload.FIELD038 = StringUtils.Left(item.Size1.ToString(CultureInfo.InvariantCulture), 6);
        //    dnload.FIELD039 = StringUtils.Left(item.Size2.ToString(CultureInfo.InvariantCulture), 6);
        //    dnload.FIELD040 = StringUtils.Left(item.Size3.ToString(CultureInfo.InvariantCulture), 6);
        //    dnload.FIELD041 = StringUtils.Left(item.Size4.ToString(CultureInfo.InvariantCulture), 6);
        //    dnload.FIELD042 = StringUtils.Left(item.Size5.ToString(CultureInfo.InvariantCulture), 6);
        //    dnload.FIELD042 = StringUtils.Left(item.Size5.ToString(CultureInfo.InvariantCulture), 6);
        //    dnload.FIELD043 = StringUtils.Left(item.ProductClass, 6);
        //    dnload.FIELD044 = StringUtils.Left(item.OrderGroup, 20);
        //    dnload.FIELD045 = StringUtils.Left(item.ReorderPoint.ToString(), 6);
        //    dnload.FIELD046 = StringUtils.Left(item.Buyer, 10);
        //    dnload.FIELD056 = StringUtils.Left(item.EDPNum, 10);
        //    dnload.FIELD057 = StringUtils.Left(item.MinimumReplenishmentLevel1.ToString(CultureInfo.InvariantCulture), 6);
        //    dnload.FIELD058 = StringUtils.Left(item.MinimumReplenishmentLevel2.ToString(CultureInfo.InvariantCulture), 6);
        //    dnload.FIELD059 = StringUtils.Left(item.MinimumReplenishmentLevel3.ToString(CultureInfo.InvariantCulture), 6);
        //    dnload.FIELD060 = StringUtils.Left(item.MinimumReplenishmentLevel4.ToString(CultureInfo.InvariantCulture), 6);
        //    dnload.FIELD061 = StringUtils.Left(item.MinimumReplenishmentLevel5.ToString(CultureInfo.InvariantCulture), 6);
        //    dnload.FIELD062 = StringUtils.Left(item.MaximumReplenishmentLevel1.ToString(CultureInfo.InvariantCulture), 6);
        //    dnload.FIELD063 = StringUtils.Left(item.MaximumReplenishmentLevel2.ToString(CultureInfo.InvariantCulture), 6);
        //    dnload.FIELD064 = StringUtils.Left(item.MaximumReplenishmentLevel3.ToString(CultureInfo.InvariantCulture), 6);
        //    dnload.FIELD065 = StringUtils.Left(item.MaximumReplenishmentLevel4.ToString(CultureInfo.InvariantCulture), 6);
        //    dnload.FIELD066 = StringUtils.Left(item.MaximumReplenishmentLevel5.ToString(CultureInfo.InvariantCulture), 6);
        //    dnload.FIELD067 = StringUtils.Left(item.CartonDef.ToString(CultureInfo.InvariantCulture), 8);
        //    dnload.FIELD073 = StringUtils.Left(item.BeeLine, 8);
        //    dnload.FIELD074 = StringUtils.Left(item.HandleCode, 1);
        //    dnload.FIELD075 = StringUtils.Left(item.GlobalCoreReservation.ToString(CultureInfo.InvariantCulture), 8);
        //    dnload.FIELD076 = item.BypassQuantity.ToString(CultureInfo.InvariantCulture);
        //    dnload.FIELD077 = item.CommodityCode;
        //    dnload.FIELD078 = StringUtils.Left(item.CartonSize, 20);
        //    dnload.FIELD079 = StringUtils.Left(item.AverageUnits.ToString(CultureInfo.InvariantCulture), 12);
        //    dnload.FIELD080 = StringUtils.Left(item.ProductUDF1, 30);
        //    dnload.FIELD081 = StringUtils.Left(item.ProductUDF2, 30);
        //    dnload.FIELD082 = StringUtils.Left(item.ProductUDF3, 30);
        //    dnload.FIELD083 = StringUtils.Left(item.ProductUDF4, 30);
        //    dnload.FIELD084 = StringUtils.Left(item.ProductUDF5, 30);
        //    dnload.FIELD089 = StringUtils.Left(item.FormalSize1.Length.ToString(), 11);
        //    dnload.FIELD090 = StringUtils.Left(item.FormalSize1.Width.ToString(), 11);
        //    dnload.FIELD091 = StringUtils.Left(item.FormalSize1.Height.ToString(), 11);
        //    dnload.FIELD092 = StringUtils.Left(item.FormalSize1.Weight.ToString(), 20);
        //    dnload.FIELD093 = StringUtils.Left(item.FormalSize2.Length.ToString(), 11);
        //    dnload.FIELD094 = StringUtils.Left(item.FormalSize2.Width.ToString(), 11);
        //    dnload.FIELD095 = StringUtils.Left(item.FormalSize2.Height.ToString(), 11);
        //    dnload.FIELD096 = StringUtils.Left(item.FormalSize2.Weight.ToString(), 20);
        //    dnload.FIELD097 = StringUtils.Left(item.FormalSize3.Length.ToString(), 11);
        //    dnload.FIELD098 = StringUtils.Left(item.FormalSize3.Width.ToString(), 11);
        //    dnload.FIELD099 = StringUtils.Left(item.FormalSize3.Height.ToString(), 11);
        //    dnload.FIELD100 = StringUtils.Left(item.FormalSize3.Weight.ToString(), 20);
        //    dnload.FIELD101 = StringUtils.Left(item.FormalSize4.Length.ToString(), 11);
        //    dnload.FIELD102 = StringUtils.Left(item.FormalSize4.Width.ToString(), 11);
        //    dnload.FIELD103 = StringUtils.Left(item.FormalSize4.Height.ToString(), 11);
        //    dnload.FIELD104 = StringUtils.Left(item.FormalSize4.Weight.ToString(), 20);
        //    dnload.FIELD105 = StringUtils.Left(item.FormalSize5.Length.ToString(), 11);
        //    dnload.FIELD106 = StringUtils.Left(item.FormalSize5.Width.ToString(), 11);
        //    dnload.FIELD107 = StringUtils.Left(item.FormalSize5.Height.ToString(), 11);
        //    dnload.FIELD108 = StringUtils.Left(item.FormalSize5.Weight.ToString(), 20);
        //    dnload.FIELD109 = StringUtils.Left(item.PackClass, 10);
        //    dnload.FIELD110 = StringUtils.Left(item.LicensePlate, 22);
        //    dnload.FIELD111 = StringUtils.Left(item.Size1ItemType, 20);
        //    dnload.FIELD112 = StringUtils.Left(item.Size2ItemType, 20);
        //    dnload.FIELD113 = StringUtils.Left(item.Size3ItemType, 20);
        //    dnload.FIELD114 = StringUtils.Left(item.Size4ItemType, 20);
        //    dnload.FIELD115 = StringUtils.Left(item.Size5ItemType, 20);

        //    return dnload;
        //}
		#endregion
    }
}
