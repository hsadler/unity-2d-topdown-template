
class HistoryStack<T>
{


    // Inspired by: https://stackoverflow.com/questions/384042/can-i-limit-the-depth-of-a-generic-stack


    private T[] items;
    private int top;
    private int cursor;


    public HistoryStack(int capacity)
    {
        this.items = new T[capacity];
        this.top = 0;
        this.cursor = 0;
    }

    public void Push(T item)
    {
        // push state forward, top of stack considered to be last pushed 
        // position
        this.cursor = this.cursor + 1 % this.items.Length;
        this.items[this.cursor] = item;
        this.top = this.cursor;
    }

    public T Previous()
    {
        // move cursor back one position 
        int newCursor = (this.items.Length + this.cursor - 1) % this.items.Length;
        // if provisional cursor position is top position, return null and 
        // cursor position remains unchanged
        if (newCursor == this.top)
        {
            return default(T);
        }
        // commit new cursor postion, return item, top of stack position remains 
        // unchanged
        this.cursor = newCursor;
        return this.items[this.cursor];
    }

    public T Next()
    {
        // if already at the top of the stack return null
        if (this.cursor == this.top)
        {
            return default(T);
        }
        // otherwise move cursor forward one position and return item, top of 
        // stack position remains unchanged
        this.cursor = (this.items.Length + this.cursor + 1) % this.items.Length;
        return this.items[this.cursor];
    }


}