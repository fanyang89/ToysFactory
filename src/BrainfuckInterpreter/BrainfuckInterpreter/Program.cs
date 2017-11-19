using System;

namespace BrainfuckInterpreter
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Brainfuck Intepreter. Written by fuis. 2016-12-2");
            var interpreter = new Interpreter();
            while (true)
            {
                Console.Write("$ ");
                interpreter.Execute(Console.ReadLine());
            }
        }
    }
}