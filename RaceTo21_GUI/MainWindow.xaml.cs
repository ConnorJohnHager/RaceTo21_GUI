﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
using static System.Net.Mime.MediaTypeNames;

namespace RaceTo21_GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Task nextTask = Task.GetNumberOfPlayers;
        public bool TaskSuccess = false;
        public int TaskOrder = 0;

        public int NumberOfPlayers;
        List<Player> players = new List<Player>();
        public int currentPlayer = 0;
        public int bet;
        public int pot;
        List<TextBlock> scores = new List<TextBlock>();
        Deck deck = new Deck();
        public int busted = 0;

        public MainWindow()
        {
            InitializeComponent();
            DoNextTask();
        }

        /// <summary>
        /// Keeps track of the tasks needed to be done and what is visible on the screen.
        /// </summary>
        public void DoNextTask()
        {
            if (nextTask == Task.GetNumberOfPlayers)
            {
                Title_Phrase.Text = "Let's Race To 21!";
                Support_Text.Text = "How many players? (between 2-8)";
                User_Input.Text = "*Input Value*";

                if (TaskSuccess == true)
                {
                    TaskSuccess = false;
                    nextTask = Task.GetNames;
                }
            }

            if (nextTask == Task.GetNames)
            {
                Title_Phrase.Text = "Player #" + (TaskOrder + 1).ToString();
                Support_Text.Text = "What is your name? (Limit to 10 characters)";
                User_Input.Text = "*Input Name*";

                if (TaskSuccess == true)
                {
                    TaskSuccess = false;
                    TaskOrder = 0;
                    nextTask = Task.SetUpBoard;
                }
            }

            if (nextTask == Task.GetBets)
            {
                Title_Phrase.Text = "Welcome " + players[TaskOrder].name + "!";
                Support_Text.Text = "How much would you like to bet? Your current bank stands at $" + players[TaskOrder].bank + ".";
                User_Input.Text = "*Input Value*";
            }

            if (nextTask == Task.SetUpBoard)
            {
                deck.Shuffle();
                UpdateScores();
                nextTask = Task.PlayerTurn;
                SetUpBoardProcess();
            }

            if (nextTask == Task.PlayerTurn)
            {
                if (players[currentPlayer].cards.Count == 0)
                {
                    Card card = deck.DealTopCard();
                    players[currentPlayer].cards.Add(card);
                    players[currentPlayer].score = ScoreHand(players[currentPlayer]);
                    UpdateScores();
                    nextTask = Task.CheckForEnd;
                }
                else if (TaskSuccess == true)
                {
                    UpdateScores();
                    TaskSuccess = false;
                    nextTask = Task.CheckForEnd;
                }
                else if (players[currentPlayer].status != PlayerStatus.active)
                {
                    UpdateScores();
                    nextTask = Task.CheckForEnd;
                }
                else
                {
                    UpdateVisualCards();
                }
            }

            if (nextTask == Task.CheckForEnd)
            {
                CheckForEndProcess();
            }
        }

        /// <summary>
        /// The three buttons that can be clicked by the user.
        /// </summary>
        private void Continue_Button_Click(object sender, RoutedEventArgs e)
        {
            CheckTaskSuccess();
        }

        private void Draw_Button_Click(object sender, RoutedEventArgs e)
        {
            Card card = deck.DealTopCard();
            players[currentPlayer].cards.Add(card);
            players[currentPlayer].score = ScoreHand(players[currentPlayer]);
            CheckPlayerForBust(players[currentPlayer]);
            TaskSuccess = true;
            DoNextTask();
        }

        private void Stay_Button_Click(object sender, RoutedEventArgs e)
        {
            players[currentPlayer].status = PlayerStatus.stay;
            TaskSuccess = true;
            DoNextTask();
        }

        /// <summary>
        /// State machine for Continue_Button
        /// </summary>
        private void CheckTaskSuccess()
        {
            if (nextTask == Task.GetNumberOfPlayers)
            {
                GetNumberOfPlayersProcess();
            }
            if (nextTask == Task.GetNames)
            {
                GetPlayerNamesProcess();
            }
            if (nextTask == Task.GetBets)
            {
                GetPlayerBetsProcess();
            }
        }

        /// <summary>
        /// Code used to process step success during DoNextTask();
        /// </summary>
        private void GetNumberOfPlayersProcess()
        {
            if (int.TryParse(User_Input.Text, out NumberOfPlayers) == true && NumberOfPlayers >= 2 && NumberOfPlayers <= 8)
            {
                NumberOfPlayers = int.Parse(User_Input.Text);
                TaskSuccess = true;
                DoNextTask();
            }
            else
            {
                User_Input.Text = "*Invalid, try again*";
            }
        }

        private void GetPlayerNamesProcess()
        {
            if (User_Input.Text != "*Input Name*")
            {
                if (User_Input.Text.Length >= 1 && User_Input.Text.Length <= 10)
                {
                    AddPlayer(User_Input.Text);
                    nextTask = Task.GetBets;
                    DoNextTask();
                }
                else
                {
                    User_Input.Text = "*Invalid, try again*";
                }
            }
        }

        private void GetPlayerBetsProcess()
        {
            if (User_Input.Text != "*Input Value*" )
            {
                if (int.TryParse(User_Input.Text, out bet) == true && bet > 0 && bet <= players[TaskOrder].bank)
                {
                    bet = int.Parse(User_Input.Text);
                    players[TaskOrder].bank -= bet;
                    pot += bet;
                    TaskOrder++;

                    if (TaskOrder == NumberOfPlayers)
                    {
                        TaskSuccess = true;
                        nextTask = Task.GetNames;
                        DoNextTask();
                    }
                    else
                    {
                        nextTask = Task.GetNames;
                        DoNextTask();
                    }
                }
                else
                {
                    User_Input.Text = "*Invalid, try again*";
                }
            }
        }

        private void SetUpBoardProcess()
        {
            Game_Content_Style.Visibility = Visibility.Hidden;
            Draw_Button.Visibility = Visibility.Visible;
            Stay_Button.Visibility = Visibility.Visible;
            Deck_Image.Visibility = Visibility.Visible;
            CardsOnTheTable.Visibility = Visibility.Visible;

            VisibilityForScoreboard(true);
            PotScore.Text = "$" + pot;

            for (int i = 0; i < players.Count; i++)
            {
                TextBlock textBlock = new TextBlock
                {
                    Text = players[i].name,
                    Foreground = Brushes.White,
                    FontSize = 20,
                    Margin = new Thickness(15, 0, 0, 0)
                };

                Grid.SetRow(textBlock, i + 1);
                Grid.SetColumn(textBlock, 0);

                myGrid.Children.Add(textBlock);
            }
            DoNextTask();
        }

        public void CheckForEndProcess()
        {
            if (players[currentPlayer].score == 21)
            {
                players[currentPlayer].status = PlayerStatus.win;
                players[currentPlayer].bank += pot;
                AnnounceWinner(players[currentPlayer]);
            }
            else if (busted == players.Count - 1 || !CheckActivePlayers())
            {
                Player winner = DoFinalScoring();

                foreach (Player player in players)
                {
                    if (winner.status == player.status)
                    {
                        player.bank += pot;
                    }
                }
                AnnounceWinner(winner);
            }
            else
            {
                currentPlayer++;
                if (currentPlayer > players.Count - 1)
                {
                    currentPlayer = 0; // back to the first player...
                }
                nextTask = Task.PlayerTurn;
                DoNextTask();
            }
        }

        /// <summary>
        /// Code used for internal game mechanics and tracking conditions/rules
        /// </summary>
        private void AddPlayer(string n)
        {
            players.Add(new Player(n));
        }

        public int ScoreHand(Player player)
        {
            int score = 0;

            foreach (Card card in player.cards)
            {
                string faceValue = card.ID.Remove(card.ID.Length - 1);
                switch (faceValue)
                {
                    case "K":
                    case "Q":
                    case "J":
                        score = score + 10;
                        break;
                    case "A":
                        score = score + 1;
                        break;
                    default:
                        score = score + int.Parse(faceValue);
                        break;
                }
            }
            return score;
        }

        public void CheckPlayerForBust(Player player)
        {
            if (player.score > 21)
            {
                player.status = PlayerStatus.bust;
                busted++;
            }
        }

        public bool CheckActivePlayers()
        {
            foreach (var player in players)
            {
                if (player.status == PlayerStatus.active)
                {
                    return true; // at least one player is still going!
                }
            }
            return false; // everyone has stayed or busted, or someone won!
        }

        public Player DoFinalScoring()
        {
            int highScore = 0;
            foreach (var player in players)
            {
                if (player.status == PlayerStatus.win)
                {
                    return player;
                }
                if (player.status == PlayerStatus.stay)
                {
                    if (player.score > highScore)
                    {
                        highScore = player.score;
                    }
                }
                // if busted don't bother checking!
            }
            if (highScore > 0) // someone scored, anyway!
            {
                // find the FIRST player in list who meets win condition
                return players.Find(player => player.score == highScore);
            }
            return null; // everyone must have busted because nobody won!
        }

        /// <summary>
        /// Code used to update visuals of the game
        /// </summary>
        private void VisibilityForScoreboard(bool visibility)
        {
            if (visibility)
            {
                Scoreboard.Visibility = Visibility.Visible;
                PlayerTitle.Visibility = Visibility.Visible;
                ScoreTitle.Visibility = Visibility.Visible;
                PotName.Visibility = Visibility.Visible;
                PotScore.Visibility = Visibility.Visible;
            }
            else
            {
                Scoreboard.Visibility = Visibility.Hidden;
                PlayerTitle.Visibility = Visibility.Hidden;
                ScoreTitle.Visibility = Visibility.Hidden;
                PotName.Visibility = Visibility.Hidden;
                PotScore.Visibility = Visibility.Hidden;
            }
        }

        private void UpdateScores()
        {
            if (scores != null)
            {
                foreach (TextBlock textBlock in scores)
                {
                    myGrid.Children.Remove(textBlock);
                }

                scores.Clear();
            }

            for (int i = 0; i < players.Count; i++)
            {
                TextBlock textBlock = new TextBlock
                {
                    Text = players[i].score.ToString(),
                    Foreground = Brushes.White,
                    FontSize = 20,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Margin = new Thickness(0, 0, 15, 0)
                };

                Grid.SetRow(textBlock, i + 1);
                Grid.SetColumn(textBlock, 0);

                scores.Add(textBlock);
                myGrid.Children.Add(textBlock);
            }
        }

        public void UpdateVisualCards()
        {
            if (players.Count == 2)
            {
                CardsFromCurrentTurn.Children.Clear();
                CardsFromNextTurn.Children.Clear();

                if (currentPlayer == 0)
                {
                    PlayerNameFromCurrentTurn.Text = "Current Turn: " + players[currentPlayer].name;

                    foreach (Card card in players[currentPlayer].cards)
                    {
                        BitmapImage bitmapImage = new BitmapImage(new Uri(card.source));

                        System.Windows.Controls.Image image = new System.Windows.Controls.Image()
                        {
                            Source = bitmapImage,
                            Width = 60
                        };

                        CardsFromCurrentTurn.Children.Add(image);
                    }

                    PlayerNameFromNextTurn.Text = "Next Turn: " + players[currentPlayer + 1].name;

                    foreach (Card card in players[currentPlayer + 1].cards)
                    {
                        BitmapImage bitmapImage = new BitmapImage(new Uri(card.source));

                        System.Windows.Controls.Image image = new System.Windows.Controls.Image()
                        {
                            Source = bitmapImage,
                            Width = 50,
                            Opacity = 0.7
                        };

                        CardsFromNextTurn.Children.Add(image);
                    }
                }
                else if (currentPlayer == 1)
                {
                    PlayerNameFromCurrentTurn.Text = "Current Turn: " + players[currentPlayer].name;

                    foreach (Card card in players[currentPlayer].cards)
                    {
                        BitmapImage bitmapImage = new BitmapImage(new Uri(card.source));

                        System.Windows.Controls.Image image = new System.Windows.Controls.Image()
                        {
                            Source = bitmapImage,
                            Width = 60
                        };

                        CardsFromCurrentTurn.Children.Add(image);
                    }

                    PlayerNameFromNextTurn.Text = "Next Turn: " + players[currentPlayer - 1].name;

                    foreach (Card card in players[currentPlayer - 1].cards)
                    {
                        BitmapImage bitmapImage = new BitmapImage(new Uri(card.source));

                        System.Windows.Controls.Image image = new System.Windows.Controls.Image()
                        {
                            Source = bitmapImage,
                            Width = 50,
                            Opacity = 0.7
                        };

                        CardsFromNextTurn.Children.Add(image);
                    }
                }
            }
            else if (players.Count >= 3)
            {
                CardsFromPreviousTurn.Children.Clear();
                CardsFromCurrentTurn.Children.Clear();
                CardsFromNextTurn.Children.Clear();

                if (currentPlayer == 0)
                {
                    PlayerNameFromPreviousTurn.Text = "Previous Turn: " + players[players.Count - 1].name;

                    foreach (Card card in players[players.Count - 1].cards)
                    {
                        BitmapImage bitmapImage = new BitmapImage(new Uri(card.source));

                        System.Windows.Controls.Image image = new System.Windows.Controls.Image()
                        {
                            Source = bitmapImage,
                            Width = 50,
                            Opacity = 0.7
                        };

                        CardsFromPreviousTurn.Children.Add(image);
                    }

                    PlayerNameFromCurrentTurn.Text = "Current Turn: " + players[currentPlayer].name;

                    foreach (Card card in players[currentPlayer].cards)
                    {
                        BitmapImage bitmapImage = new BitmapImage(new Uri(card.source));

                        System.Windows.Controls.Image image = new System.Windows.Controls.Image()
                        {
                            Source = bitmapImage,
                            Width = 60
                        };

                        CardsFromCurrentTurn.Children.Add(image);
                    }

                    PlayerNameFromNextTurn.Text = "Next Turn: " + players[currentPlayer + 1].name;

                    foreach (Card card in players[currentPlayer + 1].cards)
                    {
                        BitmapImage bitmapImage = new BitmapImage(new Uri(card.source));

                        System.Windows.Controls.Image image = new System.Windows.Controls.Image()
                        {
                            Source = bitmapImage,
                            Width = 50,
                            Opacity = 0.7
                        };

                        CardsFromNextTurn.Children.Add(image);
                    }
                }
                else if (currentPlayer == players.Count - 1)
                {
                    PlayerNameFromPreviousTurn.Text = "Previous Turn: " + players[currentPlayer - 1].name;

                    foreach (Card card in players[currentPlayer - 1].cards)
                    {
                        BitmapImage bitmapImage = new BitmapImage(new Uri(card.source));

                        System.Windows.Controls.Image image = new System.Windows.Controls.Image()
                        {
                            Source = bitmapImage,
                            Width = 50,
                            Opacity = 0.7
                        };

                        CardsFromPreviousTurn.Children.Add(image);
                    }

                    PlayerNameFromCurrentTurn.Text = "Current Turn: " + players[currentPlayer].name;

                    foreach (Card card in players[currentPlayer].cards)
                    {
                        BitmapImage bitmapImage = new BitmapImage(new Uri(card.source));

                        System.Windows.Controls.Image image = new System.Windows.Controls.Image()
                        {
                            Source = bitmapImage,
                            Width = 60
                        };

                        CardsFromCurrentTurn.Children.Add(image);
                    }

                    PlayerNameFromNextTurn.Text = "Next Turn: " + players[0].name;

                    foreach (Card card in players[0].cards)
                    {
                        BitmapImage bitmapImage = new BitmapImage(new Uri(card.source));

                        System.Windows.Controls.Image image = new System.Windows.Controls.Image()
                        {
                            Source = bitmapImage,
                            Width = 50,
                            Opacity = 0.7
                        };

                        CardsFromNextTurn.Children.Add(image);
                    }
                }
                else
                {
                    PlayerNameFromPreviousTurn.Text = "Previous Turn: " + players[currentPlayer - 1].name;

                    foreach (Card card in players[currentPlayer - 1].cards)
                    {
                        BitmapImage bitmapImage = new BitmapImage(new Uri(card.source));

                        System.Windows.Controls.Image image = new System.Windows.Controls.Image()
                        {
                            Source = bitmapImage,
                            Width = 50,
                            Opacity = 0.7
                        };

                        CardsFromPreviousTurn.Children.Add(image);
                    }

                    PlayerNameFromCurrentTurn.Text = "Current Turn: " + players[currentPlayer].name;

                    foreach (Card card in players[currentPlayer].cards)
                    {
                        BitmapImage bitmapImage = new BitmapImage(new Uri(card.source));

                        System.Windows.Controls.Image image = new System.Windows.Controls.Image()
                        {
                            Source = bitmapImage,
                            Width = 60
                        };

                        CardsFromCurrentTurn.Children.Add(image);
                    }

                    PlayerNameFromNextTurn.Text = "Next Turn: " + players[currentPlayer + 1].name;

                    foreach (Card card in players[currentPlayer + 1].cards)
                    {
                        BitmapImage bitmapImage = new BitmapImage(new Uri(card.source));

                        System.Windows.Controls.Image image = new System.Windows.Controls.Image()
                        {
                            Source = bitmapImage,
                            Width = 50,
                            Opacity = 0.7
                        };

                        CardsFromNextTurn.Children.Add(image);
                    }
                }
            }
        }

        public void AnnounceWinner(Player player)
        {
            FadeBackground.Visibility = Visibility.Visible;
            Canvas.SetZIndex(FadeBackground, 998); // covers everything behind Game_Content_Style with a opaque screen
            Announcing_Winner_Style.Visibility = Visibility.Visible;
            Canvas.SetZIndex(Announcing_Winner_Style, 999); // makes sure it appears in front of everything
            Winner_Phrase.Text = "Congratulations " + player.name + "!";
            Support_Winner_Text.Text = "You've won $" + pot + ". Thanks for playing!";
        }
    }
}
