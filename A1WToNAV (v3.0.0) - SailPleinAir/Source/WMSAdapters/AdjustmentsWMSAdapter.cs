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
        #region ParseAdjustmentsCA
        public static DataTable ParseAdjustmentCA(AdjustmentsCA adjustmentCA)
        {
            DataTable result = CompanyConfig.Instance.DownloadTableSchema;

            DataRow row = result.NewRow();

            //CA
            row["FIELD001"] = "CA";
            row["FIELD002"] = StringUtils.Left(adjustmentCA.AdjustmentSubcode, 2);
            row["FIELD003"] = StringUtils.Left(adjustmentCA.ProductCode, 100);
            row["FIELD004"] = StringUtils.Left(adjustmentCA.Description, 100); // Changed to 100 characters for A1W 64 - iCepts 10/6/15
            row["FIELD005"] = StringUtils.Left(adjustmentCA.UnitOfMeasure, 6);
            row["FIELD006"] = StringUtils.Left(adjustmentCA.ProductClass, 6);
            row["FIELD007"] = StringUtils.Left(adjustmentCA.UPC, 14);
            row["FIELD008"] = adjustmentCA.Quantity.ToString(CultureInfo.InvariantCulture);
            row["FIELD009"] = adjustmentCA.Packsize.ToString(CultureInfo.InvariantCulture);
            row["FIELD010"] = adjustmentCA.PlusMinus;
            row["FIELD011"] = StringUtils.Left(adjustmentCA.BinLocation, 8);
            row["FIELD012"] = adjustmentCA.ReservedStockFlag;
            row["FIELD013"] = StringUtils.Left(adjustmentCA.PONumber, 20);
            row["FIELD014"] = StringUtils.Left(adjustmentCA.Comment, 16);
            row["FIELD015"] = StringUtils.Left(adjustmentCA.Attribute1, 25);
            row["FIELD016"] = StringUtils.Left(adjustmentCA.Attribute2, 25);
            row["FIELD017"] = StringUtils.Left(adjustmentCA.Attribute3, 25);
            row["FIELD018"] = StringUtils.Left(adjustmentCA.Attribute4, 6);
            row["FIELD019"] = StringUtils.Left(adjustmentCA.Attribute5, 6);
            row["FIELD020"] = StringUtils.Left(adjustmentCA.Attribute6, 6);
            row["FIELD021"] = StringUtils.Left(adjustmentCA.Attribute7, 6);
            row["FIELD022"] = StringUtils.Left(adjustmentCA.Attribute8, 6);
            row["FIELD023"] = StringUtils.Left(adjustmentCA.Attribute9, 6);
			row["FIELD024"] = StringUtils.Left(adjustmentCA.Attribute10, 6);
            row["FIELD025"] = StringUtils.Left(adjustmentCA.ReceivingAttributeControl, 10);
            row["FIELD026"] = adjustmentCA.FIFODate == DateTime.MinValue ? string.Empty : adjustmentCA.FIFODate.ToString("yyyyMMdd");
            row["FIELD027"] = StringUtils.Left(adjustmentCA.ClientName, 10);
            row["FIELD028"] = adjustmentCA.InnerPack.ToString(CultureInfo.InvariantCulture);
            row["FIELD029"] = adjustmentCA.MinimumLevelOfReplenishment.ToString(CultureInfo.InvariantCulture);
            row["FIELD030"] = adjustmentCA.MaximumLevelOfReplenishment.ToString(CultureInfo.InvariantCulture);
            row["FIELD031"] = StringUtils.Left(adjustmentCA.LicensePlate, 30);

            result.Rows.Add(row);

            return result;
        }
        #endregion

		#region AdjustmentsCA to A1W Middle Tier DTO
        //public static DNLOAD AdjustmentsCAToDTO(AdjustmentsCA adjustmentCA)
        //{
        //    DNLOAD dnload = new DNLOAD();

        //    dnload.FIELD001 = "CA";
        //    dnload.FIELD002 = StringUtils.Left(adjustmentCA.AdjustmentSubcode, 2);
        //    dnload.FIELD003 = StringUtils.Left(adjustmentCA.ProductCode, 100);
        //    dnload.FIELD004 = StringUtils.Left(adjustmentCA.Description, 40);
        //    dnload.FIELD005 = StringUtils.Left(adjustmentCA.UnitOfMeasure, 6);
        //    dnload.FIELD006 = StringUtils.Left(adjustmentCA.ProductClass, 6);
        //    dnload.FIELD007 = StringUtils.Left(adjustmentCA.UPC, 14);
        //    dnload.FIELD008 = adjustmentCA.Quantity.ToString(CultureInfo.InvariantCulture);
        //    dnload.FIELD009 = adjustmentCA.Packsize.ToString(CultureInfo.InvariantCulture);
        //    dnload.FIELD010 = adjustmentCA.PlusMinus.ToString();
        //    dnload.FIELD011 = StringUtils.Left(adjustmentCA.BinLocation, 8);
        //    dnload.FIELD012 = adjustmentCA.ReservedStockFlag.ToString();
        //    dnload.FIELD013 = StringUtils.Left(adjustmentCA.PONumber, 20);
        //    dnload.FIELD014 = StringUtils.Left(adjustmentCA.Comment, 16);
        //    dnload.FIELD015 = StringUtils.Left(adjustmentCA.Attribute1, 25);
        //    dnload.FIELD016 = StringUtils.Left(adjustmentCA.Attribute2, 25);
        //    dnload.FIELD017 = StringUtils.Left(adjustmentCA.Attribute3, 25);
        //    dnload.FIELD018 = StringUtils.Left(adjustmentCA.Attribute4, 6);
        //    dnload.FIELD019 = StringUtils.Left(adjustmentCA.Attribute5, 6);
        //    dnload.FIELD020 = StringUtils.Left(adjustmentCA.Attribute6, 6);
        //    dnload.FIELD021 = StringUtils.Left(adjustmentCA.Attribute7, 6);
        //    dnload.FIELD022 = StringUtils.Left(adjustmentCA.Attribute8, 6);
        //    dnload.FIELD023 = StringUtils.Left(adjustmentCA.Attribute9, 6);
        //    dnload.FIELD024 = StringUtils.Left(adjustmentCA.Attribute10, 6);
        //    dnload.FIELD025 = StringUtils.Left(adjustmentCA.ReceivingAttributeControl, 10);
        //    dnload.FIELD026 = adjustmentCA.FIFODate == DateTime.MinValue ? string.Empty : adjustmentCA.FIFODate.ToString("yyyyMMdd");
        //    dnload.FIELD027 = StringUtils.Left(adjustmentCA.ClientName, 10);
        //    dnload.FIELD028 = adjustmentCA.InnerPack.ToString(CultureInfo.InvariantCulture);
        //    dnload.FIELD029 = adjustmentCA.MinimumLevelOfReplenishment.ToString(CultureInfo.InvariantCulture);
        //    dnload.FIELD030 = adjustmentCA.MaximumLevelOfReplenishment.ToString(CultureInfo.InvariantCulture);
        //    dnload.FIELD031 = StringUtils.Left(adjustmentCA.LicensePlate, 30);

        //    return dnload;
        //}
		#endregion

		//From WMS
        #region ParseAdjustments
        public static List<Adjustment> ParseAdjustments(DataTable mtRecords)
        {
            List<Adjustment> adjustments = new List<Adjustment>();

            foreach (DataRow row in mtRecords.Rows)
            {
                #region Adjustment mapping
                Adjustment adjustment = new Adjustment();
                adjustment.RowId = row["ROWID"].ToString();

                //3. MT-Subcode (2C)
                adjustment.TransactionCode = row["FIELD003"] as string;

                //4. Warehouse (6C)
                adjustment.Warehouse = row["FIELD004"] as string;

                //5. Product Code (20C)
                adjustment.ProductCode = row["FIELD005"] as string;

                //6. Bin Location (8C)
                adjustment.BinLocation = row["FIELD006"] as string;

				//7. In_or_Out (1C)
				//8. Quantity (8N)
				if (row["FIELD008"].ToString().Length > 0)
					adjustment.Quantity = row["FIELD007"].ToString().Trim() == "+" ? decimal.Parse(row["FIELD008"] as string) : decimal.Parse(row["FIELD008"] as string) * -1;

                //9. Customer/Vendor (16C)
                adjustment.CustomerVendor = row["FIELD009"] as string;

                //10. Order Number (20C)	
                adjustment.OrderNum = row["FIELD010"] as string;

                //11. RMA (16C)
                adjustment.RMA = row["FIELD011"] as string;

                //12. Reasoncode (2C)
                adjustment.ReasonCode = row["FIELD012"] as string;

                //13. Comment (16C)
                adjustment.Comment = row["FIELD013"] as string;

                //14. Operator (8C)
                adjustment.OperatorName = row["FIELD014"] as string;

                //15. Adjustment Date (D)
                if (row["FIELD015"].ToString().Length > 0)
                    adjustment.AdjustmentDate = DateTime.ParseExact(row["FIELD015"].ToString(), "yyyyMMdd", CultureInfo.InvariantCulture);

                //16. UPC (14C)
                adjustment.UPC = row["FIELD016"] as string;

                //17. Attribute 1 (25C)
                adjustment.Attribute1 = row["FIELD017"] as string;

                //18. Attribute 2 (25C)
                adjustment.Attribute2 = row["FIELD018"] as string;

                //19. Attribute 3 (25C)
                adjustment.Attribute3 = row["FIELD019"] as string;

                //20. Attribute 4 (25C)
                adjustment.Attribute4 = row["FIELD020"] as string;

                //21. Attribute 5 (25C)
                adjustment.Attribute5 = row["FIELD021"] as string;

                //22. Attribute 6 (25C)
                adjustment.Attribute6 = row["FIELD022"] as string;

                //23. Attribute 7 (25C)
                adjustment.Attribute7 = row["FIELD023"] as string;

                //24. Attribute 8 (25C)
                adjustment.Attribute8 = row["FIELD024"] as string;

                //25. Attribute 9 (25C)
                adjustment.Attribute9 = row["FIELD025"] as string;

                //26. Attribute 10 (25C)
                adjustment.Attribute10 = row["FIELD026"] as string;

                //27. Zone (1C)
                adjustment.Zone = row["FIELD027"] as string;

                //28. Expiry Date (D)
                if (row["FIELD028"].ToString().Length > 0)
                    adjustment.ExpiryDate = DateTime.ParseExact(row["FIELD028"].ToString(), "yyyyMMdd", CultureInfo.InvariantCulture);

                //29. Order Reference (16C)
                adjustment.OrderReference = row["FIELD029"] as string;

                //30. Special 1 (20C)
                adjustment.Special1 = row["FIELD030"] as string;

                //31. Special 2 (20C)
                adjustment.Special2 = row["FIELD031"] as string;

                //32. Special 3 (20C)
                adjustment.Special3 = row["FIELD032"] as string;

                //33. Client Name (10C)
                adjustment.ClientName = row["FIELD033"] as string;

                //34. Extra1(40C)
                adjustment.Extra1 = row["FIELD034"] as string;

                //35. Extra2 (40C)
                adjustment.Extra2 = row["FIELD035"] as string;

                //36. Extra3 (40C)
                adjustment.Extra3 = row["FIELD036"] as string;

                //37. Extra4 (40C)
                adjustment.Extra4 = row["FIELD037"] as string;

                //38. Extra5 (40C)
                adjustment.Extra5 = row["FIELD038"] as string;

                //39. Special 4 (20C) 
                adjustment.Special4 = row["FIELD039"] as string;

                //40. Special 5 (20C)
                adjustment.Special5 = row["FIELD040"] as string;

                //41. Special 6 (20C)
                adjustment.Special6 = row["FIELD041"] as string;

                //45. Line Number (0C)
                adjustment.LineNum = row["FIELD045"] as string;

                //46. Add Time (8C)
                if (row["FIELD046"].ToString().Length > 0)
                    adjustment.ExpiryDate = DateTime.ParseExact(row["FIELD046"].ToString(), "yyyyMMdd", CultureInfo.InvariantCulture);

                //47. Item Cost (16.5N)
                if (row["FIELD047"].ToString().Length > 0)
                    adjustment.ItemCost = Decimal.Parse(row["FIELD047"] as string);

                //48. Order Reference (C20)
                adjustment.TransferOrderReference = row["FIELD048"] as string;

                //49. Transfer Order Type (1C)
				adjustment.TransferOrderType = row["FIELD049"] as string;

                //50. Packsize (6N)
                if (row["FIELD050"].ToString().Length > 0)
					adjustment.Packsize = decimal.Parse(row["FIELD050"] as string); // Changed to decimal for A1W 64

                //51. License Plate (22C)
                adjustment.LicensePlate = row["FIELD051"] as string;

                //52. Reserved (1C)
                adjustment.Reserved = "Y".Equals(row["FIELD052"] as string);
                #endregion

                adjustments.Add(adjustment);
            }

            return adjustments;
        }
        #endregion

		#region A1W Middle Tier DTO to Adjustment
        //public static List<Adjustment> DTOToAdjustment(IList<UPLOAD> uploadDTOs)
        //{
        //    List<Adjustment> adjustments = new List<Adjustment>();

        //    foreach (UPLOAD upload in uploadDTOs)
        //    {
        //        Adjustment adjustment = new Adjustment();

        //        adjustment.RowId = upload.ROWID.ToString();
        //        adjustment.TransactionCode = upload.FIELD003;
        //        adjustment.Warehouse = upload.FIELD004;
        //        adjustment.ProductCode = upload.FIELD005;
        //        adjustment.BinLocation = upload.FIELD006;

        //        if (upload.FIELD008.Length > 0)
        //            adjustment.Quantity = upload.FIELD007.Trim() == "+" ? decimal.Parse(upload.FIELD008) : decimal.Parse(upload.FIELD008) * -1;

        //        adjustment.CustomerVendor = upload.FIELD009;
        //        adjustment.OrderNum = upload.FIELD010;
        //        adjustment.RMA = upload.FIELD011;
        //        adjustment.ReasonCode = upload.FIELD012;
        //        adjustment.Comment = upload.FIELD013;
        //        adjustment.OperatorName = upload.FIELD014;

        //        if (upload.FIELD015.Length > 0)
        //            adjustment.AdjustmentDate = DateTime.ParseExact(upload.FIELD015, "yyyyMMdd", CultureInfo.InvariantCulture);

        //        adjustment.UPC = upload.FIELD016;
        //        adjustment.Attribute1 = upload.FIELD017;
        //        adjustment.Attribute2 = upload.FIELD018;
        //        adjustment.Attribute3 = upload.FIELD019;
        //        adjustment.Attribute4 = upload.FIELD020;
        //        adjustment.Attribute5 = upload.FIELD021;
        //        adjustment.Attribute6 = upload.FIELD022;
        //        adjustment.Attribute7 = upload.FIELD023;
        //        adjustment.Attribute8 = upload.FIELD024;
        //        adjustment.Attribute9 = upload.FIELD025;
        //        adjustment.Attribute10 = upload.FIELD026;
        //        adjustment.Zone = upload.FIELD027;

        //        if (upload.FIELD028.Length > 0)
        //            adjustment.ExpiryDate = DateTime.ParseExact(upload.FIELD028, "yyyyMMdd", CultureInfo.InvariantCulture);

        //        adjustment.OrderReference = upload.FIELD029;
        //        adjustment.Special1 = upload.FIELD030;
        //        adjustment.Special2 = upload.FIELD031;
        //        adjustment.Special3 = upload.FIELD032;
        //        adjustment.ClientName = upload.FIELD033;
        //        adjustment.Extra1 = upload.FIELD034;
        //        adjustment.Extra2 = upload.FIELD035;
        //        adjustment.Extra3 = upload.FIELD036;
        //        adjustment.Extra4 = upload.FIELD037;
        //        adjustment.Extra5 = upload.FIELD038;
        //        adjustment.Special4 = upload.FIELD039;
        //        adjustment.Special5 = upload.FIELD040;
        //        adjustment.Special6 = upload.FIELD041;
        //        adjustment.LineNum = upload.FIELD045;

        //        if (upload.FIELD046.Length > 0)
        //            adjustment.ExpiryDate = DateTime.ParseExact(upload.FIELD046, "yyyyMMdd", CultureInfo.InvariantCulture);

        //        if (upload.FIELD047.Length > 0)
        //            adjustment.ItemCost = Decimal.Parse(upload.FIELD047);

        //        adjustment.TransferOrderReference = upload.FIELD048;
        //        adjustment.TransderOrderType = upload.FIELD049;

        //        if (upload.FIELD050.Length > 0)
        //            adjustment.Packsize = decimal.Parse(upload.FIELD050); // Changed to decimal for A1W 64

        //        adjustment.LicensePlate = upload.FIELD051;
        //        adjustment.Reserved = "Y".Equals(upload.FIELD052);

        //        adjustments.Add(adjustment);
        //    }

        //    return adjustments;
        //}
		#endregion
	}
}

