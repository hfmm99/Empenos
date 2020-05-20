using Empeños.Controles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Empeños.Clases
{
    public class BulletGraphGaugeViewModel : AttachedViewModelBase
    {
        public BulletGraphGaugeViewModel()
        {
        }

        private GaugeControl Gauge
        {
            get { return DataContext != null ? (GaugeControl)DataContext : null; }
        }

        public IEnumerable<RangeItem> Ranges
        {
            get
            {
                if (Gauge == null)
                    yield break;

                for (int i = 0; i < Gauge.QualitativeRange.Count; i++)
                {
                    var range = Gauge.QualitativeRange[i];
                    if (i == 0)
                    {
                        // first RangeItem, width is determined from the first range value
                        yield return new RangeItem()
                        {
                            Color = range.Color,
                            Width = ValueToWidth(range.Maximum - Gauge.Minimum)
                        };
                    }
                    else
                    {
                        // subsequent items, width computed as the difference between the
                        // current value and its predecessor
                        var previousRange = Gauge.QualitativeRange[i - 1];
                        yield return new RangeItem()
                        {
                            Color = range.Color,
                            Width = ValueToWidth(range.Maximum - previousRange.Maximum)
                        };
                    }
                }
            }
        }

        public IEnumerable<Tick> MajorTicks
        {
            get
            {
                if (Gauge == null)
                    yield break;

                double tickSpacing = (Gauge.Maximum - Gauge.Minimum) / 10;
                for (double tick = Gauge.Minimum; tick <= Gauge.Maximum; tick += tickSpacing)
                {
                    yield return new Tick()
                    {
                        Position = ValueToWidth(tick - Gauge.Minimum),
                        Label = tick.ToString("N0")
                    };
                }
            }
        }

        public IEnumerable<double> MinorTicks
        {
            get
            {
                if (Gauge == null)
                    yield break;

                double tickSpacing = (Gauge.Maximum - Gauge.Minimum) / 50;
                for (double tick = Gauge.Minimum; tick <= Gauge.Maximum; tick += tickSpacing)
                {
                    yield return ValueToWidth(tick - Gauge.Minimum);
                }
            }
        }

        public double FeaturedMeasureLength
        {
            get
            {
                if (Gauge == null)
                    return 0;

                return ValueToWidth(Gauge.Value - Gauge.Minimum);
            }
        }

        /// <summary>
        /// Converts the given value (whcih should be between Gauge.Maximum / Gauge.Minimum)
        /// into suitabel width for rendering within the view.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private double ValueToWidth(double value)
        {
            double range = Gauge.Maximum - Gauge.Minimum;
            return (value) / range * ElementWidth;
        }

        protected override void AdaptedDataContextPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                // ValueAngle depends on value
                OnPropertyChanged("FeaturedMeasureLength");
            }
            else
            {
                // otherwise fire a generic property changed
                OnPropertyChanged("");
            }
        }

        protected override void AttachedElement_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            base.AttachedElement_SizeChanged(sender, e);
        }

        public class RangeItem
        {
            public double Width { get; set; }
            public Color Color { get; set; }
        }

        public class Tick
        {
            public double Position { get; set; }
            public string Label { get; set; }
        }
    }
}
