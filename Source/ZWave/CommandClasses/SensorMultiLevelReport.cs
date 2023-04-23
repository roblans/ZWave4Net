using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZWave.Channel;
using ZWave.Channel.Protocol;

namespace ZWave.CommandClasses
{
    public class SensorMultiLevelReport : NodeReport
    {
        public readonly SensorType Type;
        public readonly float Value;
        public readonly string Unit;
        public readonly byte Scale;

        internal SensorMultiLevelReport(Node node, byte[] payload) : base(node)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));
            if (payload.Length < 3)
                throw new ReponseFormatException($"The response was not in the expected format. {GetType().Name}: Payload: {BitConverter.ToString(payload)}");

            Type = (SensorType)payload[0];
            Value = PayloadConverter.ToFloat(payload.Skip(1).ToArray(), out Scale);
            Unit = GetUnit(Type, Scale);
        }

        private static string GetUnit(SensorType type, byte scale)
        {
            var tankCapacityUnits = new[] { "l", "cbm", "gal" };
            var distanceUnits = new [] { "m", "cm", "ft" };
            var seismicIntensityUnits = new[] { "Mercalli", "European Macroseismic", "Liedu", "Shindo" };
            var seismicMagnitudeUnits = new[] { "Local", "Moment", "Surface Wave", "Body Wave" };
            var moistureUnits = new[] { "%", "m3/m3", "kΩ", "aW" };

            switch (type)
            {
                case SensorType.Temperature: return(scale == 1 ? "°F" : "°C");
                case SensorType.General: return (scale == 1 ? "" : "%");
                case SensorType.Luminance: return(scale == 1 ? "lux" : "%");
                case SensorType.Power: return(scale == 1 ? "BTU/h" : "W");
                case SensorType.RelativeHumidity: return("%");
                case SensorType.Velocity: return(scale == 1 ? "mph" : "m/s");
                case SensorType.Direction: return("");
                case SensorType.AtmosphericPressure: return(scale == 1 ? "inHg" : "kPa");
                case SensorType.BarometricPressure: return(scale == 1 ? "inHg" : "kPa");
                case SensorType.SolarRadiation: return("W/m2");
                case SensorType.DewPoint: return(scale == 1 ? "F" : "C");
                case SensorType.RainRate: return(scale == 1 ? "in/h" : "mm/h");
                case SensorType.TideLevel: return(scale == 1 ? "ft" : "m");
                case SensorType.Weight: return(scale == 1 ? "lb" : "kg");
                case SensorType.Voltage: return(scale == 1 ? "mV" : "V");
                case SensorType.Current: return(scale == 1 ? "mA" : "A");
                case SensorType.CO2: return("ppm");
                case SensorType.AirFlow: return(scale == 1 ? "cfm" : "m3/h");
                case SensorType.TankCapacity: return(tankCapacityUnits[scale]);
                case SensorType.Distance: return(distanceUnits[scale]);
                case SensorType.Rotation: return (scale == 1 ? "Hz" : "rpm");
                case SensorType.WaterTemperature: return (scale == 1 ? "F" : "C");
                case SensorType.SoilTemperature: return (scale == 1 ? "F" : "C");
                case SensorType.SeismicIntensity: return (seismicIntensityUnits[scale]);
                case SensorType.SeismicMagnitude: return (seismicMagnitudeUnits[scale]);
                case SensorType.ElectricalResistivity: return ("Ωm");
                case SensorType.ElectricalConductivity: return ("S/m");
                case SensorType.Loudness: return (scale == 1 ? "dBA" : "dB");
                case SensorType.Moisture: return (moistureUnits[scale]);
                case SensorType.Frequency: return (scale == 1 ? "kHz" : "Hz");
                case SensorType.Time: return ("s");
                case SensorType.TargetTemperature: return (scale == 1 ? "F" : "C");
                case SensorType.ParticulateMatter25: return (scale == 1 ? "ug/m3" : "mol/m3");
                case SensorType.FormaldehydeLevel: return ("mol/m3");
                case SensorType.RadonConcentration: return (scale == 1 ? "pCi/L" : "bq/m3");
                case SensorType.MethaneDensity: return ("mol/m3");
                case SensorType.VolatileOrganicCompoundLevel: return (scale == 1 ? "ppm" : "mol/m3");
                case SensorType.CarbonMonoxideLevel: return (scale == 1 ? "ppm" : "mol/m3");
                case SensorType.SoilHumidity: return ("%");
                case SensorType.SoilReactivity: return ("pH");
                case SensorType.SoilSalinity: return ("mol/m3");
                case SensorType.HeartRate: return ("bpm");
                case SensorType.BloodPressure: return (scale == 1 ? "Diastolic (mmHg)" : "Systolic (mmHg)");
                case SensorType.MuscleMass: return ("kg");
                case SensorType.FatMass: return ("kg");
                case SensorType.BoneMass: return ("kg");
                case SensorType.TotalBodyWater: return ("kg");
                case SensorType.BasisMetabolicRate: return ("BMR");
                case SensorType.BodyMassIndex: return ("BMI");
                case SensorType.AccelerationXAxis: return ("m/s2");
                case SensorType.AccelerationYAxis: return ("m/s2");
                case SensorType.AccelerationZAxis: return ("m/s2");
                case SensorType.SmokeDensity: return ("%");
                case SensorType.WaterFlow: return ("L/h");
                case SensorType.WaterPressure: return ("kPa");
                case SensorType.RFSignalStrength: return (scale == 1 ? "dBm" : "RSSI");
                case SensorType.ParticulateMatter10: return (scale == 1 ? "ug/m3" : "mol/m3");
                case SensorType.RespiratoryRate: return ("bpm");
                case SensorType.RelativeModulationLevel: return ("%");
                case SensorType.BoilerWaterTemperature: return (scale == 1 ? "F" : "C");
                case SensorType.DomesticHotWaterTemperature: return (scale == 1 ? "F" : "C");
                case SensorType.OutsideTemperature: return (scale == 1 ? "F" : "C");
                case SensorType.ExhaustTemperature: return (scale == 1 ? "F" : "C");
                case SensorType.WaterAcidity: return ("mg/L");
                case SensorType.WaterChlorineLevel: return ("pH");
                case SensorType.WaterOxidationReductionPotential: return ("mV");
                case SensorType.AppliedForceOnTheSensor: return ("N");
                case SensorType.ReturnAirTemperature: return (scale == 1 ? "F" : "C");
                case SensorType.SupplyAirTemperature: return (scale == 1 ? "F" : "C");
                case SensorType.EvaporatorCoilTemperature: return (scale == 1 ? "F" : "C");
                case SensorType.CondenserCoilTemperature: return (scale == 1 ? "F" : "C");
                case SensorType.LiquidLineTemperature: return (scale == 1 ? "F" : "C");
                case SensorType.DischargeLineTemperature: return (scale == 1 ? "F" : "C");
                case SensorType.SuctionPressure: return (scale == 1 ? "psi" : "kPa");
                case SensorType.DischargePressure: return (scale == 1 ? "psi" : "kPa");
                case SensorType.DefrostTemperature: return (scale == 1 ? "F" : "C");
                case SensorType.Ozone: return ("ug/m3");
                case SensorType.SulfurDioxide: return ("ug/m3");
                case SensorType.NitrogenDioxide: return ("ug/m3");
                case SensorType.Ammonia: return ("ug/m3");
                case SensorType.Lead: return ("ug/m3");
                case SensorType.ParticulateMatter1: return ("ug/m3");
                default: return string.Empty;
            }
        }

        public override string ToString()
        {
            return $"Type:{Type}, Value:\"{Value} {Unit}\"";
        }
    }
}
