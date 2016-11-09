using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using DeepCloneFactory;
using System.Diagnostics;

namespace HD_DeepCopy
{
    class Program
    {
        static void Main(string[] args)
        {
           DeepCloneMethodsDemo();
        }

        private static void WriteLog(string msg)
        {
            Console.WriteLine(msg);
        }

        public static void DeepCloneMethodsDemo()
        {
            Lecture lec4 = new Lecture();
            lec4.Name = "Lucy Lee";

            Lecture lec5 = new Lecture();
            lec5.Name = "Tom Cruise";

            Lecture lec1 = new Lecture();
            lec1.Name = "Andrew Chen";
            Console.WriteLine(lec1.LectureOffice.LectureList.Count);
            lec1.LectureOffice.LectureList.Add(lec4);
            Console.WriteLine(lec1.LectureOffice.LectureList.Count);

            lec1.BestFriend = new BestFriend();
            lec1.BestFriend.Lecture = new Lecture();
            lec1.BestFriend.Nicname = "batman";

            Lecture lec2 = (Lecture)DeepClone_Reflection.DeepClone(lec1);
            Lecture lec3 = lec1.DeepClone_XmlSer();

            //lec2.LectureOffice.LectureList[0].Name = "Ming Liu";
            lec2.BestFriend.Lecture.Name = "Jason But";
           //lec3.LectureOffice.LectureList[0].Name = "Jason Bourne";
            //lec3.BestFriend.Lecture.Name = "Jimmy Camory";

            //lec1.Display();
            lec2.Display();
            //lec3.Display();
            //Console.WriteLine(lec1.LectureOffice.LectureList[0].Name);
            //Console.WriteLine(lec2.LectureOffice.LectureList[0].Name);
            //Console.WriteLine(lec3.LectureOffice.LectureList[0].Name);

            var a = Stopwatch.StartNew();
            var totalCount = 1000;
            for (int i = 0; i < totalCount; i++)
            {
                Lecture lec6 = (Lecture)DeepClone_Reflection.DeepClone(lec1);
            }
            a.Stop();
            var aa = a.ElapsedMilliseconds;
            Console.WriteLine("Reflection : " + aa.ToString());


            a = Stopwatch.StartNew();
            for (int i = 0; i < totalCount; i++)
            {
                Lecture lec7 = lec1.DeepClone_XmlSer();
            }
            a.Stop();
            var cc = a.ElapsedMilliseconds;
            Console.WriteLine("Xml Serialize : " + cc.ToString());

            a = Stopwatch.StartNew();
            for (int i = 0; i < totalCount; i++)
            {
                Lecture lec8 = lec1.DeepClone_BinSer();
            }
            a.Stop();
            var dd = a.ElapsedMilliseconds;
            Console.WriteLine("Binary Serialize : " + dd.ToString());

            Lecture lec9 = (Lecture)DeepClone_Reflection.DeepClone(lec1);

            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, lec1);

            BinaryFormatter bf1 = new BinaryFormatter();
            MemoryStream ms1 = new MemoryStream();
            bf1.Serialize(ms1, lec9);
            byte[] b1 = ms1.ToArray();
            byte[] bt = ms.ToArray();
            if (b1.SequenceEqual(bt))
            {
                Console.WriteLine("Values Equal");
            }
            else { Console.WriteLine("Values Not Equal"); }
            Console.WriteLine(ReferenceEquals(lec1, lec9));
            Console.ReadKey();
        }
    }
}
