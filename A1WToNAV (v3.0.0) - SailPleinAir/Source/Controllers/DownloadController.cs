using System;
using System.Collections.Generic;
using System.Data;
using Accellos.InterfaceFramework;
using Accellos.InterfaceFramework.EventNotificationServer;
using Accellos.InterfaceFramework.ScriptManagementServer;
using Accellos.Interfaces.NAV.BusinessObjects;
using Accellos.Interfaces.NAV.NAVAdapters;
using Accellos.Interfaces.NAV.WMSAdapters;

namespace Accellos.Interfaces.NAV.Controllers
{
	[ExecutingClass]
	public class DownloadController
	{
		#region Constructor
		public DownloadController()
		{
			CompanyConfig.ResetConfiguration();
		}
		#endregion

        //  SVA Code Units
        #region SVA InventoryItemDownload
        /// <summary>
        /// Executing function for Inventory Item downloads. This will call the appropriate
        /// methods to determine what Item Masters are in NAV's download staging table 
        /// awaiting export to A1W, export those and move them to the A1W's download staging
        /// table (via calls to various classes/methods).
        /// </summary>
        [ExecutingFunction]
        public static void InventoryItemDownload()
        {

            try
            {
                EventNotificationClient.Instance.PostMessage("NAV.Download.InventoryItems", CompanyConfig.InstanceId, "Download started");

                List<InventoryItem> items = InventoryNAVAdapter.RetrieveFromNAV();

                if (items.Count == 0)
                {
                    EventNotificationClient.Instance.PostMessage("NAV.Download.InventoryItems", CompanyConfig.InstanceId, "No Items to Download");
                    return;
                }

                foreach (InventoryItem item in items)
                {
                    try
                    {
                        EventNotificationClient.Instance.PostMessage("NAV.Download.InventoryItems", CompanyConfig.InstanceId, "NAV -> Business object",
                            new Variable("productCode", item.ProductCode), new Variable("data", Utils.SerializeToString(item)));

                        DataTable downloadRecord = WMSAdapter.ParseInventoryItem(item);

                        EventNotificationClient.Instance.PostMessage("NAV.Download.InventoryItems", CompanyConfig.InstanceId,
                            "Business object -> WMS DataTable", new Variable("productCode", item.ProductCode),
                            new Variable("data", ControllerUtils.DataTable2LogString(downloadRecord)));

                        String postResult = CompanyConfig.Instance.PostDownloadRecords(downloadRecord);

                        if (!string.IsNullOrEmpty(postResult))
                            EventNotificationClient.Instance.PostMessage("NAV.Download.InventoryItems.Warning", CompanyConfig.InstanceId,
                                "Data being truncated", new Variable("message", postResult));
                        else
                        {
                            EventNotificationClient.Instance.PostMessage("NAV.Download.InventoryItems", CompanyConfig.InstanceId, "WMS -> Status Update -> NAV", new Variable("Status", "Confirmed by WMS"), new Variable("Product Code", item.ProductCode), new Variable("Entry No", item.SVAStagingID.ToString()));
                            // Call NAV Web Service | ConfirmStatus
                            CompanyConfig.Instance.NavWebServiceInstance.ConfirmStatus(item.SVAStagingID.ToString(), "VA");
                            CompanyConfig.Instance.NavWebServiceInstance.Dispose();
                        }
                    }
                    catch (Exception ex)
                    {
                        EventNotificationClient.Instance.PostMessage("NAV.Download.InventoryItems.Error", CompanyConfig.InstanceId, ex);
                    }
                }
                EventNotificationClient.Instance.PostMessage("NAV.Download.InventoryItems", CompanyConfig.InstanceId, "Download finished");
            }
            catch (Exception ex)
            {
                EventNotificationClient.Instance.PostMessage("NAV.Download.InventoryItems.Error", CompanyConfig.InstanceId, ex);
            }
        }
        #endregion

        #region SVA SalesOrderDownload
        /// <summary>
        /// Executing function for Sales Order downloads. This will call the appropriate
        /// methods to determine what Sales Orders are in NAV's download staging table 
        /// awaiting export to A1W, export those and move them to the A1W's download staging
        /// table (via calls to various classes/methods).
        /// </summary>
        [ExecutingFunction]
        public static void SalesOrderDownload()
        {
            SalesOrderRetrieval();   //want to ensure that we don't delete the orders we download

            EventNotificationClient.Instance.PostMessage("NAV.Download.SalesOrders", CompanyConfig.InstanceId,
                "Download started");

            try
            {
                //List<Packslip> packslips = SalesNAVAdapter.RetrieveFromNAV();
                List<Packslip> packslips = SalesNAVAdapter.RetrieveFromNAV();

                if (packslips.Count == 0)
                {
                    EventNotificationClient.Instance.PostMessage("NAV.Download.SalesOrders", CompanyConfig.InstanceId,
                        "No Orders to Download");
                    return;
                }

                foreach (Packslip packslip in packslips)
                {
                    try
                    {
						//2019/07/24  Createch++ - Check that order doesn't exit to Ensure retrieval process is complete before downloading again
						if (!String.IsNullOrEmpty(WMSAdapterUtils.FetchSalesOrderNumber(packslip.OrderNumber, packslip.ClientName)))
						{
							EventNotificationClient.Instance.PostMessage("NAV.Download.SalesOrders.Error", CompanyConfig.InstanceId, 
                            	"Order still exists in WMS - Retrieval process not complete");
							continue;
						}	
						
                        EventNotificationClient.Instance.PostMessage("NAV.Download.SalesOrders", CompanyConfig.InstanceId, "NAV -> Business object",
                            new Variable("orderNumber", packslip.OrderNumber),
                            new Variable("data", Utils.SerializeToString(packslip)));

                        DataTable downloadRecord = WMSAdapter.ParsePackslip(packslip);

                        EventNotificationClient.Instance.PostMessage("NAV.Download.SalesOrders", CompanyConfig.InstanceId,
                            "Business object -> WMS DataTable", new Variable("orderNumber", packslip.OrderNumber),
                            new Variable("data", ControllerUtils.DataTable2LogString(downloadRecord)));

                        String postResult = CompanyConfig.Instance.PostDownloadRecords(downloadRecord);

                        if (!string.IsNullOrEmpty(postResult))
                        {
                            EventNotificationClient.Instance.PostMessage("NAV.Download.SalesOrders.Warning", CompanyConfig.InstanceId,
                                "Data being truncated",
                                new Variable("message", postResult));
                        }
						//Createch++ Start 2019/07/17 Move sent confirmation after posting is done in DNLOAD table
						try
						{
							CompanyConfig.Instance.NavWebServiceInstance.SentStatus(NAVAdapterUtils.NAVFormatString(packslip.SVAStagingID.ToString(), 40), "PH");
							CompanyConfig.Instance.NavWebServiceInstance.Dispose();
						}
						catch (Exception WSexception)
						{
							EventNotificationClient.Instance.PostMessage("NAV.Download.SalesOrders.Error", CompanyConfig.InstanceId, WSexception);
						}
						//Createch++ End 2019/07/17 Move sent confirmation after posting is done in DNLOAD table
                    }
                    catch (Exception ex)
                    {					
                        EventNotificationClient.Instance.PostMessage("NAV.Download.SalesOrders.Error", CompanyConfig.InstanceId, ex);
                    }
                }

                EventNotificationClient.Instance.PostMessage("NAV.Download.SalesOrders", CompanyConfig.InstanceId,
                    "Download finished");
            }
            catch (Exception ex)
            {
                EventNotificationClient.Instance.PostMessage("NAV.Download.SalesOrders.Error", CompanyConfig.InstanceId, ex);
            }
        }
        #endregion

        #region SVA PurchaseOrderDownload
        /// <summary>
        /// Executing function for Purchase Order downloads. This will call the appropriate
        /// methods to determine what Purchase Orders are in NAV's download staging table 
        /// awaiting export to A1W, export those and move them to the A1W's download staging
        /// table (via calls to various classes/methods).
        /// </summary>
        [ExecutingFunction]
        public static void PurchaseOrderDownload()
        {
            PurchaseOrderRetrieval();   //want to ensure that we don't delete the orders we download

            EventNotificationClient.Instance.PostMessage("NAV.Download.PurchaseOrders", CompanyConfig.InstanceId, 
                "Download started");

            try
            {
                List<PurchaseOrder> purchaseOrders = PurchaseNAVAdaptor.RetrieveFromNAV();
                     
                if (purchaseOrders.Count == 0)
                {
                    EventNotificationClient.Instance.PostMessage("NAV.Download.PurchaseOrders", CompanyConfig.InstanceId, 
                        "No Orders to Download");
                    return;
                }

                foreach (PurchaseOrder purchaseOrder in purchaseOrders)
                {
                    try
                    {
						//2019/07/24  Createch++ - Check that order doesn't exit to Ensure retrieval process is complete before downloading again
						//if (!String.IsNullOrEmpty(WMSAdapterUtils.FetchPurchaseOrderNumber(purchaseOrder.PurchaseOrderNumber, purchaseOrder.ClientName)))
						//{
						//	EventNotificationClient.Instance.PostMessage("NAV.Download.PurchaseOrders.Error", CompanyConfig.InstanceId, 
                        //   	"Order still exists in WMS - Retrieval process not complete");
						//	continue;
						//}
						
                        EventNotificationClient.Instance.PostMessage("NAV.Download.PurchaseOrders", CompanyConfig.InstanceId, 
                            "NAV -> Business object",
                            new Variable("purchaseOrderNumber", purchaseOrder.PurchaseOrderNumber),
                            new Variable("data", Utils.SerializeToString(purchaseOrder)));

                        DataTable downloadRecord = WMSAdapter.ParsePurchaseOrder(purchaseOrder);

                        EventNotificationClient.Instance.PostMessage("NAV.Download.PurchaseOrders", CompanyConfig.InstanceId,
                            "Business object -> WMS DataTable", 
                            new Variable("purchaseOrderNumber", purchaseOrder.PurchaseOrderNumber),
                            new Variable("data", ControllerUtils.DataTable2LogString(downloadRecord)));

                        String postResult = CompanyConfig.Instance.PostDownloadRecords(downloadRecord);

                        if (!string.IsNullOrEmpty(postResult))
                        {
                            EventNotificationClient.Instance.PostMessage("NAV.Download.PurchaseOrders.Warning", CompanyConfig.InstanceId,
                                "Data being truncated", new Variable("message", postResult));
                        }
						//Createch++ Start 2019/07/17 Move sent confirmation after posting is done in DNLOAD table
						try
						{
							CompanyConfig.Instance.NavWebServiceInstance.SentStatus(NAVAdapterUtils.NAVFormatString(purchaseOrder.SVAStagingID.ToString(), 40), "RH");
							CompanyConfig.Instance.NavWebServiceInstance.Dispose();
						}
						catch (Exception WSexception)
						{
							EventNotificationClient.Instance.PostMessage("NAV.Download.PurchaseOrders.Error", CompanyConfig.InstanceId, WSexception);
						}
						//Createch++ End 2019/07/17 Move sent confirmation after posting is done in DNLOAD table
                    }
                    catch (Exception ex)
                    {
						EventNotificationClient.Instance.PostMessage("NAV.Download.PurchaseOrders.Error", CompanyConfig.InstanceId, ex);
                    }
                }

                EventNotificationClient.Instance.PostMessage("NAV.Download.PurchaseOrders", CompanyConfig.InstanceId, 
                    "Download finished");
            }
            catch (Exception ex)
            {				
                EventNotificationClient.Instance.PostMessage("NAV.Download.PurchaseOrders.Error", CompanyConfig.InstanceId, ex);
            }
        }

