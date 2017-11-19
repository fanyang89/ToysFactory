using System;

namespace BrainfuckInterpreter
{
    internal class Interpreter
    {
        private readonly char[] _memory;

        public Interpreter(int memorySize = 1000)
        {
            _memory = new char[memorySize];
        }

        public void Execute(string code)
        {
            var currentMemoryPosition = 0;
            var pos = 0;
            while (pos < code.Length)
            {
                var ins = code[pos];
                switch (ins)
                {
                    case '>':
                        currentMemoryPosition++;
                        break;
                    case '<':
                        currentMemoryPosition--;
                        break;
                    case '+':
                        _memory[currentMemoryPosition]++;
                        break;
                    case '-':
                        _memory[currentMemoryPosition]--;
                        break;
                    case '.':
                        Console.Write(_memory[currentMemoryPosition]);
                        break;
                    case ',':
                        _memory[currentMemoryPosition] = (char)Console.Read();
                        break;
                    case '[':
                        if (_memory[currentMemoryPosition] == 0)
                        {
                            while (code[pos] != ']')
                            {
                                pos++;
                            }
                        }
                        break;
                    case ']':
                        if (_memory[currentMemoryPosition] != 0)
                        {
                            while (code[pos] != '[')
                            {
                                pos--;
                            }
                        }
                        break;
                }
                pos++;
            }
        }
    }
}