using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipEase.Shared.Models
{
    public class UnitConversion
    {
        [ForeignKey("ConvertsToUnit")]
        public string ConvertsToUnitName { get; set; }

        [ForeignKey("ConvertsFromUnit")]
        public string ConvertsFromUnitName { get; set; }

        public double Ratio { get; set; }


        public Unit ConvertsToUnit { get; set; }
        public Unit ConvertsFromUnit { get; set; }
    }
}