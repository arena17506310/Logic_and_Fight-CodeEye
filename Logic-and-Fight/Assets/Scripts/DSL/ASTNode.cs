using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

public abstract class ASTNode
{
    
}

public class ProgramNode : ASTNode
{
    public List<ASTNode> Statements = new();
}
public class BlockNode : ASTNode
{
    public List<ASTNode> Statements = new();
}

public class  NumberLiteral : ASTNode
{
    public double Value;
}

public class StringLiteral : ASTNode
{
    public string Value;
}

public class BooleanLiteral : ASTNode
{
    public bool Value;
}

public class  NullLiteral : ASTNode { }

public class Identifier : ASTNode
{
    public string Name;
}

public class BinaryOpNode : ASTNode
{
    public ASTNode Left; 
    public TokenType Op;
    public ASTNode Right;
}

public class UnaryOpNode : ASTNode
{
    public TokenType Op;
    public ASTNode Operand;
}

public class AssignNode : ASTNode
{
    public string Name;
    public ASTNode Value;
} 

public class FuncCallNode : ASTNode
{
    public string FuncName;
    public List<ASTNode> Arguments = new();
}

public class IfNode : ASTNode
{
    public ASTNode Condition;
    public BlockNode ThenBlock;
    public ASTNode ElseBlock;
}

public class WhileNode : ASTNode
{
    public ASTNode Condition;
    public BlockNode Body;
}

public class ForNode : ASTNode
{
    public string VarName;
    public ASTNode Iterable;
    public BlockNode Body;
}

public class FuncDefNode : ASTNode
{
    public string FuncName;
    public List<string> Parameters = new();
    public BlockNode Body;
}

