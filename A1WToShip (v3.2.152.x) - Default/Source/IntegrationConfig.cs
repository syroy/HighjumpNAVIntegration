using Accellos.InterfaceFramework;
using Accellos.InterfaceFramework.ScriptManagementServer;
using System;
using System.ComponentModel;

namespace HighJump.Integrations.WarehouseToShip
{
    [ConfigurationClass("Warehouse_Ship_Config", "WarehouseShipConfig")]
    public class IntegrationConfig
    {
        private static readonly object instanceSyncObject = new object();

        #region Ctor
        public IntegrationConfig()
        {

        }
        #endregion

        #region InstanceID
        private static string instanceID;
        [Browsable(false)]
        public static string InstanceID
        {
            get
            {
                return instanceID = instanceID ?? AppDomain.CurrentDomain.FriendlyName;
            }
            set
            {
                instanceID = value;
            }
        }
        #endregion

        #region Instance
        private static IntegrationConfig instance;
        [Browsable(false)]
        public static IntegrationConfig Instance
        {
            get
            {
                lock (instanceSyncObject)
                    return instance = instance ?? ConfigurationManager<IntegrationConfig>.GetConfigurationOnServer(IntegrationConfig.InstanceID);
            }
        }

        public static void ResetConfiguration()
        {
            lock (instanceSyncObject)
                instance = null;
        }
        #endregion

        #region Application Versions
        [DescriptionAttribute("Application Version")]
        [CategoryAttribute("Application Versions")]
        public string ApplicationVersion
        {
            get
            {
                return "3.2.152.1";
            }
        }
        #endregion
    }
}
