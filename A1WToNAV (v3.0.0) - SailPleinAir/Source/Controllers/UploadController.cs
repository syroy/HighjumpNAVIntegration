using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Net;
using Accellos.InterfaceFramework;
using Accellos.InterfaceFramework.EventNotificationServer;
using Accellos.InterfaceFramework.ScriptManagementServer;
using Accellos.Interfaces.NAV.BusinessObjects;
using Accellos.Interfaces.NAV.NAVAdapters;
using Accellos.Interfaces.NAV.WMSAdapters;
using System.Text;
using System.Data.SqlClient;

//using Accellos.Interfaces.NAV.NAVWebServices;

namespace Accellos.Interfaces.NAV.Controllers
{
    [ExecutingClass]
    public class UploadController
    {
        private bool isInitialized;

        #region UpdateUpload
        /// <summary>
        /// This method updates a row in the A1W upload staging table 
        /// to an error status and adds the specified error message.
        /// </summary>
        /// <param name="rowID">
        /// Specifies the row id to update in the A1W upload staging table
        /// </param>
        /// <param name="errorMessage">
        /// Specifies the error message to add to the given row in 
        /// the A1W upload staging table.
        /// </param>
        private static void UpdateUpload(string rowID, String errorMessage)
        {
            if (errorMessage.Length > 300)
            {
                errorMessage = errorMessage.Substring(0, 300);
            }
            CompanyConfig.Instance.GetWarehouseDBConnection().ExecuteNonQuery(@"
update 
	upload 
set 
	uploaded = '{0}',
	message = '{1}'
where 
	rowid = '{2}'", (int)UploadTypes.Error, errorMessage, rowID);
        }

        /// <summary>
        /// This method updates a row in the A1W upload staging table
        /// to either a successful or error status based on the given
        /// arguments.
        /// </summary>
        /// <param name="isSuccess">
        /// Determines whether the row is to be marked successful or
        /// as erroring.
        /// </param>
        /// <param name="rowID">
        /// The row id in the A1W upload staging table to mark 
        /// as the given status.
        /// </param>
        private static void UpdateUpload(bool isSuccess, String rowID)
        {
            int uploaded = isSuccess ? (int)UploadTypes.Uploaded : (int)UploadTypes.Error;

            CompanyConfig.Instance.GetWarehouseDBConnection().ExecuteNonQuery(@"
update 
	upload 
set 
	uploaded = '{0}'
where 
	rowid = '{1}'", uploaded, rowID);
        }
        #endregion

        #region PreUpload
        /// <summary>
        /// This method loops over the given record type and
        /// client name and marks the matching ready to be uploaded
        /// records in the A1W upload staging table as being processed.
        /// </summary>
        /// <param name="recordType">
        /// The record type to be processed.
        /// </param>
        /// <param name="clientName">
        /// The client name of the given record type to be processed.
        /// </param>
        [ExecutingFunction]
        public void PreUpload(String recordType)
        {
            String fieldNum = String.Empty;
            switch (recordType)
            {
                case "XC":
                    fieldNum = "014";
                    break;
                case "RC":
                    fieldNum = "003";
                    break;
                case "PC":
                    fieldNum = "056";
                    break;
                case "MT":
                    fieldNum = "033";
                    break;
                case "ST":
                    fieldNum = "001";
                    break;
            }
            try
            {
                CompanyConfig.Instance.GetWarehouseDBConnection().ExecuteNonQuery(@"
                  update upload set uploaded = '{0}' where uploaded = '{1}' and field001 = '{2}' and field{3} = '{4}'", 
                    (int)UploadTypes.PendingUpload, (int)UploadTypes.ReadyForUpload, recordType, fieldNum, NAVAdapterUtils.getClientName());

                this.isInitialized = true;
                CompanyConfig.ResetConfiguration();
            }
            catch (Exception ex)
            {
                EventNotificationClient.Instance.PostMessage("NAV.Upload.PreUpload.Error", CompanyConfig.InstanceId, ex);
            }
        }
        #endregion

        #region SVA Post Records
        /// <summary>
        /// Calls the processing and posting routines for RC and PC records.
        /// </summary>
        [ExecutingFunction]
        public void postRecords()
        {
            try
            {
                EventNotificationClient.Instance.PostMessage("NAV.Upload.Post", CompanyConfig.InstanceId, "Post RC");
                CompanyConfig.Instance.NavWebServiceInstance.UploadStaging("RC");
                CompanyConfig.Instance.NavWebServiceInstance.Dispose();

                EventNotificationClient.Instance.PostMessage("NAV.Upload.Post", CompanyConfig.InstanceId, "Post PC");
                CompanyConfig.Instance.NavWebServiceInstance.UploadStaging("PC");
                CompanyConfig.Instance.NavWebServiceInstance.Dispose();

                EventNotificationClient.Instance.PostMessage("NAV.Upload.Post", CompanyConfig.InstanceId, "Post MT");
                CompanyConfig.Instance.NavWebServiceInstance.UploadStaging("MT");
                CompanyConfig.Instance.NavWebServiceInstance.Dispose();

                EventNotificationClient.Instance.PostMessage("NAV.Upload.Post", CompanyConfig.InstanceId, "Finished Posting");
            }
            catch (Exception ex)
            {
                EventNotificationClient.Instance.PostMessage("NAV.Upload.Post.Error", CompanyConfig.InstanceId, "Error while attempting to post.", new Variable("message", ex.Message), new Variable("full exception", ex.ToString()));
            }
        }
        #endregion

        #region SVA PurchaseOrderUpload
        /// <summary>
        /// Executing function for Receipt Confirmation uploads. This will call the appropriate
        /// methods to determine what Receipt Confirmations are in A1W's upload staging table 
        /// awaiting import to NAV, select and move them to the NAV's upload staging
        /// table and call NAV's processing methods (via calls to various classes/methods).
        /// </summary>
        [ExecutingFunction]
        public void PurchaseOrderUpload()
        {
            try
            {
                EventNotificationClient.Instance.PostMessage("NAV.Upload.PurchaseOrder", CompanyConfig.InstanceId, "Upload started");

                this.isInitialized = true;
                CompanyConfig.ResetConfiguration();

                if (!this.isInitialized)
                {
                    throw new Exception(string.Format("Cannot execute {0}, environment not initialized", MethodBase.GetCurrentMethod().Name));
                }

                PreUpload("RC");
                PreUpload("XC");

                #region Retrieve list of all PO that are pending for upload
                string clientName = NAVAdapterUtils.getClientName();

                using (DataTable pendingUpload = CompanyConfig.Instance.GetWarehouseDBConnection().ExecuteDataTableQuery(@"
                    select distinct	rowid, field017, field015 from upload where	field001 = 'XC' and	field002 = 'R' and field009 = 'UPLOADED' and uploaded = '{0}' and field014 = '{1}'",
                    (int)UploadTypes.PendingUpload, clientName))
                {
                    if (pendingUpload.Rows.Count == 0)
                    {
                        EventNotificationClient.Instance.PostMessage("NAV.Upload.PurchaseOrder", CompanyConfig.InstanceId, "No Receipts to Upload");
                        return;
                    }

                    foreach (DataRow pendingUploadRecord in pendingUpload.Rows)
                    {
                        #region Upload each PO individually
                        using (DataTable records = CompanyConfig.Instance.GetWarehouseDBConnection().ExecuteDataTableQuery(@"
                          select * from	upload where field001 = 'RC' and field044 = '{0}' and uploaded = '{1}' and field003 = '{2}' and field005 = '{3}' and field046 <> 'RMA' order by field006, TIMESTMP",
                          pendingUploadRecord["field015"], (int)UploadTypes.PendingUpload, clientName, pendingUploadRecord["field017"]))
                        
                           {
                               EventNotificationClient.Instance.PostMessage("NAV.Upload.PurchaseOrder", CompanyConfig.InstanceId, "WMS data read",
                                   new Variable("data", ControllerUtils.DataTable2LogString(records)));

                            try
                            {
                                bool fullOrderSuccess = true;

                                if (records.Rows.Count == 0)
                                {
                                    EventNotificationClient.Instance.PostMessage("NAV.Upload.PurchaseOrder", CompanyConfig.InstanceId, "No Receipts to Upload");
                                    UpdateUpload(false, pendingUploadRecord["rowid"].ToString().Trim()); // Set XC record to error
                                    
                                    continue;
                                }

                                //Parse WMS records and produce Business object
                                List<ReceiptConfirmation> receiptConfirmations = WMSAdapter.ParseReceiptConfirmation(records);

                                EventNotificationClient.Instance.PostMessage("NAV.Upload.PurchaseOrder", CompanyConfig.InstanceId,
                                    "WMS data read", new Variable("data", ControllerUtils.DataTable2LogString(records)));

                                EventNotificationClient.Instance.PostMessage("NAV.Upload.PurchaseOrder", CompanyConfig.InstanceId, "WMS -> Business object",
                                    new Variable("data", Utils.SerializeToString(receiptConfirmations)));

                                switch (CompanyConfig.Instance.UploadTransportMethod)
                                {
                                    case TransportMethod.SQL:
                                     //   fullOrderSuccess = ReceiptUploadSQL(receiptConfirmation);
                                        break;
                                    case TransportMethod.WebService:
                                        fullOrderSuccess = ReceiptUploadWS(receiptConfirmations);
                                        break;
                                }

                                if (fullOrderSuccess)
                                {
                                    #region Mark Successful
                                    foreach (DataRow rec in records.Rows) // set RC records to success
                                        UpdateUpload(true, rec["rowid"].ToString().Trim());

                                    UpdateUpload(true, pendingUploadRecord["rowid"].ToString().Trim()); // set XC record to success

                                    EventNotificationClient.Instance.PostMessage("NAV.Upload.PurchaseOrder", CompanyConfig.InstanceId,
                                        string.Format("Purchase order [{0}] processed successfully", pendingUploadRecord["field017"]));
                                    #endregion
                                }
                                else
                                {
                                    #region Mark as Error
                                    UpdateUpload(false, pendingUploadRecord["rowid"].ToString().Trim()); // set XC record to error

                                    foreach (DataRow rec in records.Rows)
                                        UpdateUpload(false, rec["rowid"].ToString().Trim());

                                    EventNotificationClient.Instance.PostMessage("NAV.Upload.PurchaseOrder.Error",
                                        CompanyConfig.InstanceId, string.Format("Error uploading PO {0} due to line level errors. Please see suspended uploads report for details.",
                                        pendingUploadRecord["field017"]));
                                    #endregion
                                }
                            }
                            catch (Exception ex)
                            {
                                #region Mark as error
                                foreach (DataRow rec in records.Rows)
                                    UpdateUpload(rec["rowid"].ToString().Trim(), ex.Message);

                                UpdateUpload(false, pendingUploadRecord["rowid"].ToString().Trim()); // Set XC record to error

                                EventNotificationClient.Instance.PostMessage("NAV.Upload.PurchaseOrder.Error", CompanyConfig.InstanceId,
                                    string.Format(string.Format("Error uploading PO {0}", pendingUploadRecord["field017"])),
                                    new Variable("message", ex.Message), new Variable("full exception", ex.ToString()));
                                #endregion
                            }
                        }
                        #endregion
                    }
                }
                #endregion

                EventNotificationClient.Instance.PostMessage("NAV.Upload.PurchaseOrder", CompanyConfig.InstanceId,
                    "Calling NAV web service (to process orders)", new Variable("Web Service URL", CompanyConfig.Instance.NavWebServiceInstance.Url));
                CompanyConfig.Instance.NavWebServiceInstance.UploadStaging("RC");
                CompanyConfig.Instance.NavWebServiceInstance.Dispose();
            }
            catch (Exception ex)
            {
                #region Generic error handling
                EventNotificationClient.Instance.PostMessage("NAV.Upload.PurchaseOrder.Error", CompanyConfig.InstanceId, ex.Message,
                    new Variable("message", ex.Message), new Variable("full exception", ex.ToString()));
                #endregion
            }
        }

        private bool ReceiptUploadSQL(List<ReceiptConfirmation> receiptConfirmations)
        {
            bool retVal = true;
            StringBuilder sbSQL;
            object[] parms;
            string strOutputParm = "";
            object oRetVal;

            sbSQL = new StringBuilder();
            sbSQL.Append("exec [");
            sbSQL.Append(CompanyConfig.Instance.CompanyId.ToString());
            sbSQL.Append("$SVA_UploadMT] {0}, {1}");

            parms = new object[2];
            parms[0] = Utils.SerializeToString(receiptConfirmations);
            parms[1] = strOutputParm;

            EventNotificationClient.Instance.PostMessage("NAV.Upload.PurchaseOrder.SQL", CompanyConfig.InstanceId, "Business object -> NAV SQL", new Variable("XML Data", parms[0]));
            oRetVal = CompanyConfig.Instance.GetNAVDBConnection().ExecuteScalar(sbSQL.ToString(), parms);
            if ((oRetVal != null) && (!(String.IsNullOrEmpty(oRetVal.ToString().Trim()))))
            {
                throw new Exception(oRetVal.ToString());
            }

            return retVal;
        }

        private static bool ReceiptUploadWS(List<ReceiptConfirmation> receiptConfirmations)
        {
            bool retVal = true;

            //Pass to Business logic adapter
            EventNotificationClient.Instance.PostMessage("NAV.Upload.PurchaseOrder", CompanyConfig.InstanceId, "Business object -> NAV");
            List<UploadConfirmation> uploadResults = PurchaseNAVAdaptor.WriteToNAV(receiptConfirmations);

            #region Check if Any Lines Failed
            foreach (UploadConfirmation confirmation in uploadResults)
            {
                if (confirmation.Ignore)
                    continue;

                if (!confirmation.IsSuccess)
                {
                    #region Mark as Error
                    retVal = false;

                    UpdateUpload(confirmation.RowId, confirmation.ErrorMessage);

                    if (!string.IsNullOrEmpty(confirmation.ErrorMessage))
                    {
                        EventNotificationClient.Instance.PostMessage("NAV.Upload.PurchaseOrder.Error",
                            CompanyConfig.InstanceId, string.Format("Line failed: [{0}]", confirmation.ErrorMessage),
                            new Variable("RowId", confirmation.RowId), new Variable("Error", confirmation.ErrorMessage));
                    }
                    #endregion
                }
            }
            #endregion

            EventNotificationClient.Instance.PostMessage("NAV.Upload.PurchaseOrder", CompanyConfig.InstanceId,
                string.Format("{0} purchase order lines read", uploadResults.Count));

            return retVal;
        }
        #endregion

        #region PurgeNavUploadStaging
        [ExecutingFunction]
        public void PurgeNavUploadStaging()
        {
            try
            {
                this.isInitialized = true;
                CompanyConfig.ResetConfiguration();

                if (!this.isInitialized)
                {
                    throw new Exception(string.Format("Cannot execute {0}, environment not initialized", MethodBase.GetCurrentMethod().Name));
                }
                EventNotificationClient.Instance.PostMessage("NAV.Upload.PurgeNavUploadStaging", CompanyConfig.InstanceId,
                 "Calling NAV web service to Purge Upload Staging", new Variable("Web Service URL", CompanyConfig.Instance.NavWebServiceInstance.Url));

                var recordsPurged = CompanyConfig.Instance.NavWebServiceInstance.PurgeUploadStaging();

                CompanyConfig.Instance.NavWebServiceInstance.Dispose();

                EventNotificationClient.Instance.PostMessage("NAV.Upload.PurgeNavUploadStaging", CompanyConfig.InstanceId, "Records Purged: " + recordsPurged,
                   new Variable("message", "Records Purged: " + recordsPurged));

            }
            catch (Exception ex)
            {
                #region Generic error handling
                EventNotificationClient.Instance.PostMessage("NAV.Upload.PurgeNavUploadStaging.Error", CompanyConfig.InstanceId, ex.Message,
                    new Variable("message", ex.Message), new Variable("full exception", ex.ToString()));
                #endregion
            }
        }
        #endregion


        #region SVA SalesOrderUpload
        /// <summary>
        /// Executing function for Pick Confirmation uploads. This will call the appropriate
        /// methods to determine what Pick Confirmations are in A1W's upload staging table 
        /// awaiting import to NAV, select and move them to the NAV's upload staging
        /// table and call NAV's processing methods (via calls to various classes/methods).
        /// </summary>
        [ExecutingFunction]
        public void SalesOrderUpload()
        {
            try
            {
                EventNotificationClient.Instance.PostMessage("NAV.Upload.SalesOrder", CompanyConfig.InstanceId, "Uploaded started");

                this.isInitialized = true;
                CompanyConfig.ResetConfiguration();

                if (!this.isInitialized)
                {
                    throw new Exception(string.Format("Cannot execute {0}, environment not initialized", MethodBase.GetCurrentMethod().Name));
                }

                PreUpload("PC");
                PreUpload("XC");
        
                #region Retrieve list of all SO that are pending for upload

                string clientName = NAVAdapterUtils.getClientName();

                using (DataTable pendingUpload = CompanyConfig.Instance.GetWarehouseDBConnection().ExecuteDataTableQuery(@"
                    select distinct	rowid, field003, field015 from upload where	field001 = 'XC' and	field002 = 'P' and field009 = 'UPLOADED' and uploaded = '{0}' and field014 = '{1}'", 
                    (int)UploadTypes.PendingUpload, clientName))
                {
                    if (pendingUpload.Rows.Count == 0)
                    {
                        EventNotificationClient.Instance.PostMessage("NAV.Upload.SalesOrder", CompanyConfig.InstanceId, "No Shipments to Upload");
                        return;
                    }

                    foreach (DataRow pendingUploadRecord in pendingUpload.Rows)
                    {
                        #region Upload each SO individually
                        using (DataTable records = CompanyConfig.Instance.GetWarehouseDBConnection().ExecuteDataTableQuery(@"
                          select * from	upload where field001 = 'PC' and field089 = '{0}' and uploaded = '{1}' and field056 = '{2}' and field004 = '{3}' order by field010, TIMESTMP", 
                          pendingUploadRecord["field015"], (int)UploadTypes.PendingUpload, clientName, pendingUploadRecord["field003"]))
                        {
                            EventNotificationClient.Instance.PostMessage("NAV.Upload.SalesOrder", CompanyConfig.InstanceId, "WMS data read",
                                new Variable("data", ControllerUtils.DataTable2LogString(records)));

                            try
                            {
                                bool fullOrderSuccess = true;

                                if (records.Rows.Count == 0)
                                {
                                    EventNotificationClient.Instance.PostMessage("NAV.Upload.SalesOrder", CompanyConfig.InstanceId, "No Picks to Upload");
                                    UpdateUpload(false, pendingUploadRecord["rowid"].ToString().Trim()); // Set XC record to error

                                    continue;
                                }

                                //Parse WMS records and produce Business object
                               List<PickConfirmation> pickConfirmations = WMSAdapter.ParsePickConfirmation(records);

                                //EventNotificationClient.Instance.PostMessage("NAV.Upload.SalesOrder", CompanyConfig.InstanceId,
                                //    "WMS -> Business object", new Variable("data", Utils.SerializeToString(pickConfirmations)));

                                switch (CompanyConfig.Instance.UploadTransportMethod)
                                {
                                    case TransportMethod.SQL:
                                    //    fullOrderSuccess = ShipmentUploadSQL(pickConfirmations;
                                        break;
                                    case TransportMethod.WebService:
                                        fullOrderSuccess = ShipmentUploadWS(pickConfirmations);
                                        break;
                                }

                                if (fullOrderSuccess)
                                {
                                    #region Mark Successful
                                    foreach (DataRow rec in records.Rows)
                                    {
                                        UpdateUpload(true, rec["rowid"].ToString().Trim()); // Set PC records to success
                                    }

                                  
                                    UpdateUpload(true, pendingUploadRecord["rowid"].ToString().Trim()); // Set XC record to success

                                    EventNotificationClient.Instance.PostMessage("NAV.Upload.SalesOrder", CompanyConfig.InstanceId,
                                        string.Format("Sales order [{0}] processed successfully", pendingUploadRecord["field003"]));
                                    #endregion
                                }
                                else
                                {
                                    #region Mark as Error
                                  
                                    UpdateUpload(false, pendingUploadRecord["rowid"].ToString().Trim()); // Set XC record to error

                                    foreach (DataRow rec in records.Rows)
                                    {
                                        UpdateUpload(false, rec["rowid"].ToString().Trim());
                                    }

                                    EventNotificationClient.Instance.PostMessage("NAV.Upload.SalesOrder.Error", CompanyConfig.InstanceId,
                                        string.Format("Error uploading SO {0} due to line level errors. Please see suspended uploads report for details.",
                                        pendingUploadRecord["rowid"]));
                                    #endregion
                                }
                            }
                            catch (Exception ex)
                            {
                                #region Mark as Error
                                foreach (DataRow rec in records.Rows)
                                {
                                    UpdateUpload(rec["rowid"].ToString().Trim(), ex.Message);
                                }

                                // ToDo: not sure why this is commented out?
                                // ACC.AD Keeep XCs for upload
                                //UpdateUpload(false, pendingUploadRecord["rowid"].ToString().Trim()); // Set XC record to error

                                EventNotificationClient.Instance.PostMessage("NAV.Upload.SalesOrder.Error", CompanyConfig.InstanceId,
                                    string.Format(string.Format("Error uploading SO {0}", pendingUploadRecord["rowid"])),
                                    new Variable("message", ex.Message), new Variable("full exception", ex.ToString()));
                                #endregion
                            }
                        }
                        #endregion
                    }
                }
                #endregion

                EventNotificationClient.Instance.PostMessage("NAV.Upload.SalesOrder", CompanyConfig.InstanceId,
                    "Calling NAV web service (to process orders)", new Variable("Web Service URL", CompanyConfig.Instance.NavWebServiceInstance.Url));
                CompanyConfig.Instance.NavWebServiceInstance.UploadStaging("PC");
                CompanyConfig.Instance.NavWebServiceInstance.Dispose();
            }
            catch (Exception ex)
            {
                #region Generic error handling
                EventNotificationClient.Instance.PostMessage("NAV.Upload.SalesOrder.Error", CompanyConfig.InstanceId, string.Format(ex.Message),
                    new Variable("message", ex.Message), new Variable("full exception", ex.ToString()));
                #endregion
            }
        }

        private bool ShipmentUploadSQL(PickConfirmation pickConfirmation)
        {
            bool retVal = true;
            //StringBuilder sbSQL;
            //object[] parms;
            string strOutputParm = "";
            object oRetVal;
            SqlCommand sqlCmd;

            sqlCmd = new SqlCommand(string.Format("[{0}$SVA_UploadPC]", CompanyConfig.Instance.CompanyId.ToString().Replace(".","_")),
                                     CompanyConfig.Instance.GetNAVDBConnection().SqlConnection);

            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.Parameters.Add(new SqlParameter("@BO", Utils.SerializeToString(pickConfirmation)));

            var outParam = new SqlParameter("@Err", SqlDbType.VarChar);
            outParam.Direction = ParameterDirection.Output;
            outParam.Size = 8000;

            sqlCmd.Parameters.Add(outParam);

            oRetVal = sqlCmd.ExecuteScalar();

            strOutputParm = sqlCmd.Parameters["@Err"].Value as string;

            if (!string.IsNullOrWhiteSpace(strOutputParm))
                throw new Exception(strOutputParm);

            //sbSQL = new StringBuilder();
            //sbSQL.Append("exec [");
            //sbSQL.Append(CompanyConfig.Instance.CompanyId.ToString());
            //sbSQL.Append("$SVA_UploadMT] {0}, {1}");

            //parms = new object[2];
            //parms[0] = Utils.SerializeToString(pickConfirmation);
            //parms[1] = strOutputParm;

            //EventNotificationClient.Instance.PostMessage("NAV.Upload.SalesOrder.SQL", CompanyConfig.InstanceId, "Business object -> NAV SQL", new Variable("XML Data", parms[0]));
            //oRetVal = CompanyConfig.Instance.GetNAVDBConnection().ExecuteScalar(sbSQL.ToString(), parms);
            //if ((oRetVal != null) && (!(String.IsNullOrEmpty(oRetVal.ToString().Trim()))))
            //{
            //    throw new Exception(oRetVal.ToString());
            //}

            return retVal;
        }

        private static bool ShipmentUploadWS(List<PickConfirmation> pickConfirmations)
        {
            bool retVal = true;

            //Pass Business object to NAV adapter
            List<UploadConfirmation> uploadResults = SalesNAVAdapter.WriteToNAV(pickConfirmations);

            EventNotificationClient.Instance.PostMessage("NAV.Upload.SalesOrder", CompanyConfig.InstanceId, "Business object -> NAV", new Variable("Upload Results Count", uploadResults.Count));

            #region Check if Any Lines Failed
            foreach (UploadConfirmation confirmation in uploadResults)
            {
                //EventNotificationClient.Instance.PostMessage("NAV.Upload.SalesOrder", CompanyConfig.InstanceId, "Is Success", 
                //    new Variable("Ignore?", confirmation.IsSuccess), new Variable("Success?", confirmation.IsSuccess));

                if (confirmation.Ignore)
                {
                    continue;
                }

                if (!confirmation.IsSuccess)
                {
                    #region Mark as Error
                    retVal = false;

                    UpdateUpload(confirmation.RowId, confirmation.ErrorMessage);

                    if (!string.IsNullOrEmpty(confirmation.ErrorMessage))
                    {
                        EventNotificationClient.Instance.PostMessage("NAV.Upload.SalesOrder.Error",
                            CompanyConfig.InstanceId, string.Format("Line failed: [{0}]", confirmation.ErrorMessage),
                            new Variable("RowId", confirmation.RowId), new Variable("Error", confirmation.ErrorMessage));
                    }
                    #endregion
                }
            }
            #endregion

            //EventNotificationClient.Instance.PostMessage("NAV.Upload.SalesOrder", CompanyConfig.InstanceId,
            //     string.Format("{0} sales order lines read", uploadResults.Count));

            return retVal;
        }
        #endregion

        #region SVA AdjustmentsUpload
        /// <summary>
        /// Executing function for Adjustment uploads. This will call the appropriate
        /// methods to determine what Adjustments are in A1W's upload staging table 
        /// awaiting import to NAV, select and move them to the NAV's upload staging
        /// table and call NAV's processing methods (via calls to various classes/methods).
        /// </summary>
        [ExecutingFunction]
        public void AdjustmentsUpload()
        {
            try
            {
                EventNotificationClient.Instance.PostMessage("NAV.Upload.Adjustment", CompanyConfig.InstanceId, "Uploaded started");

                this.isInitialized = true;
                CompanyConfig.ResetConfiguration();

                if (!this.isInitialized)
                {
                    throw new Exception(string.Format("Cannot execute {0}, environment not initialized", MethodBase.GetCurrentMethod().Name));
                }

                PreUpload("MT");

                #region Upload all adjustments found

                using (DataTable pendingUploadrecords = CompanyConfig.Instance.GetWarehouseDBConnection().ExecuteDataTableQuery(@"
select top(50)
    *
from
	upload
where
	uploaded in ('{0}','{1}') and 
	field001 = 'MT' and
    field033 = '{2}'
order by TIMESTMP", (int)UploadTypes.ReadyForUpload, (int)UploadTypes.PendingUpload, NAVAdapterUtils.getClientName()))
                {
                    if (pendingUploadrecords.Rows.Count == 0)
                    {
                        EventNotificationClient.Instance.PostMessage("NAV.Upload.Adjustment", CompanyConfig.InstanceId, "No Adjustments to Upload");
                        return;
                    }

                    try
                    {
                        EventNotificationClient.Instance.PostMessage("NAV.Upload.Adjustment", CompanyConfig.InstanceId, "WMS data read",
                            new Variable("data", ControllerUtils.DataTable2LogString(pendingUploadrecords)));

                        //Parse WMS records and produce Business object
                        List<Adjustment> adjustments = WMSAdapter.ParseAdjustments(pendingUploadrecords);

                        EventNotificationClient.Instance.PostMessage("NAV.Upload.Adjustment", CompanyConfig.InstanceId, "WMS -> Business object",
                            new Variable("data", Utils.SerializeToString(adjustments)));

                        switch (CompanyConfig.Instance.UploadTransportMethod)
                        {
                            case TransportMethod.SQL:
                                AdjustmentUploadSQL(adjustments);
                                break;
                            case TransportMethod.WebService:
                                AdjustmentUploadWS(adjustments);
                                break;
                        }

                        EventNotificationClient.Instance.PostMessage("NAV.Upload.Adjustment", CompanyConfig.InstanceId,
                            string.Format("{0} Adjustments processed", adjustments.Count));
                    }
                    catch (Exception ex)
                    {
                        #region Mark as Error
                        foreach (DataRow rec in pendingUploadrecords.Rows)
                        {
                            UpdateUpload(rec["rowid"].ToString().Trim(), ex.Message);
                        }

                        EventNotificationClient.Instance.PostMessage("NAV.Upload.Adjustment.Error", CompanyConfig.InstanceId,
                            string.Format("Error uploading adjustments"), new Variable("message", ex.Message),
                            new Variable("full exception", ex.ToString()));
                        #endregion
                    }
                }
                #endregion

                EventNotificationClient.Instance.PostMessage("NAV.Upload.Adjustment", CompanyConfig.InstanceId,
                    "Calling NAV web service (to process adjustments)", new Variable("Web Service URL", CompanyConfig.Instance.NavWebServiceInstance.Url));
                CompanyConfig.Instance.NavWebServiceInstance.UploadStaging("MT");
                CompanyConfig.Instance.NavWebServiceInstance.Dispose();
            }
            catch (Exception ex)
            {
                EventNotificationClient.Instance.PostMessage("NAV.Upload.Adjustment.Error", CompanyConfig.InstanceId, ex);
            }
        }

        private void AdjustmentUploadSQL(List<Adjustment> adjustments)
        {
            StringBuilder sbSQL;
            object[] parms;
            string strOutputParm = "";
            object retVal;

            sbSQL = new StringBuilder();
            sbSQL.Append("exec [");
            sbSQL.Append(CompanyConfig.Instance.CompanyId.ToString());
            sbSQL.Append("$SVA_UploadMT] {0}, {1}");

            parms = new object[2];
            parms[0] = Utils.SerializeToString(adjustments);
            parms[1] = strOutputParm;

            EventNotificationClient.Instance.PostMessage("NAV.Upload.Adjustment.SQL", CompanyConfig.InstanceId, "Business object -> NAV SQL", new Variable("XML Data", parms[0]));
            retVal = CompanyConfig.Instance.GetNAVDBConnection().ExecuteScalar(sbSQL.ToString(), parms);
            if ((retVal != null) && (!(String.IsNullOrEmpty(retVal.ToString().Trim()))))
            {
                foreach (Adjustment adj in adjustments)
                {
                    UpdateUpload(adj.RowId, retVal.ToString());
                }

                throw new Exception(retVal.ToString());
            }
            else
            {
                foreach (Adjustment adj in adjustments)
                {
                    UpdateUpload(true, adj.RowId);
                }
            }
        }

        private static void AdjustmentUploadWS(List<Adjustment> adjustments)
        {
            //Pass Business object to NAV adapter
            List<UploadConfirmation> uploadResults = AdjustmentNAVAdapter.WriteToNAV(adjustments);

            EventNotificationClient.Instance.PostMessage("NAV.Upload.Adjustment", CompanyConfig.InstanceId, "Business object -> NAV",
                new Variable("Upload Results Count", uploadResults.Count));

            #region Check if any lines failed
            foreach (UploadConfirmation confirmation in uploadResults)
            {
                //EventNotificationClient.Instance.PostMessage("NAV.Upload.Adjustment", CompanyConfig.InstanceId, "Check for Failures",
                //    new Variable("Is Success?", confirmation.IsSuccess), new Variable("Row ID", confirmation.RowId));

                if (confirmation.Ignore)
                {
                    continue;
                }

                if (confirmation.IsSuccess)
                {
                    UpdateUpload(true, confirmation.RowId); // Mark successful
                }
                else
                {
                    #region Mark as Error
                    UpdateUpload(confirmation.RowId, confirmation.ErrorMessage);

                    EventNotificationClient.Instance.PostMessage("NAV.Upload.Adjustment.Error", CompanyConfig.InstanceId,
                        string.Format("Adjustment with RowId [{0}] failed", confirmation.RowId),
                        new Variable("RowId", confirmation.RowId), new Variable("Error", confirmation.ErrorMessage));
                    #endregion
                }
            }
            #endregion
        }
        #endregion

        #region SVA XCUpload
        /// <summary>
        /// Executing function for Transfer Confirmation uploads. This will call the appropriate
        /// methods to determine what Transfer Confirmations are in A1W's upload staging table 
        /// awaiting import to NAV, select and move them to the NAV's upload staging
        /// table and call NAV's processing methods (via calls to various classes/methods).
        /// </summary>
        [ExecutingFunction]
        public void TransferConfirmationUpload()
        {
            try
            {
                EventNotificationClient.Instance.PostMessage("NAV.Upload.TransferConfirmation", CompanyConfig.InstanceId,
                    "Upload started");

                this.isInitialized = true;
                CompanyConfig.ResetConfiguration();

                if (!this.isInitialized)
                {
                    throw new Exception(string.Format("Cannot execute {0}, environment not initialized", MethodBase.GetCurrentMethod().Name));
                }

                PreUpload("XC");
                
                #region Retrieve list of all XC that are pending for upload
                        using (DataTable records = CompanyConfig.Instance.GetWarehouseDBConnection().ExecuteDataTableQuery(@"
                           select Top(100) * from upload where field001 = 'XC' and uploaded = '{0}' and field014 = '{1}' and field009 <> 'UPLOADED' order by field010 asc , field011 asc ",
                                   (int)UploadTypes.PendingUpload, NAVAdapterUtils.getClientName()))  
                        {
                            try
                            {
                                bool fullOrderSuccess = true;

                                if (records.Rows.Count == 0)
                                {
                                    EventNotificationClient.Instance.PostMessage("NAV.Upload.TransferConfirmation", CompanyConfig.InstanceId,
                                        "No Statuses to Upload");
                                    return;
                                }

                                EventNotificationClient.Instance.PostMessage("NAV.Upload.TransferConfirmation", CompanyConfig.InstanceId,
                                    "WMS data read", new Variable("data", ControllerUtils.DataTable2LogString(records)));

                                //Parse WMS records and produce Business object
                                List<TransferConfirmation> transferConfirmations = WMSAdapter.ParseTransferConfirmation(records);

                                EventNotificationClient.Instance.PostMessage("NAV.Upload.TransferConfirmation", CompanyConfig.InstanceId,
                                    "WMS -> Business object", new Variable("data", Utils.SerializeToString(transferConfirmations)));

                                switch (CompanyConfig.Instance.UploadTransportMethod)
                                {
                                    case TransportMethod.SQL:
                                        fullOrderSuccess = XCUploadSQL(transferConfirmations);
                                        break;
                                    case TransportMethod.WebService:
                                        fullOrderSuccess = XCUploadWS(transferConfirmations);
                                        break;
                                }

                                if (fullOrderSuccess)
                                {
                                    #region Mark Successful
                                    foreach (DataRow rec in records.Rows) // set XC records to success
                                    {
                                        UpdateUpload(true, rec["rowid"].ToString().Trim());
                                    }
                                }
                                    #endregion
                                //}
                                //else
                                //{
                                //    #region Mark as Error

                                //    foreach (DataRow rec in records.Rows)
                                //    {
                                //        UpdateUpload(false, rec["rowid"].ToString().Trim());

                                //        EventNotificationClient.Instance.PostMessage("NAV.Upload.TransferConfirmation.Error", CompanyConfig.InstanceId,
                                //            string.Format("Error uploading Status {0} due to line level errors. Please see suspended uploads report for details.", rec["field003"]));
                                //    }
                                //    #endregion
                                //}
                            }
                            catch (Exception ex)
                            {
                                #region Mark as error
                                foreach (DataRow rec in records.Rows)
                                {
                                    UpdateUpload(rec["rowid"].ToString().Trim(), ex.Message);

                                    EventNotificationClient.Instance.PostMessage("NAV.Upload.TransferConfirmation.Error", CompanyConfig.InstanceId,
                                        string.Format(string.Format("Error uploading Status {0}", rec["field003"])),
                                        new Variable("message", ex.Message),
                                        new Variable("full exception", ex.ToString()));
                                }

                                #endregion
                            }
                        }
                        #endregion
                                   
              
                CompanyConfig.Instance.NavWebServiceInstance.UploadStaging("XC");
                CompanyConfig.Instance.NavWebServiceInstance.Dispose();
            }
            catch (Exception ex)
            {
                #region Generic error handling
                EventNotificationClient.Instance.PostMessage("NAV.Upload.TransferConfirmation.Error", CompanyConfig.InstanceId,
                    ex.Message,
                    new Variable("message", ex.Message),
                    new Variable("full exception", ex.ToString()));
                #endregion
            }
        }

        private bool XCUploadSQL(List<TransferConfirmation> transferConfirmations)
        {
            bool retVal = true;
            StringBuilder sbSQL;
            object[] parms;
            string strOutputParm = "";
            object oRetVal;

            sbSQL = new StringBuilder();
            sbSQL.Append("exec [");
            sbSQL.Append(CompanyConfig.Instance.CompanyId.ToString());
            sbSQL.Append("$SVA_UploadMT] {0}, {1}");

            parms = new object[2];
            parms[0] = Utils.SerializeToString(transferConfirmations);
            parms[1] = strOutputParm;

            EventNotificationClient.Instance.PostMessage("NAV.Upload.TransferConfirmation.SQL", CompanyConfig.InstanceId, "Business object -> NAV SQL", new Variable("XML Data", parms[0]));
            oRetVal = CompanyConfig.Instance.GetNAVDBConnection().ExecuteScalar(sbSQL.ToString(), parms);
            if ((oRetVal != null) && (!(String.IsNullOrEmpty(oRetVal.ToString().Trim()))))
            {
                throw new Exception(oRetVal.ToString());
            }

            return retVal;
        }

        private static bool XCUploadWS(List<TransferConfirmation> transferConfirmations)
        {
            bool retVal = true;

            //Pass to Business logic adapter
            EventNotificationClient.Instance.PostMessage("NAV.Upload.TransferConfirmation", CompanyConfig.InstanceId,
                "Business object -> NAV");

            List<UploadConfirmation> uploadResults = XCNAVAdapter.WriteToNAV(transferConfirmations);

            #region Check if Any Lines Failed
            foreach (UploadConfirmation confirmation in uploadResults)
            {
                if (confirmation.Ignore)
                    continue;

                if (!confirmation.IsSuccess)
                {
                    #region Mark as Error
                    retVal = false;

                    UpdateUpload(confirmation.RowId, confirmation.ErrorMessage);

                    if (!string.IsNullOrEmpty(confirmation.ErrorMessage))
                        EventNotificationClient.Instance.PostMessage("NAV.Upload.TransferConfirmation.Error", CompanyConfig.InstanceId,
                            string.Format("Line failed: [{0}]", confirmation.ErrorMessage),
                            new Variable("RowId", confirmation.RowId),
                            new Variable("Error", confirmation.ErrorMessage));
                    #endregion
                }
            }
            #endregion

            EventNotificationClient.Instance.PostMessage("NAV.Upload.TransferConfirmation", CompanyConfig.InstanceId,
                string.Format("{0} status lines read", uploadResults.Count));

            return retVal;

        }
        #endregion

        #region SVA StockCountUpload
        /// <summary>
        /// Executing function for Stock Count uploads. This will call the appropriate
        /// methods to determine what Stock Counts are in A1W's upload staging table 
        /// awaiting import to NAV, select and move them to the NAV's upload staging
        /// table and call NAV's processing methods (via calls to various classes/methods).
        /// </summary>
        [ExecutingFunction]
        public void StockCountUpload()
        {
            try
            {
                EventNotificationClient.Instance.PostMessage("NAV.Upload.StockCount", CompanyConfig.InstanceId, "Uploaded started");

                this.isInitialized = true;
                CompanyConfig.ResetConfiguration();

                if (!this.isInitialized)
                {
                    throw new Exception(string.Format("Cannot execute {0}, environment not initialized", MethodBase.GetCurrentMethod().Name));
                }

                #region Upload SC Records
                using (DataTable pendingUploadRecords = CompanyConfig.Instance.GetWarehouseDBConnection().ExecuteDataTableQuery(@"
select distinct 
    rowid
from
	upload
where
	uploaded = '{0}' and 
	field001 = 'SC' and
    field024 = '{1}'", (int)UploadTypes.ReadyForUpload, NAVAdapterUtils.getClientName()))
                {
                    if (pendingUploadRecords.Rows.Count == 0)
                    {
                        EventNotificationClient.Instance.PostMessage("NAV.Upload.StockCount", CompanyConfig.InstanceId, "No Stock Count to Upload");
                        return;
                    }

                    foreach (DataRow pendingUploadRecord in pendingUploadRecords.Rows)
                    {
                        using (DataTable records = CompanyConfig.Instance.GetWarehouseDBConnection().ExecuteDataTableQuery(@"
select
    *
from
    upload
where
    rowid='{0}'", pendingUploadRecord["rowid"].ToString()))
                        {

                            try
                            {
                                EventNotificationClient.Instance.PostMessage("NAV.Upload.StockCount", CompanyConfig.InstanceId, "WMS data read",
                                    new Variable("data", ControllerUtils.DataTable2LogString(records)));

                                if (records.Rows.Count == 0)
                                {
                                    //EventNotificationClient.Instance.PostMessage("NAV.Upload.StockCount", CompanyConfig.InstanceId, "No Stock Count to Upload");
                                    continue;
                                }

                                //Parse WMS records and produce Business object
                                List<StockCount> stockCounts = WMSAdapter.ParseStockCounts(records);

                                EventNotificationClient.Instance.PostMessage("NAV.Upload.StockCount", CompanyConfig.InstanceId, "WMS -> Business object",
                                    new Variable("data", Utils.SerializeToString(stockCounts)));

                                //Pass Business object to NAV adapter
                                List<UploadConfirmation> uploadResults = StockCountNAVAdapter.WriteToNAV(stockCounts);

                                EventNotificationClient.Instance.PostMessage("NAV.Upload.StockCount", CompanyConfig.InstanceId, "Business object -> NAV",
                                    new Variable("Upload Results Count", uploadResults.Count));

                                #region Check if any lines failed
                                foreach (UploadConfirmation confirmation in uploadResults)
                                {
                                    //EventNotificationClient.Instance.PostMessage("NAV.Upload.StockCount", CompanyConfig.InstanceId, "Check for Failures",
                                    //    new Variable("Is Success?", confirmation.IsSuccess), new Variable("Row ID", confirmation.RowId));

                                    if (confirmation.Ignore)
                                    {
                                        continue;
                                    }

                                    if (confirmation.IsSuccess)
                                    {
                                        UpdateUpload(true, confirmation.RowId);
                                    }
                                    else
                                    {
                                        #region Mark as Error
                                        UpdateUpload(confirmation.RowId, confirmation.ErrorMessage);

                                        EventNotificationClient.Instance.PostMessage("NAV.Upload.StockCount.Error", CompanyConfig.InstanceId,
                                            string.Format("Stock count with RowId [{0}] failed", confirmation.RowId),
                                            new Variable("RowId", confirmation.RowId), new Variable("Error", confirmation.ErrorMessage));
                                        #endregion
                                    }
                                }
                                #endregion

                                EventNotificationClient.Instance.PostMessage("NAV.Upload.StockCount", CompanyConfig.InstanceId,
                                    string.Format("{0} stock count records processed", stockCounts.Count));
                            }
                            catch (Exception ex)
                            {
                                #region Mark as Error
                                foreach (DataRow rec in records.Rows)
                                {
                                    UpdateUpload(rec["rowid"].ToString().Trim(), ex.Message);
                                }

                                EventNotificationClient.Instance.PostMessage("NAV.Upload.StockCount.Error", CompanyConfig.InstanceId,
                                    string.Format("Error uploading stock count records"), new Variable("message", ex.Message),
                                    new Variable("full exception", ex.ToString()));
                                #endregion
                            }
                        }
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                EventNotificationClient.Instance.PostMessage("NAV.Upload.StockCount.Error", CompanyConfig.InstanceId, ex);
            }

            CompanyConfig.Instance.NavWebServiceInstance.UploadStaging("SC");
        }
        #endregion
    }

	public enum UploadTypes
	{
		ReadyForUpload = 0,
		PendingUpload = 9,
		Uploaded = 1,
		Error = 2
	}
}