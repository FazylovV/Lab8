using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AccountStatement
{
    class Program
    {
        public static void Main()
        {
            string[] text = File.ReadAllLines("file.txt");
            int startBalance = int.Parse(text[0]);

            string[] arrayWithoutStartBalance = GetArrayWithoutStartBalance(text);
            string[][] propsOfUnsortedArray = GetPropsOfTransactions(arrayWithoutStartBalance);
            long[] datesForCompare = GetDates(propsOfUnsortedArray);
            int[] indexesForSorting = SortDatesAndSavePreviousIndex(datesForCompare);
            string[][] sortedArray = SortArrayUseIndexes(indexesForSorting, propsOfUnsortedArray);

            if (IsCorrect(sortedArray, startBalance)) OpenMenu(sortedArray, startBalance);
            else Console.WriteLine("Файл некорректен!");
        }
        
        public static void OpenMenu(string[][] sortedArray, int startBalance)
        {
            string startTime = sortedArray[0][0];
            string endTime = sortedArray[sortedArray.Length - 1][0];
            long timeLong;
            string? timeStr;

            Console.WriteLine("Дата и Время вводится в формате \"YYYY-MM-DD HH:MM\" (Нажав ENTER без ввода символов, программа выведет итоговый остаток средств)");
            Console.WriteLine($"В данном файле дата и время вводятся от {startTime} до {endTime}");

            while (true)
            {
                timeStr = Console.ReadLine();

                if (timeStr == null)
                {
                    timeLong = long.Parse(GetDate(endTime));
                    break;
                }
                else
                {
                    bool isLong = long.TryParse(GetDate(timeStr), out timeLong);
                    if (isLong && timeLong <= long.Parse(GetDate(endTime)) && timeLong >= long.Parse(GetDate(startTime))) break;
                }

                Console.WriteLine("Попробуйте еще раз");
            }

            int accountBalance = CalculateAccountBalance(sortedArray, timeLong, startBalance);
            Console.WriteLine($"На момент {timeStr} на счете находится: {accountBalance}");
        }
        
        public static string[] GetArrayWithoutStartBalance(string[] fileText)
        {
            string[] arrayOperations = new string[fileText.Length-1];
            var dates = new List<long>(); 

            for (int i = 0; i < fileText.Length-1; i++) 
                arrayOperations[i] = fileText[i+1];

            return arrayOperations;
        }

        public static string[][] GetPropsOfTransactions(string[] operations)
        {

            string[][] arrayTransactions = new string[operations.Length][];
            for (int i = 0; i < operations.Length; i++)
            {
                arrayTransactions[i] = new string[3];
                string[] propsOfTransaction = operations[i].Replace(" | ", "|").Split('|');
                for (int j = 0; j < propsOfTransaction.Length; j++)
                    arrayTransactions[i][j] = propsOfTransaction[j];
            }

            return arrayTransactions;
        }

        public static long[] GetDates(string[][] operations)
        {
            var dates = new List<long>();
            foreach (string[] operation in operations)
            {
                string date = operation[0];
                date = date.Replace("-", "").Replace(" ", "").Replace(":", "");
                dates.Add(long.Parse(date));
            }

            return dates.ToArray();
        }

        public static string GetDate(string date)
        {
            return date.Replace("-", "").Replace(" ", "").Replace(":", "");
        }

        public static int[] SortDatesAndSavePreviousIndex(long[] dates)
        {
            int[] previousIndex = new int[dates.Length];
            int indexOfLastElement = dates.Length - 1;

            for (int i = 0; i < dates.Length; i++)
            {
                int index = Array.IndexOf(dates, dates.Min());
                previousIndex[i] = index;
                dates[index] = long.MaxValue;
            }

            return previousIndex;
        }

        public static string[][] SortArrayUseIndexes(int[] indexes, string[][] unsortedArray)
        {
            var sortedArray = new string[unsortedArray.Length][];
            for(int i = 0; i < indexes.Length; i++)
            {
                int index = indexes[i];
                sortedArray[i] = new string[unsortedArray[index].Length];
                sortedArray[i] = unsortedArray[index];
            }

            return sortedArray;
        }

        public static bool IsCorrect(string[][] sortedArray, int startBalance) // Проверка на правильность данных (Возвращает FALSE если: 2 команды revert подряд, баланс уходит в минус)
        {
            int sumOfMoney = startBalance;

            for (int i = 0; i < sortedArray.Length; i++)
            {
                if (int.TryParse(sortedArray[i][1], out int money))
                {
                    switch (sortedArray[i][2])
                    {
                        case "in":
                            sumOfMoney += money;
                            break;

                        case "out":
                            sumOfMoney -= money;
                            break;
                    }

                    if (sumOfMoney < 0) return false;
                }
                
                else
                {
                    int previousValue = 0;
                    if (i == 0) return false;
                    else if (sortedArray[i - 1][1] == "revert") return false;
                    else previousValue = int.Parse(sortedArray[i-1][1]);

                    switch (sortedArray[i-1][2])
                    {
                        case "in":
                            sumOfMoney -= previousValue;
                            break;

                        case "out":
                            sumOfMoney += previousValue;
                            break;
                    }
                }
            }

            return true;
        }

        public static int CalculateAccountBalance(string[][] sortedArray, long timeForCalculatingBalance, int startBalance)
        {
            int accountBalance = startBalance;

            for (int i = 0; i < sortedArray.Length; i++) 
            {
                long time = long.Parse(GetDate(sortedArray[i][0]));
                if (time <= timeForCalculatingBalance)
                {
                    if (int.TryParse(sortedArray[i][1], out int money))
                    {
                        if(sortedArray[i][2]  == "in") accountBalance += money;
                        else if(sortedArray[i][2]  == "out") accountBalance -= money;
                    }
                    else
                    {
                        int previousValue = int.Parse(sortedArray[i-1][1]);
                        if(sortedArray[i - 1][2] == "in") accountBalance -= previousValue;
                        if(sortedArray[i - 1][2] == "out") accountBalance += previousValue;
                    }
                }
            }

            return accountBalance;
        }
    }
}
