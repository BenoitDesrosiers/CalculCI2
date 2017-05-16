using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;

namespace CalculCI
{
    class Calculateur
    {
        static IList<Prof> Profs;
        static IList<Allocation> Allocations;
        static int NbrAlloc;
        public Calculateur(IList<Prof> profs, IList<Allocation> allocations, int nbrAlloc )
        {
            Profs = profs;
            Allocations = allocations; 
            NbrAlloc = nbrAlloc;
        }

        public List<Array> Calcul()
        {
            Console.WriteLine("début des calculs");
            Console.WriteLine("Calcule toutes les possibilités par prof");
            foreach (Prof unProf in Profs)
            {
                CalculPossibilite(unProf, unProf.MaskPreAlloue, unProf.MaskPreAlloue, new List<Allocation>(), 0);
                //unProf.DumpAllocationsPossibles();
                Console.WriteLine("{0} {1}", unProf.Nom, unProf.AllocationsPossibles.Count);
            }

            Console.WriteLine("Calcule toutes les combinaisons gagnantes");

            ulong[] solutionVide = new ulong[Profs.Count];
            return TrouveBonneCharge(0, new List<Array>() { solutionVide });


        }


        /// <summary>
        /// calcul toutes les possibilités d'assigné une charge à un prof en sachant ses allocations voulus
        /// </summary>
        /// <param name="prof">Le prof à qui assigner la charge</param>
        /// <param name="maskAlloc">Son assignation jusqu'à maintenant</param>
        /// <param name="fait">Les allocations qui ont déjà été faites</param>
        /// <param name="allocAdditionnelle">L'allocation qui est assignée à date ne faissant pas partie des pré-alloués</param>
        /// <param name="nbrCoursAdditionnels">le nombre de cours (pas les libérations) dans l'alloc additionnel. Conservé uniquement pour accéler les calculs</param>
        static void CalculPossibilite(Prof prof, ulong maskAlloc, ulong fait, List<Allocation> allocAdditionnelle, int nbrCoursAdditionnels)
        {
            foreach(Allocation alloc in prof.AllocationsDesireesA.Values)
            {
                // si libre et pas encore fait
                if (alloc.estLibre() & ((fait | alloc.BinId) != fait )) 
                {
                    fait += alloc.BinId;
                    allocAdditionnelle.Add(alloc);
                    nbrCoursAdditionnels += alloc.ComptePourNbrCours() ? 1 : 0;
                    
                    if(prof.PeutPrendreAllocList(allocAdditionnelle, nbrCoursAdditionnels))
                    {
                        maskAlloc += alloc.BinId;

                        // c'est une bonne alloc, mais doit être ajouté seulement si elle est assez grande (­>= ci minimum du prof)
                        if (prof.CiPourAllocListAdditionnel(allocAdditionnelle, nbrCoursAdditionnels) >= prof.CiMinimun)
                        {
                            prof.AllocationsPossibles.Add(maskAlloc);
                        }
                      
                        //Console.WriteLine("{0} {1} {2}", maskAlloc, ConvertToBinary(maskAlloc), prof.CiPourAllocListAdditionnel(allocAdditionnelle, nbrCoursAdditionnels));
                        CalculPossibilite(prof, maskAlloc, fait, allocAdditionnelle, nbrCoursAdditionnels);
                        maskAlloc -= alloc.BinId;

                    }
                    nbrCoursAdditionnels -= alloc.ComptePourNbrCours() ? 1 : 0;
                    allocAdditionnelle.Remove(alloc);

                }
            }
        }

