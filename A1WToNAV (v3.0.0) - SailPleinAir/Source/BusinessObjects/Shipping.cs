using System;
using System.Collections.Generic;
using System.Text;

namespace Accellos.Interfaces.NAV.BusinessObjects
{
	public partial class Packslip : IClient3PL
    {
        #region Constructor
        public Packslip()
        {
            Lines = new List<PackslipLine>();
        }
        #endregion

        public string RecordType { get; set; }
        public string BatchControlNumber { get; set; }
        public string CustomerNumber { get; set; }
        public string OrderNumber { get; set; }
        public string BackOrderNumber { get; set; }
        public string CustomerPONumber { get; set; }
        public string CustomerLabel { get; set; }
        public string CrystalPackingSlip { get; set; }
        public string CustomerGroup { get; set; }
        public string LabelText { get; set; }
        public string ShipName { get; set; }
		public String ShipAddressName1 { get; set; }
		public String ShipAddressName2 { get; set; }
        public string ShipCity { get; set; }
        public string ShipProvinceState { get; set; }
        public string ShipPostalZipCode { get; set; }
        public string ShipCountry { get; set; }
        public string ShipAttnTo { get; set; }
        public string ShipTelephoneNumber { get; set; }
        public string BillName { get; set; }
        public string BillAddress1 { get; set; }
        public string BillAddress2 { get; set; }
        public string BillCity { get; set; }
        public string BillProvinceState { get; set; }
        public string BillPostalZip { get; set; }
		public String BillCountry { get; set; }
        public DateTime ShipDateRequired { get; set; }
        public string ShipTimeRequired { get; set; }
        public string TruckRoute { get; set; }
        public string Priority { get; set; }
        public ShipmentPaymentType ShipmentPaymentType { get; set; }
        public string ShipViaPreferred { get; set; }
        public string ShipPayAccountNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime CancelDate { get; set; }
        public PurchaseOrderType PurchaseOrderType { get; set; }
        public bool Consolidate { get; set; }
        public bool ShipCompleteOnly { get; set; }
        public string ClientMH10Number { get; set; }
        public string ClientName { get; set; }
        public string SpecialInstructions1 { get; set; }
        public string SpecialInstructions2 { get; set; }
        public string SpecialInstructions3 { get; set; }
        public string SpecialInstructions4 { get; set; }
        public string SpecialInstructions5 { get; set; }
        public string SpecialInstructions6 { get; set; }
        public string SpecialInstructions7 { get; set; }
        public string CostCenter { get; set; }
        public string ShipToNumber { get; set; }
        public bool IsCustomerAcceptsBackorders { get; set; }
        public string TruckStop { get; set; }
        public string TruckDeliveryTime { get; set; }
        public string ShipperName { get; set; }
        public string ShipperAddress1 { get; set; }
        public string ShipperAddress2 { get; set; }
        public string ShipperCity { get; set; }
        public string ShipperProvinceState { get; set; }
        public string ShipperPostalZipCode { get; set; }
        public string PackingSlip { get; set; }
        public string IncludePickZones { get; set; }
        public string ExcludePickZones { get; set; }
        public string IncludeReplenishmentZones { get; set; }
        public bool ShipWithOtherGoods { get; set; }
        public bool DontSplitExpiryDates { get; set; }
        public string ShipmentOptions { get; set; }
        public string ShipmentActions { get; set; }
        public string ShipmentMessage { get; set; }
        public string ManifestReportName { get; set; }
        public string COD { get; set; }
        public string ShipmentConsolidation { get; set; }
        public string PricingTicket { get; set; }
        public DateTime DontShipBeforeDate { get; set; }
        public string ShipToFaxNumber { get; set; }
        public string WaveNumber { get; set; }
        public string ReplenishmentGroup { get; set; }
        public string CustomerUDF1 { get; set; }
        public string CustomerUDF2 { get; set; }
        public string CustomerUDF3 { get; set; }
        public string BusinessTelephoneNumber { get; set; }
        public string Email { get; set; }
        public string CustomerUDF4 { get; set; }
        public string CustomerUDF5 { get; set; }
        public string OrderSource { get; set; }
		public decimal TotalInvoiceAmountCharge { get; set; }
        public string ShipAddress3 { get; set; }
        public string BillAddress3 { get; set; }
        public string ShipperAddress3 { get; set; }
        public string ShipperCountry { get; set; }
        public decimal TotalFreightToCharge { get; set; }
        public decimal TotalShippingHandlingCharge { get; set; }
        public decimal PromoAmount { get; set; }
        public decimal PromoDiscount { get; set; }
        public int EndOfLineProcess { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string Special1 { get; set; }
        public string Special2 { get; set; }
        public string Special3 { get; set; }
        public int MinimumDaysOfExpiry { get; set; }
        public bool MixedLotIndicator { get; set; }
        public string Extra1 { get; set; }
        public string Extra2 { get; set; }
        public string Extra3 { get; set; }
        public string Extra4 { get; set; }
        public string Extra5 { get; set; }
        public string Handle { get; set; }
        public string CustomerCartonContentLabel { get; set; }
        public CartonType CartonType { get; set; }
        public string WarehouseTransitLocation { get; set; }
        public string WarehouseDestinationLocation { get; set; }
        public string DestinationBinLocation { get; set; }
        public string PlannerID { get; set; }
        public string ControlBuyerID { get; set; }
        public string ScheduledStartDate { get; set; }
        public string FinishedGoodItemNumber { get; set; }
        public string FinishedGoodDescription { get; set; }
        public string FinishedGoodClassID { get; set; }
        public string MOENdingQuantity { get; set; }
        public string FirstWorkCenter { get; set; }
        public string MODescription { get; set; }
        public string PrimaryBillToAddress { get; set; }
        public string WorkArea { get; set; }
        public string AssemblyNote1 { get; set; }
        public string AssemblyNote2 { get; set; }
        public string AssemblyNote3 { get; set; }
        public string AssemblyNote4 { get; set; }
        public string AssemblyNote5 { get; set; }
		public string ShipperEIN { get; set; }
		public string ShipToEIN { get; set; }
        public string Shipper { get; set; } // Added for A1W 64
		public string IntegrationUDF1 { get; set; } // Added for A1W 64
		public string IntegrationUDF2 { get; set; } // Added for A1W 64
		public string IntegrationUDF3 { get; set; } // Added for A1W 64
		public string IntegrationUDF4 { get; set; } // Added for A1W 64
		public string IntegrationUDF5 { get; set; } // Added for A1W 64
		public string IntegrationUDF6 { get; set; } // Added for A1W 64
		public string IntegrationUDF7 { get; set; } // Added for A1W 64
		public string IntegrationUDF8 { get; set; } // Added for A1W 64
		public string IntegrationUDF9 { get; set; } // Added for A1W 64
		public string IntegrationUDF10 { get; set; } // Added for A1W 64
		public string IntegrationUDF11 { get; set; } // Added for A1W 64
		public string IntegrationUDF12 { get; set; } // Added for A1W 64
		public string IntegrationUDF13 { get; set; } // Added for A1W 64
		public string IntegrationUDF14 { get; set; } // Added for A1W 64
		public string IntegrationUDF15 { get; set; } // Added for A1W 64
		public string IntegrationUDF16 { get; set; } // Added for A1W 64
		public string IntegrationUDF17 { get; set; } // Added for A1W 64
		public string IntegrationUDF18 { get; set; } // Added for A1W 64
		public string IntegrationUDF19 { get; set; } // Added for A1W 64
		public string IntegrationUDF20 { get; set; } // Added for A1W 64
		public string IntegrationUDF21 { get; set; } // Added for A1W 64
		public string IntegrationUDF22 { get; set; } // Added for A1W 64
		public string IntegrationUDF23 { get; set; } // Added for A1W 64
		public string IntegrationUDF24 { get; set; } // Added for A1W 64
		public string IntegrationUDF25 { get; set; } // Added for A1W 64
        public List<PackslipLine> Lines { get; set; }

        public string SVAStagingID { get; set; } //SVA CUSTOM - ADDED

        //Functions
        #region ToString
        public override string ToString()
        {
            if (this.OrderNumber.Length == 0)
                return base.ToString();
            return this.OrderNumber;
        }
        #endregion
    }

