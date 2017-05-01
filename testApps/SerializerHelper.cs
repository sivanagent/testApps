using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace testApps
{
    [Serializable()]
    public class Garage
    {
        private Vehicle _MyVehicle1;
        private Vehicle _MyVehicle2;

        public List<Vehicle> oListOfVehicles = new List<Vehicle> ();

        public Garage()
        {
        }
        public string GarageOwner { get; set; }
        public Vehicle MyVehicle1
        {
            get { return _MyVehicle1; }
            set { _MyVehicle1 = value; }
        }

        public Vehicle MyVehicle2
        {
            get { return _MyVehicle2; }
            set { _MyVehicle2 = value; }
        }

    }

    [XmlInclude(typeof(Car))]
    [XmlInclude(typeof(Boat))]
    [XmlInclude(typeof(Motorcycle))]
    [XmlInclude(typeof(Motorhome))]
    public abstract class Vehicle
    {
        public string VehicleType { get; set; }
        public int VehicleNumber { get; set; }
    }
    public class Car : Vehicle
    {
        public int Doors { get; set; }
    }
    public class Boat : Vehicle
    {
        public int Engines { get; set; }
    }
    public class Motorcycle : Vehicle
    {
        public int Wheels { get; set; }
    }
    public class Motorhome : Vehicle
    {
        public int Length { get; set; }
    }

    class Serializer
    {
        static string _StartupPath;
        static string _StartupFile;
        static string _StartupXML; 

        public  Serializer() {  }

        public Serializer(string t_StartupPath, string t_StartupFile)
        {
            _StartupPath = t_StartupPath;
            _StartupFile = t_StartupFile;
            _StartupXML = _StartupPath + _StartupFile;
        }


        //static void Main(string[] args)
        public void testSerialize()
        {
            Console.Write("Press w for write. Press r for read:");
            ConsoleKeyInfo cki = Console.ReadKey(true);
            Console.WriteLine("Pressed: " + cki.KeyChar.ToString());
            if (cki.KeyChar.ToString() == "w")
            {
                Garage MyGarage = new Garage();
                MyGarage.GarageOwner = "xyzJillJohn";
                Car c = new Car();
                c.VehicleType = "Lexus";
                c.VehicleNumber = 1234;
                c.Doors = 4;
                MyGarage.MyVehicle1 = c;

                Motorhome m = new Motorhome();
                m.VehicleType = "abcdMotorHome";
                m.VehicleNumber = 99991234;
                m.Length = 44;
                MyGarage.MyVehicle2 = m;

                MyGarage.oListOfVehicles.Add(c);
                MyGarage.oListOfVehicles.Add(c);
                MyGarage.oListOfVehicles.Add(c);
                MyGarage.oListOfVehicles.Add(m);

                WriteGarageXML(MyGarage);
                Console.WriteLine("Serialized");
            }
            else if (cki.KeyChar.ToString() == "r")
            {
                Garage MyGarage = ReadGarageXML();
                Console.WriteLine("Deserialized Garage owned by " + MyGarage.GarageOwner);
            }
            Console.ReadKey();
        }
        public static void WriteGarageXML(Garage pInstance)
        {
            XmlSerializer writer = new XmlSerializer(typeof(Garage));
            using (FileStream file = File.OpenWrite(_StartupXML))
            {
                writer.Serialize(file, pInstance);
            }

            /******
            XmlSerializer x = new XmlSerializer(typeof(ClassOfYOurs));
            System.IO.Stream s;

            var xmlwriter = System.Xml.XmlWriter.Create("C:\\xmlArchive.txt"); 
            x.Serialize(xmlwriter, ClassOfYOurs);
            ****/
        }
        public static Garage ReadGarageXML()
        {
            XmlSerializer reader = new XmlSerializer(typeof(Garage));
            using (FileStream input = File.OpenRead(_StartupXML))
            {
                return reader.Deserialize(input) as Garage;
            }
        }
    }
}
