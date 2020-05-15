using Accellos.InterfaceFramework.EventNotificationServer;
using Accellos.InterfaceFramework.ScriptManagementServer;
using Accellos.Ship.MT.Common.DTO;
using Accellos.Warehouse.MT.Common.DTO;
using HighJump.Integrations.ShipUtils.Adapters;
using HighJump.Integrations.WarehouseToShip.MappingAdapters;
using HighJump.Integrations.WarehouseUtils.Adapters;
using System;
using System.Collections.Generic;
using System.Reflection;
using IFUtils = Accellos.InterfaceFramework.Utils;

namespace HighJump.Integrations.WarehouseToShip.Controllers
{
    [ExecutingClass]
	public class ShipToWarehouseController
	{
		#region UpdateManualShipments
		[ExecutingFunction]
		public void UpdateManualShipments()
		{
			ControllerUtils.PreSync(false);

			try
			{
				EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}", MethodBase.GetCurrentMethod().ReflectedType.Name,
					MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.General, "Beginning manual shipment updates...");

				IList<IntegrationSync> integrationSyncs = ShipIntegrationSyncAdapter.ReadFromShip(ShipIntegrationType.AccellosOneWarehouse);

				if (integrationSyncs == null || integrationSyncs.Count == 0)
					return;

				foreach (IntegrationSync integrationSync in integrationSyncs)
				{
					try
					{
						if (!integrationSync.ShipmentId.HasValue)
							continue;

						Shipment shipment = ShipShipmentAdapter.ReadFromShip((Guid)integrationSync.ShipmentId);

						if (shipment == null || shipment.AOSSStatus == null || (shipment.AOSSStatus.Id != (int)AOSSStatusID.Completed && shipment.AOSSStatus.Id != (int)AOSSStatusID.Void))
							continue;

						EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}", MethodBase.GetCurrentMethod().ReflectedType.Name,
							MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.Data,
							string.Format("Updating manual shipping information for shipment number {0}...", shipment.ShipmentNumber),
							new Variable("shipment", IFUtils.SerializeToString(shipment)));

						List<TOTMASTR> totMastrsToUpdate = new List<TOTMASTR>();

						foreach (Container container in shipment.Containers) // Build TOTMASTRs for Update
						{
							IList<TOTMASTR> currentTotMastrs = 
								WarehouseToShipAdapter.ReadFromWarehouse(container.IdentityField, TotMastrActions.ShipmentDataSync, UniqueIdentifierType.CartonASN, true);

							if (currentTotMastrs == null || currentTotMastrs.Count == 0)
								throw new Exception(string.Format("No TOTMASTR record for carton ASN {0} could be found!", container.IdentityField));

							if (shipment.AOSSStatus.Id == (int)AOSSStatusID.Completed)
								totMastrsToUpdate.Add(ShipToWarehouseAdapter.ConvertToWarehouseDTO(container, currentTotMastrs[0], TotMastrActions.ShipmentComplete));
							else if (shipment.AOSSStatus.Id == (int)AOSSStatusID.Void)
								totMastrsToUpdate.Add(ShipToWarehouseAdapter.ConvertToWarehouseDTO(container, currentTotMastrs[0], TotMastrActions.VoidComplete));
						}

						EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}", MethodBase.GetCurrentMethod().ReflectedType.Name,
							MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.Info,
							string.Format("Finished building updated TOTMASTR objects for shipment number {0}...", shipment.ShipmentNumber),
							new Variable("updatedTotMastrs", IFUtils.SerializeToString(totMastrsToUpdate)));

						foreach (TOTMASTR totMastrToUpdate in totMastrsToUpdate) // Write each TOTMASTR update
						{
							TOTMASTR updatedTotMastr = WarehouseTotMastrAdapter.WriteToWarehouse(totMastrToUpdate);

							EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}", MethodBase.GetCurrentMethod().ReflectedType.Name,
								MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.Data,
								string.Format("TOTMASTR record for shipment number {0} and carton ASN {1} updated...", shipment.ShipmentNumber, updatedTotMastr.CARTON_ASN),
								new Variable("updatedTotMastr", IFUtils.SerializeToString(updatedTotMastr)));
						}

						ShipIntegrationSyncAdapter.DeleteFromShip(integrationSync); // Delete IntegrationSync record from Ship

						EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}", MethodBase.GetCurrentMethod().ReflectedType.Name,
							MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.General,
							string.Format("Manual shipment updates for shipment number {0} complete!", shipment.ShipmentNumber));
					}
					catch (Exception innerCatchEx)
					{
						EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}.Error", MethodBase.GetCurrentMethod().ReflectedType.Name,
							MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.Error,
							string.Format("Shipment ID {0} failed sending manual updates from Ship to Warehouse! IntegrationSync record will be deleted!", integrationSync.ShipmentId), 
							new Variable("innerCatchEx", innerCatchEx));

						ShipIntegrationSyncAdapter.DeleteFromShip(integrationSync); // Delete IntegrationSync record from Ship
					}
				}
			}
			catch (Exception outerCatchEx)
			{
				EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}.Error", MethodBase.GetCurrentMethod().ReflectedType.Name,
					MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.Error, "Manual update sync from Ship to Warehouse failed!", 
					new Variable("outerCatchEx", outerCatchEx));
			}
			finally
			{
				ControllerUtils.PostSync(false);
			}
		}
		#endregion
	}
}
