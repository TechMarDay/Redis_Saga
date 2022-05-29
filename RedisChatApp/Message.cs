using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisChatApp
{
    public static class Message
    {
        public static void MessageAction(string message)
        {
            int initialCursorTop = Console.CursorTop;
            int initialCursorLeft = Console.CursorLeft;

            Console.MoveBufferArea(0, initialCursorTop, Console.WindowWidth,
                1, 0, initialCursorTop + 1);
            Console.CursorTop = initialCursorTop;
            Console.CursorLeft = 0;

            // Print the message here
            Console.WriteLine(message);

            Console.CursorTop = initialCursorTop + 1;
            Console.CursorLeft = initialCursorLeft;
        }
    }
}
