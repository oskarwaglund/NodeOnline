using NodeOnline.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace NodeOnline
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Brush color = Brushes.Blue;
        System.TimeSpan interval = new TimeSpan(250000);

        List<Player> players = new List<Player>();

        KeyManager keyManager = new KeyManager();

        private const string SERVER_IP = "127.0.0.1";
        private const int SERVER_PORT = 12345;

        private const string SERVER_MC_IP = "224.1.2.3";
        private const int SERVER_MC_PORT = 6001;

        private GameConnection gameConnection = new GameConnection();

        public MainWindow()
        {
            InitializeComponent();

            int id = gameConnection.Connect("Olle", SERVER_IP, SERVER_PORT);
            gameConnection.ConnectToMcServer(SERVER_MC_IP, SERVER_MC_PORT);

            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += new EventHandler(gameLoop);

            timer.Interval = interval;
            timer.Start();

            KeyDown += new KeyEventHandler(keyManager.KeyDown);
            KeyUp += new KeyEventHandler(keyManager.KeyUp);
        }

        private void gameLoop(object sender, EventArgs e)
        {
            SendInput();
            ReceiveState();
            
            UpdateGame();
        }

        private void SendInput()
        {

        }

        private void ReceiveState()
        {
            byte[] state = gameConnection.ListenToMcServer();
            for(int i = 0; i < state.Length; i += 3)
            {
                byte id = state[i];
                byte x = state[i + 1];
                byte y = state[i + 2];

                Player player = players.FirstOrDefault(p => p.ID == id);
                if(player == null)
                {
                    players.Add(new Player(id, "Player"+id, x, y));
                } else
                {
                    player.X = x;
                    player.Y = y;
                }
            }
        }

        private void UpdateGame()
        {
            foreach (Player player in players.Where(p => p.IsUpdated))
            {
                Canvas.SetLeft(player.UI, player.X);
                Canvas.SetTop(player.UI, player.Y);

                player.IsUpdated = false;
            }
        }
    }
}
