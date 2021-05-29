﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using System.ComponentModel;
using System.Data;
using System.Drawing;


using System.Diagnostics;
using System.IO;



using System.Xml;
using System.Globalization;

using System.Xml.Xsl;
using System.Xml.XPath;

namespace calculator
{
    class Calculation
    {

        static private bool IsOperator(char с)
        {
            if (("+-/*^()PC!%".IndexOf(с) != -1))
                return true;
            return false;
        }
        

        static private double Count(string input)
        {
            double result = 0;
            double b = 0;
            Stack<double> temp = new Stack<double>(); 
            try { return double.Parse(input); }
            catch (Exception)
            {
                for (int i = 0; i < input.Length; i++) 
                {
                    if (Char.IsDigit(input[i]))
                    {
                        string a = string.Empty;

                        while (!IsDelimeter(input[i]) && !IsOperator(input[i])) 
                        {
                            a += input[i];
                            i++;
                            if (i == input.Length) break;
                        }
                        temp.Push(double.Parse(a)); 
                        i--;
                    }
                    else if (input[i] == '\u03C0')
                        temp.Push(Math.PI);
                    else if (input[i] == 'e')
                        temp.Push(Math.E);
                    else if (IsOperator(input[i])) 
                    {
                        double a = temp.Pop();
                        try
                        { b = temp.Pop(); }
                        catch (Exception) { b = 0; }

                        switch (input[i])
                        {
                            case '!': result = factorial((int)a); break;
                            case 'P': result = factorial((int)b) / factorial((int)(b - a)); break;
                            case 'C': result = factorial((int)b) / (factorial((int)a) * factorial((int)(b - a))); break;
                            case '^': result = Math.Pow(b,a); break;
                            case '%': result = b % a; break;
                            case '+': result = b + a; break;
                            case '-': result = b - a; break;
                            case '*': result = b * a; break;
                            case '/': if (a == 0) throw new DividedByZeroExclusion(); else result = b / a; break;
                        }
                        temp.Push(result);
                    }
                }
                try { return temp.Peek(); }
                catch (Exception) { throw new SyntaxExclusion(); }
                
            }
            
        }
        static public double Calculate(string input)
        {
            try { return double.Parse(ExpressionGet(input)); }
            catch (Exception) { return Count(ExpressionGet(input)); }

        }
        static private bool IsDelimeter(char c)
        {
            if ((" =".IndexOf(c) != -1))
                return true;
            return false;
        }
        
