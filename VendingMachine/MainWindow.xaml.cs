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
using System.Windows.Media.Animation;


namespace VendingMachine
{
    public class ItemRingGroup
    {
        public Image Item { get; set; }
        public RotateTransform ItemRotation { get; set; }
        public List<Image> Rings { get; set; }  // Может быть 1 или 2 кольца
        public List<RotateTransform> Rotations { get; set; } 
        //public double source_item_top {  get; set; }
        //public double source_item_left { get; set; }

        public Image SourceItem { get; set;  }

        public ItemRingGroup() {
            Rings = new List<Image>();
            Rotations = new List<RotateTransform>();
        }
        public ItemRingGroup(Image item, RotateTransform item_rotation, List<Image> rings, List<RotateTransform> rotations, Image source_item)
        {
            Item = item;
            ItemRotation = item_rotation;
            Rings = rings;
            Rotations = rotations;
            SourceItem = source_item;
            //source_item_left = Canvas.GetLeft(item);
            //source_item_top = Canvas.GetTop(item);
        }
    }

    public partial class MainWindow : Window
    {
        private int totalAmount = 0;
        private bool discountApplied = false;
        private string enteredText = "";

        private Dictionary<string, int> code_price = new Dictionary<string, int>
        {
            { "A01", 59 },
            { "A02", 68 },
            { "A03", 63 },
            { "A04", 85 },
            { "A05", 129 },
            { "A06", 74 },
            { "A07", 69 },
            { "A08", 79 },
            { "A09", 45 },
            { "A10", 299 },
            { "A11", 409 },
            { "A12", 115 },
            { "A13", 32 },
            { "A14", 78 },
            { "A15", 549 },
            { "A16", 119 },
            { "A17", 169 },
            { "A18", 499 },
            { "A19", 74 },
            { "A20", 129 },
            { "A21", 822 },
            { "A22", 51 },
            { "A23", 169 },
            { "A24", 145 },
            { "A25", 260 },
            { "A26", 54 },
            { "A27", 34 },
            { "A28", 60 },
            { "A29", 134 },
            { "A30", 25 },
            { "A31", 29 },
            { "A32", 42 }
        };
        private Dictionary<int, Image> value_money;
        private Dictionary<string, ItemRingGroup> code_item;
        private Image[] lids;

        private bool isAnimationBanknote = false;
        private bool isAnimationCoin = false;
        private bool isAnimationCard = false;
        private bool isAnimationItem = false;
        private Queue<string> animationItemQueue = new Queue<string>();
        private bool isAnimationCoinOut = false;
    

        private double previousLeft;
        private double previousTop;

