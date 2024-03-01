using System;
using System.Collections.Generic;
using System.Linq;
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

        int NumberOfPlayers;
        List<Player> players = new List<Player>();
        int bet;
        int pot;

        public MainWindow()
        {
            InitializeComponent();
            DoNextTask();
        }

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
                    nextTask = Task.IntroducePlayers;
                }
            }

            if (nextTask == Task.GetBets)
            {
                Title_Phrase.Text = "Welcome " + players[TaskOrder].name + "!";
                Support_Text.Text = "How much would you like to bet? Your current bank stands at $" + players[TaskOrder].bank + ".";
                User_Input.Text = "*Input Value*";
            }

            if (nextTask == Task.IntroducePlayers)
            {
                Title_Phrase.Text = "Thank you for playing today!";
                Support_Text.Visibility = Visibility.Hidden;
                User_Input.Visibility = Visibility.Hidden;
            }
        }

        private void Continue_Button_Click(object sender, RoutedEventArgs e)
        {
            CheckTaskSuccess();
        }

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
        private void AddPlayer(string n)
        {
            players.Add(new Player(n));
        }

        private void GetPlayerNamesProcess()
        {
            if (User_Input.Text != "*Insert Text*" && User_Input.Text.Length >= 1 && User_Input.Text.Length <= 10)
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

        private void GetPlayerBetsProcess()
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
}
