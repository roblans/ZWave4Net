
namespace ZWave.CommandClasses
{
    public enum NotificationState : ushort
    {
        Idle = 0x0,

        //Smoke
        SmokeDetected = 0x0101,
        SmokeDetectedUnknownLocation = 0x0102,
        SmokeAlarmTest = 0x0103,
        SmokeReplacementRequired = 0x0104,
        SmokeReplacementRequiredEOL = 0x0105,
        SmokeAlarmSilenced = 0x0106,
        SmokeMaintenanceRequired = 0x0107,
        DustPresent = 0x0108,

        //CO
        CODetected = 0x0201,
        CODetectedUnknownLocation = 0x0202,
        COAlarmTest = 0x0203,
        COReplacementRequired = 0x0204,
        COReplacementRequiredEOL = 0x0205,
        COAlarmSilenced = 0x0206,
        COMaintenanceRequired = 0x0207,

        //CO2
        CO2Detected = 0x0301,
        CO2DetectedUnknownLocation = 0x0302,
        CO2AlarmTest = 0x0303,
        CO2ReplacementRequired = 0x0304,
        CO2ReplacementRequiredEOL = 0x0305,
        CO2AlarmSilenced = 0x0306,
        CO2MaintenanceRequired = 0x0307,

        //Heat
        OverheatDetected = 0x0401,
        OverheatDetectedUnknownLocation = 0x0402,
        RapidRiseDetected = 0x0403,
        RapidRiseDetectedUnknownLocation = 0x0404,
        UnderheatDetected = 0x0405,
        UnderheatDetectedUnknownLocation = 0x0406,
        HeatAlarmTest = 0x0407,
        HeatReplacementRequiredEOL = 0x0408,
        HeatAlarmSilenced = 0x0409,
        DustPresentMaintenanceRequired = 0x040A,
        PeriodicMaintenanceRequired = 0x040B,
        RapidFallDetected = 0x040C,
        RapidFallDetectedUnknownLocation = 0x040D,

        //Water
        LeakDetected = 0x0501,
        LeakDetectedUnknownLocation = 0x0502,
        LevelDropDetected = 0x0503,
        LevelDropDetectedUnknownLocation = 0x0504,
        ReplaceFilter = 0x0505,
        FlowAlarm = 0x0506,
        PressureAlarm = 0x0507,
        TemperatureAlarm = 0x0508,
        LevelAlarm = 0x0509,
        SumpActive = 0x050A,
        SumpFailure = 0x050B,

        //Access Control - TODO

        //Home Security
        Intrusion = 0x0701,
        IntrusionUnknownLocation = 0x0702,
        TamperingProductCoveringRemoved = 0x0703,
        TamperingInvalidCode = 0x0704,
        GlassBreakage = 0x0705,
        GlassBreakageUnknownLocation = 0x0706,
        MotionDetection = 0x0707,
        MotionDetectionUnknownLocation = 0x0708,
        TamperingProductMoved = 0x0709,
        ImpactDetected = 0x070A,
        MagneticInterference = 0x070B,
        RFJamming = 0x070C,

        //Power Management - TODO

        //System - TODO

        //Emergency Alarm
        ContactPolice = 0x0A01,
        ContactFire = 0x0A02,
        ContactMedical = 0x0A03,
        Panic = 0x0A04,

        //Clock
        Wakeup = 0x0B01,
        TimerEnded = 0x0B02,
        TimeRemaining = 0x0B03,

        //Appliance - TODO

        //Home Health - TODO

        //Siren
        SirenActive = 0x0E01,

        //Water Valve - TODO

        //Weather - TODO

        //Irrigation - TODO

        //Gas - TODO

        //Pest Control - TODO

        //Light
        LightDetected = 0x1401,
        LightTransitioned = 0x1402,

        //Water Quality - TODO

        //Home Monitor
        HomeOccupied = 0x1601,
        HomeOccupiedUnknownLocation = 0x1602,

        Unknown = 0xFEFE
    };
}
