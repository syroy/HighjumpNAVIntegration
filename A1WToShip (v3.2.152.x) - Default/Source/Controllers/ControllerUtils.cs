using Accellos.InterfaceFramework.EventNotificationServer;
using HighJump.Integrations.ShipUtils;
using HighJump.Integrations.WarehouseUtils;
using System;
using System.Reflection;

namespace HighJump.Integrations.WarehouseToShip.Controllers
{
    public static class ControllerUtils
	{
		#region PreSync
		public static bool PreSync(bool isError)
		{
			try
			{
				IntegrationConfig.ResetConfiguration();
				ShipIntegrationConfig.ResetConfiguration();
				WarehouseIntegrationConfig.ResetConfiguration();
			}
			catch (Exception ex)
			{
				EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}.Error", MethodBase.GetCurrentMethod().ReflectedType.Name, 
					MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.Error, ex.Message, new Variable("ex", ex));

				isError = true;
			}

			return isError;
		}
		#endregion

		#region PostSync
		public static bool PostSync(bool isError)
		{
			try
			{
                // ToDo: any necessary cleanup
			}
			catch (Exception ex)
			{
				EventNotificationClient.Instance.PostMessage(string.Format("{0}.{1}.Error", MethodBase.GetCurrentMethod().ReflectedType.Name,
					MethodBase.GetCurrentMethod().Name), IntegrationConfig.InstanceID, CategoryType.Error, ex.Message, new Variable("ex", ex));

				isError = true;
			}

			return isError;
		}
		#endregion
	}
}
