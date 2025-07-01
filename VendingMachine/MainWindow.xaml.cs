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
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.RegularExpressions;


namespace VendingMachine
{
    public partial class MainWindow : Window
    {
        private int totalAmount = 0;
        private bool discountApplied = false;
        private string enteredText = "";
        private Dictionary<string, int> code_price = new Dictionary<string, int> { { "A01", 59 }, { "A02", 68 }, { "A17", 169 } };

        private double previousLeft;
        private double previousTop;

        public MainWindow()
        {
            InitializeComponent();
            previousLeft = this.Left;
            previousTop = this.Top;
            //CoinPopup.IsOpen = true;
            //BanknotePopup.IsOpen = true;
            //CardPopup.IsOpen = true;

        }

        private void Window_LocationChanged(object sender, System.EventArgs e)
        {
            MessageBox.Show("Window_LocationChanged вызвано!");
            if (CoinPopup.IsOpen)
            {
                double deltaX = this.Left - previousLeft;
                double deltaY = this.Top - previousTop;
                CoinPopup.HorizontalOffset += deltaX;
                CoinPopup.VerticalOffset += deltaY;
            }
            if (BanknotePopup.IsOpen)
            {
                double deltaX = this.Left - previousLeft;
                double deltaY = this.Top - previousTop;
                BanknotePopup.HorizontalOffset += deltaX;
                BanknotePopup.VerticalOffset += deltaY;
            }
            if (CardPopup.IsOpen)
            {
                double deltaX = this.Left - previousLeft;
                double deltaY = this.Top - previousTop;
                CardPopup.HorizontalOffset += deltaX;
                CardPopup.VerticalOffset += deltaY;
            }
            previousLeft = this.Left;
            previousTop = this.Top;
        }


        private async void checkTotalAmount_giveItemOrNot(string mainScreenText)
        {
            int totalPrice = Convert.ToInt32(Math.Ceiling(code_price[mainScreenText] * (0.5 + 0.5 * (discountApplied ? 0 : 1))));
            mainScreen.Content = $"NEED: {totalPrice}₽\nHAVE: {totalAmount}₽";
            await Task.Delay(5000);
            if (totalPrice <= totalAmount)
            {
                mainScreen.Content = "SUCCESS";
                totalAmount -= totalPrice;
                if (discountApplied)
                {
                    discountApplied = false;
                }
                // сделать анимацию падения сраного товара
            }
            else
            {
                mainScreen.Content = "NEED MORE\nMONEY ₽"; // ERR
                // сделать после некоторого времени сообщение "LOW MONEY"
            }
            await Task.Delay(5000);
            mainScreen.Content = $"{totalAmount} ₽";
        }

        private async void returnMoney(object sender, RoutedEventArgs e)
        {
            enteredText = "";

            int amount10 = totalAmount / 10;
            int tempAmount = totalAmount % 10;
            int amount5 = tempAmount / 5;
            tempAmount %= 5;
            int amount2 = tempAmount / 2;
            tempAmount %= 2;
            int amount1 = tempAmount;

            for (int i = 0; i < amount10; i++)
            {
                mainScreen.Content = $"RETURN\nMONEY:\n{totalAmount} ₽";
                totalAmount -= 10;
                // вывод анимации монеты в 10 рублей
                await Task.Delay(1000); //temp
            }

            for (int i = 0; i < amount5; i++)
            {
                mainScreen.Content = $"RETURN MONEY:\n{totalAmount} ₽";
                totalAmount -= 5;
                // вывод анимации монеты в 5 рублей
                await Task.Delay(1000); //temp
            }

            for (int i = 0; i < amount2; i++)
            {
                mainScreen.Content = $"RETURN MONEY:\n{totalAmount} ₽";
                totalAmount -= 2;
                // вывод анимации монеты в 2 рублей
                await Task.Delay(1000); //temp
            }

            for (int i = 0; i < amount1; i++)
            {
                mainScreen.Content = $"RETURN MONEY:\n{totalAmount} ₽";
                totalAmount -= 1;
                // вывод анимации монеты в 1 рублей
                await Task.Delay(1000); //temp
            }

            mainScreen.Content = $"{totalAmount} ₽";
        }

        private async void noKey_printErr()
        {
            mainScreen.Content = "ERR";
            await Task.Delay(1500);
            mainScreen.Content = $"{totalAmount} ₽";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                string mainScreenText = enteredText; // mainScreen.Content.ToString();
                string buttonText = button.Content.ToString();

                //if (mainScreenText[mainScreenText.Length - 1] == '₽'
                //    || mainScreenText == "ERR"
                //    || mainScreenText == "DISC 50%")




                //mainScreen.Content = enteredText;


                if (buttonText == "C")
                {
                    if (mainScreenText.Length <= 1)
                    {
                        mainScreen.Content = $"{totalAmount} ₽";
                        enteredText = "";
                    }
                    else
                    {
                        enteredText = enteredText.Substring(0, enteredText.Length - 1);
                        mainScreen.Content = enteredText;
                    }
                }
                else if (buttonText == " ")
                {
                    // возврат денег
                    returnMoney(sender, e);
                }
                else if (buttonText == "OK")
                {
                    // вычет суммы
                    if (code_price.ContainsKey(mainScreenText))
                    {
                        checkTotalAmount_giveItemOrNot(mainScreenText);
                    }
                    else
                    {
                        noKey_printErr();
                    }
                    enteredText = "";
                }
                else if (mainScreenText.Length < 3)
                {
                    enteredText += buttonText;
                    mainScreen.Content = enteredText;
                }
                //MessageBox.Show($"Нажата кнопка: {buttonText}");
            }
        }

        private void CoinAcceptor_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("CoinAcceptor_Click!!!");
            CoinPopup.IsOpen = true;
        }

        private void BanknoteAcceptor_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("BanknoteAcceptor_Click!!!");
            BanknotePopup.IsOpen = true;
        }

        private void CardReader_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("CardReader_Click!!!");
            CardPopup.IsOpen = true;
        }

        private void CoinButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && int.TryParse(button.Tag.ToString(), out int value))
            {
                totalAmount += value;
                mainScreen.Content = $"{totalAmount} ₽";
                CoinPopup.IsOpen = false;
            }
        }

        private void BanknoteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && int.TryParse(button.Tag.ToString(), out int value))
            {
                totalAmount += value;
                mainScreen.Content = $"{totalAmount} ₽";
                BanknotePopup.IsOpen = false;
            }
        }

        private async void CardButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                if (button.Tag.ToString() == "Yes")
                {
                    discountApplied = true;
                    mainScreen.Content = "DISC 50%";
                    await Task.Delay(1000);
                    mainScreen.Content = $"{totalAmount} ₽";
                    // анимация карточки
                }
                else
                {
                    discountApplied = false;
                }
                CardPopup.IsOpen = false;
            }
        }
    }
}