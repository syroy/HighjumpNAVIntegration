using System.Data;
using System.Data.SqlClient;

namespace Accellos.Interfaces.NAV
{
	public class DatabaseConnection
	{
		private readonly SqlDataAdapter adapter;
		private readonly string connectionString;

		#region Constructor
		public DatabaseConnection(string connectionString)
		{
			this.adapter = new SqlDataAdapter();
			this.connectionString = connectionString;
		}
		#endregion

		#region SqlConnection
		private SqlConnection sqlConnection;
		public SqlConnection SqlConnection
		{
			get
			{
				if (this.sqlConnection == null)
					this.sqlConnection = new SqlConnection(this.connectionString);
				if (this.sqlConnection.State != ConnectionState.Open)
					this.sqlConnection.Open();
				return this.sqlConnection;
			}
		}
		#endregion

		#region SqlCommand
		private SqlCommand sqlCommand;
		public SqlCommand SqlCommand
		{
			get
			{
				if (this.SqlConnection.State != ConnectionState.Open)
					sqlCommand = null;
				if (sqlCommand == null)
				{
					sqlCommand = this.SqlConnection.CreateCommand();
					sqlCommand.CommandTimeout = 0;
				}
				return sqlCommand;
			}
		}
		#endregion

		#region ExecuteNonQuery
		public object ExecuteNonQuery(string query, params object[] parameters)
		{
			this.SqlCommand.Parameters.Clear();

			for (int i = 0; parameters != null && i < parameters.Length; i++)
			{
				query = query.Replace("'{" + i + "}'", "@P" + i);
				this.SqlCommand.Parameters.Add(new SqlParameter("@P" + i, parameters[i]));
			}

			this.SqlCommand.CommandText = string.Format(query, parameters);
			object result = this.SqlCommand.ExecuteNonQuery();
			Destroy();
			return result;
		}
		#endregion

		#region ExecuteScalar
		public object ExecuteScalar(string query, params object[] parameters)
		{
			this.SqlCommand.Parameters.Clear();

			for (int i = 0; parameters != null && i < parameters.Length; i++)
			{
				query = query.Replace("'{" + i + "}'", "@P" + i);
				this.SqlCommand.Parameters.Add(new SqlParameter("@P" + i, parameters[i]));
			}

			this.SqlCommand.CommandText = string.Format(query, parameters);
			object result = this.SqlCommand.ExecuteScalar();
			Destroy();
			return result;
		}
		#endregion

		#region ExecuteDataTableQuery
		public DataTable ExecuteDataTableQuery(string query, params object[] parameters)
		{
			this.SqlCommand.Parameters.Clear();

			for (int i = 0; parameters != null && i < parameters.Length; i++)
			{
				query = query.Replace("'{" + i + "}'", "@P" + i);
				this.SqlCommand.Parameters.Add(new SqlParameter("@P" + i, parameters[i]));
			}

			query = string.Format(query, parameters);
			this.SqlCommand.CommandText = query;
			this.adapter.SelectCommand = this.sqlCommand;
			DataTable table = new DataTable();
			this.adapter.Fill(table);
			Destroy();
			return table;
		}
		#endregion

		#region Destroy
		public void Destroy()
		{
			if (this.sqlConnection != null)
				if (this.sqlConnection.State == ConnectionState.Open)
					this.sqlConnection.Close();
			this.sqlConnection = null;
			this.sqlCommand = null;
		}
		#endregion

		#region Transaction management
		private SqlTransaction transaction;

		#region TransactionDepth
		private int transactionDepth = 0;
		public int TransactionDepth
		{
			get { return transactionDepth; }
			set { transactionDepth = value; }
		}
		#endregion

		#region StartTransaction
		public SqlTransaction StartTransaction()
		{
			TransactionDepth++;

			if (transaction != null)
				return transaction;

			transaction = this.SqlConnection.BeginTransaction(IsolationLevel.ReadCommitted);
			this.SqlCommand.Transaction = transaction;

			return transaction;
		}
		#endregion

		#region CommitTransaction
		public void CommitTransaction()
		{
			TransactionDepth--;
			if (TransactionDepth != 0)
				return;
			if (transaction != null)
				transaction.Commit();
			transaction = null;
		}
		#endregion

		#region RollbackTransaction
		public void RollbackTransaction()
		{
			TransactionDepth = 0;
			if (transaction != null)
				transaction.Rollback();
			transaction = null;
		}
		#endregion
		#endregion
	}
}
