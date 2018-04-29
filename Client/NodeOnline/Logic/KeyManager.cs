using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NodeOnline.Logic
{
    class KeyManager
    {
        Key[] keys = { Key.W, Key.A, Key.S, Key.D };
        Dictionary<Key, bool> keyPressed;

        public KeyManager()
        {
            keyPressed = new Dictionary<Key, bool>();
            foreach(var key in keys)
            {
                keyPressed.Add(key, false);
            }
        }

        public void KeyDown(object sender, KeyEventArgs e)
        {
            HandleKey(e.Key, true);
        }

        public void KeyUp(object sender, KeyEventArgs e)
        {
            HandleKey(e.Key, false);
        }

        void HandleKey(Key key, bool pressed)
        {
            if (keyPressed.Keys.Contains(key))
            {
                keyPressed[key] = pressed;
            }
        }

        public byte GetMask()
        {
            byte mask = 0;
            byte cursor = 1;

            foreach(var key in keyPressed)
            {
                mask += cursor;
                cursor *= 2;
            }

            return mask;
        }
    }
}