    public enum ShipmentPaymentType
    {
        NotSpecified,
        Collect,
        BillRecipient,
        Prepaid,
        ThirdParty,
        DutyPaidThirdParty
    }

    public enum PurchaseOrderType
    {
        NotSpecified,
        StandAlone,
        Promotional,
        CustomerSpecialOrder,
        ReOrder,
        Transfer,
    }

    public enum CartonType
    {
        NotSpecified,
        CartonLabel,
        PalletLabel,
        TruckLoadLabel,
    }

	public partial class PackslipLine
    {
        #region Constructor
        public PackslipLine()
        {
            Attributes = new List<AttributeSet>();
            PickAttributeTracking = new AttributeTracking();
            ProductionRecipes = new List<ProductionRecipe>();
        }
        #endregion

        //properties
        public string Warehouse { get; set; }
        public string Location { get; set; }
        public string CustomerNumber { get; set; }
        public string OrderNumber { get; set; }
        public string BackOrderNumber { get; set; }
        public string StoreNumber { get; set; }
        public string DepartmentNumber { get; set; }
        public string PromoNumber { get; set; }
        public string OrderSequenceNumber { get; set; }
        public string CustomerLineReference { get; set; }
        public string ProductCode { get; set; }
        public string PrimaryLocation { get; set; }
        public string Description { get; set; }
        public string UnitOfMeasureMultiplier { get; set; }
        public string ProductClass { get; set; }
        public string UPC { get; set; }
        public decimal QuantityOrdered { get; set; }
        public decimal QuantityToPick { get; set; }
        public string HazmatCode { get; set; }
        public string CustomerXRef { get; set; }
        public string CommentIndicator { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Weight { get; set; }
        public decimal Cube { get; set; }
        public string StoreName { get; set; }
        public string StoreAddress1 { get; set; }
        public string StoreAddress2 { get; set; }
        public string StoreCity { get; set; }
        public string StoreProvinceState { get; set; }
        public string StorePostalZip { get; set; }
        public string StoreCountry { get; set; }
        public List<AttributeSet> Attributes { get; set; }
        public string SpecialInstructions1 { get; set; }
        public string SpecialInstructions2 { get; set; }
        public string SpecialInstructions3 { get; set; }
        public string SpecialInstructions4 { get; set; }
        public string SpecialInstructions5 { get; set; }
        public string SpecialInstructions6 { get; set; }
        public string SpecialInstructions7 { get; set; }
        public string Extra { get; set; }
        public AttributeTracking PickAttributeTracking { get; set; }
        public string CustomerPoNumber { get; set; }
        public decimal Discount { get; set; }
        public decimal RetailPrice { get; set; }
        public decimal ValuePrice { get; set; }
        public string PriceTicketDescription { get; set; }
        public int DaysToExpire { get; set; }
        public string VendorNumber { get; set; }
        public string CountryOfOrigin { get; set; }
        public decimal SellPrice { get; set; }
        public bool Tax1 { get; set; }
        public bool Tax2 { get; set; }
        public bool Tax3 { get; set; }
        public bool Tax4 { get; set; }
        public string StockItem { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string PurchaseOrderLineNumber { get; set; }
        public string EDPNumber { get; set; }
        public int Commitment { get; set; }
        public string Extra1 { get; set; }
        public string Extra2 { get; set; }
        public string Extra3 { get; set; }
        public string Extra4 { get; set; }
        public string Extra5 { get; set; }
        public VariableLengthType VariableLengthProduct { get; set; }
        public KitType KitType { get; set; }
		public decimal InnerPacksize { get; set; } // Changed to decimal for A1W 64
		public decimal MiddlePacksize { get; set; } // Changed to decimal for A1W 64
		public string UnitOfMeasureTextDescription { get; set; }
        public ReservationType ReservationType { get; set; }
        public string CommodityCode { get; set; }
        public string LicensePlate { get; set; }
        public string PackClass { get; set; }
        public string StoreAddress3 { get; set; }
        public string CommodityDescription { get; set; }
        public string NMFCNumber { get; set; }
        public string TariffClass { get; set; }
        public bool BackflushFlag { get; set; }
        public string Salesman { get; set; }
        public int MinimumDaysOfExpiry { get; set; }
		public string IntegrationUDF1 { get; set; } // Added for A1W 64
		public string IntegrationUDF2 { get; set; } // Added for A1W 64
		public string IntegrationUDF3 { get; set; } // Added for A1W 64
		public string IntegrationUDF4 { get; set; } // Added for A1W 64
		public string IntegrationUDF5 { get; set; } // Added for A1W 64
		public string IntegrationUDF6 { get; set; } // Added for A1W 64
		public string IntegrationUDF7 { get; set; } // Added for A1W 64
		public string IntegrationUDF8 { get; set; } // Added for A1W 64
		public string IntegrationUDF9 { get; set; } // Added for A1W 64
		public string IntegrationUDF10 { get; set; } // Added for A1W 64
		public string IntegrationUDF11 { get; set; } // Added for A1W 64
		public string IntegrationUDF12 { get; set; } // Added for A1W 64
		public string IntegrationUDF13 { get; set; } // Added for A1W 64
		public string IntegrationUDF14 { get; set; } // Added for A1W 64
		public string IntegrationUDF15 { get; set; } // Added for A1W 64
		public string IntegrationUDF16 { get; set; } // Added for A1W 64
		public string IntegrationUDF17 { get; set; } // Added for A1W 64
		public string IntegrationUDF18 { get; set; } // Added for A1W 64
		public string IntegrationUDF19 { get; set; } // Added for A1W 64
		public string IntegrationUDF20 { get; set; } // Added for A1W 64
		public string IntegrationUDF21 { get; set; } // Added for A1W 64
		public string IntegrationUDF22 { get; set; } // Added for A1W 64
		public string IntegrationUDF23 { get; set; } // Added for A1W 64
		public string IntegrationUDF24 { get; set; } // Added for A1W 64
		public string IntegrationUDF25 { get; set; } // Added for A1W 64
        public List<ProductionRecipe> ProductionRecipes { get; set; }

        //Functions
        #region ToString
        public override string ToString()
        {
            if (string.IsNullOrEmpty(this.OrderSequenceNumber))
                return base.ToString();
            return this.OrderSequenceNumber;
        }
        #endregion
    }

	public partial class ProductionRecipe : IClient3PL
    {
        #region Constructor
        public ProductionRecipe()
        {
            Attributes = new List<AttributeSet>();
            PickAttributeTracking = new AttributeTracking();    //SVA Added
            ReceiveAttributeTracking = new AttributeTracking(); //SVA Added
        }
        #endregion

        public string Warehouse { get; set; }
        public string CustomerNumber { get; set; }
        public string OrderNumber { get; set; }
        public string BackorderNumber { get; set; }
        public string OrderSequenceNumber { get; set; }
        public string WorkArea { get; set; }
        public AttributeTracking ReceiveAttributeTracking { get; set; }
        public string ProductCode { get; set; }
        public int PackQuantity { get; set; }
        public string Description { get; set; }
        public decimal QuantityToUse { get; set; }
        public string UnitOfMeasure { get; set; }
        public string PrimaryLocation { get; set; }
        public AttributeTracking PickAttributeTracking { get; set; }
        public string CommentIndicator { get; set; }
        public List<AttributeSet> Attributes { get; set; }
        public string ClientName { get; set; }
        public string Extra1 { get; set; }
        public string Extra2 { get; set; }
        public string Extra3 { get; set; }
        public string Extra4 { get; set; }
        public string Extra5 { get; set; }
        public ComponentType ComponentType { get; set; }
        public decimal QuantityBuilt { get; set; }
        public decimal ComponentCost { get; set; }
        public string ComponentSpecialInstructions { get; set; }
    }

    public enum ComponentType
    {
        NotSpecified,
        KitSubcomponent,
        CoProduct,
        ServiceItem,
    }

    public enum VariableLengthType
    {
        NotVLP,
        VLP,
        ContinuousVLP,
    }

    public enum KitType
    {
        NotSpecified,
        BuildToStockWorkOrder,
        BuildToOrderWorkOrder,
        ExplodeForPicking,
        ExplodeForPickingWithShortedComponents,
        RecipeOnly,
        ExplodeDownload
    }

    public enum ReservationType
    {
		NotSpecified,
        SeasonalReservation,
        CoreReservation,
    }

	public partial class PickConfirmation : IClient3PL
    {
        #region Constructor
        public PickConfirmation()
        {
            Shipments = new List<Shipment>();
        }
        #endregion

        public string OrderId { get; set; }
        public string BackOrderNum { get; set; }
        public string CustomerId { get; set; }
        public string StoreNum { get; set; }
        public string HostOrder { get; set; }
        public string SpecialsSelected { get; set; }
        public string CostCenter { get; set; }
        public string ShipToNum { get; set; }
        public string ClientName { get; set; }
        public string ShippedName { get; set; }
        public string ShippedAddress1 { get; set; }
        public string ShippedAddress2 { get; set; }
        public string ShippedAddress3 { get; set; }
        public string ShippedCity { get; set; }
        public string ShippedProvinceState { get; set; }
        public string ShippedPostalZip { get; set; }
        public string ShippedCountry { get; set; }
        public string ShippedAttnTo { get; set; }
        public string ShippedPhone { get; set; }
        public string CustomerPoNumber { get; set; }
        public string Extra1 { get; set; }
        public string Extra2 { get; set; }
        public string Extra3 { get; set; }
        public string Extra4 { get; set; }
        public string Extra5 { get; set; }
        public string Special1 { get; set; }
        public string Special2 { get; set; }
        public string Special3 { get; set; }
        public string POType { get; set; }
        public string InvoicePrinter { get; set; }
        public string BatchControlNum { get; set; }
        public decimal FreightHandlingAmmount { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime RequiredDate { get; set; }
		public string MasterBOLNumber { get; set; } // Added for A1W 64
        public string ShipmentConsolidationNumber { get; set; } // Added for A1W 64
        public List<Shipment> Shipments { get; set; }
    }

	public partial class Shipment
    {
        #region Constructor
        public Shipment()
        {
            this.Cartons = new List<Carton>();
        }
        #endregion

        public string ShipmentId { get; set; }
        public string ShipmentCarrier { get; set; }
        public string PickupId { get; set; }
        public string ShipmentServiceUsed { get; set; }
        public string DestinationWarehouse { get; set; }
        public string ShipPayAccount { get; set; }
        public string ShipPaymentType { get; set; }
        public string ShippingUDF1 { get; set; }
        public string ShippingUDF2 { get; set; }
        public string TrailerNum { get; set; }
        public string TruckRoute { get; set; }
        public string TruckStop { get; set; }
        public int NumberOfPiecesShipped { get; set; }
        public decimal TotalWeight { get; set; }
        public decimal TotalShipmentCharge { get; set; }
        public decimal ShipRateCharged { get; set; }
        public DateTime DateShipped { get; set; }
		public string IntegrationUDF1 { get; set; } // Added for A1W 64
		public string IntegrationUDF2 { get; set; } // Added for A1W 64
		public string IntegrationUDF3 { get; set; } // Added for A1W 64
		public string IntegrationUDF4 { get; set; } // Added for A1W 64
		public string IntegrationUDF5 { get; set; } // Added for A1W 64
		public string IntegrationUDF6 { get; set; } // Added for A1W 64
		public string IntegrationUDF7 { get; set; } // Added for A1W 64
		public string IntegrationUDF8 { get; set; } // Added for A1W 64
		public string IntegrationUDF9 { get; set; } // Added for A1W 64
		public string IntegrationUDF10 { get; set; } // Added for A1W 64
		public string IntegrationUDF11 { get; set; } // Added for A1W 64
		public string IntegrationUDF12 { get; set; } // Added for A1W 64
		public string IntegrationUDF13 { get; set; } // Added for A1W 64
		public string IntegrationUDF14 { get; set; } // Added for A1W 64
		public string IntegrationUDF15 { get; set; } // Added for A1W 64
		public string IntegrationUDF16 { get; set; } // Added for A1W 64
		public string IntegrationUDF17 { get; set; } // Added for A1W 64
		public string IntegrationUDF18 { get; set; } // Added for A1W 64
		public string IntegrationUDF19 { get; set; } // Added for A1W 64
		public string IntegrationUDF20 { get; set; } // Added for A1W 64
		public string IntegrationUDF21 { get; set; } // Added for A1W 64
		public string IntegrationUDF22 { get; set; } // Added for A1W 64
		public string IntegrationUDF23 { get; set; } // Added for A1W 64
		public string IntegrationUDF24 { get; set; } // Added for A1W 64
		public string IntegrationUDF25 { get; set; } // Added for A1W 64
		public string WMSUDF1 { get; set; } // Added for A1W 64
		public string WMSUDF2 { get; set; } // Added for A1W 64
		public string WMSUDF3 { get; set; } // Added for A1W 64
		public string WMSUDF4 { get; set; } // Added for A1W 64
        public List<Carton> Cartons { get; set; }

        #region Equals
        public override bool Equals(object obj)
        {
            Shipment inObj = obj as Shipment;
            if (inObj == null)
                return false;
            return inObj.ShipmentId.Equals(this.ShipmentId);
        }
        #endregion

        /// <summary>
        /// Definition is required for A1I.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

	public partial class Carton
    {
        #region Constructor
        public Carton()
        {
            Lines = new List<CartonLine>();
        }
        #endregion

        public string TrackTraceNumber { get; set; }
        public string Warehouse { get; set; }
        public string CartonASN { get; set; }
        public string PalletASN { get; set; }
        public string BoxSize { get; set; }
        public string SCC18 { get; set; }
        public string Dunnage { get; set; }
        public string AdditionalBarcodeReference { get; set; }
        public decimal TotalCube { get; set; }
        public decimal FreightAndHandling { get; set; }
        public List<CartonLine> Lines { get; set; }

        #region Equals
        public override bool Equals(object obj)
        {
            Carton inObj = obj as Carton;
            if (inObj == null)
                return false;
            return inObj.CartonASN.Equals(this.CartonASN);
        }
        #endregion

        /// <summary>
        /// Definition is required for A1I.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

	public partial class CartonLine
    {
        #region Constructor
        public CartonLine()
        {
            Attributes = new List<AttributeSet>();
        }
        #endregion

        public string RowId { get; set; }
        public string ProductCode { get; set; }
        public string LineNum { get; set; }
        public string UPC { get; set; }
        public string Extra1 { get; set; }
        public string Extra2 { get; set; }
        public string Extra3 { get; set; }
        public string Extra4 { get; set; }
        public string Extra5 { get; set; }
        public string DropShipWarehouse { get; set; }
        public string PickerId { get; set; }
        public string ProductSubstitute { get; set; }
        public string CustomerPartNumer { get; set; }
        public string Zone { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string PurchaseOrderLine { get; set; }
        public string TieBreaker { get; set; }
        public string HostOrder { get; set; }
        public string BinLocation { get; set; }
        public decimal QuantityPacked { get; set; }
        public decimal QuantityForPackslip { get; set; } //DRS 05/02/16
        public decimal QuantityOrdered { get; set; }
        public string IsComplete { get; set; } //DRS 05/02/16
		public decimal Packsize { get; set; } // Changed to decimal for A1W 64
		public string IntegrationUDF1 { get; set; } // Added for A1W 64
		public string IntegrationUDF2 { get; set; } // Added for A1W 64
		public string IntegrationUDF3 { get; set; } // Added for A1W 64
		public string IntegrationUDF4 { get; set; } // Added for A1W 64
		public string IntegrationUDF5 { get; set; } // Added for A1W 64
		public string IntegrationUDF6 { get; set; } // Added for A1W 64
		public string IntegrationUDF7 { get; set; } // Added for A1W 64
		public string IntegrationUDF8 { get; set; } // Added for A1W 64
		public string IntegrationUDF9 { get; set; } // Added for A1W 64
		public string IntegrationUDF10 { get; set; } // Added for A1W 64
		public string IntegrationUDF11 { get; set; } // Added for A1W 64
		public string IntegrationUDF12 { get; set; } // Added for A1W 64
		public string IntegrationUDF13 { get; set; } // Added for A1W 64
		public string IntegrationUDF14 { get; set; } // Added for A1W 64
		public string IntegrationUDF15 { get; set; } // Added for A1W 64
		public string IntegrationUDF16 { get; set; } // Added for A1W 64
		public string IntegrationUDF17 { get; set; } // Added for A1W 64
		public string IntegrationUDF18 { get; set; } // Added for A1W 64
		public string IntegrationUDF19 { get; set; } // Added for A1W 64
		public string IntegrationUDF20 { get; set; } // Added for A1W 64
		public string IntegrationUDF21 { get; set; } // Added for A1W 64
		public string IntegrationUDF22 { get; set; } // Added for A1W 64
		public string IntegrationUDF23 { get; set; } // Added for A1W 64
		public string IntegrationUDF24 { get; set; } // Added for A1W 64
		public string IntegrationUDF25 { get; set; } // Added for A1W 64
		public string WMSUDF1 { get; set; } // Added for A1W 64
		public string WMSUDF2 { get; set; } // Added for A1W 64
        public List<AttributeSet> Attributes { get; set; }

        #region Equals
        public override bool Equals(object obj)
        {
            CartonLine inObj = obj as CartonLine;
            if (inObj == null)
                return false;
            return inObj.LineNum.Equals(this.LineNum);
        }
        #endregion

        /// <summary>
        /// Definition is required for A1I.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}

