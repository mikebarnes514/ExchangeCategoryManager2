﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ExchangeCategoryMonitor2.Categories
{
    public enum CategoryColor : byte
    {
        [XmlEnum("-1")]
        None,
        [XmlEnumAttribute("0")]
        Red,
        [XmlEnumAttribute("1")]
        Orange,
        [XmlEnumAttribute("2")]
        Peach,
        [XmlEnumAttribute("3")]
        Yellow,
        [XmlEnumAttribute("4")]
        Green,
        [XmlEnumAttribute("5")]
        Teal,
        [XmlEnumAttribute("6")]
        Olive,
        [XmlEnumAttribute("7")]
        Blue,
        [XmlEnumAttribute("8")]
        Purple,
        [XmlEnumAttribute("9")]
        Maroon,
        [XmlEnumAttribute("10")]
        Steel,
        [XmlEnumAttribute("11")]
        DarkSteel,
        [XmlEnumAttribute("12")]
        Gray,
        [XmlEnumAttribute("13")]
        DarkGray,
        [XmlEnumAttribute("14")]
        Black,
        [XmlEnumAttribute("15")]
        DarkRed,
        [XmlEnumAttribute("16")]
        DarkOrange,
        [XmlEnumAttribute("17")]
        DarkPeach,
        [XmlEnumAttribute("18")]
        DarkYellow,
        [XmlEnumAttribute("19")]
        DarkGreen,
        [XmlEnumAttribute("20")]
        DarkTeal,
        [XmlEnumAttribute("21")]
        DarkOlive,
        [XmlEnumAttribute("22")]
        DarkBlue,
        [XmlEnumAttribute("23")]
        DarkPurple,
        [XmlEnumAttribute("24")]
        DarkMaroon
    }
}
