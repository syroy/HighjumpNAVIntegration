using System;
using Accellos.Interfaces.NAV.BusinessObjects;

namespace Accellos.Interfaces.NAV.NAVAdapters
{
    public class NAVAdapterUtils
    {
        #region String Conversions
		public static String CreatechNAVFormatSubString(String myValue, int myLength, int myStartPos)
        {
            int myValueLength;

            if (myValue == "System.String[]")
                return " ";
            else
            {
                if (!string.IsNullOrEmpty(myValue))
                {
                    myValueLength = myValue.Length;					
                    if (myValueLength-myStartPos < myLength)
                    {
                        myLength = myValueLength-myStartPos;
                    }

                    if (myLength > 0)
                    {
                       return myValue.ToString().Substring(myStartPos, myLength);
                    }
                    else
                        return " ";
                }
                else
                    return " ";
            }
        }
		
        public static String NAVFormatString(String myValue, int myMaxLength)
        {
            int myValueLength;
            int myFirstPosition = 0;

            if (myValue == "System.String[]")
                return " ";
            else
            {
                if (!string.IsNullOrEmpty(myValue))
                {
                    myValueLength = myValue.Length;
                    if (myValueLength < myMaxLength)
                    {
                        myMaxLength = myValueLength;
                    }

                    if (myMaxLength > 0)
                    {
                        myFirstPosition = 0;
                        return myValue.ToString().Substring(myFirstPosition, myMaxLength);
                    }
                    else
                        return " ";
                }
                else
                    return " ";
            }
        }

        /// <summary>
        /// Converts a NAV string to char value
        /// </summary>
        /// <param name="myValue">
        /// String to convert a char
        /// </param>
        /// <returns>
        /// Char version of the string
        /// </returns>
        /// <remarks>
        /// 11/06/2012 - [6.4] Initial Version
        /// </remarks>
        public static char NAVFormatChar(String myValue)
        {
            char myResult = ' ';
            if (myValue == "System.String[]")
                return myResult;
            else
            {
                if (!string.IsNullOrEmpty(myValue))
                {
                    try
                    {
                        myResult = Convert.ToChar(myValue);
                    }
                    catch
                    {
                        myResult = ' ';
                    }
                }
            }
            return myResult;
        }

        public static Decimal NAVFormatDecimal(String myValue)
        {
            Decimal myResult = 0;
            if (myValue == "System.String[]")
                return myResult;
            else
            {
                if (!string.IsNullOrEmpty(myValue))
                    try
                    {
                        myResult = Convert.ToDecimal(myValue);
                    }
                    catch
                    {
                        myResult = 0;
                    }
                else
                    return myResult;
            }
            return myResult;
        }

        public static int NAVFormatInt(String myValue)
        {
            int myResult = 0;
            if (myValue == "System.String[]")
                return myResult;
            else
            {
                if (!string.IsNullOrEmpty(myValue))
                    try
                    {
                        myResult = Convert.ToInt32(myValue);
                    }
                    catch
                    {
                        myResult = 0;
                    }
                else
                    return myResult;
            }
            return myResult;
        }

        public static Boolean NAVFormatBoolean(String myValue)
        {
            Boolean myResult = false;

            if (myValue == "System.String[]")
                return myResult;
            else
            {
                if (!string.IsNullOrEmpty(myValue))
                {
                    if (myValue.Substring(0, 1).ToString() == "1")
                        myResult = true;

                    if (myValue.Substring(0, 1).ToString() == "T")
                        myResult = true;

                    if (myValue.Substring(0, 1).ToString() == "t")
                        myResult = true;
                }
            }
            return myResult;
        }

        public static String[] NAVFormatArray(String myValue, int intMax)
        {
            String[] myArray = new String[40];
            int intIndex = 0;
            int intMyValueLength = myValue.Length;
            if (intMyValueLength < intMax)
                intMax = intMyValueLength;

            while (intIndex <= intMax - 1)
            {
                myArray[intIndex] = myValue.Substring(intIndex, 1);
                intIndex += 1;
            }

            return myArray;

        }
        # endregion

        #region Type Conversions

