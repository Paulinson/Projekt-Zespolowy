using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ksiegarnia.ViewModels
{
    public class UsunZKoszykaViewModel
    {
        public decimal koszykCenaCalkowita { get; set; }
        public int koszykIloscPozycji { get; set; }
        public int iloscPozycjiUsuwanej { get; set; }
        public int idPozycjiUsuwanej { get; set; }
    }
}