using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;


/*
 * Запросы:                                          Ответы:
 * AB 10 - запрет на передачу 2F                     -
 * AB 00 - типа полнаяя синхронизация                -
 * 
 * AB 04 - изменение режимов и клапанов               -
 * AB 06 - чтение давлений                           -
 * AB 03 - чтение состояния и клапанов      
 * AB 07 - посылка на 320 байт                       320 байт
 * AB 05 - чтение режима работы ти и состояние электрометра  -
 */
namespace IzmerPC
{
    public class BytesOperations
    {
        public static void State_valve()            => ComPort.Write(new byte[] { 0xAB, 0x03 });
        public static void State_pressure()         => ComPort.Write(new byte[] { 0xAB, 0x06 });
        public static void State_full()             => ComPort.Write(new byte[] { 0xAB, 0x07 });
        public static void State_electrometer()     => ComPort.Write(new byte[] { 0xAB, 0x05 });
        public static void Trash_off()              => ComPort.Write(new byte[] { 0xAB, 0x10 });
    }
}
