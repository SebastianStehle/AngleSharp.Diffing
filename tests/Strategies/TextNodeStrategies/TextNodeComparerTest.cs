﻿using Egil.AngleSharp.Diffing.Core;
using Shouldly;
using Xunit;

namespace Egil.AngleSharp.Diffing.Strategies.TextNodeStrategies
{
    public class TextNodeComparerTest : TextnodeStrategyTestBase
    {
        [Theory(DisplayName = "When option is Preserve or RemoveWhitespaceNodes, comparer does not run nor change the current decision")]
        [InlineData(WhitespaceOption.Preserve)]
        [InlineData(WhitespaceOption.RemoveWhitespaceNodes)]
        public void Test5(WhitespaceOption whitespaceOption)
        {
            var comparison = new Comparison(ToComparisonSource("hello world", ComparisonSourceType.Control), ToComparisonSource("  hello   world  ", ComparisonSourceType.Test));
            var sut = new TextNodeComparer(whitespaceOption);

            sut.Compare(comparison, CompareResult.Different).ShouldBe(CompareResult.Different);
            sut.Compare(comparison, CompareResult.DifferentAndBreak).ShouldBe(CompareResult.DifferentAndBreak);
            sut.Compare(comparison, CompareResult.Same).ShouldBe(CompareResult.Same);
            sut.Compare(comparison, CompareResult.SameAndBreak).ShouldBe(CompareResult.SameAndBreak);
        }

        [Fact(DisplayName = "When option is Normalize and current decision is Same or SameAndBreak, compare uses the current decision")]
        public void Test55()
        {
            var comparison = new Comparison();
            var sut = new TextNodeComparer(WhitespaceOption.Normalize);
            sut.Compare(comparison, CompareResult.Same).ShouldBe(CompareResult.Same);
            sut.Compare(comparison, CompareResult.SameAndBreak).ShouldBe(CompareResult.SameAndBreak);
        }

        [Theory(DisplayName = "When option is Normalize, any whitespace before and after a text node is removed before comparison")]
        [MemberData(nameof(WhitespaceCharStrings))]
        public void Test7(string whitespace)
        {
            var sut = new TextNodeComparer(WhitespaceOption.Normalize);
            var normalText = "text";
            var whitespaceText = $"{whitespace}text{whitespace}";
            var c1 = new Comparison(ToComparisonSource(normalText, ComparisonSourceType.Control), ToComparisonSource(normalText, ComparisonSourceType.Test));
            var c2 = new Comparison(ToComparisonSource(normalText, ComparisonSourceType.Control), ToComparisonSource(whitespaceText, ComparisonSourceType.Test));
            var c3 = new Comparison(ToComparisonSource(whitespaceText, ComparisonSourceType.Control), ToComparisonSource(normalText, ComparisonSourceType.Test));
            var c4 = new Comparison(ToComparisonSource(whitespaceText, ComparisonSourceType.Control), ToComparisonSource(whitespaceText, ComparisonSourceType.Test));

            sut.Compare(c1, CompareResult.Different).ShouldBe(CompareResult.Same);
            sut.Compare(c2, CompareResult.Different).ShouldBe(CompareResult.Same);
            sut.Compare(c3, CompareResult.Different).ShouldBe(CompareResult.Same);
            sut.Compare(c4, CompareResult.Different).ShouldBe(CompareResult.Same);
        }

        [Theory(DisplayName = "When option is Normalize, any consecutive whitespace characters are collapsed into one before comparison")]
        [MemberData(nameof(WhitespaceCharStrings))]
        public void Test9(string whitespace)
        {
            var sut = new TextNodeComparer(WhitespaceOption.Normalize);
            var normalText = "hello world";
            var whitespaceText = $"{whitespace}hello{whitespace}{whitespace}world{whitespace}";
            var c1 = new Comparison(ToComparisonSource(normalText, ComparisonSourceType.Control), ToComparisonSource(normalText, ComparisonSourceType.Test));
            var c2 = new Comparison(ToComparisonSource(normalText, ComparisonSourceType.Control), ToComparisonSource(whitespaceText, ComparisonSourceType.Test));
            var c3 = new Comparison(ToComparisonSource(whitespaceText, ComparisonSourceType.Control), ToComparisonSource(normalText, ComparisonSourceType.Test));
            var c4 = new Comparison(ToComparisonSource(whitespaceText, ComparisonSourceType.Control), ToComparisonSource(whitespaceText, ComparisonSourceType.Test));

            sut.Compare(c1, CompareResult.Different).ShouldBe(CompareResult.Same);
            sut.Compare(c2, CompareResult.Different).ShouldBe(CompareResult.Same);
            sut.Compare(c3, CompareResult.Different).ShouldBe(CompareResult.Same);
            sut.Compare(c4, CompareResult.Different).ShouldBe(CompareResult.Same);
        }

