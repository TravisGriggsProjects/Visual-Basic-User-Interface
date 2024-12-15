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

namespace CarSalesV3_TravisGriggs
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
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
            //The following code initally sets the warranty cost to zero and subsequenctly determines the cost based on the selected warranty duration
            double warrantyCost = 0;
            const double TWO_YEAR_INSURANCE = 0.05;
            const double THREE_YEAR_INSURANCE = 0.10;
            const double FIVE_YEAR_INSURANCE = 0.20;
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
            //The following code initally sets the extra cost to zero and subsequenctly determines the cost based on which optional extra are selected.
            //This is archive through the use of the += operator which addes the value on the right to the vairble on the left
            double extrasCost = 0;
            if (windowTintingCheckBox.IsChecked == true) extrasCost += 150;
            if (ducoProtectionCheckBox.IsChecked == true) extrasCost += 180;
            if (floorMatsCheckBox.IsChecked == true) extrasCost += 320;
            if (deluxeSoundSystemCheckBox.IsChecked == true) extrasCost += 350;
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
            const double UNDERAGE_DRIVER_PREMIUM = 0.20;
            const double OVERAGE_DRIVER_PREMIUM = 0.10;

            //The following code initally checks if the insurance toggle switch is on
            //If this is the case then depedning on which policy is choosen the code returns a value for the insurace cost based on a predetermined percentage of the total of vehicle price and optional extras
            if (insurancePolicyToggleSwitch.IsOn)
            {
                if (driversUnderAgeRadioButton.IsChecked == true)
                {
                    return (vehiclePrice + optionalExtras) * UNDERAGE_DRIVER_PREMIUM;
                }
                else if (olderDriversRadioButton.IsChecked == true)
                {
                    return (vehiclePrice + optionalExtras) * OVERAGE_DRIVER_PREMIUM;
                }
            }
            return 0;
        }

        /// <summary>
        /// Summary Button Click Event Method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

            double warrantyCost = calcVehicleWarranty(vehiclePrice);
            double optionalExtrasCost = calcOptionalExtras();
            double insuranceCost = calcAccidentInsurance(vehiclePrice, optionalExtrasCost);

            subAmount = (decimal)(vehiclePrice + warrantyCost + optionalExtrasCost + insuranceCost - lessTradeIn);
            subAmountTextBox.Text = subAmount.ToString("C");

            gstAmount = subAmount * GST_RATE;
            gstAmountTextBox.Text = gstAmount.ToString("C");

            finalAmount = subAmount + gstAmount;
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
            if (insurancePolicyToggleSwitch.IsOn)
            {
                driversUnderAgeRadioButton.IsEnabled = true;
                olderDriversRadioButton.IsEnabled = true;
                olderDriversRadioButton.IsChecked = true;
            }
            else
            {
                driversUnderAgeRadioButton.IsEnabled = false;
                olderDriversRadioButton.IsEnabled = false;
                driversUnderAgeRadioButton.IsChecked = false;
                olderDriversRadioButton.IsChecked = false;
            }
        }

        //Array Creation//
        private string[] customerNames = new string[10];
        private string[] customerPhones = new string[10];
        private string[] vehicleMakes;

        /// <summary>
        /// Populate Arrays Method
        /// </summary>
        private void populateArrays()
        {
            //Array of random first/last names and vehicles
            string[] firstNames = {"John", "Jane", "Alex", "Alice", "Michael", "Ruby", "David", "Ethan", "James", "Liam", "Noah", "Emma", "Amelia", "Mia", "Lucas"
};
            string[] lastNames = { "Smith", "Johnson", "Williams", "Cherry", "Jones", "Allen", "Reid", "Blair", "Thomas", "Spencer", "Jones", "Beck", "Cooper", "Samuel", "Bailey", "Hughes", "Ross", "Fox", "Clare", "Wood", "Ronin", "Morris" };

            vehicleMakes = new string[] { "Toyota", "Holden", "Mitsubishi", "Ford", "BMW", "Mazda", "Volkswagen", "Mini" };

            //random method to return random parameters//
            Random random = new Random();

            //The following for loop starts at i=0 and after each loop addes one. This loop continues to repeat while i<10//
            for (int i = 0; i < 10; i++)
            {
                //defines first name and last name as a random entry for the first name and last name array//
                string firstName = firstNames[random.Next(firstNames.Length)];
                string lastName = lastNames[random.Next(lastNames.Length)];
                customerNames[i] = $"{firstName} {lastName}";
                //defines random number as any number between 10000000 and 99999999//
                int randomNumber = random.Next(10000000, 99999999);
                customerPhones[i] = $"04{randomNumber}";
            }
        }

        /// <summary>
        /// Main Page Loaded Method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //The following code is called on when the page is loaded//
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            populateArrays();

            //Sort vehicleMakes[] array in alphabetical order
            Array.Sort(vehicleMakes);
        }

        /// <summary>
        /// display customer method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void displayAllCustomersButton_Click(object sender, RoutedEventArgs e)
        {
            summaryTextBlock.Text = "Customers:\n";
            //For-loop to print customerNames[i] & customerPhones[i]
            //The following for loop starts at i=0 and after each loop addes one. This loop continues to repeat until i = customerNames.Length //
            for (int i = 0; i < customerNames.Length; i++)
            {
                summaryTextBlock.Text += $"{customerNames[i]} - {customerPhones[i]}\n";
            }
        }

        /// <summary>
        /// Search Name Method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void searchNameButton_Click(object sender, RoutedEventArgs e)
        {
            //Defines name to search as the content of the customer name textbox//
            string nameToSearch = customerNameTextBox.Text;

            //If loop which outputs error meassage when the customer name textbox is left empty/null//
            if (nameToSearch == "")
            {
                await new MessageDialog("Please enter a name.").ShowAsync();
                customerNameTextBox.Focus(FocusState.Programmatic);
                return;
            }

            displayAllCustomersButton_Click(sender, e);

            //The following for loop starts at i=0 and after each loop addes one. This loop continues to repeat while i<customerNames.Length//
            for (int i = 0; i < customerNames.Length; i++)
            {
                //The following if loop check each element of the customer names array against the nameToSerach, in the event that they match then found is set to true and the loop is broken.//
                if (customerNames[i] == nameToSearch)
                {
                    customerPhoneTextBox.Text = customerPhones[i];
                    return;
                }
            }

        //Code to output error message if the name is not found//
        await new MessageDialog("Customer not found.").ShowAsync();  
    }

        /// <summary>
        /// Delete Name Method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void deleteNameButton_Click(object sender, RoutedEventArgs e)
        {
            //Defines name to delete as the content of the customer name textbox//
            string nameToDelete = customerNameTextBox.Text;

            //If loop which outputs error meassage when the customer name textbox is left empty/null//
            if (nameToDelete == "")
            {
                await new MessageDialog("Please enter a name.").ShowAsync(); ;
                customerNameTextBox.Focus(FocusState.Programmatic);
                return;
            }

            //The following for loop starts at i=0 and increase by steps of 1 until i = customerNames.Length//
            for (int i = 0; i < customerNames.Length; i++)
            {
                //If-statement executed when customerNames[i] equals the entered name to delete//
                if (customerNames[i] == nameToDelete)
                {
                    //Defining variables deletedName & deletedPhone
                    string deletedName = customerNames[i];
                    string deletedPhone = customerPhones[i];

                    //For-loop to move each array element down one incirement to account for the delete entry//
                    for (int j = i; j < customerNames.Length - 1; j++)
                    {
                        customerNames[j] = customerNames[j + 1];
                        customerPhones[j] = customerPhones[j + 1];
                    }

                    //Reduce customerNames[] and customerPhones[] array lengths by 1 to account for the deleted entry//
                    Array.Resize(ref customerNames, customerNames.Length - 1);
                    Array.Resize(ref customerPhones, customerPhones.Length - 1);

                    //Message to output deleted customer name/phone//
                    await new MessageDialog($"{deletedName} - {deletedPhone} has been deleted. New length: {customerNames.Length}").ShowAsync(); ;

                    //Display All Customers event call to display the updated list of names and phonenumbers in the summary textbox//
                    displayAllCustomersButton_Click(sender, e);

                    //Return to break the For-loop//
                    return;
                }
            }

            //Output error message if name to delete is not found by For-loop//
            await new MessageDialog("Customer not found.").ShowAsync();
        }    
      
        /// <summary>
        /// Display Makes Method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void displayAllMakesButton_Click(object sender, RoutedEventArgs e)
        {
            //The following codes sorts the elements in the vehicle make into alphabetical order and then outputs each element to the summary textbox//
            Array.Sort(vehicleMakes);
            summaryTextBlock.Text = "Vehicle Makes:\n";
            foreach (var make in vehicleMakes)
            {
                summaryTextBlock.Text += $"{make}\n";
            }
        }

        /// <summary>
        /// Binary Search Method
        /// </summary>
        /// <param name="vehicleMakes"></param>
        /// <param name="item"></param>
        /// <returns>index number of vehicle make. Note: this method returns -1 if the vehicle make in not found</returns>
        public int binarySearch(string[] vehicleMakes, string item)
        {
            //Defining variables min & max
            int min = 0;
            int max = vehicleMakes.Length - 1;

            //While loop for binary serach
            while (min <= max)
            {
                int mid = (min + max) / 2;
                if (vehicleMakes[mid].ToUpper() == item)  //if the item is found return the index mid                                                   
                    return mid;

                if (item.CompareTo(vehicleMakes[mid].ToUpper()) > 0)   //check if the item wanted is in the top half of the search 
                    min = mid + 1;      //set the min part of the search to the mid +1
                else
                    max = mid - 1;      //otherwise the item must be in the lower half of the search, set max to the mid-1

            }

            return -1;                  // -1 is returned when not found
        }


        /// <summary>
        /// Search Vehicle Make Method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void binarySearchVehicleMakeButton_Click(object sender, RoutedEventArgs e)
        {
            //If loop which outputs error meassage when the vehicle make textbox is left empty/null//
            if (string.IsNullOrWhiteSpace(vehicleMakeTextBox.Text))
            {
                await new MessageDialog("Vehicle make cannot be blank.").ShowAsync();
                vehicleMakeTextBox.Focus(FocusState.Programmatic);
                return;
            }

            //Defines make to search as the content of the vehicle make textbox//
            string makeToSearch = vehicleMakeTextBox.Text.ToUpper();

            //The following code uses a Binary Search algorithm method which works by checking the value in the center of the array.//
            //If the target value is lower, the next value to check is in the center of the left half of the array. Similarly, if the target value is higher, the next value to check is in the center of the right half of the array.//
            //This process of halving the search area happens until the target value is found, or until the search area of the array is empty.//
            int index = binarySearch(vehicleMakes, makeToSearch); //call the arrayBinarySearch method
            if (index == -1)
            {
                var dialogMessage = new MessageDialog(makeToSearch + " not found ");
                await dialogMessage.ShowAsync();
            }
            else
            {
                var dialogMessage = new MessageDialog(makeToSearch + " found at index " + index);
                await dialogMessage.ShowAsync();
            }
        }

        /// <summary>
        /// Insert Vehicle Make Method 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void insertVehicleMakeButton_Click(object sender, RoutedEventArgs e)

        {
            //Defines make to insert as the content of the vehicle make textbox//
            string makeToInsert = vehicleMakeTextBox.Text;

            //If loop which outputs error meassage when the vehicle make textbox is left empty/null//
            if (string.IsNullOrWhiteSpace(vehicleMakeTextBox.Text))
            {
                await new MessageDialog("Vehicle make cannot be blank.").ShowAsync();
                vehicleMakeTextBox.Focus(FocusState.Programmatic);
                return;
            }

            int vehicleIndex = binarySearch(vehicleMakes, makeToInsert); //call the arrayBinarySearch method

            //The following if loop addes the make to insert to the vehicle makes array, if index was found to be less than 0//
            //If the index in not less than 0 then the if statement outputs an error message stating the vehicle make already exists//
            if (vehicleIndex < 0)
            {
                Array.Resize(ref vehicleMakes, vehicleMakes.Length + 1);
                vehicleMakes[vehicleMakes.Length - 1] = makeToInsert;
                Array.Sort(vehicleMakes);

                displayAllMakesButton_Click(sender, e);
            }
            else
            {
                await new MessageDialog($"{makeToInsert} already exists.").ShowAsync();
            }
        }

        private void headingTextBlock_SelectionChanged()
        {

        }
    }
}
