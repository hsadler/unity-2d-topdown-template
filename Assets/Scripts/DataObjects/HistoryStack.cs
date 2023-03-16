using UnityEngine;

class HistoryStack<T>
{


    // Inspired by: https://stackoverflow.com/questions/384042/can-i-limit-the-depth-of-a-generic-stack


    private T[] items;
    private int top;
    private int cursor;

    private bool useLogging = true;


    public HistoryStack(int capacity)
    {
        this.items = new T[capacity];
        this.top = -1;
        this.cursor = -1;
    }

    public void Push(T item)
    {
        // push state forward, top of stack considered to be last pushed 
        // position
        this.cursor = this.cursor + 1 % this.items.Length;
        this.items[this.cursor] = item;
        this.top = this.cursor;
        if (this.useLogging)
        {
            Debug.Log("Pushing item to HistoryStack with Cursor: " + this.cursor.ToString() + " and Top: " + this.top.ToString());
        }
    }

    public T Previous()
    {
        // move cursor back one position 
        int newCursor = (this.items.Length + this.cursor - 1) % this.items.Length;
        // if no history
        // OR
        // if provisional cursor position is same as top position, return 
        // default, cursor position remains unchanged
        if (this.cursor == -1 || newCursor == this.top)
        {
            if (this.useLogging)
            {
                Debug.Log("Cannot get HistoryStack Previous item. Returning default.");
            }
            return default(T);
        }
        // commit new cursor postion, return item, top of stack position remains 
        // unchanged
        this.cursor = newCursor;
        if (this.useLogging)
        {
            Debug.Log("Setting HistoryStack to Previous position with Cursor: " + this.cursor.ToString() + " and Top: " + this.top.ToString());
        }
        return this.items[this.cursor];
    }

    public T Next()
    {
        // if no history of already at the top of the stack return null
        if (this.cursor == -1 || this.cursor == this.top)
        {
            if (this.useLogging)
            {
                Debug.Log("Cannot get HistoryStack Next since Cursor is already at Top with shared value: " + this.cursor.ToString() + ". Returning default.");
            }
            return default(T);
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

    public bool IsTop()
    {
        return this.cursor == this.top;
    }


}