        public static TrackingType NAVFormatTrackingType(String myValue)
        {
            TrackingType myResult = TrackingType.NotTracked;

            if (myValue == "System.String[]")
                return myResult;
            else
            {
                if (!string.IsNullOrEmpty(myValue))
                {
                    if (myValue.Substring(0, 1).ToString() == "0")
                        myResult = TrackingType.NotTracked;

                    if (myValue.Substring(0, 1).ToString() == "1")
                        myResult = TrackingType.NonUnique;

                    if (myValue.Substring(0, 1).ToString() == "2")
                        myResult = TrackingType.Unique;
                }
            }
            return myResult;
        }

        public static StockItemIndicatorType NAVFormatStockItemIndicatorType(String myValue)
        {
            StockItemIndicatorType myResult = StockItemIndicatorType.NotSpecified;

            if (myValue == "System.String[]")
                return myResult;
            else
            {
                if (!string.IsNullOrEmpty(myValue))
                {
                    if (myValue.Substring(0, 1).ToString() == "0")
                        myResult = StockItemIndicatorType.StockItem;

                    if (myValue.Substring(0, 1).ToString() == "1")
                        myResult = StockItemIndicatorType.NonStockItem;
                }
            }
            return myResult;
        }

        public static DateTime NAVFormatDateTime(String myValue)
        {
            DateTime myResult = System.DateTime.Now.Date;

            if (myValue == "System.String[]")
                return myResult;
            else
            {
                if (!string.IsNullOrEmpty(myValue))
                {
                    try
                    {
                        System.DateTime tempDateTime = System.DateTime.Parse(myValue);
                        myResult = tempDateTime.Date;
                    }
                    catch
                    {
                        return myResult;
                    }
                }
            }
            return myResult;
        }

        public static PoType NAVFormatPOType(String myValue)
        {
            PoType myResult = PoType.NotSpecified;

            if (myValue == "System.String[]")
                return myResult;
            else
            {
                if (!string.IsNullOrEmpty(myValue))
                    if (myValue.ToString() == "TRANSFER")
                        myResult = PoType.Transfer;
                if (myValue.ToString() == "REQUISITION")
                    myResult = PoType.Requisition;
            }
            return myResult;

        }

        public static CartonType NAVFormatCartonType(String myValue)
        {
            CartonType myResult = CartonType.NotSpecified;

            if (myValue == "System.String[]")
                return myResult;
            else
            {
                if (!string.IsNullOrEmpty(myValue))
                {
                    if (myValue.Substring(0, 1).ToString() == "0")
                        myResult = CartonType.CartonLabel;

                    if (myValue.Substring(0, 1).ToString() == "1")
                        myResult = CartonType.PalletLabel;

                    if (myValue.Substring(0, 1).ToString() == "2")
                        myResult = CartonType.TruckLoadLabel;

                }
                else
                {
                    myResult = CartonType.NotSpecified;
                }
            }
            return myResult;
        }

        public static PurchaseOrderType NAVFormatPurchaseOrderType(String myValue)
        {
            PurchaseOrderType myResult = PurchaseOrderType.NotSpecified;

            if (myValue == "System.String[]")
                return myResult;
            else
            {
                if (!string.IsNullOrEmpty(myValue))
                {
                    if (myValue.Substring(0, 1).ToString() == "SA")
                        myResult = PurchaseOrderType.StandAlone;

                    if (myValue.Substring(0, 1).ToString() == "PR")
                        myResult = PurchaseOrderType.Promotional;

                    if (myValue.Substring(0, 1).ToString() == "OS")
                        myResult = PurchaseOrderType.CustomerSpecialOrder;

                    if (myValue.Substring(0, 1).ToString() == "RE")
                        myResult = PurchaseOrderType.ReOrder;

                    if (myValue.Substring(0, 1).ToString() == "TRANSFER")
                        myResult = PurchaseOrderType.Transfer;
                }
                else
                {
                    myResult = PurchaseOrderType.NotSpecified;
                }
            }
            return myResult;

        }

        public static ShipmentPaymentType NAVFormatShipmentPaymentType(String myValue)
        {
            ShipmentPaymentType myResult = ShipmentPaymentType.NotSpecified;

            if (myValue == "System.String[]")
                return myResult;
            else
            {
                if (!string.IsNullOrEmpty(myValue))
                {
                    if (myValue.Substring(0, 1).ToString() == "C")
                        myResult = ShipmentPaymentType.Collect;

                    if (myValue.Substring(0, 1).ToString() == "R")
                        myResult = ShipmentPaymentType.BillRecipient;

                    if (myValue.Substring(0, 1).ToString() == "P")
                        myResult = ShipmentPaymentType.Prepaid;

                    if (myValue.Substring(0, 1).ToString() == "T")
                        myResult = ShipmentPaymentType.ThirdParty;
                    
                    if (myValue.Substring(0, 1).ToString() == "G")
                        myResult = ShipmentPaymentType.DutyPaidThirdParty;
                }
                else
                {
                    myResult = ShipmentPaymentType.NotSpecified;
                }
            }
            return myResult;
        }

