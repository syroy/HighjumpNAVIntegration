using System;
using System.Collections.Generic;
using System.Text;

namespace Accellos.Interfaces.NAV.BusinessObjects
{
    public class AttributeSet
    {
        #region Constructor
        public AttributeSet()
        {
            Attribute1 = string.Empty;
            Attribute2 = string.Empty;
            Attribute3 = string.Empty;
            Attribute4 = string.Empty;
            Attribute5 = string.Empty;
            Attribute6 = string.Empty;
            Attribute7 = string.Empty;
            Attribute8 = string.Empty;
            Attribute9 = string.Empty;
            Attribute10 = string.Empty;
        }
        #endregion

        public String RowId { get; set; }
        public decimal Quantity { get; set; }
        public string Attribute1 { get; set; }
        public string Attribute2 { get; set; }
        public string Attribute3 { get; set; }
        public string Attribute4 { get; set; }
        public string Attribute5 { get; set; }
        public string Attribute6 { get; set; }
        public string Attribute7 { get; set; }
        public string Attribute8 { get; set; }
        public string Attribute9 { get; set; }
        public string Attribute10 { get; set; }
        public DateTime ExpiryDate { get; set; }

        #region Empy
        public bool Empy
        {
            get
            {
                return string.IsNullOrEmpty(this.Attribute1) &&
                       string.IsNullOrEmpty(this.Attribute2) &&
                       string.IsNullOrEmpty(this.Attribute3) &&
                       string.IsNullOrEmpty(this.Attribute4) &&
                       string.IsNullOrEmpty(this.Attribute5) &&
                       string.IsNullOrEmpty(this.Attribute6) &&
                       string.IsNullOrEmpty(this.Attribute7) &&
                       string.IsNullOrEmpty(this.Attribute8) &&
                       string.IsNullOrEmpty(this.Attribute9) &&
                       string.IsNullOrEmpty(this.Attribute10) &&
                       (this.Quantity == 0.0m); //DNF - Added this condition (still interested in FIELD027 even if not attribute tracked, thus AttributeQuantity is necessary to compare to see if empty)
            }
        }
        #endregion

        #region Equals
        public override bool Equals(object obj)
        {
            AttributeSet inAttr = obj as AttributeSet;
            if (inAttr == null)
                return false;

            return inAttr.Attribute1.Equals(this.Attribute1) &&
                   inAttr.Attribute2.Equals(this.Attribute2) &&
                   inAttr.Attribute3.Equals(this.Attribute3) &&
                   inAttr.Attribute4.Equals(this.Attribute4) &&
                   inAttr.Attribute5.Equals(this.Attribute5) &&
                   inAttr.Attribute6.Equals(this.Attribute6) &&
                   inAttr.Attribute7.Equals(this.Attribute7) &&
                   inAttr.Attribute8.Equals(this.Attribute8) &&
                   inAttr.Attribute9.Equals(this.Attribute9) &&
                   inAttr.Attribute10.Equals(this.Attribute10);
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

	public partial class AttributeTracking
    {
        public TrackingType Attribute1Tracking { get; set; }
        public TrackingType Attribute2Tracking { get; set; }
        public TrackingType Attribute3Tracking { get; set; }
        public TrackingType Attribute4Tracking { get; set; }
        public TrackingType Attribute5Tracking { get; set; }
        public TrackingType Attribute6Tracking { get; set; }
        public TrackingType Attribute7Tracking { get; set; }
        public TrackingType Attribute8Tracking { get; set; }
        public TrackingType Attribute9Tracking { get; set; }
        public TrackingType Attribute10Tracking { get; set; }
    }

    public enum TrackingType
    {
        NotTracked,
        Unique,
        NonUnique
    }

    public partial interface IClient3PL
    {
        string ClientName { get; set; }
    }
}