        public MainWindow()
        {
            InitializeComponent();

            value_money = new Dictionary<int, Image>
            {
                { 1, rub1Image },
                { 2, rub2Image },
                { 5, rub5Image },
                { 10, rub10Image },
                { 50, rub50Image },
                { 100, rub100Image },
                { 1000000, rub100Image} // for debug
            };

            code_item = new Dictionary<string, ItemRingGroup>
            {
                {"A01", new ItemRingGroup {Item = itemA01, ItemRotation=itemA01Rotation, Rings = new List<Image> { ringA01 }, Rotations = new List<RotateTransform> { ringA01Rotation }, SourceItem=itemA01_back } },
                {"A02", new ItemRingGroup {Item = itemA02, ItemRotation=itemA02Rotation, Rings = new List<Image> { ringA02 }, Rotations = new List<RotateTransform> { ringA02Rotation }, SourceItem=itemA02_back } },
                {"A03", new ItemRingGroup {Item = itemA03, ItemRotation=itemA03Rotation, Rings = new List<Image> { ringA03 }, Rotations = new List<RotateTransform> { ringA03Rotation }, SourceItem=itemA03_back } },
                {"A04", new ItemRingGroup {Item = itemA04, ItemRotation=itemA04Rotation, Rings = new List<Image> { ringA04 }, Rotations = new List<RotateTransform> { ringA04Rotation }, SourceItem=itemA04_back } },
                {"A05", new ItemRingGroup {Item = itemA05, ItemRotation=itemA05Rotation, Rings = new List<Image> { ringA05 }, Rotations = new List<RotateTransform> { ringA05Rotation }, SourceItem=itemA05_back } },
                {"A06", new ItemRingGroup {Item = itemA06, ItemRotation=itemA06Rotation, Rings = new List<Image> { ringA06 }, Rotations = new List<RotateTransform> { ringA06Rotation }, SourceItem=itemA06_back } },
                {"A07", new ItemRingGroup {Item = itemA07, ItemRotation=itemA07Rotation, Rings = new List<Image> { ringA07 }, Rotations = new List<RotateTransform> { ringA07Rotation }, SourceItem=itemA07_back } },
                {"A08", new ItemRingGroup {Item = itemA08, ItemRotation=itemA08Rotation, Rings = new List<Image> { ringA08 }, Rotations = new List<RotateTransform> { ringA08Rotation }, SourceItem=itemA08_back } },
                {"A09", new ItemRingGroup {Item = itemA09, ItemRotation=itemA09Rotation, Rings = new List<Image> { ringA09 }, Rotations = new List<RotateTransform> { ringA09Rotation }, SourceItem=itemA09_back } },
                {"A10", new ItemRingGroup {Item = itemA10, ItemRotation=itemA10Rotation, Rings = new List<Image> { ringA10 }, Rotations = new List<RotateTransform> { ringA10Rotation }, SourceItem=itemA10_back } },
                {"A11", new ItemRingGroup {Item = itemA11, ItemRotation=itemA11Rotation, Rings = new List<Image> { ringA11 }, Rotations = new List<RotateTransform> { ringA11Rotation }, SourceItem=itemA11_back } },
                {"A12", new ItemRingGroup {Item = itemA12, ItemRotation=itemA12Rotation, Rings = new List<Image> { ringA12 }, Rotations = new List<RotateTransform> { ringA12Rotation }, SourceItem=itemA12_back } },
                {"A13", new ItemRingGroup {Item = itemA13, ItemRotation=itemA13Rotation, Rings = new List<Image> { ringA13 }, Rotations = new List<RotateTransform> { ringA13Rotation }, SourceItem=itemA13_back } },
                {"A14", new ItemRingGroup {Item = itemA14, ItemRotation=itemA14Rotation, Rings = new List<Image> { ringA14 }, Rotations = new List<RotateTransform> { ringA14Rotation }, SourceItem=itemA14_back } },
                {"A15", new ItemRingGroup {Item = itemA15, ItemRotation=itemA15Rotation, Rings = new List<Image> { ringA15 }, Rotations = new List<RotateTransform> { ringA15Rotation }, SourceItem=itemA15_back } },
                {"A16", new ItemRingGroup {Item = itemA16, ItemRotation=itemA16Rotation, Rings = new List<Image> { ringA16 }, Rotations = new List<RotateTransform> { ringA16Rotation }, SourceItem=itemA16_back } },

                {"A17", new ItemRingGroup {Item = itemA17, ItemRotation=itemA17Rotation, Rings = new List<Image> { ringA17, ringA17_1 }, Rotations = new List<RotateTransform> { ringA17Rotation, ringA17_1Rotation }, SourceItem=itemA17_back } },
                {"A18", new ItemRingGroup {Item = itemA18, ItemRotation=itemA18Rotation, Rings = new List<Image> { ringA18, ringA18_1 }, Rotations = new List<RotateTransform> { ringA18Rotation, ringA18_1Rotation }, SourceItem=itemA18_back } },
                {"A19", new ItemRingGroup {Item = itemA19, ItemRotation=itemA19Rotation, Rings = new List<Image> { ringA19, ringA19_1 }, Rotations = new List<RotateTransform> { ringA19Rotation, ringA19_1Rotation }, SourceItem=itemA19_back } },
                {"A20", new ItemRingGroup {Item = itemA20, ItemRotation=itemA20Rotation, Rings = new List<Image> { ringA20, ringA20_1 }, Rotations = new List<RotateTransform> { ringA20Rotation, ringA20_1Rotation }, SourceItem=itemA20_back } },
                {"A21", new ItemRingGroup {Item = itemA21, ItemRotation=itemA21Rotation, Rings = new List<Image> { ringA21, ringA21_1 }, Rotations = new List<RotateTransform> { ringA21Rotation, ringA21_1Rotation }, SourceItem=itemA21_back } },
                {"A22", new ItemRingGroup {Item = itemA22, ItemRotation=itemA22Rotation, Rings = new List<Image> { ringA22, ringA22_1 }, Rotations = new List<RotateTransform> { ringA22Rotation, ringA22_1Rotation }, SourceItem=itemA22_back } },
                {"A23", new ItemRingGroup {Item = itemA23, ItemRotation=itemA23Rotation, Rings = new List<Image> { ringA23, ringA23_1 }, Rotations = new List<RotateTransform> { ringA23Rotation, ringA23_1Rotation }, SourceItem=itemA23_back } },
                {"A24", new ItemRingGroup {Item = itemA24, ItemRotation=itemA24Rotation, Rings = new List<Image> { ringA24, ringA24_1 }, Rotations = new List<RotateTransform> { ringA24Rotation, ringA24_1Rotation }, SourceItem=itemA24_back } },

                {"A25", new ItemRingGroup {Item = itemA25, ItemRotation=itemA25Rotation, Rings = new List<Image> { ringA25 }, Rotations = new List<RotateTransform> { ringA25Rotation }, SourceItem=itemA25_back } },
                {"A26", new ItemRingGroup {Item = itemA26, ItemRotation=itemA26Rotation, Rings = new List<Image> { ringA26 }, Rotations = new List<RotateTransform> { ringA26Rotation }, SourceItem=itemA26_back } },
                {"A27", new ItemRingGroup {Item = itemA27, ItemRotation=itemA27Rotation, Rings = new List<Image> { ringA27 }, Rotations = new List<RotateTransform> { ringA27Rotation }, SourceItem=itemA27_back } },
                {"A28", new ItemRingGroup {Item = itemA28, ItemRotation=itemA28Rotation, Rings = new List<Image> { ringA28 }, Rotations = new List<RotateTransform> { ringA28Rotation }, SourceItem=itemA28_back } },
                {"A29", new ItemRingGroup {Item = itemA29, ItemRotation=itemA29Rotation, Rings = new List<Image> { ringA29 }, Rotations = new List<RotateTransform> { ringA29Rotation }, SourceItem=itemA29_back } },
                {"A30", new ItemRingGroup {Item = itemA30, ItemRotation=itemA30Rotation, Rings = new List<Image> { ringA30 }, Rotations = new List<RotateTransform> { ringA30Rotation }, SourceItem=itemA30_back } },
                {"A31", new ItemRingGroup {Item = itemA31, ItemRotation=itemA31Rotation, Rings = new List<Image> { ringA31 }, Rotations = new List<RotateTransform> { ringA31Rotation }, SourceItem=itemA31_back } },
                {"A32", new ItemRingGroup {Item = itemA32, ItemRotation=itemA32Rotation, Rings = new List<Image> { ringA32 }, Rotations = new List<RotateTransform> { ringA32Rotation }, SourceItem=itemA32_back } }
            };

            lids = new Image[]
            {
                lid_closed,
                lid_half_closed,
                lid_almost_opened,
                lid_opened
            };

            //CoinPopup.IsOpen = true;
            //BanknotePopup.IsOpen = true;
            //CardPopup.IsOpen = true;

            // для Window_LocationChanged
            //previousLeft = this.Left;
            //previousTop = this.Top;

        }

