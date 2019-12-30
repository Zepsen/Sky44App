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
                   //0, 0, 0, 0,
                   //0, 0, 0, 0,
                   //0, 0, 0, 0,
                   //0, 0, 0, 0

                   //0, 0, 1, 2,
                   //0, 2, 0, 0,
                   //0, 3, 0, 0,
                   //0, 1, 0, 0

                   2, 2, 1, 3,
                   2, 2, 3, 1,
                   1, 2, 2, 3,
                   3, 2, 1, 3
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
            Display();
        }

        private void Display()
        {
            for (int i = 0; i < _size; i++)
            {
                for (int j = 0; j < _size; j++)
                {
                    Console.Write($" {_arr[i][j]}" + new string(' ', _size - _arr[i][j].Length));
                }

                Console.WriteLine();
            }

            Console.WriteLine(new string('-', 50));
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
            Display();

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
            Display();

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

                Display();
            }

            DoForLefted();
        }

        // todo: optimize this
        private void DoForLefted()
        {
            List<char> res = new List<char>(4);

            for (int i = 0; i < _size; i++)
            {
                for (int j = 0; j < _size; j++)
                {
                    //row
                    var temp = _arr[i][j];
                    res = temp.ToList();
                    for (int k = 0; k < _size; k++)
                    {
                        if (j == k) continue;
                        if (_arr[i][k].Length == 1) continue;

                        res = res.Except(_arr[i][k]).ToList();
                        if (!res.Any()) break;
                    }

                    if(res.Any() && new string(res.ToArray()) != temp)
                    {
                        foreach (var item in res)
                        {
                            SetAndDelete(i, j, item - '0');
                        }
                    }


                    //cols
                    temp = _arr[j][i];
                    res = temp.ToList();
                    for (int k = 0; k < _size; k++)
                    {
                        if (j == k) continue;
                        if (_arr[k][i].Length == 1) continue;

                        res = res.Except(_arr[k][i]).ToList();
                        
                        if (!res.Any()) break;
                    }


                    if (res.Any() && new string(res.ToArray()) != temp)
                    {
                        foreach (var item in res)
                        {
                            SetAndDelete(j, i, item - '0');
                        }
                    }

                }

            } 
        }

        private void SetAndDelete(int i, int j, int item)
        {
            Set(i, j, item);
            DeleteInLineAndRow(i, j, item);
        }

        private void DoForN(int idx, int n)
        {
            Console.WriteLine($"idx {idx},  n {n}");
            (int x, int y) = GetCoords(idx);

            while (n > 0)
            {
                for (int i = 0; i < n - 1; i++)
                {
                    Remove(x, y, _size - i);
                }

                n--;
                ModifyCoords(ref x, ref y, idx);
            }
        }

        private void ModifyCoords(ref int x, ref int y, int idx)
        {
            if (idx < _size)
                x++;
            else if (idx >= _size && idx < _size * 2)
                y--;
            else if (idx >= _size * 2 && idx < _size * 3)
                x--;
            else
                y++;
        }

        private void DoForSize(int idx)
        {
            Console.WriteLine($"{_size} - idx{idx}");
            if (idx < _size)
            {
                for (int i = 0, n = 1; i < _size; i++, n++)
                {
                    SetAndDelete(i, idx, n);
                }
            }

            else if (idx >= _size && idx < _size * 2)
            {
                var j = idx % _size;
                for (int i = _size - 1, n = 1; i >= 0; i--, n++)
                {
                    SetAndDelete(j, i, n);
                }
            }

            else if (idx >= _size * 2 && idx < _size * 3)
            {
                var j = _size - 1 - idx % _size;
                for (int i = _size - 1, n = 1; i >= 0; i--, n++)
                {
                    SetAndDelete(i, j, n);
                }
            }

            else
            {
                var j = _size - 1 - idx % _size;
                for (int i = 0, n = 1; i < _size; i++, n++)
                {                   
                    SetAndDelete(i, j, n);
                }
            }

        }

        private void DoFor1(int idx)
        {
            (int x, int y) = GetCoords(idx);
            Console.WriteLine($"1 - x{x} y{y}");

            SetAndDelete(x, y, _size);
        }

        private void DeleteInLineAndRow(int x, int y, int n)
        {
            for (int i = 0; i < _size; i++)
            {
                Remove(x, i, n);
                Remove(i, y, n);
            }
        }

        //private void DeleteInRow(int x, int y, int n)
        //{
        //    for (int i = 0; i < _size; i++)
        //    {
        //        Remove(x, i, n);                
        //    }
        //}

        //private void DeleteInLine(int x, int y, int n)
        //{
        //    for (int i = 0; i < _size; i++)
        //    {
        //        Remove(i, y, n);
        //    }
        //}


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
                y = _size - 1;
                x = idx % _size;
            }

            else if (idx >= _size * 2 && idx < _size * 3)
            {
                y = _size - 1 - idx % _size;
                x = _size - 1;
            }

            else
            {
                x = _size - 1 - idx % _size;
                y = 0;
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
