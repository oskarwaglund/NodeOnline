using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace NodeOnline.Logic
{
    class Player
    {
        public int ID
        {
            get;
        }

        public string Name
        {
            get; set;
        }

        private int _x;
        public int X
        {
            get
            {
                return _x;
            }
            set
            {
                if(value != _x)
                {
                    IsUpdated = false;
                    _x = value;
                }
            }
        }

        private int _y;
        public int Y
        {
            get
            {
                return _y;
            }
            set
            {
                if(value != _y)
                {
                    IsUpdated = false;
                    _y = value;
                }
            }
        }

        private int _health;
        public int Health
        {
            get
            {
                return _health;
            }
            set
            {
                if (value != _health)
                {
                    IsUpdated = false;
                    _health = value;
                }
            }
        }

        public bool IsUpdated
        {
            get; set;
        }

        Ellipse _ui;
        byte _r, _g, _b;

        public Ellipse UI
        {
            get
            {
                return _ui;
            }
        }

        public void SetColor(byte r, byte g, byte b)
        {
            if(r != _r || g != _g || b != _b)
            {
                _ui.Fill = new SolidColorBrush(Color.FromRgb(r, g, b));
                IsUpdated = false;
            }
        }

        public Player(int id, string name, int x, int y)
        {
            ID = id;
            X = x;
            Y = y;
            Name = name;
            IsUpdated = false;
            _ui = new Ellipse
            {
                Width = 20,
                Height = 20,
                Fill = Brushes.Green
            };
        }




    }
}
