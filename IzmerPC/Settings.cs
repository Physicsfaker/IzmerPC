using System;
using System.IO;
using System.Text;

namespace IzmerPC
{
    public static class Settings
    {

        private static string com;
        private static int baudRate;

        public static string Com { get => com; set { com = value; SaveChanges(); } }
        public static int BaudRate { get => baudRate; set { baudRate = value; SaveChanges(); } }

        static Settings()
        {
            try
            {
                using (FileStream fstream = new FileStream($"settings.dat", FileMode.CreateNew)) { }
                    com = "COM1";
                    baudRate = 115200;
                    SaveChanges();
            }
            catch (IOException)
            {
                using (StreamReader sr = new StreamReader($"settings.dat", Encoding.Default))
                {
                    com = sr.ReadLine();
                    baudRate = Convert.ToInt32(sr.ReadLine());
                }
            }
        }

        private static void SaveChanges()
        {
            using (StreamWriter sw = new StreamWriter($"settings.dat", false, Encoding.Default))
            {
                sw.WriteLine(Com);
                sw.WriteLine(BaudRate.ToString());
            }
        }
    }
}
