using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using System.Text.Json.Serialization;

namespace Models
{
    public class Fractionable
    {
        [JsonPropertyName("Whole")]
        public int Whole { get; set; }
        [JsonPropertyName("Numerator")]
        public int Numerator { get; set; }
        [JsonPropertyName("Denominator")]
        public int Denominator { get; set; }

    public static Fractionable operator +(Fractionable a, Fractionable b)
        {
            int LowestCommonDenominator = Fractionable.LowestCommonDenominator(a.Denominator, b.Denominator);
            Fractionable intermediary = new Fractionable
            {
                Whole = 0,
                Numerator = a.Whole * LowestCommonDenominator 
                + b.Whole * LowestCommonDenominator
                + a.Numerator * LowestCommonDenominator / a.Denominator 
                + b.Numerator * LowestCommonDenominator / b.Denominator,
                Denominator = LowestCommonDenominator
            };
            return MakeProperFraction(intermediary);
        }

        public static Fractionable MakeProperFraction(Fractionable improper)
        {
            int numeratorPartial = (improper.Numerator % improper.Denominator);
            return new Fractionable
            {
                Whole = (improper.Whole * improper.Denominator + improper.Numerator) / improper.Denominator,
                Numerator = numeratorPartial / GreatestCommonFactor(numeratorPartial, improper.Denominator),
                Denominator = (improper.Denominator) / GreatestCommonFactor(numeratorPartial, improper.Denominator)
            };            
        }

        public static int LowestCommonDenominator(int d1, int d2)
        {
            return (d1 / GreatestCommonFactor(d1, d2)) * d2;
        }

        public static int GreatestCommonFactor(int d1, int d2)
        {
            int den1 = d1, den2 = d2, temp;
            while (den2 != 0)
            {
                temp = den2;
                den2 = den1 % den2;
                den1 = temp;
            }
            return den1;
        }
    }
}
