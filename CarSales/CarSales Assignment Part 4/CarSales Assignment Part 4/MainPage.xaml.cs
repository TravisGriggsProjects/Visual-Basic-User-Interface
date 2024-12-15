using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CarSales_Assignment_Part_4
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

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            customerPhoneTextBox.IsEnabled = false;
            customerNameTextBox.IsEnabled = false;
            vehiclePriceTextBox.Focus(FocusState.Programmatic);
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

        private void summaryButton_Click(object sender, RoutedEventArgs e)
        {
            const decimal GST_RATE = 0.1m;
            decimal subAmount;
            decimal gstAmount;
            decimal finalAmount;

            subAmount = decimal.Parse(vehiclePriceTextBox.Text) -
            decimal.Parse(lessTradeInTextBox.Text);
            subAmountTextBox.Text = subAmount.ToString("C");

            gstAmount = subAmount * GST_RATE;
            gstAmountTextBox.Text = gstAmount.ToString("C");

            finalAmount = subAmount + gstAmount;
            finalAmountTextBox.Text = finalAmount.ToString("C");
            summaryTextBlock.Text = "Summary data displayed here";
        }
        private void headingTextBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private void collectionDateTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }

}