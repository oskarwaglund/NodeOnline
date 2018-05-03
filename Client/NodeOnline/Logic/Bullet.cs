using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Shapes;

namespace NodeOnline.Logic
{
    class Bullet
    {
        public int Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public Ellipse UI { get; set; }

        public const int Size = 5;
        private static readonly Color Color = Colors.Black;

        public Bullet(int id, int x, int y)
        {
            Id = id;
            X = x;
            Y = y;
            
            UI = new Ellipse
            {
                Width = Size,
                Height = Size,
                Fill = new SolidColorBrush(Color)
            };
        }


    }
}