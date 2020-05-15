using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Design;
using System.IO;
using System.Text;
using Accellos.InterfaceFramework;
using Accellos.InterfaceFramework.ScriptManagementServer;
using Accellos.Interfaces.NAV.NAVWebServices;
using System.Net;

namespace Accellos.Interfaces.NAV
{
    [ConfigurationClass("NAV_config", "NAV Integration Configuration")]
	public class CompanyConfig
	{
        public static object syncObject = new object();

		#region Constructor
		public CompanyConfig()
		{
            //this.CompanyId = "CRONUS-USA";
		}
		#endregion

        #region InstanceId
        private static String instanceId;
        [Browsable(false)]
        public static String InstanceId
        {
            get
            {
              if (AppDomain.CurrentDomain.FriendlyName == "Test.vshost.exe") 
                  instanceId = "NAV2013-Busch"; // iCepts for Testing - DRS 12/17/15

              return instanceId = instanceId ?? AppDomain.CurrentDomain.FriendlyName; 
            }
            set
            {
                instanceId = value;
            }
        }
        #endregion

		//Static Properties
		#region Instance
		private static CompanyConfig instance;
		[Browsable(false)]
		public static CompanyConfig Instance
		{
			get
			{
				if (instance == null)
					instance = ConfigurationManager<CompanyConfig>.GetConfigurationOnServer(CompanyConfig.InstanceId);

				return instance;
			}
		}

		public static void ResetConfiguration()
		{
			instance = null;
		}
		#endregion

        //NAV Webservice accessor
        #region NavWebServiceInstance
        private NAVA1WIntegration navWebServiceInstance;
        [Browsable(false)]
        public NAVA1WIntegration NavWebServiceInstance
        {
            get
            {
                if (navWebServiceInstance == null)
                {
                    navWebServiceInstance = new NAVA1WIntegration();
                    navWebServiceInstance.Url = this.Url;
                    //navWebServiceInstance.UseDefaultCredentials = true;
                    navWebServiceInstance.UseDefaultCredentials = false;
                    navWebServiceInstance.PreAuthenticate = false;
                    navWebServiceInstance.Credentials = new NetworkCredential(Username, Password, Domain);
					navWebServiceInstance.Timeout = 1200000; //10 Minutes Added 03/26/18  //Createch updated to 20 min 
                }

                return navWebServiceInstance;
            }
        }
        #endregion
        
        //NAV SQL Accessor
        #region NAVDatabaseConnection
        private DatabaseConnection dbConnNAV;
        public DatabaseConnection GetNAVDBConnection()
        {
            if (dbConnNAV == null)
            {
                dbConnNAV = new DatabaseConnection(this.NAVDBConnectionString);
            }

            return dbConnNAV;
        }
        #endregion

		//WMS Accessor operations
		#region WmsDatabaseConnection
		private DatabaseConnection wmsDatabaseConnection;
		public DatabaseConnection GetWarehouseDBConnection()
		{
			lock (syncObject)
			{
				if (wmsDatabaseConnection == null)
                    wmsDatabaseConnection = new DatabaseConnection(this.WarehouseDBConnectionString);

				return wmsDatabaseConnection;
			}
		}
		#endregion

		#region DownloadDataTable
		[BrowsableAttribute(false)]
		public DataTable DownloadTableSchema
		{
			get
			{
				return this.GetWarehouseDBConnection().ExecuteDataTableQuery("select * from dnload where 1 = 0");
			}
		}
		#endregion

        #region UploadDataTable
		[BrowsableAttribute(false)]
		public DataTable UploadDataTable
		{
			get
			{
				return this.GetWarehouseDBConnection().ExecuteDataTableQuery("select * from upload where 1 = 0");
			}
		}
		#endregion

		#region DownloadSqlAdapter
		private SqlDataAdapter downloadSqlAdapter;
		[BrowsableAttribute(false)]
		private SqlDataAdapter DownloadSqlAdapter
		{
			get
			{
				lock (syncObject)
				{
					if (downloadSqlAdapter == null)
					{
						downloadSqlAdapter = new SqlDataAdapter("select * from dnload where 1 = 0", this.wmsDatabaseConnection.SqlConnection);
						new SqlCommandBuilder(downloadSqlAdapter);
					}

					return downloadSqlAdapter;
				}
			}
		}
		#endregion

