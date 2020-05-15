using System;

namespace Accellos.Interfaces.NAV.BusinessObjects
{
	public partial class TransferConfirmation
    {
        public string RowId { get; set; }   //SVA Added
        public XCOrderType OrderType { get; set; }
        public string OrderNumber { get; set; }
        public string CustomerNumber { get; set; }
        public string BackorderNumber { get; set; }
        public int NumberOfLines { get; set; }
        public decimal NumberOfUnits { get; set; }
        public string ContainerBatchReference { get; set; }
        public string Action { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public XCStatus Status { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string ClientName { get; set; }
        public string Extra1 { get; set; }
        public string Extra2 { get; set; }
        public string Extra3 { get; set; }
        public string Extra4 { get; set; }
        public string Extra5 { get; set; }
        public bool Nuked { get; set; }
    }

    public enum XCOrderType
    {
        Receiving,
        Picking,
        TransferSalesOrder,
        TransferPurchaseOrder
    }

    public enum XCStatus
    {
        Locked,
        Unlocked,
        HostSuspendedRequest
    }
}

