using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeepCloneFactory;

namespace XmlDeepCopy
{
    public class Program
    {


        static void Main(string[] args)
        {
            Lecture lec = new Lecture();
            Console.WriteLine("Shallow Cloning using MemberwiseClone");
            Lecture lecMemberWiseClone = lec.MemberWiseShallowClone();
            lecMemberWiseClone.Display();
            Console.WriteLine(String.Empty);

            Console.WriteLine("Shallow Cloning using ICloneable interface");
            Lecture lecClone = (Lecture)lec.Clone();
            lecClone.Display();
            Console.WriteLine(String.Empty);

            Console.WriteLine("Shallow Cloning using reflection");
            Lecture newlec = (Lecture)DeepClone_Reflection.DeepClone(lec);
            newlec.Display();


            Console.WriteLine("Deep Cloning using Memory stream");
            Lecture empDeepClone_1 = (Lecture)lec.DeepClone_BinSer();
            empDeepClone_1.Display();
            Console.WriteLine(String.Empty);

            Console.WriteLine("Deep Cloning using File stream");
            Lecture empDeepClone_2 = (Lecture)lec.DeepClone_BinSerFile("C:\temp\file.txt");
            empDeepClone_2.Display();
            Console.WriteLine(String.Empty);

            Console.WriteLine("Deep Cloning using Xml Serializer");
            Lecture empDeepClone_3 = (Lecture)lec.DeepClone_XmlSer();
            empDeepClone_3.Display();
            Console.WriteLine(String.Empty);

            //Change the value of a field           
            Console.WriteLine("Address value of the main class changed");
            lec.LectureOffice.officeNo_1 = "Address changed";

            //print shallow copiues
            Console.WriteLine("Re-Printing values of the MemberwiseClone Copy");
            lecMemberWiseClone.Display();
            Console.WriteLine(String.Empty);

            Console.WriteLine("Re-Printing values of the Deep Clone Copy");
            empDeepClone_1.Display();

            Console.ReadLine();
        }
    }
}