		#region PostDownloadRecords
		[BrowsableAttribute(false)]
		public String PostDownloadRecords(DataTable records)
		{
			StringBuilder builder = new StringBuilder();
			DateTime date = DateTime.Now;
			
			#region Createch - Clientname
            /*
			DataRow[] DetailRow, HeaderRow;
			DataRow CloneRow = records.NewRow();
            string DetailSearchExpression="", HeaderSearchExpression="";
            string clientnameField = "", clientname = "", warehouse="", bin="";

            DetailSearchExpression = "FIELD001 in ('PD','RD')";
            HeaderSearchExpression = "FIELD001 in ('PH','RH','VA','CA')";
			
			HeaderRow = records.Select(HeaderSearchExpression);
            if (HeaderRow != null && HeaderRow.Length>0)
            {
                switch(HeaderRow[0]["FIELD001"].ToString())
				{
           			case "PH":
						clientnameField = "FIELD041";
						DetailRow = records.Select(DetailSearchExpression);
			            if (DetailRow != null && DetailRow.Length>0)
			            {
							warehouse = DetailRow[0]["FIELD002"].ToString(); //warehouse in column FIELD002, only look for 1st row found, no need for others
			                clientname = CompanyConfig.instance.SAIL_ClientWhList.IndexOf(warehouse)>=0 ? "SAIL" : "SPORTIUM"; 
			            }
						else
						{
							CloneRow.ItemArray = HeaderRow[0].ItemArray;
							CloneRow[clientnameField]="SAIL";
							records.Rows.Add(CloneRow);						
							clientname = "SPORTIUM";  //Clone row is set to SAIL so initial row is set to SPORTIUM
						}
						break;
					case "RH":
						clientnameField = "FIELD009";
			            DetailRow = records.Select(DetailSearchExpression);
			            if (DetailRow != null && DetailRow.Length>0)
			            {
							warehouse = DetailRow[0]["FIELD002"].ToString(); //warehouse in column FIELD002, only look for 1st row found, no need for others
			                clientname = CompanyConfig.instance.SAIL_ClientWhList.IndexOf(warehouse)>=0 ? "SAIL" : "SPORTIUM"; 
			            }
						else
						{
							CloneRow.ItemArray = HeaderRow[0].ItemArray;
							CloneRow[clientnameField]="SAIL";
							records.Rows.Add(CloneRow);						
							clientname = "SPORTIUM";  //Clone row is set to SAIL so initial row is set to SPORTIUM
						}
						break;
					case "CA":
						clientnameField = "FIELD027";
						bin = HeaderRow[0]["FIELD011"].ToString();
						clientname = CompanyConfig.instance.SAIL_ClientWhList.IndexOf(bin)>=0 ? "SAIL" : "SPORTIUM"; 
						break;
					case "VA":
						clientnameField = "FIELD019";
						CloneRow.ItemArray = HeaderRow[0].ItemArray;
						CloneRow[clientnameField]="SAIL";
						records.Rows.Add(CloneRow);						
						clientname = "SPORTIUM";  //Clone row is set to SAIL so initial row is set to SPORTIUM
						break;
				}
				HeaderRow[0][clientnameField] = clientname; 
			}
			*/
            #endregion
			
			foreach (DataRow row in records.Rows)
			{
				foreach (DataColumn column in records.Columns)
				{
                    if (this.DNLoadFieldLengthSchema[column.ColumnName].DataType == "varchar" || this.DNLoadFieldLengthSchema[column.ColumnName].DataType == "varchar") // Changed for A1W 64
						if (row[column.ColumnName].ToString().Length > this.DNLoadFieldLengthSchema[column.ColumnName].Length)
						{
							builder.AppendLine(string.Format("Field [{0}] is truncated to {1} length:", column.ColumnName, 
								this.DNLoadFieldLengthSchema[column.ColumnName].Length));
							builder.AppendLine(string.Format("Old value: {0}", row[column.ColumnName]));
							row[column.ColumnName] = row[column.ColumnName].ToString().Substring(0, 
								this.DNLoadFieldLengthSchema[column.ColumnName].Length);
							builder.AppendLine(string.Format("New value: {0}", row[column.ColumnName]));
							builder.AppendLine();
						}
				}

				row["TIMESTMP"] = date.ToString("yyyyMMdd:HH-mm-ss.ffff");
			}

			try
			{
				SqlTransaction transaction = this.GetWarehouseDBConnection().StartTransaction();

				this.DownloadSqlAdapter.SelectCommand.Transaction = transaction;
				this.DownloadSqlAdapter.Update(records);
				this.GetWarehouseDBConnection().CommitTransaction();
				this.downloadSqlAdapter = null;
			}
			catch
			{
				this.GetWarehouseDBConnection().RollbackTransaction();
				throw;
			}

			return builder.ToString();
		}
		#endregion

