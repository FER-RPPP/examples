﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace EFModel;

public partial class Tvrtka
{
    /// <summary>
    /// Identifikator tvrtke
    /// </summary>
    public int IdTvrtke { get; set; }

    /// <summary>
    /// Matični broj tvrtke
    /// </summary>
    public string MatBrTvrtke { get; set; }

    /// <summary>
    /// Naziv tvrtke
    /// </summary>
    public string NazivTvrtke { get; set; }

    public virtual Partner IdTvrtkeNavigation { get; set; }
}