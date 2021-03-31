using WealthLab.Core;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace WealthLab.Indicators
{
    public class HiLoLimit : IndicatorBase
    {
        //parameterless constructor
        public HiLoLimit() : base()
        {
        }

        //for code based construction
        public HiLoLimit(BarHistory source, int period, double level, double minrange)
            : base()
        {
            Parameters[0].Value = source;
            Parameters[1].Value = period;
            Parameters[2].Value = level;
            Parameters[3].Value = minrange;
            Populate();
        }

        //static method
        public static HiLoLimit Series(BarHistory source, int period, double level, double minrange)
        {
            string key = CacheKey("HiLoLimit", period, level, minrange);
            if (source.Cache.ContainsKey(key))
                return (HiLoLimit)source.Cache[key];
            HiLoLimit hll = new HiLoLimit(source, period, level, minrange);
            source.Cache[key] = hll;
            return hll;
        }

        //name
        public override string Name
        {
            get
            {
                return "HiLoLimit";
            }
        }

        //abbreviation
        public override string Abbreviation
        {
            get
            {
                return "HiLoLimit";
            }
        }

        //description
        public override string HelpDescription
        {
            get
            {
                return "The HiLoLimit indicator developed by Dr.Koch is a lag-free limit based on highest/lowest levels.";
            }
        }

        //price pane
        public override string PaneTag
        {
            get
            {
                return "Price";
            }
        }

        //default color
        public override Color DefaultColor
        {
            get
            {
                return Color.Blue;
            }
        }

        //populate
        public override void Populate()
        {
            BarHistory source = Parameters[0].AsBarHistory;
            Int32 period = Parameters[1].AsInt;
            Double level = Parameters[2].AsDouble;
            Double minrange = Parameters[3].AsDouble;
            DateTimes = source.DateTimes;

            if (period > DateTimes.Count)
                period = DateTimes.Count;
            if (DateTimes.Count < period || period <= 0)
                return;

            var HiLoRange = Highest.Series(source.High, period) - Lowest.Series(source.Low, period);

            for (int n = 0; n < period; n++)
                Values[n] = 0;

            for (int bar = period; bar < source.Count; bar++)
            {
                double result = 0.0;

                double ls = Lowest.Series(source.Low, period)[bar];

                if (minrange == 0.0)
                {
                    result = ls + (HiLoRange[bar] * (level / 100));
                }
                else
                {
                    double l = ls;
                    double range = HiLoRange[bar];
                    double mid = l + range / 2;
                    double mrange = l * minrange / 100.0;

                    if (range < mrange)
                        range = mrange;

                    result = mid + (level / 100.0 - 0.5) * range;
                }

                Values[bar] = result;
            }
        }

        //generate parameters
        protected override void GenerateParameters()
        {
            AddParameter("Source", ParameterTypes.BarHistory, null);
            AddParameter("Period", ParameterTypes.Int32, 14);
            AddParameter("Level", ParameterTypes.Double, 2);
            AddParameter("Min Range", ParameterTypes.Double, 2);
        }
    }
}