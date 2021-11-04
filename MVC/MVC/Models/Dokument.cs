﻿using System;
using System.Collections.Generic;

namespace MVC.Models
{
    public partial class Dokument
    {
        public Dokument()
        {
            InverseIdPrethDokumentaNavigation = new HashSet<Dokument>();
            Stavka = new HashSet<Stavka>();
        }

        public int IdDokumenta { get; set; }
        public string VrDokumenta { get; set; }
        public int BrDokumenta { get; set; }
        public DateTime DatDokumenta { get; set; }
        public int IdPartnera { get; set; }
        public int? IdPrethDokumenta { get; set; }
        public decimal PostoPorez { get; set; }
        public decimal IznosDokumenta { get; set; }

        public virtual Partner IdPartneraNavigation { get; set; }
        public virtual Dokument IdPrethDokumentaNavigation { get; set; }
        public virtual ICollection<Dokument> InverseIdPrethDokumentaNavigation { get; set; }
        public virtual ICollection<Stavka> Stavka { get; set; }
    }
}
