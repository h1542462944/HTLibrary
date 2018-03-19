using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User
{
    namespace UMath
    {
        /// <summary>
        /// 表示一个复数.
        /// </summary>
        public struct Virtualnumber:IEqualityComparer<Virtualnumber>
        {
            double specificpart;
            double virtualpart;

            public Virtualnumber(double specificpart, double virtualpart) : this()
            {
                Specificpart = specificpart;
                Virtualpart = virtualpart;
            }

            public double Specificpart { get => specificpart; set => specificpart = value; }
            public double Virtualpart { get => virtualpart; set => virtualpart = value; }

            public override string ToString()
            {
                if (Specificpart != 0.0)
                {
                    if (Virtualpart > 0)
                    {
                        return Specificpart + "+" + Virtualpart + "i";
                    }
                    else if (Virtualpart == 0)
                    {
                        return Specificpart.ToString();
                    }
                    else
                    {
                        return Specificpart.ToString() + Virtualpart + "i";
                    }
                }
                else if(Virtualpart != 0.0)
                {
                    return Specificpart.ToString() + "i";
                }
                else
                {
                    return "0";
                }
            }

            public static Virtualnumber operator +(Virtualnumber v1, Virtualnumber v2)
            {
                return new Virtualnumber(v1.Specificpart + v2.Specificpart, v1.Virtualpart + v2.Virtualpart);
            }
            public static Virtualnumber operator -(Virtualnumber v1, Virtualnumber v2)
            {
                return new Virtualnumber(v1.Specificpart - v2.Specificpart, v1.Virtualpart - v2.Virtualpart);
            }
            public static Virtualnumber operator *(Virtualnumber v1, Virtualnumber v2)
            {
                return new Virtualnumber(v1.Specificpart * v2.Specificpart - v1.Virtualpart * v2.Virtualpart, v1.Specificpart * v2.Virtualpart + v1.Virtualpart * v2.Specificpart);
            }
            public static Virtualnumber operator /(Virtualnumber v1, Virtualnumber v2)
            {
                Virtualnumber newv = v1 * GetConjugate(v2);
                double mod = (v2 * GetConjugate(v2)).Specificpart;
                return new Virtualnumber(newv.Specificpart / mod, newv.Virtualpart / mod);
            }
            public static Virtualnumber GetConjugate(Virtualnumber value)
            {
                return new Virtualnumber(value.Specificpart, -value.Virtualpart);
            }
            public static double GetModel(Virtualnumber value)
            {
                return System.Math.Sqrt(System.Math.Pow(value.Specificpart, 2) + System.Math.Pow(value.Virtualpart, 2));
            }
            public Virtualnumber GetConjugate()
            {
                return new Virtualnumber(Specificpart, -Virtualpart);
            }
            public double GetModel()
            {
                return System.Math.Sqrt(System.Math.Pow(Specificpart, 2) + System.Math.Pow(Virtualpart, 2));
            }
            public bool Equals(Virtualnumber x, Virtualnumber y)
            {
                return x.Specificpart == y.Specificpart && x.Virtualpart == y.Virtualpart;
            }
            public int GetHashCode(Virtualnumber obj)
            {
               return  base.GetHashCode();
            }
            public static implicit operator Virtualnumber (double value)
            {
                return new Virtualnumber(value, 0);
            }
        }
    }
}
