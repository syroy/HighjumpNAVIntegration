using System;
using System.Collections.Generic;
using System.Text;
using Accellos.Interfaces.NAV.BusinessObjects;

namespace Accellos.Interfaces.NAV.BusinessObjects
{
	public partial class Adjustment : AttributeSet
    {
        /*  
        AT ? Attribute Transfer
        BS ? Borrowed Stock
        CC ? Cycle Count (Created when a discrepancy is found while cycle counting.)
        DF ? Damaged in Field/while out of the warehouse
        DG ? Removal of Damaged Stock
        DN ? Donations
        FC ? Front Counter
        IU ? Internal Use
        KP ? Kit (Assembly) Pull from workarea
        KC ? Kit (Assembly) Complete ? Zero Scrap
        KR ? Kit (Assembly) Reverse Kitting
        CW ? Clear WorkArea; Default Adjustment Type for Items Cleared when kit is finished. Configurable by SUBCODE_FK switch
        MA ? Misc. Adjustments (i.e.: Damaged, Returned)
        PK ? Packaging Materials
        PR ? Promotional Materials
        RC ? Return from Customer
        RE ? Receiving Errors
        RM ? RMA
        RN ? Rename Product (usually caused by host-initiated CA record)
        RP ? Repairs
        RV ? Receiving/Receiving stock not on PO (This type also reports the same data as Receiving Confirmation records, however, the data is available immediately, rather than when the receipt is completed. Note: Do not double count receipts.)
        SU ? Supplies
        TR ? Transfers in/out of the warehouse
        VR ? Return to Vendor
        WP ? Work in Progress
        */

        public string TransactionCode { get; set; }
        public string Warehouse { get; set; }
        public string ProductCode { get; set; }
        public string BinLocation { get; set; }
        public string CustomerVendor { get; set; }
        public string OrderNum { get; set; }
        public string RMA { get; set; }
        public string ReasonCode { get; set; }
        public string Comment { get; set; }
        public string OperatorName { get; set; }
        public string UPC { get; set; }
        public string Zone { get; set; }
        public string OrderReference { get; set; }
        public string TransferOrderReference { get; set; }
        public string Special1 { get; set; }
        public string Special2 { get; set; }
        public string Special3 { get; set; }
        public string ClientName { get; set; }
        public string Extra1 { get; set; }
        public string Extra2 { get; set; }
        public string Extra3 { get; set; }
        public string Extra4 { get; set; }
        public string Extra5 { get; set; }
        public string Special4 { get; set; }
        public string Special5 { get; set; }
        public string Special6 { get; set; }
        public string LineNum { get; set; }
        public string TransferOrderType { get; set; }
        public string LicensePlate { get; set; }
		public decimal Packsize { get; set; } // Changed to decimal for A1W 64
		public decimal ItemCost { get; set; }
        public bool Reserved { get; set; }
        public DateTime AdjustmentDate { get; set; }
        public DateTime AddTime { get; set; }
    }
}

