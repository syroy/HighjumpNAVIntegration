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
        #region ParseTransferConfirmation
        public static List<TransferConfirmation> ParseTransferConfirmation(DataTable xcRecords)
        {
            List<TransferConfirmation> result = new List<TransferConfirmation>();

            foreach (DataRow row in xcRecords.Rows)
            {
                TransferConfirmation confirmation = new TransferConfirmation();
                result.Add(confirmation);

                confirmation.RowId = row["rowid"].ToString();

                #region XC mapping
                //2. Order Type (1C)
				switch (row["FIELD002"] as string)
                {
                    case "R":
                        confirmation.OrderType = XCOrderType.Receiving;
                        break;
                    case "P":
                        confirmation.OrderType = XCOrderType.Picking;
                        break;
                    case "S":
                        confirmation.OrderType = XCOrderType.TransferSalesOrder;
                        break;
                    case "T":
                        confirmation.OrderType = XCOrderType.TransferPurchaseOrder;
                        break;
                }

                //3. Order Number (20C)
				confirmation.OrderNumber = row["FIELD003"] as string;

                //4. Customer Number (16C)
				confirmation.CustomerNumber = row["FIELD004"] as string;

                //5. BackOrder Number (16C)
				confirmation.BackorderNumber = row["FIELD005"] as string;

                //6. Number of Lines (10N)
				confirmation.NumberOfLines = Int32.Parse(row["FIELD006"] as string);

                //7. Number of Units (10N)
				confirmation.NumberOfUnits = Decimal.Parse(row["FIELD007"] as string);

                //8. Container / Batch Reference (20C)
				confirmation.ContainerBatchReference = row["FIELD008"] as string;

                //9. Action (16C)
				confirmation.Action = row["FIELD009"] as string;

                //10. Date (8C)(format YYYYMMDD)
				if (row["FIELD010"].ToString().Length > 0)
					confirmation.Date = DateTime.ParseExact(row["FIELD010"] as string, "yyyyMMdd", CultureInfo.InvariantCulture);

                //11. Time (8C)(format 00:00:00)
                string[] split = row["FIELD011"].ToString().Split(':');
                confirmation.Time = new TimeSpan(0, Int32.Parse(split[0]), Int32.Parse(split[1]), Int32.Parse(split[2]));

                //12. Status (1C)
				switch (row["FIELD012"] as string)
                {
                    case "L":
                        confirmation.Status = XCStatus.Locked;
                        break;
                    case "U":
                        confirmation.Status = XCStatus.Unlocked;
                        break;
                    case "H":
                        confirmation.Status = XCStatus.HostSuspendedRequest;
                        break;
                }

                //13. Purchase Order Number (20C)
				confirmation.PurchaseOrderNumber = row["FIELD013"] as string;

                //14. Client Name (10C)
				confirmation.ClientName = row["FIELD014"] as string;

                //15. Extra1 (40C)
				confirmation.Extra1 = row["FIELD015"] as string;

                //16. Extra1 (40C)
				confirmation.Extra2 = row["FIELD016"] as string;

                //17. Extra1 (40C)
				confirmation.Extra3 = row["FIELD017"] as string;

                //18. Extra1 (40C)
				confirmation.Extra4 = row["FIELD018"] as string;

                //19. Extra1 (40C)
				confirmation.Extra5 = row["FIELD019"] as string;

                //20. Nuked (1C)
                confirmation.Nuked = row["FIELD019"].ToString().Equals("1");
                #endregion
            }
            return result;
        }
        #endregion

		#region A1W Middle Tier DTO to TransferConfirmation
        //public static TransferConfirmation DTOToTransferConfirmation(UPLOAD upload)
        //{
        //    TransferConfirmation transferConfirmation = new TransferConfirmation();

        //    switch (upload.FIELD002)
        //    {
        //        case "R":
        //            transferConfirmation.OrderType = XCOrderType.Receiving;
        //            break;
        //        case "P":
        //            transferConfirmation.OrderType = XCOrderType.Picking;
        //            break;
        //        case "S":
        //            transferConfirmation.OrderType = XCOrderType.TransferSalesOrder;
        //            break;
        //        case "T":
        //            transferConfirmation.OrderType = XCOrderType.TransferPurchaseOrder;
        //            break;
        //    }

        //    transferConfirmation.OrderNumber = upload.FIELD003;
        //    transferConfirmation.CustomerNumber = upload.FIELD004;
        //    transferConfirmation.BackorderNumber = upload.FIELD005;

        //    if (upload.FIELD006.Length > 0)
        //        transferConfirmation.NumberOfLines = Int32.Parse(upload.FIELD006);

        //    if (upload.FIELD007.Length > 0)
        //        transferConfirmation.NumberOfUnits = Decimal.Parse(upload.FIELD007);

        //    transferConfirmation.ContainerBatchReference = upload.FIELD008;
        //    transferConfirmation.Action = upload.FIELD009;

        //    if (upload.FIELD010.Length > 0)
        //        transferConfirmation.Date = DateTime.ParseExact(upload.FIELD010, "yyyyMMdd", CultureInfo.InvariantCulture);

        //    string[] split = upload.FIELD011.Split(':');
        //    transferConfirmation.Time = new TimeSpan(0, Int32.Parse(split[0]), Int32.Parse(split[1]), Int32.Parse(split[2]));

        //    switch (upload.FIELD012)
        //    {
        //        case "L":
        //            transferConfirmation.Status = XCStatus.Locked;
        //            break;
        //        case "U":
        //            transferConfirmation.Status = XCStatus.Unlocked;
        //            break;
        //        case "H":
        //            transferConfirmation.Status = XCStatus.HostSuspendedRequest;
        //            break;
        //    }

        //    transferConfirmation.PurchaseOrderNumber = upload.FIELD013;
        //    transferConfirmation.ClientName = upload.FIELD014;
        //    transferConfirmation.Extra1 = upload.FIELD015;
        //    transferConfirmation.Extra2 = upload.FIELD016;
        //    transferConfirmation.Extra3 = upload.FIELD017;
        //    transferConfirmation.Extra4 = upload.FIELD018;
        //    transferConfirmation.Extra5 = upload.FIELD019;
        //    transferConfirmation.Nuked = upload.FIELD019.Equals("1");

        //    return transferConfirmation;
        //}
		#endregion
    }
}
