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

namespace CarSalesV2_TravisGriggs
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        /// <summary>
        /// MainPage
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Save Button Click Event Method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>    
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

        /// <summary>
        /// Reset Button Click Event Method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            windowTintingCheckBox.IsChecked = false;
            ducoProtectionCheckBox.IsChecked = false;
            floorMatsCheckBox.IsChecked = false;
            deluxeSoundSystemCheckBox.IsChecked = false;
            vehicleWarrantyComboBox.SelectedIndex = 0; // Default to 1 year warranty
            insurancePolicyToggleSwitch.IsOn = false;
            driversUnderAgeRadioButton.IsChecked = false;
            olderDriversRadioButton.IsChecked = false;
        }

        /// <summary>
        /// Calculating Warranty Method
        /// </summary>
        /// <param name="vehiclePrice"></param>
        /// <returns>the double warrantyCost calculated based on selected insurance</returns>
        private double calcVehicleWarranty(double vehiclePrice)
        {
            //Define constants for insurance policy costs
            const double TWO_YEAR_INSURANCE = 0.05;
            const double THREE_YEAR_INSURANCE = 0.10;
            const double FIVE_YEAR_INSURANCE = 0.20;

            //The following code initally sets the warranty cost to zero and subsequenctly employs a switch statement to determine the warrenty cost based on the selected warranty duration
            double warrantyCost = 0;
            switch (vehicleWarrantyComboBox.SelectedIndex)
            {
                // If 2 years warranty is selected, calculate 5% of vehicle price
                case 1:
                    warrantyCost = vehiclePrice * TWO_YEAR_INSURANCE;
                    break;
                // If 3 years warranty is selected, calculate 10% of vehicle price
                case 2:
                    warrantyCost = vehiclePrice * THREE_YEAR_INSURANCE;
                    break;
                // If 5 years warranty is selected, calculate 20% of vehicle price
                case 3:
                    warrantyCost = vehiclePrice * FIVE_YEAR_INSURANCE;
                    break;
                // If 1 year warranty is selected cost remains 0 (1 year warranty is free)
                default:
                    warrantyCost = 0;
                    break;
            }
            return warrantyCost;
        }

        /// <summary>
        /// Calculating Optional Extras Method
        /// </summary>
        /// <returns>the double extrasCost based on the selected constant double cost assoicated with each optional extra</returns>
        private double calcOptionalExtras()
        {
            //Defining constants for optional extras costs
            const double WINDOW_TINTING = 150;
            const double DUCO_PROTECTION = 180;
            const double FLOOR_MATS = 320;
            const double DELUXE_SOUND_SYSTEM = 350;

            //The following code initally sets the extra cost to zero and subsequenctly determines the cost based on which optional extra are selected.
            //This is archive through the use of the += operator which addes the value on the right to the vairble on the left
            double extrasCost = 0;
            if (windowTintingCheckBox.IsChecked == true) extrasCost += WINDOW_TINTING;
            if (ducoProtectionCheckBox.IsChecked == true) extrasCost += DUCO_PROTECTION;
            if (floorMatsCheckBox.IsChecked == true) extrasCost += FLOOR_MATS;
            if (deluxeSoundSystemCheckBox.IsChecked == true) extrasCost += DELUXE_SOUND_SYSTEM;
            return extrasCost;
        }

        /// <summary>
        /// Calculating Accident Insurance Method
        /// </summary>
        /// <param name="vehiclePrice"></param>
        /// <param name="optionalExtras"></param>
        /// <returns>the double insuranceCost (vehiclePrice + optionalExtras) based on selected insurace premium. Note if no premium is selected it returns 0</returns>
        private double calcAccidentInsurance(double vehiclePrice, double optionalExtras)
        {
            //Defining constants for insurance premiums
            const double UNDERAGE_DRIVER_PREMIUM = 0.20;
            const double OLDER_DRIVER_PREMIUM = 0.10;

            //If statment to calculate insurance price depending on which policy is choosen
            if (driversUnderAgeRadioButton.IsChecked == true)
            {
                return (vehiclePrice + optionalExtras) * UNDERAGE_DRIVER_PREMIUM;
            }
            else if (olderDriversRadioButton.IsChecked == true)
            {
                return (vehiclePrice + optionalExtras) * OLDER_DRIVER_PREMIUM;
            }
            //else statement to return 0 if no insurance is selected
            else
                return 0;
        }

        /// <summary>
        /// Summary Button Click Event Method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void summaryButton_Click(object sender, RoutedEventArgs e)
        {
            //Defining constants for summary button
            const double GST_RATE = 0.1;
            double subAmount;
            double gstAmount;
            double finalAmount;
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

            //Defining vairables for warrantyCost, optionalExtrasCost, & insurance cost as double
            double warrantyCost = calcVehicleWarranty(vehiclePrice);
            double optionalExtrasCost = calcOptionalExtras();
            double insuranceCost = calcAccidentInsurance(vehiclePrice, optionalExtrasCost);

            //subAmount calculation
            subAmount = (vehiclePrice + warrantyCost + optionalExtrasCost + insuranceCost - lessTradeIn);
            subAmountTextBox.Text = subAmount.ToString("C");

            //gstAmount calculation
            gstAmount = subAmount * GST_RATE;
            gstAmountTextBox.Text = gstAmount.ToString("C");

            //finalAmount calculation
            finalAmount = subAmount + gstAmount;

            //Summary textblock output, converting calculated final amount into a curracy formatted string
            finalAmountTextBox.Text = finalAmount.ToString("C");
            summaryTextBlock.Text = $"Customer Name: {customerNameTextBox.Text}\n\n" +
                                    $"Phone: {customerPhoneTextBox.Text}\n\n" +
                                    $"Vehicle Cost: {vehiclePrice:C}\n\n" +
                                    $"Trade-In Amount: {lessTradeIn:C}\n\n" +
                                    $"Warranty Cost: {warrantyCost:C}\n\n" +
                                    $"Optional Extras Cost: {optionalExtrasCost:C}\n\n" +
                                    $"Insurance Cost: {insuranceCost:C}\n\n" +
                                    $"Sub Amount: {subAmount:C}\n\n" +
                                    $"GST Amount: {gstAmount:C}\n\n" +
                                    $"Final Amount: {finalAmount:C}";
        }

        /// <summary>
        /// Insurance Policy Toggle Switch Method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void insurancePolicyToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            //If statement to execute when toggle switch is turned on, enabling desired radio buttons
            if (insurancePolicyToggleSwitch.IsOn)
            {
                driversUnderAgeRadioButton.IsEnabled = true;
                olderDriversRadioButton.IsEnabled = true;
                olderDriversRadioButton.IsChecked = true;
            }
            //else statement to execute when toggle switch is turned off, disabling desired radio buttons
            else
            {
                driversUnderAgeRadioButton.IsEnabled = false;
                olderDriversRadioButton.IsEnabled = false;
                driversUnderAgeRadioButton.IsChecked = false;
                olderDriversRadioButton.IsChecked = false;
            }
        }
    }
}