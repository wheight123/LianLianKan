﻿using System;
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

        // game page auxiliary properties
        private Point firstPoint;
        private Point secondPoint;

        private Point foundPoint1;
        private Point foundPoint2;
        private Point resultPointList;

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

            // initialize game page auxiliary properties
            initGamePageAuxiliaryProperties();
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
                    if (type > 0)
                    {
                        setGamePanelBlockImageSourceNormal(image, type);
                    }
                    image.Tap += new EventHandler<GestureEventArgs>(blockTapped);
                    gameCanvas.Children.Add(image);
                    gamePanelBlockMatrix[i, j] = image;
                }
            }
        }

        // initialize game page auxiliary properties
        private void initGamePageAuxiliaryProperties()
        {
            firstPoint = new Point(-1, -1);
            secondPoint = new Point(-1, -1);
        }

        // game page initialize functions ending //
        //*******************************************************************//

        //*******************************************************************//
        // game main logic functions beginning //

        // image block tapped event response function
        private void blockTapped(object sender, GestureEventArgs e)
        {
            Image image = (Image)sender;
            String name = image.Name;
            String[] array = name.Split(new Char[] { '_' });
            int x = Int32.Parse(array[1]);
            int y = Int32.Parse(array[2]);
            if (isEmptyPoint(firstPoint))
            {
                firstPoint = new Point(x, y);
                updateImageToTapped(image, firstPoint);
            }
            else if (isEmptyPoint(secondPoint))
            {
                secondPoint = new Point(x, y);
                updateImageToTapped(image, secondPoint);
                List<Point> pointList = game.findPathBetweenPoints(firstPoint, secondPoint);
                textBox1.Text = "";
                foreach (Point point in pointList)
                {
                    textBox1.Text +="(" + point.X + "," + point.Y + "); ";
                }
                updateImageToNormal(firstPoint);
                updateImageToNormal(secondPoint);
                firstPoint = new Point(-1, -1);
                secondPoint = new Point(-1, -1);
            }
            //else // clear tapped image before finishing find path function
            //{
            //    updateImageToNormal(firstPoint);
            //    updateImageToNormal(secondPoint);
            //    firstPoint = new Point(-1, -1);
            //    secondPoint = new Point(-1, -1);
            //}
        }
        private void updateImageToTapped(Image image, Point point)
        {
            int x = (int)point.X;
            int y = (int)point.Y;
            gameCanvas.Children.Remove(image);

            int type = game.getGameZoneMatrix()[x, y];
            setGamePanelBlockImageSourceTapped(image, type);
            gameCanvas.Children.Add(image);
            gamePanelBlockMatrix[x, y] = image;
        }
        private void updateImageToNormal(Point point)
        { 
            int x = (int)point.X;
            int y = (int)point.Y;
            Image image = gamePanelBlockMatrix[x, y];
            gameCanvas.Children.Remove(image);

            int type = game.getGameZoneMatrix()[x, y];
            setGamePanelBlockImageSourceNormal(image, type);
            gamePanelBlockMatrix[x, y] = image;
            gameCanvas.Children.Add(image);
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

        // check whether is point is empty
        private Boolean isEmptyPoint(Point point)
        {
            int x = (int)point.X;
            int y = (int)point.Y;
            if (x == -1 && y == -1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // update game panel block
        private void updateGamePanelBlocks()
        {
 
            int[,] gameZoneMatrix = game.getGameZoneMatrix();
            for (int i = 0; i < ROW_AMOUNT; i++)
            {
                int y = gamePanelStartPointY + i * BLOCK_HEIGHT;
                for (int j = 0; j < COLUM_AMOUNT; j++)
                {
                    Image image = gamePanelBlockMatrix[i, j];
                    gameCanvas.Children.Remove(image);

                    int type = gameZoneMatrix[i, j];
                    if (type > 0)
                    {
                        setGamePanelBlockImageSourceNormal(image, type);
                        gameCanvas.Children.Add(image);
                        gamePanelBlockMatrix[i, j] = image;
                    }
                    else
                    {
                        int x = gamePanelStartPointX + j * BLOCK_WIDTH;
                        Image tempImage = new Image();
                        tempImage.Name = "blockImage_" + i + "_" + j;
                        Thickness imageThickness = new Thickness(x, y, 0, 0);
                        tempImage.Margin = imageThickness;
                        tempImage.Tap += new EventHandler<GestureEventArgs>(blockTapped);
                        gameCanvas.Children.Add(tempImage);
                        gamePanelBlockMatrix[i, j] = tempImage;
                    }
                }
            }
        }

        // internal auxiliary functions ending //
        //*******************************************************************//

        //*******************************************************************//
        private void btn_refresh_Click(object sender, RoutedEventArgs e)
        {
            game.refreshGameZoneMatrix();
            updateGamePanelBlocks();
        }
        //*******************************************************************//
    }
}