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

      


        static void Main(string[] args)
        {
            SeedCours();
            SeedLiberations();
            SeedProf();
            Console.WriteLine("CI et Allocation Par Cours");
            double ci =0;

           

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

            Calculateur laChose = new Calculateur(Enseignants.Values.ToList(), allocLibre);
            laChose.Calcul();



            /*
            for (int i = 0; i <= 1000000; i++)
                       {
                           List<Allocation> test = new List<Allocation>() { LiberationsA["Syndicat2"], CursusA["420-SD2-2"], CursusA["420-SE2"] };
                           ci = CalculLaCI(test);

                       }
                       Console.WriteLine("Louis résultat {0}", ci);

                        test = new List<Allocation>() { LiberationsA["Syndicat2"], CursusA["420-BD1"], CursusA["420-UC1"] };
                        ci = CalculLaCI(test);

                       Console.WriteLine("Guy résultat {0}", ci);

                       test = new List<Allocation>() { LiberationsA["Coord Programme"], LiberationsA["CATI"], LiberationsA["Hololens"],  CursusA["420-CM1"], CursusA["420-FT1"], CursusA["420-PR3"] };
                       ci = CalculLaCI(test);

                       Console.WriteLine("Stephane résultat {0}", ci);


                       test = new List<Allocation>() { LiberationsA["Coord Départementale"],CursusA["4201"] };
                       ci = CalculLaCI(test);

                       Console.WriteLine("Jonathan résultat {0}", ci);
                       */
            Console.ReadLine();
        }

        

        private static void SeedCours()
        {
            CursusA.Add("204-CJV", new Cours("204-CJV", 25, 3));
            CursusA.Add("420-BD1", new Cours("420-BD1", 26, 3));
            CursusA.Add("420-CM1", new Cours("420-CM1", 20, 3));
            CursusA.Add("420-CN2", new Cours("420-CN2", 13, 4));
            CursusA.Add("420-DD1", new Cours("420-DD1", 12, 4));
            CursusA.Add("420-DM1", new Cours("420-DM1", 13, 10));
            CursusA.Add("420-FT1", new Cours("420-FT1", 20, 3));
            CursusA.Add("420-PR3", new Cours("420-PR3", 26, 3));
            CursusA.Add("420-PRA", new Cours("420-PRA", 20, 3));
            CursusA.Add("420-REB", new Cours("420-REB", 13, 6));
            CursusA.Add("420-SD2-1", new Cours("420-SD2-1", 16, 5, 2));
            CursusA.Add("420-SD2-2", new Cours("420-SD2-2", 15, 5, 2));
            CursusA.Add("420-SE1", new Cours("420-SE1", 20, 3));
            CursusA.Add("420-SE2", new Cours("420-SE2", 26, 4));
            CursusA.Add("420-UC1", new Cours("420-UC1", 26, 3));

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
            List<string> lesProfs = new List<string>() { "Guy", "Louis", "Jonathan", "Benoit", "Stéphane", "Daniel", "Nathalie" };

            foreach (string nomProf in lesProfs)
            {
                Prof unProf = new Prof(nomProf, true);
                Enseignants.Add(nomProf, unProf);
            }
            Enseignants["Nathalie"].TempsPlein = false;

            // Ajoute les allocations qui sont pré-assignées (lib syndicales, ou tout cours qu'on veut forcer à qqun)

            Enseignants["Guy"].AjouteAllocation(LiberationsA["Syndicat2"]);
            Enseignants["Louis"].AjouteAllocation(LiberationsA["Syndicat1"]);

            // Crée la liste des cours qu'un prof VEUT/PEUT donner. 

            Dictionary<string, List<string>> CoursVoulus = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> LiberationsVoulues = new Dictionary<string, List<string>>();

            CoursVoulus.Add("Benoit", new List<string>() { "204-CJV", "420-BD1", "420-CM1", "420-CN2", "420-DD1", "420-DM1",
                "420-FT1", "420-PR3", "420-PRA", "420-REB","420-SD2-1", "420-SD2-2", "420-SE1", "420-SE2", "420-UC1", });
            LiberationsVoulues.Add("Benoit", new List<string>(){ "Coord Programme", "Coord Départementale", "CATI", "Syndicat1", "Syndicat2", "Hololens" });

            CoursVoulus.Add("Louis", new List<string>() { "204-CJV", "420-BD1", "420-CM1", "420-CN2", "420-DD1", "420-DM1",
                "420-FT1", "420-PR3", "420-PRA", "420-REB","420-SD2-1", "420-SD2-2", "420-SE1", "420-SE2", "420-UC1", });
            LiberationsVoulues.Add("Louis", new List<string>() { "Coord Programme", "Coord Départementale", "CATI", "Syndicat1", "Syndicat2", "Hololens" });

            CoursVoulus.Add("Guy", new List<string>() { "204-CJV", "420-BD1", "420-CM1", "420-CN2", "420-DD1", "420-DM1",
                "420-FT1", "420-PR3", "420-PRA", "420-REB","420-SD2-1", "420-SD2-2", "420-SE1", "420-SE2", "420-UC1", });
            LiberationsVoulues.Add("Guy", new List<string>() { "Coord Programme", "Coord Départementale", "CATI", "Syndicat1", "Syndicat2", "Hololens" });

            CoursVoulus.Add("Jonathan", new List<string>() { "204-CJV", "420-BD1", "420-CM1", "420-CN2", "420-DD1", "420-DM1",
                "420-FT1", "420-PR3", "420-PRA", "420-REB","420-SD2-1", "420-SD2-2", "420-SE1", "420-SE2", "420-UC1", });
            LiberationsVoulues.Add("Jonathan", new List<string>() { "Coord Programme", "Coord Départementale", "CATI", "Syndicat1", "Syndicat2", "Hololens" });

            CoursVoulus.Add("Stéphane", new List<string>() { "204-CJV", "420-BD1", "420-CM1", "420-CN2", "420-DD1", "420-DM1",
                "420-FT1", "420-PR3", "420-PRA", "420-REB","420-SD2-1", "420-SD2-2", "420-SE1", "420-SE2", "420-UC1", });
            LiberationsVoulues.Add("Stéphane", new List<string>() { "Coord Programme", "Coord Départementale", "CATI", "Syndicat1", "Syndicat2", "Hololens" });

            CoursVoulus.Add("Daniel", new List<string>() { "204-CJV", "420-BD1", "420-CM1", "420-CN2", "420-DD1", "420-DM1",
                "420-FT1", "420-PR3", "420-PRA", "420-REB","420-SD2-1", "420-SD2-2", "420-SE1", "420-SE2", "420-UC1" });
            LiberationsVoulues.Add("Daniel", new List<string>() { "Coord Programme", "Coord Départementale", "CATI", "Syndicat1", "Syndicat2", "Hololens" });

            CoursVoulus.Add("Nathalie", new List<string>() { "204-CJV" });
            LiberationsVoulues.Add("Nathalie", new List<string>() { });


            foreach(string unprof in Enseignants.Keys)
            {
                foreach (string uneAlloc in CoursVoulus[unprof])
                {
                    Enseignants[unprof].AjouteAllocationDesirees(CursusA[uneAlloc]);
                }
                foreach (string uneAlloc in LiberationsVoulues[unprof])
                {
                    Enseignants[unprof].AjouteAllocationDesirees(LiberationsA[uneAlloc]);
                }
            }


        }
    }
}
