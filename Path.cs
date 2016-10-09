using System;

//This class defines the Path object which represents a segment of the game being played in Game.cs
public class Path
{
    //the index of the front of this Path
    int front;
    //the index of the back of this Path
    int back;

    public Path(int front, int back)
    {
        this.front = front;
        this.back = back;
    }

    //splits this Path into two by removing vertex i, setting this to be the front half and returns a new Path containing the second half
    public Path Split(int i)
    {
        int backEnd = back;
        back = i - 1;
        return new Path(i + 1, backEnd);
    }

    //returns true if this Path contains the vertex of index n
    public bool Contains(int n)
    {
        return (front <= n && back >= n);
    }

    //returns true if this Path is one vertex or less
    public bool IsEmpty()
    {
        return (back - front <= 0) ? true : false;
    }

    //returns the number of vertices in this Path
    public int Size()
    {
        return back - front + 1;
    }

    //returns the index of the first vertex in this Path
    public int Front()
    {
        return front;
    }

    public override string ToString()
    {
        return front + ", " + back;
    }
}