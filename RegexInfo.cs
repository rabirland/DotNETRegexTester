using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;

namespace RegexTest
{
    class RegexInfo
    {
        public static readonly Regex GroupNameRegex = new Regex(@"\(\?(?:\<(?<Name>\w+)\>|\'(?<Name>\w+)\')", System.Text.RegularExpressions.RegexOptions.Compiled);

        public static readonly char[] EscapedAncors = { 'A', 'Z', 'z', 'G', 'B', 'b' };
        public static readonly char[] SingularAnchors = { '^', '$' };
        public static readonly char[] EscapeMetacharacters = { 'D', 'd', 'w', 'W', 's', 'S' };
        public static readonly char[] SingularMetacharacters = { '|', '?', '*', '+', '.' };
        public static readonly char[] EscapeSpecialCharacters = { 'a', 'b', 't', 'r', 'v', 'f', 'n', 'e' };

        public static readonly char[] HexDigits = "0123456789abcdefABCDEF".ToCharArray();
        public static readonly char[] DecimalDigits = "0123456789".ToCharArray();
        public static readonly char[] Letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        public static readonly char[] RegexOptions = "imsnxIMSNX".ToCharArray();

        // Suggestion helpers
        public static readonly IReadOnlyDictionary<char, string> EscapedCharacterDescriptions = new ReadOnlyDictionary<char, string>(new Dictionary<char, string>()
        {
            // Anchors
            { 'A', "Beginning of string only (no multiline)" },
            { 'Z', "End of string or before \\n" },
            { 'z', "End of string only" },
            { 'G', "Match start at end of previous match" },
            { 'b', "Word boundary" },
            { 'B', "Not word boundary" },

            // Escaped characters
            { 'a', "Bell character \\u0007" },
            //{ 'b', "Backspace (in character class)" },
            { 't', "Tab" },
            { 'r', "Carriage return" },
            { 'v', "Vertical tab" },
            { 'f', "Form feed" },
            { 'n', "New line" },
            { 'e', "Escape" },

            // Coded Character prefixes
            { 'x', "Hex coded character \\xAB" },
            { 'c', "Ascii control character \\cA" },
            { 'u', "Unicode character \\u1234" },
            { '\\', "Backslash" },

            // Meta Characters
            { 'w', "Word character" },
            { 'W', "Non-word character" },
            { 's', "Whitespace character" },
            { 'S', "Non-whitespace character" },
            { 'd', "Decimal digit" },
            { 'D', "Not decimal digit" },

            // Classes
            { 'p', "Unicode general category \\p{group_name}" },
            { 'P', "Not in unicode general category \\P{group_name}" },
        });

        // Grouping infos
        public const string NamedGroup1Description = "(?<Name>...) - Named Group";
        public const string NamedGroup2Description = "(?'Name'...) - Named Group";
        public const string PositiveLookaheadDescription = "(?=...) - Positive Lookahead";
        public const string NegativeLookaheadDescription = "(?!...) - Positive Lookahead";
        public const string ZeroWidthPositiveLookaheadDescription = "(?<=...) - Positive Lookahead";
        public const string ZeroWidthNegativeLookaheadDescription = "(?<!...) - Positive Lookahead";
        public const string GreedySubexpressionDescription = "(?>...) - Greedy subexpression";
        public const string ConditionalGroupDescription = "(?(...)...) - Conditional subexpression";
        public const string SimpleGroupDescription = "(...) - Group";
        public const string NonCaptureGroupDescription = "(?:...) - Non-capture group";
        public const string SubexpressionOptionsDescription = "(?imnsx-imnsx:...) - Subexpression options";
        public const string BalancingGroupDescription = "(?<Name1-Name2>...) - Balancing Group";

        public static string[] GroupingDescriptions = 
        {
            NamedGroup1Description,
            NamedGroup2Description,
            PositiveLookaheadDescription,
            NegativeLookaheadDescription,
            ZeroWidthPositiveLookaheadDescription,
            ZeroWidthNegativeLookaheadDescription,
            GreedySubexpressionDescription,
            ConditionalGroupDescription,
            SimpleGroupDescription,
            NonCaptureGroupDescription,
            SubexpressionOptionsDescription,
            BalancingGroupDescription
        };

        // Quantifier infos
        public const string FixedLengthDescription = "{n} - Exact amount";
        public const string MinimumLengthDescription = "{n,} - Minimum amount";
        public const string BetweenLengthDescription = "{n,m} - Amount between n and m";

        public static string[] QuantifierDescriptions =
        {
            FixedLengthDescription,
            MinimumLengthDescription,
            BetweenLengthDescription,
        };

        private const string CharacterGroup = "[...] - Character in group";
        private const string NotCharacterGroup = "[^...] - Character NOT in group";

        public static string[] CharacterGroupDescriptions =
        {
            CharacterGroup,
            NotCharacterGroup,
        };

        // Colors
        public static readonly Color MetacharacterColor = Color.Green;
        public static readonly Color AnchorColor = Color.Orange;
        public static readonly Color GroupingColor = Color.Blue;
        public static readonly Color CharacterGroupColor = Color.Brown;
        public static readonly Color SpecialCharacterEncodeColor = Color.DarkGoldenrod;
        public static readonly Color EscapedCharacterEncodeColor = Color.DarkCyan;
        public static readonly Color QuantifierColor = Color.Red;
    }
}
