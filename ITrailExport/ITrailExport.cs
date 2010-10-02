using System;
using System.Collections.Generic;
using System.Text;
using ZoneFiveSoftware.Common.Data;
using ZoneFiveSoftware.Common.Data.Fitness;

namespace ITrailExport
{
    public interface ITrailResult
    {
        int Order { get; }

        TimeSpan StartTime { get; }

        TimeSpan EndTime { get; }

        TimeSpan Duration { get; }

        string Distance { get; }

        float AvgCadence { get; }

        float AvgHR { get; }

        float MaxHR { get; }

        float AvgPower { get; }

        float AvgGrade { get; }

        float AvgSpeed { get; }

        float FastestSpeed { get; }

        double AvgPace { get; }

        double FastestPace { get; }

        string ElevChg { get; }

        IDistanceDataTrack DistanceMetersTrack { get; }

        INumericTimeDataSeries ElevationMetersTrack { get; }

        IActivityCategory Category { get; }

        INumericTimeDataSeries CopyTrailTrack(INumericTimeDataSeries source);

        INumericTimeDataSeries CadencePerMinuteTrack { get; }

        INumericTimeDataSeries HeartRatePerMinuteTrack { get; }

        INumericTimeDataSeries PowerWattsTrack { get; }

        INumericTimeDataSeries SpeedTrack { get; }

        INumericTimeDataSeries PaceTrack { get; }

        INumericTimeDataSeries GradeTrack { get; }
    }
}