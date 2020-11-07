using System;
using System.Threading;
using System.Numerics;


namespace VizzHuiz
{
    class ConsoleDrawer
    {
        public static float GetDeltaWidth() 
        {
            return (float)Console.WindowWidth / Constants.InitialWidth;
        }
        public static float GetDeltaHeight()
        { 
            return (float)Console.WindowHeight / Constants.InitialHeight;
        }
        int top_cursor;
        public int SleepindAfterDraw;
        public ConsoleDrawer(int sleepindAfterDraw=0)
        {
            top_cursor = Console.CursorTop + 1;
            SleepindAfterDraw = sleepindAfterDraw;
        }
        public void ResetCursorTop()
        {
            //moving cursor to top 
            Console.SetCursorPosition(0, top_cursor);

            //expanding strings in console
            var height = Console.WindowHeight;
            for (int i = 0; i < height; i++)
            {
                Console.WriteLine();
            }
            
            //returning cursor back
            Console.SetCursorPosition(0,0);
        }
        public void DrawTriangle(Vector2 p1, Vector2 p2, Vector2 p3, ConsoleColor color=ConsoleColor.White)
        {
            DrawLine(p1, p2, color);
            DrawLine(p2, p3, color);
            DrawLine(p3, p1, color);
        }
        public void DrawLine(Vector2 start, Vector2 end, ConsoleColor color=ConsoleColor.White)
        {
            var dw = GetDeltaWidth();
            var dh = GetDeltaHeight();
            //scaling to console coords
            start.X *= dw;
            end.X *= dw;

            start.Y *= dh;
            end.Y *= dh;

            int newTop = (int)Math.Max(start.Y, end.Y) + 1;
            if(newTop > top_cursor) top_cursor = newTop;

            var lastColor = Console.BackgroundColor;
            Console.BackgroundColor = color;
            float dx = end.X - start.X;
            float dy = end.Y - start.Y;

            float steps = Math.Max(Math.Abs(dx), Math.Abs(dy));

            float mx = (float)dx / steps;
            float my = (float)dy / steps;

            float x = start.X;
            float y = start.Y;

            for (int i = 0; i < steps; i++)
            {
                //writing in global coords with cursor offsets
                Console.SetCursorPosition((int)x, (int)y);
                Console.Write('*');
                Thread.Sleep(SleepindAfterDraw);
                x += mx;
                y += my;
            }
            Console.BackgroundColor = lastColor;
        }

        public void WriteAt(string str, int left, int top)
        {
            Console.SetCursorPosition(left, top);
            Console.Write(str);
            Thread.Sleep(SleepindAfterDraw);

            int newTop = top + 1;
            if (newTop > top_cursor) top_cursor = newTop;
        }

        public void WriteAt(char sym, int left, int top)
        {
            Console.SetCursorPosition(left, top);
            Console.Write(sym);
            Thread.Sleep(SleepindAfterDraw);

            int newTop = top + 1;
            if (newTop > top_cursor) top_cursor = newTop;
        }

        public void WriteAt(string str, int left, int top, ConsoleColor color)
        {
            var lastColor = Console.BackgroundColor;
            Console.BackgroundColor = color;
            Console.SetCursorPosition(left, top);
            Console.Write(str);
            Console.BackgroundColor = lastColor;
            Thread.Sleep(SleepindAfterDraw);

            int newTop = top + 1;
            if (newTop > top_cursor) top_cursor = newTop;
        }

        public void WriteAt(char sym, int left, int top, ConsoleColor color)
        {
            var lastColor = Console.BackgroundColor;
            Console.BackgroundColor = color;
            Console.SetCursorPosition(left, top);
            Console.Write(sym);
            Console.BackgroundColor = lastColor;
            Thread.Sleep(SleepindAfterDraw);

            int newTop = top + 1;
            if (newTop > top_cursor) top_cursor = newTop;
        }
    }
}