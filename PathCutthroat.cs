using System;
using System.Collections.Generic;

//Nadav Kohen
//This program solves Cutthroat played on a path by calculating the Grundy values of each path length using dynamic programming
public class PathCutthroat
{
    //grunds[i] stores the grundy number of a path of length i
    static int[] grunds;
    //pgames stores all second-player-wins games
    static IList<int> pgames = new List<int>();

    //prints the array grunds
    public static void PrintGrunds()
    {
        for (int i = 0; i < grunds.Length; i++)
            Console.Out.WriteLine(i + ": " + grunds[i]);
    }

    //prints the array pgames
    public static void PrintPGames()
    {
        foreach (Object obj in pgames)
            Console.Out.WriteLine((int)obj);
    }

    public static void Main()
    {
        Console.Out.Write("How many vertices are there? ");
        int size = Convert.ToInt32(Console.ReadLine());

        CalcGrunds(size);
        PrintGrunds();

        CalcPGames();
        PrintPGames();
        Console.ReadLine();
    }

    //calculates the grundy numbers of paths and stores them in grunds
    public static int[] CalcGrunds(int size)
    {
        grunds = new int[size + 1];
        grunds[0] = 0;
        grunds[1] = 0;
        grunds[2] = 1;

        for (int i = 3; i <= size; i++)
            SetGrund(i);

        //this method returns grunds for use by other programs using PathCutthroat, this return is not used inside this file
        return grunds;
    }

    //finds all grundy values of 0 which correspond to second-player-wins games and their indices (path lengths) are stored in pgames
    public static void CalcPGames()
    {
        for (int i = 0; i < grunds.Length; i++)
        {
            if (grunds[i] == 0)
                pgames.Add(i);
        }
    }

    //calculates grunds[n] using dynamic programming and the natural reduction of Cutthroat on a path to Nim
    public static void SetGrund(int n)
    {
        bool[] moves = new bool[n];
        for (int i = 0; i < n / 2 + 1; i++)
            moves[ModAdd(grunds[i], grunds[n - i - 1])] = true;
        grunds[n] = FindMex(moves);
    }

    //Calculates the first missing whole number from x where x[i] true means that i is in the set x represents
    public static int FindMex(bool[] x)
    {
        int mex = 0;
        bool flag = true;
        for (int i = 0; flag && i < x.Length; i++)
        {
            if (x[i])
                mex++;
            else
                flag = false;
        }
        return mex;
    }

    //adds n1 and n2 in binary without carry-over
    public static int ModAdd(int n1, int n2)
    {
        if (n1 == n2)
            return 0;
        if (n1 == 0 || n2 == 0)
            return n1 + n2;

        bool[] n1_2 = ToBinArray(n1);
        bool[] n2_2 = ToBinArray(n2);
        bool[] sum_2 = ModAdd(n1_2, n2_2);

        return FromBinArray(sum_2);
    }

    //adds n1 and n2 in binary without carry-over, taking inputs and giving outputs in binary
    public static bool[] ModAdd(bool[] n1, bool[] n2)
    {
        bool[] sum = new bool[Math.Max(n1.Length, n2.Length)];
        for (int i = 0; i < Math.Min(n1.Length, n2.Length); i++)
            sum[i] = n1[i] ^ n2[i];
        for (int i = Math.Min(n1.Length, n2.Length); i < Math.Max(n1.Length, n2.Length); i++)
            sum[i] = (n1.Length > n2.Length) ? n1[i] : n2[i];

        return sum;
    }

    //converts x into an array of bools corresponding to x's base 2 representation
    public static bool[] ToBinArray(int x)
    {
        if (x == 0)
            return new bool[1];

        int len = (int)(Math.Log(x, 2)) + 1;
        bool[] rep = new bool[len];

        for (int i = 0; i < len; i++)
        {
            if (x % 2 == 1)
                rep[i] = true;
            x = x >> 1;
        }

        return rep;
    }

    //converts x into the base 10 integer it represents
    public static int FromBinArray(bool[] x)
    {
        int result = 0;
        for (int i = 0; i < x.Length; i++)
            result += (x[i]) ? (int)Math.Pow(2, i) : 0;
        return result;
    }
}