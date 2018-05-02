using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace NodeOnline.Logic
{
    class ColorPicker
    {
        private Canvas _canvas;
        
        private Slider redSlider, blueSlider, greenSlider;
        private Ellipse colorShower;

        public event EventHandler OnUpdate;

        private const int SLIDER_WIDTH = 150;
        private const int SLIDER_X = 400;
        private const int SLIDER_Y = 300;
        private const int SLIDER_SPACING = 25;

        public ColorPicker(Canvas canvas, Color color)
        {
            _canvas = canvas;

            redSlider = CreateSlider(SLIDER_X, SLIDER_Y, color.R);
            blueSlider = CreateSlider(SLIDER_X, SLIDER_Y + SLIDER_SPACING, color.G);
            greenSlider = CreateSlider(SLIDER_X, SLIDER_Y + SLIDER_SPACING*2, color.B);

            colorShower = new Ellipse
            {
                Width = 20,
                Height = 20,
                Fill = new SolidColorBrush(color)
            };

            _canvas.Children.Add(colorShower);
            Canvas.SetTop(colorShower, 375);
            Canvas.SetLeft(colorShower, 400);

            Button updateButton = new Button
            {
                Width = 70,
                Height = 20,
                Content = "Update"
            };
            _canvas.Children.Add(updateButton);
            Canvas.SetLeft(updateButton, 430);
            Canvas.SetTop(updateButton, 375);
            updateButton.Click += OnUpdateButtonClick;
        }

        private void OnUpdateButtonClick(object sender, RoutedEventArgs routedEventArgs)
        {
            if (OnUpdate != null)
            {
                OnUpdate.Invoke(this, EventArgs.Empty);
            }
        }

        public Slider CreateSlider(int x, int y, int value)
        {
            Slider slider = new Slider
            {
                Orientation = Orientation.Horizontal,
                Minimum = 0,
                Maximum = 255,
                Width = 100,
                Value = value
            };

            _canvas.Children.Add(slider);
            Canvas.SetLeft(slider, x);
            Canvas.SetTop(slider, y);

            slider.ValueChanged += OnSliderValueChanged;

            return slider;
        }

        private void OnSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> routedPropertyChangedEventArgs)
        {
            colorShower.Fill = new SolidColorBrush(GetCurrentColor());
        }

        public Color GetCurrentColor()
        {
            return Color.FromRgb(
                (byte) Math.Round(redSlider.Value),
                (byte) Math.Round(blueSlider.Value),
                (byte) Math.Round(greenSlider.Value));
        }
    }
}
