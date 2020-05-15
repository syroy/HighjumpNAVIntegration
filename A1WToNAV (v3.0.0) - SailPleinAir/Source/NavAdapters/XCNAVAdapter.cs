using System;
using System.Collections.Generic;
using System.Text;
using Accellos.InterfaceFramework;
using Accellos.InterfaceFramework.EventNotificationServer;
using Accellos.InterfaceFramework.ScriptManagementServer;
using Accellos.Interfaces.NAV.BusinessObjects;
using Accellos.Interfaces.NAV.NAVWebServices;
using Accellos.Interfaces.NAV.WMSAdapters;

namespace Accellos.Interfaces.NAV.NAVAdapters
{
    public class XCNAVAdapter 
    {
        #region SVA WriteToNAV
        public static List<UploadConfirmation> WriteToNAV(List<TransferConfirmation> xcRecords)
        {
            List<UploadConfirmation> uploadConfirmations = new List<UploadConfirmation>();
            UploadConfirmation myUploadConfirmation;
            String myRowID = String.Empty;

            try
            {
                NAVTransferConfirmationRoot myNAVTransferConfirmationRoot = new NAVTransferConfirmationRoot();
                List<NAVTransferConfirmation> myNAVTransferConfirmations = new List<NAVTransferConfirmation>();
                NAVTransferConfirmation myNAVTransferConfirmation;
                String myResult = String.Empty;
               
                myUploadConfirmation = new UploadConfirmation();
                
                //Build list of Confrimations
                foreach (TransferConfirmation xcRecord in xcRecords)
                {
                    try
                    {
                        //NAV XMLPort Note:  Items must be assigned in the Column Order that is defined in the XMLPort object

                        //Start with new NAV Transfer Confirmation
                        myNAVTransferConfirmation = new NAVTransferConfirmation();
                        myRowID = xcRecord.RowId.ToString();
                    
                        //Parse Receipt Confirmation Header
                        myNAVTransferConfirmation.RecordType = "XC";
                        myNAVTransferConfirmation.UploadType = "XC";
                        myNAVTransferConfirmation.OrderType = xcRecord.OrderType.ToString();
                        myNAVTransferConfirmation.OrderNumber = xcRecord.OrderNumber.ToString();
                        myNAVTransferConfirmation.CustomerNumber = xcRecord.CustomerNumber.ToString();
                        myNAVTransferConfirmation.BackorderNumber = xcRecord.BackorderNumber.ToString();
                        myNAVTransferConfirmation.NumberOfLines = xcRecord.NumberOfLines.ToString();
                        myNAVTransferConfirmation.NumberOfUnits = xcRecord.NumberOfUnits.ToString();
                        myNAVTransferConfirmation.ContainerBatchReference = xcRecord.ContainerBatchReference.ToString();
                        myNAVTransferConfirmation.Action = xcRecord.Action.ToString();
                        myNAVTransferConfirmation.Date = xcRecord.Date.ToString();
                        myNAVTransferConfirmation.Time = xcRecord.Time.ToString();
                        myNAVTransferConfirmation.Status = xcRecord.Status.ToString();
                        myNAVTransferConfirmation.PurchaseOrderNumber = xcRecord.PurchaseOrderNumber.ToString();
                        myNAVTransferConfirmation.ClientName = xcRecord.ClientName.ToString();
                        myNAVTransferConfirmation.Extra1 = xcRecord.Extra1.ToString();
                        myNAVTransferConfirmation.Extra2 = xcRecord.Extra2.ToString();
                        myNAVTransferConfirmation.Extra3 = xcRecord.Extra3.ToString();
                        myNAVTransferConfirmation.Extra4 = xcRecord.Extra4.ToString();
                        myNAVTransferConfirmation.Extra5 = xcRecord.Extra5.ToString();
                        myNAVTransferConfirmation.Nuked = xcRecord.Nuked.ToString();
                        myNAVTransferConfirmation.RowId = myRowID;

                        //Add Confirmation to the List of Confirmations
                        myNAVTransferConfirmations.Add(myNAVTransferConfirmation);
						
						//Createch 20190503++ - Changer le Order# pour Extra3. Mauvaise gestion des BO pour les PO avec un tiret
						myNAVTransferConfirmation.OrderNumber = xcRecord.Extra3.ToString();
                    }
                    catch (Exception ex)
                    {
                        myUploadConfirmation.IsSuccess = false;
                        myUploadConfirmation.RowId = myRowID;
                        myUploadConfirmation.ErrorMessage = ex.ToString();
                        uploadConfirmations.Add(myUploadConfirmation);
                    }
                }

               //Send list of Confirmations to NAV 
               myNAVTransferConfirmationRoot.NAVTransferConfirmation = myNAVTransferConfirmations.ToArray();
               myResult = CompanyConfig.Instance.NavWebServiceInstance.ImportTransferConfirmation(myNAVTransferConfirmationRoot);
               CompanyConfig.Instance.NavWebServiceInstance.Dispose();

               if (myResult == "OK")
               {
                   foreach (TransferConfirmation xcRecord in xcRecords)
                   { 
                     myUploadConfirmation.IsSuccess = true;
                     myUploadConfirmation.RowId = xcRecord.RowId;
                     myUploadConfirmation.ErrorMessage = String.Empty;
                     uploadConfirmations.Add(myUploadConfirmation);
                   }

               }
               else
               {
                   foreach (TransferConfirmation xcRecord in xcRecords) 
                   { 
                     myUploadConfirmation.IsSuccess = false;
                     myUploadConfirmation.RowId = xcRecord.RowId;
                     myUploadConfirmation.ErrorMessage = "Upload to NAV failed.";
                     uploadConfirmations.Add(myUploadConfirmation);
                   }
               }
            }
            catch (Exception ex)
            {
                myUploadConfirmation = new UploadConfirmation();
                foreach (TransferConfirmation xcRecord in xcRecords)
                {
                    myUploadConfirmation.IsSuccess = false;
                    myUploadConfirmation.RowId = xcRecord.RowId;
                    myUploadConfirmation.ErrorMessage = ex.ToString();
                    uploadConfirmations.Add(myUploadConfirmation);
                }
            }

            return uploadConfirmations;
        }
        #endregion
	}
}