        [Theory(DisplayName = "When a parent node has a inline whitespace option, that overrides the global whitespace option")]
        [InlineData(@"<header><h1><em diff:whitespace=""normalize""> foo   bar </em></h1></header>", @"<header><h1><em>foo bar</em></h1></header>")]
        [InlineData(@"<header><h1 diff:whitespace=""normalize""><em> foo   bar </em></h1></header>", @"<header><h1><em>foo bar</em></h1></header>")]
        [InlineData(@"<header diff:whitespace=""normalize""><h1><em> foo   bar </em></h1></header>", @"<header><h1><em>foo bar</em></h1></header>")]
        public void Test001(string controlHtml, string testHtml)
        {
            var sut = new TextNodeComparer(WhitespaceOption.Preserve);
            var controlSource = new ComparisonSource(ToNode(controlHtml).FirstChild.FirstChild.FirstChild, 0, "dummypath", ComparisonSourceType.Control);
            var testSource = new ComparisonSource(ToNode(testHtml).FirstChild.FirstChild.FirstChild, 0, "dummypath", ComparisonSourceType.Test);
            var comparison = new Comparison(controlSource, testSource);

            sut.Compare(comparison, CompareResult.Different).ShouldBe(CompareResult.Same);
        }

        [Theory(DisplayName = "When whitespace option is Preserve or RemoveWhitespaceNodes, a string ordinal comparison is performed")]
        [InlineData(WhitespaceOption.Preserve)]
        [InlineData(WhitespaceOption.RemoveWhitespaceNodes)]
        public void Test003(WhitespaceOption whitespaceOption)
        {
            var sut = new TextNodeComparer(whitespaceOption);
            var comparison = new Comparison(ToComparisonSource("  hello\n\nworld ", ComparisonSourceType.Control),
                                            ToComparisonSource("  hello\n\nworld ", ComparisonSourceType.Test));

            sut.Compare(comparison, CompareResult.Different).ShouldBe(CompareResult.Same);
        }

        [Fact(DisplayName = "When IgnoreCase is true, a string ordinal ignore case comparison is performed")]
        public void Test004()
        {
            var sut = new TextNodeComparer(ignoreCase: true);
            var comparison = new Comparison(ToComparisonSource("HELLO WoRlD", ComparisonSourceType.Control),
                                            ToComparisonSource("hello world", ComparisonSourceType.Test));
            
            sut.Compare(comparison, CompareResult.Different).ShouldBe(CompareResult.Same);
        }

        [Fact(DisplayName = "When the parent element is <pre>, the is implicitly set to Preserve")]
        public void Test005()
        {
            var sut = new TextNodeComparer(WhitespaceOption.Normalize);
            var pre = ToComparisonSource("<pre>foo   bar</pre>");
            var controlSource = new ComparisonSource(pre.Node.FirstChild, 0, pre.Path, ComparisonSourceType.Control);
            var testSource = ToComparisonSource("foo bar", ComparisonSourceType.Test);
            var comparison = new Comparison(controlSource, testSource);

            sut.Compare(comparison, CompareResult.Different).ShouldBe(CompareResult.Different);
        }

        [Fact(DisplayName = "When the parent element is <pre> and the whitespace option is set inline, the inline option is used instead of Preserve")]
        public void Test006()
        {
            var sut = new TextNodeComparer(WhitespaceOption.Normalize);
            var pre = ToComparisonSource("<pre diff:whitespace=\"normalize\">foo   bar</pre>");
            var controlSource = new ComparisonSource(pre.Node.FirstChild, 0, pre.Path, ComparisonSourceType.Control);
            var testSource = ToComparisonSource("foo bar", ComparisonSourceType.Test);
            var comparison = new Comparison(controlSource, testSource);

            sut.Compare(comparison, CompareResult.Different).ShouldBe(CompareResult.Same);
        }

        [Fact(DisplayName = "When IgnoreCase='true' inline attribute is present in a parent element, a string ordinal ignore case comparison is performed")]
        public void Test007()
        {
            var sut = new TextNodeComparer(ignoreCase: false);            
            var pre = ToComparisonSource("<h1 diff:ignoreCase=\"True\">HELLO WoRlD</pre>");
            var controlSource = new ComparisonSource(pre.Node.FirstChild, 0, pre.Path, ComparisonSourceType.Control);
            var testSource = ToComparisonSource("hello world", ComparisonSourceType.Test);
            var comparison = new Comparison(controlSource, testSource);


            sut.Compare(comparison, CompareResult.Different).ShouldBe(CompareResult.Same);
        }


        // When diff:regex attribute is found on the containing element, the control text is expected to a regex and that used when comparing to the test text node.
    }
}


