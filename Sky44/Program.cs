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
                LoopLefted();

                do
                {
                    _set = false;
                    LoopForN(DoFullCheck);
                } while (_set);

            }

            private void LoopLefted()
            {
                do
                {
                    _set = false;
                    DoForLefted();
                }
                while (_set);
            }

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
                var line = GetLine(x, y, vector);
                var right = GetOppositeIdx(idx);
                var s = _size.ToString();

                //var size = GetLineInfo(line);

                #region  Optimize 
                if (n > 2)
                {
                    //If n - 1 == s than we can use 0.max < 1.max ... < (n - 1).max 
                    if (line[n - 1] == s)
                    {
                        for (int i = n - 2; i >= 0; i--)
                        {
                            var x1 = x;
                            var y1 = y;
                            var max = line[i].Last() - '0';
                            for (int j = 0; j < i; j++)
                            {
                                Remove(x1, y1, max);
                                (x1, y1) = ModifyCoords(x1, y1, vector);
                            }
                        }

                        LoopLefted();
                    }
                }

                #region 2 when already set max value
                if (n == 2)
                {
                    var sindx = line.IndexOf(s);
                    // if set in the last position, 0 - always will be _size - 1;
                    if (sindx == _max)
                    {
                        SetAndDelete(x, y, _max);
                        LoopLefted();

                        line = GetLine(x, y, vector); //?
                    }

                    if (sindx > 1)
                    {
                        
                        //If set size, we can delete some nums from first position, if it's not set on the 2nd 
                        var max = line.Skip(1).Take(sindx - 1).Where(i => i.Length == 1);
                        if (max.Any())
                        {
                            var val = max.Select(s => s[0] - '0').Max();
                            while (val > 0)
                            {
                                Remove(x, y, val);
                                val--;
                            }
                        } else
                        {
                            Remove(x, y, 1);
                        }

                        LoopLefted();
                        line = GetLine(x, y, vector);
                    }

                    if (sindx == n)
                    {
                        var last = line[0].Last();
                        if (line[1].Last() == last)
                        {
                            (int x1, int y1) = ModifyCoords(x, y, vector);
                            Remove(x1, y1, last - '0');
                            LoopLefted();
                        }
                    }

                    //if set first and max, we can delete all greater than first in range between fst and max
                    if (line[0].Count() == 1)
                        {
                            var x1 = x;
                            var y1 = y;
                            var val = line[0].First() - '0';
                            for (int i = sindx - 1; i > 0; i--)
                            {
                                (x1, y1) = ModifyCoords(x1, y1, vector);
                                for (int j = val + 1; j < _size; j++)
                                {
                                    Remove(x1, y1, j);
                                }
                            }
                        }


                }
                #endregion

                //if set first value == max on position (n-1) 
                if(line[0].Count() == 1)
                {
                    var sindx = line.IndexOf(s);
                    if(sindx > 1 && line[n - 1] == s)
                    {
                        var from = line[0].First() - '0';
                        if (from == _size - n + 1)
                        {
                            var (x1, y1) = ModifyCoords(x, y, vector);
                            for (int i = from + 1; i < _size; i++)
                            {
                                SetAndDelete(x1, y1, i);
                                (x1, y1) = ModifyCoords(x1, y1, vector);
                            }
                        }
                    }
                }
              
                #endregion optimize
            }


            private (int, string) GetLineInfo(List<string> line)
            {
                var size = 0;
                var max = line[0].Last().ToString();

                foreach (var item in line)
                {
                    if (item.Length == 1)
                    {
                        if (item[0] > max[0])
                        {
                            max = item;
                            size++;
                        }
                    }
                }

                return (size, max);
            }

            private List<string> GetLine(int x, int y, Vector v)
            {
                var list = new List<string>(4);
                switch (v)
                {
                    case Vector.Left:
                        return _arr[x].ToList();
                    case Vector.Right:
                        return _arr[x].Reverse().ToList();
                    case Vector.Top:
                        for (int i = 0; i < _size; i++)
                        {
                            list.Add(_arr[i][y]);
                        }
                        return list;
                    case Vector.Bottom:
                        for (int i = _max; i >= 0; i--)
                        {
                            list.Add(_arr[i][y]);
                        }
                        return list;
                    default: return list;
                }
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
                    (x, y) = ModifyCoords(x, y, vector);
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

            /*     private void ModifyCoords(ref int x, ref int y, Vector vector)
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
                 }*/

            private (int x, int y) ModifyCoords(int x, int y, Vector vector)
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

                return (x, y);
            }

            private bool IsFinish()
            {
                for (int i = 0; i < _size; i++)
                {
                    for (int j = 0; j < _size; j++)
                    {
                        if (_arr[i][j].Length > 1) return false;
                    }
                }

                return true;
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
