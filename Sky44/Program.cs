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
            /*            resolver.Run(
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
            */
            resolver = new Resolve();
            resolver.Run(new[]
            {
                0, 0, 0, 2, 2, 0,
                0, 0, 0, 6, 3, 0,
                0, 4, 0, 0, 0, 0,
                4, 4, 0, 3, 0, 0
            });
        }
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
            Display();
        }

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
            LoopForN(DoForN);
            Display();

            do
            {
                _set = false;
                DoForLefted();
            }
            while (_set);

            Display();

            /* do
             {
                 _set = false;
                 LoopForN(DoFullCheck);
             }
             while (_set);*/
            GoFull();

            Display();
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

        private void GoFull()
        {
            for (int idx = 0; idx < _clues.Count - 1; idx++)
            {

                (int x, int y, Vector vector) = GetCoords(idx);
                var left = _clues[idx];
                int right = 0;

                var temp = new string[_size];
                switch (vector)
                {
                    case Vector.Left:
                        temp = _arr[x].ToArray();
                        right = _clues[_size * 2 - 1 - (idx % _size)];
                        break;
                    case Vector.Right:
                        temp = _arr[x].Reverse().ToArray();
                        right = _clues[_size * 4 - 1 - (idx % _size)];
                        break;
                    case Vector.Top:
                        for (int i = 0; i < _size; i++)
                        {
                            temp[i] = _arr[i][y];
                        }
                        right = _clues[_size * 3 - 1 - idx];
                        break;
                    case Vector.Bottom:
                        for (int i = _max; i >= 0; i--)
                        {
                            temp[_max - i] = _arr[i][y];
                        }
                        right = _clues[_max - idx % 4];
                        break;
                }

                AnalyzeLine(temp, left, right);

            }
        }

        private void AnalyzeLine(string[] temp, int left, int right)
        {
            if (left == 0 && right == 0) return;
            if (left == _size || right == _size) return;

            Console.WriteLine($"{left} {right}");

/*          if (temp[_max] == _size.ToString())
            {
                temp[0] = (_size - (left - 1)).ToString();
            } 
*/        }

        [Obsolete]
        private void DoFullCheck(int idx, int n)
        {
            (int x, int y, Vector vector) = GetCoords(idx);
            var val = _size - (n - 1);

            switch (vector)
            {
                case Vector.Left:
                    if (_arr[x][_max] == _size.ToString())
                    {
                        SetAndDelete(x, 0, val);
                    } else if (_arr[x][_max - 1] == _size.ToString())
                    {
                        DoForSize(idx, 1);
                    }
                    break;
                case Vector.Right:
                    if (_arr[x][0] == _size.ToString())
                    {
                        SetAndDelete(x, _max, val);
                    } else if (_arr[x][1] == _size.ToString())
                    {
                        if (n == _size - 1)
                            DoForSize(idx, 1);
                    }
                    break;
                case Vector.Top:
                    if (_arr[_max][y] == _size.ToString())
                    {
                        if (n == _size - 1)
                            SetAndDelete(0, y, val);
                    } else if (_arr[_max - 1][y] == _size.ToString())
                    {
                        if (n == _size - 1)
                            DoForSize(idx, 1);

                    }
                    break;
                case Vector.Bottom:
                    if (_arr[0][y] == _size.ToString())
                    {
                        SetAndDelete(_max, y, val);
                    } else if (_arr[1][y] == _size.ToString())
                    {
                        if (n == _size - 1)
                            DoForSize(idx, 1);
                    }
                    break;
            }

            Console.WriteLine($"Index {idx}, Vector {vector}");
            Display();


        }


        /* private void DoFullCheck(int idx, int n)
         {
             (int x, int y, Vector vector) = GetCoords(idx);
             var must = Enumerable.Range(_size - (n - 2), n - 1);
             var mustRes = string.Join("", must);
             //var mustRes = _size.ToString();
             var availabel = _size - n;

             for (int i = availabel; i < _size; i++)
             {
                 switch (vector)
                 {
                     case Vector.Left:
                         foreach (var item in must)
                         {
                             if(_arr[x][_max] == _size.ToString())
                             {
                                 mustRes = string.Empty;
                             }

                             if (_arr[x][i] == item.ToString())
                             {
                                 mustRes = mustRes.Replace(item.ToString(), "");
                             }
                         }

                         break;
                     case Vector.Right:
                         foreach (var item in must)
                         {
                             if(_arr[x][0] == _size.ToString())
                             {
                                 mustRes = string.Empty;
                             }

                             if (_arr[_max][_max - i] == item.ToString())
                             {
                                 mustRes = mustRes.Replace(item.ToString(), "");
                             }
                         }
                         break;
                     case Vector.Top:
                         foreach (var item in must)
                         {
                             if(_arr[_max][y] == _size.ToString())
                             {
                                 mustRes = string.Empty;
                             }

                             if (_arr[i][y] == item.ToString())
                             {
                                 mustRes = mustRes.Replace(item.ToString(), "");
                             }
                         }
                         break;
                     case Vector.Bottom:
                         foreach (var item in must)
                         {
                             if(_arr[0][y] == _size.ToString())
                             {
                                 mustRes = string.Empty;
                             }

                             if (_arr[_max - i][_max] == item.ToString())
                             {
                                 mustRes = mustRes.Replace(item.ToString(), "");
                             }
                         }
                         break;
                     default:
                         break;
                 }
             }

             if (string.IsNullOrEmpty(mustRes))
             {
                 var newval = _arr[x][y].LastOrDefault() - '0';
                 SetAndDelete(x, y, newval);
             }
         }
 */
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

        private void SetAndDelete(int i, int j, int item)
        {
            Set(i, j, item);
            DeleteInLineAndRow(i, j, item);
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

        private void Set(int x, int y, int n)
        {
            if (_arr[x][y].Length > 1)
            {
                _arr[x][y] = n.ToString();
                _set = true;
            }
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
