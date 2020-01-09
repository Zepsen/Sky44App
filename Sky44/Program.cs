using System;
using System.Collections.Generic;
using System.Linq;

namespace Sky44
{
    [Flags]
    public enum Vector
    {
        Left = 1,
        Right = 2,
        Top = 4,
        Bottom = 8,

    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var resolver = new Resolve();
            resolver.Run(
                new int[] {
                               2, 2, 1, 3,
                               2, 2, 3, 1,
                               1, 2, 2, 3,
                               3, 2, 1, 3
                });

            resolver = new Resolve();
            resolver.Run(new[]
            {
                               0, 0, 1, 2,
                               0, 2, 0, 0,
                               0, 3, 0, 0,
                               0, 1, 0, 0
                        });

            Console.ForegroundColor = ConsoleColor.Yellow;

            resolver = new Resolve();
            resolver.Run(new[]
            {
                            3, 2, 2, 3, 2, 1,
                            1, 2, 3, 3, 2, 2,
                            5, 1, 2, 2, 4, 3,
                            3, 2, 1, 2, 2, 4
                        });

            resolver = new Resolve();
            resolver.Run(new[]
            {
                0, 0, 0, 2, 2, 0,
                0, 0, 0, 6, 3, 0,
                0, 4, 0, 0, 0, 0,
                4, 4, 0, 3, 0, 0
            });

            resolver = new Resolve();
            resolver.Run(new[] {
                 7, 0, 0, 0, 2, 2, 3,
                 0, 0, 3, 0, 0, 0, 0,
                 3, 0, 3, 0, 0, 5, 0,
                 0, 0, 0, 0, 5, 0, 4
            });

            resolver = new Resolve();
            resolver.Run(new[]
            {
                 0, 2, 3, 0, 2, 0, 0,
                 5, 0, 4, 5, 0, 4, 0,
                 0, 4, 2, 0, 0, 0, 6,
                 5, 2, 2, 2, 2, 4, 1
            });
        }

        public class Resolve
        {
            private int _size;
            private int _max;
            private bool _set;

            private string[][] _arr;
            private List<int> _clues = new List<int>();

            public void Run(int[] clues)
            {
                _clues = clues.ToList();
                _size = clues.Length / 4;
                _arr = Generate();
                _max = _size - 1;
                _set = false;

                Console.WriteLine("Size is " + _size);
                Proceed();

                Console.WriteLine("Final");
                Display();
            }
         
            private void Proceed()
            {
                Base();

                /*do
                {
                    _set = false;
                    DoForLefted();
                }
                while (_set);

                Display();*/

                /*do
                {
                    _set = false;
                    LoopForN(DoFullCheck);
                }
                while (_set);*/

            }

            //Do not delete yet
            private void LoopForN(Action<int, int> func)
            {
                for (int n = 2; n < _size; n++)
                {
                    if (_clues.Any(i => i == n))
                    {
                        var idx = _clues.IndexOf(n, 0);
                        while (idx != -1)
                        {
                            func(idx, n);
                            idx = _clues.IndexOf(n, idx + 1);
                        }
                    }
                }
            }

            private void DoFullCheck(int idx, int n)
            {
                (int x, int y, Vector vector) = GetCoords(idx);
                var val = _size - (n - 1);
                var s = _size.ToString();
                switch (vector)
                {
                    case Vector.Left:
                        if (_arr[x][_max] == s)
                        {
                            SetAndDelete(x, 0, val);
                        }

                        break;
                    case Vector.Right:
                        if (_arr[x][0] == s)
                        {
                            SetAndDelete(x, _max, val);
                        }
                        break;
                    case Vector.Top:
                        if (_arr[_max][y] == s)
                        {
                            if (n == _size - 1)
                                SetAndDelete(0, y, val);
                        }
                        break;
                    case Vector.Bottom:
                        if (_arr[0][y] == s)
                        {
                            SetAndDelete(_max, y, val);
                        }
                        break;
                }

                Console.WriteLine($"Index {idx}, Vector {vector}");
                Display();

            }

            // todo: optimize this but it works correct
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

                        if (res.Any() && new string(res.ToArray()) != temp)
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

            #region Base 
            private void Base()
            {
                for (int i = 0; i < _clues.Count; i++)
                {
                    if (_clues[i] == 0) continue;

                    if (_clues[i] == 1)
                        DoFor1(i);

                    else if (_clues[i] == _size)
                        DoForSize(i);

                    else
                    {
                        DoForOpposite(i);
                        DoForN(i, _clues[i]);
                    }
                }
            }

            private void DoForN(int idx, int n)
            {
                (int x, int y, Vector vector) = GetCoords(idx);
                while (n > 0)
                {
                    for (int i = 0; i < n - 1; i++)
                    {
                        Remove(x, y, _size - i);
                    }

                    n--;
                    ModifyCoords(ref x, ref y, vector);
                }
            }

