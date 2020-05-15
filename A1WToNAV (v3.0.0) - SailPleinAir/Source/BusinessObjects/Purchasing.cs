using System;
using System.Collections.Generic;
using System.Text;

namespace Accellos.Interfaces.NAV.BusinessObjects
{
	public partial class PurchaseOrder : IClient3PL
    {
        #region Constructor
        public PurchaseOrder()
        {
            Lines = new List<PurchaseOrderLine>();
        }
        #endregion

        public string ContainerBatchRef { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string VendorNumber { get; set; }
        public string VendorName { get; set; }
        public string BuyerName { get; set; }
        public bool AllowBackOrder { get; set; }
        public string ClientName { get; set; }
        public string SpecialInstructions1 { get; set; }
        public string SpecialInstructions2 { get; set; }
        public string SpecialInstructions3 { get; set; }
        public string SpecialInstructions4 { get; set; }
        public string SpecialInstructions5 { get; set; }
        public string SpecialInstructions6 { get; set; }
        public string SpecialInstructions7 { get; set; }
        public string BackOrder { get; set; }
        public bool IsRMA { get; set; }
        public DateTime DueDate { get; set; }
        public string Extra1 { get; set; }
        public string Extra2 { get; set; }
        public string Extra3 { get; set; }
        public string Extra4 { get; set; }
        public string Extra5 { get; set; }
        public string WarehouseTransferLocation { get; set; }
        public PoType PoType { get; set; }
        public string RequisitionNumber { get; set; }
        public string Email { get; set; }
        public string Packslip { get; set; }
        public string RequestPOD { get; set; }
        public string RequestSH { get; set; }
        public string NoSHReship { get; set; }
		public string OrderSource { get; set; } // Added for A1W 64
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
        public List<PurchaseOrderLine> Lines { get; set; }

        public string SVAStagingID { get; set; } //SVA CUSTOM - ADDED
    }

    public enum PoType
    {
        NotSpecified,
        Transfer,
        Requisition,
    }

	public partial class PurchaseOrderLine
    {
        #region Constructor
        public PurchaseOrderLine()
        {
            Attributes = new List<AttributeSet>();
            ReceiveAttributeTracking = new AttributeTracking();
        }
        #endregion

        public string Warehouse { get; set; }
        public string Location { get; set; }
        public string ContainerBatchRef { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string LineNumber { get; set; }
        public string ProductCode { get; set; }
        public string PrimaryLocation { get; set; }
        public string Description { get; set; }
        public string VendorProductNumber { get; set; }
        public string UnitOfMeasureMultiplier { get; set; }
        public string ProductClass { get; set; }
        public string UPC { get; set; }
        public decimal QuantityExpected { get; set; }
        public bool IsSpecial { get; set; }
        public string CustomerNumber { get; set; }
        public string CustomerName { get; set; }
        public string SalesOrderNumber { get; set; }
        public string SalesOrderLineNumber { get; set; }
        public List<AttributeSet> Attributes { get; set; }
        public AttributeTracking ReceiveAttributeTracking { get; set; }
        public DateTime ExpiryDate { get; set; }
        public StockItemIndicatorType StockItemIndicator { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal CostPrice { get; set; }
        public string Extra1 { get; set; }
        public string Extra2 { get; set; }
        public string Extra3 { get; set; }
        public string Extra4 { get; set; }
        public string Extra5 { get; set; }
        public DateTime ExpectedRequiredDate { get; set; }
        public string SpecialInstructions1 { get; set; }
        public string SpecialInstructions2 { get; set; }
        public string SpecialInstructions3 { get; set; }
        public string SpecialInstructions4 { get; set; }
        public string SpecialInstructions5 { get; set; }
        public string SpecialInstructions6 { get; set; }
        public string SpecialInstructions7 { get; set; }
        public string UnitOfMeasureText { get; set; }
        public string CountryOfOrigin { get; set; }
        public string ContainerStatus { get; set; }
        public string HostLineReference { get; set; }
        public string RMAReason { get; set; }
        public string RMARestockCharge { get; set; }
        public bool CreditNow { get; set; }
        public int Reship { get; set; }
        public bool ReInvoice { get; set; }
		public string OverReceiptPercentage { get; set; } // Added for A1W 64
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
    }

    public enum StockItemIndicatorType
    {
        NotSpecified,
        StockItem,
        NonStockItem,
    }

	public partial class ReceiptConfirmation : IClient3PL
    {
        #region Constructor
        public ReceiptConfirmation()
        {
            Lines = new List<ReceiptConfirmationLine>();
        }
        #endregion

        public string ClientName { get; set; }
        public string ContainerReceivingBatchRef { get; set; }
        public string PurchaseOrderId { get; set; }
        public string PackingSlipId { get; set; }
        public string Carrier { get; set; }
        public string AssignedContainerCost { get; set; }
        public string ShipmentPaymentType { get; set; }
        public string FreightStatus { get; set; }
        public string Extra1 { get; set; }
        public string Extra2 { get; set; }
        public string Extra3 { get; set; }
        public string Extra4 { get; set; }
        public string Extra5 { get; set; }
        public string BackOrderNum { get; set; }
        public string VesselNum { get; set; }
        public DateTime DateLeftPort { get; set; }
        public DateTime ArriveDate { get; set; }
        public string VendorId { get; set; }
        public string Email { get; set; }
        public string Packslip { get; set; }
        public bool RequestPOD { get; set; }
        public bool CreditSH { get; set; }
        public bool NoSHReship { get; set; }
        public string RMAReason { get; set; }
        public string RestockCharge { get; set; }
        public bool CreditNow { get; set; }
        public string ReShip { get; set; }
        public bool ReInvoice { get; set; }
		public string LicensePlate { get; set; } // Corrected for A1W 64
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
        public List<ReceiptConfirmationLine> Lines { get; set; }
    }

	public partial class ReceiptConfirmationLine
    {
        #region Constructor
        public ReceiptConfirmationLine()
        {
            Attributes = new List<AttributeSet>();
        }
        #endregion

        public string RowId { get; set; }
        public string Warehouse { get; set; }
        public string LineNum { get; set; }
        public string ProductCode { get; set; }
        public decimal QuantityExpected { get; set; }
        public DateTime ReceivedOnDate { get; set; }
        public string ReceivedBy { get; set; }
        public decimal QuantityReceived { get; set; }
        public string RMACode { get; set; }
		public DateTime ExpiryDate { get; set; }
        public string ReceivePutawayLocation { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal CostPrice { get; set; }
        public string Extra1 { get; set; }
        public string Extra2 { get; set; }
        public string Extra3 { get; set; }
        public string Extra4 { get; set; }
        public string Extra5 { get; set; }
        public string Tiebreaker { get; set; }
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
            ReceiptConfirmationLine inObj = obj as ReceiptConfirmationLine;
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

