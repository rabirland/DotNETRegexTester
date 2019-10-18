using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace RegexTest
{
    class RegexInfo
    {
        public static readonly char[] EscapedAncors = { 'A', 'Z', 'z', 'G', 'B', 'b' };
        public static readonly char[] SingularAnchors = { '^', '$' };
        public static readonly char[] EscapeMetacharacters = { 'D', 'd', 'w', 'W', 's', 'S' };
        public static readonly char[] SingularMetacharacters = { '|', '?', '*', '+', '.' };
        public static readonly char[] EscapeSpecialCharacters = { 'a', 'b', 't', 'r', 'v', 'f', 'n', 'e' };

        public static readonly char[] HexDigits = "0123456789abcdefABCDEF".ToCharArray();
        public static readonly char[] DecimalDigits = "0123456789".ToCharArray();
        public static readonly char[] Letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        public static readonly char[] RegexOptions = "imsnxIMSNX".ToCharArray();

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
