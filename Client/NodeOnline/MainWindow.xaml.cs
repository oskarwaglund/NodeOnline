using NodeOnline.Logic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        System.TimeSpan interval = new TimeSpan(100000);

        List<Player> players = new List<Player>();

        KeyManager keyManager = new KeyManager();

        private const string SERVER_IP = "192.168.0.7";
        private const int SERVER_PORT = 12345;

        private const string SERVER_MC_IP = "224.1.2.3";
        private const int SERVER_MC_PORT = 6000;

        private GameConnection gameConnection = new GameConnection();

        private int ID;

        PreciseTimer preciseTimer;

        public MainWindow()
        {
            InitializeComponent();

            string name = Microsoft.VisualBasic.Interaction.InputBox("Select name");
            string localIP = Microsoft.VisualBasic.Interaction.InputBox("Enter local network interface (IP Address)");
            string server = (localIP == SERVER_IP) ? "localhost" : SERVER_IP;

            ID = gameConnection.Connect(name, server, SERVER_PORT);
            gameConnection.ConnectToMcServer(SERVER_MC_IP, SERVER_MC_PORT, localIP);
            gameConnection.DataReceived += UpdateGame;

            KeyDown += new KeyEventHandler(keyManager.KeyDown);
            KeyUp += new KeyEventHandler(keyManager.KeyUp);

            preciseTimer = new PreciseTimer(25);
            preciseTimer.Tick += GameLoop;

            Closing += OnClose;
        }

        void OnClose(object sender, EventArgs e)
        {
            preciseTimer.Stop();
            gameConnection.Stop();
        }

        private void GameLoop(object sender, EventArgs e)
        {
            SendInput();
        }

        private void SendInput()
        {
            byte mask = keyManager.GetMask();
            gameConnection.SendInput(ID, mask);
        }

        private void UpdateGame(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                byte[] state = gameConnection.GetState(out int numberOfBytes);
                for (int i = 0; i < numberOfBytes; i += 5)
                {
                    byte id = state[i];
                    int x = (state[i + 1] << 8) | state[i + 2];
                    int y = (state[i + 3] << 8) | state[i + 4];

                    Player player = players.FirstOrDefault(p => p.ID == id);
                    if (player == null)
                    {
                        Player newPlayer = new Player(id, "Player" + id, x, y);
                        players.Add(newPlayer);
                        paintCanvas.Children.Add(newPlayer.UI);
                    }
                    else
                    {
                        player.X = x;
                        player.Y = y;
                    }
                }

                foreach (Player player in players.Where(p => !p.IsUpdated))
                {
                    Canvas.SetLeft(player.UI, player.X);
                    Canvas.SetTop(player.UI, player.Y);

                    player.IsUpdated = true;
                }
            }));
        }
    }
}
