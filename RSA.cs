using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LB1
{
    class RSA
    {
        public static BigInt module;
        public static BigInt Decrypt(BigInt a, BigInt key) => BigInt.Pow(a, key) % module;
        public static BigInt Encrypt(BigInt a, BigInt key) => BigInt.Pow(a, key) % module;

        public static List<BigInt> CreateKeys(BigInt p, BigInt q)
        {
            module = p * q;
            var func = (p - new BigInt(1)) * (q - new BigInt(1));
            var d = Calculate_d(Convert.ToInt32(func.ToString())) + func;
            var e = Calculate_e(Convert.ToInt32(d.ToString()), Convert.ToInt32(func.ToString()));
            var result = new List<BigInt>(){ e, d };
            return result;
        }
        private static BigInt Calculate_d(int func)
        {
            var d = func - 1;
            for (var i = 2; i <= func; i++)
                if ((func % i == 0) && (d % i == 0))
                {
                    d--;
                    i = 1;
                }
            return new BigInt(d);
        }
        private static BigInt Calculate_e(int d, int m)
        {
            var e = 10;
            while (true)
            {
                if ((e * d) % m == 1) break;
                else e++;
            }
            return new BigInt(e);
        }
    }

}
