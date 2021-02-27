using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Text;
using System;



class Result
{

    /*
     * Complete the 'balancedSum' function below.
     *
     * The function is expected to return an INTEGER.
     * The function accepts INTEGER_ARRAY arr as parameter.
     */

    public static int balancedSum(List<int> arr)
    {
        decimal target = Math.Floor((decimal)(arr.Sum(i => i) / 2));
        int current = 0;
        int pivot = 0;

        for (pivot = 0; pivot < arr.Count; pivot++)
        {
            current += arr[pivot];

            if (current >= target)
                break;
        }

        return pivot + 1;
    }

}

class Solution
{
    public static void Main(string[] args)
    {
        TextWriter textWriter = new StreamWriter(@System.Environment.GetEnvironmentVariable("OUTPUT_PATH"), true);

        int arrCount = Convert.ToInt32(Console.ReadLine().Trim());

        List<int> arr = new List<int>();

        for (int i = 0; i < arrCount; i++)
        {
            int arrItem = Convert.ToInt32(Console.ReadLine().Trim());
            arr.Add(arrItem);
        }

        int result = Result.balancedSum(arr);

        textWriter.WriteLine(result);

        textWriter.Flush();
        textWriter.Close();
    }
}