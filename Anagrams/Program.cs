using System;
using System.Collections.Generic;
using System.Linq;

namespace Anagrams
{
    internal class Program
    {
        static void Main()
        {
            var array = new string[] { "code", "doce", "ecod", "framer", "frame" };
            var list = new List<string>(array);

            foreach (var item in GetArrayWithoutAnagrams(list))
                Console.WriteLine(item);
        }

        public static bool CheckTwoWordsOnAnagrams(string firstWord, string secondWord)
        {
            List<char> firstChars = firstWord.ToList();
            List<char> secondChars = secondWord.ToList();
            int cntOfSymbols = firstChars.Count;
            int cntOfRemoves = 0;

            if (firstChars.Count != secondChars.Count) return false;

            for (int i = 0; i < cntOfSymbols; i++)
            {
                char symbolToFind = firstChars[i];
                if (secondChars.Contains(symbolToFind))
                {
                    secondChars.Remove(symbolToFind);
                    cntOfRemoves++;
                }
            }

            if (cntOfSymbols == cntOfRemoves) return true;
            else return false;
        }

        public static string[] GetArrayWithoutAnagrams(List<string> listOfWords)
        {
            for (int i = 0; i < listOfWords.Count; i++)
            {
                for (int j = i + 1; j < listOfWords.Count; j++)
                {
                    if (CheckTwoWordsOnAnagrams(listOfWords[i], listOfWords[j]))
                    {
                        listOfWords.RemoveAt(j);
                        j--;
                    }
                }
            }
            return listOfWords.ToArray();
        }
    }
}