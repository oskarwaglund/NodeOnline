using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace NodeOnline.Logic
{
    class Player
    {
        public static int SIZE = 20;

        public int ID
        {
            get; private set;
        }

        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if(_name != value)
                {
                    _name = value;
                    _nameText.Text = value;
                    IsUpdated = false;
                }
            }
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

        public TextBlock _nameText;
        public TextBlock NameText
        {
            get
            {
                return _nameText;
            }
        }

        public void SetColor(byte r, byte g, byte b)
        {
            if(r != _r || g != _g || b != _b)
            {
                _ui.Fill = new SolidColorBrush(Color.FromRgb(r, g, b));
                _r = r;
                _g = g;
                _b = b;
                IsUpdated = false;
            }
        }

        public Player(int id, string name, int x, int y)
        {
            ID = id;
            _x = x;
            _y = y;
            _name = name;
            IsUpdated = false;
            _ui = new Ellipse
            {
                Width = SIZE,
                Height = SIZE,
                Fill = Brushes.Green
            };

            _nameText = new TextBlock
            {
                Text = Name
            };
        }
    }
}