        #endregion

        #region SVA ProductionOrderDownload
        /// <summary>
        /// Executing function for Production Order downloads. This will call the appropriate
        /// methods to determine what Production Orders are in NAV's download staging table 
        /// awaiting export to A1W, export those and move them to the A1W's download staging
        /// table (via calls to various classes/methods).
        /// </summary>
        [ExecutingFunction]
        public static void ProductionOrderDownload()
        {
            ProductionOrderRetrieval();   //want to ensure that we don't delete the orders we download

            try
            {
                EventNotificationClient.Instance.PostMessage("NAV.Download.ProductionOrders", CompanyConfig.InstanceId, "Download started");

                List<Packslip> productionOrders = ProductionNAVAdapter.RetrieveFromNAV();

                if (productionOrders.Count == 0)
                {
                    EventNotificationClient.Instance.PostMessage("NAV.Download.ProductionOrders", CompanyConfig.InstanceId, "No Orders to download");                 
                    return;
                }
                foreach (Packslip productionOrder in productionOrders)
                {
                    try
                    {
						//2019/07/24  Createch++ - Check that order doesn't exit to Ensure retrieval process is complete before downloading again
						if (!String.IsNullOrEmpty(WMSAdapterUtils.FetchWorkOrderNumber(productionOrder.OrderNumber, productionOrder.ClientName)))
						{
							EventNotificationClient.Instance.PostMessage("NAV.Download.ProductionOrders.Error", CompanyConfig.InstanceId, 
                            	"Order still exists in WMS - Retrieval process not complete");
							continue;
						}
						
                        EventNotificationClient.Instance.PostMessage("NAV.Download.ProductionOrders", CompanyConfig.InstanceId, "NAV -> Business object",
                            new Variable("orderNumber", productionOrder.OrderNumber), new Variable("data", Utils.SerializeToString(productionOrder)));

                        DataTable downloadRecord = WMSAdapter.ParsePackslip(productionOrder);

                        EventNotificationClient.Instance.PostMessage("NAV.Download.ProductionOrders", CompanyConfig.InstanceId,
                            "Business object -> WMS DataTable", new Variable("orderNumber", productionOrder.OrderNumber),
                            new Variable("data", ControllerUtils.DataTable2LogString(downloadRecord)));

                        String postResult = CompanyConfig.Instance.PostDownloadRecords(downloadRecord);

                        if (!string.IsNullOrEmpty(postResult))
                        {
                            EventNotificationClient.Instance.PostMessage("NAV.Download.ProductionOrders.Warning", CompanyConfig.InstanceId,
                                "Data being truncated", new Variable("message", postResult));
                        }
						//Createch++ Start 2019/07/17 Put back to ready to sent if error while writing in WMS DNLOAD table
						try
						{
							CompanyConfig.Instance.NavWebServiceInstance.SentStatus(NAVAdapterUtils.NAVFormatString(productionOrder.SVAStagingID.ToString(), 40), "WO");
							CompanyConfig.Instance.NavWebServiceInstance.Dispose();
						}
						catch (Exception WSexception)
						{
							EventNotificationClient.Instance.PostMessage("NAV.Download.ProductionOrders.Error", CompanyConfig.InstanceId, WSexception);
						}
						//Createch++ End 2019/07/17 Put back to ready to sent if error while writing in WMS DNLOAD table
                    }
                    catch (Exception ex)
                    {
                        EventNotificationClient.Instance.PostMessage("NAV.Download.ProductionOrders.Error", CompanyConfig.InstanceId, ex);
                    }
                }

                EventNotificationClient.Instance.PostMessage("NAV.Download.ProductionOrders", CompanyConfig.InstanceId, "Download finished");
            }
            catch (Exception ex)
            {
                EventNotificationClient.Instance.PostMessage("NAV.Download.ProductionOrders.Error", CompanyConfig.InstanceId, ex);
            }
        }

        #endregion

        #region SVA AssemblyOrderDownload
        /// <summary>
        /// Executing function for Assembly Order downloads. This will call the appropriate
        /// methods to determine what Assembly Orders are in NAV's download staging table 
        /// awaiting export to A1W, export those and move them to the A1W's download staging
        /// table (via calls to various classes/methods).
        /// </summary>
        [ExecutingFunction]
        public static void AssemblyOrderDownload()
        {
            AssemblyOrderRetrieval();   //want to ensure that we don't delete the orders we download

            try
            {
                EventNotificationClient.Instance.PostMessage("NAV.Download.AssemblyOrders", CompanyConfig.InstanceId, "Download started");

                List<Packslip> assemblyOrders = AssemblyNAVAdapter.RetrieveFromNAV();

                if (assemblyOrders.Count == 0)
                {
                    EventNotificationClient.Instance.PostMessage("NAV.Download.AssemblyOrders", CompanyConfig.InstanceId, "No Orders to download");
                    return;
                }
                foreach (Packslip assemblyOrder in assemblyOrders)
                {
                    try
                    {
						//2019/07/24  Createch++ - Check that order doesn't exit to Ensure retrieval process is complete before downloading again
						if (!String.IsNullOrEmpty(WMSAdapterUtils.FetchWorkOrderNumber(assemblyOrder.OrderNumber, assemblyOrder.ClientName)))
						{
							EventNotificationClient.Instance.PostMessage("NAV.Download.AssemblyOrders.Error", CompanyConfig.InstanceId, 
                            	"Order still exists in WMS - Retrieval process not complete");
							continue;
						}
						
                        EventNotificationClient.Instance.PostMessage("NAV.Download.AssemblyOrders", CompanyConfig.InstanceId, "NAV -> Business object",
                            new Variable("orderNumber", assemblyOrder.OrderNumber), new Variable("data", Utils.SerializeToString(assemblyOrder)));

                        DataTable downloadRecord = WMSAdapter.ParsePackslip(assemblyOrder);

                        EventNotificationClient.Instance.PostMessage("NAV.Download.AssemblyOrders", CompanyConfig.InstanceId,
                            "Business object -> WMS DataTable", new Variable("orderNumber", assemblyOrder.OrderNumber),
                            new Variable("data", ControllerUtils.DataTable2LogString(downloadRecord)));

                        String postResult = CompanyConfig.Instance.PostDownloadRecords(downloadRecord);

                        if (!string.IsNullOrEmpty(postResult))
                        {
                            EventNotificationClient.Instance.PostMessage("NAV.Download.AssemblyOrders.Warning", CompanyConfig.InstanceId,
                                "Data being truncated", new Variable("message", postResult));
                        }
						//Createch++ Start 2019/07/17 Put back to ready to sent if error while writing in WMS DNLOAD table
						try
						{
							CompanyConfig.Instance.NavWebServiceInstance.SentStatus(NAVAdapterUtils.NAVFormatString(assemblyOrder.SVAStagingID.ToString(), 40), "AO");
							CompanyConfig.Instance.NavWebServiceInstance.Dispose();
						}
						catch (Exception WSexception)
						{
							EventNotificationClient.Instance.PostMessage("NAV.Download.AssemblyOrders.Error", CompanyConfig.InstanceId, WSexception);
						}
						//Createch++ End 2019/07/17 Put back to ready to sent if error while writing in WMS DNLOAD table
                    }
                    catch (Exception ex)
                    {
                        EventNotificationClient.Instance.PostMessage("NAV.Download.AssemblyOrders.Error", CompanyConfig.InstanceId, ex);
                    }
                }

                EventNotificationClient.Instance.PostMessage("NAV.Download.AssemblyOrders", CompanyConfig.InstanceId, "Download finished");
            }
            catch (Exception ex)
            {
                EventNotificationClient.Instance.PostMessage("NAV.Download.AssemblyOrders.Error", CompanyConfig.InstanceId, ex);
            }
        }

        #endregion