        public static KitType NAVFormatKitType(String myValue)
        {
            KitType myResult = KitType.NotSpecified;

            if (myValue == "System.String[]")
                return myResult;
            else
            {
                if (!string.IsNullOrEmpty(myValue))
                {
                    if (myValue.Substring(0, 1).ToString() == "0")
                        myResult = KitType.BuildToStockWorkOrder;

                    if (myValue.Substring(0, 1).ToString() == "1")
                        myResult = KitType.BuildToOrderWorkOrder;

                    if (myValue.Substring(0, 1).ToString() == "2")
                        myResult = KitType.ExplodeForPicking;

                    if (myValue.Substring(0, 1).ToString() == "3")
                        myResult = KitType.ExplodeForPickingWithShortedComponents;

                    if (myValue.Substring(0, 1).ToString() == "4")
                        myResult = KitType.RecipeOnly;
                }
                else
                {
                    myResult = KitType.NotSpecified;
                }
            }
            return myResult;
        }

        public static ReservationType NAVFormatReservationType(String myValue)
        {
            ReservationType myResult = ReservationType.NotSpecified;

            if (myValue == "System.String[]")
                return myResult;
            else
            {
                if (!string.IsNullOrEmpty(myValue))
                {
                    if (myValue.Substring(0, 1).ToString() == "S")
                        myResult = ReservationType.SeasonalReservation;

                    if (myValue.Substring(0, 1).ToString() == "C")
                        myResult = ReservationType.CoreReservation;

                }
                else
                {
                    myResult = ReservationType.NotSpecified;
                }
            }
            return myResult;
        }

        public static VariableLengthType NAVFormatVariableLengthType(String myValue)
        {
            VariableLengthType myResult = VariableLengthType.NotVLP;

            if (myValue == "System.String[]")
                return myResult;
            else
            {
                if (!string.IsNullOrEmpty(myValue))
                {
                    if (myValue.Substring(0, 1).ToString() == "Y")
                        myResult = VariableLengthType.VLP;

                    if (myValue.Substring(0, 1).ToString() == "C")
                        myResult = VariableLengthType.ContinuousVLP;

                }
                else
                {
                    myResult = VariableLengthType.NotVLP;
                }
            }
            return myResult;
        }


        public static ComponentType NAVFormatComponentType(String myValue)
        {
            ComponentType myResult = ComponentType.NotSpecified;

            if (myValue == "System.String[]")
                return myResult;
            else
            {
                if (!string.IsNullOrEmpty(myValue))
                {
                    if (myValue.Substring(0, 1).ToString() == "S")
                        myResult = ComponentType.KitSubcomponent;

                    if (myValue.Substring(0, 1).ToString() == "C")
                        myResult = ComponentType.CoProduct;

                    if (myValue.Substring(0, 1).ToString() == "A")
                        myResult = ComponentType.ServiceItem;

                }
                else
                {
                    myResult = ComponentType.NotSpecified;
                }
            }
            return myResult;
        }
 
        # endregion

        public static String getClientName()
        {
            String is3pl = CompanyConfig.Instance.GetWarehouseDBConnection().ExecuteScalar("select TOP 1 C_VALUE from config nolock where config = '3PL_EDITION'").ToString();
            String retVal = String.Empty;
            if (is3pl == "Y")   //6.4 moved from below
            {
                retVal = CompanyConfig.Instance.ClientName;  //6.4 changed from myNAVItem to CompanyConfig, as it'll be the same for a given integration (and requires less maintanence)
            }

            return retVal;
        }
    }
	public enum NAVStatusType
	{
		Empty = 0,
		ReadyToDownload = 1,
		Downloaded = 2,
		DNRejected = 3,
		BeingAllocated = 4,
		BeingUnalloc = 5,
		BeingPicked = 6,
		WaitingToPick = 7,
		Nuked = 8,
		BeingReceived = 9,
		Uploaded = 10,
		Proceed = 11
	}
 
}

