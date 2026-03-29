using UnityEngine;
using System.Collections.Generic;

public class TestLexer : MonoBehaviour
{
    void Start()
    {
        var lexer = new Lexer("print(10*5+5**2)");
        var tokens = lexer.Tokenize();
        foreach (var t in tokens)
            Debug.Log($"{t.type} : {t.value}");
    }

}
