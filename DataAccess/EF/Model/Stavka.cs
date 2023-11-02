﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace EF.Model;

public partial class Stavka
{
    public int IdStavke { get; set; }

    /// <summary>
    /// Identifikator dokumenta
    /// </summary>
    public int IdDokumenta { get; set; }

    /// <summary>
    /// Šifra artikla
    /// </summary>
    public int SifArtikla { get; set; }

    /// <summary>
    /// Količina artikla (za pojedine jedinice mjere može biti decimalni broj)
    /// </summary>
    public decimal KolArtikla { get; set; }

    /// <summary>
    /// Cijena jediničnog artikla bez poreza. Inicijalno cijena iz tablice Artikl
    /// </summary>
    public decimal JedCijArtikla { get; set; }

    /// <summary>
    /// Postotak popusta za pojedinu stavku
    /// </summary>
    public decimal PostoRabat { get; set; }

    public virtual Dokument IdDokumentaNavigation { get; set; }

    public virtual Artikl SifArtiklaNavigation { get; set; }
}