        #region SVA TransferOrderDownload
        /// <summary>
        /// Executing function for Transfer Order downloads. This will call the appropriate
        /// methods to determine what Transfer Orders are in NAV's download staging table 
        /// awaiting export to A1W, export those and move them to the A1W's download staging
        /// table (via calls to various classes/methods).
        /// </summary>
        [ExecutingFunction]
        public static void TransferOrderDownload()
        {
            TransferOrderRetrieval();   //want to ensure that we don't delete the orders we download

            EventNotificationClient.Instance.PostMessage("NAV.Download.TransferOrders", CompanyConfig.InstanceId, "Download started");

            #region Pick Headers
            try
            {
                List<Packslip> transfers = TransferOrderNAVAdapter.RetrievePHFromNAV();

                if (transfers.Count == 0)
                {
                    EventNotificationClient.Instance.PostMessage("NAV.Download.TransferOrders", CompanyConfig.InstanceId,
                        "No Packslips to Download");
                }

                foreach (Packslip transfer in transfers)
                {
                    try
                    {
						//2019/07/24  Createch++ - Check that order doesn't exit to Ensure retrieval process is complete before downloading again
						if (!String.IsNullOrEmpty(WMSAdapterUtils.FetchTransferOrderNumberPH(transfer.OrderNumber, transfer.ClientName)))
						{
							EventNotificationClient.Instance.PostMessage("NAV.Download.TransferOrders.Error", CompanyConfig.InstanceId, 
                            	"Order still exists in WMS - Retrieval process not complete");
							continue;
						}
						
                        EventNotificationClient.Instance.PostMessage("NAV.Download.TransferOrders.PickHeaders", CompanyConfig.InstanceId, "NAV -> Business object",
                            new Variable("orderNumber", transfer.OrderNumber), new Variable("data", Utils.SerializeToString(transfer)));

                        DataTable downloadRecord = WMSAdapter.ParsePackslip(transfer);

                        EventNotificationClient.Instance.PostMessage("NAV.Download.TransferOrders.PickHeaders", CompanyConfig.InstanceId,
                            "Business object -> WMS DataTable", new Variable("orderNumber", transfer.OrderNumber),
                            new Variable("data", ControllerUtils.DataTable2LogString(downloadRecord)));

                        String postResult = CompanyConfig.Instance.PostDownloadRecords(downloadRecord);

                        if (!string.IsNullOrEmpty(postResult))
                        {
                            EventNotificationClient.Instance.PostMessage("NAV.Download.TransferOrders.PickHeaders.Warning", CompanyConfig.InstanceId,
                                "Data being truncated", new Variable("message", postResult));
                        }
                    }
                    catch (Exception ex)
                    {
                        EventNotificationClient.Instance.PostMessage("NAV.Download.TransferOrders.PickHeaders.Error", CompanyConfig.InstanceId, ex);
                    }
                }
            }
            catch (Exception ex)
            {
                EventNotificationClient.Instance.PostMessage("NAV.Download.TransferOrders.PickHeaders.Error", CompanyConfig.InstanceId, ex);
            }
            #endregion

            #region Receipt Headers
            try
            {
                List<PurchaseOrder> transfers = TransferOrderNAVAdapter.RetrieveRHFromNAV();

                if (transfers.Count == 0)
                {
                    EventNotificationClient.Instance.PostMessage("NAV.Download.TransferOrders", CompanyConfig.InstanceId,
                        "No Receipt Headers to Download");
                    return;
                }

                foreach (PurchaseOrder transfer in transfers)
                {
                    try
                    {
						//2019/07/24  Createch++ - Check that order doesn't exit to Ensure retrieval process is complete before downloading again
						if (!String.IsNullOrEmpty(WMSAdapterUtils.FetchTransferOrderNumberRH(transfer.PurchaseOrderNumber, transfer.ClientName)))
						{
							EventNotificationClient.Instance.PostMessage("NAV.Download.TransferOrders.Error", CompanyConfig.InstanceId, 
                            	"Order still exists in WMS - Retrieval process not complete");
							continue;
						}
						
                        EventNotificationClient.Instance.PostMessage("NAV.Download.TransferOrders.ReceiptHeaders", CompanyConfig.InstanceId,
                            "NAV -> Business object",
                            new Variable("purchaseOrderNumber", transfer.PurchaseOrderNumber),
                            new Variable("data", Utils.SerializeToString(transfer)));

                        DataTable downloadRecord = WMSAdapter.ParsePurchaseOrder(transfer);

                        EventNotificationClient.Instance.PostMessage("NAV.Download.TransferOrders.ReceiptHeaders", CompanyConfig.InstanceId,
                            "Business object -> WMS DataTable",
                            new Variable("purchaseOrderNumber", transfer.PurchaseOrderNumber),
                            new Variable("data", ControllerUtils.DataTable2LogString(downloadRecord)));

                        String postResult = CompanyConfig.Instance.PostDownloadRecords(downloadRecord);

                        if (!string.IsNullOrEmpty(postResult))
                        {
                            EventNotificationClient.Instance.PostMessage("NAV.Download.TransferOrders.ReceiptHeaders.Warning", CompanyConfig.InstanceId,
                                "Data being truncated",
                                new Variable("message", postResult));
                        }
						
						//Createch++ Start 2019/07/17 Move sent confirmation after posting is done in DNLOAD table
						try
						{
							CompanyConfig.Instance.NavWebServiceInstance.SentStatus(NAVAdapterUtils.NAVFormatString(transfer.SVAStagingID.ToString(), 40), "TO");
							CompanyConfig.Instance.NavWebServiceInstance.Dispose();
						}
						catch (Exception WSexception)
						{
							EventNotificationClient.Instance.PostMessage("NAV.Download.TransferOrders.Error", CompanyConfig.InstanceId, WSexception);
						}
						//Createch++ End 2019/07/17 Move sent confirmation after posting is done in DNLOAD table

                        EventNotificationClient.Instance.PostMessage("NAV.Download.TransferOrders", CompanyConfig.InstanceId,
                            "Download finished");
                    }
                    catch (Exception ex)
                    {
                        EventNotificationClient.Instance.PostMessage("NAV.Download.TransferOrders.ReceiptHeaders.Error", CompanyConfig.InstanceId, ex);
                    }

                    EventNotificationClient.Instance.PostMessage("NAV.Download.TransferOrders", CompanyConfig.InstanceId, "Download finished");
                }
            }
            catch (Exception ex)
            {
                EventNotificationClient.Instance.PostMessage("NAV.Download.TransferOrders.ReceiptHeaders.Error", CompanyConfig.InstanceId, ex);
            }
            #endregion
        }
        #endregion

        #region SVA RmaOrderDownload
        /// <summary>
        /// Executing function for Sales Return Order downloads. This will call the appropriate
        /// methods to determine what Sales Return Orders are in NAV's download staging table 
        /// awaiting export to A1W, export those and move them to the A1W's download staging
        /// table (via calls to various classes/methods).
        /// </summary>
        [ExecutingFunction]
        public static void RmaOrderDownload()
        {
            RMAOrderRetrieval();   //want to ensure that we don't delete the orders we download

            EventNotificationClient.Instance.PostMessage("NAV.Download.RMAOrders", CompanyConfig.InstanceId,
                "Download started");

            try
            {
                List<PurchaseOrder> rmaOrders = RmaNAVAdapter.RetrieveFromNAV();

                if (rmaOrders.Count == 0)
                {
                    EventNotificationClient.Instance.PostMessage("NAV.Download.RMAOrders", CompanyConfig.InstanceId,
                        "No Orders to Download");
                    return;
                }

                foreach (PurchaseOrder rmaOrder in rmaOrders)
                {
                    try
                    {
						//2019/07/24  Createch++ - Check that order doesn't exit to Ensure retrieval process is complete before downloading again
						if (!String.IsNullOrEmpty(WMSAdapterUtils.FetchRMAOrderNumber(rmaOrder.PurchaseOrderNumber, rmaOrder.ClientName)))
						{
							EventNotificationClient.Instance.PostMessage("NAV.Download.RMAOrders.Error", CompanyConfig.InstanceId, 
                            	"Order still exists in WMS - Retrieval process not complete");
							continue;
						}
						
                        EventNotificationClient.Instance.PostMessage("NAV.Download.RMAOrders", CompanyConfig.InstanceId,
                            "NAV -> Business object",
                            new Variable("rmaOrderNumber", rmaOrder.PurchaseOrderNumber),
                            new Variable("data", Utils.SerializeToString(rmaOrder)));

                        DataTable downloadRecord = WMSAdapter.ParsePurchaseOrder(rmaOrder);

                        EventNotificationClient.Instance.PostMessage("NAV.Download.RMAOrders", CompanyConfig.InstanceId,
                            "Business object -> WMS DataTable",
                            new Variable("rmaOrderNumber", rmaOrder.PurchaseOrderNumber),
                            new Variable("data", ControllerUtils.DataTable2LogString(downloadRecord)));

                        String postResult = CompanyConfig.Instance.PostDownloadRecords(downloadRecord);

                        if (!string.IsNullOrEmpty(postResult))
                        {
                            EventNotificationClient.Instance.PostMessage("NAV.Download.RMAOrders.Warning", CompanyConfig.InstanceId,
                                "Data being truncated", new Variable("message", postResult));
                        }
						//Createch++ Start 2019/07/17 Move sent confirmation after posting is done in DNLOAD table
						try
						{
							CompanyConfig.Instance.NavWebServiceInstance.SentStatus(NAVAdapterUtils.NAVFormatString(rmaOrder.SVAStagingID.ToString(), 40), "RH");
							CompanyConfig.Instance.NavWebServiceInstance.Dispose();
						}
						catch (Exception WSexception)
						{
							EventNotificationClient.Instance.PostMessage("NAV.Download.RMAOrders.Error", CompanyConfig.InstanceId, WSexception);
						}
						//Createch++ End 2019/07/17 Move sent confirmation after posting is done in DNLOAD table
                    }
                    catch (Exception ex)
                    {
                        EventNotificationClient.Instance.PostMessage("NAV.Download.RMAOrders.Error", CompanyConfig.InstanceId, ex);
                    }
                }

                EventNotificationClient.Instance.PostMessage("NAV.Download.RMAOrders", CompanyConfig.InstanceId,
                    "Download finished");
            }
            catch (Exception ex)
            {
                EventNotificationClient.Instance.PostMessage("NAV.Download.RMAOrders.Error", CompanyConfig.InstanceId, ex);
            }
        }

