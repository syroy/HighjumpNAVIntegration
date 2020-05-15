using Accellos.InterfaceFramework.ScriptManagementServer;
using HighJump.Integrations.ShipUtils.Services;
using HighJump.Integrations.WarehouseToShip.Controllers;
using System.ServiceModel;

namespace HighJump.Integrations.WarehouseToShip.Services
{
    [WcfServiceClassAttribute]
	[ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
	public class ShipRequest : IShipRequest
	{
		public string PreRateOrder(string orderNumber)
		{
			return WarehouseToShipController.PreRateOrder(orderNumber);
		}

		public string RateManifestShipment(string shipmentNumber)
		{
			return WarehouseToShipController.RateManifestShipment(shipmentNumber);
		}

		public string PrePrintLabel(string containerNumber)
		{
			return WarehouseToShipController.PrePrintLabel(containerNumber);
		}

		public string PrePrintLabels(string shipmentNumber)
		{
			return string.Empty;
		}

		public string VoidShipment(string shipmentNumber)
		{
			return WarehouseToShipController.VoidShipment(shipmentNumber);
		}

		public string DeleteContainer(string containerNumber)
		{
			return null;
		}
	}
}
