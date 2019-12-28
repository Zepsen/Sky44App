using System;
using System.Collections.Generic;
using System.Linq;

namespace Sky44
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var resolver = new Resolve();
            resolver.Run(
                new int[] {
                    0, 0, 1, 2,
                    0, 2, 0, 0,
                    0, 3, 0, 0,
                    0, 1, 0, 0
                });
        }
    }

    public class Resolve
    {
        private int _size;
        private string[][] _arr;
        private List<int> _clues = new List<int>();

        public void Run(int[] clues)
        {
            _clues = clues.ToList();
            _size = clues.Length / 4;
            _arr = Generate();

            Console.WriteLine("Size is " + _size);
            Proceed();

            for (int i = 0; i < _size; i++)
            {
                for (int j = 0; j < _size; j++)
                {
                    Console.Write($" {_arr[i][j]}");
                }

                Console.WriteLine();
            }
        }

        private void Proceed()
        {
            //for last
            if (_clues.Any(i => i == _size))
            {
                var idx = _clues.IndexOf(_size, 0);
                while (idx != -1)
                {
                    DoForSize(idx);
                    idx = _clues.IndexOf(_size, idx + 1);
                }
            }

            // for first
            if (_clues.Any(i => i == 1))
            {
                var idx = _clues.IndexOf(1, 0);
                while (idx != -1)
                {
                    DoFor1(idx);
                    idx = _clues.IndexOf(1, idx + 1);
                }
            }

            // From snd to last
            for (int n = 2; n < _size; n++)
            {
                if (_clues.Any(i => i == n))
                {
                    var idx = _clues.IndexOf(n, 0);
                    while (idx != -1)
                    {
                        DoForN(idx, n);
                        idx = _clues.IndexOf(n, idx + 1);
                    }
                }
            }
        }

        private void DoForN(int idx, int n)
        {
            Console.WriteLine($"Idx {idx}, {n}");
            (int x, int y) = GetCoords(idx);
            for (int i = 0; i < n-1; i++)
            {
                for (int j = 0; j < _size - n; j++)
                {
                    Remove(x, y, _size - i - j);
                }
            }

            //DoForN(x-1, y-1, n-1);
        }

        private void DoForSize(int idx)
        {
            Console.WriteLine($"{_size} - x y");
            if (idx < _size)
            {
                for (int i = 0, n = 1; i < _size; i++, n++)
                {
                    Set(i, idx, n);
                    DeleteInLineAndRow(i, idx, n);
                }
            }

            else if (idx >= _size && idx < _size * 2)
            {
                var j = idx % _size;
                for (int i = _size - 1, n = 1; i >= 0; i--, n++)
                {
                    Set(j, i, n);
                    DeleteInLineAndRow(j, i, n);
                }
            }

            else if (idx >= _size * 2 && idx < _size * 3)
            {
                var j = _size - 1 - idx % _size;
                for (int i = _size - 1, n = 1; i >= 0; i--, n++)
                {
                    Set(i, j, n);
                    DeleteInLineAndRow(i, j, n);
                }
            }

            else
            {
                var j = _size - 1 - idx % _size;
                for (int i = 0, n = 1; i < _size; i++, n++)
                {
                    Set(j, i, n);
                    DeleteInLineAndRow(j, i, n);
                }
            }

        }

        private void DoFor1(int idx)
        {
            (int x, int y) = GetCoords(idx);
            Console.WriteLine($"1 - x{x} y{y}");

            Set(x, y, _size);
            DeleteInLineAndRow(x, y, _size);
        }

        private void DeleteInLineAndRow(int x, int y, int n)
        {
            for (int i = 0; i < _size; i++)
            {
                Remove(x, i, n);
                Remove(i, y, n);
            }
        }

        private void Remove(int x, int y, int n)
        {
            if (_arr[x][y].Length > 1)
            {
                _arr[x][y] = _arr[x][y].Replace(n.ToString(), "");

                if (_arr[x][y].Length == 1)
                {
                    DeleteInLineAndRow(x, y, Convert.ToInt32(_arr[x][y]));
                }
            }
        }

        private void Set(int x, int y, int n)
        {
            if (_arr[x][y].Length > 1)
            {
                _arr[x][y] = n.ToString();
            }
        }

        (int x, int y) GetCoords(int idx)
        {
            int x, y;

            if (idx < _size)
            {
                y = idx % _size;
                x = 0;
            }

            else if (idx >= _size && idx < _size * 2)
            {
                x = _size - 1;
                y = idx % _size;
            }

            else if (idx >= _size * 2 && idx < _size * 3)
            {
                x = _size - 1 - idx % _size;
                y = _size - 1;
            }

            else
            {
                x = 0;
                y = _size - 1 - idx % _size;
            }

            return (x, y);
        }

        string[][] Generate()
        {
            var res = new string[_size][];
            for (int i = 0; i < _size; i++)
            {
                res[i] = new string[_size];
                for (int j = 0; j < _size; j++)
                {
                    res[i][j] = GenerateValue();
                }
            }

            return res;
        }

        string GenerateValue()
        {
            char[] res = new char[_size];
            for (int i = 0; i < _size; i++)
            {
                res[i] = Convert.ToChar(i + 49);
            }

            return new string(res);
        }
    }
}