        // хотел чтобы при изменении позиции окна в компе,
        // popup окна сохраняли свою позицию относительно автомата
        // но пока забил на это болт...

        //private void Window_LocationChanged(object sender, System.EventArgs e)
        //{
        //    MessageBox.Show("Window_LocationChanged вызвано!");
        //    if (CoinPopup.IsOpen)
        //    {
        //        double deltaX = this.Left - previousLeft;
        //        double deltaY = this.Top - previousTop;
        //        CoinPopup.HorizontalOffset += deltaX;
        //        CoinPopup.VerticalOffset += deltaY;
        //    }
        //    if (BanknotePopup.IsOpen)
        //    {
        //        double deltaX = this.Left - previousLeft;
        //        double deltaY = this.Top - previousTop;
        //        BanknotePopup.HorizontalOffset += deltaX;
        //        BanknotePopup.VerticalOffset += deltaY;
        //    }
        //    if (CardPopup.IsOpen)
        //    {
        //        double deltaX = this.Left - previousLeft;
        //        double deltaY = this.Top - previousTop;
        //        CardPopup.HorizontalOffset += deltaX;
        //        CardPopup.VerticalOffset += deltaY;
        //    }
        //    previousLeft = this.Left;
        //    previousTop = this.Top;
        //}
        
        // не удалять, другая версия анимации выдачи сдачи, тоже очень хорошая
        // можно спокойно использовать эти 2 функции, работаю нормально
        
