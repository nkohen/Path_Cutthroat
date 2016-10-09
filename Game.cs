using System;
using System.Collections.Generic;

//Nadav Kohen
//This program implements the game Cutthroat played on a path as well as the solution to that game
public class Game
{
    //grunds[i] stores the grundy number of a path of length i
    static int[] grunds;
    //display[i] is true if something needs to be displayed in position i where positions count both vertices and edges
    static bool[] display;
    //neighbors[i] stores the number of neighbors still in play for vertex i+1, if vertex i+1 is not in play then neighbors[i] will be 0
    static int[] neighbors;
    //pieces stores Path objects corresponding with those still in the game
    static IList<Path> pieces;

    public static void Main()
    {
        //display instructions
        Console.Out.WriteLine("Welcome to the game of Path Cutthroat!\nYou will take turns with the computer removing vertices and their edges from a path.\nIf a vertex loses all of its neighbors, then that vertex will dissapear too.\nThe player to remove the last vertex wins!");
        //retrieve game size (in valid format)
        int size = 0;
        bool number;
        do
        {
            number = true;
            Console.Out.Write("\nHow many vertices will you play with? ");
            try
            {
                size = Convert.ToInt32(Console.ReadLine());
            }
            catch (FormatException e)
            {
                number = false;
            }
        } while (!number || size < 1);

        //initialize grunds as is done in PathCutthroat
        grunds = PathCutthroat.CalcGrunds(size);

        //initialize pieces with a Path the size of the game
        pieces = new List<Path>();
        pieces.Add(new Path(1, size));

        InitDisplay(size);

        //playerTurn true if it is the player's turn, this will be used to determine who wins
        bool playerTurn = true;

        //while the game is not over
        while (pieces.Count > 0)
        {
            playerTurn = true;

            //retrieve the player's move (in valid format)
            int move = 0;
            do
            {
                number = true;
                Console.Out.Write("Which vertex will you remove? ");
                try
                {
                    move = Convert.ToInt32(Console.ReadLine());
                }
                catch (FormatException e)
                {
                    number = false;
                }
            } while (!number || move < 1 || move > size || !display[2 * move - 2]); //true only when the player's input is invalid

            
            Turn(move);

            //if the game is over, break
            if (pieces.Count == 0)
                break;

            //computer's turn
            playerTurn = false;
            move = CalcMove();
            Console.Out.WriteLine("The computer removes vertex " + move + ".");
            Turn(move);
        }
        //write end message
        Console.Out.WriteLine((playerTurn) ? "Congratulations! You played perfectly." : "Sorry, you lost.");
        Console.ReadLine();
    }

    //given an integer, move, remove that vertex and update the display as well as the pieces list
    public static void Turn(int move)
    {
        UpdateDisplay(move);
        DisplayGame(display);

        int i;
        for (i = 0; i < pieces.Count; i++)
        {
            if (pieces[i].Contains(move))
                break;
        }
        pieces.Add(pieces[i].Split(move));
        RemoveEmptyPieces();
    }

    //calculate a winning move if one exists, otherwise just choose the first vertex of the first piece in pieces
    public static int CalcMove()
    {
        //piles represents the grundy numbers of the sizes of each Path in pieces, in binary
        bool[][] piles = new bool[pieces.Count][];
        for (int i = 0; i < pieces.Count; i++)
            piles[i] = PathCutthroat.ToBinArray(grunds[pieces[i].Size()]);
        //totalSum is the sum of all numbers in piles without carry-over
        bool[] totalSum = CalcSum(piles);
        //endAt is the largest index in totalSum to hold a 1 (true)
        int endAt;
        for (endAt = totalSum.Length - 1; endAt >= 0; endAt--)
        {
            if (totalSum[endAt] == true)
                break;
        }

        //endAt == -1 only if there is no winning move (i.e. totalSum == 0) and so return the first vertex of the first piece in pieces
        if (endAt == -1)
            return pieces[0].Front();

        //coorPiles stores the binary numbers in piles truncated at the index endAt
        bool[][] coorPiles = new bool[pieces.Count][];
        for (int i = 0; i < pieces.Count; i++)
        {
            bool[] p = piles[i];
            coorPiles[i] = new bool[Math.Min(endAt + 1, p.Length)];
            for (int j = 0; j < p.Length && j <= endAt; j++)
                coorPiles[i][j] = p[j];
        }

        //the optimal move is to remove from the Path that corresponds to the max number in coorPiles
        int targetIndex = FindMax(coorPiles);
        //the optimal move is to make the targetIndex Path have the grundy number of the sum of all the other Paths (without carry-over)
        //first calculate what this sum is for the truncated numbers (coorPiles) since it is already equal to the sum for all places above since they were zero
        int lowerTargetVal = PathCutthroat.FromBinArray(CalcSumOthers(coorPiles, targetIndex));
        //diff stores the change that needs to happen to pieces[targetIndex] in terms of grundy numbers
        int diff = PathCutthroat.FromBinArray(coorPiles[targetIndex]) - lowerTargetVal;
        //targetVal then stores the grundy number that pieces[targetVal] must have (as an optimal move)
        int targetVal = PathCutthroat.FromBinArray(piles[targetIndex]) - diff;

        //Need to split pieces[targetIndex] so that the two paths sum to targetVal in grundy value
        //splitter will store the index of what needs to be removed in pieces[targetValue] where 0 is the front
        int splitter = 0;
        int n = pieces[targetIndex].Size();
        for (int i = 0; i < n / 2 + 1; i++)
        {
            //when i is found such that it splits pieces[targetIndex] into two piles equivalent to a nim pile of targetVal, set splitter to i and break
            if (PathCutthroat.ModAdd(grunds[i], grunds[n - i - 1]) == targetVal)
            {
                splitter = i;
                break;
            }
        }
        //splitter stores how far into pieces[targetIndex] the vertex to be removed is
        return pieces[targetIndex].Front() + splitter;
    }

