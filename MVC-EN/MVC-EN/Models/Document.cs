﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace MVC_EN.Models
{
    public partial class Document
    {
        public Document()
        {
            InversePreviousDocument = new HashSet<Document>();
            Items = new HashSet<Item>();
        }

        public int DocumentId { get; set; }
        public string DocumentType { get; set; }
        public int DocumentNo { get; set; }
        public DateTime DocumentDate { get; set; }
        public int PartnerId { get; set; }
        public int? PreviousDocumentId { get; set; }
        public decimal Vat { get; set; }
        public decimal Amount { get; set; }

        public virtual Partner Partner { get; set; }
        public virtual Document PreviousDocument { get; set; }
        public virtual ICollection<Document> InversePreviousDocument { get; set; }
        public virtual ICollection<Item> Items { get; set; }
    }
}