        static private bool IsFunction(String s)
        {
            String[] func = { "sin", "cos", "tg", "asin", "acos", "atg", "sqrt", "ln","lg" };
            if (Array.Exists(func, e => e == s))
                return true;
            return false;
        }
        static private String doFunction(String fun,double param)
        {
            switch (fun)
            {
                case "cos": return Math.Cos(param).ToString();

                case "sin": return Math.Sin(param).ToString();

                case "tg": if (Math.Abs(param % (2 * Math.PI)) == (Math.PI / 2)) throw new TgExclusion(param); else return Math.Tan(param).ToString();
                
                case "asin": if (param < -1 || param > 1) throw new ArcSinCosExclusion(param); else return Math.Asin(param).ToString();
                
                case "acos": if (param < -1 || param > 1) throw new ArcSinCosExclusion(param); else return Math.Acos(param).ToString();
                
                case "atg": return Math.Atan(param).ToString();
                
                case "sqrt": if (param < 0) throw new SqrtExclusion(param); else return Math.Sqrt(param).ToString();
                
                case "ln": if (param <= 0) throw new LogExclusion(param); else return Math.Log(param).ToString();
                
                case "lg": if (param <= 0) throw new LogExclusion(param); else return Math.Log10(param).ToString();
                default: return "";
            }
        }
        static private byte PriorityGet(char s)
        {
            switch (s)
            {
                case '(': return 0;
                case ')': return 1;
                case '+': return 2;
                case '-': return 3;
                case '!': return 4;
                case '%': return 4;
                case '*': return 4;
                case '/': return 4;
                case '^': return 5;
                default: return 4;
            }
        }
        private static int factorial(int x)
        {
            int i = 1;
            for (int s = 1; s <= x; s++)
                i = i * s;
            if (x < 0) throw new NegativeFactorialExclusion(x);
            return i;
        }
        static private string ExpressionGet(string input)
        {
            string outputing = "";
            string acd = "";
            Stack<char> operStack1 = new Stack<char>();
            char k = ' '; string p = "";
            for (int i = 0; i < input.Length; i++)
            {
                if (IsOperator(input[i]) || Char.IsDigit(input[i]))
                {
                    if (k == ' ')
                        k = input[i];
                    else
                        if (input[i] == '-' && !Char.IsDigit(k))
                        p += " 0 ";
                    k = input[i];
                }
                p += input[i];
            }
            input = p;
            for (int i = 0; i < input.Length; i++)
            {
                if (IsDelimeter(input[i]))
                    continue;
                if (Char.IsDigit(input[i]))
                {
                    while (!IsDelimeter(input[i]) && !IsOperator(input[i]))
                    {
                        outputing += input[i];
                        i++;
                        if (i == input.Length) break;
                    }
                    outputing += " ";
                    i--;
                }
                else
                    if (IsOperator(input[i]))
                {
                    if (input[i] == '(')
                        operStack1.Push(input[i]);
                    else if (input[i] == ')')
                    {
                        char s = operStack1.Pop();
                        while (s != '(')
                        {
                            outputing += s.ToString() + ' ';
                            s = operStack1.Pop();
                        }
                    }
                    else
                    {
                        if (operStack1.Count > 0)
                            if (PriorityGet(input[i]) <= PriorityGet(operStack1.Peek()))
                                outputing += operStack1.Pop().ToString() + " ";

                        operStack1.Push(char.Parse(input[i].ToString()));

                    }
                }
                else if (input[i] == '\u03C0')
                    outputing += " \u03C0 ";
                else if (input[i] == 'e')
                    outputing += " e ";
                else
                {
                    acd = String.Empty;
                    while (input[i] != '(')
                    {
                        acd += input[i];
                        i++;
                        if (i == input.Length) break;
                    }
                    i++;
                    if (IsFunction(acd))
                    {
                        String param = String.Empty;
                        while (input[i] != ')')
                        {
                            param += input[i];
                            i++;
                            if (i == input.Length) break;
                        }
                        double d;
                        try { d = double.Parse(param); }
                        catch (Exception) { d = Count(ExpressionGet(param)); }
                        outputing += doFunction(acd, d);
                    }
                }
            }
            while (operStack1.Count > 0)
                outputing += operStack1.Pop() + " ";

            return outputing;
        }
        public void removeBackSlash()
        {
           
            
        }
    }
    public class Exclusion : Exception
    {
        public string type;



    }
    public class NegativeFactorialExclusion : Exclusion
    {
        public NegativeFactorialExclusion(int x)
        {
            this.type = "Математична помилка";
            MessageBox.Show("Факторіал(" + x + ") не існує", type, MessageBoxButtons.OK);
        }
    }
    public class TgExclusion : Exclusion
    {
        public TgExclusion(double x)
        {
            this.type = "Математична помилка";
            MessageBox.Show("Tg(" + x + ") не існує", type, MessageBoxButtons.OK);
        }
    }
    public class SqrtExclusion : Exclusion
    {
        public SqrtExclusion(double x)
        {
            this.type = "Математична помилка";
            MessageBox.Show("Корінь(" + x + ") не існує", type, MessageBoxButtons.OK);
        }
    }
    public class DividedByZeroExclusion : Exclusion
    {
        public DividedByZeroExclusion()
        {
            this.type = "Математична помилка";
            MessageBox.Show("Ділення на 0 неможливе", type, MessageBoxButtons.OK);
        }
    }
    public class LogExclusion : Exclusion
    {
        public LogExclusion(double x)
        {
            this.type = "Математична помилка";
            MessageBox.Show("Log(" + x + ") не існує", type, MessageBoxButtons.OK);
        }
    }
    public class SyntaxExclusion : Exclusion
    {
        public SyntaxExclusion()
        {
            this.type = "Синтаксична помилка";
            MessageBox.Show("Ви зробили помилку", type, MessageBoxButtons.OK);
        }
    }
    public class ArcSinCosExclusion : Exclusion
    {
        public ArcSinCosExclusion(double x)
        {
            this.type = "Математична помилка";
            MessageBox.Show("Acos(або Asin) (" + x + ") не існує", type, MessageBoxButtons.OK);
        }
    }
}