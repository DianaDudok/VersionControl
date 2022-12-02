using _9.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace _9
{
    public partial class Form1 : Form
    {
        List<Person> Population = new List<Person>();
        List<BirthProbability> BirthProbabilities = new List<BirthProbability>();
        List<DeathProbability> DeathProbabilities = new List<DeathProbability>();
        public Form1()
        {
            InitializeComponent();
            Population=GetPopulation(@"C:\Temp\nép.csv");
            BirthProbabilities = GetBirthProbabilities(@"C:\Temp\születés.csv");
            DeathProbabilities = GetDeathProbabilities(@"C:\Temp\halál.csv");

            for (int year = 2005; year <=2024; year++)
            {
                for (int i = 0; i < Population.Count; i++)
                {
                    SimSTep(year,Population[i]);
                }
                int nbrOfMales = (from x in Population
                                  where x.Gender == Gender.Male && x.IsAlive
                                  select x).Count(); 
                int nbrOfFemales = (from x in Population
                                  where x.Gender == Gender.Female && x.IsAlive
                                  select x).Count();
                Console.WriteLine(string.Format("Év: {0} Fiúk:{1} Lányok:{2}", year, nbrOfMales, nbrOfFemales));
            }
        }

        public List<Person> GetPopulation(string csvpath)
        {
            List<Person> population = new List<Person>();

            using(StreamReader sr =new StreamReader(csvpath,Encoding.Default))
            {
                while(!sr.EndOfStream)
                {
                    var line = sr.ReadLine().Split(';');
                    population.Add(new Person()
                    {
                        BirthYear = int.Parse(line[0]),
                        Gender = (Gender)Enum.Parse(typeof(Gender), line[1]),
                        NbrOfChildren = int.Parse(line[2])
                    });
                }
            }
            return population;
        }
        public List<BirthProbability> GetBirthProbabilities(string csvpath)
        {
            List<BirthProbability> birthProbabilities = new List<BirthProbability>();

            using (StreamReader sr = new StreamReader(csvpath, Encoding.Default))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine().Split(';');
                    birthProbabilities.Add(new BirthProbability()
                    {
                        Kor = int.Parse(line[0]),
                        NbrOfChildren = int.Parse(line[1]),
                        BProbability=double.Parse(line[2])
                    });
                }
            }
            return birthProbabilities;
        }
        public List<DeathProbability> GetDeathProbabilities(string csvpath)
        {
            List<DeathProbability> deathProbabilities = new List<DeathProbability>();

            using (StreamReader sr = new StreamReader(csvpath, Encoding.Default))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine().Split(';');
                    deathProbabilities.Add(new DeathProbability()
                    {
                        Gender = (Gender)Enum.Parse(typeof(Gender), line[0]),
                        Kor = int.Parse(line[1]),
                        DProbability = double.Parse(line[2])
                    });
                }
            }
            return deathProbabilities;
        }

        Random rng = new Random(1234);

        private void SimSTep(int year, Person person)
        {
            if (!person.IsAlive) return;

            byte age = (byte)(year - person.BirthYear);

            double pDeath = (from x in DeathProbabilities
                             where x.Gender == person.Gender && x.Kor == age
                             select x.DProbability).FirstOrDefault();

            if (rng.NextDouble() <= pDeath)
                person.IsAlive = false;

            if (person.IsAlive && person.Gender == Gender.Female)
            {
                double pBirth = (from x in BirthProbabilities
                                 where x.Kor == age
                                 select x.BProbability).FirstOrDefault();

                if (rng.NextDouble() <= pBirth)
                {
                    Person ujszulott = new Person();
                    ujszulott.BirthYear = year;
                    ujszulott.NbrOfChildren = 0;
                    ujszulott.Gender = (Gender)(rng.Next(1, 3));
                    Population.Add(ujszulott);
                }
            }
        }
    }
}
