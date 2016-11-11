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
using DeepCloneFactory;

namespace HD_DeepCopy
{
    [Serializable]
    public class Lecture : ICloneable
    {
        public Lecture()
        {
            _lecNo = 20;
            //_name = "Andrew Chen";
            _school = "Swinburne";
            _lectureOffice = new Colleagues();
            _unit = "Computer Science";
            _studentIDs = new int[] { 100, 101, 102, 103 };
            _history.History_1 = "Researcher in Monash University: 2000-2002";
            _history.History_2 = "Lecture in RMIT: 2002-2004";
            _level = Level.Beginning;
            //_lectureOffice.lec.Name = "Lucy Lee";
        }

        private int _lecNo;
        private Colleagues _lectureOffice;
        private string _unit = String.Empty;
        private string _school = String.Empty;
        private string _name = String.Empty;
        private int[] _studentIDs;
        private History _history;
        private Level _level;
        private BestFriend _bestFriend;

        public int LecNo
        {
            get
            {
                return _lecNo;
            }
            set
            {
                _lecNo = value;
            }
        }

        public Colleagues LectureOffice
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

        // Attach the CloneAllowAttribute to this property.
        // Note the use of the 'cs' enum and 'pt' string arguments to the CloneAllowAttribute.
        [CloneAllow(CloneSwitch.NOT_ALLOW)]
        [CloneAllow((CloneSwitch.ALLOW), "Name")]
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

        public BestFriend BestFriend
        {
            get
            {
                return _bestFriend;
            }

            set
            {
                _bestFriend = value;
            }
        }

        public Lecture MemberWiseShallowClone()
        {
            return (Lecture)this.MemberwiseClone();
        }

        public Object Clone()
        {
            Lecture lec = MemberWiseShallowClone();
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

        //public Lecture DeepClone_BinSerFile(string fileName)
        //{
        //    Stream newstream = File.Open("lecture.bin", FileMode.Create);
        //    BinaryFormatter newbinFormatter = new BinaryFormatter();
        //    newbinFormatter.Serialize(newstream, this);
        //    newstream.Close();
        //    newstream = File.Open("lecture.bin", FileMode.Open);
        //    Lecture lec = (Lecture)newbinFormatter.Deserialize(newstream);
        //    newstream.Close();
        //    return lec;
        //}

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
            Console.WriteLine("     Name: " + Name);
            Console.WriteLine("     School: " + _school);
            Console.WriteLine("     Unit Allocated: " + _unit);
            Console.WriteLine("     Lecture ID: " + LecNo.ToString());
            //Console.WriteLine("Lecture Office Address: " + LectureOffice.officeNo_1);
            foreach (int i in StudentIDs)
                Console.WriteLine("     In Room Student ID: " + i);
            Console.WriteLine("     Experience_1: " + History.History_1);
            Console.WriteLine("     Experience_2: " + History.History_2);
            Console.WriteLine("     Academic Level: " + Level);
            foreach (Lecture l in LectureOffice.LectureList)
            Console.WriteLine("     Office Partenter: " + l.Name);
            Console.WriteLine("     Best Friend: " + BestFriend.Lecture.Name);
            Console.WriteLine("     Best Friend Nick Name: " + BestFriend.Nicname);
            Console.WriteLine();
            Console.WriteLine();
        }

    }

    [Serializable]
    public class Colleagues
    {
        private List<Lecture> _lectureList = new List<Lecture>();
        public List<Lecture> LectureList
        {
            get
            {
                return _lectureList;
            }
            set
            {
                _lectureList = value;
            }
        }
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

    [Serializable]
    public class BestFriend
    {
        public string Nicname { get; set; }
        private Lecture _lecture = new Lecture();

        public Lecture Lecture
        {
            get
            {
                return _lecture;
            }
            set
            {
                _lecture = value;
            }
        }
    }
}