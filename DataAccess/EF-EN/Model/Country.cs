﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace EF_EN.Model
{
    public partial class Country
    {
        public Country()
        {
            Cities = new HashSet<City>();
        }

        public string CountryCode { get; set; }
        public string CoutryName { get; set; }
        public string CountryIso3 { get; set; }

        public virtual ICollection<City> Cities { get; set; }
    }
}