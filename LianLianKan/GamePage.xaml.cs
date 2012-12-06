using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace LianLianKan
{
    public partial class GamePage : PhoneApplicationPage
    {
        private Game game;
        private const int BLOCK_WIDTH = 50;
        private const int BLOCK_HEIGHT = 50;
        private const int COLUM_AMOUNT = 10;
        private const int ROW_AMOUNT = 13;
        private const int BLOCK_TYPE = 10;

        private Image[,] gamePanelBackgroudMatrix;
        private Image[,] gamePanelBlockMatrix;
        private int gamePanelStartPointX;
        private int gamePanelStartPointY;

        public GamePage()
        {
            InitializeComponent();
            // initialize game
            game = new Game(ROW_AMOUNT, COLUM_AMOUNT, BLOCK_TYPE);
            // initialize game panel start point
            initGamePanelStartPoint();
            // initialize game panel background
            initGamePanelBackground();
            // initialize game panel blocks
            initGamePanelBlocks();
        }

        //*******************************************************************//
        // game page initialize functions beginning //

        // initialize game panel start point
        private void initGamePanelStartPoint()
        {
            int screenWidth = (int)System.Windows.Application.Current.Host.Content.ActualWidth;
            int screenHeight = (int)System.Windows.Application.Current.Host.Content.ActualHeight;
            gamePanelStartPointX = (screenWidth - COLUM_AMOUNT * BLOCK_WIDTH) / 2;
            gamePanelStartPointY = (screenHeight - ROW_AMOUNT * BLOCK_HEIGHT) / 2;
        }

        // initialize game panel background
        private void initGamePanelBackground()
        {
            gamePanelBackgroudMatrix = new Image[ROW_AMOUNT, COLUM_AMOUNT];
            for (int i = 0; i < ROW_AMOUNT; i++)
            {
                int y = gamePanelStartPointY + i * BLOCK_HEIGHT;
                for (int j = 0; j < COLUM_AMOUNT; j++)
                {
                    int x = gamePanelStartPointX + j * BLOCK_WIDTH;
                    Image image = new Image();
                    image.Name = "bgImage_" + i + "_" + j;
                    Thickness imageThickness = new Thickness(x, y, 0, 0);
                    image.Margin = imageThickness;
                    image.Source = bgBlock.Source;
                    gameCanvas.Children.Add(image);
                    gamePanelBackgroudMatrix[i, j] = image;
                }
            }
        }

        // initialize game panel block
        private void initGamePanelBlocks()
        {
            //Random random = new Random();
            gamePanelBlockMatrix = new Image[ROW_AMOUNT, COLUM_AMOUNT];
            int[,] gameZoneMatrix = game.getGameZoneMatrix();
            for (int i = 0; i < ROW_AMOUNT; i++)
            {
                int y = gamePanelStartPointY + i * BLOCK_HEIGHT;
                for (int j = 0; j < COLUM_AMOUNT; j++)
                {
                    int x = gamePanelStartPointX + j * BLOCK_WIDTH;
                    Image image = new Image();
                    image.Name = "blockImage_" + i + "_" + j;
                    Thickness imageThickness = new Thickness(x, y, 0, 0);
                    image.Margin = imageThickness;
                    int type = gameZoneMatrix[i, j];
                    setGamePanelBlockImageSourceNormal(image, type);
                    image.Tap +=new EventHandler<GestureEventArgs>(blockTapped);
                    gameCanvas.Children.Add(image);
                    gamePanelBlockMatrix[i, j] = image;
                }
            }
        }

        // game page initialize functions ending //
        //*******************************************************************//

        //*******************************************************************//
        // game main logic functions beginning //
        private void blockTapped(object sender, GestureEventArgs e)
        {
            Image image = (Image)sender;
            String name = image.Name;
            String[] array = name.Split(new Char[] { '_' });
            int x = Int32.Parse(array[1]);
            int y = Int32.Parse(array[2]);
            gameCanvas.Children.Remove(image);
            int type = game.getGameZoneMatrix()[x ,y];
            setGamePanelBlockImageSourceTapped(image, type);
            gameCanvas.Children.Add(image);
        }
        private int randomType()
        {
            Random random = new Random();
            int result = random.Next() % BLOCK_TYPE + 1;
            return result;
        }

        // game main logic functions ending //
        //*******************************************************************//

        //*******************************************************************//
        // internal auxiliary functions beginning //
        
        // set game panle block image source normally
        private void setGamePanelBlockImageSourceNormal(Image image, int type)
        {
            switch (type)
            {
                case 1:
                    image.Source = num1_1.Source;
                    break;
                case 2:
                    image.Source = num2_1.Source;
                    break;
                case 3:
                    image.Source = num3_1.Source;
                    break;
                case 4:
                    image.Source = num4_1.Source;
                    break;
                case 5:
                    image.Source = num5_1.Source;
                    break;
                case 6:
                    image.Source = num6_1.Source;
                    break;
                case 7:
                    image.Source = num7_1.Source;
                    break;
                case 8:
                    image.Source = num8_1.Source;
                    break;
                case 9:
                    image.Source = num9_1.Source;
                    break;
                case 10:
                    image.Source = num10_1.Source;
                    break;
            }
        }
        // set game panle block image source when block tapped
        private void setGamePanelBlockImageSourceTapped(Image image, int type)
        {
            switch (type)
            {
                case 1:
                    image.Source = num1_2.Source;
                    break;
                case 2:
                    image.Source = num2_2.Source;
                    break;
                case 3:
                    image.Source = num3_2.Source;
                    break;
                case 4:
                    image.Source = num4_2.Source;
                    break;
                case 5:
                    image.Source = num5_2.Source;
                    break;
                case 6:
                    image.Source = num6_2.Source;
                    break;
                case 7:
                    image.Source = num7_2.Source;
                    break;
                case 8:
                    image.Source = num8_2.Source;
                    break;
                case 9:
                    image.Source = num9_2.Source;
                    break;
                case 10:
                    image.Source = num10_2.Source;
                    break;
            }
        }
        // internal auxiliary functions ending //
        //*******************************************************************//
    }
}