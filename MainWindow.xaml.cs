using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Security.Cryptography;
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

using System.Windows.Threading; //import timer

namespace racingGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer gameTimer = new DispatcherTimer();
        List<Rectangle> itemRemover = new List<Rectangle>();

        Random rand = new Random();

        ImageBrush playerImage = new ImageBrush();
        ImageBrush batImage = new ImageBrush();

        Rect playerHitbox;

        int speed = 15;
        int playerSpeed = 10;
        int carNum;
        int batCounter = 50;
        int powerModeCounter = 200;

        double score;
        double i;

        bool moveLeft, moveRight, gameOver, powerMode;

        public MainWindow()
        {
            InitializeComponent();

            myCanvas.Focus(); //listens for key up and down

            gameTimer.Tick += GameLoop;
            gameTimer.Interval = TimeSpan.FromMilliseconds(15);

            startGame();
        }

        private void GameLoop(object sender, EventArgs e)
        {
            score += .05;

            batCounter -= 1;

            scoreText.Content = "Survived " + score.ToString("#.#") + " Seconds";

            playerHitbox = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width, player.Height);

            if (moveLeft == true && Canvas.GetLeft(player) > 0)
            {
                Canvas.SetLeft(player, Canvas.GetLeft(player) - playerSpeed);
            }
            if (moveRight == true && Canvas.GetLeft(player) + 90 < Application.Current.MainWindow.Width)
            {
                Canvas.SetLeft(player, Canvas.GetLeft(player) + playerSpeed);
            }

            if (batCounter < 1)
            {
                MakeBat();
                batCounter = rand.Next(600, 900);
            }

            foreach(var x in myCanvas.Children.OfType<Rectangle>())
            {
                if((string)x.Tag == "roadMarks")
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) + speed);

                    if (Canvas.GetTop(x) > 710) 
                    {
                        Canvas.SetTop(x, -252);
                    }
                }
                if((string)x.Tag == "Car")
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) + speed);

                    if (Canvas.GetTop(x) > 700)
                    {
                        ChangeCars(x);
                    }

                    Rect carHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    if (playerHitbox.IntersectsWith(carHitBox) && powerMode == true)
                    {
                        ChangeCars(x);
                    }
                    else if (playerHitbox.IntersectsWith(carHitBox) && powerMode == false)
                    {
                        gameTimer.Stop();
                        scoreText.Content += " Press Enter to replay ";
                        gameOver = true;
                    }
                }

                if ((string)x.Tag == "bat")
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) + 5);
                    Rect batHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    if (playerHitbox.IntersectsWith(batHitBox)) 
                    {
                        itemRemover.Add(x);

                        powerMode = true;

                        powerModeCounter = 800;
                    }

                    if(Canvas.GetTop(x)> 700)
                    {
                        itemRemover.Add(x);
                    }
                }
                if (powerMode == true)
                {
                    powerModeCounter -= 1;
                    PowerUp();

                    if (powerModeCounter < 1)
                    {
                        powerMode = false;
                    }
                }
                else
                {
                    playerImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/batMobile.png"));
                    myCanvas.Background = Brushes.Gray;
                }
            }
            foreach(Rectangle y in itemRemover)
            {
                myCanvas.Children.Remove(y);
            }

            if (score >= 10 && score < 20)
            {
                speed = 12;
            }

            if (score >=20 && score < 30)
            {
                speed = 14;
            }
            if (score >= 30 && score < 40)
            {
                speed = 16;
            }
            if (score >= 40 && score < 50)
            {
                speed = 18;
            }
            if (score >= 50 && score < 80)
            {
                speed = 20;
            }
        }

        private void onKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                moveLeft = true;
            }
            if (e.Key == Key.Right)
            {
                moveRight = true;
            }
        }

        private void onKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                moveLeft = false;
            }
            if (e.Key == Key.Right)
            {
                moveRight = false;
            }

            if (e.Key == Key.Enter && gameOver == true)
            {
                startGame();
            }
        }

        private void startGame() {
            speed = 8;
            gameTimer.Start();

            moveLeft = false;
            moveRight = false;
            gameOver = false;
            powerMode = false;

            score = 0;

            scoreText.Content = "Survived: 0 Seconds";

            playerImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/batMobile.png"));
            batImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/batLogo.png"));

            player.Fill = playerImage;

            myCanvas.Background = Brushes.Gray;

            foreach (var x in myCanvas.Children.OfType<Rectangle>()) // want other cars to be put on screen
            {
                if((string)x.Tag == "Car") // check for Car tag 
                {
                    Canvas.SetTop(x, (rand.Next(100, 600) * -1)); // 
                    Canvas.SetLeft(x, rand.Next(0, 630));
                    ChangeCars(x);
                }

                if ((string)x.Tag == "bat")
                {
                    itemRemover.Add(x); // 
                }
            }
            itemRemover.Clear();
        }

        private void ChangeCars(Rectangle car) {
            carNum = rand.Next(1, 3);

            ImageBrush carImage = new ImageBrush();

            switch (carNum)
            {
                case 1:
                    carImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/carBlue8bit.png"));
                    break;
                case 2:
                    carImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/carGreen8bit.png"));
                    break;
                case 3:
                    carImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/carRed8bit.png"));
                    break;
            }

            car.Fill = carImage; //change the car style based on rand input

            Canvas.SetTop(car, (rand.Next(100, 300) * -1));
            Canvas.SetLeft(car, rand.Next(0, 300));
        }

        private void PowerUp() {
            i += .5;

            if (i > 4)
            {
                i = 1;
            }

            switch (i) //switch through each possible start version
            {
                case 1:
                    playerImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/batMobile.png"));
                    break;
                case 2:
                    playerImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/batMobile.png"));
                    break;
                case 3:
                    playerImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/batMobile.png"));
                    break;
                case 4:
                    playerImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/batMobile.png"));
                    break;
            }

            myCanvas.Background = Brushes.LightCoral;
        }

        private void MakeBat()
        {
            Rectangle newBat = new Rectangle // placing the bat logo on the canvas
            {
                Height = 70,
                Width = 70,
                Tag = "bat",
                Fill = batImage
            };

            Canvas.SetLeft(newBat, rand.Next(0, 430));
            Canvas.SetTop(newBat, (rand.Next(100, 400) * -1));

            myCanvas.Children.Add(newBat);

        }
    }
}
