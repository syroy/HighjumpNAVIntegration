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
        #region ParseTransferST
        public static List<TransferST> ParseTransferST(DataTable stRecords)
        {
            List<TransferST> transfers = new List<TransferST>();

            foreach (DataRow row in stRecords.Rows)
            {
                TransferST transfer = new TransferST();
                transfer.RowId = row["RowId"].ToString();

                //Client Name (10C)
				transfer.ClientName = row["field002"] as string;

                //From Warehouse (6C)
				transfer.FromWarehouse = row["field003"] as string;

                //To Warehouse (6C)
				transfer.ToWarehouse = row["field004"] as string;

                //Product Code (20C)
				transfer.ProductCode = row["field005"] as string;

                //UPC (14C)
				transfer.UPC = row["field006"] as string;

                //From Bin Location (8C)
				transfer.FromBinLocation = row["field007"] as string;

                //To Bin Location (8C)
				transfer.ToBinLocation = row["field008"] as string;

                //From Zone (1C)
				transfer.FromZone = row["field009"] as string;

                //To Zone (1C)
				transfer.ToZone = row["field010"] as string;

                //From Quantity (8N) - Quantity Field used for Business Object
				if (row["FIELD011"].ToString().Length > 0)
					transfer.Quantity = Decimal.Parse(row["field011"] as string);

                //To Quantity (8N)
				if (row["FIELD012"].ToString().Length > 0)
					transfer.ToQuantity = Decimal.Parse(row["field012"] as string);

				//From Packsize (8N)
				if (row["FIELD013"].ToString().Length > 0)
					transfer.FromPacksize = decimal.Parse(row["field013"] as string); // Changed to decimal for A1W 64

				//To Packsize (8N)
				if (row["FIELD014"].ToString().Length > 0)
					transfer.ToPacksize = decimal.Parse(row["field014"] as string); // Changed to decimal for A1W 64

                //Reson Code (2C)
				transfer.ReasonCode = row["field015"] as string;

                //Comment (16C)
				transfer.Comment = row["field016"] as string;

                //Operator (8C)
				transfer.Operator = row["field017"] as string;

                //Move Date (D)
                if (row["field018"].ToString().Length > 0)
                    transfer.MoveDate = DateTime.ParseExact(row["field018"].ToString(), "yyyyMMdd", CultureInfo.InvariantCulture);

                //Move Time (8C)
				transfer.MoveTime = row["field019"] as string;

                //Attribute 1 (25C)
				transfer.Attribute1 = row["field020"] as string;

                //Attribute 2 (25C)
				transfer.Attribute2 = row["field021"] as string;

                //Attribute 3 (6C)
				transfer.Attribute3 = row["field022"] as string;

                //Attribute 4 (6C)
				transfer.Attribute4 = row["field023"] as string;

                //Attribute 5 (6C)
				transfer.Attribute5 = row["field024"] as string;

                //Attribute 6 (6C)
				transfer.Attribute6 = row["field025"] as string;

                //Attribute 7 (6C)
				transfer.Attribute7 = row["field026"] as string;

                //Attribute 8 (6C)
				transfer.Attribute8 = row["field027"] as string;

                //Attribute 9 (6C)
				transfer.Attribute9 = row["field028"] as string;

                //Attribute 10 (6C)
				transfer.Attribute10 = row["field029"] as string;

                //Expiry Date (D)
                if (row["field030"].ToString().Length > 0)
                    transfer.ExpiryDate = DateTime.ParseExact(row["field030"].ToString(), "yyyyMMdd", CultureInfo.InvariantCulture);

                //Order Reference (20C)
				transfer.OrderReference = row["field031"] as string;

                //Transfer Order Type (1C)
				transfer.OrderType = row["field032"] as string;

                //Line Reference (20C)
				transfer.LineReference = row["field033"] as string;

                //From License Plate (22C)
				transfer.FromLicensePlate = row["field034"] as string;

                //To License Plate (22C)
				transfer.ToLicensePlate = row["field035"] as string;

                //From Reserved (1C)
				transfer.FromReserved = row["field036"] as string;

                //To Reserved (1C)
				transfer.ToReserved = row["field037"] as string;

                transfers.Add(transfer);
            }

            return transfers;
        }
        #endregion

		#region A1W Middle Tier DTO to TransferST
        //public static List<TransferST> DTOToTransferST(IList<UPLOAD> uploadDTOs)
        //{
        //    List<TransferST> transferSTs = new List<TransferST>();

        //    foreach (UPLOAD upload in uploadDTOs)
        //    {
        //        TransferST transferST = new TransferST();

        //        transferST.RowId = upload.ROWID.ToString();
        //        transferST.ClientName = upload.FIELD002;
        //        transferST.FromWarehouse = upload.FIELD003;
        //        transferST.ToWarehouse = upload.FIELD004;
        //        transferST.ProductCode = upload.FIELD005;
        //        transferST.UPC = upload.FIELD006;
        //        transferST.FromBinLocation = upload.FIELD007;
        //        transferST.ToBinLocation = upload.FIELD008;
        //        transferST.FromZone = upload.FIELD009;
        //        transferST.ToZone = upload.FIELD010;

        //        if (upload.FIELD011.Length > 0)
        //            transferST.Quantity = Decimal.Parse(upload.FIELD011);

        //        if (upload.FIELD012.Length > 0)
        //            transferST.ToQuantity = Decimal.Parse(upload.FIELD012);

        //        if (upload.FIELD013.Length > 0)
        //            transferST.FromPacksize = decimal.Parse(upload.FIELD013); // Changed to decimal for A1W 64

        //        if (upload.FIELD014.Length > 0)
        //            transferST.ToPacksize = decimal.Parse(upload.FIELD014); // Changed to decimal for A1W 64

        //        transferST.ReasonCode = upload.FIELD015;
        //        transferST.Comment = upload.FIELD016;
        //        transferST.Operator = upload.FIELD017;

        //        if (upload.FIELD018.ToString().Length > 0)
        //            transferST.MoveDate = DateTime.ParseExact(upload.FIELD018.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture);

        //        transferST.MoveTime = upload.FIELD019;
        //        transferST.Attribute1 = upload.FIELD020;
        //        transferST.Attribute2 = upload.FIELD021;
        //        transferST.Attribute3 = upload.FIELD022;
        //        transferST.Attribute4 = upload.FIELD023;
        //        transferST.Attribute5 = upload.FIELD024;
        //        transferST.Attribute6 = upload.FIELD025;
        //        transferST.Attribute7 = upload.FIELD026;
        //        transferST.Attribute8 = upload.FIELD027;
        //        transferST.Attribute9 = upload.FIELD028;
        //        transferST.Attribute10 = upload.FIELD029;

        //        if (upload.FIELD030.Length > 0)
        //            transferST.ExpiryDate = DateTime.ParseExact(upload.FIELD030, "yyyyMMdd", CultureInfo.InvariantCulture);

        //        transferST.OrderReference = upload.FIELD031;
        //        transferST.OrderType = upload.FIELD032;
        //        transferST.LineReference = upload.FIELD033;
        //        transferST.FromLicensePlate = upload.FIELD034;
        //        transferST.ToLicensePlate = upload.FIELD035;
        //        transferST.FromReserved = upload.FIELD036;
        //        transferST.ToReserved = upload.FIELD037;

        //        transferSTs.Add(transferST);
        //    }

        //    return transferSTs;
        //}
		#endregion
    }
}
