﻿using System.ComponentModel.DataAnnotations;

namespace Debby.Admin.Sample.Models.Northwind
{
    public class Shipper
    {
        public int ShipperID { get; set; }

        [Required, MaxLength(40)]
        public string CompanyName { get; set; }

        [MaxLength(24)]
        public string Phone { get; set; }
    }
}