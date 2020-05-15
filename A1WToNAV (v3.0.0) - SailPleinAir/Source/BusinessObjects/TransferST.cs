using System;

namespace Accellos.Interfaces.NAV.BusinessObjects
{
	public partial class TransferST : AttributeSet
    {
        public string ClientName { get; set; }
        public string FromWarehouse { get; set; }
        public string ToWarehouse { get; set; }
        public string ProductCode { get; set; }
        public string UPC { get; set; }
        public string FromBinLocation { get; set; }
        public string ToBinLocation { get; set; }
        public string FromZone { get; set; }
        public string ToZone { get; set; }
        public decimal ToQuantity { get; set; }
		public decimal FromPacksize { get; set; } // Changed to decimal for A1W 64
		public decimal ToPacksize { get; set; } // Changed to decimal for A1W 64
        public string ReasonCode { get; set; }
        public string Comment { get; set; }
        public string Operator { get; set; }
        public DateTime MoveDate { get; set; }
        public string MoveTime { get; set; }
        public string OrderReference { get; set; }
        public string OrderType { get; set; }
        public string LineReference { get; set; }
        public string FromLicensePlate { get; set; }
        public string ToLicensePlate { get; set; }
        public string FromReserved { get; set; }
        public string ToReserved { get; set; }
    }
}

