using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LB1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Введите два простых числа одинакового размера");
            var p = new BigInt(Console.ReadLine());
            var q = new BigInt(Console.ReadLine());

            var keys = RSA.CreateKeys(p, q);

            Console.WriteLine($"Открытый ключ ({keys[0]}, {RSA.module})");
            Console.WriteLine($"Закрытый ключ ({keys[1]}, {RSA.module})");

            Console.WriteLine("Введите сообщение");
            var message = Console.ReadLine();

            var encrMessage = RSA.Encrypt(new BigInt(message), keys[1]);
            Console.WriteLine("Зашифрованное  " + encrMessage);

            var decrMessage = RSA.Decrypt(encrMessage, keys[0]);
            Console.WriteLine("Расшифрованное  " + decrMessage);
                    
         }
    }
}
