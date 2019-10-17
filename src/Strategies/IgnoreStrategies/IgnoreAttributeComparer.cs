﻿using System;
using Egil.AngleSharp.Diffing.Core;

namespace Egil.AngleSharp.Diffing.Strategies.IgnoreStrategies
{
    public static class IgnoreAttributeComparer
    {
        private const string DIFF_IGNORE_POSTFIX = ":ignore";

        public static CompareResult Compare(in AttributeComparison comparison, CompareResult currentDecision)
        {
            if (currentDecision.IsDecisionFinal()) return currentDecision;

            return comparison.Control.Attribute.Name.EndsWith(DIFF_IGNORE_POSTFIX, StringComparison.OrdinalIgnoreCase)
                ? CompareResult.Same
                : currentDecision;
        }
    }
}