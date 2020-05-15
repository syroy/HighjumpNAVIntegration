using System;
using System.Collections.Generic;
using System.Text;

namespace Accellos.Interfaces.NAV.BusinessObjects
{
    public partial class UploadConfirmation
    {
        public UploadConfirmation()
        {
            this.RowId = string.Empty;
			this.IsSuccess = true;
            this.Ignore = false;
            this.ErrorMessage = string.Empty;
        }

        public string RowId { get; set; }
		public bool IsSuccess { get; set; }
        public bool Ignore { get; set; }
        public string ErrorMessage { get; set; }

        public override bool Equals(object obj)
        {
            UploadConfirmation inObj = obj as UploadConfirmation;
            if (inObj == null)
                return false;
            return this.RowId.Equals(inObj.RowId);
        }

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

