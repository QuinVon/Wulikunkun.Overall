﻿using System;
using System.Text.RegularExpressions;

namespace Wulikunkun.Utility
{
    /// <summary>
    /// 
    /// </summary>
    public class Decode
    {
        public static string Unicode2String(string source)
        {
            return new Regex(@"\\u([0-9A-F]{4})", RegexOptions.IgnoreCase | RegexOptions.Compiled).Replace(
                         source, x => string.Empty + Convert.ToChar(Convert.ToUInt16(x.Result("$1"), 16)));
        }
    }
}
