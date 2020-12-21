using System;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Text
{
    public class StreamTokenizer
    {
        private TextReader reader;
        private Predicate<char> isDelim;
        private CultureInfo cultureInfo;

        /** A constant indicating that the end of the stream has been readd */
        public const int TT_EOF = -1;

        /** A constant indicating that the end of the line has been readd */
        public const int TT_EOL = '\n';

        /** A constant indicating that a number token has been readd */
        public const int TT_NUMBER = -2;

        /** A constant indicating that a word token has been readd */
        public const int TT_WORD = -3;

        /** A constant indicating that no tokens have been read yetd */
        private const int TT_NONE = -4;

        /**
         * Contains the type of the token read resulting from a call to nextToken
         * The rules are as follows:
         * <ul>
         * <li>For a token consisting of a single ordinary character, this is the 
         *     value of that character.</li>
         * <li>For a quoted string, this is the value of the quote character</li>
         * <li>For a word, this is TT_WORD</li>
         * <li>For a number, this is TT_NUMBER</li>
         * <li>For the end of the line, this is TT_EOL</li>
         * <li>For the end of the stream, this is TT_EOF</li>
         * </ul>
         */
        public int ttype = TT_NONE;

        /** The String associated with word and string tokensd */
        private String _sval;

        /** The numeric value associated with number tokensd */
        private double _nval;

        /* Indicates whether end-of-line is recognized as a tokend */
        private Boolean _eolSignificant = false;

        /* Indicates whether word tokens are automatically made lower cased */
        private Boolean _lowerCase = false;

        /* Indicates whether C++ style comments are recognized and skippedd */
        private Boolean _slashSlash = false;

        /* Indicates whether C style comments are recognized and skippedd */
        private Boolean _slashStar = false;

        /* Attribute tables of each byte from 0x00 to 0xFFd */
        private Boolean[] whitespace = new Boolean[256];
        private Boolean[] alphabetic = new Boolean[256];
        private Boolean[] numeric = new Boolean[256];
        private Boolean[] quote = new Boolean[256];
        private Boolean[] comment = new Boolean[256];

        /* The Reader associated with this classd */
        private PushbackReader inRead;

        /* Indicates if a token has been pushed backd */
        private Boolean pushedBack = false;

        /* Contains the current line number of the readerd */
        private int lineNumber = 1;

        #region Properties
        /** The String associated with word and string tokensd */
        public String Sval
        {
            get { return _sval; }
            set { _sval = value; }
        }

        /** The numeric value associated with number tokensd */
        public double Nval
        {
            get { return _nval; }
            set { _nval = value; }
        }

        /// <summary>
        /// This method gets/sets a flag that indicates whether or not the end of line
        /// sequence terminates and is a tokend  The defaults to <code>false</code>
        /// </summary>
        public Boolean EolSignificant
        {
            get { return _eolSignificant; }
            set { _eolSignificant = value; }
        }

        /// <summary>
        /// This method gets/sets a flag that indicates whether or not alphabetic
        /// tokens that are returned should be converted to lower case.
        /// </summary>
        public Boolean LowerCase
        {
            get { return _lowerCase; }
            set { _lowerCase = value; }
        }

        /// <summary>
        /// This method sets a flag that indicates whether or not "C++" language style
        /// comments ("//" comments through EOL ) are handled by the parser.
        /// If this is <code>true</code> commented out sequences are skipped and
        /// ignored by the parserd  This defaults to <code>false</code>.
        /// </summary>
        public Boolean SlashSlash
        {
            get { return _slashSlash; }
            set { _slashSlash = value; }
        }

        /// <summary>
        /// This method sets a flag that indicates whether or not "C" language style
        /// comments (with nesting not allowed) are handled by the parser.
        /// If this is <code>true</code> commented out sequences are skipped and
        /// ignored by the parserd  This defaults to <code>false</code>.
        /// </summary>
        public Boolean SlashStar
        {
            get { return _slashStar; }
            set { _slashStar = value; }
        }

        public int LineNumber
        {
            get { return lineNumber; }
            set { lineNumber = value; }
        }
        #endregion


        public StreamTokenizer(TextReader reader) : this(reader, new[] { ' ', '\r', '\n', '\t' }, CultureInfo.CurrentCulture)
        {

        }

        public StreamTokenizer(TextReader reader, Predicate<char> isDelim, CultureInfo cultureInfo)
        {
            this.reader = reader;
            this.isDelim = isDelim;
            this.cultureInfo = cultureInfo;
        }

        public StreamTokenizer(TextReader reader, char[] delims, CultureInfo cultureInfo)
        {
            this.reader = reader;
            this.isDelim = c => Array.IndexOf(delims, c) >= 0;
            this.cultureInfo = cultureInfo;
        }

        public TextReader BaseReader
        {
            get { return reader; }
        }

        public T ReadToken<T>()
        {
            StringBuilder sb = new StringBuilder();
            while (true)
            {
                int c = reader.Peek();
                if (c < 0 || isDelim((char)c))
                {
                    break;
                }
                sb.Append((char)reader.Read());
            }
            return (T)Convert.ChangeType(sb.ToString(), typeof(T));
        }


        private Boolean isWhitespace(int ch)
        {
            return (ch >= 0 && ch <= 255 && whitespace[ch]);
        }

        private Boolean isAlphabetic(int ch)
        {
            return ((ch > 255) || (ch >= 0 && alphabetic[ch]));
        }

        private Boolean isNumeric(int ch)
        {
            return (ch >= 0 && ch <= 255 && numeric[ch]);
        }

        private Boolean isQuote(int ch)
        {
            return (ch >= 0 && ch <= 255 && quote[ch]);
        }

        private Boolean isComment(int ch)
        {
            return (ch >= 0 && ch <= 255 && comment[ch]);
        }

        public int nextToken()
        {
            if (pushedBack)
            {
                pushedBack = false;
                if (ttype != TT_NONE)
                    return ttype;
            }

            _sval = null;
            int ch;

            // Skip whitespaced  Deal with EOL along the way.
            while (isWhitespace(ch = inRead.Read()))
                if (ch == '\n' || ch == '\r')
                {
                    lineNumber++;

                    // Throw away \n if in combination with \r.
                    if (ch == '\r' && (ch = inRead.Read()) != '\n')
                    {
                        if (ch != TT_EOF)
                            inRead = inRead.Unread();
                    }
                    if (_eolSignificant)
                        return (ttype = TT_EOL);
                }

            if (ch == '/')
                if ((ch = inRead.Read()) == '/' && _slashSlash)
                {
                    while ((ch = inRead.Read()) != '\n' && ch != '\r' && ch != TT_EOF)
                        ;

                    if (ch != TT_EOF)
                        inRead = inRead.Unread();
                    return nextToken(); // Recursive, but not too deep in normal cases
                }
                else if (ch == '*' && _slashStar)
                {
                    while (true)
                    {
                        ch = inRead.Read();
                        if (ch == '*')
                        {
                            if ((ch = inRead.Read()) == '/')
                                break;
                            else if (ch != TT_EOF)
                                inRead = inRead.Unread();
                        }
                        else if (ch == '\n' || ch == '\r')
                        {
                            lineNumber++;
                            if (ch == '\r' && (ch = inRead.Read()) != '\n')
                            {
                                if (ch != TT_EOF)
                                    inRead = inRead.Unread();
                            }
                        }
                        else if (ch == TT_EOF)
                        {
                            break;
                        }
                    }
                    return nextToken(); // Recursive, but not too deep in normal cases
                }
                else
                {
                    if (ch != TT_EOF)
                        inRead = inRead.Unread();
                    ch = '/';
                }

            if (ch == TT_EOF)
                ttype = TT_EOF;
            else if (isNumeric(ch))
            {
                Boolean isNegative = false;
                if (ch == '-')
                {
                    // Read ahead to see if this is an ordinary '-' rather than numeric.
                    ch = inRead.Read();
                    if (isNumeric(ch) && ch != '-')
                    {
                        isNegative = true;
                    }
                    else
                    {
                        if (ch != TT_EOF)
                            inRead = inRead.Unread();
                        return (ttype = '-');
                    }
                }

                StringBuilder tokbuf = new StringBuilder();
                tokbuf.Append((char)ch);

                int decCount = 0;
                while (isNumeric(ch = inRead.Read()) && ch != '-')
                    if (ch == '.' && decCount++ > 0)
                        break;
                    else
                        tokbuf.Append((char)ch);

                if (ch != TT_EOF)
                    inRead = inRead.Unread();
                ttype = TT_NUMBER;
                try
                {
                    _nval = Double.Parse(tokbuf.ToString());
                }
                catch (FormatException fex)
                {
                    Console.Write(fex.Message);
                    _nval = 0.0;
                }
                if (isNegative)
                    _nval = -_nval;
            }
            else if (isAlphabetic(ch))
            {
                StringBuilder tokbuf = new StringBuilder();
                tokbuf.Append((char)ch);
                while (isAlphabetic(ch = inRead.Read()) || isNumeric(ch))
                    tokbuf.Append((char)ch);
                if (ch != TT_EOF)
                    inRead = inRead.Unread();
                ttype = TT_WORD;
                _sval = tokbuf.ToString();
                if (_lowerCase)
                    _sval = _sval.ToLower();
            }
            else if (isComment(ch))
            {
                while ((ch = inRead.Read()) != '\n' && ch != '\r' && ch != TT_EOF)
                    ;

                if (ch != TT_EOF)
                    inRead = inRead.Unread();
                return nextToken();    // Recursive, but not too deep in normal cases.
            }
            else if (isQuote(ch))
            {
                ttype = ch;
                StringBuilder tokbuf = new StringBuilder();
                while ((ch = inRead.Read()) != ttype && ch != '\n' && ch != '\r' &&
                       ch != TT_EOF)
                {
                    if (ch == '\\')
                        switch (ch = inRead.Read())
                        {
                            case 'a':
                                ch = 0x7;
                                break;
                            case 'b':
                                ch = '\b';
                                break;
                            case 'f':
                                ch = 0xC;
                                break;
                            case 'n':
                                ch = '\n';
                                break;
                            case 'r':
                                ch = '\r';
                                break;
                            case 't':
                                ch = '\t';
                                break;
                            case 'v':
                                ch = 0xB;
                                break;
                            case '\n':
                                ch = '\n';
                                break;
                            case '\r':
                                ch = '\r';
                                break;
                            case '\"':
                                break;
                            case '\'':
                                break;
                            case '\\':
                                break;
                            default:
                                int ch1, nextch;
                                if ((nextch = ch1 = ch) >= '0' && ch <= '7')
                                {
                                    ch -= '0';
                                    if ((nextch = inRead.Read()) >= '0' && nextch <= '7')
                                    {
                                        ch = ch * 8 + nextch - '0';
                                        if ((nextch = inRead.Read()) >= '0' && nextch <= '7' &&
                                        ch1 >= '0' && ch1 <= '3')
                                        {
                                            ch = ch * 8 + nextch - '0';
                                            nextch = inRead.Read();
                                        }
                                    }
                                }

                                if (nextch != TT_EOF)
                                    inRead = inRead.Unread();

                                break;
                        }

                    tokbuf.Append((char)ch);
                }

                // Throw away matching quote char.
                if (ch != ttype && ch != TT_EOF)
                    inRead = inRead.Unread();

                _sval = tokbuf.ToString();
            }
            else
            {
                ttype = ch;
            }

            return ttype;

        }

        private void resetChar(int ch)
        {
            whitespace[ch] = alphabetic[ch] = numeric[ch] = quote[ch] = comment[ch] =
              false;
        }

        /**
         * This method makes the specified character an ordinary characterd  This
         * means that none of the attributes (whitespace, alphabetic, numeric,
         * quote, or comment) will be set on this characterd  This character will
         * parse as its own token.
         *
         * @param ch The character to make ordinary, passed as an int
         */
        public void ordinaryChar(int ch)
        {
            if (ch >= 0 && ch <= 255)
                resetChar(ch);
        }

        /**
         * This method makes all the characters in the specified range, range
         * terminators included, ordinaryd  This means the none of the attributes
         * (whitespace, alphabetic, numeric, quote, or comment) will be set on
         * any of the characters in the ranged  This makes each character in this
         * range parse as its own token.
         *
         * @param low The low end of the range of values to set the whitespace
         * attribute for
         * @param hi The high end of the range of values to set the whitespace
         * attribute for
         */
        public void ordinaryChars(int low, int hi)
        {
            if (low < 0)
                low = 0;
            if (hi > 255)
                hi = 255;
            for (int i = low; i <= hi; i++)
                resetChar(i);
        }

        /**
         * This method sets the numeric attribute on the characters '0' - '9' and
         * the characters '.' and '-'.
         * When this method is used, the result of giving other attributes
         * (whitespace, quote, or comment) to the numeric characters may
         * vary depending on the implementationd For example, if
         * parseNumbers() and then whitespaceChars('1', '1') are called,
         * this implementation reads "121" as 2, while some other implementation
         * will read it as 21.
         */
        public void parseNumbers()
        {
            for (int i = 0; i <= 9; i++)
                numeric['0' + i] = true;

            numeric['.'] = true;
            numeric['-'] = true;
        }

        /**
         * Puts the current token back into the StreamTokenizer so
         * <code>nextToken</code> will return the same value on the next call.
         * May cause the lineno method to return an incorrect value
         * if lineno is called before the next call to nextToken.
         */
        public void pushBack()
        {
            pushedBack = true;
        }

        /**
         * This method sets the quote attribute on the specified character.
         * Other attributes for the character are cleared.
         *
         * @param ch The character to set the quote attribute for, passed as an int.
         */
        public void quoteChar(int ch)
        {
            if (ch >= 0 && ch <= 255)
            {
                quote[ch] = true;
                comment[ch] = false;
                whitespace[ch] = false;
                alphabetic[ch] = false;
                numeric[ch] = false;
            }
        }

        /**
         * This method removes all attributes (whitespace, alphabetic, numeric,
         * quote, and comment) from all charactersd  It is equivalent to calling
         * <code>ordinaryChars(0x00, 0xFF)</code>.
         *
         * @see #ordinaryChars(int, int)
         */
        public void resetSyntax()
        {
            ordinaryChars(0x00, 0xFF);
        }

        /**
         * This method returns the current token value as a <code>String</code> in
         * the form "Token[x], line n", where 'n' is the current line numbers and
         * 'x' is determined as follows.
         * <p>
         * <ul>
         * <li>If no token has been read, then 'x' is "NOTHING" and 'n' is 0</li>
         * <li>If <code>ttype</code> is TT_EOF, then 'x' is "EOF"</li>
         * <li>If <code>ttype</code> is TT_EOL, then 'x' is "EOL"</li>
         * <li>If <code>ttype</code> is TT_WORD, then 'x' is <code>sval</code></li>
         * <li>If <code>ttype</code> is TT_NUMBER, then 'x' is "n=strnval" where
         * 'strnval' is <code>String.ValueOf(nval)</code>.</li>
         * <li>If <code>ttype</code> is a quote character, then 'x' is
         * <code>sval</code></li>
         * <li>For all other cases, 'x' is <code>ttype</code></li>
         * </ul>
         */
        public override String ToString()
        {
            String tempstr;
            if (ttype == TT_EOF)
                tempstr = "EOF";
            else if (ttype == TT_EOL)
                tempstr = "EOL";
            else if (ttype == TT_WORD)
                tempstr = _sval;
            else if (ttype == TT_NUMBER)
                tempstr = "n=" + _nval;
            else if (ttype == TT_NONE)
                tempstr = "NOTHING";
            else // must be an ordinary char.
                tempstr = "\'" + (char)ttype + "\'";

            return "Token[" + tempstr + "], line " + LineNumber;
        }

        /**
         * This method sets the whitespace attribute for all characters in the
         * specified range, range terminators included.
         *
         * @param low The low end of the range of values to set the whitespace
         * attribute for
         * @param hi The high end of the range of values to set the whitespace
         * attribute for
         */
        public void whitespaceChars(int low, int hi)
        {
            if (low < 0)
                low = 0;
            if (hi > 255)
                hi = 255;
            for (int i = low; i <= hi; i++)
            {
                resetChar(i);
                whitespace[i] = true;
            }
        }

        /**
         * This method sets the alphabetic attribute for all characters in the
         * specified range, range terminators included.
         *
         * @param low The low end of the range of values to set the alphabetic
         * attribute for
         * @param hi The high end of the range of values to set the alphabetic
         * attribute for
         */
        public void wordChars(int low, int hi)
        {
            if (low < 0)
                low = 0;
            if (hi > 255)
                hi = 255;
            for (int i = low; i <= hi; i++)
                alphabetic[i] = true;
        }

    }
}