        #endregion

        #region SVA PurchaseReturnOrderDownload
        /// <summary>
        /// Executing function for Purchase Return Order downloads. This will call the appropriate
        /// methods to determine what Purchase Return Orders are in NAV's download staging table 
        /// awaiting export to A1W, export those and move them to the A1W's download staging
        /// table (via calls to various classes/methods).
        /// </summary>
        [ExecutingFunction]
        public static void PurchaseReturnOrderDownload()
        {
            PurchaseReturnOrderRetrieval();   //want to ensure that we don't delete the orders we download

            EventNotificationClient.Instance.PostMessage("NAV.Download.PurchaseReturnOrders", CompanyConfig.InstanceId,
                "Download started");

            try
            {
                List<Packslip> purchaseReturns = PurchaseReturnNAVAdapter.RetrieveFromNAV();

                if (purchaseReturns.Count == 0)
                {
                    EventNotificationClient.Instance.PostMessage("NAV.Download.PurchaseReturnOrders", CompanyConfig.InstanceId,
                        "No Orders to Download");
                    return;
                }

                foreach (Packslip purchaseReturn in purchaseReturns)
                {
                    try
                    {
						//2019/07/24  Createch++ - Check that order doesn't exit to Ensure retrieval process is complete before downloading again
						if (!String.IsNullOrEmpty(WMSAdapterUtils.FetchPurchaseReturnOrderNumber(purchaseReturn.OrderNumber, purchaseReturn.ClientName)))
						{
							EventNotificationClient.Instance.PostMessage("NAV.Download.PurchaseReturnOrders.Error", CompanyConfig.InstanceId, 
                            	"Order still exists in WMS - Retrieval process not complete");
							continue;
						}
						
                        EventNotificationClient.Instance.PostMessage("NAV.Download.PurchaseReturnOrders", CompanyConfig.InstanceId, "NAV -> Business object",
                            new Variable("orderNumber", purchaseReturn.OrderNumber),
                            new Variable("data", Utils.SerializeToString(purchaseReturn)));

                        DataTable downloadRecord = WMSAdapter.ParsePackslip(purchaseReturn);

                        EventNotificationClient.Instance.PostMessage("NAV.Download.PurchaseReturnOrders", CompanyConfig.InstanceId,
                            "Business object -> WMS DataTable", new Variable("orderNumber", purchaseReturn.OrderNumber),
                            new Variable("data", ControllerUtils.DataTable2LogString(downloadRecord)));

                        String postResult = CompanyConfig.Instance.PostDownloadRecords(downloadRecord);

                        if (!string.IsNullOrEmpty(postResult))
                        {
                            EventNotificationClient.Instance.PostMessage("NAV.Download.PurchaseReturnOrders.Warning", CompanyConfig.InstanceId,
                                "Data being truncated",
                                new Variable("message", postResult));
                        }
						//Createch++ Start 2019/07/17 Move sent confirmation after posting is done in DNLOAD table
						try
						{
							CompanyConfig.Instance.NavWebServiceInstance.SentStatus(NAVAdapterUtils.NAVFormatString(purchaseReturn.SVAStagingID.ToString(), 40), "PH");
							CompanyConfig.Instance.NavWebServiceInstance.Dispose();
						}
						catch (Exception WSexception)
						{
							EventNotificationClient.Instance.PostMessage("NAV.Download.PurchaseReturnOrders.Error", CompanyConfig.InstanceId, WSexception);
						}
						//Createch++ End 2019/07/17 Move sent confirmation after posting is done in DNLOAD table
                    }
                    catch (Exception ex)
                    {
                        EventNotificationClient.Instance.PostMessage("NAV.Download.PurchaseReturnOrders.Error", CompanyConfig.InstanceId, ex);
                    }
                }

                EventNotificationClient.Instance.PostMessage("NAV.Download.PurchaseReturnOrders", CompanyConfig.InstanceId,
                    "Download finished");
            }
            catch (Exception ex)
            {
                EventNotificationClient.Instance.PostMessage("NAV.Download.PurchaseReturnOrders.Error", CompanyConfig.InstanceId, ex);
            }
        }
        #endregion

        #region SVA ComputerAdjustmentDownload
        /// <summary>
        /// This function will (through supporting classes/methods) retrieve
        /// the data about NAV's bin contents to populate A1W's quantities
        /// appropriatly. This can be done via a file or via NAV.
        /// 
        /// Note this is not an executing function because one wouldn't want 
        /// to run this on a schedule, rather only on request. 
        /// </summary>
        /// <param name="dataSource">
        /// This specifies the means to which the data is to be collected. 
        /// For exmaple the data could be pulled via a XML port or a file.
        /// </param>
       
        public static void ComputerAdjustmentDownloadDataSource(DataSourceType dataSource)
        {
            EventNotificationClient.Instance.PostMessage("NAV.Download.ComputerAdjustments", CompanyConfig.InstanceId,
                "Download started");

            try
            {
                List<AdjustmentsCA> computerAdjustments;
                switch (dataSource)
                {
                    case DataSourceType.File:
                        computerAdjustments = ComputerAdjustmentNAVAdapter.RetrieveFromFile();
                        break;
                    case DataSourceType.NAVWebService:
                        computerAdjustments = ComputerAdjustmentNAVAdapter.RetrieveFromNAV();
                        break;
                    default:
                        throw new NotImplementedException("Data source not provided!");
                }


                if (computerAdjustments.Count == 0)
                {
                    EventNotificationClient.Instance.PostMessage("NAV.Download.ComputerAdjustments", CompanyConfig.InstanceId,
                        "Nothing to Download");
                    return;
                }

                foreach (AdjustmentsCA computerAdjustment in computerAdjustments)
                {
                    try
                    {
                        EventNotificationClient.Instance.PostMessage("NAV.Download.ComputerAdjustments", CompanyConfig.InstanceId,
                            "NAV -> Business object",
                            new Variable("Product Code", computerAdjustment.ProductCode),
                            new Variable("data", Utils.SerializeToString(computerAdjustment)));

                        DataTable downloadRecord = WMSAdapter.ParseAdjustmentCA(computerAdjustment);

                        EventNotificationClient.Instance.PostMessage("NAV.Download.ComputerAdjustments", CompanyConfig.InstanceId,
                            "Business object -> WMS DataTable",
                            new Variable("Product Code", computerAdjustment.ProductCode),
                            new Variable("data", ControllerUtils.DataTable2LogString(downloadRecord)));

                        String postResult = CompanyConfig.Instance.PostDownloadRecords(downloadRecord);

                        if (!string.IsNullOrEmpty(postResult))
                        {
                            EventNotificationClient.Instance.PostMessage("NAV.Download.ComputerAdjustments.Warning", CompanyConfig.InstanceId,
                                "Data being truncated", new Variable("message", postResult));
                        }
                    }
                    catch (Exception ex)
                    {
                        EventNotificationClient.Instance.PostMessage("NAV.Download.ComputerAdjustments.Error", CompanyConfig.InstanceId, ex);
                    }
                }

                EventNotificationClient.Instance.PostMessage("NAV.Download.ComputerAdjustments", CompanyConfig.InstanceId,
                    "Download finished");
            }
            catch (Exception ex)
            {
                EventNotificationClient.Instance.PostMessage("NAV.Download.ComputerAdjustments.Error", CompanyConfig.InstanceId, ex);
            }
        }


        /// <summary>
        /// This function will (through supporting classes/methods) retrieve
        /// the data about NAV's bin contents to populate A1W's quantities
        /// appropriatly.
        /// </summary>

        [ExecutingFunction]
        public static void ComputerAdjustmentDownload()
        {
            EventNotificationClient.Instance.PostMessage("NAV.Download.ComputerAdjustments", CompanyConfig.InstanceId,
                "Download started");

            try
            {
                List<AdjustmentsCA> computerAdjustments;
             
                computerAdjustments = ComputerAdjustmentNAVAdapter.RetrieveFromNAV();

                if (computerAdjustments.Count == 0)
                {
                    EventNotificationClient.Instance.PostMessage("NAV.Download.ComputerAdjustments", CompanyConfig.InstanceId,
                        "Nothing to Download");
                    return;
                }

                foreach (AdjustmentsCA computerAdjustment in computerAdjustments)
                {
                    try
                    {
                        EventNotificationClient.Instance.PostMessage("NAV.Download.ComputerAdjustments", CompanyConfig.InstanceId,
                            "NAV -> Business object",
                            new Variable("Product Code", computerAdjustment.ProductCode),
                            new Variable("data", Utils.SerializeToString(computerAdjustment)));

                        DataTable downloadRecord = WMSAdapter.ParseAdjustmentCA(computerAdjustment);

                        EventNotificationClient.Instance.PostMessage("NAV.Download.ComputerAdjustments", CompanyConfig.InstanceId,
                            "Business object -> WMS DataTable",
                            new Variable("Product Code", computerAdjustment.ProductCode),
                            new Variable("data", ControllerUtils.DataTable2LogString(downloadRecord)));

                        String postResult = CompanyConfig.Instance.PostDownloadRecords(downloadRecord);

                        if (!string.IsNullOrEmpty(postResult))
                        {
                            EventNotificationClient.Instance.PostMessage("NAV.Download.ComputerAdjustments.Warning", CompanyConfig.InstanceId,
                                "Data being truncated", new Variable("message", postResult));
                        }
                            else
                        {
                            EventNotificationClient.Instance.PostMessage("NAV.Download.InventoryItems", CompanyConfig.InstanceId, "WMS -> Status Update -> NAV", 
                                new Variable("Status", "Confirmed by WMS"), new Variable("Product Code", computerAdjustment.ProductCode), new Variable("Entry No", computerAdjustment.SVAStagingID.ToString()));
                            // Call NAV Web Service | ConfirmStatus
                            CompanyConfig.Instance.NavWebServiceInstance.ConfirmStatus(computerAdjustment.SVAStagingID.ToString(), "CA");
                            CompanyConfig.Instance.NavWebServiceInstance.Dispose();
                        }
                      }
                    catch (Exception ex)
                    {
                        EventNotificationClient.Instance.PostMessage("NAV.Download.ComputerAdjustments.Error", CompanyConfig.InstanceId, ex);
                    }
                }

                EventNotificationClient.Instance.PostMessage("NAV.Download.ComputerAdjustments", CompanyConfig.InstanceId,
                    "Download finished");
            }
            catch (Exception ex)
            {
                EventNotificationClient.Instance.PostMessage("NAV.Download.ComputerAdjustments.Error", CompanyConfig.InstanceId, ex);
            }
        }

