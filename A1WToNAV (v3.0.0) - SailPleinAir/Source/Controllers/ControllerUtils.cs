using System.Data;
using System.Text;

namespace Accellos.Interfaces.NAV.Controllers
{
	public static class ControllerUtils
	{
		#region DataTable2LogString
		public static string DataTable2LogString(DataTable dt)
		{
			StringBuilder strBuilder = new StringBuilder();

			foreach (DataRow row in dt.Rows)
			{
				foreach (DataColumn column in dt.Columns)
					strBuilder.Append(string.Format("[{0}]:{1}", column.ColumnName, row[column.ColumnName]));
				strBuilder.Append("\n");
			}

			return strBuilder.ToString();
		}
		#endregion
	}
}