        //private async void CoinOutAnimation(int value)
        //{
        //    Image cur_coin = value_money[value];

        //    hideCoinOutImage.Visibility = Visibility.Visible;
        //    cur_coin.Visibility = Visibility.Visible;

        //    Canvas.SetLeft(cur_coin, 472);
        //    Canvas.SetTop(cur_coin, 521);
        //    // Анимация движения вниз
        //    DoubleAnimation moveDown = new DoubleAnimation
        //    {
        //        From = 521,
        //        To = 593,
        //        Duration = TimeSpan.FromSeconds(1) // 1 секунда на движение
        //    };
        //    cur_coin.BeginAnimation(Canvas.TopProperty, moveDown);

        //    await Task.Delay(1000); // Ждем завершения анимации (1 секунда)

        //    hideCoinOutImage.Visibility = Visibility.Hidden;

        //    // Анимация движения вправо
        //    DoubleAnimation moveRight = new DoubleAnimation
        //    {
        //        From = 472,
        //        To = 644,
        //        Duration = TimeSpan.FromSeconds(1), // 1 секунда на движение
        //        //EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
        //    };
        //    cur_coin.BeginAnimation(Canvas.LeftProperty, moveRight);

        //    await Task.Delay(1000); // Ждем завершения анимации (1 секунда)

        //    cur_coin.BeginAnimation(Canvas.TopProperty, null); // Останавливаем анимацию Bottom
        //    cur_coin.BeginAnimation(Canvas.LeftProperty, null);

        //    cur_coin.Visibility = Visibility.Hidden;
        //    Canvas.SetLeft(cur_coin, 662);
        //    Canvas.SetTop(cur_coin, 413);
        //}


        //private async void returnMoney(object sender, RoutedEventArgs e)
        //{
        //    enteredText = "";

        //    int amount10 = totalAmount / 10;
        //    int tempAmount = totalAmount % 10;
        //    int amount5 = tempAmount / 5;
        //    tempAmount %= 5;
        //    int amount2 = tempAmount / 2;
        //    tempAmount %= 2;
        //    int amount1 = tempAmount;

        //    for (int i = 0; i < amount10; i++)
        //    {
        //        mainScreen.Content = $"RETURN\nMONEY:\n{totalAmount} ₽";
        //        totalAmount -= 10;
        //        // вывод анимации монеты в 10 рублей
        //        CoinOutAnimation(10);
        //        await Task.Delay(2500); // wait animation
        //    }

        //    for (int i = 0; i < amount5; i++)
        //    {
        //        mainScreen.Content = $"RETURN MONEY:\n{totalAmount} ₽";
        //        totalAmount -= 5;
        //        // вывод анимации монеты в 5 рублей
        //        CoinOutAnimation(5);
        //        await Task.Delay(2500); // wait animation
        //    }

        //    for (int i = 0; i < amount2; i++)
        //    {
        //        mainScreen.Content = $"RETURN MONEY:\n{totalAmount} ₽";
        //        totalAmount -= 2;
        //        // вывод анимации монеты в 2 рублей
        //        CoinOutAnimation(2);
        //        await Task.Delay(2500); // wait animation
        //    }

        //    for (int i = 0; i < amount1; i++)
        //    {
        //        mainScreen.Content = $"RETURN MONEY:\n{totalAmount} ₽";
        //        totalAmount -= 1;
        //        // вывод анимации монеты в 1 рублей
        //        CoinOutAnimation(1);
        //        await Task.Delay(2500); // wait animation
        //    }

