using NodeOnline.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
        private const int SERVER_MC_PORT = 6000;

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

        private void AddPlayer()
        {
            Player player = new Player(123, "Harry", 100, 100);
            players.Add(player);
            paintCanvas.Children.Add(player.UI);
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

            }
        }

        private void UpdateGame()
        {
            foreach (Player player in players)
            {
                player.X += 0;
            }

            foreach (Player player in players.Where(p => p.IsUpdated))
            {
                Canvas.SetLeft(player.UI, player.X);
                Canvas.SetTop(player.UI, player.Y);

                player.IsUpdated = false;
            }
        }
    }
}
