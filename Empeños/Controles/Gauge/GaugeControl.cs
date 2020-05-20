using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Empeños.Controles
{
    public class GaugeControl : Control, INotifyPropertyChanged
    {
        public GaugeControl()
        {
            QualitativeRange = new QualitativeRanges();
            this.DefaultStyleKey = typeof(GaugeControl);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Grid root = GetTemplateChild("LayoutRoot") as Grid;
            root.DataContext = this;
        }

        #region Value DP

        /// <summary>
        /// Gets / sets the Gauge value
        /// </summary>
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        static DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double),
          typeof(GaugeControl), new PropertyMetadata(50.0, OnValuePropertyChanged));

        private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((GaugeControl)d).OnPropertyChanged("Value");
        }

        #endregion

        #region Maximum DP

        /// <summary>
        /// Gets / sets the Gauge Maximum
        /// </summary>
        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        static DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(double),
          typeof(GaugeControl), new PropertyMetadata(100.0));

        #endregion

        #region QualitativeRanges DP

        /// <summary>
        /// Gets / sets the Gauges Qualitative Ranges
        /// </summary>
        public QualitativeRanges QualitativeRange
        {
            get { return (QualitativeRanges)GetValue(QualitativeRangeProperty); }
            set { SetValue(QualitativeRangeProperty, value); }
        }

        static DependencyProperty QualitativeRangeProperty = DependencyProperty.Register("QualitativeRange", typeof(QualitativeRanges),
          typeof(GaugeControl), new PropertyMetadata(null));

        #endregion

        #region Minimum DP

        /// <summary>
        /// Gets / set the Gauge minimum value
        /// </summary>
        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        static DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(double),
          typeof(GaugeControl), new PropertyMetadata(0.0));

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion
    }

    public class QualitativeRanges : ObservableCollection<QualitativeRange>
    { }

    /// <summary>
    /// Defines a single qualitative range value
    /// </summary>
    public class QualitativeRange
    {
        private double maximum;

        public double Maximum
        {
            get { return maximum; }
            set { maximum = value; }
        }

        private Color color;

        public Color Color
        {
            get { return color; }
            set { color = value; }
        }
    }
}
