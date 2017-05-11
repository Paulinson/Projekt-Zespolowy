using Ksiegarnia.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ksiegarnia.ViewModels
{
    public class KoszykViewModel
    {
        public List<PozycjaKoszyka> pozycjeKoszyka { get; set; }
        public decimal cenaCalkowita { get; set; }
    }
}