using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculCI
{
    abstract class Allocation
    {
        // étant donné que l'id est un ulong, on ne peut avoir que 64 allocations

        static ulong GlobalBinId = 1;

        public string Nom { get; }
        public bool Assigne = false;
        public double CiSimple { get; set; } = 0;  // la CI pour ce cours. Précalculée pour accélerer les calculs
        public ulong BinId { get; }



        public Allocation(string nom)
        {
            Nom = nom;
            BinId = GlobalBinId;
            GlobalBinId = GlobalBinId * 2;
        }

        public static void ResetBinId()
        {
            GlobalBinId = 1;
        }

       
        abstract public double CalculCI(int nbrCours);

        abstract public bool ComptePourNbrCours();

        public bool estAssigne()
        {
            return Assigne;
        }

        public bool estLibre()
        {
            return !Assigne;
        }
    }
}