        //    mainScreen.Content = $"{totalAmount} ₽";
        //}
        private async Task CoinOutAnimation(int value)
        {
            Image cur_coin = value_money[value];

            hideCoinOutImage.Visibility = Visibility.Visible;
            cur_coin.Visibility = Visibility.Visible;

            Canvas.SetLeft(cur_coin, 472);
            Canvas.SetTop(cur_coin, 521);

            // Анимация движения вниз
            DoubleAnimation moveDown = new DoubleAnimation
            {
                From = 521,
                To = 593,
                Duration = TimeSpan.FromSeconds(1)
            };
            TaskCompletionSource<bool> tcsDown = new TaskCompletionSource<bool>();
            moveDown.Completed += (s, e) => tcsDown.SetResult(true);
            cur_coin.BeginAnimation(Canvas.TopProperty, moveDown);
            await tcsDown.Task; // Ждем завершения анимации вниз

            hideCoinOutImage.Visibility = Visibility.Hidden;

            // Анимация движения вправо
            DoubleAnimation moveRight = new DoubleAnimation
            {
                From = 472,
                To = 644,
                Duration = TimeSpan.FromSeconds(1)
            };
            TaskCompletionSource<bool> tcsRight = new TaskCompletionSource<bool>();
            moveRight.Completed += (s, e) => tcsRight.SetResult(true);
            cur_coin.BeginAnimation(Canvas.LeftProperty, moveRight);
            await tcsRight.Task; // Ждем завершения анимации вправо

            cur_coin.BeginAnimation(Canvas.TopProperty, null);
            cur_coin.BeginAnimation(Canvas.LeftProperty, null);
            cur_coin.Visibility = Visibility.Hidden;
            Canvas.SetLeft(cur_coin, 662);
            Canvas.SetTop(cur_coin, 413);
        }

        private async void returnMoney(object sender, RoutedEventArgs e)
        {
            while (isAnimationCoinOut)
            {
                await Task.Delay(100);
            }

            isAnimationCoinOut = true;

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
                await CoinOutAnimation(10); // Ожидаем завершения анимации
            }

            for (int i = 0; i < amount5; i++)
            {
                mainScreen.Content = $"RETURN MONEY:\n{totalAmount} ₽";
                totalAmount -= 5;
                await CoinOutAnimation(5);
            }

            for (int i = 0; i < amount2; i++)
            {
                mainScreen.Content = $"RETURN MONEY:\n{totalAmount} ₽";
                totalAmount -= 2;
                await CoinOutAnimation(2);
            }

            for (int i = 0; i < amount1; i++)
            {
                mainScreen.Content = $"RETURN MONEY:\n{totalAmount} ₽";
                totalAmount -= 1;
                await CoinOutAnimation(1);
            }

            mainScreen.Content = $"{totalAmount} ₽";

            isAnimationCoinOut = false;

        }

        private ItemRingGroup doRingRotateAndReturnItemRingsImages(string mainScreenText)
        {
            ItemRingGroup obj = code_item[mainScreenText];
            List<DoubleAnimation> rotateRings = new List<DoubleAnimation>();
            DoubleAnimation rotateAnimation = new DoubleAnimation
            {
                From = 0,
                To = 360,
                Duration = TimeSpan.FromSeconds(4),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };
            for (int i = 0; i < obj.Rotations.Count; i++)
            {
                obj.Rotations[i].BeginAnimation(RotateTransform.AngleProperty, rotateAnimation);
            }
            return obj;
        }