        #endregion

        #region SVA SalesOrderRetrieval
        /// <summary>
        /// Executing function for Sales Order retrievals. This will call the appropriate
        /// methods to determine what Sales Orders are in NAV's download staging table 
        /// awaiting retrieval from A1W, retrieve those and ultimately remove them from A1W
        /// altogether (via calls to various classes/methods).
        /// </summary>
        [ExecutingFunction]
        public static void SalesOrderRetrieval()
        {
            try
            {
                EventNotificationClient.Instance.PostMessage("NAV.Download.SalesOrderRetrieval", CompanyConfig.InstanceId, "Retrieval started");

                List<Packslip> myPackslips = SalesNAVAdapter.RetrievalRequestFromNAV();

                if (myPackslips.Count == 0)
                {
                    EventNotificationClient.Instance.PostMessage("NAV.Download.SalesOrderRetrieval", CompanyConfig.InstanceId, "No Orders to Retrieve");
                }

                foreach (Packslip myPackslip in myPackslips)
                {

                    String clientName = String.Empty;
                    String orderNumber = String.Empty;
                    String stagingId = String.Empty;
                    String is3pl = CompanyConfig.Instance.GetWarehouseDBConnection().ExecuteScalar("select TOP 1 C_VALUE from config nolock where config = '3PL_EDITION'").ToString();

                    if (is3pl == "Y")
                    {
                        clientName = myPackslip.ClientName;
                    }

                    orderNumber = myPackslip.OrderNumber;
                    stagingId = myPackslip.SVAStagingID;

                    if(1==0) //Createch++ 2019/05/30 disable order exist check from retireval script
					if (String.IsNullOrEmpty(WMSAdapterUtils.FetchSalesOrderNumber(orderNumber, clientName)))
                    {
                        try
                        {
                            // Call NAV Web Service | ConfirmStatus
                            CompanyConfig.Instance.NavWebServiceInstance.ConfirmStatus(stagingId, "PH");
                            CompanyConfig.Instance.NavWebServiceInstance.Dispose();
                        }
                        catch (Exception ex1)
                        {
                            EventNotificationClient.Instance.PostMessage("NAV.Download.SalesOrderRetrieval.Error", CompanyConfig.InstanceId, "Error: A1I Entry No -> NAV", new Variable("Error Sending to WMS", ex1));
                        }

                        throw new Exception(string.Format("Cannot delete sales order ID [{0}], order is missing from WMS", orderNumber));
                    }
                    EventNotificationClient.Instance.PostMessage("NAV.Download.SalesOrderRetrieval", CompanyConfig.InstanceId, "NAV -> Business object",
                        new Variable("salesOrderId", myPackslip.OrderNumber), new Variable("data", Utils.SerializeToString(myPackslip)));

                    DataTable downloadRecord = WMSAdapter.ParsePackslip(myPackslip);

                    EventNotificationClient.Instance.PostMessage("NAV.Download.SalesOrderRetrieval", CompanyConfig.InstanceId,
                        "Business object -> WMS DataTable", new Variable("salesOrderId", orderNumber),
                        new Variable("data", ControllerUtils.DataTable2LogString(downloadRecord)));

                    String postResult = CompanyConfig.Instance.PostDownloadRecords(downloadRecord);

                    if (!string.IsNullOrEmpty(postResult))
                        EventNotificationClient.Instance.PostMessage("WMS.Download.SalesOrderRetrieval.Warning", CompanyConfig.InstanceId, "Data being truncated",
                            new Variable("message", postResult));

                    EventNotificationClient.Instance.PostMessage("NAV.Download.SalesOrderRetrieval", CompanyConfig.InstanceId, "Delete finished",
                        new Variable("salesOrderId", orderNumber));

                    try
                    {
                        // Call NAV Web Service | ConfirmStatus
                        CompanyConfig.Instance.NavWebServiceInstance.ConfirmStatus(stagingId, "PH");
                        CompanyConfig.Instance.NavWebServiceInstance.Dispose();
                    }
                    catch (Exception ex1)
                    {
                        EventNotificationClient.Instance.PostMessage("NAV.Download.SalesOrderRetrieval.Error", CompanyConfig.InstanceId, "Error: A1I Entry No -> NAV", new Variable("Error Sending to WMS", ex1));
                    }
                }

            }
            catch (Exception ex)
            {
                EventNotificationClient.Instance.PostMessage("NAV.Download.SalesOrderRetrieval.Error", CompanyConfig.InstanceId, ex);
            }
        }
        #endregion

        #region SVA PurchaseOrderRetrieval
        /// <summary>
        /// Executing function for Purchase Order retrievals. This will call the appropriate
        /// methods to determine what Purchase Orders are in NAV's download staging table 
        /// awaiting retrieval from A1W, retrieve those and ultimately remove them from A1W
        /// altogether (via calls to various classes/methods).
        /// </summary>
        [ExecutingFunction]
        public static void PurchaseOrderRetrieval()
        {
            EventNotificationClient.Instance.PostMessage("NAV.Download.PurchaseOrderRetrieval", CompanyConfig.InstanceId, "Retrieval started");

            try
            {
                List<PurchaseOrder> myPurchaseOrders = PurchaseNAVAdaptor.RetrievalRequestFromNAV();

                if (myPurchaseOrders.Count == 0)
                {
                    EventNotificationClient.Instance.PostMessage("NAV.Download.PurchaseOrderRetrieval", CompanyConfig.InstanceId, "No Orders to Retrieve");
                }

                foreach (PurchaseOrder myPurchaseOrder in myPurchaseOrders)
                {

                    String clientName = String.Empty;
                    String purchaseOrderNumber = String.Empty;
                    String stagingId = String.Empty;
                    String is3pl = CompanyConfig.Instance.GetWarehouseDBConnection().ExecuteScalar("select TOP 1 C_VALUE from config nolock where config = '3PL_EDITION'").ToString();

                    if (is3pl == "Y")
                    {
                        clientName = myPurchaseOrder.ClientName;
                    }

                    purchaseOrderNumber = myPurchaseOrder.PurchaseOrderNumber;
                    stagingId = myPurchaseOrder.SVAStagingID;
                    
					if(1==0) //Createch++ 2019/05/30 disable order exist check from retireval script
                    if (String.IsNullOrEmpty(WMSAdapterUtils.FetchPurchaseOrderNumber(purchaseOrderNumber, clientName)))
                    {
                        try
                        {
                            // Call NAV Web Service | ConfirmStatus
                            CompanyConfig.Instance.NavWebServiceInstance.ConfirmStatus(stagingId, "RH");
                            CompanyConfig.Instance.NavWebServiceInstance.Dispose();
                        }
                        catch (Exception ex1)
                        {
                            EventNotificationClient.Instance.PostMessage("NAV.Download.PurchaseOrderRetrieval.Error", CompanyConfig.InstanceId, "Error: A1I Entry No -> NAV", new Variable("Error Sending to WMS", ex1));
                        }
                        throw new Exception(string.Format("Cannot delete purchase order ID [{0}], order is missing from WMS", purchaseOrderNumber));
                    }

                    EventNotificationClient.Instance.PostMessage("NAV.Download.PurchaseOrderRetrieval", CompanyConfig.InstanceId, "NAV -> Business object",
                        new Variable("purchaseOrderId", myPurchaseOrder.PurchaseOrderNumber), new Variable("data", Utils.SerializeToString(myPurchaseOrder)));

                    DataTable downloadRecord = WMSAdapter.ParsePurchaseOrder(myPurchaseOrder);

                    EventNotificationClient.Instance.PostMessage("NAV.Download.PurchaseOrderRetrieval", CompanyConfig.InstanceId,
                        "Business object -> WMS DataTable", new Variable("purchaseOrderId", purchaseOrderNumber),
                        new Variable("data", ControllerUtils.DataTable2LogString(downloadRecord)));

                    String postResult = CompanyConfig.Instance.PostDownloadRecords(downloadRecord);

                    if (!string.IsNullOrEmpty(postResult))
                        EventNotificationClient.Instance.PostMessage("WMS.Download.PurchaseOrderRetrieval.Warning", CompanyConfig.InstanceId, "Data being truncated",
                            new Variable("message", postResult));

                    EventNotificationClient.Instance.PostMessage("NAV.Download.PurchaseOrderRetrieval", CompanyConfig.InstanceId, "Delete finished",
                        new Variable("purchaseOrderId", purchaseOrderNumber));

                    try
                    {
                        // Call NAV Web Service | ConfirmStatus
                        CompanyConfig.Instance.NavWebServiceInstance.ConfirmStatus(stagingId, "RH");
                        CompanyConfig.Instance.NavWebServiceInstance.Dispose();
                    }
                    catch (Exception ex1)
                    {
                        EventNotificationClient.Instance.PostMessage("NAV.Download.PurchaseOrderRetrieval.Error", CompanyConfig.InstanceId, "Error: A1I Entry No -> NAV", new Variable("Error Sending to WMS", ex1));
                    }
                }

            }
            catch (Exception ex)
            {
                EventNotificationClient.Instance.PostMessage("NAV.Download.PurchaseOrderRetrieval.Error", CompanyConfig.InstanceId, ex);
            }
        }
        #endregion

