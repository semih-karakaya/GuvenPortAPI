﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable enable
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GuvenPortAPI.Models;

public partial class Office
{
    public int Id { get; set; }

    public string? Address { get; set; }

    public string? OName { get; set; }

    public string? Crm { get; set; }

    public bool? Active { get; set; }

    public int? IdManagerstaff { get; set; }
    [JsonIgnore]
    public virtual Staff? IdManagerstaffNavigation { get; set; }
    [JsonIgnore]
    public virtual ICollection<StaffOffice> StaffOffice { get; set; } = new List<StaffOffice>();
    [JsonIgnore]
    public virtual ICollection<Workplace> Workplace { get; set; } = new List<Workplace>();
}