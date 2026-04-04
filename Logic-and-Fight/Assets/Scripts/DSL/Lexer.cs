using System;
using System.Collections.Generic;
public enum TokenType
{
    // 리터럴
    IDENT,          // 예약어, 변수명, 함수명
    NUMBER,         // 숫자
    STRING,         // 문자열
    BOOLEAN,        // True, False

    // 괄호·구분자
    LPAREN,         // (
    RPAREN,         // )
    LBRACE,         // {
    RBRACE,         // }
    LBRACKET,       // [
    RBRACKET,       // ]
    COLON,          // :
    SEMICOLON,      // ;
    DOT,            // .
    COMMA,          // ,
    PERCENT,        // %

    // 산술 연산자
    PLUS,           // +
    MINUS,          // -
    STAR,           // *
    DSTAR,          // **
    SLASH,          // /

    // 비교·대입 연산자
    ASSIGN,         // =
    EQUAL,          // ==
    EXCLAM,         // !
    EXCLAM_EQUAL,   // !=
    GREATER,        // >
    GREATER_EQUAL,  // >=
    LESS,           // <
    LESS_EQUAL,     // <=

    // 키워드
    AND,            // AND
    OR,             // OR
    IF,             // IF
    ELSE,           // ELSE
    FOR,            // FOR
    IN,             // IN
    WHILE,          // WHILE
    FUNC,           // FUNC
    RETURN,         // RETURN
    NULL,           // null

    // 구조
    NEWLINE,        // 줄바꿈
    EOF,            // 끝
}

public class Token
{
    public TokenType type;
    public string value;

    public Token(TokenType type, string value)
    {
        this.type = type;
        this.value = value;
    }
}
public class Lexer
{
    private string source;
    private int pos = 0;

    public Lexer(string source) { this.source = source; }

    public List<Token> Tokenize()
    {
        var tokens = new List<Token>();
        while (pos < source.Length)
        {
            SkipWhiteSpace();
            if (pos >= source.Length) break;

            char c = source[pos];

            if (char.IsLetter(c) || c == '_')
            {
                tokens.Add(ReadIdent());
            }
            else if (char.IsDigit(c))
            {
                tokens.Add(ReadNumber());
            }
            else if (c == '"')
            {
                tokens.Add(ReadString());
            }
            else if (c == '(') { tokens.Add(new Token(TokenType.LPAREN, "(")); pos++; }
            else if (c == ')') { tokens.Add(new Token(TokenType.RPAREN, ")")); pos++; }
            else if (c == '{') { tokens.Add(new Token(TokenType.LBRACE, "{")); pos++; }
            else if (c == '}') { tokens.Add(new Token(TokenType.RBRACE, "}")); pos++; }
            else if (c == '[') { tokens.Add(new Token(TokenType.LBRACKET, "[")); pos++; }
            else if (c == ']') { tokens.Add(new Token(TokenType.RBRACKET, "]")); pos++; }
            else if (c == ':') { tokens.Add(new Token(TokenType.COLON, ":")); pos++; }
            else if (c == ';') { tokens.Add(new Token(TokenType.SEMICOLON, ";")); pos++; }
            else if (c == '.') { tokens.Add(new Token(TokenType.DOT, ".")); pos++; }
            else if (c == ',') { tokens.Add(new Token(TokenType.COMMA, ",")); pos++; }
            else if (c == '%') { tokens.Add(new Token(TokenType.PERCENT, "%")); pos++; }
            else if (c == '+') { tokens.Add(new Token(TokenType.PLUS, "+")); pos++; }
            else if (c == '-') { tokens.Add(new Token(TokenType.MINUS, "-")); pos++; }
            else if (c == '*')
            {
                if (Peek() == '*')
                {
                    tokens.Add(new Token(TokenType.DSTAR, "**"));
                    pos += 2;
                }
                else
                {
                    tokens.Add(new Token(TokenType.STAR, "*"));
                    pos++;
                }
            }
            else if (c == '/') { tokens.Add(new Token(TokenType.SLASH, "/")); pos++; }
            else if (c == '=')
            {
                if (Peek() == '=')
                {
                    tokens.Add(new Token(TokenType.EQUAL, "=="));
                    pos += 2;
                }
                else
                {
                    tokens.Add(new Token(TokenType.ASSIGN, "="));
                    pos++;
                }
            }
            else if (c == '!')
            {
                if (Peek() == '=')
                {
                    tokens.Add(new Token(TokenType.EXCLAM_EQUAL, "!="));
                    pos += 2;
                }
                else
                {
                    tokens.Add(new Token(TokenType.EXCLAM, "!"));
                    pos++;
                }
            }
            else if (c == '>')
            {
                if (Peek() == '=')
                {
                    tokens.Add(new Token(TokenType.GREATER_EQUAL, ">="));
                    pos += 2;
                }
                else
                {
                    tokens.Add(new Token(TokenType.GREATER, ">"));
                    pos++;
                }
            }
            else if (c == '<')
            {
                if (Peek() == '=')
                {
                    tokens.Add(new Token(TokenType.LESS_EQUAL, "<="));
                    pos += 2;
                }
                else
                {
                    tokens.Add(new Token(TokenType.LESS, "<"));
                    pos++;
                }
            }
            else if (c == '\n')
            {
                tokens.Add(new Token(TokenType.NEWLINE, "\n"));
                pos++;
            }
            else
            {
                throw new Exception($"적절한 문자가 아닙니다! : {c}");
            }
        }
        tokens.Add(new Token(TokenType.EOF, ""));
        return tokens;
    }

    Token ReadIdent()
    {
        int start = pos;
        while (pos < source.Length && (char.IsLetterOrDigit(source[pos]) || source[pos] == '_'))
            pos++;
        string value = source.Substring(start, pos - start);

        TokenType type = value.ToLower() switch
        {
            "if" => TokenType.IF,
            "else" => TokenType.ELSE,
            "for" => TokenType.FOR,
            "while" => TokenType.WHILE,
            "func" => TokenType.FUNC,
            "return" => TokenType.RETURN,
            "and" => TokenType.AND,
            "or" => TokenType.OR,
            "in" => TokenType.IN,
            "true" => TokenType.BOOLEAN,
            "false" => TokenType.BOOLEAN,
            _ => TokenType.IDENT
        };

        return new Token(type, value);
    }

    Token ReadNumber()
    {
        int start = pos;
        while (pos < source.Length && char.IsDigit(source[pos]))
            pos++;
        string value = source.Substring(start, pos - start);
        return new Token(TokenType.NUMBER, value);
    }

    Token ReadString()
    {
        pos++; // 시작 " 건너뛰기
        int start = pos;
        while (pos < source.Length && source[pos] != '"')
            pos++;

        string value = source.Substring(start, pos - start);

        if (pos >= source.Length)
            throw new Exception("문자열이 닫히지 않았습니다!");

        pos++; // 닫는 " 건너뛰기
        return new Token(TokenType.STRING, value);
    }

    char Peek()
    {
        if (pos + 1 >= source.Length) return '\0';
        return source[pos + 1];
    }

    void SkipWhiteSpace()
    {
        while (pos < source.Length && source[pos] == ' ' || source[pos] == '\t' || source[pos] == '\r')
            pos++;
    }

}

