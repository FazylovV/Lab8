using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Subtitles
{
    class Program
    {
        public enum Props
        {
            Time,
            Position,
            Color,
            Text,
        }

        public enum Time
        {
            Start,
            Stop,
        }

        public static void Main()
        {
            string[] lines = File.ReadAllLines("file.txt");
            string timeOfLastCommand = GetProps(lines[lines.Length - 1])[(int)Props.Time];
            int timeProgram = GetStartAndStopTime(timeOfLastCommand)[(int)Time.Stop];
            var listOfEndCommand = GetDictOfStopTimeAndPosition(lines);
            int cnt = 0;

            WriteFrame();
            for (int i = 0; i <= timeProgram; i++)
            {
                for (int j = 0 + cnt; j < lines.Length; j++ )
                {
                    string[] props = GetProps(lines[j]);
                    int[] timeOfCommand = GetStartAndStopTime(props[(int)Props.Time]);
                    if (timeOfCommand[(int)Time.Start] == i)
                    {
                        WriteText(props[(int)Props.Position], props[(int)Props.Color], props[(int)Props.Text]);
                        cnt++;
                    }
                }

                if (listOfEndCommand.ContainsKey(i)) ClearText(listOfEndCommand[i]);

                Console.SetCursorPosition(Console.WindowWidth/2, Console.WindowHeight/2);
                Console.Write(i);
                WaitSecond();
            }

            Console.SetCursorPosition(0, Console.WindowHeight);
            Console.Write("");
        }

        public static void WriteFrame()
        {
            Console.WriteLine("----------------------------------------------------------------------------------------------------------------------");
            for (int i = 0; i < 28; i++)
            {
                Console.WriteLine("|                                                                                                                    |");
            }
            Console.WriteLine("----------------------------------------------------------------------------------------------------------------------");
        }

        public static void WriteText(string position, string color, string text)
        {
            int width = Console.WindowWidth;
            int height = Console.WindowHeight;

            int centerOnHorizontal = width / 2 - text.Length / 2;
            int centerOnVertical = height / 2;

            if (color == "Red") Console.ForegroundColor = ConsoleColor.Red;
            else if (color == "Green") Console.ForegroundColor = ConsoleColor.Green;
            else if (color == "Blue") Console.ForegroundColor = ConsoleColor.Blue;
            else Console.ResetColor();

            switch (position)
            {
                case "Top":
                    Console.SetCursorPosition(centerOnHorizontal, 1);
                    Console.Write(text);
                    break;

                case "Bottom":
                    Console.SetCursorPosition(centerOnHorizontal, height - 2);
                    Console.Write(text);
                    break;

                case "Left":
                    Console.SetCursorPosition(1, centerOnVertical);
                    Console.Write(text);
                    break;

                case "Right":
                    Console.SetCursorPosition(width - position.Length - 1, centerOnVertical);
                    Console.Write(text);
                    break;
                default:
                    Console.SetCursorPosition(centerOnHorizontal, height - 2);
                    Console.Write(text);
                    break;
            }
        }

        public static void ClearText(List<string> positions)
        {
            foreach (string position in positions)
            {
                switch (position)
                {
                    case "Top":
                        Console.SetCursorPosition(2, 1);
                        Console.Write("                                                                                                                   ");
                        break;

                    case "Bottom":
                        Console.SetCursorPosition(1, Console.WindowHeight - 2);
                        Console.Write("                                                                                                                    ");
                        break;

                    case "Left":
                        Console.SetCursorPosition(1, Console.WindowHeight / 2);
                        Console.Write("                                                         ");
                        break;

                    case "Right":
                        Console.SetCursorPosition(Console.WindowWidth / 2, Console.WindowHeight / 2);
                        Console.Write("                                                         ");
                        break;

                    default:
                        Console.SetCursorPosition(1, Console.WindowHeight - 2);
                        Console.Write("                                                                                                                    ");
                        break;
                }
            }
        }

        public static string[] GetProps (string line)
        {
            string[] props = new string[4];
            line = line.Replace(" - ", "-");
            props[(int)Props.Time] = line.Substring(0, 11);
            
            if (line.Contains('['))
            {
                props[(int)Props.Position] = line.Substring(line.IndexOf('[') + 1, line.IndexOf(',') - line.IndexOf('[') - 1);
                props[(int)Props.Color] = line.Substring(line.IndexOf(',') + 2, line.IndexOf(']')-line.IndexOf(',') - 2);
                props[(int)Props.Text] = line.Substring(line.IndexOf(']') + 2);
            }

            else
            {
                props[(int)Props.Position] = "not exist";
                props[(int)Props.Color] = "not exist";
                props[(int)Props.Text] = line.Substring(line.IndexOf(' ') + 1);
            }

            return props;
        }

        public static void WaitSecond()
        {
            Thread.Sleep(1000);
        }

        public static int[] GetStartAndStopTime(string time)
        {
            string[] startAndStopTimeStr = time.Split('-');
            string[] startTime = startAndStopTimeStr[0].Split(':');
            string[] stopTime = startAndStopTimeStr[1].Split(':');
            var startAndStopTime = new int[2];

            startAndStopTime[(int)Time.Start] = int.Parse(startTime[0]) * 60 + int.Parse(startTime[1]);
            startAndStopTime[(int)Time.Stop] = int.Parse(stopTime[0]) * 60 + int.Parse(stopTime[1]);

            return startAndStopTime;
        }

        public static Dictionary<int, List<string>> GetDictOfStopTimeAndPosition(string[] lines)
        {
            var listOfEndCommand = new Dictionary<int, List<string>>();
            foreach (string line in lines)
            {
                string[] props = GetProps(line);
                int[] timeOfCommand = GetStartAndStopTime(props[0]);
                if (!listOfEndCommand.ContainsKey(timeOfCommand[1])) listOfEndCommand[timeOfCommand[1]] = new List<string>();
                listOfEndCommand[timeOfCommand[1]].Add(props[1]);
            }

            return listOfEndCommand;
        }
    }
}
