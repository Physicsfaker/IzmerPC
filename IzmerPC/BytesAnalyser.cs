using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/*
 * Запросы:                                          Ответы:
 * AB 10 - запрет на передачу 2F                     -
 * AB 00 - типа полнаяя синхронизация                -
 * AB 03 - чтение состояния и клапанов      
 * AB 04 - изменениережимов и клапанов               -
 * AB 05 - режим работы ти и состояние электрометра  -
 * AB 06 - чтение давлений                           -
 * AB 07 - посылка на 320 байт                       320 байт
 */
namespace IzmerPC
{
    public class BytesAnalyser : MainWindow
    {

        static public void Manager(byte[] reciveBytes)
        {
            if (reciveBytes.Length < 100)
            {

            }
            else
            {

            }
        }



    }
}
