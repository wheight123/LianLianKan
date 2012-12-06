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
        // random arrange value pari in game zone matrix
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

    }
}
