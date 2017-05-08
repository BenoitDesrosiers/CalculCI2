using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculCI
{
    class Program
    {
        private static IDictionary<string, Cours> CursusA = new Dictionary<string, Cours>();
        private static IDictionary<string, Cours> CursusH = new Dictionary<string, Cours>();

        private static IDictionary<string, Liberation> LiberationsA = new Dictionary<string, Liberation>();
        private static IDictionary<string, Liberation> LiberationsB = new Dictionary<string, Liberation>();

        private static IDictionary<string, Prof> Enseignants = new Dictionary<string, Prof>();

        const int CIMaxSession = 55;
        const int CIMaxAnnuel = 85;

        const double AllocAut = 6.4237;


        static void Main(string[] args)
        {
            SeedCours();
            SeedLiberations();
            SeedProf();
            Console.WriteLine("CI et Allocation Par Cours");
            double ci =0;

            foreach(Cours c in CursusA.Values)
            {
                List<Allocation> test = new List<Allocation>() {c };

                ci = CalculLaCI(test);
                Console.WriteLine("{0 } résultat  CI {1}  allocation {2}", c.Nom, ci, ci/40);

            }

            // trouve les allocations non pré-allouées 

            List<Allocation> allocLibre = new List<Allocation>();
            foreach(Cours c in CursusA.Values)
            {
                if ( c.estAssigne() )
                {
                    allocLibre.Add(c);
                }
            }

            foreach(Liberation l in LiberationsA.Values)
            {
                if ( l.estAssigne())
                {
                    allocLibre.Add(l);
                }
            }

            // calcule toutes les combinaisons gagnantes pour chacun des profs.  

            foreach( Prof p in Enseignants.Values)
            {
                List<Allocation> combinaison ;

                //reset la liste pour est les cours PreAlloues
                combinaison = new List<Allocation>(p.AllocationsPreAlloues);
                     
            }

            /*
            for (int i = 0; i <= 1000000; i++)
                       {
                           List<Allocation> test = new List<Allocation>() { LiberationsA["Syndicat2"], CursusA["420-SD2-DM-2"], CursusA["420-SE2-DM"] };
                           ci = CalculLaCI(test);

                       }
                       Console.WriteLine("Louis résultat {0}", ci);

                        test = new List<Allocation>() { LiberationsA["Syndicat2"], CursusA["420-BD1-DM"], CursusA["420-UC1-DM"] };
                        ci = CalculLaCI(test);

                       Console.WriteLine("Guy résultat {0}", ci);

                       test = new List<Allocation>() { LiberationsA["Coord Programme"], LiberationsA["CATI"], LiberationsA["Hololens"],  CursusA["420-CM1-DM"], CursusA["420-FT1-DM"], CursusA["420-PR3-DM"] };
                       ci = CalculLaCI(test);

                       Console.WriteLine("Stephane résultat {0}", ci);


                       test = new List<Allocation>() { LiberationsA["Coord Départementale"],CursusA["420-DM1-DM"] };
                       ci = CalculLaCI(test);

                       Console.WriteLine("Jonathan résultat {0}", ci);
                       */
            Console.ReadLine();
        }

        private static double CalculLaCI(IList<Allocation> allocations)
        {
            int nbrCours = 0;
            foreach (Allocation alloc in allocations)
            { nbrCours += alloc.ComptePourNbrCours() ? 1 : 0; }

            double ci = 0;
            foreach (Allocation alloc in allocations)
            {
                ci += alloc.CalculCI(nbrCours);
            }

            return ci;
        }

        private static void SeedCours()
        {
            CursusA.Add("204-CJV-DM", new Cours("204-CJV-DM", 25, 3));
            CursusA.Add("420-BD1-DM", new Cours("420-BD1-DM", 26, 3));
            CursusA.Add("420-CM1-DM", new Cours("420-CM1-DM", 20, 3));
            CursusA.Add("420-CN2-DM", new Cours("420-CN2-DM", 13, 4));
            CursusA.Add("420-DD1-DM", new Cours("420-DD1-DM", 12, 4));
            CursusA.Add("420-DM1-DM", new Cours("420-DM1-DM", 13, 10));
            CursusA.Add("420-FT1-DM", new Cours("420-FT1-DM", 20, 3));
            CursusA.Add("420-PR3-DM", new Cours("420-PR3-DM", 26, 3));
            CursusA.Add("420-PRA-DM", new Cours("420-PRA-DM", 20, 3));
            CursusA.Add("420-REB-DM", new Cours("420-REB-DM", 13, 6));
            CursusA.Add("420-SD2-DM-1", new Cours("420-SD2-DM-1", 16, 5, 2));
            CursusA.Add("420-SD2-DM-2", new Cours("420-SD2-DM-2", 15, 5, 2));
            CursusA.Add("420-SE1-DM", new Cours("420-SE1-DM", 20, 3));
            CursusA.Add("420-SE2-DM", new Cours("420-SE2-DM", 26, 4));
            CursusA.Add("420-UC1-DM", new Cours("420-UC1-DM", 26, 3));

        }

        private static void SeedLiberations()
        {
            LiberationsA.Add("Coord Programme", new Liberation("Coord Programme", 0.1));
            LiberationsA.Add("Coord Départementale", new Liberation("Coord Départementale", 0.3631));
            LiberationsA.Add("CATI", new Liberation("CATI", 0.1));
            LiberationsA.Add("Syndicat1", new Liberation("Syndicat1", 0.5));
            LiberationsA.Add("Syndicat2", new Liberation("Syndicat2", 0.5));
            LiberationsA.Add("Hololens", new Liberation("Hololens", 0.1));
        }

        private static void SeedProf()
        {
            List<string> lesProfs = new List<string>() { "Bernard", "Louis", "Jonathan", "Benoit", "Stéphane", "Daniel", "Nathalie" };

            foreach (string nomProf in lesProfs)
            {
                Prof unProf = new Prof(nomProf, true);
                Enseignants.Add(nomProf, unProf);
            }
            Enseignants["Nathalie"].TempsPlein = false;

            //unProf.AjouteAllocationDesirees(Cursus[])  <<<< à ajouter plus tard, probablement que ca serait mieux d'avoir les cours non-désirés

            Enseignants["Bernard"].AjouteAllocation(LiberationsA["Syndicat2"]);
            Enseignants["Louis"].AjouteAllocation(LiberationsA["Syndicat1"]);

        }
    }
}
