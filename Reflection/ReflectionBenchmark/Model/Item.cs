﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace ReflectionBenchmark.Model;

public partial class Item
{
    public int ItemId { get; set; }

    public int DocumentId { get; set; }

    public int ProductNumber { get; set; }

    public decimal Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal Discount { get; set; }

    public virtual Document Document { get; set; }

    public virtual Product ProductNumberNavigation { get; set; }
}