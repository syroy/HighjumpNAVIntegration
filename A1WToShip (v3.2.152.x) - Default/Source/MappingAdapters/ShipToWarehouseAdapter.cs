using Accellos.Ship.MT.Common.Constants;
using Accellos.Ship.MT.Common.DTO;
using Accellos.Warehouse.MT.Common.DTO;
using HighJump.Integrations.IntegrationUtils;
using HighJump.Integrations.ShipUtils;
using HighJump.Integrations.WarehouseUtils;
using HighJump.Integrations.WarehouseUtils.Adapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighJump.Integrations.WarehouseToShip.MappingAdapters
{
    public static class ShipToWarehouseAdapter
    {
        #region ConvertToWarehouseDTO (Rate and Manifest, Pre-Print)
        public static TOTMASTR ConvertToWarehouseDTO(Container container, TOTMASTR totMastr, TotMastrActions currentTotMastrAction, TotMastrActions newTotMastrAction)
        {
            if (totMastr.ACTION != (short)currentTotMastrAction)
                return totMastr;

            return ConvertToWarehouseDTO(container, totMastr, newTotMastrAction);
        }

        public static TOTMASTR ConvertToWarehouseDTO(Container container, TOTMASTR totMastr, TotMastrActions newTotMastrAction)
        {
            #region Label
            if (container.Labels != null && container.Labels.Count > 0)
            {
                IEnumerable<byte> labelImage = null;

                foreach (Label label in container.Labels)
                    labelImage = labelImage == null ? Convert.FromBase64String(label.LabelBuffer) : labelImage.Concat(Convert.FromBase64String(label.LabelBuffer));

                totMastr.LABEL_IMAGE = ShipIntegrationConfig.Instance.UseBase64Labels ?
                    Convert.ToBase64String(labelImage.ToArray()) : Encoding.UTF8.GetString(labelImage.ToArray());
            }
            #endregion

            totMastr.ACTION = (short)newTotMastrAction;

            #region Shipment Costs
            if (container.Shipment.ShipmentCostDetails != null && container.Shipment.ShipmentCostDetails.Count > 0)
            {
				ShipmentCostDetail overriddenFreightCost = container.Shipment.ShipmentCostDetails.FirstOrDefault(c => c.Type == (int)CostDetailType.OverriddenFreightCost);
				ShipmentCostDetail listedFreightCost = container.Shipment.ShipmentCostDetails.FirstOrDefault(c => c.Type == (int)CostDetailType.ListedFreightCost);
				ShipmentCostDetail freightCost = null;

				if (overriddenFreightCost == null) // Standard logic
				{
					freightCost = container.Shipment.ShipmentCostDetails.FirstOrDefault(c => c.Type == (int)CostDetailType.TotalFreightCost);

					if (freightCost != null && listedFreightCost != null)
						totMastr.CHRG_SHIP = ShipIntegrationConfig.Instance.PassOnFreightDiscount ? freightCost.Value : listedFreightCost.Value;
				}
				else // Override logic
				{
					freightCost = overriddenFreightCost;
					totMastr.CHRG_SHIP = overriddenFreightCost.Value;
				}

				if (freightCost != null)
				{
					totMastr.BAS_CHARGE = freightCost.Value;
					totMastr.TOTAL_CHG = freightCost.Value;
					totMastr.EST_CHARGE = freightCost.Value;
				}
			}
            #endregion

            #region Carrier/Service
            if (container.Shipment.CarrierServiceCode.Contains(";"))
            {
                totMastr.CARRIER = container.Shipment.CarrierServiceCode.Substring(0, container.Shipment.CarrierServiceCode.IndexOf(";"));
                totMastr.CARR_SRVC = StringUtils.Left(container.Shipment.CarrierServiceCode.Substring(container.Shipment.CarrierServiceCode.IndexOf(";") + 1), 25);
            }
            else
            {
                totMastr.CARRIER = container.Shipment.CarrierServiceCode;
                totMastr.CARR_SRVC = string.Empty;
            }

            totMastr.CARRIER = StringUtils.Left(totMastr.CARRIER, 10);
            #endregion

            totMastr.DATE_SHIP = container.Shipment.ShipDate;
            totMastr.ERR_CODE = string.Empty;

            if (container.Shipment.ChildShipmentXRefChilds != null && container.Shipment.ChildShipmentXRefChilds.Count > 0)
                totMastr.MASTER_BOL = StringUtils.Left(container.Shipment.ChildShipmentXRefChilds[0].Shipment.BOLNumber, 20);

            totMastr.MAN_NUMBER = StringUtils.Left(container.Shipment.BOLNumber, 20);
            totMastr.SHIPFIELD1 = StringUtils.Left(container.ModifiedBy, 30);

            #region Tracking and Pro Number Handling
            totMastr.SHIP_NUM = StringUtils.Left(string.IsNullOrWhiteSpace(container.Shipment.ProNumber) ? container.Shipment.ShipmentNumber : container.Shipment.ProNumber, 22);

            if (newTotMastrAction == TotMastrActions.PreRateComplete)
                totMastr.SHIP_NUM = string.Empty;
            else if (newTotMastrAction == TotMastrActions.VoidComplete)
            {
                if (totMastr.SHIP_NUM == container.Shipment.ShipmentNumber)
                    totMastr.SHIP_NUM = string.Empty;

                totMastr.SHIPMENT = string.Empty;
                totMastr.TRACKTRACE = container.Shipment.VoidId;
            }
            else
            {
                totMastr.SHIPMENT = StringUtils.Left(container.Shipment.ShipmentNumber, 22);

                if (string.IsNullOrWhiteSpace(container.TrackingNumber))
                    totMastr.TRACKTRACE = container.Shipment.ProNumber;
                else if (ShipIntegrationConfig.Instance.UseLastMileTrackingNumber && !string.IsNullOrWhiteSpace(container.LastMileTrackingNumber))
                {
                    totMastr.CAPTUREDBC = container.TrackingNumber;
                    totMastr.TRACKTRACE = container.LastMileTrackingNumber;
                }
                else
                {
                    totMastr.CAPTUREDBC = string.IsNullOrWhiteSpace(container.ReturnTrackingNumber) ? container.LastMileTrackingNumber : container.ReturnTrackingNumber;
                    totMastr.TRACKTRACE = container.TrackingNumber;
                }
            }

            totMastr.CAPTUREDBC = StringUtils.Left(totMastr.CAPTUREDBC, 30);
            totMastr.TRACKTRACE = StringUtils.Left(totMastr.TRACKTRACE, 34);
            #endregion

            if (newTotMastrAction != TotMastrActions.PrePrintComplete)
            {
                if (WarehouseIntegrationConfig.Instance.UseContainerWeights)
                {
                    totMastr.WEIGHT_PC = "1";
                    totMastr.WEIGHT_SHP = container.Weight;
                }
                else
                    totMastr.WEIGHT_SHP = container.Shipment.Containers.Sum(c => c.Weight ?? 0m);
            }

            return totMastr;
        }
        #endregion
    }
}