            private void DoForSize(int idx, int from = 0)
            {
                var vector = GetVector(idx);
                int j = 0;
                switch (vector)
                {
                    case Vector.Top:
                        for (int i = 0, n = 1 + from; i < _size; i++, n++)
                        {
                            SetAndDelete(i, idx, n);
                        }
                        break;
                    case Vector.Right:
                        j = idx % _size;
                        for (int i = _max, n = 1 + from; i >= 0; i--, n++)
                        {
                            SetAndDelete(j, i, n);
                        }
                        break;
                    case Vector.Bottom:
                        j = _max - idx % _size;
                        for (int i = _max, n = 1 + from; i >= 0; i--, n++)
                        {
                            SetAndDelete(i, j, n);
                        }
                        break;
                    case Vector.Left:
                        j = _max - idx % _size;
                        for (int i = 0, n = 1 + from; i < _size; i++, n++)
                        {
                            SetAndDelete(i, j, n);
                        }
                        break;
                }
            }

            private void DoFor1(int idx)
            {
                (int x, int y, _) = GetCoords(idx);
                SetAndDelete(x, y, _size);
            }

            private void DoForOpposite(int idx)
            {
                var right = GetOppositeIdx(idx);
                (int x, int y, Vector v) = GetCoords(idx);

                /// For 7 - 62 | 53 | 44 
                ///     6 - 43 | 52 
                ///     4 - 32 
                if (_clues[idx] + right == _size + 1)
                {
                    switch (v)
                    {
                        case Vector.Left:
                            SetAndDelete(x, _clues[idx] - 1, _size);
                            break;
                        case Vector.Right:
                            SetAndDelete(x, right - 1, _size);
                            break;
                        case Vector.Top:
                            SetAndDelete(_clues[idx] - 1, y, _size);
                            break;
                        case Vector.Bottom:
                            SetAndDelete(right - 1, y, _size);
                            break;
                        default:
                            break;
                    }
                }
            }

            #endregion Base

            #region Helpers

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
                        //_set = true;
                    }
                }
            }

            private void SetAndDelete(int i, int j, int item)
            {
                Set(i, j, item);
                DeleteInLineAndRow(i, j, item);
            }

            private void Set(int x, int y, int n)
            {
                if (_arr[x][y].Length > 1)
                {
                    _arr[x][y] = n.ToString();
                    _set = true;
                }
            }

            private Vector GetVector(int idx)
            {
                if (idx < _size)
                    return Vector.Top;
                else if (idx >= _size && idx < _size * 2)
                    return Vector.Right;
                else if (idx >= _size * 2 && idx < _size * 3)
                    return Vector.Bottom;
                else
                    return Vector.Left;
            }

            (int x, int y, Vector vector) GetCoords(int idx)
            {
                int x = 0, y = 0;
                var vector = GetVector(idx);

                switch (vector)
                {
                    case Vector.Left:
                        x = _max - idx % _size;
                        y = 0;
                        break;
                    case Vector.Right:
                        y = _max;
                        x = idx % _size;
                        break;
                    case Vector.Top:
                        y = idx % _size;
                        x = 0;
                        break;
                    case Vector.Bottom:
                        y = _max - idx % _size;
                        x = _max;
                        break;
                }

                return (x, y, vector);
            }

            private int GetOppositeIdx(int idx)
            {
                var vector = GetVector(idx);
                switch (vector)
                {
                    case Vector.Left:
                        return _clues[_size * 2 - 1 - (idx % _size)];
                    case Vector.Right:
                        return _clues[_size * 4 - 1 - (idx % _size)];
                    case Vector.Top:
                        return _clues[_size * 3 - 1 - idx];
                    case Vector.Bottom:
                        return _clues[_max - idx % 4];
                    default: return 0;
                }
            }

            private void ModifyCoords(ref int x, ref int y, Vector vector)
            {
                switch (vector)
                {
                    case Vector.Left:
                        y++;
                        break;
                    case Vector.Right:
                        y--;
                        break;
                    case Vector.Top:
                        x++;
                        break;
                    case Vector.Bottom:
                        x--;
                        break;
                }
            }

            #endregion Helpers

            #region Generate
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

            #endregion Generate

            private void Display()
            {
                Console.Write("  ");
                for (int k = 0; k < _size; k++)
                {
                    Console.Write($"{_clues[k]}" + new string(' ', _size));
                }

                Console.WriteLine();

                for (int i = 0; i < _size; i++)
                {
                    Console.Write(_clues[_clues.Count - 1 - i]);

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    for (int j = 0; j < _size; j++)
                    {
                        Console.Write($" {_arr[i][j]}" + new string(' ', _size - _arr[i][j].Length));
                    }

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(_clues[_size + i]);
                    Console.WriteLine();
                }

                Console.Write("  ");
                for (int k = 0; k < _size; k++)
                {
                    Console.Write($"{_clues[_size * 3 - 1 - k]}" + new string(' ', _size));
                }

                Console.WriteLine();
                Console.WriteLine(new string('-', 50));
            }

        }
    }
}
