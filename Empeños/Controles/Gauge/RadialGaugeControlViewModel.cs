using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Empeños.Controles
{
    public class RadialGaugeControlViewModel : AttachedViewModelBase
    {
        public RadialGaugeControlViewModel()
        {
            SweepAngle = 300;
            Clip = true;
        }

        /// <summary>
        /// Gets / sets the sweep angle of the radial gauge
        /// </summary>
        public double SweepAngle
        { get; set; }

        /// <summary>
        /// Gets / sets whether the view model should apply a clipping
        /// geometry to the Grid it is attached to
        /// </summary>
        public new bool Clip
        { get; set; }

        private GaugeControl Gauge
        {
            get { return DataContext != null ? (GaugeControl)DataContext : null; }
        }

        public double ValueAngle
        {
            get
            {
                if (Gauge == null)
                    return 0.0;

                return ValueToAngle(Gauge.Value);
            }
        }

        private double ValueToAngle(double value)
        {
            double minAngle = -SweepAngle / 2;
            double maxAngle = SweepAngle / 2;
            double angularRange = maxAngle - minAngle;

            return (value - Gauge.Minimum) / (Gauge.Maximum - Gauge.Minimum) *
               angularRange + minAngle;
        }

        public IEnumerable<Tick> MinorTicks
        {
            get
            {
                if (Gauge == null)
                    yield break;

                double tickSpacing = (Gauge.Maximum - Gauge.Minimum) / 50;
                for (double tick = Gauge.Minimum; tick <= Gauge.Maximum; tick += tickSpacing)
                {
                    yield return new Tick()
                    {
                        Angle = ValueToAngle(tick),
                        Value = tick.ToString("N0"),
                        Parent = this
                    };
                }
            }
        }

        public IEnumerable<ArcDescriptor> Ranges
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
                        yield return new ArcDescriptor()
                        {
                            Color = range.Color,
                            StartAngle = ValueToAngle(Gauge.Minimum),
                            EndAngle = ValueToAngle(range.Maximum)
                        };
                    }
                    else
                    {
                        var previousRange = Gauge.QualitativeRange[i - 1];
                        yield return new ArcDescriptor()
                        {
                            Color = range.Color,
                            StartAngle = ValueToAngle(previousRange.Maximum),
                            EndAngle = ValueToAngle(range.Maximum)
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
                        Angle = ValueToAngle(tick),
                        Value = tick.ToString("N0"),
                        Parent = this
                    };
                }
            }
        }



        /// <summary>
        /// SizeChanged event handler overriden to add a clip geometry.
        /// </summary>
        protected override void AttachedElement_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            base.AttachedElement_SizeChanged(sender, e);

            if (Clip)
            {
                Grid grid = AttachedElement as Grid;
                grid.Clip = new EllipseGeometry()
                {
                    RadiusX = ElementWidth / 2,
                    RadiusY = ElementHeight / 2,
                    Center = new Point(ElementWidth / 2, ElementHeight / 2)
                };
            }
        }

        protected override void AdaptedDataContextPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                // ValueAngle depends on value
                OnPropertyChanged("ValueAngle");
            }
            else
            {
                // otherwise fire a generic property changed
                OnPropertyChanged("");
            }
        }

        public class Tick
        {
            public double Angle { get; set; }
            public string Value { get; set; }
            public RadialGaugeControlViewModel Parent { get; set; }
        }

        public class ArcDescriptor
        {
            public double StartAngle { get; set; }
            public double EndAngle { get; set; }
            public Color Color { get; set; }
        }
    }


}