        #region SVA TransferOrderRetrieval
        /// <summary>
        /// Executing function for Transfer Order retrievals. This will call the appropriate
        /// methods to determine what Transfer Orders are in NAV's download staging table 
        /// awaiting retrieval from A1W, retrieve those and ultimately remove them from A1W
        /// altogether (via calls to various classes/methods).
        /// </summary>
        [ExecutingFunction]
        public static void TransferOrderRetrieval()
        {
            EventNotificationClient.Instance.PostMessage("NAV.Download.TransferOrderRetrieval", CompanyConfig.InstanceId, "Retrieval started");

            #region Pick Retrieval
            try
            {
                List<Packslip> myPackslips = TransferOrderNAVAdapter.RetrievalRequestFromNAVPH();

                if (myPackslips.Count == 0)
                {
                    EventNotificationClient.Instance.PostMessage("NAV.Download.TransferOrderRetrieval.Packslips", CompanyConfig.InstanceId, "No Orders to Retrieve");
                }

                foreach (Packslip myPackslip in myPackslips)
                {
                    String clientName = String.Empty;
                    String orderNumber = String.Empty;
                    String stagingId = String.Empty;
                    String is3pl = CompanyConfig.Instance.GetWarehouseDBConnection().ExecuteScalar("select TOP 1 C_VALUE from config nolock where config = '3PL_EDITION'").ToString();

                    if (is3pl == "Y")
                    {
                        clientName = myPackslip.ClientName;
                    }

                    orderNumber = myPackslip.OrderNumber;
                    stagingId = myPackslip.SVAStagingID;

					if(1==0) //Createch++ 2019/05/30 disable order exist check from retireval script
                    if (String.IsNullOrEmpty(WMSAdapterUtils.FetchTransferOrderNumberPH(orderNumber, clientName)))
                    {
                        try
                        {
                            // Call NAV Web Service | ConfirmStatus
                            CompanyConfig.Instance.NavWebServiceInstance.ConfirmStatus(stagingId, "PH");
                            CompanyConfig.Instance.NavWebServiceInstance.Dispose();
                        }
                        catch (Exception ex1)
                        {
                            EventNotificationClient.Instance.PostMessage("NAV.Download.TransferOrderRetrieval.Packslips.Error", CompanyConfig.InstanceId, "Error: A1I Entry No -> NAV", new Variable("Error Sending to WMS", ex1));
                        }

                        throw new Exception(string.Format("Cannot delete Packslip for Transfer Order [{0}], order is missing from WMS", orderNumber));
                    }

                    EventNotificationClient.Instance.PostMessage("NAV.Download..TransferOrderRetrieval.Packslips", CompanyConfig.InstanceId, "NAV -> Business object",
                        new Variable("PackslipID", myPackslip.OrderNumber), new Variable("data", Utils.SerializeToString(myPackslip)));

                    DataTable downloadRecord = WMSAdapter.ParsePackslip(myPackslip);

                    EventNotificationClient.Instance.PostMessage("NAV.Download.TransferOrderRetrieval.Packslips", CompanyConfig.InstanceId,
                        "Business object -> WMS DataTable", new Variable("PackslipID", orderNumber),
                        new Variable("data", ControllerUtils.DataTable2LogString(downloadRecord)));

                    String postResult = CompanyConfig.Instance.PostDownloadRecords(downloadRecord);

                    if (!string.IsNullOrEmpty(postResult))
                    {
                        EventNotificationClient.Instance.PostMessage("WMS.Download.TransferOrderRetrieval.Packslips.Warning", CompanyConfig.InstanceId, 
                            "Data being truncated", new Variable("message", postResult));
                    }

                    EventNotificationClient.Instance.PostMessage("NAV.Download.TransferOrderRetrieval.Packslips", CompanyConfig.InstanceId, "Delete finished",
                        new Variable("PackslipID", orderNumber));
                }
            }
            catch (Exception ex)
            {
                EventNotificationClient.Instance.PostMessage("NAV.Download.TransferOrderRetrieval.Packslips.Error", CompanyConfig.InstanceId, ex);
            }
            #endregion

            #region SVA PurchaseOrderRetrieval
            try
            {
                List<PurchaseOrder> myPurchaseOrders = TransferOrderNAVAdapter.RetrievalRequestFromNAVRH();

                if (myPurchaseOrders.Count == 0)
                {
                    EventNotificationClient.Instance.PostMessage("NAV.Download.TransferOrderRetrieval.PurchaseOrders", CompanyConfig.InstanceId, "No Orders to Retrieve");
                }

                foreach (PurchaseOrder myPurchaseOrder in myPurchaseOrders)
                {

                    String clientName = String.Empty;
                    String purchaseOrderNumber = String.Empty;
                    String stagingId = String.Empty;
                    String is3pl = CompanyConfig.Instance.GetWarehouseDBConnection().ExecuteScalar("select TOP 1 C_VALUE from config nolock where config = '3PL_EDITION'").ToString();

                    if (is3pl == "Y")
                    {
                        clientName = myPurchaseOrder.ClientName;
                    }

                    purchaseOrderNumber = myPurchaseOrder.PurchaseOrderNumber;
                    stagingId = myPurchaseOrder.SVAStagingID;
                    
					if(1==0) //Createch++ 2019/05/30 disable order exist check from retireval script
                    if (String.IsNullOrEmpty(WMSAdapterUtils.FetchTransferOrderNumberRH(purchaseOrderNumber, clientName)))
                    {
                        try
                        {
                            // Call NAV Web Service | ConfirmStatus
                            CompanyConfig.Instance.NavWebServiceInstance.ConfirmStatus(stagingId, "TO");
                            CompanyConfig.Instance.NavWebServiceInstance.Dispose();
                        }
                        catch (Exception ex1)
                        {
                            EventNotificationClient.Instance.PostMessage("NAV.Download.TransferOrderRetrieval.Error", CompanyConfig.InstanceId, "Error: A1I Entry No -> NAV", new Variable("Error Sending to WMS", ex1));
                        }

                        throw new Exception(string.Format("Cannot delete Purchase Order for Transfer Order [{0}], order is missing from WMS", purchaseOrderNumber));
                    }

                    EventNotificationClient.Instance.PostMessage("NAV.Download.TransferOrderRetrieval.PurchaseOrders", CompanyConfig.InstanceId, "NAV -> Business object",
                        new Variable("PurchaseOrderID", myPurchaseOrder.PurchaseOrderNumber), new Variable("data", Utils.SerializeToString(myPurchaseOrder)));

                    DataTable downloadRecord = WMSAdapter.ParsePurchaseOrder(myPurchaseOrder);

                    EventNotificationClient.Instance.PostMessage("NAV.Download.TransferOrderRetrieval.PurchaseOrders", CompanyConfig.InstanceId,
                        "Business object -> WMS DataTable", new Variable("PurchaseOrderID", purchaseOrderNumber),
                        new Variable("data", ControllerUtils.DataTable2LogString(downloadRecord)));

                    String postResult = CompanyConfig.Instance.PostDownloadRecords(downloadRecord);

                    if (!string.IsNullOrEmpty(postResult))
                    {
                        EventNotificationClient.Instance.PostMessage("WMS.Download.TransferOrderRetrieval.PurchaseOrders.Warning", CompanyConfig.InstanceId,
                            "Data being truncated", new Variable("message", postResult));
                    }

                    EventNotificationClient.Instance.PostMessage("NAV.Download.TransferOrderRetrieval.PurchaseOrders", CompanyConfig.InstanceId, 
                        "Delete finished", new Variable("PurchaseOrderID", purchaseOrderNumber));

                    try
                    {
                        // Call NAV Web Service | ConfirmStatus
                        CompanyConfig.Instance.NavWebServiceInstance.ConfirmStatus(stagingId, "TO");
                        CompanyConfig.Instance.NavWebServiceInstance.Dispose();
                    }
                    catch (Exception ex1)
                    {
                        EventNotificationClient.Instance.PostMessage("NAV.Download.TransferOrderRetrieval.PurchaseOrders.Error", CompanyConfig.InstanceId,
                            "Error: A1I Entry No -> NAV", new Variable("Error Sending to WMS", ex1));
                    }
                }
            }
            catch (Exception ex)
            {
                EventNotificationClient.Instance.PostMessage("NAV.Download.TransferOrderRetrieval.PurchaseOrders.Error", CompanyConfig.InstanceId, ex);
            }
            #endregion
        }
        #endregion

