using System;
using System.ComponentModel.DataAnnotations;

namespace RecipEase.Shared.Models
{
    public enum UnitType
    {
        Volume, Weight
    }

    public class Unit
    {
        [Key]
        public string Name { get; set; }

        [Required]
        public UnitType UnitType { get; set; }

        //some unit doesn't have symbol
        public string Symbol { get; set; }
    }
}