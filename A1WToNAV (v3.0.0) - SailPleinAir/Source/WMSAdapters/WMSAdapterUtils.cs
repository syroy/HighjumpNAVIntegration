using System.Data;
using Accellos.Interfaces.NAV.BusinessObjects;
using System.Reflection;

namespace Accellos.Interfaces.NAV.WMSAdapters
{
    public partial class WMSAdapter
    {
        #region ParseTracking
        public static string ParseTracking(AttributeTracking tracking)
        {
            string result = string.Empty;

            if (tracking == null)
                return result;

            PropertyInfo[] properties = tracking.GetType().GetProperties();

            foreach (PropertyInfo property in properties)
            {
                if (!property.Name.StartsWith("Attribute"))
                    continue;

                TrackingType trackType = (TrackingType)property.GetValue(tracking, null);

                switch (trackType)
                {
                    case TrackingType.NotTracked:
                        result += "0";
                        break;
                    case TrackingType.NonUnique:
                        result += "1";
                        break;
                    case TrackingType.Unique:
                        result += "2";
                        break;
                }
            }

            return result;
        }
        #endregion

		#region IsWarehouse3PL
		public static bool IsWarehouse3PL()
		{
			string is3PL = string.Empty;

			is3PL = CompanyConfig.Instance.GetWarehouseDBConnection().ExecuteScalar("select C_VALUE from config nolock where config = '3PL_EDITION'").ToString();

			return is3PL != null && is3PL.Trim() == "Y";
		}
		#endregion
    }

