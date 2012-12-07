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
using System.Windows.Threading;

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
        private Image[] gameRemainAmountArray;
        private int gamePanelStartPointX;
        private int gamePanelStartPointY;

        private DispatcherTimer timeSliderTimer;

        // game page auxiliary properties
        private Point firstPoint;
        private Point secondPoint;

        private DispatcherTimer showPathTimer;
        private Point foundPoint1;
        private Point foundPoint2;
        private List<Point> foundPointList;

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
            // initialize game remain block amount
            initGameRemainBlockAmount();
            // initialize game remain time
            initGameRemainTime();
            // initialize game tools
            initGameTools();
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

        // initialize game remain block amount
        private void initGameRemainBlockAmount()
        {
            gameRemainAmountArray = new Image[3];
            int y = gamePanelStartPointY - 40;
            int x = gamePanelStartPointX + BLOCK_WIDTH * (COLUM_AMOUNT - 2);
            for (int i = 0; i < 3; i++)
            {
                Image image = new Image();
                image.Name = "number_" + i;
                int tempX = x + i * (19 + 4);
                Thickness imageThickness = new Thickness(tempX, y, 0, 0);
                image.Margin = imageThickness;
                image.Source = Num0.Source;
                gameRemainAmountArray[i] = image;
                gameCanvas.Children.Add(image);
            }
            // update game remain block amount
            updateGameRemainBlockAmount();
        }

        // initialize game remain time
        private void initGameRemainTime()
        {
            // initialize timeSlide
            int x = gamePanelStartPointX + 5;
            int y = gamePanelStartPointY - 75;
            Thickness thickness = new Thickness(x, y, 0, 0);
            timeSlider.Margin = thickness;
            timeSlider.Maximum = game.getRemainTime();
            timeSlider.Value = game.getRemainTime();

            // initialize timeSliderTimer
            timeSliderTimer = new DispatcherTimer();
            timeSliderTimer.Interval = TimeSpan.FromSeconds(1);
            timeSliderTimer.Tick += timeSliderTimer_Tick;
            timeSliderTimer.Start();
        }

        // initialize game tools
        private void initGameTools()
        {
            int interval = 100;
            int startX = gamePanelStartPointX + BLOCK_WIDTH * 1;
            int y = gamePanelStartPointY + BLOCK_HEIGHT * ROW_AMOUNT - 10;
            int x1 = startX;
            Thickness imgRefreshThickness = new Thickness(x1 , y, 0, 0);
            int x2 = startX + interval + 5;
            Thickness imgBombThickness = new Thickness(x2, y, 0, 0);
            int x3 = startX + interval * 2;
            Thickness imgFinderThickness = new Thickness(x3, y, 0, 0);
            imgRefresh.Margin = imgRefreshThickness;
            imgBomb.Margin = imgBombThickness;
            imgFinder.Margin = imgFinderThickness;
        }

        // initialize game page auxiliary properties
        private void initGamePageAuxiliaryProperties()
        {
            // initialize tapped two points
            firstPoint = generateEmptyPoint();
            secondPoint = generateEmptyPoint();

            // initialzie found point and point list
            foundPoint1 = generateEmptyPoint();
            foundPoint2 = generateEmptyPoint();
            foundPointList = new List<Point>();

            showPathTimer = new DispatcherTimer();
            showPathTimer.Interval = TimeSpan.FromSeconds(0.2);
            showPathTimer.Tick += showPathTimer_Tick;

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
                if (isTwoPointEqual(firstPoint, secondPoint))
                {
                    doPathNotFoundUpdate();
                }
                else
                {
                    List<Point> pointList = game.findPathBetweenPoints(firstPoint, secondPoint);
                    if (pointList.Count > 0)
                    {
                        doPathFoundUpdate(firstPoint, secondPoint, pointList);
                    }
                    else
                    {
                        doPathNotFoundUpdate();
                    }
                }
                // check game status
                checkGameStatus();
            }
        }
        // do path found update
        private void doPathFoundUpdate(Point startPoint, Point endPoint, List<Point> pointList)
        {
            // do game finish block pair update
            game.doFinishBlockPairUpdate(startPoint, endPoint);
            
            // clear user tapped two points
            clearTappedTwoPoint();

            // set found point and point list for show path, erase path and found points
            foundPoint1 = startPoint;
            foundPoint2 = endPoint;
            foundPointList = pointList;

            // show path and start showPathTimer
            showConnectPath(pointList);
            showPathTimer.Start();
        }
        private void showPathTimer_Tick(object sender, object e)
        {
            // update game remain block amount
            updateGameRemainBlockAmount();

            // erase connect path
            eraseConnectPath(foundPointList);

            // remove user tapped two points from game panel
            removeGamePanelBlockImage(foundPoint1);
            removeGamePanelBlockImage(foundPoint2);

            // stop showPathTimer
            showPathTimer.Stop();
        }
        // show connect path
        private void showConnectPath(List<Point> pointList)
        {
            for(int i = 0; i < pointList.Count - 1; i++)
            {
                Point point1 = pointList[i];
                Point point2 = pointList[i + 1];
                drawPathBetweenTwoPoint(point1, point2);
            }
        }
        private void drawPathBetweenTwoPoint(Point point1, Point point2)
        {
            int x1 = (int)point1.X;
            int y1 = (int)point1.Y;
            int x2 = (int)point2.X;
            int y2 = (int)point2.Y;
            if (x1 == x2)
            {
                int x = x1;
                int startY;
                int endY;
                if (y1 > y2)
                {
                    startY = y2;
                    endY = y1;
                }
                else
                {
                    startY = y1;
                    endY = y2;
                }
                for (int i = startY; i <= endY; i++)
                {
                    Image image = gamePanelBackgroudMatrix[x, i];
                    gameCanvas.Children.Remove(image);

                    image.Source = point.Source;
                    gameCanvas.Children.Add(image);
                    gamePanelBackgroudMatrix[x, i] = image;
                }
            }
            else
            {
                int y = y1;
                int startX;
                int endX;
                if (x1 > x2)
                {
                    startX = x2;
                    endX = x1;
                }
                else
                {
                    startX = x1;
                    endX = x2;
                }
                for (int i = startX; i <= endX; i++)
                {
                    Image image = gamePanelBackgroudMatrix[i, y];
                    gameCanvas.Children.Remove(image);

                    image.Source = point.Source;
                    gameCanvas.Children.Add(image);
                    gamePanelBackgroudMatrix[i, y] = image;
                }
            }
        }
        // erase connect path
        private void eraseConnectPath(List<Point> pointList)
        {
            for (int i = 0; i < pointList.Count - 1; i++)
            {
                Point point1 = pointList[i];
                Point point2 = pointList[i + 1];
                erasePathBetweenTwoPoint(point1, point2);
            }
        }
        private void erasePathBetweenTwoPoint(Point point1, Point point2)
        {
            int x1 = (int)point1.X;
            int y1 = (int)point1.Y;
            int x2 = (int)point2.X;
            int y2 = (int)point2.Y;
            if (x1 == x2)
            {
                int x = x1;
                int startY;
                int endY;
                if (y1 > y2)
                {
                    startY = y2;
                    endY = y1;
                }
                else
                {
                    startY = y1;
                    endY = y2;
                }
                for (int i = startY; i <= endY; i++)
                {
                    Image image = gamePanelBackgroudMatrix[x, i];
                    gameCanvas.Children.Remove(image);

                    image.Source = bgBlock.Source;
                    gameCanvas.Children.Add(image);
                    gamePanelBackgroudMatrix[x, i] = image;
                }
            }
            else
            {
                int y = y1;
                int startX;
                int endX;
                if (x1 > x2)
                {
                    startX = x2;
                    endX = x1;
                }
                else
                {
                    startX = x1;
                    endX = x2;
                }
                for (int i = startX; i <= endX; i++)
                {
                    Image image = gamePanelBackgroudMatrix[i, y];
                    gameCanvas.Children.Remove(image);

                    image.Source = bgBlock.Source;
                    gameCanvas.Children.Add(image);
                    gamePanelBackgroudMatrix[i, y] = image;
                }
            }
        }
        // do path not found update
        private void doPathNotFoundUpdate()
        {
            updateImageToNormal(firstPoint);
            updateImageToNormal(secondPoint);
            clearTappedTwoPoint();
        }
        // remote game panel block image
        private void removeGamePanelBlockImage(Point point)
        {
            int x = (int)point.X;
            int y = (int)point.Y;
            Image image = gamePanelBlockMatrix[x, y];
            gameCanvas.Children.Remove(image);
            image.Source = null;
            gamePanelBlockMatrix[x, y] = image;
        }
        // update block image to tapped
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
        // update block image to normal
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
        // clear tapped two point
        private void clearTappedTwoPoint()
        {
            firstPoint = generateEmptyPoint();
            secondPoint = generateEmptyPoint();
        }
        // update game remain block amount
        private void updateGameRemainBlockAmount()
        {
            int remainAmount = game.getRemainBlockAmount();
            for (int i = 2; i >= 0; i--)
            {
                int value = remainAmount % 10;
                remainAmount = remainAmount / 10;
                Image image = gameRemainAmountArray[i];
                gameCanvas.Children.Remove(image);
                setGameNumberImageSource(image, value);
                gameRemainAmountArray[i] = image;
                gameCanvas.Children.Add(image);
            }
        }

        // timeSlider timer tick callback function
        private void timeSliderTimer_Tick(object sender, object e)
        {
            // decrease game remain time normally
            game.decreaseRemainTime();
            // update game time
            updateGameTime();
            // check game status
            checkGameStatus();
        }

        // update game time
        private void updateGameTime()
        {
            timeSlider.Value = game.getRemainTime();
        }

        // check game status
        private void checkGameStatus()
        {
            if (game.isGameOver())
            {
                timeSliderTimer.Stop();
                clearGamePanelBlockTappedEvent();
                imgRefresh.Visibility = System.Windows.Visibility.Collapsed;
                imgBomb.Visibility = System.Windows.Visibility.Collapsed;
                imgFinder.Visibility = System.Windows.Visibility.Collapsed;
                btn_restart.Visibility = System.Windows.Visibility.Visible;
                tbRemind.Visibility = System.Windows.Visibility.Visible;
                if (game.isWin())
                {
                    tbRemind.Text = "胜利！";
                }
                if (game.isLost())
                {
                    tbRemind.Text = "失败！";
                }
            }
        }

        // clear game panel block tapped event
        private void clearGamePanelBlockTappedEvent()
        {
            for (int i = 0; i < ROW_AMOUNT; i++)
            {
                for (int j = 0; j < COLUM_AMOUNT; j++)
                {
                    Image image = gamePanelBlockMatrix[i, j];
                    gameCanvas.Children.Remove(image);
                    image.Tap -= blockTapped;
                    
                    gameCanvas.Children.Add(image);
                    gamePanelBlockMatrix[i, j] = image;
                }
            }
        }

        // add game panel block tapped event
        private void addGamePanelBlockTappedEvent()
        {
            for (int i = 0; i < ROW_AMOUNT; i++)
            {
                for (int j = 0; j < COLUM_AMOUNT; j++)
                {
                    Image image = gamePanelBlockMatrix[i, j];
                    gameCanvas.Children.Remove(image);
                    image.Tap += blockTapped;

                    gameCanvas.Children.Add(image);
                    gamePanelBlockMatrix[i, j] = image;
                }
            }
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

        // set game number image scource
        private void setGameNumberImageSource(Image image, int value)
        {
            switch (value)
            {
                case 1:
                    image.Source = Num1.Source;
                    break;
                case 2:
                    image.Source = Num2.Source;
                    break;
                case 3:
                    image.Source = Num3.Source;
                    break;
                case 4:
                    image.Source = Num4.Source;
                    break;
                case 5:
                    image.Source = Num5.Source;
                    break;
                case 6:
                    image.Source = Num6.Source;
                    break;
                case 7:
                    image.Source = Num7.Source;
                    break;
                case 8:
                    image.Source = Num8.Source;
                    break;
                case 9:
                    image.Source = Num9.Source;
                    break;
                case 0:
                    image.Source = Num0.Source;
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

        // generate empty point
        private Point generateEmptyPoint()
        {
            Point point = new Point(-1, -1);
            return point;
        }

        // check whether two points is the same
        private Boolean isTwoPointEqual(Point point1, Point point2)
        {
            if (point1.X == point2.X && point1.Y == point2.Y)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // internal auxiliary functions ending //
        //*******************************************************************//


        //*******************************************************************//
        // normal user controls event response functions beginning //

        // refresh image tap event response function
        private void imgRefresh_Tap(object sender, GestureEventArgs e)
        {
            game.refreshGameZoneMatrix();
            updateGamePanelBlocks();
        }

        // bomb image tap event response function
        private void imgBomb_Tap(object sender, GestureEventArgs e)
        {

        }

        // finder image tap event response function
        private void imgFinder_Tap(object sender, GestureEventArgs e)
        {

        }

        // restart button click resposne function
        private void btnRestart_Click(object sender, RoutedEventArgs e)
        {
            game.doGameInfoReset();
            addGamePanelBlockTappedEvent();
            updateGamePanelBlocks();
            clearTappedTwoPoint();
            updateGameRemainBlockAmount();
            resetTimeSliderTimer();

            imgRefresh.Visibility = System.Windows.Visibility.Visible;
            imgBomb.Visibility = System.Windows.Visibility.Visible;
            imgFinder.Visibility = System.Windows.Visibility.Visible;
            btn_restart.Visibility = System.Windows.Visibility.Collapsed;
            tbRemind.Visibility = System.Windows.Visibility.Collapsed;
        }

        // normal user controls event response functions ending //
        //*******************************************************************//


        // reset timeSliderTimer
        private void resetTimeSliderTimer()
        {
            // start timeSliderTimer
            timeSliderTimer.Start();
            // update game time
            updateGameTime();
        }

        // update game panel blocks
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

        //*******************************************************************//
    }
}