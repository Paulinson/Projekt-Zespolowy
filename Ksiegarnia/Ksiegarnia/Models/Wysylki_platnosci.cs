//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Ksiegarnia.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Wysylki_platnosci
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Wysylki_platnosci()
        {
            this.Zamowienia = new HashSet<Zamowienia>();
        }
    
        public int id_wysylka { get; set; }
        public string nazwa { get; set; }
        public Nullable<double> cena { get; set; }
        public string opis { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Zamowienia> Zamowienia { get; set; }
    }
}
