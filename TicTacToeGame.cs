using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace TicTacToe
{
    public class TicTacToeGame
    {
        private string[,] board;
        public string currentPlayer{ set; get; }
        private string player1Name;
        private string player2Name;
        public int boardSize { set; get; }
        private int counter;
        public bool fin { get; set; }

        public TicTacToeGame(string player1Name, string player2Name, int boardSize)
        {
            this.player1Name = player1Name;
            this.player2Name = player2Name;
            this.boardSize = boardSize;
            this.fin = false;
            start();
        }

        public void start()
        {
            board = new string[boardSize, boardSize];
            Random rnd = new Random();
            int startingplayer = rnd.Next(1, 3);
            if (startingplayer == 1) {
                currentPlayer = player1Name;
            }
            else
                currentPlayer = player2Name;
            ((MainWindow)System.Windows.Application.Current.MainWindow).currentPlayerText.Visibility = Visibility.Visible;
            ((MainWindow)System.Windows.Application.Current.MainWindow).currentPlayerText.Content = "Začíná hráč: " + currentPlayer;


        }

        public void add()
        {
            counter++;
            if(counter == boardSize*boardSize) {
                MessageBox.Show("Remíza", "Piškvorky", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        public bool procCoords(int x, int y, String playerSymbol)
        
        {
            if(!fin) board[x, y] = playerSymbol;
            if(CheckForWin())
            {
                fin = true;
                MessageBox.Show("Vyhrál " + currentPlayer +", počet tahů: " + counter, "Výhra", MessageBoxButton.OK, MessageBoxImage.Information);
                ((MainWindow)System.Windows.Application.Current.MainWindow).nextRound.Visibility = Visibility.Visible;
                return true;
            }
            add();
            return false;
        }

        private bool CheckForWin()
        {
            int cnt;
            string previous;

            //Vyhodnoceni viteztvi ve sloupcich
            for (int c = 0; c < boardSize; c++)
            {
                cnt = 0;
                previous = null;
                for (int r = 0; r < boardSize; r++)
                {
                    if (board[r, c] != null && board[r, c] == previous) { cnt++; }
                    else { cnt = 1; } // Pokud je aktuální prvek null nebo se liší od předchozího, resetujeme počítadlo na 1
                    if (cnt == boardSize) { return true; }
                    previous = board[r, c];
                }
            }

            // Vyhodnocení vítězství v řádcích
            for (int r = 0; r < boardSize; r++)
            {
                cnt = 0;
                previous = null;
                for (int c = 0; c < boardSize; c++)
                {
                    if (board[r, c] != null && board[r, c] == previous) { cnt++; }
                    else { cnt = 1; } // Pokud je aktuální prvek null nebo se liší od předchozího, resetujeme počítadlo na 1
                    if (cnt == boardSize) { return true; }
                    previous = board[r, c];
                }
            }

            // Vyhodnocení diagonální vítězství zleva dolů
            cnt = 0;
            previous = null;
            for (int i = 0; i < boardSize; i++)
            {
                if (board[i, i] != null && board[i, i] == previous) { cnt++; }
                else { cnt = 1; } // Pokud je aktuální prvek null nebo se liší od předchozího, resetujeme počítadlo na 1
                if (cnt == boardSize) { return true; }
                previous = board[i, i];
            }

            // Vyhodnocení diagonální vítězství zprava dolů
            cnt = 0;
            previous = null;
            for (int i = 0; i < boardSize; i++)
            {
                if (board[i, boardSize - 1 - i] != null && board[i, boardSize - 1 - i] == previous) { cnt++; }
                else { cnt = 1; } // Pokud je aktuální prvek null nebo se liší od předchozího, resetujeme počítadlo na 1
                if (cnt == boardSize) { return true; }
                previous = board[i, boardSize - 1 - i];
            }

            return false;
        }

        public String changeCurrentPlayer()
        {
        if (currentPlayer == player1Name)
            {
                currentPlayer = player2Name;
            }
        else
            {
                currentPlayer= player1Name;
            }
            return currentPlayer;
        }
    }
}
