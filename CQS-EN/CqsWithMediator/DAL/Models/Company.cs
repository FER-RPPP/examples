﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Company
{
    public int CompanyId { get; set; }

    public string RegistrationNumber { get; set; }

    public string CompanyName { get; set; }

    public virtual Partner CompanyNavigation { get; set; }
}