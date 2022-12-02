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
            Population = GetPopulation(@textBox1.Text);
            BirthProbabilities = GetBirthProbabilities(@"C:\Temp\születés.csv");
            DeathProbabilities = GetDeathProbabilities(@"C:\Temp\halál.csv"); 
        }

        private void Simulation()
        {
            for (int year = 2005; year <= numericUpDown1.Value; year++)
            {
                for (int i = 0; i < Population.Count; i++)
                {
                    SimSTep(year, Population[i]);
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

        private void button2_Click(object sender, EventArgs e)
        {
            NBRoffemale.Clear();
            NBRofmale.Clear();
            richTextBox1.Clear();
            Simulation();
        }

        List<Person> NBRofmale = new List<Person>();
        List<Person> NBRoffemale = new List<Person>();

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.InitialDirectory = Application.StartupPath; // elérési útvonal ahonnan futtattuk az alkalmazást
            ofd.Filter = "vesszővel tagolt szöveg (*.csv) | *.csv";
            ofd.DefaultExt = "csv"; // alap felajánlott mentési kiterjesztés
            ofd.AddExtension = true; // kiterjesztést ad, ha elfelejtünk

            if (ofd.ShowDialog() != DialogResult.OK) return;

            StreamReader sr = new StreamReader(ofd.FileName, Encoding.Default);
            while (!sr.EndOfStream)
            {
                string[] sor = sr.ReadLine().Split(';'); // darabolás ;-nél
                Person p = new Person();
                p.BirthYear = int.Parse(sor[0]);
                p.Gender = (Gender)Enum.Parse(typeof(Gender), sor[1]);
            }

                /*s.Neptun = sor[0];
                s.Nev = sor[1];

                if (string.IsNullOrEmpty(sor[2]))
                {
                    s.BirthDate = Convert.ToDateTime(sor[2]);
                }
                // nem biztos, hogy sikerül a konvertáslás a datetime-nál a lehetséges null érték miatt

                if (string.IsNullOrEmpty(sor[3]))
                {
                    s.AvarageGrade = Convert.ToDecimal(sor[3]);
                }

                s.IsActive = bool.Parse(sor[4]);

                students.Add(s);*/
            }

        private void DisplayResult()
        {

        }
    }
}