        private void swap_ring_item(ItemRingGroup obj, int ring_i, int item_i)
        {
            for (int i = 0; i < obj.Rings.Count; i++)
            {
                Canvas.SetZIndex(obj.Rings[i], ring_i);
            }
            Canvas.SetZIndex(obj.Item, item_i);
        }
        private async Task ItemAnimation(string mainScreenText)
        {

            //while (isAnimationItem)
            //{
            //    await Task.Delay(100);
            //}

            //isAnimationItem = true;

            ItemRingGroup obj = doRingRotateAndReturnItemRingsImages(mainScreenText);

            await Task.Delay(1000); // wait ?
            //swap_ring_item(obj, 2, 3); // ring and item have just swapped
            Canvas.SetZIndex(obj.Item, 4); // for covering items and rings,
                                           // but be under lid and external body of vendmachine
            await Task.Delay(1000);

            //obj.Item.BeginAnimation(Canvas.TopProperty, null);
            //obj.ItemRotation.BeginAnimation(RotateTransform.AngleProperty, null);

            DoubleAnimation moveDown = new DoubleAnimation
            {
                From = Canvas.GetTop(obj.Item),
                To = 620,
                Duration = TimeSpan.FromSeconds(2)
            };
            DoubleAnimation rotateItem = new DoubleAnimation
            {
                From = 0,
                To = 360,
                Duration = TimeSpan.FromSeconds(2),
                RepeatBehavior = RepeatBehavior.Forever
            };

            obj.Item.BeginAnimation(Canvas.TopProperty, moveDown);
            obj.ItemRotation.BeginAnimation(RotateTransform.AngleProperty, rotateItem);

            await Task.Delay(2500);

            obj.Item.BeginAnimation(Canvas.TopProperty, null);
            obj.ItemRotation.BeginAnimation(RotateTransform.AngleProperty, null);

            Canvas.SetLeft(obj.Item, 222);
            Canvas.SetTop(obj.Item, 669);

            for (int i = 0; i < lids.Length - 1; i++)
            {
                lids[i].Visibility = Visibility.Hidden;
                lids[i + 1].Visibility = Visibility.Visible;
                await Task.Delay(500);
            }

            Canvas.SetZIndex(obj.Item, 6);
            DoubleAnimation moveLeft = new DoubleAnimation
            {
                From = Canvas.GetLeft(obj.Item),
                To = -59,
                Duration = TimeSpan.FromSeconds(2)
            };
            obj.Item.BeginAnimation(Canvas.LeftProperty, moveLeft);


            for (int i = lids.Length - 1; i > 0; i--)
            {
                lids[i].Visibility = Visibility.Hidden;
                lids[i - 1].Visibility = Visibility.Visible;
                await Task.Delay(500);
            }
            await Task.Delay(2000);

            obj.Item.BeginAnimation(Canvas.LeftProperty, null);

            Canvas.SetZIndex(obj.Item, 2);
            Canvas.SetLeft(obj.Item, Canvas.GetLeft(obj.SourceItem));
            Canvas.SetTop(obj.Item, Canvas.GetTop(obj.SourceItem));

            //isAnimationItem = false;
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
                animationItemQueue.Enqueue(mainScreenText);

                while (animationItemQueue.Count > 0)
                {
                    string nextItem = animationItemQueue.Dequeue();
                    await ItemAnimation(nextItem);
                }
                //ItemAnimation(mainScreenText);
            }
            else
            {
                mainScreen.Content = "NEED MORE\nMONEY ₽"; // ERR
            }
            await Task.Delay(5000);
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