		#region DNLoadFieldLengthSchema
		private Dictionary<string, SqlColumndDetails> dnLoadSchema = null;
		private class SqlColumndDetails
		{
			public String ColumnName { get; set; }
			public String DataType { get; set; }
			public short Length { get; set; }
		}

		private Dictionary<string, SqlColumndDetails> DNLoadFieldLengthSchema
		{
			get
			{
				if (dnLoadSchema == null)
				{
					dnLoadSchema = new Dictionary<string, SqlColumndDetails>();
					using (DataTable dt = CompanyConfig.Instance.GetWarehouseDBConnection().ExecuteDataTableQuery(@"
SELECT
sysobjects.name AS TABLE_NAME, 
syscolumns.name AS COLUMN_NAME, 
systypes.name AS DATA_TYPE, 
syscolumns.LENGTH AS LENGTH 
FROM
sysobjects 
INNER JOIN 
syscolumns ON sysobjects.id = syscolumns.id 
INNER JOIN                      
systypes ON syscolumns.xtype = systypes.xtype 
WHERE     
(sysobjects.xtype = 'U') and
sysobjects.name = 'DNLOAD'
ORDER BY sysobjects.name, syscolumns.colid"))
						foreach (DataRow row in dt.Rows)
							dnLoadSchema[row["COLUMN_NAME"].ToString()] = new SqlColumndDetails
							{
								ColumnName = row["COLUMN_NAME"].ToString(),
								DataType = row["DATA_TYPE"].ToString(),
								Length = (short)row["LENGTH"],
							};
				}
				return dnLoadSchema;
			}
		}
		#endregion
        
		#region A1W DNLoadFieldLengthSchema
//        private Dictionary<string, SqlColumndDetails> dnLoadSchema = null;
//        private class SqlColumndDetails
//        {
//            public string ColumnName { get; set; }
//            public string DataType { get; set; }
//            public short Length { get; set; }
//        }

//        private Dictionary<string, SqlColumndDetails> DNLoadFieldLengthSchema
//        {
//            get
//            {
//                if (dnLoadSchema == null)
//                {
//                    dnLoadSchema = new Dictionary<string, SqlColumndDetails>();

//                    using (DataTable dt = this.GetWarehouseDBConnection().ExecuteDataTableQuery(@"
//select 
//	c.name, t.name type, c.length, c.prec,c.scale, c.cdefault, c.isnullable, (c.colstat & 1) as isIdentity, ident_seed(object_name(c.id)) seed, 
//	ident_incr(object_name(c.id)) incr,m.text, '' processed 
//from 
//	syscolumns c 
//	left outer join systypes t on c.xtype = t.xtype 
//	left outer join syscomments m on c.cdefault = m.id 
//where 
//	t.name != 'sysname' and object_name(c.id) ='DNLOAD'"))
//                        foreach (DataRow row in dt.Rows)
//                            if (row["type"].ToString() == "nvarchar")
//                                dnLoadSchema[row["name"].ToString()] = new SqlColumndDetails
//                                {
//                                    ColumnName = row["name"].ToString(),
//                                    DataType = row["type"].ToString(),
//                                    Length = (short)row["prec"],
//                                };
//                            else
//                                dnLoadSchema[row["name"].ToString()] = new SqlColumndDetails
//                                {
//                                    ColumnName = row["name"].ToString(),
//                                    DataType = row["type"].ToString(),
//                                    Length = (short)row["length"],
//                                };
//                }

//                return dnLoadSchema;
//            }
//        }
		#endregion

        //Properties
		#region Application Versions
        [DescriptionAttribute("Application Version")]
        [CategoryAttribute("Application Versions")]
        public String ApplicationVersion
        {
            get
            {
                return "1.0.0.0";
            }
        }

        [DescriptionAttribute("Warehouse Middle Tier Target Version")]
        [CategoryAttribute("Application Versions")]
        public string WarehouseMiddleTierTargetVersion
        {
            get
            {
                return "64.1.2600.0";
            }
        }
		#endregion

        #region Business
        [DescriptionAttribute("Defines whether the integration uses SQL or the Web Service to upload data from A1W to NAV.")]
        [CategoryAttribute("Business")]
        public TransportMethod UploadTransportMethod { get; set; }

        [DescriptionAttribute("Whether to delete successful records from upload table or not")]
        [CategoryAttribute("Business")]
        public bool AutoPurgeRecordsOnUpload { get; set; }
        #endregion

        #region NAV Connection Setup
        [DescriptionAttribute("Company database ID to use for this session. The company database ID must be a valid ID set up in NAV.")]
        [CategoryAttribute("NAV Connection Setup")]
        public String CompanyId { get; set; }

        [DescriptionAttribute("Connection string to the NAV database (for SQL transport).")]
        [CategoryAttribute("NAV Connection Setup")]
        public String NAVDBConnectionString { get; set; }

        [DescriptionAttribute("NAV Web service connection string.")]
        [CategoryAttribute("NAV Connection Setup")]
        public String Url { get; set; }

        [DescriptionAttribute("NAV Web service username.")]
        [CategoryAttribute("NAV Connection Setup")]
        public String Username { get; set; }

        [DescriptionAttribute("NAV Web service password.")]
        [CategoryAttribute("NAV Connection Setup")]
        [PasswordPropertyText(true)]
        public String Password { get; set; }

        [DescriptionAttribute("NAV Web service domain.")]
        [CategoryAttribute("NAV Connection Setup")]
        public String Domain { get; set; }

        #endregion

        #region WMS Connection Setup
        [DescriptionAttribute("Client Name to be used by the WMS in a 3PL setup.")]
        [CategoryAttribute("WMS Connection Setup")]
        public String ClientName { get; set; }

        [DescriptionAttribute("Connection String to WMS database.")]
        [CategoryAttribute("WMS Connection Setup")]
        public String WarehouseDBConnectionString { get; set; }
        #endregion
		
		#region Createch Custom Parameters
		/*
		[DescriptionAttribute("Warehouses that uses SAIL Clientname")]
        [CategoryAttribute("Createch Custom Parameters")]
        public String SAIL_ClientWhList { get; set; }
		
		[DescriptionAttribute("Warehouses that uses SPORTIUM Clientname")]
        [CategoryAttribute("Createch Custom Parameters")]
        public String SPORTIUM_ClientWhList { get; set; }
		*/
		#endregion
		
	}

	#region AdjustmentMap
	public class AdjustmentMap
	{
		public AdjustmentMap()
		{
			AdjustmentTypes = new List<string>();
            
		}

		#region Business
		[Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
		[DescriptionAttribute("List of adjustments to be uploded from WMS for this account number")]
		[CategoryAttribute("Business")]
		public List<string> AdjustmentTypes { get; set; }

		[DescriptionAttribute("Account number")]
		[CategoryAttribute("Business")]
		public String AccountNumber { get; set; }
		#endregion

		#region ToString
		public override String ToString()
		{
			return this.AccountNumber;
		}
		#endregion
	}
	#endregion
	
}

