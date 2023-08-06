using UnityEngine;

class HistoryStack<T>
{


    // Inspired by: https://stackoverflow.com/questions/384042/can-i-limit-the-depth-of-a-generic-stack


    private readonly T[] items;
    private int top;
    private int bottom;
    private int cursor;

    private readonly bool useLogging = false;


    public HistoryStack(int capacity)
    {
        this.items = new T[capacity];
        this.top = -1;
        this.bottom = -1;
        this.cursor = -1;
    }

    public T GetCurrent()
    {
        if (this.cursor > -1)
        {
            return this.items[this.cursor];
        }
        return default;
    }

    public void Push(T item)
    {
        // push state forward, top of stack always considered to be last pushed 
        // position
        this.cursor = (this.cursor + 1) % this.items.Length;
        this.items[this.cursor] = item;
        this.top = this.cursor;
        // if top has wrapped around to reach bottom, push bottom forward by one 
        // position
        if (this.top == this.bottom)
        {
            this.bottom = (this.top + 1) % this.items.Length;
        }
        // make bottom 0 if this is the first history push
        if (this.bottom == -1)
        {
            this.bottom = 0;
        }
        if (this.useLogging)
        {
            Debug.Log("Pushing item to HistoryStack with Cursor: " + this.cursor.ToString() + " and Top: " + this.top.ToString());
        }
    }

    public T Previous()
    {
        // if no history yet
        // OR
        // cursor is at the bottom if the stack, return default, cursor position 
        // remains unchanged
        if (this.cursor == -1 || this.IsBottom())
        {
            if (this.useLogging)
            {
                Debug.Log("Cannot get HistoryStack Previous item. Returning default.");
            }
            return default;
        }
        // move cursor postion backward, return item, top of stack position 
        // remains unchanged
        this.cursor = (this.items.Length + this.cursor - 1) % this.items.Length;
        if (this.useLogging)
        {
            Debug.Log("Setting HistoryStack to Previous position with Cursor: " + this.cursor.ToString() + " and Top: " + this.top.ToString());
        }
        return this.items[this.cursor];
    }

    public T Next()
    {
        // if no history yet or already at the top of the stack return default
        if (this.cursor == -1 || this.IsTop())
        {
            if (this.useLogging)
            {
                Debug.Log("Cannot get HistoryStack Next since Cursor is already at Top with shared value: " + this.cursor.ToString() + ". Returning default.");
            }
            return default;
        }
        // otherwise move cursor forward one position and return item, top of 
        // stack position remains unchanged
        this.cursor = (this.items.Length + this.cursor + 1) % this.items.Length;
        if (this.useLogging)
        {
            Debug.Log("Setting HistoryStack to Next position with Cursor: " + this.cursor.ToString() + " and Top: " + this.top.ToString());
        }
        return this.items[this.cursor];
    }

    private bool IsBottom()
    {
        return this.cursor == this.bottom;
    }

    private bool IsTop()
    {
        return this.cursor == this.top;
    }


}