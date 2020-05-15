using System;
using System.Collections.Generic;
using System.Text;

namespace Accellos.Interfaces.NAV.BusinessObjects
{
	public partial class InventoryItem : IClient3PL
    {
        #region Constructor
        public InventoryItem()
        {
            PickAttributeTracking = new AttributeTracking();
            ReceiveAttributeTracking = new AttributeTracking();
            Attributes = new List<AttributeSet>();
            FormalSize1 = new Dimension();
            FormalSize2 = new Dimension();
            FormalSize3 = new Dimension();
            FormalSize4 = new Dimension();
            FormalSize5 = new Dimension();
        }
        #endregion

        public string ProductCode { get; set; }
        public string VendorProductNumber { get; set; }
        public string Description { get; set; }
        public string VendorNumber { get; set; }
        public int QuantityAvailable { get; set; }
        public string UPC { get; set; }
        public decimal Cost { get; set; }
        public decimal RetailPrice { get; set; }
        public int WholesalePacksize { get; set; }
        public string MinimumDaysToExpiry { get; set; }
        public string SupplierType { get; set; }
        public string BinLocation { get; set; }
        public string ClientName { get; set; }
        public AttributeTracking PickAttributeTracking { get; set; }
        public AttributeTracking ReceiveAttributeTracking { get; set; }
        public List<AttributeSet> Attributes { get; set; }
        public bool ExpiryDateRequired { get; set; }
        public decimal Weight { get; set; }
        public decimal Cubage { get; set; }
        public decimal Height { get; set; }
        public decimal Width { get; set; }
        public decimal Length { get; set; }
        public int Size1 { get; set; }
        public int Size2 { get; set; }
        public int Size3 { get; set; }
        public int Size4 { get; set; }
        public int Size5 { get; set; }
        public string ProductClass { get; set; }
        public string OrderGroup { get; set; }
        public int ReorderPoint { get; set; }
        public string Buyer { get; set; }
        public int GroupLimit1 { get; set; }
        public int GroupLimit2 { get; set; }
        public int GroupLimit3 { get; set; }
        public int GroupLimit4 { get; set; }
        public int GroupLimit5 { get; set; }
        public int GroupLimit6 { get; set; }
        public int GroupLimit7 { get; set; }
        public int GroupLimit8 { get; set; }
        public int GroupLimit9 { get; set; }
        public string EDPNum { get; set; }
        public int MinimumReplenishmentLevel1 { get; set; }
        public int MinimumReplenishmentLevel2 { get; set; }
        public int MinimumReplenishmentLevel3 { get; set; }
        public int MinimumReplenishmentLevel4 { get; set; }
        public int MinimumReplenishmentLevel5 { get; set; }
        public int MaximumReplenishmentLevel1 { get; set; }
        public int MaximumReplenishmentLevel2 { get; set; }
        public int MaximumReplenishmentLevel3 { get; set; }
        public int MaximumReplenishmentLevel4 { get; set; }
        public int MaximumReplenishmentLevel5 { get; set; }
        public int CartonDef { get; set; }
        public string TariffCode { get; set; }
        public string CommodityCode { get; set; }
        public int InnerSize1 { get; set; }
        public int InnerSize2 { get; set; }
        public int InnerSize3 { get; set; }
        public int InnerSize4 { get; set; }
        public int InnerSize5 { get; set; }
        public string BeeLine { get; set; }
        public string HandleCode { get; set; }
        public int GlobalCoreReservation { get; set; }
        public int BypassQuantity { get; set; }
        public string CartonSize { get; set; }
        public decimal AverageUnits { get; set; }
        public string ProductUDF1 { get; set; }
        public string ProductUDF2 { get; set; }
        public string ProductUDF3 { get; set; }
        public string ProductUDF4 { get; set; }
        public string ProductUDF5 { get; set; }
        public Dimension FormalSize1 { get; set; }
        public Dimension FormalSize2 { get; set; }
        public Dimension FormalSize3 { get; set; }
        public Dimension FormalSize4 { get; set; }
        public Dimension FormalSize5 { get; set; }
        public string PackClass { get; set; }
        public string LicensePlate { get; set; }
        public string Size1ItemType { get; set; }
        public string Size2ItemType { get; set; }
        public string Size3ItemType { get; set; }
        public string Size4ItemType { get; set; }
        public string Size5ItemType { get; set; }
        public string CountryofOrigin { get; set; }
		public int QuantityPrecision { get; set; } // Added for A1W 64
		public int PacksizePrecision { get; set; } // Added for A1W 64
		public string OverReceiptPercentage { get; set; } // Added for A1W 64

        public string SVAStagingID { get; set; } //SVA CUSTOM - ADDED
    }

	public partial class Dimension
    {
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
    }
}

