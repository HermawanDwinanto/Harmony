﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harmony.Shared.Constants.Application
{
    public static class LabelColorsConstants
    {
        public const string GREEN = "#216e4e";
        public const string YELLOW = "#7f5f01";
        public const string ORANGE = "#974f0c";
        public const string RED = "#ae2a19";
        public const string PURPLE = "#5e4db2";
        public const string BLUE = "#0055cc";
        public const string PINK = "#943d73";

        public static List<string> GetDefaultColors()
        {
            var colors = new List<string> { GREEN, YELLOW, ORANGE, RED, PURPLE, BLUE, PINK };

            return colors;
        }
    }
}