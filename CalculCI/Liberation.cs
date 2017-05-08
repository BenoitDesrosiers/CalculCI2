using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculCI
{
    class Liberation : Allocation
    {
        public double Charge { get; }
         
        public Liberation(string nom, double charge) : base(nom)
        {
            Charge = charge;
        }

        public override bool ComptePourNbrCours()
        {
            return false;
        }

        public override double CalculCI(int nbrCours)
        {
            // le nombre de cours est pas important pour une libération
            return Charge * 40;
        }
    
    }

}
