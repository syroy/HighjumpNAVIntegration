using Accellos.InterfaceFramework.ScriptManagementServer;
using HighJump.Integrations.ShipUtils.Services;
using HighJump.Integrations.WarehouseToShip.Controllers;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace HighJump.Integrations.WarehouseToShip.Services
{
    [WcfServiceClassAttribute]
	[ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
	public class ManualShipment : IManualShipment
	{
		public IList<ManualShipmentResponse> GetExternalShipment(object uniqueIdentifier)
		{
			IList<ManualShipmentResponse> manualShipmentResponses = new List<ManualShipmentResponse>();
			ManualShipmentResponse manualShipmentResponse = new ManualShipmentResponse();

			try
			{
				manualShipmentResponse.ShipmentId = WarehouseToShipController.SendManualShipment(uniqueIdentifier);
			}
			catch (Exception ex)
			{
				manualShipmentResponse.ErrorMessage = ex.ToString();
			}

			manualShipmentResponses.Add(manualShipmentResponse);

			return manualShipmentResponses;
		}

		public void UpdateExternalShipment()
		{
			ShipToWarehouseController shipToWarehouseController = new ShipToWarehouseController();

			shipToWarehouseController.UpdateManualShipments();
		}
	}
}
