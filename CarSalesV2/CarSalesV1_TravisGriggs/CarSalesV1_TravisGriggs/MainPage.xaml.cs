using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CarSalesV1_TravisGriggs
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void saveButton_Click(object sender, RoutedEventArgs e)
        {
            if (customerNameTextBox.Text == "")
            {
                var dialogMessage = new MessageDialog("Error! Please enter a valid Name");
                await dialogMessage.ShowAsync();
                customerNameTextBox.Focus(FocusState.Programmatic);
            }
            else
            {
                customerNameTextBox.IsEnabled = false;


                if (customerPhoneTextBox.Text == "")
                {
                    var dialogMessage = new MessageDialog("Error! Please enter a valid phone number");
                    await dialogMessage.ShowAsync();
                    customerPhoneTextBox.Focus(FocusState.Programmatic);
                }
                else
                {
                    customerPhoneTextBox.IsEnabled = false;
                    vehiclePriceTextBox.Focus(FocusState.Programmatic);
                }
            }
        }

        private void resetButton_Click(object sender, RoutedEventArgs e)
        {
            customerPhoneTextBox.Text = "";
            customerNameTextBox.Text = "";
            vehiclePriceTextBox.Text = "";
            lessTradeInTextBox.Text = "";
            subAmountTextBox.Text = "";
            gstAmountTextBox.Text = "";
            finalAmountTextBox.Text = "";
            customerPhoneTextBox.IsEnabled = true;
            customerNameTextBox.IsEnabled = true;
            customerNameTextBox.Focus(FocusState.Programmatic);
        }

        private async void summaryButton_Click(object sender, RoutedEventArgs e)
        {
            const decimal GST_RATE = 0.1m;
            decimal subAmount;
            decimal gstAmount;
            decimal finalAmount;
            double vehiclePrice;
            double lessTradeIn;

            //The following try/catch statments checks if the content in the vehicle price textbox is a double precision floating point number.
            //If this test fails it catches the exceptions through outputing an approriabte error message, returning the code to before the test, and return focus to the vehicle price textbox. .
            //This allows for the user to correctly input a valid number to the vehicle price textbox and allows for the content of the textbox to be tested again.
            try
            {
                vehiclePrice = double.Parse(vehiclePriceTextBox.Text);
            }

            catch
            {
                var dialogMessage = new MessageDialog("Error! Please enter a vehicle price.");
                await dialogMessage.ShowAsync();
                vehiclePriceTextBox.Focus(FocusState.Programmatic);
                vehiclePriceTextBox.SelectAll();
                return;
            }

            //The following if statement validates that the vehicle price is greater than 0.
            //This is achived through outputing an error message if the vehicle price is <= 0, returning focus to the vehicle price textbox for the user to re-input a valid number,
            //and return the code to before the if statement so as to allow the content of the textbox to be tested/validated again.
            if (vehiclePrice <= 0)
            {
                var dialogMessage = new MessageDialog("Error! Vehicle price must be greater than $0");
                await dialogMessage.ShowAsync();
                vehiclePriceTextBox.Focus(FocusState.Programmatic);
                vehiclePriceTextBox.SelectAll();
                return;
            }           

            //The following if statement checks if the content of the lessTradeIn textbox is left blank, and if that is the case it sets it to 0.
            if (lessTradeInTextBox.Text == "")
            {
                lessTradeInTextBox.Text = "0";                
            }

            //The following try/catch statments checks if the content in the less trade in textbox is a double precision floating point number.
            //If this test fails it catches the exceptions through outputing an approriabte error message, returning the code to before the test, and returns focus to the lessTradeIn textbox.
            //This allows for the user to correctly input a valid entry to the less trade in textbox and allows for the content of the textbox to be tested again.

            try
            {
                lessTradeIn = double.Parse(lessTradeInTextBox.Text);
            }

            catch
            {
                var dialogMessage = new MessageDialog("Error! Please enter a valid less trade in value.");
                await dialogMessage.ShowAsync();
                lessTradeInTextBox.Focus(FocusState.Programmatic);
                lessTradeInTextBox.SelectAll();
                return;
                

            }

            //The following if statement validates that the less trade in is greater than or equal to 0.
            //This is achived through outputing an error message if the vehicle price is < 0, returning focus to the less trade in textbox for the user to re-input a valid number,
            //and return the code to before the if statement so as to allow the content of the textbox to be tested/validated again.
            if (lessTradeIn < 0)
            {
                var dialogMessage = new MessageDialog("Error! Less trade in must be greater than $0.");
                await dialogMessage.ShowAsync();
                lessTradeInTextBox.Focus(FocusState.Programmatic);
                lessTradeInTextBox.SelectAll();
                return;
            }

            //The following if statement validates that the less trade in is less than the vehicle price.
            //This is achived through outputing an error message if the less trade in is greater than the vehicle price, returning focus to the less trade in textbox for the user to re-input a valid number,
            //and return the code to before the if statement so as to allow the content of the textbox to be tested/validated again.
            if (lessTradeIn > vehiclePrice)
            {
                var dialogMessage = new MessageDialog("Error! Vehicle price must be greater than less trade in.");
                await dialogMessage.ShowAsync();
                lessTradeInTextBox.Focus(FocusState.Programmatic);
                lessTradeInTextBox.SelectAll();
                return;
            }

            subAmount = decimal.Parse(vehiclePriceTextBox.Text) - decimal.Parse(lessTradeInTextBox.Text);
            subAmountTextBox.Text = subAmount.ToString("C");

            gstAmount = subAmount * GST_RATE;
            gstAmountTextBox.Text = gstAmount.ToString("C");

            finalAmount = subAmount + gstAmount;
            finalAmountTextBox.Text = finalAmount.ToString("C");
            summaryTextBlock.Text = "Summary data displayed here";
        }
    }
}
