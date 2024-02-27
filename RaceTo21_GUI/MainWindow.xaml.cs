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

namespace RaceTo21_GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int TaskNumber = 0;
        bool TaskSuccess = false;
        int NumberOfPlayers;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Continue_Button_Click(object sender, RoutedEventArgs e)
        {
            if (TaskNumber == 0)
            {
                if (int.TryParse(User_Input.Text, out NumberOfPlayers) == false || NumberOfPlayers < 2 || NumberOfPlayers > 8)
                {
                    User_Input.Text = "Invalid, try again.";
                }
                else
                {
                    Title_Phrase.Text = "Welcome Players!";
                    Support_Text.Text = "Please input your names";
                    TaskSuccess = true; 
                }
                Title_Phrase.Text = "Welcome Players!";
                Support_Text.Text = "Please input your names";
            }
            if (TaskNumber == 1)
            {
                Title_Phrase.Text = "Betting Phase";
                Support_Text.Text = "How much would you like to bet? Your current bank total is $100.";
            }

            if (TaskSuccess == true)
            {
                TaskNumber++;
                TaskSuccess = false;
            }
        }
    }
}
