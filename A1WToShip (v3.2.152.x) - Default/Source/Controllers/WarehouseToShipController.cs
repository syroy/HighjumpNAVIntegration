using Accellos.InterfaceFramework.EventNotificationServer;
using Accellos.InterfaceFramework.ScriptManagementServer;
using Accellos.Ship.MT.Common.Aoss;
using Accellos.Ship.MT.Common.DTO;
using Accellos.Warehouse.MT.Common.DTO;
using HighJump.Integrations.ShipUtils.Adapters;
using HighJump.Integrations.WarehouseToShip.Adapters;
using HighJump.Integrations.WarehouseToShip.MappingAdapters;
using HighJump.Integrations.WarehouseUtils.Adapters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading;
using IFUtils = Accellos.InterfaceFramework.Utils;

namespace HighJump.Integrations.WarehouseToShip.Controllers
{
    [ExecutingClass]
	public class WarehouseToShipController
	{
		private static readonly object sendManualShipmentSyncObject = new object();

		#region PreRateOrder (Event Based)
		public static string PreRateOrder(string orderNumber)
		{
			ControllerUtils.PreSync(false);

			try
			{
				EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}", MethodBase.GetCurrentMethod().ReflectedType.Name,
					MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.General,
					string.Format("Beginning pre-rate for order number {0}...", orderNumber));

				IList<TOTMASTR> totMastrs = WarehouseToShipAdapter.ReadFromWarehouse(orderNumber, TotMastrActions.PreRateOrder, UniqueIdentifierType.OrderNumber, false); // Get all TOTMASTR records for order

				if (totMastrs == null || totMastrs.Count == 0)
					throw new Exception(string.Format("No TOTMASTR records could be found for order number {0} that are ready for pre-rate! The pre-rate will not occur.", orderNumber));

				try
				{
					EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}", MethodBase.GetCurrentMethod().ReflectedType.Name,
						MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.Data,
						string.Format("{0} TOTMASTR records retrieved for order number {1}...", totMastrs.Count, orderNumber),
						new Variable("totMastrs", IFUtils.SerializeToString(totMastrs)));

					Shipment shipment = WarehouseToShipAdapter.ConvertToShipDTO(totMastrs[0], WarehouseToShipAdapter.ConvertToShipDTO(totMastrs)); // Build Shipment

					EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}", MethodBase.GetCurrentMethod().ReflectedType.Name,
						MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.Info,
						string.Format("Finished building Shipment object for order number {0}...", orderNumber));

					shipment = ShipShipmentAdapter.UpdateFields(shipment);

					EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}", MethodBase.GetCurrentMethod().ReflectedType.Name,
						MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.Data,
						string.Format("TOTMASTR records and Ship Containers for order number {0} converted to Ship Shipment...", orderNumber),
						new Variable("shipment", IFUtils.SerializeToString(shipment)));

					#region Rate Shopping
					IList<RateShopGroup> rateShopGroups = ShipRateShopGroupAdapter.ReadFromShip(shipment.CarrierServiceCode);

					if (rateShopGroups != null && rateShopGroups.Count > 0)
					{
						EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}", MethodBase.GetCurrentMethod().ReflectedType.Name,
							MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.Data,
							string.Format("Rate shopping for order number {0}...", orderNumber),
							new Variable("shipment", IFUtils.SerializeToString(shipment)), new Variable("rateShopGroup", IFUtils.SerializeToString(rateShopGroups[0])));

						shipment = ShipShipmentAdapter.SetCarrierAsRateShopGroup(shipment, rateShopGroups[0]);
					}
					#endregion

					List<AossRateResponse> rateResponses = ShipShipmentAdapter.PreRateOrder(shipment); // Tell Ship to pre-rate
					shipment = ShipShipmentAdapter.ProcessPreRateResponse(shipment, rateResponses);

					#region Update Warehouse
					EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}", MethodBase.GetCurrentMethod().ReflectedType.Name,
						MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.Data,
						string.Format("Processing pre-rate response for order number {0}...", orderNumber),
						new Variable("shipment", IFUtils.SerializeToString(shipment)), new Variable("rateResponses", IFUtils.SerializeToString(rateResponses)));

					foreach (TOTMASTR totMastr in totMastrs)
					{
						TOTMASTR totMastrToUpdate = new TOTMASTR(totMastr);

						totMastrToUpdate = WarehouseTotMastrAdapter.WriteToWarehouse(ShipToWarehouseAdapter.ConvertToWarehouseDTO(shipment.Containers[0], totMastr, TotMastrActions.PreRateOrder, TotMastrActions.PreRateComplete));

						EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}", MethodBase.GetCurrentMethod().ReflectedType.Name,
							MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.Data,
							string.Format("Ship pre-rate results synchronized to Warehouse carton ASN {0}...", totMastrToUpdate.CARTON_ASN),
							new Variable("totMastrToUpdate", IFUtils.SerializeToString(totMastrToUpdate)));
					}
					#endregion

					EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}", MethodBase.GetCurrentMethod().ReflectedType.Name,
						MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.General,
						string.Format("Pre-rate for order number {0} complete!", orderNumber));

					return string.Empty;
				}
				catch (Exception innerCatchEx)
				{
					EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}.Error", MethodBase.GetCurrentMethod().ReflectedType.Name,
						MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.Error,
						string.Format("Order number {0} failed pre-rate from Warehouse to Ship!", orderNumber), new Variable("innerCatchEx", innerCatchEx));

					foreach (TOTMASTR totMastr in totMastrs) // Update WMS with error information
						WarehouseTotMastrAdapter.WriteToWarehouse(WarehouseTotMastrAdapter.ChangeError(totMastr, TotMastrActions.PreRateOrder, innerCatchEx.Message));

					return innerCatchEx.Message;
				}
			}
			catch (Exception outerCatchEx)
			{
				EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}.Error", MethodBase.GetCurrentMethod().ReflectedType.Name,
					MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.Error,
					string.Format("Order number {0} failed pre-rate from Warehouse to Ship because the TOTMASTR records could not be found!", orderNumber),
					new Variable("outerCatchEx", outerCatchEx));

				return outerCatchEx.Message;
			}
			finally
			{
				ControllerUtils.PostSync(false);
			}
		}
		#endregion

		#region PreRateOrderPolling
		[ExecutingFunction]
		public void PreRateOrderPolling()
		{
			try
			{
				EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}", MethodBase.GetCurrentMethod().ReflectedType.Name,
					MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.General, "Checking Warehouse for orders ready for pre-rate...");

				WarehouseA1ShipRequestAdapter.UpdateWarehouse(WarehouseA1ShipRequestAdapter.A1ShipRequestStatusInProcess,
					WarehouseA1ShipRequestAdapter.A1ShipRequestStatusNew, TotMastrActions.PreRateOrder);

				using (DataTable warehouseA1ShipRequestRecords =
					WarehouseA1ShipRequestAdapter.ReadFromWarehouse(TotMastrActions.PreRateOrder, WarehouseA1ShipRequestAdapter.A1ShipRequestStatusInProcess))
				{
					if (warehouseA1ShipRequestRecords.Rows.Count > 0)
					{
						foreach (DataRow warehouseA1ShipRequestRecord in warehouseA1ShipRequestRecords.Rows)
							PreRateOrder(warehouseA1ShipRequestRecord["uniqueIdentifier"].ToString());

						WarehouseA1ShipRequestAdapter.UpdateWarehouse(warehouseA1ShipRequestRecords, WarehouseA1ShipRequestAdapter.A1ShipRequestStatusProcessed);
					}
				}

				EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}", MethodBase.GetCurrentMethod().ReflectedType.Name,
					MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.General, "Finished pre-rate for Warehouse orders!");
			}
			catch (Exception ex)
			{
				EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}.Error", MethodBase.GetCurrentMethod().ReflectedType.Name,
					MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.Error, "Unable to check Warehouse for orders ready for pre-rate!",
					new Variable("ex", ex));
			}
			finally
			{
				ControllerUtils.PostSync(false);
			}
		}
		#endregion

		#region RateManifestShipment (Event Based)
		public static string RateManifestShipment(string warehouseShipmentNumber)
		{
			ControllerUtils.PreSync(false);

			try
			{
				EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}", MethodBase.GetCurrentMethod().ReflectedType.Name,
					MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.General,
					string.Format("Beginning rate and manifest for Warehouse shipment number {0}...", warehouseShipmentNumber));

				IList<TOTMASTR> totMastrs = WarehouseToShipAdapter.ReadFromWarehouse(
					warehouseShipmentNumber, TotMastrActions.RateManifestShipment, UniqueIdentifierType.ShipmentNumber, false); // Get all TOTMASTR records for shipment

				if (totMastrs == null || totMastrs.Count == 0)
					throw new Exception(string.Format("No TOTMASTR records could be found for Warehouse shipment number {0} that are ready for rate and manifest! The rate and manifest will not occur.",
						warehouseShipmentNumber));

				try
				{
					#region Clean Up Unused Containers
					IList<TOTMASTR> unusedTotMastrs = WarehouseToShipAdapter.ReadUnusedFromWarehouse(warehouseShipmentNumber);

					if (unusedTotMastrs != null)
					{
						foreach (TOTMASTR unusedTotMastr in unusedTotMastrs)
						{
							Container unusedContainer = ShipContainerAdapter.ReadFromShip(unusedTotMastr.CARTON_ASN);

							if (unusedContainer != null)
								ShipContainerAdapter.RemoveContainer(unusedContainer);
						}
					}
					#endregion

					EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}", MethodBase.GetCurrentMethod().ReflectedType.Name,
						MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.Data,
						string.Format("{0} TOTMASTR records retrieved for Warehouse shipment number {1}...", totMastrs.Count, warehouseShipmentNumber),
						new Variable("totMastrs", IFUtils.SerializeToString(totMastrs)));

					Shipment shipment = WarehouseToShipAdapter.ConvertToShipDTO(totMastrs[0], WarehouseToShipAdapter.ConvertToShipDTO(totMastrs)); // Build Shipment

					EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}", MethodBase.GetCurrentMethod().ReflectedType.Name,
						MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.Info,
						string.Format("Finished building Shipment object for Warehouse shipment number {0}...", warehouseShipmentNumber));

					shipment = ShipShipmentAdapter.UpdateFields(shipment);

					EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}", MethodBase.GetCurrentMethod().ReflectedType.Name,
						MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.Data,
						string.Format("TOTMASTR records for Warehouse shipment number {0} converted to Ship Shipment...", warehouseShipmentNumber),
						new Variable("shipment", IFUtils.SerializeToString(shipment)));

					#region Rate Shopping
					IList<RateShopGroup> rateShopGroups = ShipRateShopGroupAdapter.ReadFromShip(shipment.CarrierServiceCode);

					if (rateShopGroups != null && rateShopGroups.Count > 0)
					{
						EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}", MethodBase.GetCurrentMethod().ReflectedType.Name,
							MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.Data,
							string.Format("Rate shopping for Warehouse shipment number {0}...", warehouseShipmentNumber),
							new Variable("shipment", IFUtils.SerializeToString(shipment)), new Variable("rateShopGroup", IFUtils.SerializeToString(rateShopGroups[0])));

						shipment = ShipShipmentAdapter.ProcessPreRateResponse(ShipShipmentAdapter.SetCarrierAsRateShopGroup(shipment, rateShopGroups[0]), ShipShipmentAdapter.PreRateOrder(shipment));
					}
					#endregion

					shipment = ShipShipmentAdapter.RateAndManifest(shipment, shipment.AOSSStatus == null ? false : (shipment.AOSSStatus.Id == (short)AOSSStatusID.Hold)); // Tell Ship to rate and manifest

					EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}", MethodBase.GetCurrentMethod().ReflectedType.Name,
						MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.Data,
						string.Format("Ship shipment {0} created/updated...", shipment.ShipmentNumber),
						new Variable("shipment", IFUtils.SerializeToString(shipment)));

					#region Update Warehouse
					foreach (Container container in shipment.Containers)
					{
						IEnumerable<TOTMASTR> totMastrsToUpdate = totMastrs.Where(item => item.CARTON_ASN == container.IdentityField);

						if (totMastrsToUpdate == null || !totMastrsToUpdate.Any())
							throw new Exception(string.Format("No TOTMASTR record could be found for Warehouse shipment number {0} and carton ASN {1}! Warehouse cannot be updated with the results of the rate and manifest.",
								warehouseShipmentNumber, container.IdentityField));

						TOTMASTR totMastrToUpdate = WarehouseTotMastrAdapter.WriteToWarehouse(ShipToWarehouseAdapter.ConvertToWarehouseDTO(
							container, totMastrsToUpdate.ElementAt<TOTMASTR>(0), TotMastrActions.RateManifestShipment, TotMastrActions.ShipmentComplete));

						EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}", MethodBase.GetCurrentMethod().ReflectedType.Name,
							MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.Data,
							string.Format("Ship results synchronized to Warehouse carton ASN {0}...", totMastrToUpdate.CARTON_ASN),
							new Variable("totMastrToUpdate", IFUtils.SerializeToString(totMastrToUpdate)));
					}
					#endregion

					EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}", MethodBase.GetCurrentMethod().ReflectedType.Name,
						MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.General,
						string.Format("Rate and manifest for Warehouse shipment number {0} (Ship shipment number {1}) complete!", warehouseShipmentNumber, shipment.ShipmentNumber),
						new Variable("shipment", IFUtils.SerializeToString(shipment)));

					return string.Empty;
				}
				catch (Exception innerCatchEx)
				{
					EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}.Error", MethodBase.GetCurrentMethod().ReflectedType.Name,
						MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.Error,
						string.Format("Warehouse shipment number {0} failed rate and manifest from Warehouse to Ship!", warehouseShipmentNumber), 
						new Variable("innerCatchEx", innerCatchEx));

					foreach (TOTMASTR totMastr in totMastrs) // Update WMS with error information
						WarehouseTotMastrAdapter.WriteToWarehouse(WarehouseTotMastrAdapter.ChangeError(totMastr, TotMastrActions.RateManifestShipment, innerCatchEx.Message));

					return innerCatchEx.Message;
				}
			}
			catch (Exception outerCatchEx)
			{
				EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}.Error", MethodBase.GetCurrentMethod().ReflectedType.Name,
					MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.Error,
					string.Format("Warehouse shipment number {0} failed rate and manifest from Warehouse to Ship because the TOTMASTR records could not be found!", warehouseShipmentNumber),
					new Variable("outerCatchEx", outerCatchEx));

				return outerCatchEx.Message;
			}
			finally
			{
				ControllerUtils.PostSync(false);
			}
		}
		#endregion

		#region RateManifestShipmentPolling
		[ExecutingFunction]
		public void RateManifestShipmentPolling()
		{
			try
			{
				EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}", MethodBase.GetCurrentMethod().ReflectedType.Name,
					MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.General, "Checking Warehouse for shipments ready for rate and manifest...");

				WarehouseA1ShipRequestAdapter.UpdateWarehouse(WarehouseA1ShipRequestAdapter.A1ShipRequestStatusInProcess,
					WarehouseA1ShipRequestAdapter.A1ShipRequestStatusNew, TotMastrActions.RateManifestShipment);

				using (DataTable warehouseA1ShipRequestRecords =
					WarehouseA1ShipRequestAdapter.ReadFromWarehouse(TotMastrActions.RateManifestShipment, WarehouseA1ShipRequestAdapter.A1ShipRequestStatusInProcess))
				{
					if (warehouseA1ShipRequestRecords.Rows.Count > 0)
					{
						foreach (DataRow warehouseA1ShipRequestRecord in warehouseA1ShipRequestRecords.Rows)
							RateManifestShipment(warehouseA1ShipRequestRecord["uniqueIdentifier"].ToString());

						WarehouseA1ShipRequestAdapter.UpdateWarehouse(warehouseA1ShipRequestRecords, WarehouseA1ShipRequestAdapter.A1ShipRequestStatusProcessed);
					}
				}

				EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}", MethodBase.GetCurrentMethod().ReflectedType.Name,
					MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.General, "Finished rate and manifest for Warehouse shipments!");
			}
			catch (Exception ex)
			{
				EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}.Error", MethodBase.GetCurrentMethod().ReflectedType.Name,
					MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.Error, "Unable to check Warehouse for shipments ready for rate and manifest!",
					new Variable("ex", ex));
			}
			finally
			{
				ControllerUtils.PostSync(false);
			}
		}
		#endregion

		#region PrePrintLabel (Event Based)
		public static string PrePrintLabel(string containerNumber)
		{
			ControllerUtils.PreSync(false);

			try
			{
				EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}", MethodBase.GetCurrentMethod().ReflectedType.Name,
					MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.General,
					string.Format("Beginning pre-print for container number {0}...", containerNumber));

				IList<TOTMASTR> totMastrs = WarehouseToShipAdapter.ReadFromWarehouse(containerNumber, TotMastrActions.PrePrintCartonLabel, UniqueIdentifierType.CartonASN, false); // TOTMASTR record for pre-print request

				if (totMastrs == null || totMastrs.Count == 0)
					throw new Exception(string.Format("No TOTMASTR records could be found for carton ASN {0} that are ready for pre-printing! The pre-print will not occur.",
						containerNumber));

				try
				{
					EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}", MethodBase.GetCurrentMethod().ReflectedType.Name,
						MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.Data,
						string.Format("TOTMASTR record retrieved for container number {0} (Warehouse shipment number {1})...", containerNumber, totMastrs[0].SHP_CNSLDT), 
						new Variable("totMastrs", IFUtils.SerializeToString(totMastrs)));

					Shipment shipment = WarehouseToShipAdapter.ConvertToShipDTO(totMastrs[0], WarehouseToShipAdapter.ConvertToShipDTO(totMastrs)); // Build Shipment

					EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}", MethodBase.GetCurrentMethod().ReflectedType.Name,
						MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.Info,
						string.Format("Finished building Shipment object for container number {0} (Warehouse shipment number {1})...", containerNumber, shipment.DocumentNumber));

					shipment = ShipShipmentAdapter.UpdateFields(shipment);

					EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}", MethodBase.GetCurrentMethod().ReflectedType.Name,
						MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.Data,
						string.Format("TOTMASTR records and Ship Container for container number {0} (Warehouse shipment number {1}) converted to Ship Shipment...", containerNumber, shipment.DocumentNumber),
						new Variable("shipment", IFUtils.SerializeToString(shipment)));

					shipment = ShipShipmentAdapter.PrePrint(shipment); // Tell Ship to pre-print

					EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}", MethodBase.GetCurrentMethod().ReflectedType.Name,
						MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.Data,
						string.Format("Ship shipment {0} created/updated...", shipment.ShipmentNumber),
						new Variable("shipment", IFUtils.SerializeToString(shipment)));

					#region Update Warehouse
					foreach (Container container in shipment.Containers)
					{
						IEnumerable<TOTMASTR> totMastrsToUpdate = totMastrs.Where(item => item.CARTON_ASN == container.IdentityField);

						if (totMastrsToUpdate == null || !totMastrsToUpdate.Any())
							throw new Exception(string.Format("No TOTMASTR record could be found for carton ASN {0}! The generated label cannot be sent back to Warehouse.",
								containerNumber, container.IdentityField));

						TOTMASTR totMastrToUpdate = WarehouseTotMastrAdapter.WriteToWarehouse(ShipToWarehouseAdapter.ConvertToWarehouseDTO(
							container, totMastrsToUpdate.ElementAt<TOTMASTR>(0), TotMastrActions.PrePrintCartonLabel, TotMastrActions.PrePrintComplete));

						EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}", MethodBase.GetCurrentMethod().ReflectedType.Name,
							MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.Data,
							string.Format("Ship results synchronized to Warehouse carton ASN {0}...", totMastrToUpdate.CARTON_ASN),
							new Variable("totMastrToUpdate", IFUtils.SerializeToString(totMastrToUpdate)));
					}
					#endregion

					EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}", MethodBase.GetCurrentMethod().ReflectedType.Name,
						MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.General,
						string.Format("Pre-print for container number {0} (Warehouse shipment number {1}) complete!", containerNumber, shipment.DocumentNumber),
						new Variable("shipment", IFUtils.SerializeToString(shipment)));

					return string.Empty;
				}
				catch (Exception innerCatchEx)
				{
					EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}.Error", MethodBase.GetCurrentMethod().ReflectedType.Name,
						MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.Error,
						string.Format("Container number {0} (Warehouse shipment number {1}) failed pre-print from Warehouse to Ship!", containerNumber, totMastrs[0].SHP_CNSLDT), 
						new Variable("innerCatchEx", innerCatchEx));

					foreach (TOTMASTR totMastr in totMastrs) // Update WMS with error information
						WarehouseTotMastrAdapter.WriteToWarehouse(WarehouseTotMastrAdapter.ChangeError(totMastr, TotMastrActions.PrePrintCartonLabel, innerCatchEx.Message));

					return innerCatchEx.Message;
				}
			}
			catch (Exception outerCatchEx)
			{
				EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}.Error", MethodBase.GetCurrentMethod().ReflectedType.Name,
					MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.Error,
					string.Format("Container number {0} failed pre-print from Warehouse to Ship because the TOTMASTR records could not be found!", containerNumber),
					new Variable("outerCatchEx", outerCatchEx));

				return outerCatchEx.Message;
			}
			finally
			{
				ControllerUtils.PostSync(false);
			}
		}
		#endregion

		#region PrePrintLabelPolling
		[ExecutingFunction]
		public void PrePrintLabelPolling()
		{
			try
			{
				EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}", MethodBase.GetCurrentMethod().ReflectedType.Name,
					MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.General, "Checking Warehouse for containers ready for pre-printing...");

				WarehouseA1ShipRequestAdapter.UpdateWarehouse(WarehouseA1ShipRequestAdapter.A1ShipRequestStatusInProcess,
					WarehouseA1ShipRequestAdapter.A1ShipRequestStatusNew, TotMastrActions.PrePrintCartonLabel);

				using (DataTable warehouseA1ShipRequestRecords =
					WarehouseA1ShipRequestAdapter.ReadFromWarehouse(TotMastrActions.PrePrintCartonLabel, WarehouseA1ShipRequestAdapter.A1ShipRequestStatusInProcess))
				{
					if (warehouseA1ShipRequestRecords.Rows.Count > 0)
					{
						foreach (DataRow warehouseA1ShipRequestRecord in warehouseA1ShipRequestRecords.Rows)
							PrePrintLabel(warehouseA1ShipRequestRecord["uniqueIdentifier"].ToString());

						WarehouseA1ShipRequestAdapter.UpdateWarehouse(warehouseA1ShipRequestRecords, WarehouseA1ShipRequestAdapter.A1ShipRequestStatusProcessed);
					}
				}

				EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}", MethodBase.GetCurrentMethod().ReflectedType.Name,
					MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.General, "Finished pre-printing for Warehouse containers!");
			}
			catch (Exception ex)
			{
				EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}.Error", MethodBase.GetCurrentMethod().ReflectedType.Name,
					MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.Error, "Unable to check Warehouse for containers ready for pre-printing!",
					new Variable("ex", ex));
			}
			finally
			{
				ControllerUtils.PostSync(false);
			}
		}
		#endregion

		#region VoidShipment (Event Based)
		public static string VoidShipment(string warehouseShipmentNumber)
		{
			ControllerUtils.PreSync(false);

			try
			{
				EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}", MethodBase.GetCurrentMethod().ReflectedType.Name,
					MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.General, 
					string.Format("Beginning void of Warehouse shipment number {0}...", warehouseShipmentNumber));

				IList<TOTMASTR> totMastrs = WarehouseToShipAdapter.ReadFromWarehouse(
					warehouseShipmentNumber, TotMastrActions.VoidShipment, UniqueIdentifierType.ShipmentNumber, false); // Get all TOTMASTR records for shipment

				if (totMastrs == null || totMastrs.Count == 0)
					throw new Exception(string.Format("No TOTMASTR records could be found for Warehouse shipment number {0}! The void will not occur.", warehouseShipmentNumber));

				try
				{
					EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}", MethodBase.GetCurrentMethod().ReflectedType.Name,
						MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.Data,
						string.Format("{0} TOTMASTR records retrieved for Warehouse shipment number {1}...", totMastrs.Count, warehouseShipmentNumber), 
						new Variable("totMastrs", IFUtils.SerializeToString(totMastrs)));

					Shipment shipment = ShipShipmentAdapter.ReadFromShip(warehouseShipmentNumber);

					if (shipment == null)
						throw new Exception(string.Format("No shipment with document number {0} could be found in Ship! The void will not occur.", warehouseShipmentNumber));

					EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}", MethodBase.GetCurrentMethod().ReflectedType.Name,
						MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.Data,
						string.Format("Retrieved Shipment object for Warehouse shipment number {0} (Ship shipment number {1})...", warehouseShipmentNumber, shipment.ShipmentNumber), 
						new Variable("shipment", IFUtils.SerializeToString(shipment)));

					string result = ShipShipmentAdapter.VoidShipment(shipment); // Tell Ship to void

					if (!string.IsNullOrWhiteSpace(result))
						throw new Exception(result);

					#region Update Warehouse
					foreach (TOTMASTR totMastr in totMastrs)
					{
						TOTMASTR totMastrToUpdate = 
							WarehouseTotMastrAdapter.WriteToWarehouse(WarehouseTotMastrAdapter.ChangeAction(totMastr, TotMastrActions.VoidShipment, TotMastrActions.VoidComplete));

						EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}", MethodBase.GetCurrentMethod().ReflectedType.Name,
							MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.Data,
							string.Format("Ship void results synchronized to Warehouse carton ASN {0}...", totMastrToUpdate.CARTON_ASN),
							new Variable("totMastrToUpdate", IFUtils.SerializeToString(totMastrToUpdate)));
					}
					#endregion

					EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}", MethodBase.GetCurrentMethod().ReflectedType.Name,
						MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.General,
						string.Format("Void of Ship shipment number {0} complete!", shipment.ShipmentNumber), new Variable("shipment", IFUtils.SerializeToString(shipment)));
					
					return string.Empty;
				}
				catch (Exception innerCatchEx)
				{
					EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}.Error", MethodBase.GetCurrentMethod().ReflectedType.Name,
						MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.Error,
						string.Format("Warehouse shipment number {0} failed voiding in Ship!", warehouseShipmentNumber), new Variable("innerCatchEx", innerCatchEx));

					foreach (TOTMASTR totMastr in totMastrs) // Update WMS with error information
						WarehouseTotMastrAdapter.WriteToWarehouse(WarehouseTotMastrAdapter.ChangeError(totMastr, TotMastrActions.VoidShipment, innerCatchEx.Message));

					return innerCatchEx.Message;
				}
			}
			catch (Exception outerCatchEx)
			{
				EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}.Error", MethodBase.GetCurrentMethod().ReflectedType.Name,
					MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.Error,
					string.Format("Warehouse shipment number {0} failed void in Ship because the TOTMASTR records could not be found!", warehouseShipmentNumber),
					new Variable("outerCatchEx", outerCatchEx));

				return outerCatchEx.Message;
			}
			finally
			{
				ControllerUtils.PostSync(false);
			}
		}
		#endregion

		#region SendManualShipment (Event Based)
		public static string SendManualShipment(object uniqueIdentifier)
		{
			string shipmentID = string.Empty;

			ControllerUtils.PreSync(false);

			try
			{
				if (string.IsNullOrWhiteSpace(Thread.CurrentThread.Name))
					Thread.CurrentThread.Name = Guid.NewGuid().ToString();

				string castedUniqueIdentifier = uniqueIdentifier as string;

				if (string.IsNullOrWhiteSpace(castedUniqueIdentifier))
					throw new Exception("The manual shipment identifier that Ship has requested from Warehouse is null, blank, or empty, and thus invalid!");

				lock (sendManualShipmentSyncObject)
				{
					if (WarehouseA1ShipRequestAdapter.IsRecordInStatus(TotMastrActions.ShipmentDataSync, WarehouseA1ShipRequestAdapter.A1ShipRequestStatusInProcess, castedUniqueIdentifier))
						throw new Exception(string.Format("The integration is already processing a manual shipment for identifier {0}. This request will be skipped.", castedUniqueIdentifier));

					WarehouseA1ShipRequestAdapter.InsertToWarehouse(TotMastrActions.ShipmentDataSync, castedUniqueIdentifier, WarehouseA1ShipRequestAdapter.A1ShipRequestStatusInProcess);
				}

				EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}", MethodBase.GetCurrentMethod().ReflectedType.Name,
					MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.General,
					string.Format("Checking if unique identifier {0} is a valid carton ASN...", castedUniqueIdentifier));

				IList<TOTMASTR> totMastrs = WarehouseToShipAdapter.ReadFromWarehouse(castedUniqueIdentifier, TotMastrActions.ShipmentDataSync, UniqueIdentifierType.CartonASN, false); // Get all TOTMASTR records based on carton ASN

				if (totMastrs == null || totMastrs.Count == 0) // Get all TOTMASTR records based on packslip if carton ASN query failed to find results
				{
					EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}", MethodBase.GetCurrentMethod().ReflectedType.Name,
						MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.General,
						string.Format("Checking if unique identifier {0} is a valid shipment consolidation number...", castedUniqueIdentifier));

					totMastrs = WarehouseToShipAdapter.ReadFromWarehouse(castedUniqueIdentifier, TotMastrActions.ShipmentDataSync, UniqueIdentifierType.ShipmentNumber, false);
				}

				if (totMastrs == null || totMastrs.Count == 0)
					throw new Exception(string.Format("No TOTMASTR records could be found with a carton ASN or shipment consolidation number of {0} that are ready for synchronization! The synchronization will not occur.",
						castedUniqueIdentifier));

				try
				{
					#region Clean Up Unused Containers
					IList<TOTMASTR> unusedTotMastrs = WarehouseToShipAdapter.ReadUnusedFromWarehouse(totMastrs[0].SHP_CNSLDT);

					if (unusedTotMastrs != null)
					{
						foreach (TOTMASTR unusedTotMastr in unusedTotMastrs)
						{
							Container unusedContainer = ShipContainerAdapter.ReadFromShip(unusedTotMastr.CARTON_ASN);

							if (unusedContainer != null)
								ShipContainerAdapter.RemoveContainer(unusedContainer);
						}
					}
					#endregion

					EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}", MethodBase.GetCurrentMethod().ReflectedType.Name,
						MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.Data,
						string.Format("{0} TOTMASTR records retrieved for unique identifier {1}...", totMastrs.Count, castedUniqueIdentifier),
						new Variable("totMastrs", IFUtils.SerializeToString(totMastrs)));

					Shipment shipment = WarehouseToShipAdapter.ConvertToShipDTO(totMastrs[0], WarehouseToShipAdapter.ConvertToShipDTO(totMastrs)); // Build Shipment

					EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}", MethodBase.GetCurrentMethod().ReflectedType.Name,
						MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.Info,
						string.Format("Finished building Shipment object for unique identifier {0}...", castedUniqueIdentifier));

					shipment = ShipShipmentAdapter.UpdateFields(shipment);

					if (shipment.AOSSStatus != null && shipment.AOSSStatus.Id > (int)AOSSStatusID.Hold) // All statuses above Hold (10) are invalid for the integration to modify the existing shipment
					{
						EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}", MethodBase.GetCurrentMethod().ReflectedType.Name,
							MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.Info,
							string.Format("Shipment {0} is already complete in Ship, so synchronization will not continue.", castedUniqueIdentifier),
							new Variable("shipment", IFUtils.SerializeToString(shipment)));

						return shipment.Id.ToString();
					}

					EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}", MethodBase.GetCurrentMethod().ReflectedType.Name,
						MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.Data,
						string.Format("TOTMASTR records and Ship Containers for unique identifier {0} converted to Ship Shipment...", castedUniqueIdentifier),
						new Variable("shipment", IFUtils.SerializeToString(shipment)));

					#region Check if Carrier is Rate Shop Group
					IList<RateShopGroup> rateShopGroups = ShipRateShopGroupAdapter.ReadFromShip(shipment.CarrierServiceCode);

					if (rateShopGroups != null && rateShopGroups.Count > 0)
						shipment = ShipShipmentAdapter.SetCarrierAsRateShopGroup(shipment, rateShopGroups[0]);
					#endregion

					shipment = ShipShipmentAdapter.SaveShipment(shipment); // Save Shipment to Ship

					EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}", MethodBase.GetCurrentMethod().ReflectedType.Name,
						MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.General,
						string.Format("Synchronization for unique identifier {0} complete!", castedUniqueIdentifier),
						new Variable("shipment", IFUtils.SerializeToString(shipment)));

					shipmentID = shipment.Id.ToString();
				}
				catch (Exception innerCatchEx)
				{
					EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}.Error", MethodBase.GetCurrentMethod().ReflectedType.Name,
						MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.Error,
						string.Format("Unique identifier {0} failed synchronization from Warehouse to Ship!", castedUniqueIdentifier), new Variable("innerCatchEx", innerCatchEx));

					foreach (TOTMASTR totMastr in totMastrs) // Update WMS with error information
					{
						totMastr.ACTION = (short)TotMastrActions.Error;
						totMastr.ERR_CODE = innerCatchEx.Message;

						WarehouseTotMastrAdapter.WriteToWarehouse(totMastr);
					}

					throw innerCatchEx;
				}
			}
			catch (Exception outerCatchEx)
			{
				EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}.Error", MethodBase.GetCurrentMethod().ReflectedType.Name,
					MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.Error,
					string.Format("Unique identifier {0} failed synchronization from Warehouse to Ship!", uniqueIdentifier),
					new Variable("outerCatchEx", outerCatchEx));

				throw outerCatchEx;
			}
			finally
			{
				string castedUniqueIdentifier = uniqueIdentifier as string;

				if (!string.IsNullOrWhiteSpace(castedUniqueIdentifier))
					WarehouseA1ShipRequestAdapter.UpdateWarehouse(WarehouseA1ShipRequestAdapter.ReadFromWarehouse(TotMastrActions.ShipmentDataSync, castedUniqueIdentifier, 
						WarehouseA1ShipRequestAdapter.A1ShipRequestStatusInProcess), WarehouseA1ShipRequestAdapter.A1ShipRequestStatusProcessed);

				ControllerUtils.PostSync(false);
			}

			return shipmentID;
		}
		#endregion

		#region DeleteContainer (Event Based)
		public static string DeleteContainer(string containerNumber)
		{
			ControllerUtils.PreSync(false);

			try
			{
				EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}", MethodBase.GetCurrentMethod().ReflectedType.Name,
					MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.General,
					string.Format("Beginning deletion of container number {0}...", containerNumber));

				Container container = ShipContainerAdapter.ReadFromShip(containerNumber);

				if (container == null || container.Id == Guid.Empty)
					throw new Exception(string.Format("Could not locate a container in Ship with an identity field value of {0}! Deletion will not occur.", containerNumber));

				try
				{
					EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}", MethodBase.GetCurrentMethod().ReflectedType.Name,
						MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.Data,
						string.Format("Container record retrieved for container number {0}...", containerNumber), new Variable("container", IFUtils.SerializeToString(container)));

					Shipment shipment = ShipContainerAdapter.RemoveContainer(container);

					EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}", MethodBase.GetCurrentMethod().ReflectedType.Name,
						MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.General,
						string.Format("Deletion of container number {0} complete!", containerNumber),
						new Variable("shipment", IFUtils.SerializeToString(shipment)));

					return string.Empty;
				}
				catch (Exception innerCatchEx)
				{
					EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}.Error", MethodBase.GetCurrentMethod().ReflectedType.Name,
						MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.Error,
						string.Format("Deletion of container number {0} from Ship failed!", containerNumber), new Variable("innerCatchEx", innerCatchEx));

					return innerCatchEx.Message;
				}
			}
			catch (Exception outerCatchEx)
			{
				EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}.Error", MethodBase.GetCurrentMethod().ReflectedType.Name,
					MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.Error,
					string.Format("Deletion of container number {0} from Ship failed!", containerNumber), new Variable("outerCatchEx", outerCatchEx));

				return outerCatchEx.Message;
			}
			finally
			{
				ControllerUtils.PostSync(false);
			}
		}
		#endregion

		#region DeleteContainerPolling
		[ExecutingFunction]
		public void DeleteContainerPolling()
		{
			try
			{
				EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}", MethodBase.GetCurrentMethod().ReflectedType.Name,
					MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.General, "Checking Warehouse for deleted containers...");

				WarehouseA1ShipRequestAdapter.UpdateWarehouse(WarehouseA1ShipRequestAdapter.A1ShipRequestStatusInProcess,
					WarehouseA1ShipRequestAdapter.A1ShipRequestStatusNew, TotMastrActions.DeleteCarton);

				using (DataTable warehouseA1ShipRequestRecords =
					WarehouseA1ShipRequestAdapter.ReadFromWarehouse(TotMastrActions.DeleteCarton, WarehouseA1ShipRequestAdapter.A1ShipRequestStatusInProcess))
				{
					if (warehouseA1ShipRequestRecords.Rows.Count > 0)
					{
						foreach (DataRow warehouseA1ShipRequestRecord in warehouseA1ShipRequestRecords.Rows)
							DeleteContainer(warehouseA1ShipRequestRecord["uniqueIdentifier"].ToString());

						WarehouseA1ShipRequestAdapter.UpdateWarehouse(warehouseA1ShipRequestRecords, WarehouseA1ShipRequestAdapter.A1ShipRequestStatusProcessed);
					}
				}

				EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}", MethodBase.GetCurrentMethod().ReflectedType.Name,
					MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.General, "Finished deleting containers from Ship!");
			}
			catch (Exception ex)
			{
				EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}.Error", MethodBase.GetCurrentMethod().ReflectedType.Name,
					MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.Error, "Unable to check Warehouse for deleted containers!",
					new Variable("ex", ex));
			}
			finally
			{
				ControllerUtils.PostSync(false);
			}
		}
		#endregion
	}
}
