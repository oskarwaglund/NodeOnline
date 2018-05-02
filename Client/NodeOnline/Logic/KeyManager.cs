using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace NodeOnline.Logic
{
    class KeyManager
    {
        Key[] keys = { Key.W, Key.A, Key.S, Key.D };
        Dictionary<Key, bool> keyPressed;
        private Window Window;

        private Point latestClick;
        private bool clickAvailable;

        public KeyManager(Window window)
        {
            Window = window;

            Window.KeyDown += KeyDown;
            Window.KeyUp += KeyUp;
            Window.MouseLeftButtonUp += OnClick;

            keyPressed = new Dictionary<Key, bool>();
            foreach(var key in keys)
            {
                keyPressed.Add(key, false);
            }

            clickAvailable = false;
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

        void OnClick(object sender, MouseButtonEventArgs e)
        {
            latestClick = e.GetPosition(Window);
            clickAvailable = true;

        }

        public byte[] GetInput()
        {
            byte mask = 0;
            byte cursor = 1;

            foreach(var key in keys)
            {
                if (keyPressed[key])
                {
                    mask += cursor;
                }
                cursor *= 2;
            }

            if (mask == 0 && !clickAvailable)
            {
                return null;
            }

            byte[] packet = clickAvailable ? new byte[5] : new byte[1];
            
            packet[0] = mask;
            if (clickAvailable)
            {
                int x = (int) Math.Round(latestClick.X);
                int y = (int) Math.Round(latestClick.Y);
                packet[1] = (byte)((x >> 8) & 0xFF);
                packet[2] = (byte) (x & 0xFF);
                packet[3] = (byte)((y >> 8) & 0xFF);
                packet[4] = (byte)(y & 0xFF);

                clickAvailable = false;
            }

            return packet;
        }
    }
}