        #region SVA RMAOrderRetrieval
        /// <summary>
        /// Executing function for Sales Order Return retrievals. This will call the appropriate
        /// methods to determine what Sales Order Returns are in NAV's download staging table 
        /// awaiting retrieval from A1W, retrieve those and ultimately remove them from A1W
        /// altogether (via calls to various classes/methods).
        /// </summary>
        [ExecutingFunction]
        public static void RMAOrderRetrieval()
        {
            EventNotificationClient.Instance.PostMessage("NAV.Download.RMAOrderRetrieval", CompanyConfig.InstanceId, "Retrieval started");

            try
            {
                List<PurchaseOrder> myRMAOrders = RmaNAVAdapter.RetrievalRequestFromNAV();

                if (myRMAOrders.Count == 0)
                {
                    EventNotificationClient.Instance.PostMessage("NAV.Download.RMAOrderRetrieval", CompanyConfig.InstanceId, "No Orders to Retrieve");
                }

                foreach (PurchaseOrder myRMAOrder in myRMAOrders)
                {

                    String clientName = String.Empty;
                    String purchaseOrderNumber = String.Empty;
                    String stagingId = String.Empty;
                    String is3pl = CompanyConfig.Instance.GetWarehouseDBConnection().ExecuteScalar("select TOP 1 C_VALUE from config nolock where config = '3PL_EDITION'").ToString();

                    if (is3pl == "Y")
                    {
                        clientName = myRMAOrder.ClientName;
                    }

                    purchaseOrderNumber = myRMAOrder.PurchaseOrderNumber;
                    stagingId = myRMAOrder.SVAStagingID;

					if(1==0) //Createch++ 2019/05/30 disable order exist check from retireval script
                    if (String.IsNullOrEmpty(WMSAdapterUtils.FetchRMAOrderNumber(purchaseOrderNumber, clientName))) //Createch 2019/07/24 Corrected with function FetchRMAOrderNumber instead of FetchPurchaseOrderNumber
                    {
                        try
                        {
                            // Call NAV Web Service | ConfirmStatus
                            CompanyConfig.Instance.NavWebServiceInstance.ConfirmStatus(stagingId, "RH");
                            CompanyConfig.Instance.NavWebServiceInstance.Dispose();
                        }
                        catch (Exception ex1)
                        {
                            EventNotificationClient.Instance.PostMessage("NAV.Download.RMAOrderRetrieval.Error", CompanyConfig.InstanceId, "Error: A1I Entry No -> NAV", new Variable("Error Sending to WMS", ex1));
                        }
                        throw new Exception(string.Format("Cannot delete RMA order ID [{0}], order is missing from WMS", purchaseOrderNumber));
                    }
                    EventNotificationClient.Instance.PostMessage("NAV.Download.RMAOrderRetrieval", CompanyConfig.InstanceId, "NAV -> Business object",
                        new Variable("RMAOrderId", myRMAOrder.PurchaseOrderNumber), new Variable("data", Utils.SerializeToString(myRMAOrder)));

                    DataTable downloadRecord = WMSAdapter.ParsePurchaseOrder(myRMAOrder);

                    EventNotificationClient.Instance.PostMessage("NAV.Download.RMAOrderRetrieval", CompanyConfig.InstanceId,
                        "Business object -> WMS DataTable", new Variable("RMAOrderId", purchaseOrderNumber),
                        new Variable("data", ControllerUtils.DataTable2LogString(downloadRecord)));

                    String postResult = CompanyConfig.Instance.PostDownloadRecords(downloadRecord);

                    if (!string.IsNullOrEmpty(postResult))
                    {
                        EventNotificationClient.Instance.PostMessage("WMS.Download.RMAOrderRetrieval.Warning", CompanyConfig.InstanceId, "Data being truncated",
                            new Variable("message", postResult));
                    }

                    EventNotificationClient.Instance.PostMessage("NAV.Download.RMAOrderRetrieval", CompanyConfig.InstanceId, "Delete finished",
                        new Variable("RMAOrderId", purchaseOrderNumber));

                    try
                    {
                        // Call NAV Web Service | ConfirmStatus
                        CompanyConfig.Instance.NavWebServiceInstance.ConfirmStatus(stagingId, "RH");
                        CompanyConfig.Instance.NavWebServiceInstance.Dispose();
                    }
                    catch (Exception ex1)
                    {
                        EventNotificationClient.Instance.PostMessage("NAV.Download.RMAOrderRetrieval.Error", CompanyConfig.InstanceId, "Error: A1I Entry No -> NAV", new Variable("Error Sending to WMS", ex1));
                    }
                }

            }
            catch (Exception ex)
            {
                EventNotificationClient.Instance.PostMessage("NAV.Download.RMAOrderRetrieval.Error", CompanyConfig.InstanceId, ex);
            }
        }
        #endregion

        #region SVA PurchaseReturnOrderRetrieval
        /// <summary>
        /// Executing function for Purchase Order Return retrievals. This will call the appropriate
        /// methods to determine what Purchase Order Returns are in NAV's download staging table 
        /// awaiting retrieval from A1W, retrieve those and ultimately remove them from A1W
        /// altogether (via calls to various classes/methods).
        /// </summary>
        [ExecutingFunction]
        public static void PurchaseReturnOrderRetrieval()
        {
            try
            {
                EventNotificationClient.Instance.PostMessage("NAV.Download.PurchaseReturnOrderRetrieval", CompanyConfig.InstanceId, "Retrieval started");

                List<Packslip> myPurchaseReturns = PurchaseReturnNAVAdapter.RetrievalRequestFromNAV();

                if (myPurchaseReturns.Count == 0)
                {
                    EventNotificationClient.Instance.PostMessage("NAV.Download.PurchaseReturnOrderRetrieval", CompanyConfig.InstanceId, "No Orders to Retrieve");
                }

                foreach (Packslip myPurchaseReturn in myPurchaseReturns)
                {
                    String clientName = String.Empty;
                    String orderNumber = String.Empty;
                    String stagingId = String.Empty;
                    String is3pl = CompanyConfig.Instance.GetWarehouseDBConnection().ExecuteScalar("select TOP 1 C_VALUE from config nolock where config = '3PL_EDITION'").ToString();

                    if (is3pl == "Y")
                    {
                        clientName = myPurchaseReturn.ClientName;
                    }

                    orderNumber = myPurchaseReturn.OrderNumber;
                    stagingId = myPurchaseReturn.SVAStagingID;

					if(1==0) //Createch++ 2019/05/30 disable order exist check from retireval script
                    if (String.IsNullOrEmpty(WMSAdapterUtils.FetchPurchaseReturnOrderNumber(orderNumber, clientName)))
                    {
                        try
                        {
                            // Call NAV Web Service | ConfirmStatus
                            CompanyConfig.Instance.NavWebServiceInstance.ConfirmStatus(stagingId, "PH");
                            CompanyConfig.Instance.NavWebServiceInstance.Dispose();
                        }
                        catch (Exception ex1)
                        {
                            EventNotificationClient.Instance.PostMessage("NAV.Download.PurchaseReturnOrderRetrieval.Error", CompanyConfig.InstanceId, "Error: A1I Entry No -> NAV", new Variable("Error Sending to WMS", ex1));
                        }

                        throw new Exception(string.Format("Cannot delete Purchase return order ID [{0}], order is missing from WMS", orderNumber));
                    }

                    EventNotificationClient.Instance.PostMessage("NAV.Download.PurchaseReturnOrderRetrieval", CompanyConfig.InstanceId, "NAV -> Business object",
                        new Variable("salesOrderId", myPurchaseReturn.OrderNumber), new Variable("data", Utils.SerializeToString(myPurchaseReturn)));

                    DataTable downloadRecord = WMSAdapter.ParsePackslip(myPurchaseReturn);

                    EventNotificationClient.Instance.PostMessage("NAV.Download.PurchaseReturnOrderRetrieval", CompanyConfig.InstanceId,
                        "Business object -> WMS DataTable", new Variable("purchaseReturnOrderId", orderNumber),
                        new Variable("data", ControllerUtils.DataTable2LogString(downloadRecord)));

                    String postResult = CompanyConfig.Instance.PostDownloadRecords(downloadRecord);

                    if (!string.IsNullOrEmpty(postResult))
                    {
                        EventNotificationClient.Instance.PostMessage("WMS.Download.PurchaseReturnOrderRetrieval.Warning", CompanyConfig.InstanceId, "Data being truncated",
                            new Variable("message", postResult));
                    }

                    EventNotificationClient.Instance.PostMessage("NAV.Download.PurchaseReturnOrderRetrieval", CompanyConfig.InstanceId, "Delete finished",
                        new Variable("purchaseReturnOrderId", orderNumber));

                    try
                    {
                        // Call NAV Web Service | ConfirmStatus
                        CompanyConfig.Instance.NavWebServiceInstance.ConfirmStatus(stagingId, "PH");
                        CompanyConfig.Instance.NavWebServiceInstance.Dispose();
                    }
                    catch (Exception ex1)
                    {
                        EventNotificationClient.Instance.PostMessage("NAV.Download.PurchaseReturnOrderRetrieval.Error", CompanyConfig.InstanceId, "Error: A1I Entry No -> NAV", new Variable("Error Sending to WMS", ex1));
                    }
                }
            }
            catch (Exception ex)
            {
                EventNotificationClient.Instance.PostMessage("NAV.Download.PurchaseReturnOrderRetrieval.Error", CompanyConfig.InstanceId, ex);
            }
        }
        #endregion

