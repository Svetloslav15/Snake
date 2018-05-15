using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Snake
{
    public struct Position
    {
        public int Row;
        public int Col;
        public Position(int row, int col)
        {
            this.Row = row;
            this.Col = col;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            byte left = 1;
            byte right = 0;
            byte down = 2;
            byte top = 3;
            int points = 0;
            int totalTime = Environment.TickCount;
            int speed = 100;

            Position[] directions = new Position[]
            {
                new Position(0, 1), // right
                new Position(0, -1), // left
                new Position(1, 0), // down
                new Position(-1, 0) // top
            };
            int direction = 0;
            Console.BufferHeight = Console.WindowHeight;
            int startTime = Environment.TickCount;

            Queue<Position> snakeElements = new Queue<Position>();
            List<Position> obstacles = new List<Position>();
            Position food = CreateApple(snakeElements, obstacles);

            for (int number = 1; number <= 20; number++)
            {
                Position obstacle = CreateObstacle(snakeElements, obstacles, food);
                WriteSymbol(obstacle, 'X', "obstacle");
            }

            foreach (var obstacle in obstacles)
            {
                WriteSymbol(obstacle, 'X', "obstacle");
            }

            for (int i = 0; i <= 5; i++)
            {
                snakeElements.Enqueue(new Position(0, i));
            }

            foreach (Position position in snakeElements)
            {
                WriteSymbol(position, '*', "tail");
            }
            WriteSymbol(food, '@', "food");

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo userInput = Console.ReadKey();
                    if (userInput.Key == ConsoleKey.LeftArrow)
                    {
                        direction = left;
                    }
                    if (userInput.Key == ConsoleKey.RightArrow)
                    {
                        direction = right;
                    }
                    if (userInput.Key == ConsoleKey.DownArrow)
                    {
                        direction = down;
                    }
                    if (userInput.Key == ConsoleKey.UpArrow)
                    {
                        direction = top;
                    }
                }

                Position snakeHead = snakeElements.Last();
                Position nextDirection = directions[direction];
                Position snakeNewHead = new Position(snakeHead.Row + nextDirection.Row, snakeHead.Col + nextDirection.Col);

                if (snakeNewHead.Row < 0) snakeNewHead.Row = Console.WindowHeight - 1;
                if (snakeNewHead.Col < 0) snakeNewHead.Col = Console.WindowWidth - 1;
                if (snakeNewHead.Col >= Console.WindowWidth) snakeNewHead.Col = 0;
                if (snakeNewHead.Row >= Console.WindowHeight) snakeNewHead.Row = 0;

                // check if you lose the game
                if (snakeElements.Contains(snakeNewHead) ||
                    (obstacles.Contains(snakeNewHead)))
                {
                    int finalTime = Environment.TickCount;
                    int minutes = (int)(finalTime - (double)totalTime) / 1000 / 60;
                    int sec = (int)(finalTime - (double)totalTime) / 1000 % 60;
                    int milSec = (finalTime - totalTime) % 60;
                    Console.SetCursorPosition(0, 0);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Game over!");
                    Console.WriteLine($"Your points: {Math.Max(0, points)}");
                    Console.WriteLine($"You time: {minutes:d2}:{sec:d2}:{milSec:d2}m");

                    return;
                }

                // add head
                snakeElements.Enqueue(snakeNewHead);
                WriteSymbol(snakeHead, '*', "tail");

                // draw head
                Console.SetCursorPosition(snakeNewHead.Col, snakeNewHead.Row);
                Console.ForegroundColor = ConsoleColor.Blue;
                if (direction == right)
                {
                    WriteSymbol(snakeNewHead, '>', "head");
                }
                if (direction == left)
                {
                    WriteSymbol(snakeNewHead, '<', "head");
                }
                if (direction == down)
                {
                    WriteSymbol(snakeNewHead, 'v', "head");
                }
                if (direction == top)
                {
                    WriteSymbol(snakeNewHead, '^', "head");
                }

                // check if food is eaten
                if (snakeNewHead.Col == food.Col && snakeNewHead.Row == food.Row)
                {
                    food = CreateApple(snakeElements, obstacles);
                    points += 10;
                    speed -= 3;
                    Position obstacle = CreateObstacle(snakeElements, obstacles, food);
                    WriteSymbol(obstacle, 'X', "obstacle");
                }
                else
                {
                    Position last = snakeElements.Dequeue();
                    Console.SetCursorPosition(last.Col, last.Row);
                    Console.Write(' ');
                }

                int currentTime = Environment.TickCount;
                if (Math.Abs(currentTime - startTime) >= 15000)
                {
                    Console.SetCursorPosition(food.Col, food.Row);
                    Console.Write(' ');
                    food = CreateApple(snakeElements, obstacles);
                    Position obstacle = CreateObstacle(snakeElements, obstacles, food);
                    WriteSymbol(obstacle, 'X', "obstacle");
                    startTime = Environment.TickCount;
                    speed -= 5;
                    points--;
                }
                WriteSymbol(food, '@', "food");
                speed -= (int)(0.01 * snakeElements.Count);
                Thread.Sleep(speed);
            }
        }
        static Position CreateApple(Queue<Position> snakeElements, List<Position> obstacles)
        {
            Random random = new Random();
            Position food = new Position();
            do
            {
                food = new Position(random.Next(0, Console.WindowHeight), random.Next(0, Console.WindowWidth));
            }
            while (snakeElements.Contains(food) || obstacles.Contains(food));
            return food;
        }

        static Position CreateObstacle(Queue<Position> snakeElements, List<Position> obstacles, Position food)
        {
            Position obstacle = new Position();
            do
            {
                Random random = new Random();
                obstacle = new Position(random.Next(0, Console.WindowHeight), random.Next(0, Console.WindowWidth));
            }
            while (snakeElements.Contains(obstacle) || obstacles.Contains(obstacle) || (food.Row == obstacle.Row && food.Col == obstacle.Col));
            obstacles.Add(obstacle);
            return obstacle;
        }

        static void WriteSymbol(Position position, char symbol, string type)
        {
            Console.SetCursorPosition(position.Col, position.Row);
            switch (type)
            {
                case "food": Console.ForegroundColor = ConsoleColor.Cyan; break;
                case "head": Console.ForegroundColor = ConsoleColor.DarkGray; break;
                case "tail": Console.ForegroundColor = ConsoleColor.Gray; break;
                case "obstacle": Console.ForegroundColor = ConsoleColor.Yellow; break;
            }
            Console.Write(symbol);
        }
    }
}