        private async Task CoinAnimation(int value)
        {

            while (isAnimationCoin)
            {
                await Task.Delay(100);
            }

            isAnimationCoin = true;

            Image cur_coin = value_money[value];

            cur_coin.Visibility = Visibility.Visible;
            // Анимация движения влево
            DoubleAnimation moveLeft = new DoubleAnimation
            {
                From = 662,
                To = 490,
                Duration = TimeSpan.FromSeconds(1) // 1 секунда на движение
            };
            cur_coin.BeginAnimation(Canvas.LeftProperty, moveLeft);

            await Task.Delay(1000); // Ждем завершения анимации (1 секунда)

            hideCoinImage.Visibility = Visibility.Visible;
            
            // Анимация движения вверх
            DoubleAnimation moveUp = new DoubleAnimation
            {
                From = 413,
                To = 375,
                Duration = TimeSpan.FromSeconds(1), // 1 секунда на движение
                //EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };
            cur_coin.BeginAnimation(Canvas.TopProperty, moveUp);

            await Task.Delay(1000); // Ждем завершения анимации (1 секунда)

            cur_coin.BeginAnimation(Canvas.LeftProperty, null); // Останавливаем анимацию Left
            cur_coin.BeginAnimation(Canvas.TopProperty, null);

            cur_coin.Visibility = Visibility.Hidden;
            Canvas.SetLeft(cur_coin, 662);
            Canvas.SetTop(cur_coin, 413);
            hideCoinImage.Visibility = Visibility.Hidden;

            isAnimationCoin = false;
        }

        private async void CoinButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && int.TryParse(button.Tag.ToString(), out int value))
            {
                CoinPopup.IsOpen = false;
                await CoinAnimation(value);
                await Task.Delay(2500); // wait until animation gone
                totalAmount += value;
                mainScreen.Content = $"{totalAmount} ₽";
            }
        }

        private async Task BanknoteAnimation(int value)
        {

            while (isAnimationBanknote)
            {
                await Task.Delay(100); // Короткая задержка перед повторной проверкой
            }

            isAnimationBanknote = true;

            Image cur_banknote = value_money[value];

            cur_banknote.Visibility = Visibility.Visible;
            // Анимация движения влево
            DoubleAnimation moveLeft = new DoubleAnimation
            {
                From = 670,
                To = 468,
                Duration = TimeSpan.FromSeconds(1) // 1 секунда на движение
            };
            cur_banknote.BeginAnimation(Canvas.LeftProperty, moveLeft);

            await Task.Delay(1000); // Ждем завершения анимации (1 секунда)

            hideBanknoteImage.Visibility = Visibility.Visible;

            // Анимация движения вверх
            DoubleAnimation moveUp = new DoubleAnimation
            {
                From = 381,
                To = 280,
                Duration = TimeSpan.FromSeconds(1), // 1 секунда на движение
                //EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };
            cur_banknote.BeginAnimation(Canvas.TopProperty, moveUp);

            await Task.Delay(1000); // Ждем завершения анимации (1 секунда)

            cur_banknote.BeginAnimation(Canvas.LeftProperty, null); // Останавливаем анимацию Left
            cur_banknote.BeginAnimation(Canvas.TopProperty, null);

            cur_banknote.Visibility = Visibility.Hidden;
            Canvas.SetLeft(cur_banknote, 670);
            Canvas.SetTop(cur_banknote, 381);
            hideBanknoteImage.Visibility = Visibility.Hidden;

            isAnimationBanknote = false;

        }
        private async void BanknoteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && int.TryParse(button.Tag.ToString(), out int value))
            {
                BanknotePopup.IsOpen = false;
                await BanknoteAnimation(value);
                await Task.Delay(2500);
                totalAmount += value;
                mainScreen.Content = $"{totalAmount} ₽";
            }
        }

        private async Task vipCardAnimation()
        {

            while (isAnimationCard)
            {
                await Task.Delay(100);
            }

            isAnimationCard = true;

            VipCardImage.Visibility = Visibility.Visible;
            // Анимация движения влево (с 653 до 453)
            DoubleAnimation moveLeft = new DoubleAnimation
            {
                From = 653,
                To = 453,
                Duration = TimeSpan.FromSeconds(1) // 1 секунда на движение
            };
            VipCardImage.BeginAnimation(Canvas.LeftProperty, moveLeft);

            await Task.Delay(1000); // Ждем завершения анимации (1 секунда)

            // Пауза 0.5 секунды
            await Task.Delay(500);

            // Анимация возвращения вправо (с 453 до 653)
            DoubleAnimation moveRight = new DoubleAnimation
            {
                From = 453,
                To = 653,
                Duration = TimeSpan.FromSeconds(1) // 1 секунда на возвращение
            };
            VipCardImage.BeginAnimation(Canvas.LeftProperty, moveRight);
            await Task.Delay(1000); // Ждем завершения анимации (1 секунда)
            VipCardImage.Visibility = Visibility.Hidden;
            isAnimationCard = false;
        }

        private async void CardButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                CardPopup.IsOpen = false;
                if (button.Tag.ToString() == "Yes")
                {
                    await vipCardAnimation(); // анимация карточки
                    discountApplied = true;
                    await Task.Delay(1000); // wait until card animation works
                    mainScreen.Content = "DISC 50%";
                    await Task.Delay(1000);
                    mainScreen.Content = $"{totalAmount} ₽";
                }
                else
                {
                    discountApplied = false;
                }
            }
        }
    }
}