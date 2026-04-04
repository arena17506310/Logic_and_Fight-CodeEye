using System.Collections.Generic;
using UnityEngine;

public class Parser
{
    private List<Token> tokens;
    private int pos = 0;

    public Parser(List <Token> tokens)
    {
        this.tokens = tokens;
        this.pos = 0;
    }

    private Token Peek()
    {
        return tokens[pos];
    }

    private Token Advance()
    {
        Token curtoken = tokens[pos];
        pos++;
        return curtoken;
    }

    private Token Expect(TokenType type)
    {
        if(Peek().type == type)
        {
            return Advance();
        }
        else
        {
            throw new System.Exception($"예상된 토큰 : {type} \n 입력된 토큰 : {Peek().type}");
        }
    }

    public ProgramNode ParseProgram()
    {
        ProgramNode program = new();

        while(Peek().type != TokenType.EOF)
        {
            if(Peek().type == TokenType.NEWLINE)
            {
                Advance();
                continue;
            }
            program.Statements.Add(ParseStatement());
        }
        return program;
    }

    private ASTNode ParseStatement()
    {
        TokenType type = Peek().type;

        if (type == TokenType.IF) return ParseIf();
        if (type == TokenType.WHILE) return ParseWhile();
        if (type == TokenType.FOR) return ParseFor();
        if (type == TokenType.FUNC) return ParseFuncDef();
        if (type == TokenType.IDENT)
        {
            if (tokens[pos + 1].type == TokenType.ASSIGN)
            {
                return ParseASSIGN();
            }
            else
            {
                ParseExpression();
            }
        }

        throw new System.Exception("알 수 없는 문장: " + Peek().type);
    }

    private BlockNode ParseBlock()
    {
        BlockNode block = new();
        Expect(TokenType.LBRACE);
        while (Peek().type != TokenType.RBRACE)
        {
            if (Peek().type == TokenType.NEWLINE)
            {
                Advance();
                continue;
            }
            block.Statements.Add(ParseStatement());
        }
        Expect(TokenType.RBRACE);
        return block;
    }

    private AssignNode ParseAssign()
    {
        AssignNode assign = new();
        assign.Name = Expect(TokenType.IDENT).value;
        Expect(TokenType.ASSIGN);
        assign.Value = ParseExpression();
        return assign;
    }

    private IfNode ParseIf()
    {
        IfNode ifNode = new();
        Expect(TokenType.IF);
        Expect(TokenType.LPAREN);
        ifNode.Condition = ParseExpression();
        Expect(TokenType.RPAREN);
        ifNode.ThenBlock = ParseBlock();
        if (Peek().type == TokenType.ELSE)
        {
            Advance(); // else 소비
            if (Peek().type == TokenType.IF)
            {
                ifNode.ElseBlock = ParseIf(); // else if
            }
            else
            {
                ifNode.ElseBlock = ParseBlock(); // else
            }
        }
        return ifNode;
    }

    private WhileNode ParseWhile()
    {
        WhileNode whileNode = new();
        Expect(TokenType.WHILE);
        Expect(TokenType.LPAREN);
        whileNode.Condition = ParseExpression();
        Expect(TokenType.RPAREN);
        whileNode.Body = ParseBlock();
        return whileNode;
    }

    private ForNode ParseFor()
    {
        ForNode forNode = new();
        Expect(TokenType.FOR);
        forNode.VarName = Expect(TokenType.IDENT).value;
        Expect(TokenType.IN);
        forNode.Iterable = ParseExpression();
        forNode.Body = ParseBlock();
        return forNode;
    }

    private FuncDefNode ParseFunc()
    {
        FuncDefNode funcDefNode = new();
        Expect(TokenType.FUNC);
        funcDefNode.FuncName = Expect(TokenType.IDENT).value;
        Expect(TokenType.LPAREN);

        if(Peek().type != TokenType.RPAREN)
        {
            funcDefNode.Parameters.Add(Expect(TokenType.IDENT).value);
            while(Peek().type == TokenType.COMMA)
            {
                Advance(); // 콤마 소비
                funcDefNode.Parameters.Add(Expect(TokenType.IDENT).value);
            }
        }

        Expect(TokenType.RPAREN);
        funcDefNode.Body = ParseBlock();
        return funcDefNode;
    }
}