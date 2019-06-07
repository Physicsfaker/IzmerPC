using System;
using System.IO;
using System.Text;

namespace IzmerPC
{
    public static class Settings
    {

        private static string com;
        private static int baudRate;
        private static bool oldversion;
        private static bool logining;

        public static string Com { get => com; set { com = value; SaveChanges(); } }
        public static int BaudRate { get => baudRate; set { baudRate = value; SaveChanges(); } }
        public static bool Oldversion { get => oldversion; set { oldversion = value; SaveChanges(); } }
        public static bool Logining { get => logining; set { logining = value; SaveChanges(); } }

        static Settings()
        {
            try
            {
                using (FileStream fstream = new FileStream($"settings.dat", FileMode.CreateNew)) { }
                    com = "COM1";
                    baudRate = 115200;
                    oldversion = false;
                    logining = false;
                    SaveChanges();
            }
            catch (IOException)
            {
                using (StreamReader sr = new StreamReader($"settings.dat", Encoding.Default))
                {
                    com = sr.ReadLine();
                    baudRate = Convert.ToInt32(sr.ReadLine());
                    oldversion = Convert.ToBoolean(sr.ReadLine());
                    logining = Convert.ToBoolean(sr.ReadLine());
                }
            }
        }

        private static void SaveChanges()
        {
            using (StreamWriter sw = new StreamWriter($"settings.dat", false, Encoding.Default))
            {
                sw.WriteLine(Com);
                sw.WriteLine(BaudRate.ToString());
                sw.WriteLine(Oldversion);
                sw.WriteLine(Logining);
            }
        }
    }
}
