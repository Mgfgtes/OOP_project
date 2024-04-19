using Accessibility;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Linq;

namespace TicTacToe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TicTacToeGame game;
        Dictionary<string,int> victoryMap = new Dictionary<string,int>();
        string fileName = "leaderboard.txt";
        public MainWindow()
        {
            InitializeComponent();
        }

        public void StartGame_Click(object sender, RoutedEventArgs e)
        {
            // Získání jmen hráčů ze vstupních polí
            string player1Name = Player1NameTextBox.Text; // X
            string player2Name = Player2NameTextBox.Text; // O

            if (string.IsNullOrWhiteSpace(player1Name) || string.IsNullOrWhiteSpace(player2Name) || player1Name == player2Name)
            {
                MessageBox.Show("Zadejte jména obou hráčů.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            

            // Vytvoření instance herní třídy s jmény hráčů a velikostí hracího pole
            int boardSize = int.Parse(((ComboBoxItem)BoardSizeComboBox.SelectedItem).Content.ToString().Split('x')[0]);
            //game = new TicTacToeGame(player1Name, player2Name, boardSize);
            InitializeGameBoard(boardSize);
            game = new TicTacToeGame(player1Name,player2Name,boardSize);
            Player1NameTextBox.IsEnabled = false;
            Player2NameTextBox.IsEnabled = false;
            startButton.IsEnabled = false;
            BoardSizeComboBox.IsEnabled = false; 

        }

        private void NextRound_Click(object sender, RoutedEventArgs e)
        {
            Player1NameTextBox.IsEnabled = true;
            Player2NameTextBox.IsEnabled = true;
            startButton.IsEnabled = true;
            BoardSizeComboBox.IsEnabled = true;
            nextRound.Visibility = Visibility.Hidden;
            loadPrevious.Visibility = Visibility.Visible;
        }

        private void LoadPrevious_Click(object sender, RoutedEventArgs e)
        {
            loadPrevious.Visibility = Visibility.Hidden;
            InitializeGameBoard(game.boardSize);
            game.start();
            Player1NameTextBox.IsEnabled = false;
            Player2NameTextBox.IsEnabled = false;
            startButton.IsEnabled = false;
            BoardSizeComboBox.IsEnabled = false;
        }

        private void ShowLeaderboard_Click(object sender, RoutedEventArgs e) {
            
            string leaderboard = "";
            

            // Zkontrolujeme, zda soubor existuje
            if (!File.Exists(fileName))
            {
                // Pokud soubor neexistuje, vytvoříme ho a zapíšeme do něj nějaký text
                File.CreateText(fileName);
            }

            var sortedLeaderboard = victoryMap.OrderByDescending(pair => pair.Value);
            // Otevřeme existující soubor a načteme řádek po řádku
            using (StreamReader sr = File.OpenText(fileName))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] parts = line.Split(" ");
                    victoryMap.Add(parts[0], Int32.Parse(parts[1]));
                }
            }
        
            
            
            foreach (KeyValuePair<string,int> pair in sortedLeaderboard)
            {
                // Přidáme hodnotu záznamu do výsledného řetězce
                leaderboard = leaderboard + pair.Key + " " + pair.Value + "\n";
            }
            MessageBox.Show(leaderboard, "Leaderboard", MessageBoxButton.OK);
            victoryMap.Clear();
            



        }

        private void Player1NameTextBox_TextChanged(object sender, TextChangedEventArgs e) { }

        private void BoardSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) { }
        
        public void HandleMove_Click(object sender, RoutedEventArgs e)
        {
        Button btn = (Button)sender;
            if (!game.fin)
            {
                btn.IsEnabled = false;
                if (String.Equals(game.currentPlayer, Player1NameTextBox.Text))
                {
                    btn.Content = "X";
                }
                else
                {
                    btn.Content = "O";
                }
                Tuple<int, int> position = (Tuple<int, int>)btn.Tag;

                // Získání pozice tahu
                int row = position.Item1;
                int col = position.Item2;
                if (game.procCoords(row, col, btn.Content.ToString()))
                {
                   
                    string leaderboard = "";

                    // Otevřeme existující soubor a načteme řádek po řádku
                    using (StreamReader sr = File.OpenText(fileName))
                    {
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            string[] parts = line.Split(" ");
                            victoryMap.Add(parts[0], Int32.Parse(parts[1]));
                        }
                    }



                    foreach (KeyValuePair<string, int> pair in victoryMap)
                    {
                        // Přidáme hodnotu záznamu do výsledného řetězce
                        leaderboard = leaderboard + pair.Key + " " + pair.Value + "\n";
                    }


                    if (File.Exists(fileName))
                    {
                        // Pokud soubor existuje, smažeme ho
                        File.Delete(fileName);
                    }
                    if (victoryMap.ContainsKey(game.currentPlayer))
                    {
                        victoryMap[game.currentPlayer]++;
                    }
                    else
                    {
                        victoryMap.Add(game.currentPlayer, 1);
                    }
                    using (StreamWriter outputFile = new StreamWriter(fileName))
                    {
                        foreach (KeyValuePair<string, int> pair in victoryMap)
                            outputFile.WriteLine(pair.Key + " " + pair.Value);
                    }
                    victoryMap.Clear();

                }
                game.changeCurrentPlayer();
                currentPlayerText.Content = "Pokračuje hráč: " + game.currentPlayer;
            }
            
        }
        private void InitializeGameBoard(int size)
        {
            // Vymazání starého obsahu herního pole
            GameBoard.Items.Clear();
            // Vytvoření nového herního pole
            for (int row = 0; row < size; row++)
            {
                StackPanel rowPanel = new StackPanel { Orientation = Orientation.Horizontal };
                for (int col = 0; col < size; col++)
                {
                    Button button = new Button
                    {
                        Tag = new Tuple<int, int>(row, col),
                        Width = 50,
                        Height = 50,
                        Margin = new Thickness(5),
                        Content = "",
                        // Při kliknutí na tlačítko zavolá metodu HandleMove_Click pro zpracování tahu
                        ClickMode = ClickMode.Press,
                    };
                    button.Click += HandleMove_Click;
                    rowPanel.Children.Add(button);
                }
                GameBoard.Items.Add(rowPanel);
            }
        }

        
    }
}