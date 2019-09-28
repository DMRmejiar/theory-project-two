using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrammarCreator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GrammarElement A = new GrammarElement(true, "A");
        GrammarElement B = new GrammarElement(true, "B");
        GrammarElement C = new GrammarElement(true, "C");
        GrammarElement D = new GrammarElement(true, "D");
        GrammarElement E = new GrammarElement(true, "E");
        GrammarElement a = new GrammarElement(false, "a");
        GrammarElement b = new GrammarElement(false, "b");
        GrammarElement c = new GrammarElement(false, "c");
        GrammarElement d = new GrammarElement(false, "d");
        GrammarElement e = new GrammarElement(false, "e");
        GrammarElement f = new GrammarElement(false, "f");
        
        GrammarProduction one = new GrammarProduction(A, new List<GrammarElement>() { a, B, C });
        GrammarProduction two = new GrammarProduction(A, new List<GrammarElement>() { D, b, A });
        GrammarProduction three = new GrammarProduction(B, new List<GrammarElement>() {  });
        GrammarProduction four = new GrammarProduction(B, new List<GrammarElement>() { b, A, B });
        GrammarProduction five = new GrammarProduction(C, new List<GrammarElement>() { c, C });
        GrammarProduction six = new GrammarProduction(C, new List<GrammarElement>() { D, d, B });
        GrammarProduction seven = new GrammarProduction(D, new List<GrammarElement>() {  });
        GrammarProduction eight = new GrammarProduction(D, new List<GrammarElement>() { e, E });
        GrammarProduction nine = new GrammarProduction(E, new List<GrammarElement>() { B, D });
        GrammarProduction ten = new GrammarProduction(E, new List<GrammarElement>() { f });

        List<GrammarProduction> productions = new List<GrammarProduction>() { one, two, three, four, five, six, seven, eight, nine, ten };

        Grammar grammar = new Grammar(productions, new List<GrammarElement>() { A, B, C, D, E });

        foreach (string item in grammar.GetVoidableNonTerminals())
        {
            Debug.Log(item);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
