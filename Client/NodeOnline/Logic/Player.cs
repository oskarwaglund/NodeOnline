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
            get;
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
                    IsUpdated = true;
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
                    IsUpdated = true;
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
                    IsUpdated = true;
                    _health = value;
                }
            }
        }

        public bool IsUpdated
        {
            get; set;
        }

        public UIElement UI
        {
            get;
        }

        public Player(int id, string name, int x, int y)
        {
            ID = id;
            X = x;
            Y = y;
            Name = name;
            IsUpdated = true;
            UI = new Ellipse
            {
                Width = 20,
                Height = 20,
                Fill = Brushes.Green
            };
        }




    }
}