        static List<Array> TrouveBonneCharge(int indexProf, List<Array> bonneAlloc)
        {
            List<Array> vraieBonneAlloc = new List<Array>(); // contiendra juste les allocs qui ont tous les cours
            if (indexProf == Profs.Count)
            {
                // on a finit
                // on trouve les vraies bonne combinaisons (tous les cours assignée, et charge qui balance) 
                // et on sort d'ici
                Console.WriteLine("en Partant {0}", bonneAlloc.Count);

                // Conserve uniquement les allocs ou tous les cours sont assignés.
                foreach (ulong[] uneBonneAlloc in bonneAlloc)
                {
                    ulong allocTotal = UlongOr(uneBonneAlloc);

                    if ((allocTotal == ((ulong)Math.Pow(2, NbrAlloc) - 1)))
                    {
                        vraieBonneAlloc.Add(uneBonneAlloc);
                    }
                }
                Console.WriteLine("conservée {0}", vraieBonneAlloc.Count);


                for (int i = 0; i < vraieBonneAlloc.Count - 1; i++)
                {
                    for (int j = i + 1; j < vraieBonneAlloc.Count; j++)
                    {
                        bool diff = false;

                        for (int k = 0; k < Profs.Count; k++)
                        {
                            ulong[] x = (ulong[])vraieBonneAlloc[i];
                            ulong[] y = (ulong[])vraieBonneAlloc[j];
                            if (x[k] != y[k])
                            { diff = true; }
                        }

                        if (!diff)
                        {
                            Console.WriteLine("{0} {1} {2} {3}", i, j, UlongOr((ulong[])vraieBonneAlloc[i]), UlongOr((ulong[])vraieBonneAlloc[j]));
                        }
                    }
                }

                foreach (ulong[] uneBonneAlloc in vraieBonneAlloc)
                {
                    int profid = 0;
                    char[] allocString = new char[Allocations.Count];
                    foreach (ulong allocProf in uneBonneAlloc)
                    {
                        string allocProfString = ConvertToBinary(allocProf, Profs[profid].Nom[0]);
                        for( int i = 0; i< allocProfString.Length; i++ )
                        {
                            if(allocProfString[i] != '0')
                            {
                                allocString[i] = allocProfString[i];
                            }
                        }
                        profid++;
                    }

                    foreach(char c in allocString)
                        Console.Write($"{c}");

                    Console.WriteLine();


                    /*
                    int profid = 0
                    foreach (ulong allocProf in uneBonneAlloc)
                    {
                        ConvertToBinary(allocProf, Profs[profid].Nom[0]);
                        foreach (Allocation alloc in ConvertiUlongEnCours(allocProf))
                        {
                            Console.WriteLine("{0}", alloc.Nom);
                        }
                        profid++;

                    }
                    */

                }
                Console.WriteLine("conservée {0}", vraieBonneAlloc.Count);

            }

            else
            {
                List<Array> listDeNouvelleBonneAlloc = new List<Array>();

                // passe à travers toutes les allocations possibles pour ce profs
                foreach (ulong allocProf in Profs[indexProf].AllocationsPossibles)
                {
                    //check si l'alloc du prof est en conflit avec toutes les allocs trouvées à date
                    foreach (ulong[] uneBonneAlloc in bonneAlloc)
                    {
                        ulong allocTotal = UlongOr(uneBonneAlloc);
                        if ((allocTotal & allocProf) == 0)
                        {
                            // c'est une bonne alloc (pas en conflit), on l'ajoute 

                            ulong[] uneNouvelleBonneAlloc = new ulong[Profs.Count];
                            uneBonneAlloc.CopyTo(uneNouvelleBonneAlloc, 0);
                            uneNouvelleBonneAlloc[indexProf] = allocProf;
                            listDeNouvelleBonneAlloc.Add(uneNouvelleBonneAlloc);
                        }

                    }
                    Console.Write(".");

                }
                Console.WriteLine("-------------- {0}", indexProf);
                // on a fini ce prof, on passe au suivant avec toutes les allocs encore bonnes
                bonneAlloc = null; // flush la vieille liste car elle ne sert plus à rien. 
                TrouveBonneCharge(++indexProf, listDeNouvelleBonneAlloc);
            }

            return vraieBonneAlloc;

        }