    public static class WMSAdapterUtils
    {
        #region SVA DeleteAssemblyInstructions
        internal static void DeleteAssemblyInstructions(string orderNumber)
        {
            CompanyConfig.Instance.GetWarehouseDBConnection().ExecuteDataTableQuery(@"
delete from
    pickdetl
where
    ORDER_NUM = '{0}' and
    ORDER_SEQ like '%.%'", orderNumber);
        }
        #endregion

        public static string FetchSalesOrderNumber(string salesOrderID, string clientName)
        {
            string orderNumber;

            using (DataTable existingRbRecord = CompanyConfig.Instance.GetWarehouseDBConnection().ExecuteDataTableQuery(@"
select 
	* 
from 
	pickhead (nolock) 
where 
    ProcStep in (1,3,4,5,19) and 
	Extra1 = '36' and 
	Extra3 = '{0}' and
    ClientName = '{1}'", salesOrderID, clientName))
            {
                orderNumber = existingRbRecord.Rows.Count == 0 ? string.Empty : existingRbRecord.Rows[0]["ORDER_NUM"].ToString();
            }

            return orderNumber;
        }

        public static string FetchTransferOrderNumberPH(string transferOrderID, string clientName)
        {
            string orderNumber;

            using (DataTable existingRbRecord = CompanyConfig.Instance.GetWarehouseDBConnection().ExecuteDataTableQuery(@"
select 
	* 
from 
	pickhead (nolock) 
where 
    ProcStep in (1,3,4,5,19) and 
	Extra1 = '5740' and 
	Extra3 = '{0}' and
    ClientName = '{1}'", transferOrderID, clientName))
            {
                orderNumber = existingRbRecord.Rows.Count == 0 ? string.Empty : existingRbRecord.Rows[0]["ORDER_NUM"].ToString();
            }

            return orderNumber;
        }

        public static string FetchTransferOrderNumberRH(string transferOrderID, string clientName)
        {
            string orderNumber;

            using (DataTable existingRbRecord = CompanyConfig.Instance.GetWarehouseDBConnection().ExecuteDataTableQuery(@"
select 
	* 
from 
	recvhead (nolock)
where 
    RMA = 0 and 
    ProcStep in (32) and
	Extra1 = '5740' and 
	Extra3 = '{0}' and 
    ClientName = '{1}'", transferOrderID, clientName))
            {
                orderNumber = existingRbRecord.Rows.Count == 0 ? string.Empty : existingRbRecord.Rows[0]["PO_NUM"].ToString();
            }

            return orderNumber;
        }

        public static string FetchPurchaseOrderNumber(string purchaseOrderID, string clientName)
        {
            string orderNumber;

            using (DataTable existingRbRecord = CompanyConfig.Instance.GetWarehouseDBConnection().ExecuteDataTableQuery(@"
select 
	* 
from 
	recvhead (nolock)
where 
    RMA = 0 and 
    ProcStep in (32) and
	Extra1 = '38' and 
	Extra3 = '{0}' and 
    ClientName = '{1}'", purchaseOrderID, clientName))
            {
                orderNumber = existingRbRecord.Rows.Count == 0 ? string.Empty : existingRbRecord.Rows[0]["PO_NUM"].ToString();
            }

            return orderNumber;
        }

        public static string FetchRMAOrderNumber(string rmaOrderID, string clientName)
        {
            string orderNumber;

            using (DataTable existingRbRecord = CompanyConfig.Instance.GetWarehouseDBConnection().ExecuteDataTableQuery(@"
select 
	* 
from 
	recvhead (nolock)
where 
    RMA <> 0 and
    ProcStep in (32) and
	Extra1 = '38' and 
	Extra3 = '{0}' and 
    ClientName = '{1}'", rmaOrderID, clientName))
            {
                orderNumber = existingRbRecord.Rows.Count == 0 ? string.Empty : existingRbRecord.Rows[0]["PO_NUM"].ToString();
            }

            return orderNumber;
        }

        public static string FetchPurchaseReturnOrderNumber(string salesOrderID, string clientName)
        {
            string orderNumber;

            using (DataTable existingRbRecord = CompanyConfig.Instance.GetWarehouseDBConnection().ExecuteDataTableQuery(@"
select 
	* 
from 
	pickhead (nolock) 
where 
    ProcStep in (1,3,4,5,19) and 
	Extra1 = '38' and 
	Extra3 = '{0}' and
    ClientName = '{1}'", salesOrderID, clientName))
            {
                orderNumber = existingRbRecord.Rows.Count == 0 ? string.Empty : existingRbRecord.Rows[0]["ORDER_NUM"].ToString();
            }

            return orderNumber;
        }

        #region SVA FetchWorkOrderNumber
        public static string FetchWorkOrderNumber(string workOrderID, string clientName)
        {
            string orderNumber;

            using (DataTable existingRbRecord = CompanyConfig.Instance.GetWarehouseDBConnection().ExecuteDataTableQuery(@"
select 
	* 
from 
	pickhead (nolock) 
where 
    ProcStep in (1,3,4,5,19,22) and 
	Extra1 = '5405' and 
	ORDER_NUM = '{0}' and
    ClientName = '{1}'", workOrderID, clientName))
            {
                orderNumber = existingRbRecord.Rows.Count == 0 ? string.Empty : existingRbRecord.Rows[0]["ORDER_NUM"].ToString();
            }

            return orderNumber;
        }
        #endregion

        public static string CheckPurchaseOrderLockStatus(string orderNumber, string clientName)
        {
            string lockStatus;

            using (DataTable existingRbRecord = CompanyConfig.Instance.GetWarehouseDBConnection().ExecuteDataTableQuery(@"
select top 1
	PO_NUM,
    'LOCK_STATUS' = case when PROCSTEP in (32) then 'Unlocked' else 'Locked' end  
from 
	RECVHEAD (nolock)
where 
    RMA = 0 and
	EXTRA1 = '38' and 
	EXTRA3 = '{0}' and 
    CLIENTNAME = '{1}'", orderNumber, clientName))
            {
                lockStatus = existingRbRecord.Rows.Count == 0 ? "Unlocked" : existingRbRecord.Rows[0]["LOCK_STATUS"].ToString();
            }

            return lockStatus;
        }

        public static string CheckRMAOrderLockStatus(string orderNumber, string clientName)
        {
            string lockStatus;

            using (DataTable existingRbRecord = CompanyConfig.Instance.GetWarehouseDBConnection().ExecuteDataTableQuery(@"
select top 1
	PO_NUM,
    'LOCK_STATUS' = case when PROCSTEP in (32) then 'Unlocked' else 'Locked' end  
from 
	RECVHEAD (nolock)
where 
    RMA <> 0 and
	EXTRA1 = '38' and 
	EXTRA3 = '{0}' and 
    CLIENTNAME = '{1}'", orderNumber, clientName))
            {
                lockStatus = existingRbRecord.Rows.Count == 0 ? "Unlocked" : existingRbRecord.Rows[0]["LOCK_STATUS"].ToString();
            }

            return lockStatus;
        }

        public static string CheckSalesOrderLockStatus(string orderNumber, string clientName)
        {
            string lockStatus;

            using (DataTable existingRbRecord = CompanyConfig.Instance.GetWarehouseDBConnection().ExecuteDataTableQuery(@"
select top 1
	ORDER_NUM,
    'LOCK_STATUS' = case when PROCSTEP in (1,3,4,5,19) then 'Unlocked' else 'Locked' end  
from 
	PICKHEAD (nolock)
where 
	EXTRA1 = '36' and 
	EXTRA3 = '{0}' and 
    CLIENTNAME = '{1}'", orderNumber, clientName))
            {
                lockStatus = existingRbRecord.Rows.Count == 0 ? "Unlocked" : existingRbRecord.Rows[0]["LOCK_STATUS"].ToString();
            }

            return lockStatus;
        }

        public static string CheckTransferOrderLockStatus(string orderNumber, string clientName)
        {
            string lockStatus;

            using (DataTable existingRbRecord = CompanyConfig.Instance.GetWarehouseDBConnection().ExecuteDataTableQuery(@"
select top 1
	ORDER_NUM,
    'LOCK_STATUS' = case when PROCSTEP in (1,3,4,5,19) then 'Unlocked' else 'Locked' end  
from 
	PICKHEAD (nolock)
where 
	EXTRA1 = '5740' and 
	EXTRA3 = '{0}' and 
    CLIENTNAME = '{1}'", orderNumber, clientName))
            {
                lockStatus = existingRbRecord.Rows.Count == 0 ? "Unlocked" : existingRbRecord.Rows[0]["LOCK_STATUS"].ToString();
            }

            return lockStatus;
        }

        public static string CheckPurchaseReturnOrderLockStatus(string orderNumber, string clientName)
        {
            string lockStatus;

            using (DataTable existingRbRecord = CompanyConfig.Instance.GetWarehouseDBConnection().ExecuteDataTableQuery(@"
select top 1
	ORDER_NUM,
    'LOCK_STATUS' = case when PROCSTEP in (1,3,4,5,19) then 'Unlocked' else 'Locked' end  
from 
	PICKHEAD (nolock)
where 
	EXTRA1 = '38' and 
	EXTRA3 = '{0}' and 
    CLIENTNAME = '{1}'", orderNumber, clientName))
            {
                lockStatus = existingRbRecord.Rows.Count == 0 ? "Unlocked" : existingRbRecord.Rows[0]["LOCK_STATUS"].ToString();
            }

            return lockStatus;
        }
    }

	public enum UploadTypes
	{
		ReadyForUpload = 0,
		PendingUpload = 9,
		Uploaded = 1,
		Error = 2
	}
}

