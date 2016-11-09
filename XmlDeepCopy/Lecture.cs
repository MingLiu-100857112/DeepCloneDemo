using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace XmlDeepCopy
{
    [Serializable]
    public class Lecture : ICloneable
    {
        public Lecture()
        {
            _lecNo = 20;
            _name = "Andrew Chen";
            _school = "Swinburne";
            _lectureOffice = new LectureOffice();
            _lectureOffice.officeNo_1 = "EA302";
            _unit = "Computer Science";
            _studentIDs = new int[] { 100, 101, 102, 103 };
            _history.History_1 = "Researcher in Monash University: 2000-2002";
            _history.History_2 = "Lecture in RMIT: 2002-2004";
            _level = Level.Beginning;
            //_lectureOffice.lec.Name = "Lucy Lee";
        }

        private int _lecNo;
        private LectureOffice _lectureOffice;
        private string _unit = String.Empty;
        private string _school = String.Empty;
        private string _name = String.Empty;
        private int[] _studentIDs;
        private History _history;
        private Level _level;

        public int LecNo
        {
            get { return _lecNo; }
        }

        public LectureOffice LectureOffice
        {
            get { return _lectureOffice; }
            set { _lectureOffice = value; }
        }

        public int[] StudentIDs
        {
            get
            {
                return _studentIDs;
            }

            set
            {
                _studentIDs = value;
            }
        }

        public History History
        {
            get
            {
                return _history;
            }

            set
            {
                _history = value;
            }
        }

        public Level Level
        {
            get
            {
                return _level;
            }

            set
            {
                _level = value;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value;
            }
        }

        public Lecture MemberWiseShallowClone()
        {
            return (Lecture)this.MemberwiseClone();
        }

        public Object Clone()
        {
            Lecture lec = MemberWiseShallowClone();
            lec._school = String.Empty;
            return lec;
        }

        public Lecture DeepClone_BinSer()
        {
            MemoryStream newms = new MemoryStream();
            BinaryFormatter newbf = new BinaryFormatter();
            newbf.Serialize(newms, this);
            newms.Position = 0;
            object obj = newbf.Deserialize(newms);
            newms.Close();
            return (Lecture)obj;
        }

        public Lecture DeepClone_BinSerFile(string fileName)
        {
            Stream newstream = File.Open("lecture.bin", FileMode.Create);
            BinaryFormatter newbinFormatter = new BinaryFormatter();
            newbinFormatter.Serialize(newstream, this);
            newstream.Close();
            newstream = File.Open("lecture.bin", FileMode.Open);
            Lecture lec = (Lecture)newbinFormatter.Deserialize(newstream);
            newstream.Close();
            return lec;
        }

        public Lecture DeepClone_XmlSer()
        {
            using (var newms = new MemoryStream())
            {
                XmlSerializer newxs = new XmlSerializer(typeof(Lecture));
                newxs.Serialize(newms, this);
                newms.Position = 0;
                return (Lecture)newxs.Deserialize(newms);
            }
        }

        public void Display()
        {
            Console.WriteLine("Name: " + Name);
            Console.WriteLine("School: " + _school);
            Console.WriteLine("Unit Allocated: " + _unit);
            Console.WriteLine("Lecture ID: " + LecNo.ToString());
            Console.WriteLine("Lecture Office Address: " + _lectureOffice.officeNo_1);
            foreach(int i in StudentIDs)
            Console.WriteLine("In Room Student ID: " + i);
            Console.WriteLine("Experience_1: " + History.History_1);
            Console.WriteLine("Experience_2: " + History.History_2);
            Console.WriteLine("Academic Level: " + Level);
            //Console.WriteLine("Office Partenter: " + LectureOffice.lec.Name);
        }

    }

    [Serializable]
    public class LectureOffice
    {
        public Lecture lec;
        public string officeNo_1;
        public string officeNo_2;
    }

    [Serializable]
    public struct History
    {
        public string History_1;
        public string History_2;
    }

    public enum Level
    {
        Beginning = 0,
        Experienced = 1
    }
}