        /// <summary>
        /// Convertie le ulong représentant une allocation en nom de cours
        /// </summary>
        /// <param name="alloc"></param>
        static List<Allocation> ConvertiUlongEnCours(ulong alloc)
        {
            int id = 0;
            List<Allocation> allocList = new List<Allocation>();
            while (alloc != 0)
            {
                // si c'est impair (odd), alors c'est que le dernier bit à droite est set
                if(alloc %2 !=0)
                {
                    allocList.Add(Allocations.Where(a => a.BinId == Math.Pow(2, id)).First());
                }
                alloc >>= 1;
                id++;
            }

            return allocList;
        }


        // ulong.sum ca existe pas de base. 
        public static ulong UlongSum(ulong[] source)
        {
            ulong total = 0;

            foreach (var item in source)
                total += item;

            return total;
        }

        /// <summary>
        /// Fait un OU logique entre tous les éléments d'un tableau 
        /// C'est comme l'addition, mais ca corrige l'erreur quand y a 
        /// 3 fois (ou plus) la même valeur    1+1+1 = 3, ce qui est une bonne 
        /// valeur dans le tableau, mais pas le bon résultat. 1^1^1 = 1, ce qui est la bonne 
        /// valeur. 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static ulong UlongOr(ulong[] source)
        {
            ulong total = 0;

            foreach (var item in source)
                total = total ^ item;

            return total;
        }

        public static string ConvertToBinary(ulong value, char charactere = '0')
        {
            if (value == 0) return "".PadLeft(NbrAlloc,'0');
            System.Text.StringBuilder b = new System.Text.StringBuilder();
            while (value != 0)
            {
                b.Insert(0, ((value & 1) == 1) ? charactere : '0');
                value >>= 1;
            }
            return b.ToString().PadLeft(NbrAlloc, '0');
        }




        //***********************************************************************
        static void AjoutAlloc(IList<Prof> profs, int indexProf, int indexLibre)
        {
            Allocation nouvelle = null;

            if (indexProf == profs.Count )
            {
                Debug.WriteLine("profs empty");
                // on a fait le tour, on dépille
                AfficheResultat();
                return; 
            }
            else
            {
                Debug.WriteLine(string.Format("AjoutAlloc {0} ci {1}", indexProf, profs[indexProf].CiActuelle()));

                List<Allocation> alloc1 = AllocLibrePourProf(profs[indexProf]);
                if (alloc1.Count == 0)
                {
                    // on a fini un prof. On passe au suivant. 
                    AjoutAlloc(profs, indexProf+1, 0);

                }
                else
                {
                    List<Allocation> preAlloues = profs[indexProf].AllocationPreAlloueA;
                    if(profs[indexProf].PeutPrendreAlloc(alloc1[indexLibre]) )
                    {
                        nouvelle = alloc1[indexLibre];
                        profs[indexProf].AjoutePreAllocation(nouvelle);
                        Debug.Write(nouvelle.Nom);
                    }
                    AjoutAlloc(profs, indexProf, indexLibre);

                }

                //On dépile
                indexProf--;
                indexLibre++;
                if(nouvelle != null )
                    { profs[indexProf].EnlevePreAllocation(nouvelle); }

                AjoutAlloc(profs, indexProf, indexLibre);
            }

        }


        



       /* obsolete, remplacé par prof.PeutPrendreAlloc()
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
        */

        static void AfficheResultat()
        {
            Console.WriteLine("------------------------------------");
            foreach(Prof unprof in Profs)
            {
                Console.WriteLine("{0} ci {1}", unprof.Nom, unprof.CiActuelle());
            }
        }

        static List<Allocation> AllocLibrePourProf(Prof prof)
        {
            double ciActuelle = prof.CiActuelle();
            List<Allocation> libre = new List<Allocation>();
            if (ciActuelle < Constantes.CIMaxSession)
            {
                foreach(Allocation alloc in prof.AllocationsDesireesA.Values)
                {
                    if(!alloc.estAssigne() & prof.PeutPrendreAlloc(alloc))
                    {
                        libre.Add(alloc);
                    }
                }

            }
            return libre;
        }
    }
}
