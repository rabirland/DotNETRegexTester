using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RegexTest
{
    public partial class Form1 : Form
    {
        const int InnerMargin = 10;
        const int CheckboxWidth = 150;

        // Common controls
        RichTextBox _inputTextBox;
        TreeView _resultTreeView;
        RichTextBox _patternTextBox;
        Label _errorLabel;
        CheckBox _ignoreCaseCheckbox;
        CheckBox _multilineCheckbox;
        CheckBox _explicitCaptureCheckbox;
        CheckBox _singleLineCheckbox;
        CheckBox _ignorePatternWhitespaceCheckbox;
        CheckBox _rightToLeftCheckbox;
        CheckBox _ecmaScriptCheckbox;
        CheckBox _cultureInvariantCheckbox;

        Regex _patternRegex;
        private RegexOptions _regexOptions = RegexOptions.CultureInvariant;
        bool handlingHighlight = false;

        public Form1()
        {
            InitializeComponent();

            this.DoubleBuffered = true;

            // VS Winform designer not working, create the layout here
            var mainSplitContanier = new SplitContainer();
            mainSplitContanier.Orientation = Orientation.Horizontal;
            mainSplitContanier.SplitterWidth = 5;
            mainSplitContanier.Dock = DockStyle.Fill;
            this.Controls.Add(mainSplitContanier);

            // Bottom panel
            var bottomPanelLabel = new Label();
            bottomPanelLabel.Text = "Input text";
            bottomPanelLabel.Font = new Font("Arial", 11f, FontStyle.Bold);
            bottomPanelLabel.Left = 10;
            bottomPanelLabel.Top = 10;
            mainSplitContanier.Panel2.Controls.Add(bottomPanelLabel);

            _inputTextBox = new RichTextBox();
            _inputTextBox.Top = 35;
            _inputTextBox.Left = InnerMargin;
            _inputTextBox.Width = mainSplitContanier.Panel2.Width - CheckboxWidth - InnerMargin - InnerMargin - InnerMargin;
            _inputTextBox.Height = mainSplitContanier.Panel2.Height - _inputTextBox.Top - InnerMargin;
            _inputTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            _inputTextBox.Font = new Font("Consolas", 10f, FontStyle.Bold);
            _inputTextBox.TextChanged += (o, e) =>
            {
                if (!this.handlingHighlight)
                {
                    this.handlingHighlight = true;
                    RescanRegex(RegexRefreshMode.ReparseOnly);
                    this.handlingHighlight = false;
                }
            };
            _inputTextBox.AutoWordSelection = false;
            mainSplitContanier.Panel2.Controls.Add(_inputTextBox);

            // Top Panel
            _ignoreCaseCheckbox = new CheckBox();
            _ignoreCaseCheckbox.Text = "Ignore case";
            _ignoreCaseCheckbox.Left = mainSplitContanier.Panel1.Width - InnerMargin - CheckboxWidth;
            _ignoreCaseCheckbox.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            _ignoreCaseCheckbox.Checked = this._regexOptions.HasFlag(RegexOptions.IgnoreCase);
            _ignoreCaseCheckbox.CheckedChanged += (o, e) =>
            {
                bool c = ((CheckBox)o).Checked;
                if (c) this._regexOptions |= RegexOptions.IgnoreCase;
                else this._regexOptions &= ~RegexOptions.IgnoreCase;
                
                this.RescanRegex(RegexRefreshMode.ReparseOnly);
            };
            mainSplitContanier.Panel1.Controls.Add(_ignoreCaseCheckbox);

            _multilineCheckbox = new CheckBox();
            _multilineCheckbox.Text = "Multiline";
            _multilineCheckbox.Left = _ignoreCaseCheckbox.Left;
            _multilineCheckbox.Top = _ignoreCaseCheckbox.Bottom;
            _multilineCheckbox.Width = CheckboxWidth;
            _multilineCheckbox.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            _multilineCheckbox.Checked = this._regexOptions.HasFlag(RegexOptions.Multiline);
            _multilineCheckbox.CheckedChanged += (o, e) =>
            {
                bool c = ((CheckBox)o).Checked;
                if (c) this._regexOptions |= RegexOptions.Multiline;
                else this._regexOptions &= ~RegexOptions.Multiline;

                this.RescanRegex(RegexRefreshMode.ReparseOnly);
            };
            mainSplitContanier.Panel1.Controls.Add(_multilineCheckbox);

            _explicitCaptureCheckbox = new CheckBox();
            _explicitCaptureCheckbox.Text = "Explicit capture";
            _explicitCaptureCheckbox.Left = _ignoreCaseCheckbox.Left;
            _explicitCaptureCheckbox.Top = _multilineCheckbox.Bottom;
            _explicitCaptureCheckbox.Width = CheckboxWidth;
            _explicitCaptureCheckbox.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            _explicitCaptureCheckbox.Checked = this._regexOptions.HasFlag(RegexOptions.ExplicitCapture);
            _explicitCaptureCheckbox.CheckedChanged += (o, e) =>
            {
                bool c = ((CheckBox)o).Checked;
                if (c) this._regexOptions |= RegexOptions.ExplicitCapture;
                else this._regexOptions &= ~RegexOptions.ExplicitCapture;

                this.RescanRegex(RegexRefreshMode.ReparseOnly);
            };
            mainSplitContanier.Panel1.Controls.Add(_explicitCaptureCheckbox);

            _singleLineCheckbox = new CheckBox();
            _singleLineCheckbox.Text = "Single line";
            _singleLineCheckbox.Left = _ignoreCaseCheckbox.Left;
            _singleLineCheckbox.Top = _explicitCaptureCheckbox.Bottom;
            _singleLineCheckbox.Width = CheckboxWidth;
            _singleLineCheckbox.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            _singleLineCheckbox.Checked = this._regexOptions.HasFlag(RegexOptions.Singleline);
            _singleLineCheckbox.CheckedChanged += (o, e) =>
            {
                bool c = ((CheckBox)o).Checked;
                if (c) this._regexOptions |= RegexOptions.Singleline;
                else this._regexOptions &= ~RegexOptions.Singleline;

                this.RescanRegex(RegexRefreshMode.ReparseOnly);
            };
            mainSplitContanier.Panel1.Controls.Add(_singleLineCheckbox);

            _ignorePatternWhitespaceCheckbox = new CheckBox();
            _ignorePatternWhitespaceCheckbox.Text = "Pattern whitespace";
            _ignorePatternWhitespaceCheckbox.Left = _ignoreCaseCheckbox.Left;
            _ignorePatternWhitespaceCheckbox.Top = _singleLineCheckbox.Bottom;
            _ignorePatternWhitespaceCheckbox.Width = CheckboxWidth;
            _ignorePatternWhitespaceCheckbox.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            _ignorePatternWhitespaceCheckbox.Checked = this._regexOptions.HasFlag(RegexOptions.IgnorePatternWhitespace);
            _ignorePatternWhitespaceCheckbox.CheckedChanged += (o, e) =>
            {
                bool c = ((CheckBox)o).Checked;
                if (c) this._regexOptions |= RegexOptions.IgnorePatternWhitespace;
                else this._regexOptions &= ~RegexOptions.IgnorePatternWhitespace;

                this.RescanRegex(RegexRefreshMode.ReparseOnly);
            };
            mainSplitContanier.Panel1.Controls.Add(_ignorePatternWhitespaceCheckbox);

            _rightToLeftCheckbox = new CheckBox();
            _rightToLeftCheckbox.Text = "Right to left";
            _rightToLeftCheckbox.Left = _ignoreCaseCheckbox.Left;
            _rightToLeftCheckbox.Top = _ignorePatternWhitespaceCheckbox.Bottom;
            _rightToLeftCheckbox.Width = CheckboxWidth;
            _rightToLeftCheckbox.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            _rightToLeftCheckbox.Checked = this._regexOptions.HasFlag(RegexOptions.RightToLeft);
            _rightToLeftCheckbox.CheckedChanged += (o, e) =>
            {
                bool c = ((CheckBox)o).Checked;
                if (c) this._regexOptions |= RegexOptions.RightToLeft;
                else this._regexOptions &= ~RegexOptions.RightToLeft;

                this.RescanRegex(RegexRefreshMode.ReparseOnly);
            };
            mainSplitContanier.Panel1.Controls.Add(_rightToLeftCheckbox);

            _ecmaScriptCheckbox = new CheckBox();
            _ecmaScriptCheckbox.Text = "JavaScript";
            _ecmaScriptCheckbox.Left = _ignoreCaseCheckbox.Left;
            _ecmaScriptCheckbox.Top = _rightToLeftCheckbox.Bottom;
            _ecmaScriptCheckbox.Width = CheckboxWidth;
            _ecmaScriptCheckbox.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            _ecmaScriptCheckbox.Checked = this._regexOptions.HasFlag(RegexOptions.ECMAScript);
            _ecmaScriptCheckbox.CheckedChanged += (o, e) =>
            {
                bool c = ((CheckBox)o).Checked;
                if (c) this._regexOptions |= RegexOptions.ECMAScript;
                else this._regexOptions &= ~RegexOptions.ECMAScript;

                this.RescanRegex(RegexRefreshMode.ReparseOnly);
            };
            mainSplitContanier.Panel1.Controls.Add(_ecmaScriptCheckbox);

            _cultureInvariantCheckbox = new CheckBox();
            _cultureInvariantCheckbox.Text = "Culture invariant";
            _cultureInvariantCheckbox.Checked = true;
            _cultureInvariantCheckbox.Left = _ignoreCaseCheckbox.Left;
            _cultureInvariantCheckbox.Top = _ecmaScriptCheckbox.Bottom;
            _cultureInvariantCheckbox.Width = CheckboxWidth;
            _cultureInvariantCheckbox.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            _cultureInvariantCheckbox.Checked = this._regexOptions.HasFlag(RegexOptions.CultureInvariant);
            _cultureInvariantCheckbox.CheckedChanged += (o, e) =>
            {
                bool c = ((CheckBox)o).Checked;
                if (c) this._regexOptions |= RegexOptions.CultureInvariant;
                else this._regexOptions &= ~RegexOptions.CultureInvariant;

                this.RescanRegex(RegexRefreshMode.ReparseOnly);
            };
            mainSplitContanier.Panel1.Controls.Add(_cultureInvariantCheckbox);

            var topSplitContainer = new SplitContainer();
            topSplitContainer.Top = 0;
            topSplitContainer.Left = 0;
            topSplitContainer.Height = mainSplitContanier.Panel1.Height;
            topSplitContainer.Width = mainSplitContanier.Panel1.Width - CheckboxWidth;
            topSplitContainer.Anchor = AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom;
            topSplitContainer.Orientation = Orientation.Horizontal;
            topSplitContainer.SplitterDistance = topSplitContainer.Height - 100;
            mainSplitContanier.Panel1.Controls.Add(topSplitContainer);

            var resultsLabel = new Label();
            resultsLabel.Text = "Results";
            resultsLabel.Font = new Font("Arial", 11f, FontStyle.Bold);
            resultsLabel.Left = InnerMargin;
            resultsLabel.Top = InnerMargin;
            topSplitContainer.Panel1.Controls.Add(resultsLabel);

            var collapseAllButton = new Button();
            collapseAllButton.Text = "Collapse all";
            collapseAllButton.Left = resultsLabel.Right + InnerMargin;
            collapseAllButton.Top = resultsLabel.Top;
            collapseAllButton.Click += (o, s) => this._resultTreeView.CollapseAll();
            topSplitContainer.Panel1.Controls.Add(collapseAllButton);

            var expandAllButton = new Button();
            expandAllButton.Text = "Expand all";
            expandAllButton.Left = collapseAllButton.Right + InnerMargin;
            expandAllButton.Top = collapseAllButton.Top;
            expandAllButton.Click += (o, s) => this._resultTreeView.ExpandAll();
            topSplitContainer.Panel1.Controls.Add(expandAllButton);

            _resultTreeView = new TreeView();
            _resultTreeView.Top = resultsLabel.Bottom;
            _resultTreeView.Left = InnerMargin;
            _resultTreeView.Height = topSplitContainer.Panel1.Height - InnerMargin - InnerMargin;
            _resultTreeView.Width = topSplitContainer.Panel1.Width - InnerMargin - InnerMargin - InnerMargin;
            _resultTreeView.Anchor = AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom;
            topSplitContainer.Panel1.Controls.Add(_resultTreeView);

            var patternLabel = new Label();
            patternLabel.Text = "Pattern";
            patternLabel.Font = new Font("Arial", 11f, FontStyle.Bold);
            patternLabel.Left = InnerMargin;
            patternLabel.Top = InnerMargin;
            topSplitContainer.Panel2.Controls.Add(patternLabel);

            _errorLabel = new Label();
            _errorLabel.Font = new Font("Arial", 8f, FontStyle.Bold);
            _errorLabel.Top = patternLabel.Bottom - _errorLabel.Height;
            _errorLabel.Left = patternLabel.Right + InnerMargin;
            _errorLabel.Width = topSplitContainer.Panel2.Width - _errorLabel.Left - InnerMargin;
            _errorLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            topSplitContainer.Panel2.Controls.Add(_errorLabel);

            _patternTextBox = new RichTextBox();
            _patternTextBox.Top = patternLabel.Bottom;
            _patternTextBox.Left = InnerMargin;
            _patternTextBox.Height = topSplitContainer.Panel1.Height - InnerMargin - InnerMargin;
            _patternTextBox.Width = topSplitContainer.Panel1.Width - InnerMargin - InnerMargin - InnerMargin;
            _patternTextBox.Anchor = AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom;
            _patternTextBox.Font = new Font("Consolas", 10f, FontStyle.Bold);
            _patternTextBox.TextChanged += (o, e) =>
            {
                if (!this.handlingHighlight)
                {
                    this.handlingHighlight = true;
                    RescanRegex(RegexRefreshMode.Rehighlight);
                    this.handlingHighlight = false;
                }
                
            };
            _patternTextBox.AutoWordSelection = false;
            topSplitContainer.Panel2.Controls.Add(_patternTextBox);
        }

        private void RescanRegex(RegexRefreshMode mode)
        {
            this._resultTreeView.Nodes.Clear();

            List<HighlightRange> patternRanges = null;
            List<HighlightRange> inputRanges = null;
            string error = null;
            MatchCollection matches = null;

            string pattern = this._patternTextBox.Text;
            string input = this._inputTextBox.Text;
            Task.Run(async () =>
            {
                if (mode >= RegexRefreshMode.ReparseOnly)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(pattern))
                        {
                            this._patternRegex = new Regex(pattern, this._regexOptions);
                        }
                        else
                        {
                            this._patternRegex = null;
                        }
                    }
                    catch (Exception e)
                    {
                        error = e.Message;
                        this._patternRegex = null;
                    }
                }

                Task patternHighlightTask = null;
                Task inputHighlightTask = null;

                if (this._patternRegex != null)
                {
                    if (mode == RegexRefreshMode.Rehighlight)
                    {
                        patternHighlightTask = Task.Run(() => patternRanges = this.HighlightPattern(pattern));
                    }

                    inputHighlightTask = Task.Run(() =>
                    {
                        matches = this._patternRegex.Matches(input);
                        inputRanges = this.HighlightInput(matches);
                    });
                }

                if (patternHighlightTask != null)
                {
                    await patternHighlightTask;
                }
               
                if (inputHighlightTask != null)
                {
                    await inputHighlightTask;
                }
            }).Wait();

            if (this._patternRegex != null && mode == RegexRefreshMode.Rehighlight) // Highlight the pattern text
            {
                this._errorLabel.Text = string.Empty;

                int ss = this._patternTextBox.SelectionStart;
                int sl = this._patternTextBox.SelectionLength;

                this._patternTextBox.SelectAll();
                this._patternTextBox.SelectionColor = Color.Black;

                foreach (var range in patternRanges)
                {
                    this._patternTextBox.Select(range.Start, range.Length);
                    this._patternTextBox.SelectionColor = range.Color;
                }
                this._patternTextBox.Select(ss, sl);
                this._patternTextBox.SelectionColor = Color.Black;
            }
            else if (this._patternRegex == null && !string.IsNullOrEmpty(error)) // Color the whole pattern text to red
            {
                int ss = this._patternTextBox.SelectionStart;
                int sl = this._patternTextBox.SelectionLength;
                this._patternTextBox.SelectAll();
                this._patternTextBox.SelectionColor = Color.Red;
                this._patternTextBox.Select(ss, sl);
            }

            // Remove highlight from input text
            int currentSelectionStart = this._inputTextBox.SelectionStart;
            int currentSelectionLength = this._inputTextBox.SelectionLength;
            this._inputTextBox.SelectAll();
            this._inputTextBox.SelectionColor = Color.Black;
            this._inputTextBox.SelectionBackColor = Color.White;
            this._inputTextBox.SelectionStart = currentSelectionStart;
            this._inputTextBox.SelectionLength = currentSelectionLength;

            // Highlight input text
            if (inputRanges != null)
            {
                int ss = this._inputTextBox.SelectionStart;
                int sl = this._inputTextBox.SelectionLength;
                foreach (var range in inputRanges)
                {
                    this._inputTextBox.Select(range.Start, range.Length);
                    this._inputTextBox.SelectionBackColor = range.Color;
                }
                this._inputTextBox.Select(ss, sl);
                this._inputTextBox.SelectionColor = Color.Black;
            }

            // Refresh the tree view
            this._resultTreeView.Nodes.Clear();
            if (matches != null)
            {
                int i = 0;
                foreach (var m in matches)
                {
                    var match = (Match)m;
                    TreeNode matchNode = new TreeNode($"Match {i + 1}: {match.Value}");

                    foreach (var g in match.Groups)
                    {
                        var group = (Group)g;

                        TreeNode groupNode = new TreeNode($"{group.Name}: \"{group.Value}\"");
                        matchNode.Nodes.Add(groupNode);
                    }

                    this._resultTreeView.Nodes.Add(matchNode);
                }
                this._resultTreeView.ExpandAll();
            }
        }

        private List<HighlightRange> HighlightInput(MatchCollection matches)
        {
            List<HighlightRange> ret = new List<HighlightRange>();
            if (this._patternRegex != null)
            {
                bool flag = false;
                foreach (var match in matches)
                {
                    var m = (Match)match;
                    flag = !flag;
                    ret.Add(new HighlightRange(flag ? Color.Yellow : Color.Orange, m.Index, m.Length));
                }
            }

            return ret;
        }

        private List<HighlightRange> HighlightPattern(string pattern)
        {
            List<HighlightRange> ret = new List<HighlightRange>();

            for (int i = 0; i < pattern.Length; i++)
            {
                int length = 0;
                Color color = Color.Black;

                char current = pattern[i];

                if (current == '\\') // Escape or Escape Anchor or Escape Metacharacter
                {
                    length++; // The escape character
                    char nextChar = pattern[i + 1];

                    if (RegexInfo.EscapedAncors.Contains(nextChar)) // Escaped Anchor
                    {
                        color = RegexInfo.AnchorColor;
                        length++;
                    }
                    else if (RegexInfo.EscapeMetacharacters.Contains(nextChar)) // Escaped Metachar
                    {
                        color = RegexInfo.MetacharacterColor;
                        length++;
                    }
                    else if (this.PatternMatch(pattern, i + 1, RegexInfo.DecimalDigits, RegexInfo.DecimalDigits, RegexInfo.DecimalDigits)) // \123 (octal)
                    {
                        color = RegexInfo.SpecialCharacterEncodeColor;
                        length += 4;
                    }
                    else if (this.PatternMatch(pattern, i + 1, 'x', RegexInfo.HexDigits, RegexInfo.HexDigits)) // Hex coded character
                    {
                        color = RegexInfo.SpecialCharacterEncodeColor;
                        length += 3;
                    }
                    else if (this.PatternMatch(pattern, i + 1, 'c', RegexInfo.Letters)) // Followed by an ASCII control character's letter
                    {
                        color = RegexInfo.SpecialCharacterEncodeColor;
                        length += 2;
                    }
                    else if (this.PatternMatch(pattern, i + 1, 'u', RegexInfo.HexDigits, RegexInfo.HexDigits, RegexInfo.HexDigits, RegexInfo.HexDigits)) // \u 1234 Unicode character
                    {
                        color = RegexInfo.SpecialCharacterEncodeColor;
                        length += 5;
                    }
                    else if (nextChar == 'p') // \p{name} Name of unicode character group or unicode block
                    {
                        color = RegexInfo.SpecialCharacterEncodeColor;
                        int nameLength = this.CountParenthesis(pattern, i + 2, '{', '}');
                        length = nameLength + 1; // +1 for the 'P'
                    }
                    else if (nextChar == 'P') // \p{name} Name of unicode character group or unicode block to NOT match
                    {
                        color = RegexInfo.SpecialCharacterEncodeColor;
                        int nameLength = this.CountParenthesis(pattern, i + 2, '{', '}');
                        length = nameLength + 1; // +1 for the 'P'
                    }
                    else if (nextChar == 'k') // \k<name> Named backreference
                    {
                        color = RegexInfo.GroupingColor;
                        int nameLength = this.CountParenthesis(pattern, i + 2, '<', '>');
                        length = nameLength + 1; // +1 for the 'k'
                    }
                    else // Simple escaped character
                    {
                        color = RegexInfo.EscapedCharacterEncodeColor;
                        length++;
                    }
                }
                else if (RegexInfo.SingularAnchors.Contains(current)) // Singular anchor
                {
                    color = RegexInfo.AnchorColor;
                    length++;
                }
                else if (RegexInfo.SingularMetacharacters.Contains(current)) // Singular Metachar
                {
                    color = RegexInfo.MetacharacterColor;
                    length++;
                }
                else if (current == '{') // Quantifiers
                {
                    /*
                     * {n}
                     * {n,c}
                     * {n,}
                     */
                    int end = pattern.IndexOf('}', i + 1);
                    color = RegexInfo.QuantifierColor;
                    length += (end - i) + 1;
                }
                else if (current == '(') // Group start
                {
                    /*
                     * Group types:
                     * (                Simple numbered group
                     * (?:              Non capturing group
                     * (?<Name>         Named group
                     * (?'Name'         Named group
                     * (?=              Positive lookahead
                     * (?!              Negative lookahead
                     * (?imnsx-imnsx:   Disables options in subexpression
                     * (?>              Non-backtracking (greedy) subexpression
                     * (?(xyz)yes|no)   If 'xyz' group has a match, matches 'yes' otherwise matches 'no'
                     */

                    length++; // The '(' character
                    color = RegexInfo.GroupingColor;
                    char next = pattern[i + 1];
                    if (next == '?')
                    {
                        length++; // The '?' character
                        char after = pattern[i + 2];
                        if (after == ':') // Non capturing group
                        {
                            length++; // The ':' character
                        }
                        else if(after == '<') // Named group
                        {
                            length += this.CountParenthesis(pattern, i + 2, '<', '>');
                        }
                        else if (after == '\'') // Named group
                        {
                            length += this.CountParenthesis(pattern, i + 2, '\'', '\'');
                        }
                        else if (after == '=') // Positive lookahead
                        {
                            length++;
                        }
                        else if (after == '!') // Negative lookahead
                        {
                            length++;
                        }
                        else if (after == '>') // Greedy subexpression
                        {
                            length++;
                        }
                        else if (after == '(') // Conditinal
                        {
                            length += this.CountParenthesis(pattern, i + 2, '(', ')');
                        }
                        else if (RegexInfo.RegexOptions.Contains(after)) // Subexpression options
                        {
                            int end = pattern.IndexOf(':', i + 1);
                            length = (end - i) + 1;
                        }
                    }
                    else // Simple numbered group
                    {
                        // No additinal character have to be marked
                    }
                }
                else if (current == ')')
                {
                    length++;
                    color = RegexInfo.GroupingColor;
                }
                else if (current == '[') // Character group
                {
                    /*
                     * Character groups:
                     * [...]
                     * [^...]
                     */
                    color = RegexInfo.CharacterGroupColor;
                    int end = pattern.IndexOf(']', i + 1);
                    length += (end - i) + 1;
                }

                if (length > 0)
                {
                    ret.Add(new HighlightRange(color, i, length));
                    i += length - 1;
                }
            }

            return ret;
        }

        private bool PatternMatch(string input, int index, params object[] patternElements)
        {
            if (index + patternElements.Length <= input.Length)
            {
                for (int i = 0; i < patternElements.Length; i++)
                {
                    object obj = patternElements[i];
                    char current = input[index + i];
                    switch(obj)
                    {
                        case char ch: 
                            if (current != ch)
                            {
                                return false;
                            }
                            break;
                        case string str: 
                            if (!str.Contains(current))
                            {
                                return false;
                            }
                            break;
                        case char[] chars:
                            if (!chars.Contains(current))
                            {
                                return false;
                            }
                            break;
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        private int CountParenthesis(string input, int index, char open, char close)
        {
            if (input[index] == open)
            {
                int level = 0;
                for (int i = index + 1; i < input.Length; i++)
                {
                    if (input[i] == open && open != close)
                    {
                        level++;
                    }
                    else if (input[i] == close)
                    {
                        if (level == 0)
                        {
                            return (i - index) + 1; // +1 for the open and close parenthesis
                        }
                        else
                        {
                            level--;
                        }
                    }
                }

                return -1;
            }
            else
            {
                return 0;
            }
        }

        private struct HighlightRange
        {
            public Color Color { get; }

            public int Start { get; }

            public int Length { get; }

            public HighlightRange(Color color, int start, int length)
            {
                this.Color = color;
                this.Start = start;
                this.Length = length;
            }
        }

        private enum RegexRefreshMode
        {
            None = 0,
            ReparseOnly = 1,
            Rehighlight = 2
        }
    }
}
