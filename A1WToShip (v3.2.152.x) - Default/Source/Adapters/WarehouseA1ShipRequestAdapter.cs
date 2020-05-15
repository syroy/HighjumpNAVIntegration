using HighJump.Integrations.IntegrationUtils.DB;
using HighJump.Integrations.WarehouseUtils.Adapters;
using System;
using System.Data;
using System.Threading;

namespace HighJump.Integrations.WarehouseToShip.Adapters
{
    public static class WarehouseA1ShipRequestAdapter
	{
		private static readonly object databaseSyncObject = new object();
		public static readonly string A1ShipRequestStatusInProcess = "I";
		public static readonly string A1ShipRequestStatusNew = "N";
		public static readonly string A1ShipRequestStatusProcessed = "P";
		public static readonly string A1ShipRequestTableSchemaQuery = "select * from A1ShipRequest where 1 = 0";

		#region ReadFromWarehouse (Bulk for Given Action, Status, and Current Thread)
		public static DataTable ReadFromWarehouse(TotMastrActions totMastrAction, string a1ShipRequestStatus)
		{
			using (DBConnection warehouseDBConnection = WarehouseDatabaseConnectionAdapter.GetDatabaseConnection())
				return warehouseDBConnection.ExecuteDataTableQuery(@"
select 
	* 
from 
	A1ShipRequest 
where 
	[action] = {0} and 
	[status] = '{1}' and 
	[threadName] = '{2}'
order by [createdDate] asc", (short)totMastrAction, a1ShipRequestStatus, Thread.CurrentThread.Name);
		}
		#endregion

		#region ReadFromWarehouse (Specific Record)
		public static DataTable ReadFromWarehouse(TotMastrActions totMastrAction, string uniqueIdentifier, string a1ShipRequestStatus)
		{
			using (DBConnection warehouseDBConnection = WarehouseDatabaseConnectionAdapter.GetDatabaseConnection())
				return warehouseDBConnection.ExecuteDataTableQuery(@"
select 
	* 
from 
	A1ShipRequest 
where 
	[action] = {0} and 
	[uniqueIdentifier] = '{1}' and 
	[status] = '{2}' and 
	[threadName] = '{3}'
order by [createdDate] asc", (short)totMastrAction, uniqueIdentifier, a1ShipRequestStatus, Thread.CurrentThread.Name);
		}
		#endregion

		#region ReadSchemaFromWarehouse
		public static DataTable ReadSchemaFromWarehouse()
		{
			using (DBConnection warehouseDBConnection = WarehouseDatabaseConnectionAdapter.GetDatabaseConnection())
				return warehouseDBConnection.ExecuteDataTableQuery(A1ShipRequestTableSchemaQuery);
		}
		#endregion

		#region IsRecordInStatus
		public static bool IsRecordInStatus(TotMastrActions totMastrAction, string a1ShipRequestStatus, string uniqueIdentifier)
		{
			using (DBConnection warehouseDBConnection = WarehouseDatabaseConnectionAdapter.GetDatabaseConnection())
			{
				int? recordCount = warehouseDBConnection.ExecuteScalar(@"
select 
	count(id) 
from 
	A1ShipRequest 
where 
	[action] = {0} and 
	[uniqueIdentifier] = '{1}' and 
	[status] = '{2}'", (short)totMastrAction, uniqueIdentifier, a1ShipRequestStatus) as int?;

				return recordCount.HasValue && recordCount > 0;
			}
		}
		#endregion

		#region InsertToWarehouse
		public static DataTable InsertToWarehouse(TotMastrActions totMastrAction, string uniqueIdentifier, string status)
		{
			DataTable warehouseA1ShipRequestRecords = ReadSchemaFromWarehouse();

			warehouseA1ShipRequestRecords.Rows.Add(warehouseA1ShipRequestRecords.NewRow());
			warehouseA1ShipRequestRecords.Rows[0]["id"] = Guid.NewGuid();
			warehouseA1ShipRequestRecords.Rows[0]["action"] = (short)totMastrAction;
			warehouseA1ShipRequestRecords.Rows[0]["uniqueIdentifier"] = uniqueIdentifier;
			warehouseA1ShipRequestRecords.Rows[0]["status"] = status;
			warehouseA1ShipRequestRecords.Rows[0]["threadName"] = Thread.CurrentThread.Name;
			warehouseA1ShipRequestRecords.Rows[0]["createdDate"] = DateTime.UtcNow;

			lock (databaseSyncObject)
				using (DBConnection warehouseDBConnection = WarehouseDatabaseConnectionAdapter.GetDatabaseConnection())
					using (DBAdapter a1ShipRequestDBAdapter = new DBAdapter(A1ShipRequestTableSchemaQuery, warehouseDBConnection.SqlConnection))
						a1ShipRequestDBAdapter.Update(warehouseA1ShipRequestRecords);

			return warehouseA1ShipRequestRecords;
		}
		#endregion

		#region UpdateWarehouse (Passed In)
		public static DataTable UpdateWarehouse(DataTable warehouseA1ShipRequestRecords, string a1ShipRequestStatus)
		{
			if (warehouseA1ShipRequestRecords.Rows.Count == 0)
				throw new Exception("The A1ShipRequest table update failed because the submitted table contains no rows!");

			foreach (DataRow warehouseA1ShipRequestRecord in warehouseA1ShipRequestRecords.Rows)
			{
				warehouseA1ShipRequestRecord["status"] = a1ShipRequestStatus;
				warehouseA1ShipRequestRecord["threadName"] = string.Empty;
			}

			lock (databaseSyncObject)
				using (DBConnection warehouseDBConnection = WarehouseDatabaseConnectionAdapter.GetDatabaseConnection())
					using (DBAdapter a1ShipRequestDBAdapter = new DBAdapter(A1ShipRequestTableSchemaQuery, warehouseDBConnection.SqlConnection))
						a1ShipRequestDBAdapter.Update(warehouseA1ShipRequestRecords);

			return warehouseA1ShipRequestRecords;
		}
		#endregion

		#region UpdateWarehouse (Set Bulk)
		public static void UpdateWarehouse(string setA1ShipRequestStatus, string currentA1ShipRequestStatus, TotMastrActions totMastrAction)
		{
			lock (databaseSyncObject)
				using (DBConnection warehouseDBConnection = WarehouseDatabaseConnectionAdapter.GetDatabaseConnection())
					warehouseDBConnection.ExecuteNonQuery(@"
update 
	A1ShipRequest 
set 
	[status] = '{0}', 
	[threadName] = '{1}' 
where 
	[status] = '{2}' and 
	[action] = {3} and 
	([threadName] = '{1}' or [threadName] = '' or [threadName] is null)", setA1ShipRequestStatus, Thread.CurrentThread.Name, currentA1ShipRequestStatus, (short)totMastrAction);
		}
		#endregion
	}
}
