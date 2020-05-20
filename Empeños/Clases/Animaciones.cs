using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Empeños.Clases
{
    public static class Animaciones
    {
        public static void DeslizarElementos(FrameworkElement elementoPadre, FrameworkElement elemento1, FrameworkElement elemento2)
        {
            if (elemento1.Visibility == Visibility.Visible)
            {
                elemento2.Visibility = Visibility.Visible;
                elemento2.RenderTransform.BeginAnimation(TranslateTransform.XProperty, CreateSlideAnimation(elementoPadre.ActualWidth, 0));
                elemento1.RenderTransform.BeginAnimation(TranslateTransform.XProperty, CreateSlideAnimation(0, -elementoPadre.ActualWidth, (s, ev) => elemento1.Visibility = Visibility.Collapsed));
            }
            else
            {
                elemento1.Visibility = Visibility.Visible;
                elemento2.RenderTransform.BeginAnimation(TranslateTransform.XProperty, CreateSlideAnimation(0, elementoPadre.ActualWidth));
                elemento1.RenderTransform.BeginAnimation(TranslateTransform.XProperty, CreateSlideAnimation(-elementoPadre.ActualWidth, 0, (s, ev) => elemento2.Visibility = Visibility.Collapsed));
            }
        }

        private static AnimationTimeline CreateSlideAnimation(double from, double to, EventHandler whenDone = null)
        {
            IEasingFunction ease = new BackEase { Amplitude = 0.5, EasingMode = EasingMode.EaseOut };
            var duration = new Duration(TimeSpan.FromSeconds(0.5));
            var anim = new DoubleAnimation(from, to, duration) { EasingFunction = ease };
            if (whenDone != null)
                anim.Completed += whenDone;
            anim.Freeze();
            return anim;
        }
    }
}
