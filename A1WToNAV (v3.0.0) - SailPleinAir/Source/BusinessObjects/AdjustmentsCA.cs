using System;

namespace Accellos.Interfaces.NAV.BusinessObjects
{
	public partial class AdjustmentsCA : AttributeSet, IClient3PL
    {
        public string AdjustmentSubcode { get; set; }
        public string ProductCode { get; set; }
        public string Description { get; set; }
        public string UnitOfMeasure { get; set; }
        public string ProductClass { get; set; }
        public string UPC { get; set; }
        public decimal Packsize { get; set; } // Changed for A1W 64
        public string PlusMinus { get; set; }
        public string BinLocation { get; set; }
        public string ReservedStockFlag { get; set; }
        public string PONumber { get; set; }
        public string Comment { get; set; }
        public string ReceivingAttributeControl { get; set; }
        public DateTime FIFODate { get; set; }
        public string ClientName { get; set; }
        public int InnerPack { get; set; }
        public int MinimumLevelOfReplenishment { get; set; }
        public int MaximumLevelOfReplenishment { get; set; }
        public string LicensePlate { get; set; }

        public string SVAStagingID { get; set; } //SVA CUSTOM - ADDED
    }
}

