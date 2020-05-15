using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Reflection;
using Accellos.Interfaces.NAV.BusinessObjects;
using Accellos.InterfaceFramework.EventNotificationServer;

namespace Accellos.Interfaces.NAV.WMSAdapters
{
    public partial class WMSAdapter
    {
        //From WMS
        #region ParseStockCounts
        public static List<StockCount> ParseStockCounts(DataTable scRecords)
        {
            List<StockCount> stockCounts = new List<StockCount>();

            foreach (DataRow row in scRecords.Rows)
            {
                #region Stock Count Mapping
                StockCount stockCount = new StockCount();

                stockCount.RowID = row["ROWID"].ToString();

                // 1. 'SC' (2C)
				stockCount.TransactionCode = row["FIELD001"] as string;

                // 2. Warehouse (6C)
				stockCount.Warehouse = row["FIELD002"] as string;

                // 4. Product Code (20C)
				stockCount.ProductCode = row["FIELD004"] as string;

                // 5. Bin Location (8C)
				stockCount.BinLocation = row["FIELD005"] as string;

                // 6. Bin On-Hand (10N)
				if (row["FIELD006"].ToString().Length > 0)
					stockCount.Quantity = decimal.Parse(row["FIELD006"] as string);

                // 7. Product On-Hand (10N)
				if (row["FIELD007"].ToString().Length > 0)
					stockCount.WarehouseQty = decimal.Parse(row["FIELD007"] as string);

                // 8. Total On-Hand (10N)
				if (row["FIELD008"].ToString().Length > 0)
					stockCount.TotalQty = decimal.Parse(row["FIELD008"] as string);

                // 9. UPC (14C)
				stockCount.UPC = row["FIELD009"] as string;

                // 10. Attribute 1 (25C)
				stockCount.Attribute1 = row["FIELD010"] as string;

                // 11. Attribute 2 (25C)
				stockCount.Attribute2 = row["FIELD011"] as string;

                // 12. Attribute 3 (25C)
				stockCount.Attribute3 = row["FIELD012"] as string;

                // 13. Attribute 4 (6C)
				stockCount.Attribute4 = row["FIELD013"] as string;

                // 14. Attribute 5 (6C)
				stockCount.Attribute5 = row["FIELD014"] as string;

                // 15. Attribute 6 (6C)
				stockCount.Attribute6 = row["FIELD015"] as string;

                // 16. FIFO Date (D)
                if (row["FIELD016"].ToString().Length > 0)
                    stockCount.ExpiryDate = DateTime.ParseExact(row["FIELD016"].ToString(), "yyyyMMdd", CultureInfo.InvariantCulture);

                // 17. Pack Size (6N)
                if (row["FIELD017"].ToString().Length > 0)
					stockCount.UnitOfMeasureMultiplier = row["FIELD017"] as string;

                // 18. Attribute 7 (6C)
				stockCount.Attribute7 = row["FIELD018"] as string;

                // 19. Attribute 8 (6C)
				stockCount.Attribute8 = row["FIELD019"] as string;

                // 20. Attribute 9 (6C)
				stockCount.Attribute9 = row["FIELD020"] as string;

                // 21. Attribute 10 (6C)
				stockCount.Attribute10 = row["FIELD021"] as string;

                // 23. On-Hand at that Pack Size (10N)
				if (row["FIELD023"].ToString().Length > 0)
					stockCount.QtyInPacksize = decimal.Parse(row["FIELD023"] as string);

                // 24. ClientName (10C)
				stockCount.ClientName = row["FIELD024"] as string;

                // 25. Reserved Indicator (Y/N) (1C)
                if (row["FIELD025"].ToString().Trim().Equals("Y"))
                    stockCount.IsReserved = true;

                // 26. At-Shipping (8N)
				if (row["FIELD026"].ToString().Length > 0)
					stockCount.AtShipQty = decimal.Parse(row["FIELD026"] as string);

                // 27. At-Receiving (8N)
				if (row["FIELD027"].ToString().Length > 0)
					stockCount.AtReceiveQty = decimal.Parse(row["FIELD027"] as string);
                #endregion

                stockCounts.Add(stockCount);
            }

            return stockCounts;
        }
        #endregion

		#region A1W Middle Tier DTO to StockCount
        //public static List<StockCount> DTOToStockCount(IList<UPLOAD> uploadDTOs)
        //{
        //    List<StockCount> stockCounts = new List<StockCount>();

        //    foreach (UPLOAD upload in uploadDTOs)
        //    {
        //        StockCount stockCount = new StockCount();

        //        stockCount.RowID = upload.ROWID.ToString();
        //        stockCount.TransactionCode = upload.FIELD001;
        //        stockCount.Warehouse = upload.FIELD002;
        //        stockCount.ProductCode = upload.FIELD004;
        //        stockCount.BinLocation = upload.FIELD005;

        //        if (upload.FIELD006.Length > 0)
        //            stockCount.Quantity = decimal.Parse(upload.FIELD006);

        //        if (upload.FIELD007.Length > 0)
        //            stockCount.WarehouseQty = decimal.Parse(upload.FIELD007);

        //        if (upload.FIELD008.Length > 0)
        //            stockCount.TotalQty = decimal.Parse(upload.FIELD008);

        //        stockCount.UPC = upload.FIELD009;
        //        stockCount.Attribute1 = upload.FIELD010;
        //        stockCount.Attribute2 = upload.FIELD011;
        //        stockCount.Attribute3 = upload.FIELD012;
        //        stockCount.Attribute4 = upload.FIELD013;
        //        stockCount.Attribute5 = upload.FIELD014;
        //        stockCount.Attribute6 = upload.FIELD015;

        //        if (upload.FIELD016.Length > 0)
        //            stockCount.ExpiryDate = DateTime.ParseExact(upload.FIELD016, "yyyyMMdd", CultureInfo.InvariantCulture);

        //        if (upload.FIELD017.Length > 0)
        //            stockCount.UnitOfMeasureMultiplier = upload.FIELD017;

        //        stockCount.Attribute7 = upload.FIELD018;
        //        stockCount.Attribute8 = upload.FIELD019;
        //        stockCount.Attribute9 = upload.FIELD020;
        //        stockCount.Attribute10 = upload.FIELD021;

        //        if (upload.FIELD023.Length > 0)
        //            stockCount.QtyInPacksize = decimal.Parse(upload.FIELD023);

        //        stockCount.ClientName = upload.FIELD024;
        //        stockCount.IsReserved = upload.FIELD025.ToString().Trim().Equals("Y");

        //        if (upload.FIELD026.Length > 0)
        //            stockCount.AtShipQty = decimal.Parse(upload.FIELD026);

        //        if (upload.FIELD027.Length > 0)
        //            stockCount.AtReceiveQty = decimal.Parse(upload.FIELD027);

        //        stockCounts.Add(stockCount);
        //    }

        //    return stockCounts;
        //}
		#endregion
    }
}
