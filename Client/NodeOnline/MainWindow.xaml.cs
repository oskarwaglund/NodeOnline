using NodeOnline.Logic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.VisualBasic.CompilerServices;

namespace NodeOnline
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Player> players = new List<Player>();

        KeyManager keyManager = new KeyManager();

        private const string SERVER_IP = "192.168.0.7";
        private const int SERVER_PORT = 12345;

        private GameConnection gameConnection = new GameConnection();

        private PreciseTimer preciseTimer;
        private ColorPicker colorPicker;

        public MainWindow()
        {
            InitializeComponent();

            string name = Microsoft.VisualBasic.Interaction.InputBox("Select name", "Select name", Environment.UserName);

            gameConnection.Connect(name, SERVER_IP, SERVER_PORT);
            gameConnection.StateReceived += MovePlayers;
            gameConnection.PlayerDataReceived += UpdatePlayers;
            
            KeyDown += keyManager.KeyDown;
            KeyUp += keyManager.KeyUp;

            preciseTimer = new PreciseTimer(25);
            preciseTimer.Tick += GameLoop;

            Closing += OnClose;

            colorPicker = new ColorPicker(paintCanvas);
            colorPicker.OnUpdate += ColorPickerOnOnUpdate;
        }

        private void ColorPickerOnOnUpdate(object sender, EventArgs eventArgs)
        {
            Color color = colorPicker.GetCurrentColor();
            gameConnection.UpdatePlayerColor(color);
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
            gameConnection.SendInput(mask);
        }

        private void MovePlayers(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                int numberOfBytes;
                byte[] state = gameConnection.GetGameStateBuffer(out numberOfBytes);
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
                        paintCanvas.Children.Add(newPlayer.NameText);
                    }
                    else
                    {
                        player.X = x;
                        player.Y = y;
                    }
                }

                foreach (Player player in players.Where(p => !p.IsUpdated))
                {
                    Canvas.SetLeft(player.UI, player.X - Player.SIZE/2);
                    Canvas.SetTop(player.UI, player.Y - Player.SIZE/2);

                    int width = (int)Math.Round(MeasureString(player.NameText).Width);
                    Canvas.SetLeft(player.NameText, player.X - width/2);
                    Canvas.SetTop(player.NameText, player.Y + Player.SIZE/2);

                    player.IsUpdated = true;
                }
            }));
        }

        private Size MeasureString(TextBlock textBlock)
        {
            var formattedText = new FormattedText(
                textBlock.Text,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(textBlock.FontFamily, textBlock.FontStyle, textBlock.FontWeight, textBlock.FontStretch),
                textBlock.FontSize,
                Brushes.Black,
                new NumberSubstitution(),
                TextFormattingMode.Display);

            return new Size(formattedText.Width, formattedText.Height);
        }

        private void UpdatePlayers(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                int numberOfBytes;
                byte[] data = gameConnection.GetPlayerDataBuffer(out numberOfBytes);
                for(int i = 0; i < numberOfBytes; i += 21)
                {
                    byte id = data[i];
                    Player player = players.FirstOrDefault(p => p.ID == id);
                    if(player == null)
                    {
                        throw new Exception("Received player data for unknown player!");
                    }

                    int length;
                    byte point = 1;
                    for (length = 0; length < 16 && point != 0; length++)
                        point = data[i + 1 + length + 1];

                    string name = System.Text.Encoding.Default.GetString(data, i + 1, length);
                    byte r = data[i + 17];
                    byte g = data[i + 18];
                    byte b = data[i + 19];
                    int health = data[i + 20];

                    player.Name = name;
                    player.SetColor(r, g, b);
                    player.Health = health;
                }
            }));
        }  
    }
}
