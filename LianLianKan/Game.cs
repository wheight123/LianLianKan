using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace LianLianKan
{
    public class Game
    {
        // game general information properties
        private int rowAmount;
        private int columAmount;
        private int blockType;

        // game zone matrix
        private int[,] gameZoneMatrix;

        // game basic information properties
        private int gameMode;
        private int gameLevel;
        private int blockAmount;

        // game user information properties
        private int finishBlockAmount;
        private double remainTime;
        private int score;

        // game constants properites
        private const int GAME_MODE_LEVEL = 1;
        private const int GAME_MODE_CHALLENGE = 2;
        private const int GAME_LEVEL_DEFAULT = 5;
        private const int GAME_LEVE_MIN = 1;
        private const int GAME_LEVEL_MAX = 12;

        private const int RANDOM_EXCHANGE_TIME = 200;

        public Game(int rowAmount, int columAmount, int blockType)
        {
            // set game general information
            this.rowAmount = rowAmount;
            this.columAmount = columAmount;
            this.blockType = blockType;
            // initialize game basic information
            initGameBasicInfo(GAME_MODE_LEVEL, GAME_LEVEL_DEFAULT);
            // initialize game zone matrix
            initGameZoneMatrix();
            // initialize user game information
            initUserGameInfo();
        }

        //*******************************************************************//
        // initialize game information functions beginning //

        // initialize game basic information
        private void initGameBasicInfo(int gameMode, int gameLevel)
        {
            this.gameMode = gameMode;
            this.gameLevel = gameLevel;
            blockAmount = 30 + 4 * gameLevel;
        }

        // initialize game zone matrix
        private void initGameZoneMatrix()
        {
            gameZoneMatrix = new int[rowAmount, columAmount];
            clearGameZoneMatrix();
            randomArrangeValuePairInGameZoneMatrix();
            randomExchangeValuePointInGameZoneMatrix();
        }
        // clear game zone matrix
        private void clearGameZoneMatrix()
        {
            for (int i = 0; i < rowAmount; i++)
            {
                for (int j = 0; j < columAmount; j++)
                {
                    gameZoneMatrix[i, j] = 0;
                }
            }
        }
        // random arrange value pair in game zone matrix
        private void randomArrangeValuePairInGameZoneMatrix()
        {
            Random random = new Random();
            int blockPairAmount = blockAmount / 2;
            int xIndex1 = 1;
            int yIndex = 1;
            int xIndex2 = rowAmount / 2;
            for (int i = 1; i <= blockPairAmount; i++)
            {
                int type = random.Next() % blockType + 1;
                gameZoneMatrix[xIndex1, yIndex] = type;
                gameZoneMatrix[xIndex2, yIndex] = type;
                yIndex++;
                if (yIndex >= columAmount - 1)
                {
                    yIndex = 1;
                    xIndex1++;
                    xIndex2++;
                }
            }
        }
        // random exchange value position in game zone matrix
        private void randomExchangeValuePointInGameZoneMatrix()
        {
            Random random = new Random();
            int xSeed = rowAmount - 2;
            int ySeed = columAmount - 2;
            for (int i = 1; i <= RANDOM_EXCHANGE_TIME; i++)
            {
                int x1 = random.Next() % xSeed + 1;
                int y1 = random.Next() % ySeed + 1;
                int x2 = random.Next() % xSeed + 1;
                int y2 = random.Next() % ySeed + 1;
                int value = gameZoneMatrix[x1, y1];
                gameZoneMatrix[x1, y1] = gameZoneMatrix[x2, y2];
                gameZoneMatrix[x2, y2] = value;
            }
        }

        // initialize user game infomation
        private void initUserGameInfo()
        {
            finishBlockAmount = 0;
            remainTime = 100;
            score = 0;
        }

        // initialize game information functions ending //
        //*******************************************************************//


        //*******************************************************************//
        // game properties getter and setter functions begining //

        // get game zone matrix
        public int[,] getGameZoneMatrix()
        {
            return gameZoneMatrix;
        }

        // get finish block amount
        public int getFinishBlockAmount()
        {
            return finishBlockAmount;
        }

        // get remain time
        public double getRemainTime()
        {
            return remainTime;
        }

        // get score
        public int getScore()
        {
            return score;
        }

        // game properties getter and setter functions ending //
        //*******************************************************************//

        //*******************************************************************//
        // game business service functions beginning //

        // find path between two points
        public List<Point> findPathBetweenPoints(Point startPoint, Point endPoint)
        {
            List<Point> pointPathList = new List<Point>();
            // check whether the two points own the same value
            if (isTwoPointsOwnTheSameValue(startPoint, endPoint))
            {
                // find point path in line
                pointPathList = findPathInLine(startPoint, endPoint);
                if (pointPathList.Count == 0)
                {
                    // find point path in rectangle
                    pointPathList = findPathInRectangle(startPoint, endPoint);
                    if (pointPathList.Count == 0)
                    {
                        // find point path
                        pointPathList = findPath(startPoint, endPoint);
                    }
                }
            }
            return pointPathList;
        }
        // check whether the two points own the same value
        private Boolean isTwoPointsOwnTheSameValue(Point point1, Point point2)
        {
            int x1 = (int)point1.X;
            int y1 = (int)point1.Y;
            int x2 = (int)point2.X;
            int y2 = (int)point2.Y;
            Boolean flag = false;
            if (gameZoneMatrix[x1, y1] == gameZoneMatrix[x2, y2])
            {
                flag = true;
            }
            return flag;
        }
        // find path in line
        private List<Point> findPathInLine(Point startPoint, Point endPoint)
        {
            List<Point> pointPathList = new List<Point>();
            if (isTwoPointsInLine(startPoint, endPoint))
            {
                if (canTwoPointsBeConnected(startPoint, endPoint))
                {
                    pointPathList.Add(startPoint);
                    pointPathList.Add(endPoint);
                }
            }
            return pointPathList;
        }
        // find path in rectangle
        private List<Point> findPathInRectangle(Point startPoint, Point endPoint)
        {
            List<Point> pointPathList = new List<Point>();
            int x1 = (int)startPoint.X;
            int y1 = (int)startPoint.Y;
            int x2 = (int)endPoint.X;
            int y2 = (int)endPoint.Y;
            Point tempPoint1 = new Point(x1, y2);
            Point tempPoint2 = new Point(x2, y1);
            if(canThreePointsBeConnected(startPoint, endPoint, tempPoint1))
            {
                pointPathList.Add(startPoint);
                pointPathList.Add(tempPoint1);
                pointPathList.Add(endPoint);
            }
            else if(canThreePointsBeConnected(startPoint, endPoint, tempPoint2))
            {
                pointPathList.Add(startPoint);
                pointPathList.Add(tempPoint2);
                pointPathList.Add(endPoint);
            }
            return pointPathList;
        }
        // find path by the third method
        private List<Point> findPath(Point startPoint, Point endPoint)
        {
            List<Point> pointPathList1 = new List<Point>();
            List<Point> pointPathList2 = new List<Point>();
            Point[] startXPointArray = getXPointArrayOfPoint(startPoint);
            Point[] startYPointArray = getYPointArrayOfPoint(startPoint);
            Point[] endXPointArray = getXPointArrayOfPoint(endPoint);
            Point[] endYPointArray = getYPointArrayOfPoint(endPoint);
            int distance1 = getPointPairDistanceInXAxis(
                startXPointArray, endXPointArray, startPoint, endPoint, pointPathList1);
            int distance2 = getPointPairDistanceInYAxis(
                startYPointArray, endYPointArray, startPoint, endPoint, pointPathList2);
            if (pointPathList1.Count > 0 && pointPathList2.Count > 0)
            {
                if (distance1 <= distance2)
                {
                    return pointPathList1;
                }
                else
                {
                    return pointPathList2;
                }
            }
            else if (pointPathList1.Count > 0 && pointPathList2.Count == 0)
            {
                return pointPathList1;
            }
            else if (pointPathList1.Count == 0 && pointPathList2.Count > 0)
            {
                return pointPathList2;
            }
            else
            {
                return new List<Point>();
            }
        }
        // get x axis point array of point
        private Point[] getXPointArrayOfPoint(Point point)
        {
            int x = (int)point.X;
            int y = (int)point.Y;
            Point[] xPointArray = new Point[rowAmount];
            xPointArray[x] = generateEmptyPoint();
            int i;
            Boolean flag = false;
            for (i = x -1; i >= 0; i--)
            {
                if (flag)
                {
                    xPointArray[i] = generateEmptyPoint();
                }
                else
                {
                    if (gameZoneMatrix[i, y] != 0)
                    {
                        flag = true;
                        xPointArray[i] = generateEmptyPoint();
                    }
                    else
                    {
                        xPointArray[i] = new Point(i, y);
                    }
                }
                
            }
            flag = false;
            for (i = x + 1; i < rowAmount; i++)
            {
                if (flag)
                {
                    xPointArray[i] = generateEmptyPoint();
                }
                else
                {
                    if (gameZoneMatrix[i, y] != 0)
                    {
                        flag = true;
                        xPointArray[i] = generateEmptyPoint();
                    }
                    else
                    {
                        xPointArray[i] = new Point(i, y);
                    }
                }
            }
            return xPointArray;
        }
        // get y axis piont array of point
        private Point[] getYPointArrayOfPoint(Point point)
        {
            int x = (int)point.X;
            int y = (int)point.Y;
            Point[] yPointArray = new Point[columAmount];
            yPointArray[y] = generateEmptyPoint();
            int i;
            Boolean flag = false;
            for (i = y - 1; i >= 0; i--)
            {
                if (flag)
                {
                    yPointArray[i] = generateEmptyPoint();
                }
                else
                {
                    if (gameZoneMatrix[x, i] != 0)
                    {
                        flag = true;
                        yPointArray[i] = generateEmptyPoint();
                    }
                    else
                    {
                        yPointArray[i] = new Point(x, i);
                    }
                }

            }
            flag = false;
            for (i = y + 1; i < columAmount; i++)
            {
                if (flag)
                {
                    yPointArray[i] = generateEmptyPoint();
                }
                else
                {
                    if (gameZoneMatrix[x, i] != 0)
                    {
                        flag = true;
                        yPointArray[i] = generateEmptyPoint();
                    }
                    else
                    {
                        yPointArray[i] = new Point(x, i);
                    }
                }
            }
            return yPointArray;
        }
        private int getPointPairDistanceInXAxis(Point[] startPointArray, Point[] endPointArray,
            Point startPoint, Point endPoint, List<Point> pointPathList)
        {
            int x1 = (int)startPoint.X;
            int x2 = (int)endPoint.X;
            int minDistance = 10000;
            Point minPoint1 = new Point();
            Point minPoint2 = new Point();
            Boolean flag = false;
            for (int i = 0; i < rowAmount; i++)
            {
                Point point1 = startPointArray[i];
                Point point2 = endPointArray[i];
                if ((!isEmptyPoint(point1)) && (!isEmptyPoint(point2)))
                {
                    if (canTwoPointsBeConnected(point1, point2))
                    {
                        flag = true;
                        int tempDistance = Math.Abs(i - x1) + Math.Abs(i - x2);
                        if (minDistance > tempDistance)
                        {
                            minDistance = tempDistance;
                            minPoint1 = point1;
                            minPoint2 = point2;
                        }
                    }
                }
            }
            if (flag)
            {
                pointPathList.Add(startPoint);
                pointPathList.Add(minPoint1);
                pointPathList.Add(minPoint2);
                pointPathList.Add(endPoint);
                return minDistance;
            }
            else
            {
                return -1;
            }
        }
        private int getPointPairDistanceInYAxis(Point[] startPointArray, Point[] endPointArray,
            Point startPoint, Point endPoint, List<Point> pointPathList)
        {
            int y1 = (int)startPoint.Y;
            int y2 = (int)endPoint.Y;
            int minDistance = 10000;
            Point minPoint1 = new Point();
            Point minPoint2 = new Point();
            Boolean flag = false;
            for (int i = 0; i < columAmount; i++)
            {
                Point point1 = startPointArray[i];
                Point point2 = endPointArray[i];
                if ((!isEmptyPoint(point1)) && (!isEmptyPoint(point2)))
                {
                    if (canTwoPointsBeConnected(point1, point2))
                    {
                        flag = true;
                        int tempDistance = Math.Abs(i - y1) + Math.Abs(i - y2);
                        if (minDistance > tempDistance)
                        {
                            minDistance = tempDistance;
                            minPoint1 = point1;
                            minPoint2 = point2;
                        }
                    }
                }
            }
            if (flag)
            {
                pointPathList.Add(startPoint);
                pointPathList.Add(minPoint1);
                pointPathList.Add(minPoint2);
                pointPathList.Add(endPoint);
                return minDistance;
            }
            else
            {
                return -1;
            }
        }

        // check whether three point can be connected
        private Boolean canThreePointsBeConnected(Point startPoint, Point endPoint, Point tempPoint)
        {
            int x = (int)tempPoint.X;
            int y = (int)tempPoint.Y;
            Boolean flag = false;
            if (gameZoneMatrix[x, y] == 0)
            {
                if (canTwoPointsBeConnected(startPoint, tempPoint) &&
                    canTwoPointsBeConnected(tempPoint, endPoint))
                {
                    flag = true;
                }
            }
            return flag;
        }
        // check whether two point can be connected
        private Boolean canTwoPointsBeConnected(Point point1, Point point2)
        {
            int x1 = (int)point1.X;
            int y1 = (int)point1.Y;
            int x2 = (int)point2.X;
            int y2 = (int)point2.Y;
            Boolean flag = true;
            if (x1 == x2)
            {
                int x = x1;
                int startY;
                int endY;
                if (y1 > y2)
                {
                    startY = y2 + 1;
                    endY = y1;
                }
                else
                {
                    startY = y1 + 1;
                    endY = y2;
                }
                for (int i = startY; i < endY; i++)
                {
                    if (gameZoneMatrix[x, i] != 0)
                    {
                        flag = false;
                        break;
                    }
                }
            }
            else
            {
                int y = y1;
                int startX;
                int endX;
                if (x1 > x2)
                {
                    startX = x2 + 1;
                    endX = x1;
                }
                else
                {
                    startX = x1 + 1;
                    endX = x2;
                }
                for (int i = startX; i < endX; i++)
                {
                    if (gameZoneMatrix[i, y] != 0)
                    {
                        flag = false;
                        break;
                    }
                }
            }
            return flag;
        }

        // refresh game zone matrix
        public void refreshGameZoneMatrix()
        {
            randomExchangeValuePointInGameZoneMatrix();
        }

        // decrease remain time
        public void decreaseReaminTime()
        {
            remainTime -= 1;
        }

        // do finish block pair update
        public void doFinishBlockPairUpdate(Point point1, Point point2)
        {
            finishBlockAmount += 2;
            int x1 = (int)point1.X;
            int y1 = (int)point1.Y;
            int x2 = (int)point2.X;
            int y2 = (int)point2.Y;
            gameZoneMatrix[x1, y1] = 0;
            gameZoneMatrix[x2, y2] = 0;
        }

        // restart game, do some reset work
        public void doGameInfoReset()
        {
            resetGameZoneMatrix();
        }

        private void resetGameZoneMatrix()
        {
            clearGameZoneMatrix();
            randomArrangeValuePairInGameZoneMatrix();
            randomExchangeValuePointInGameZoneMatrix();
        }
        // game business service functions ending //
        //*******************************************************************//


        //*******************************************************************//
        // internal auxiliary functions beginning //
        private Boolean isTwoPointsInLine(Point point1, Point point2)
        {
            double x1 = point1.X;
            double y1 = point1.Y;
            double x2 = point2.X;
            double y2 = point2.Y;
            if (x1 == x2 || y1 == y2)
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
        // check whether point is empty point
        private Boolean isEmptyPoint(Point point)
        {
            if (point.X == -1 && point.Y == -1)
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

    }
}
