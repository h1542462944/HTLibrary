using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace User.Math.Lambda
{
    /// <summary>
    /// Parse Exception
    /// </summary>
    [DebuggerStepThrough]
    public sealed class ParseException : Exception
    {
        private int position;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParseException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="position">The position.</param>
        internal ParseException(string message, int position)
            : base(message)
        {
            this.position = position;
        }

        /// <summary>
        /// Gets the position.
        /// </summary>
        public int Position
        {
            get { return position; }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0} (at index {1})", Message, position);
        }
    }
    /// <summary>
    /// Symbol Parser
    /// </summary>
    [DebuggerStepThrough]
    [DebuggerDisplay("CurrentPosition = {CurrentPosition}, Source = {Source}")]
    public sealed class SymbolParser
    {
        #region Fields And Properties
        /// <summary>
        /// Gets the source.
        /// </summary>
        public string Source { get; private set; }
        /// <summary>
        /// Gets the current position.
        /// </summary>
        public int CurrentPosition { get; private set; }
        /// <summary>
        /// Gets the length.
        /// </summary>
        public int Length { get; private set; }
        /// <summary>
        /// Gets the current char.
        /// </summary>
        public char CurrentChar { get; private set; }

        private Token currentToken;
        /// <summary>
        /// Gets the current token.
        /// </summary>
        public Token CurrentToken { get { return currentToken; } }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolParser"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        public SymbolParser(string source)
        {
            if (ReferenceEquals(null, source))
                throw new ArgumentNullException("source");

            Source = source;
            Length = source.Length;
            SetPosition(0);
        }
        #endregion

        #region Business Methods
        /// <summary>
        /// Sets the position.
        /// </summary>
        /// <param name="index">The index.</param>
        public void SetPosition(int index)
        {
            CurrentPosition = index;
            CurrentChar = CurrentPosition < Length ? Source[CurrentPosition] : '\0';
        }

        /// <summary>
        /// Nexts the char.
        /// </summary>
        public void NextChar()
        {
            if (CurrentPosition < Length) CurrentPosition++;
            CurrentChar = CurrentPosition < Length ? Source[CurrentPosition] : '\0';
        }

        /// <summary>
        /// Nexts the token.
        /// </summary>
        /// <returns></returns>
        public Token NextToken()
        {
            while (Char.IsWhiteSpace(CurrentChar)) NextChar();
            TokenId t;
            int tokenPos = CurrentPosition;
            switch (CurrentChar)
            {
                case '!':
                    NextChar();
                    if (CurrentChar == '=')
                    {
                        NextChar();
                        t = TokenId.ExclamationEqual;
                    }
                    else
                    {
                        t = TokenId.Exclamation;
                    }
                    break;
                case '%':
                    NextChar();
                    t = TokenId.Percent;
                    break;
                case '&':
                    NextChar();
                    if (CurrentChar == '&')
                    {
                        NextChar();
                        t = TokenId.DoubleAmphersand;
                    }
                    else
                    {
                        t = TokenId.Amphersand;
                    }
                    break;
                case '(':
                    NextChar();
                    t = TokenId.OpenParen;
                    break;
                case ')':
                    NextChar();
                    t = TokenId.CloseParen;
                    break;
                case '*':
                    NextChar();
                    t = TokenId.Asterisk;
                    break;
                case '+':
                    NextChar();
                    t = TokenId.Plus;
                    break;
                case ',':
                    NextChar();
                    t = TokenId.Comma;
                    break;
                case '-':
                    NextChar();
                    t = TokenId.Minus;
                    break;
                case '.':
                    NextChar();
                    t = TokenId.Dot;
                    break;
                case '/':
                    NextChar();
                    t = TokenId.Slash;
                    break;
                case ':':
                    NextChar();
                    t = TokenId.Colon;
                    break;
                case '<':
                    NextChar();
                    if (CurrentChar == '=')
                    {
                        NextChar();
                        t = TokenId.LessThanEqual;
                    }
                    else if (CurrentChar == '>')
                    {
                        NextChar();
                        t = TokenId.LessGreater;
                    }
                    else
                    {
                        t = TokenId.LessThan;
                    }
                    break;
                case '=':
                    NextChar();
                    if (CurrentChar == '=')
                    {
                        NextChar();
                        t = TokenId.DoubleEqual;
                    }
                    else if (CurrentChar == '>')
                    {
                        NextChar();
                        t = TokenId.LambdaPrefix;
                    }
                    else
                    {
                        t = TokenId.Equal;
                    }
                    break;
                case '>':
                    NextChar();
                    if (CurrentChar == '=')
                    {
                        NextChar();
                        t = TokenId.GreaterThanEqual;
                    }
                    else
                    {
                        t = TokenId.GreaterThan;
                    }
                    break;
                case '?':
                    NextChar();
                    if (CurrentChar == '?')
                    {
                        NextChar();
                        t = TokenId.DoubleQuestion;
                    }
                    else
                    {
                        t = TokenId.Question;
                    }
                    break;
                case '[':
                    NextChar();
                    t = TokenId.OpenBracket;
                    break;
                case ']':
                    NextChar();
                    t = TokenId.CloseBracket;
                    break;
                case '{':
                    NextChar();
                    t = TokenId.OpenBrace;
                    break;
                case '}':
                    NextChar();
                    t = TokenId.CloseBrace;
                    break;
                case '|':
                    NextChar();
                    if (CurrentChar == '|')
                    {
                        NextChar();
                        t = TokenId.DoubleBar;
                    }
                    else
                    {
                        t = TokenId.Bar;
                    }
                    break;
                case '"':
                case '\'':
                    char quote = CurrentChar;
                    do
                    {
                        NextChar();
                        while (CurrentPosition < Length && CurrentChar != quote) NextChar();
                        if (CurrentPosition == Length)
                            throw ParseError(CurrentPosition, "Unterminated string literal");
                        NextChar();
                    } while (CurrentChar == quote);
                    t = TokenId.StringLiteral;
                    break;
                default:
                    if (Char.IsLetter(CurrentChar) || CurrentChar == '@' || CurrentChar == '_')
                    {
                        do
                        {
                            NextChar();
                        } while (Char.IsLetterOrDigit(CurrentChar) || CurrentChar == '_' || CurrentChar == '?');
                        t = TokenId.Identifier;
                        break;
                    }
                    if (Char.IsDigit(CurrentChar))
                    {
                        t = TokenId.IntegerLiteral;
                        do
                        {
                            NextChar();
                        } while (Char.IsDigit(CurrentChar));

                        if (CurrentChar == 'l' || CurrentChar == 'L')
                        {
                            t = TokenId.LongIntegerLiteral;
                            NextChar();
                            break;
                        }
                        else if (CurrentChar == 'f' || CurrentChar == 'F')
                        {
                            t = TokenId.SingleRealLiteral;
                            NextChar();
                            break;
                        }
                        else if (CurrentChar == 'm' || CurrentChar == 'M')
                        {
                            t = TokenId.DecimalRealLiteral;
                            NextChar();
                            break;
                        }
                        else if (CurrentChar == 'd' || CurrentChar == 'D')
                        {
                            t = TokenId.RealLiteral;
                            NextChar();
                            break;
                        }

                        if (CurrentChar == '.')
                        {
                            t = TokenId.RealLiteral;
                            NextChar();
                            ValidateDigit();
                            do
                            {
                                NextChar();
                            } while (Char.IsDigit(CurrentChar));
                        }

                        if (CurrentChar == 'E' || CurrentChar == 'e')
                        {
                            t = TokenId.RealLiteral;
                            NextChar();
                            if (CurrentChar == '+' || CurrentChar == '-') NextChar();
                            ValidateDigit();
                            do
                            {
                                NextChar();
                            } while (Char.IsDigit(CurrentChar));
                        }

                        if (CurrentChar == 'F' || CurrentChar == 'f')
                        {
                            t = TokenId.SingleRealLiteral;
                            NextChar();
                            break;
                        }
                        else if (CurrentChar == 'm' || CurrentChar == 'M')
                        {
                            t = TokenId.DecimalRealLiteral;
                            NextChar();
                            break;
                        }
                        else if (CurrentChar == 'd' || CurrentChar == 'D')
                        {
                            t = TokenId.RealLiteral;
                            NextChar();
                            break;
                        }

                        break;
                    }
                    if (CurrentPosition == Length)
                    {
                        t = TokenId.End;
                        break;
                    }
                    throw ParseError(CurrentPosition, "Syntax error '{0}'", CurrentChar);
            }
            currentToken.ID = t;
            currentToken.Text = Source.Substring(tokenPos, CurrentPosition - tokenPos);
            currentToken.Index = tokenPos;

            return new Token { ID = t, Text = currentToken.Text, Index = tokenPos, };
        }

        /// <summary>
        /// Builds the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>The Build result.</returns>
        public static SymbolParseResult Build(string source)
        {
            var item = new SymbolParser(source);
            List<Token> data = new List<Token>();
            while (true)
            {
                var token = item.NextToken();
                data.Add(token);
                if (token.ID == TokenId.End)
                    break;
            }
            return new SymbolParseResult(data);
        }
        #endregion

        #region Private Methods
        private void ValidateDigit()
        {
            if (!Char.IsDigit(CurrentChar)) throw ParseError(CurrentPosition, "Digit expected");
        }

        private Exception ParseError(string format, params object[] args)
        {
            return ParseError(currentToken.Index, format, args);
        }

        private Exception ParseError(int pos, string format, params object[] args)
        {
            return new ParseException(string.Format(CultureInfo.CurrentCulture, format, args), pos);
        }
        #endregion
    }
    /// <summary>
    /// Symbol Parse Result
    /// </summary>
    [Serializable]
    [DebuggerStepThrough]
    [DebuggerDisplay("{ToString()}")]
    public class SymbolParseResult : ReadOnlyCollection<Token>
    {
        #region Private Fields
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int _maxIndex = 0;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int _lastIndex = 0;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int _index = -1;
        #endregion

        #region Constuction
        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolParseResult"/> class.
        /// </summary>
        internal SymbolParseResult()
            : base(new List<Token>())
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolParseResult"/> class.
        /// </summary>
        /// <param name="list">The list.</param>
        internal SymbolParseResult(IList<Token> list)
            : base(list)
        {
            _maxIndex = list.Count - 1;
        }
        #endregion

        #region Business Properties
        /// <summary>
        /// 获取或设置当前读取索引
        /// </summary>
        public int Index
        {
            get { return _index; }
            private set
            {
                _lastIndex = _index;
                _index = value;
            }
        }
        /// <summary>
        /// 获取当前读取中的字符单元
        /// </summary>
        public Token Current
        {
            get
            {
                if (Index < 0 || Index > _maxIndex)
                    return Token.Empty;

                return this[Index];
            }
        }
        /// <summary>
        /// 获取完整的字符串表达式
        /// </summary>
        private string StringExpression
        {
            get { return string.Join(" ", this); }
        }
        #endregion

        #region Business Methods
        /// <summary>
        /// 读取下一个字符单元, 同时读取索引前进.
        /// </summary>
        /// <returns>读取得到的字符单元</returns>
        public Token Next()
        {
            Token token;
            if (TryGetElement(out token, Index + 1))
                return token;
            else
                return Token.Empty;
        }

        /// <summary>
        /// 判断下一个字符单元是否是指定的类型, 同时读取索引前进.
        /// </summary>
        /// <param name="tokenId">期待得到的字符单元类型.</param>
        /// <param name="throwIfNot">如果设置为 <c>true</c> 表示抛出异常. 默认为 <c>false</c> 表示不抛出异常.</param>
        /// <returns><c>true</c> 表示读取的单元类型和期待的单元类型一致; 否则返回 <c>false</c> .</returns>
        public bool NextIs(TokenId tokenId, bool throwIfNot = false)
        {
            var result = Next().ID == tokenId;
            if (!result && throwIfNot)
                throw new ApplicationException(string.Format("next is not {0}", tokenId));
            return result;
        }

        /// <summary>
        /// 尝试读取下一个字符单元, 但并不前进.
        /// </summary>
        /// <param name="count">尝试读取的当前字符单元的后面第几个单元, 默认为后面第一个单元.</param>
        /// <returns>读取得到的字符单元.</returns>
        public Token PeekNext(int count = 1)
        {
            Token token;
            if (PeekGetElement(out token, Index + count))
                return token;
            else
                return Token.Empty;
        }

        /// <summary>
        /// 判断下一个字符单元是否是指定的类型, 但读取索引不前进.
        /// </summary>
        /// <param name="tokenId">期待得到的字符单元类型.</param>
        /// <param name="count">判断当前字符后面第几个是指定的字符单元类型, 默认值为 1 .</param>
        /// <param name="throwIfNot">如果设置为 <c>true</c> 表示抛出异常. 默认为 <c>false</c> 表示不抛出异常.</param>
        /// <returns>
        /// 	<c>true</c> 表示读取的单元类型和期待的单元类型一致; 否则返回 <c>false</c> .
        /// </returns>
        public bool PeekNextIs(TokenId tokenId, int count = 1, bool throwIfNot = false)
        {
            var result = PeekNext(count).ID == tokenId;
            if (!result && throwIfNot)
                throw new ApplicationException(string.Format("next is not {0}", tokenId));
            return result;
        }

        /// <summary>
        /// 前进跳过指定的字符单元.
        /// </summary>
        /// <param name="count">The count.</param>
        public void Skip(int count = 1)
        {
            count = Index + count;
            CheckIndexOut(count);

            Index = count;
        }

        /// <summary>
        /// 读取直到符合 predicate 的条件时停止.
        /// </summary>
        /// <param name="predicate">比较当前 Token 是否符合条件的方法.</param>
        /// <returns>读取停止时的 Token 列表.</returns>
        public IList<Token> SkipUntil(Func<Token, bool> predicate)
        {
            List<Token> data = new List<Token>();
            while (!predicate(Current) || Current.ID == TokenId.End)
                data.Add(Next());
            return data;
        }

        /// <summary>
        /// 返回到指定的读取索引.
        /// </summary>
        /// <param name="index">目标读取索引.</param>
        public void ReturnToIndex(int index)
        {
            if (index < -1 || index > _maxIndex)
                throw new IndexOutOfRangeException();

            Index = index;
        }
        #endregion

        #region Private Methods
        private bool TryGetElement(out Token token, int index)
        {
            bool result = PeekGetElement(out token, index);
            if (result)
                Index = index;
            return result;
        }

        private bool PeekGetElement(out Token token, int index)
        {
            if (index < 0 || index > _maxIndex)
            {
                token = Token.Empty;
                return false;
            }
            else
            {
                token = this[index];
                return true;
            }
        }

        private void CheckIndexOut(int index)
        {
            if (index < 0 || index > _maxIndex)
                throw new IndexOutOfRangeException();
        }
        #endregion

        #region Override Methods
        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Join(" ", this.TakeWhile(p => p.Index < Current.Index));
        }
        #endregion
    }
    /// <summary>
    /// 字符单元
    /// </summary>
    [DebuggerStepThrough]
    [DebuggerDisplay("Text = {Text}, ID = {ID}, Index = {Index}")]
    public struct Token
    {
        #region Fields
        /// <summary>
        /// 空的字符单元
        /// </summary>
        public static readonly Token Empty = new Token();
        private TokenId id;
        private string text;
        private int index;
        private int? hash;
        #endregion

        #region Properties
        /// <summary>
        /// 获取或设置字符类型
        /// </summary>
        public TokenId ID
        {
            get { return id; }
            set
            {
                id = value;
                hash = null;
            }
        }
        /// <summary>
        /// 获取或设置当前字符单元的文本表示
        /// </summary>
        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                hash = null;
            }
        }
        /// <summary>
        /// 获取或设置当前字符单元在整体结果中的索引
        /// </summary>
        public int Index
        {
            get { return index; }
            set
            {
                index = value;
                hash = null;
            }
        }
        #endregion

        #region Override Methods
        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null)) return false;
            if (obj is Token)
                return Equals((Token)obj);
            else
                return false;
        }

        /// <summary>
        /// Equalses the specified token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        public bool Equals(Token token)
        {
            if (ReferenceEquals(token, null)) return false;
            return ID == token.id && Text == token.Text;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                if (!hash.HasValue)
                {
                    hash = ID.GetHashCode();
                    hash ^= Text.GetHashCode();
                    hash ^= Index.GetHashCode();
                }
                return hash.Value;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return text;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Lenic.DI.Core.Token"/> to <see cref="System.String"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator string(Token value)
        {
            return value.text;
        }
        #endregion

        #region Exception Throw
        /// <summary>
        /// 如果当前实例的文本表示与指定的字符串不符, 则抛出异常.
        /// </summary>
        /// <param name="id">待判断的字符串.</param>
        /// <returns>当前实例对象.</returns>
        public Token Throw(TokenId id)
        {
            if (ID != id)
                throw new ParserSyntaxErrorException();

            return this;
        }

        /// <summary>
        /// 如果当前实例的字符类型与指定的字符类型不符, 则抛出异常.
        /// </summary>
        /// <param name="id">待判断的目标类型的字符类型.</param>
        /// <returns>当前实例对象.</returns>
        public Token Throw(string text)
        {
            if (Text != text)
                throw new ParserSyntaxErrorException();

            return this;
        }
        #endregion
    }
    /// <summary>
    /// 字符单元类型
    /// </summary>
    public enum TokenId
    {
        /// <summary>
        /// End
        /// </summary>
        End,
        /// <summary>
        /// Identifier
        /// </summary>
        Identifier,
        /// <summary>
        /// String
        /// </summary>
        StringLiteral,
        /// <summary>
        /// Integer Literal
        /// </summary>
        IntegerLiteral,
        /// <summary>
        /// Long Integer Literal
        /// </summary>
        LongIntegerLiteral,
        /// <summary>
        /// Single Real Literal
        /// </summary>
        SingleRealLiteral,
        /// <summary>
        /// Decimal Real Literal
        /// </summary>
        DecimalRealLiteral,
        /// <summary>
        /// Real Literal
        /// </summary>
        RealLiteral,
        /// <summary>
        /// !
        /// </summary>
        Exclamation,
        /// <summary>
        /// %
        /// </summary>
        Percent,
        /// <summary>
        /// &amp;
        /// </summary>
        Amphersand,
        /// <summary>
        /// (
        /// </summary>
        OpenParen,
        /// <summary>
        /// )
        /// </summary>
        CloseParen,
        /// <summary>
        /// *
        /// </summary>
        Asterisk,
        /// <summary>
        /// +
        /// </summary>
        Plus,
        /// <summary>
        /// ,
        /// </summary>
        Comma,
        /// <summary>
        /// -
        /// </summary>
        Minus,
        /// <summary>
        /// .
        /// </summary>
        Dot,
        /// <summary>
        /// /
        /// </summary>
        Slash,
        /// <summary>
        /// :
        /// </summary>
        Colon,
        /// <summary>
        /// &lt;
        /// </summary>
        LessThan,
        /// <summary>
        /// =
        /// </summary>
        Equal,
        /// <summary>
        /// &gt;
        /// </summary>
        GreaterThan,
        /// <summary>
        /// ?
        /// </summary>
        Question,
        /// <summary>
        /// ??
        /// </summary>
        DoubleQuestion,
        /// <summary>
        /// [
        /// </summary>
        OpenBracket,
        /// <summary>
        /// ]
        /// </summary>
        CloseBracket,
        /// <summary>
        /// |
        /// </summary>
        Bar,
        /// <summary>
        /// !=
        /// </summary>
        ExclamationEqual,
        /// <summary>
        /// &amp;&amp;
        /// </summary>
        DoubleAmphersand,
        /// <summary>
        /// &lt;=
        /// </summary>
        LessThanEqual,
        /// <summary>
        /// &lt;&gt; 
        /// </summary>
        LessGreater,
        /// <summary>
        /// ==
        /// </summary>
        DoubleEqual,
        /// <summary>
        /// &gt;=
        /// </summary>
        GreaterThanEqual,
        /// <summary>
        /// ||
        /// </summary>
        DoubleBar,
        /// <summary>
        /// =&gt;
        /// </summary>
        LambdaPrefix,
        /// <summary>
        /// {
        /// </summary>
        OpenBrace,
        /// <summary>
        /// }
        /// </summary>
        CloseBrace,
    }
}
