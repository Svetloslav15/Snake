using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Snake
{
    struct Position
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
            int time = 80;

            Position[] directions = new Position[]
            {
                new Position(0, 1), // right
                new Position(0, -1), // left
                new Position(1, 0), // down
                new Position(-1, 0) // top
            };
            int direction = 0;
            Console.BufferHeight = Console.WindowHeight;

            List<Position> obstacles = new List<Position>()
            {
                new Position(23, 56),
                new Position(12, 10),
                new Position(23, 45),
                new Position(10, 46),
                new Position(24, 19),
                new Position(12, 12),
                new Position(11, 78),
                new Position(5, 62),
                new Position(23, 100),
                new Position(8, 89),
                new Position(6, 43),
            };

            foreach (var obstacle in obstacles)
            {
                Console.SetCursorPosition(obstacle.Col, obstacle.Row);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write('X');
            }

            Queue<Position> snakeElements = new Queue<Position>();
            Position food = CreateApple(snakeElements);

            for (int i = 0; i <= 5; i++)
            {
                snakeElements.Enqueue(new Position(0, i));
            }

            foreach (Position position in snakeElements)
            {
                Console.SetCursorPosition(position.Col, position.Row);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write('*');
            }
            Console.SetCursorPosition(food.Col, food.Row);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write('@');

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

                if (snakeNewHead.Row < 0 ||
                    snakeNewHead.Col < 0 ||
                    snakeNewHead.Col >= Console.WindowWidth ||
                    snakeNewHead.Row >= Console.WindowHeight ||
                    snakeElements.Contains(snakeNewHead) ||
                    (obstacles.Contains(snakeNewHead)))
                {
                    Console.SetCursorPosition(0, 0);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Game over!");
                    Console.WriteLine($"Your points: {snakeElements.Count}");
                    return;
                }
                
                snakeElements.Enqueue(snakeNewHead);
                Console.SetCursorPosition(snakeHead.Col, snakeHead.Row);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write('*');

                Console.SetCursorPosition(snakeNewHead.Col, snakeNewHead.Row);
                Console.ForegroundColor = ConsoleColor.Blue;
                if (direction == right) Console.Write('>');
                if (direction == left) Console.Write('<');
                if (direction == down) Console.Write('v');
                if (direction == top) Console.Write('^');


                if (snakeNewHead.Col == food.Col && snakeNewHead.Row == food.Row)
                {
                    food = CreateApple(snakeElements);
                    points += 10;
                    time -= 5;
                    Position obstacle = new Position();
                    do
                    {
                        Random random = new Random();
                        obstacle = new Position(random.Next(0, Console.WindowHeight), random.Next(0, Console.WindowWidth));

                    } while (snakeElements.Contains(obstacle) || obstacles.Contains(obstacle));
                    obstacles.Add(obstacle);
                    Console.SetCursorPosition(obstacle.Col, obstacle.Row);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write('X');
                }
                else
                {
                    Position last = snakeElements.Dequeue();
                    Console.SetCursorPosition(last.Col, last.Row);
                    Console.Write(' ');
                }

                time -= (int)0.01 * snakeElements.Count;
                Console.SetCursorPosition(food.Col, food.Row);
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write('@');
                Thread.Sleep(time);

            }
        }
        static Position CreateApple(Queue<Position> snakeElements)
        {
            Random random = new Random();
            Position food = new Position();
            do
            {
               food = new Position(random.Next(0, Console.WindowHeight), random.Next(0, Console.WindowWidth));
            }
            while (snakeElements.Contains(food));
            return food;
        }

        
    }
}

