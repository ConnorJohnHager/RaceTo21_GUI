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
        public int NumberOfPlayers;

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
                Support_Text.Text = "How many players? (Between 2-8)";
                User_Input.Text = "*Input Value*";

                if (TaskSuccess == true)
                {
                    TaskSuccess = false;
                    nextTask = Task.GetNames;
                }
            }

            if (nextTask == Task.GetNames)
            {
                Title_Phrase.Text = "Welcome players!";
                Support_Text.Text = "Player #1: What is your name? (Limit to 10 characters)";
                User_Input.Text = "*Input Name*";

                if (TaskSuccess == true)
                {
                    TaskSuccess = false;
                    nextTask = Task.GetNames;
                }
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

        private void GetPlayerNamesProcess()
        {
            if (User_Input.Text != "*Insert Text*" && User_Input.Text.Length >= 1 || User_Input.Text.Length <= 10)
            {

            }
            else
            {
                User_Input.Text = "*Invalid, try again*";
            }
        }
    }
}
