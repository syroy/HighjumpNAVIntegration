using System;

namespace Accellos.Interfaces.NAV.BusinessObjects
{
	public partial class StockCount : AttributeSet, IClient3PL
    {
        public string RowID { get; set; }
        public string TransactionCode { get; set; }
        public string Warehouse { get; set; }
        public string ProductCode { get; set; }
        public string BinLocation { get; set; }
        public decimal BinQty
        {
            get { return base.Quantity; }
            set { base.Quantity = value; }
        }
        public decimal WarehouseQty { get; set; }
        public decimal TotalQty { get; set; }
        public string UPC { get; set; }
        public string UnitOfMeasureMultiplier { get; set; }
        public decimal QtyInPacksize { get; set; }
        public string ClientName { get; set; }
        public bool IsReserved { get; set; }
        public decimal AtShipQty { get; set; }
        public decimal AtReceiveQty { get; set; }
		public string LicensePlate { get; set; }
    }
}
