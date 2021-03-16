using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace LB1
{
    public class BigInt
    {
        private readonly List<byte> numbers;
        private char sign;

        private BigInt(char s, List<byte> list)
        {
            sign = s;
            numbers = list;
        }

        public BigInt(string str)
        {
            sign = '+';
            numbers = new List<byte>();            
            if (str.First().ToString() == "-")
            {
                sign = '-';
                str = str.Substring(1);
            }
            str.Reverse();
            foreach (var elem in str)
                numbers.Add(Convert.ToByte(elem.ToString()));
        }

        public BigInt(int a)
        {
            numbers = new List<byte>();            
            if (a >= 0) sign = '+';
            else sign = '-';

            var list = new List<byte>();
            var num = (uint)Math.Abs(a);
            while (num > 0)
            {
                list.Add((byte)(num % 10));
                num /= 10;
            };
            numbers.AddRange(list);
        }

        public bool IsNull => numbers.Count == 0 || numbers.All(a => a == 0) || this == new BigInt(0);
        public int Count => numbers.Count;



        private byte Get(int a)
        {
            if (a < Count) return numbers[a];
            return 0;
        }
        private void Set(int a, byte b)
        {
            while (numbers.Count <= a) numbers.Add(0);
            numbers[a] = b;
        }

        public override string ToString()
        {
            RemoveZero();
            if (IsNull) return "0";
            var builder = new StringBuilder(sign == '+' ? "" : "-");

            for (var i = numbers.Count - 1; i >= 0; i--)
                builder.Append(Convert.ToString(numbers[i]));

            return builder.ToString();
        }

        private static BigInt Add(BigInt x, BigInt y)
        {
            var nums = new List<byte>();
            var length = Math.Max(x.Count, y.Count);
            var p = 0;
            for (var i = 0; i < length; i++)
            {
                var sum = (byte)(x.Get(i) + y.Get(i) + p);
                p = sum / 10;
                nums.Add((byte)(sum % 10));
            }
            if (p > 0) nums.Add((byte)p);
            return new BigInt(x.sign, nums);
        }

        private static BigInt Sub(BigInt x, BigInt y)
        {
            var nums = new List<byte>();
            var n1 = new BigInt(0);
            var n2 = new BigInt(0);
            var flag = Do(x, y, true);

            if (flag == -1)
            {
                n1 = y;
                n2 = x;
            }            
            else if(flag == 1)
            {                
                n1 = x;
                n2 = y;
            }
            else if (flag == 0) return new BigInt(0);

            var length = Math.Max(x.Count, y.Count);

            var p = 0;
            for (var i = 0; i < length; i++)
            {
                var item = n1.Get(i) - n2.Get(i) - p;
                if (item < 0)
                {
                    item += 10;
                    p = 1;
                }
                nums.Add((byte)item);
            }
            return new BigInt(n1.sign, nums);
        }

        private static BigInt Multiply(BigInt x, BigInt y)
        {
            var elem = new BigInt(0);
            for (var i = 0; i < x.Count; i++)
            {
                for (int j = 0, p = 0; (p > 0) ||(j < y.Count); j++)
                {
                    var res = elem.Get(i + j) + x.Get(i) * y.Get(j) + p;
                    elem.Set(i + j, (byte)(res % 10));
                    p = res / 10;
                }
            } 
            elem.sign = x.sign == y.sign ? '+' : '-';          
            return elem;
        }

        private static BigInt Div(BigInt x, BigInt y)
        {
            var res = Zero;            
            res.sign = x.sign == y.sign ? '+' : '-';    
            var cur = Zero;
            for (var i = x.Count - 1; i >= 0; i--)
            {
                var h = Zero;
                h.Set(i, x.Get(i));
                h.RemoveZero();
                cur += h;

                var a = 0;
                var lft = 0;
                var rght = 10;
                while (lft <= rght)
                {                    
                    var mid = (lft + rght) / 2;
                    var h1 = Zero;
                    h1.Set(i, (byte)mid);
                    h1.RemoveZero();
                    var vr = y * h1;
                    if (vr > cur) rght = mid - 1;
                    else
                    {
                        a = mid;
                        lft = mid + 1;
                    }                        
                }
                res.Set(i, (byte)(a % 10));
                var h2 = Zero;
                h2.Set(i, (byte)a);
                h2.RemoveZero();
                var p = y * h2;
                cur -= p;
            }
            res.RemoveZero();
            return res;
        }

        public static BigInt Zero = new BigInt(0);

        private static BigInt Mod(BigInt x, BigInt y)
        {
            var res = Zero;
            res.sign = x.sign == y.sign ? '+' : '-';

            for (var i = x.Count - 1; i >= 0; i--)
            {
                var h = Zero;
                h.Set(i, x.Get(i));
                h.RemoveZero();
                res += h;

                var a = 0;
                var lft = 0;
                var rght = 10;

                while (lft <= rght)
                {
                    var h1 = Zero;
                    var mid = (lft + rght) >> 1;
                    h1.Set(i, (byte)mid);
                    h1.RemoveZero();
                    var cur = y * h1;
                    if (cur <= res)
                    {
                        a = mid;
                        lft = mid + 1;
                    }
                    else
                        rght = mid - 1;
                }
                var h2 = Zero;
                h2.Set(i, (byte)a);
                h2.RemoveZero();
                res -= y * h2;
            }
            return res;
        }
      

        private static int Do(BigInt a, BigInt b, bool flag = false)
        {
            if (flag) 
            { 
                if (a.Count < b.Count) return -1;
                if (a.Count > b.Count) return 1;
                else
                {
                    var length = Math.Max(a.Count, b.Count);
                    for (var i = length; i >= 0; i--)
                    {
                        if (a.Get(i) < b.Get(i)) return -1;
                        if (a.Get(i) > b.Get(i)) return 1;
                    }
                    return 0;
                };            
            }; 
            if (a.sign < b.sign) return -1;
            if (a.sign > b.sign) return 1;
            else
            {
                if (a.Count < b.Count) return -1;
                if (a.Count > b.Count) return 1;
                else 
                {
                    var length = Math.Max(a.Count, b.Count);
                    for (var i = length; i >= 0; i--)
                    {
                        if (a.Get(i) < b.Get(i)) return -1;
                        if (a.Get(i) > b.Get(i)) return 1;
                    }
                    return 0;
                }
            }
        }        

        public static BigInt operator -(BigInt x)
        {
            x.sign = x.sign == '+' ? '-' : '+';
            return x;
        }
        public static BigInt operator +(BigInt x, BigInt y) =>
            x.sign == y.sign ? Add(x, y) : Sub(x, y);
        public static BigInt operator -(BigInt x, BigInt y) => x + -y;
        public static BigInt operator *(BigInt x, BigInt y) => Multiply(x, y);
        public static BigInt operator /(BigInt x, BigInt y) => Div(x, y);
        public static BigInt operator %(BigInt x, BigInt y) => Mod(x, y);
        public static bool operator <(BigInt x, BigInt y) => Do(x, y) < 0;
        public static bool operator >(BigInt x, BigInt y) => Do(x, y) > 0;
        public static bool operator <=(BigInt x, BigInt y) => Do(x, y) <= 0;
        public static bool operator >=(BigInt x, BigInt y) => Do(x, y) >= 0;
        public static bool operator ==(BigInt x, BigInt y) => Do(x, y) == 0;
        public static bool operator !=(BigInt x, BigInt y) => Do(x, y) != 0;
        

        
        public static BigInt Pow(BigInt value, BigInt exponent)
        {
            var original = value;
            var a = Convert.ToInt32(exponent.ToString());
            while(a-- > 1)
            {
                value = Multiply(value, original);
            }                
            return value;
        }

        private void RemoveZero()
        {
            var index = numbers.Count - 1;
            while (index >= 0 && numbers[index] == 0)
            {
                numbers.RemoveAt(index);
                index--;
            }
        }
    }
    }
