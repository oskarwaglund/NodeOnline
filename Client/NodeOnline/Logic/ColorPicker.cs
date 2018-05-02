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

        public ColorPicker(Canvas canvas)
        {
            _canvas = canvas;

            redSlider = CreateSlider(400, 300, 255);
            blueSlider = CreateSlider(400, 325, 0);
            greenSlider = CreateSlider(400, 350, 0);

            colorShower = new Ellipse
            {
                Width = 20,
                Height = 20,
                Fill = new SolidColorBrush(GetCurrentColor())
            };

            _canvas.Children.Add(colorShower);
            Canvas.SetTop(colorShower, 375);
            Canvas.SetLeft(colorShower, 400);

            Button updateButton = new Button
            {
                Width = 70,
                Height = 20
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