    //calculates the modular sum of each bool[] in nums which represents a number in binary
    public static bool[] CalcSum(bool[][] nums)
    {
        if (nums.Length == 1)
            return nums[0];

        bool[] total = nums[0];
        for (int i = 1; i < nums.Length; i++)
            total = PathCutthroat.ModAdd(total, nums[i]);

        return total;
    }

    //calculates the modular sum of each bool[] in nums except for nums[notIndex]
    public static bool[] CalcSumOthers(bool[][] nums, int notIndex)
    {
        if (nums.Length == 1 && notIndex == 0)
            return new bool[1];

        bool[] total = (notIndex != 0) ? nums[0] : nums[1];
        for (int i = ((notIndex != 0) ? 1 : 2); i < nums.Length; i++)
        {
            if (i != notIndex)
                total = PathCutthroat.ModAdd(total, nums[i]);
        }

        return total;
    }

    //finds the maximum number in nums where each bool[] in nums represents a binary number
    public static int FindMax(bool[][] nums)
    {
        int index = 0;
        int num = PathCutthroat.FromBinArray(nums[index]);
        for (int i = 1; i < nums.Length; i++)
        {
            int numCheck = PathCutthroat.FromBinArray(nums[i]);
            if (num < numCheck)
            {
                index = i;
                num = numCheck;
            }
        }
        return index;
    }

    //removes empty Path objects from pieces
    public static void RemoveEmptyPieces()
    {
        for (int i = 0; i < pieces.Count; i++)
        {
            if (pieces[i].IsEmpty())
            {
                pieces.RemoveAt(i);
                i--;
            }
        }
    }

    //Takes the last move made (vertex removed) and updates the display accordingly
    public static void UpdateDisplay(int move)
    {
        //remove vertex move
        display[2 * move - 2] = false;
        neighbors[move - 1] = 0;
        if (move > 1)
        {
            //remove the edge to the left of vertex move
            display[2 * move - 3] = false;
            neighbors[move - 2]--;
            if (neighbors[move - 2] == 0) //If the vertex to the left of vertex move has no neighbors then remove that vertex too
                display[2 * move - 4] = false;
        }
        if (move < neighbors.Length)
        {
            //remove the edge to the right of vertex move
            display[2 * move - 1] = false;
            neighbors[move]--;
            if (neighbors[move] == 0) //If the vertex to the right of vertex move has no neighbors then remove that vertex too
                display[2 * move] = false;
        }
    }

    //Displays the current position of the game
    public static void DisplayGame(bool[] setup)
    {
        for (int i = 0; i < setup.Length; i++)
        {
            if (setup[i])
            {
                if (i % 2 == 0)
                    Console.Out.Write("O");
                else
                    Console.Out.Write("--");
            }
            else
                Console.Out.Write((i % 2 == 0) ? " " : "  ");
        }
        Console.Out.WriteLine();
        for (int i = 0; i < setup.Length; i++)
        {
            if (i % 2 == 0)
                Console.Out.Write((i / 2 + 1) + (( i / 2 + 1 <= 9) ? "  " : " "));
        }
        Console.Out.WriteLine();
    }

    //Initializes the display
    public static void InitDisplay(int size)
    {
        display = new bool[size * 2 - 1];
        for (int i = 0; i < display.Length; i++)
            display[i] = true;

        DisplayGame(display);

        neighbors = new int[size];
        neighbors[0] = 1;
        neighbors[size - 1] = 1;
        for (int i = 1; i < size - 1; i++)
            neighbors[i] = 2;
    }
}