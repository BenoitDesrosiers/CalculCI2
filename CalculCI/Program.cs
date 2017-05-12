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
        private static IDictionary<string, Liberation> LiberationsH = new Dictionary<string, Liberation>();

        private static IDictionary<string, Prof> Enseignants = new Dictionary<string, Prof>();




        static void Main(string[] args)
        {
            SeedAllocationAutomne();
            //SeedAllocationHiver();
            SeedProf();
            Console.WriteLine("CI et Allocation Par Cours");

            List<Allocation> touteLallocA = new List<Allocation>();
            foreach ( Cours unCours in CursusA.Values)
            {
                touteLallocA.Add(unCours);
            }
            foreach ( Liberation unLib in LiberationsA.Values)
            {
                touteLallocA.Add(unLib);
            }
            


            // calcule toutes les combinaisons gagnantes pour chacun des profs.  

            // Automne
            Calculateur laChose = new Calculateur(Enseignants.Values.ToList(), touteLallocA, CursusA.Count+LiberationsA.Count);
            List<Array> possibleA = laChose.Calcul();

            //Hiver
           /*
           laChose = new Calculateur(Enseignants.Values.ToList(), touteLallocA, CursusA.Count + LiberationsA.Count);
            List<Array> possibleH = laChose.Calcul();
            */
            Console.ReadLine();
        }

        private static void SeedAllocationAutomne()
        {
            Allocation.ResetBinId();
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

            LiberationsA.Add("Coord Programme", new Liberation("Coord Programme", 0.2631));
            LiberationsA.Add("Coord Départementale", new Liberation("Coord Départementale", 0.2));
            LiberationsA.Add("CATI", new Liberation("CATI", 0.1));
            LiberationsA.Add("Syndicat1", new Liberation("Syndicat1", 0.5));
            LiberationsA.Add("Syndicat2", new Liberation("Syndicat2", 0.5));
            LiberationsA.Add("Hololens", new Liberation("Hololens", 0.1));
        }

        private static void SeedAllocationHiver()
        {
            

            LiberationsH.Add("Refonte", new Liberation("Refonte", 0.1)); 
        }

        private static void SeedProf()
        {
            List<string> lesProfs = new List<string>() { "Guy", "Louis", "Jonathan", "Benoit", "Stéphane", "Daniel", "Nathalie" };

            foreach (string nomProf in lesProfs)
            {
                Prof unProf = new Prof(nomProf, Constantes.CIMinSession);
                Enseignants.Add(nomProf, unProf);
            }
            Enseignants["Nathalie"].CiMinimun = 0;

            // Ajoute les allocations qui sont pré-assignées (lib syndicales, ou tout cours qu'on doit/veut forcer à qqun)

            Enseignants["Guy"].AjoutePreAllocation(LiberationsA["Syndicat2"]);
            Enseignants["Louis"].AjoutePreAllocation(LiberationsA["Syndicat1"]);
            Enseignants["Jonathan"].AjoutePreAllocation(LiberationsA["Coord Départementale"]);
            Enseignants["Stéphane"].AjoutePreAllocation(LiberationsA["Coord Programme"]);
            Enseignants["Stéphane"].AjoutePreAllocation(LiberationsA["Hololens"]);
            Enseignants["Stéphane"].AjoutePreAllocation(LiberationsA["CATI"]);
            //Enseignants["Stéphane"].AjoutePreAllocation(LiberationsA["Refonte"]);

            Enseignants["Benoit"].AjoutePreAllocation(CursusA["420-CN2"]);
            Enseignants["Stéphane"].AjoutePreAllocation(CursusA["420-DM1"]);
            Enseignants["Stéphane"].AjoutePreAllocation(CursusA["420-UC1"]);
            Enseignants["Louis"].AjoutePreAllocation(CursusA["420-SE2"]);
            //Enseignants["Louis"].AjoutePreAllocation(CursusH["420-PRB"]);
            //Enseignants["Daniel"].AjoutePreAllocation(CursusH["420-TE1"]);

            Enseignants["Jonathan"].AjoutePreAllocation(CursusA["420-PR3"]);
            


            Enseignants["Nathalie"].AjoutePreAllocation(CursusA["204-CJV"]);


            // L'allocation pré-allouée est une charge possible si elle est plus grande que le Ci Min.
            foreach (string nomProf in lesProfs)
            {
                Enseignants[nomProf].AllocationsPossibles = new List<ulong>();

                if (Enseignants[nomProf].CiActuelle() >= Enseignants[nomProf].CiMinimun)
                {
                    Enseignants[nomProf].AllocationPossibleAjouteListe(Enseignants[nomProf].AllocationPreAlloueA);
                }
            }
           

            // Crée la liste des cours qu'un prof VEUT/PEUT donner. 

            
            Dictionary<string, List<string>> CoursVoulus = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> LiberationsVoulues = new Dictionary<string, List<string>>();

            CoursVoulus.Add("Benoit", new List<string>() {  "420-BD1", "420-CM1", "420-DD1", "420-DM1", "420-FT1", "420-PR3", "420-PRA", });
            LiberationsVoulues.Add("Benoit", new List<string>() { "CATI", "Hololens" });

            CoursVoulus.Add("Louis", new List<string>() {  "420-PR3", "420-PRA", "420-REB","420-SD2-1", "420-SD2-2", "420-SE1", "420-SE2" });
            LiberationsVoulues.Add("Louis", new List<string>() { "CATI", "Hololens" });

            CoursVoulus.Add("Guy", new List<string>() { "420-BD1", "420-CM1", "420-DD1", "420-FT1", "420-PR3", "420-PRA", "420-UC1", });
            LiberationsVoulues.Add("Guy", new List<string>() { "CATI", "Hololens" });

            CoursVoulus.Add("Jonathan", new List<string>() {   "420-DM1","420-FT1", "420-PR3", "420-PRA", });
            LiberationsVoulues.Add("Jonathan", new List<string>() { "CATI", "Hololens" });

            CoursVoulus.Add("Stéphane", new List<string>() {  "420-BD1", "420-CM1", "420-DD1", "420-DM1", "420-FT1", "420-PR3", "420-PRA", "420-REB", });
            LiberationsVoulues.Add("Stéphane", new List<string>() {  "CATI",  "Hololens" });

            CoursVoulus.Add("Daniel", new List<string>() {   "420-DD1",  "420-PRA", "420-REB","420-SD2-1", "420-SD2-2", "420-SE1", "420-SE2"});
            LiberationsVoulues.Add("Daniel", new List<string>() { "CATI", "Hololens" });

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