        #region SVA ProductionOrderRetrieval
        /// <summary>
        /// Executing function for Production Order retrievals. This will call the appropriate
        /// methods to determine what Production Orders are in NAV's download staging table 
        /// awaiting retrieval from A1W, retrieve those and ultimately remove them from A1W
        /// altogether (via calls to various classes/methods).
        /// </summary>
        [ExecutingFunction]
        public static void ProductionOrderRetrieval()
        {
            try
            {
                EventNotificationClient.Instance.PostMessage("NAV.Download.ProductionOrderRetrieval", CompanyConfig.InstanceId, "Retrieval started");

                List<Packslip> myWorkOrders = ProductionNAVAdapter.RetrievalRequestFromNAV();

                if (myWorkOrders.Count == 0)
                {
                    EventNotificationClient.Instance.PostMessage("NAV.Download.ProductionOrderRetrieval", CompanyConfig.InstanceId, "No Orders to Retrieve");
                }

                foreach (Packslip myWorkOrder in myWorkOrders)
                {
                    String clientName = String.Empty;
                    String orderNumber = String.Empty;
                    String stagingId = String.Empty;
                    String is3pl = CompanyConfig.Instance.GetWarehouseDBConnection().ExecuteScalar("select TOP 1 C_VALUE from config nolock where config = '3PL_EDITION'").ToString();

                    if (is3pl == "Y")
                    {
                        clientName = myWorkOrder.ClientName;
                    }

                    orderNumber = myWorkOrder.OrderNumber;
                    stagingId = myWorkOrder.SVAStagingID;

					if(1==0) //Createch++ 2019/05/30 disable order exist check from retireval script
                    if (String.IsNullOrEmpty(WMSAdapterUtils.FetchWorkOrderNumber(orderNumber, clientName)))
                    {
                        try
                        {
                            // Call NAV Web Service | ConfirmStatus
                            CompanyConfig.Instance.NavWebServiceInstance.ConfirmStatus(stagingId, "WO");
                            CompanyConfig.Instance.NavWebServiceInstance.Dispose();
                        }
                        catch (Exception ex1)
                        {
                            EventNotificationClient.Instance.PostMessage("NAV.Download.ProductionOrderRetrieval.Error", CompanyConfig.InstanceId, "Error: A1I Entry No -> NAV", new Variable("Error Sending to WMS", ex1));
                        }

                        throw new Exception(string.Format("Cannot delete Work order ID [{0}], order is missing from WMS", orderNumber));
                    }

                    EventNotificationClient.Instance.PostMessage("NAV.Download.ProductionOrderRetrieval", CompanyConfig.InstanceId, "NAV -> Business object",
                        new Variable("WorkOrderId", myWorkOrder.OrderNumber), new Variable("data", Utils.SerializeToString(myWorkOrder)));

                    DataTable downloadRecord = WMSAdapter.ParsePackslip(myWorkOrder);

                    EventNotificationClient.Instance.PostMessage("NAV.Download.ProductionOrderRetrieval", CompanyConfig.InstanceId,
                        "Business object -> WMS DataTable", new Variable("WorkOrderId", orderNumber),
                        new Variable("data", ControllerUtils.DataTable2LogString(downloadRecord)));

                    String postResult = CompanyConfig.Instance.PostDownloadRecords(downloadRecord);

                    if (!string.IsNullOrEmpty(postResult))
                    {
                        EventNotificationClient.Instance.PostMessage("WMS.Download.ProductionOrderRetrieval.Warning", CompanyConfig.InstanceId, "Data being truncated",
                            new Variable("message", postResult));
                    }

                    WMSAdapterUtils.DeleteAssemblyInstructions(orderNumber);

                    EventNotificationClient.Instance.PostMessage("NAV.Download.ProductionOrderRetrieval", CompanyConfig.InstanceId, "Delete finished",
                        new Variable("purchaseReturnOrderId", orderNumber));

                    try
                    {
                        // Call NAV Web Service | ConfirmStatus
                        CompanyConfig.Instance.NavWebServiceInstance.ConfirmStatus(stagingId, "WO");
                        CompanyConfig.Instance.NavWebServiceInstance.Dispose();
                    }
                    catch (Exception ex1)
                    {
                        EventNotificationClient.Instance.PostMessage("NAV.Download.ProductionOrderRetrieval.Error", CompanyConfig.InstanceId, "Error: A1I Entry No -> NAV", new Variable("Error Sending to WMS", ex1));
                    }
                }
            }
            catch (Exception ex)
            {
                EventNotificationClient.Instance.PostMessage("NAV.Download.ProductionOrderRetrieval.Error", CompanyConfig.InstanceId, ex);
            }
        }
        #endregion

        #region SVA AssemblyOrderRetrieval
        /// <summary>
        /// Executing function for Assembly Order retrievals. This will call the appropriate
        /// methods to determine what Assembly Orders are in NAV's download staging table 
        /// awaiting retrieval from A1W, retrieve those and ultimately remove them from A1W
        /// altogether (via calls to various classes/methods).
        /// </summary>
        [ExecutingFunction]
        public static void AssemblyOrderRetrieval()
        {
            try
            {
                EventNotificationClient.Instance.PostMessage("NAV.Download.AssemblyOrderRetrieval", CompanyConfig.InstanceId, "Retrieval started");

                List<Packslip> myWorkOrders = AssemblyNAVAdapter.RetrievalRequestFromNAV();

                if (myWorkOrders.Count == 0)
                {
                    EventNotificationClient.Instance.PostMessage("NAV.Download.AssemblyOrderRetrieval", CompanyConfig.InstanceId, "No Orders to Retrieve");
                }

                foreach (Packslip myWorkOrder in myWorkOrders)
                {
                    String clientName = String.Empty;
                    String orderNumber = String.Empty;
                    String stagingId = String.Empty;
                    String is3pl = CompanyConfig.Instance.GetWarehouseDBConnection().ExecuteScalar("select TOP 1 C_VALUE from config nolock where config = '3PL_EDITION'").ToString();

                    if (is3pl == "Y")
                    {
                        clientName = myWorkOrder.ClientName;
                    }

                    orderNumber = myWorkOrder.OrderNumber;
                    stagingId = myWorkOrder.SVAStagingID;

					if(1==0) //Createch++ 2019/05/30 disable order exist check from retireval script
                    if (String.IsNullOrEmpty(WMSAdapterUtils.FetchWorkOrderNumber(orderNumber, clientName)))
                    {
                        try
                        {
                            // Call NAV Web Service | ConfirmStatus
                            CompanyConfig.Instance.NavWebServiceInstance.ConfirmStatus(stagingId, "WO");
                            CompanyConfig.Instance.NavWebServiceInstance.Dispose();
                        }
                        catch (Exception ex1)
                        {
                            EventNotificationClient.Instance.PostMessage("NAV.Download.AssemblyOrderRetrieval.Error", CompanyConfig.InstanceId, "Error: A1I Entry No -> NAV", new Variable("Error Sending to WMS", ex1));
                        }

                        throw new Exception(string.Format("Cannot delete Work order ID [{0}], order is missing from WMS", orderNumber));
                    }

                    EventNotificationClient.Instance.PostMessage("NAV.Download.AssemblyOrderRetrieval", CompanyConfig.InstanceId, "NAV -> Business object",
                        new Variable("WorkOrderId", myWorkOrder.OrderNumber), new Variable("data", Utils.SerializeToString(myWorkOrder)));

                    DataTable downloadRecord = WMSAdapter.ParsePackslip(myWorkOrder);

                    EventNotificationClient.Instance.PostMessage("NAV.Download.AssemblyOrderRetrieval", CompanyConfig.InstanceId,
                        "Business object -> WMS DataTable", new Variable("WorkOrderId", orderNumber),
                        new Variable("data", ControllerUtils.DataTable2LogString(downloadRecord)));

                    String postResult = CompanyConfig.Instance.PostDownloadRecords(downloadRecord);

                    if (!string.IsNullOrEmpty(postResult))
                    {
                        EventNotificationClient.Instance.PostMessage("WMS.Download.AssemblyOrderRetrieval.Warning", CompanyConfig.InstanceId, "Data being truncated",
                            new Variable("message", postResult));
                    }

                    WMSAdapterUtils.DeleteAssemblyInstructions(orderNumber);

                    EventNotificationClient.Instance.PostMessage("NAV.Download.AssemblyOrderRetrieval", CompanyConfig.InstanceId, "Delete finished",
                        new Variable("purchaseReturnOrderId", orderNumber));

                    try
                    {
                        // Call NAV Web Service | ConfirmStatus
                        CompanyConfig.Instance.NavWebServiceInstance.ConfirmStatus(stagingId, "AO");
                        CompanyConfig.Instance.NavWebServiceInstance.Dispose();
                    }
                    catch (Exception ex1)
                    {
                        EventNotificationClient.Instance.PostMessage("NAV.Download.AssemblyOrderRetrieval.Error", CompanyConfig.InstanceId, "Error: A1I Entry No -> NAV", new Variable("Error Sending to WMS", ex1));
                    }
                }
            }
            catch (Exception ex)
            {
                EventNotificationClient.Instance.PostMessage("NAV.Download.AssemblyOrderRetrieval.Error", CompanyConfig.InstanceId, ex);
            }
        }
        #endregion

        #region PurgeNavDownloadStaging
        [ExecutingFunction]
        public void PurgeNavDownloadStaging()
        {
            try
            {
                EventNotificationClient.Instance.PostMessage("NAV.Download.PurgeNavDownloadStaging", CompanyConfig.InstanceId,
                 "Calling NAV web service to Purge Download Staging", new Variable("Web Service URL", CompanyConfig.Instance.NavWebServiceInstance.Url));

                var recordsPurged = CompanyConfig.Instance.NavWebServiceInstance.PurgeDownloadStaging();
                CompanyConfig.Instance.NavWebServiceInstance.Dispose();

                EventNotificationClient.Instance.PostMessage("NAV.Download.PurgeNavDownloadStaging", CompanyConfig.InstanceId, "Records Purged: " + recordsPurged,
                  new Variable("message", "Records Purged: " + recordsPurged));

            }
            catch (Exception ex)
            {
                #region Generic error handling
                EventNotificationClient.Instance.PostMessage("NAV.Download.PurgeNavDownloadStaging.Error", CompanyConfig.InstanceId, ex.Message,
                    new Variable("message", ex.Message), new Variable("full exception", ex.ToString()));
                #endregion
            }
        }
        #endregion

	}

    public enum DataSourceType
    {
        File,
        NAVWebService
    }
}

