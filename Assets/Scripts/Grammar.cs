using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Grammar
{
    List<GrammarProduction> productions;
    List<GrammarElement> nonTerminals;
    Dictionary<GrammarElement, bool> voidableNT;
    List<bool> voidableProductions;
    Dictionary<GrammarElement, List<GrammarElement>> firstOfEachNT;
    List<List<GrammarElement>> firstOfEachProduction;
    Dictionary<GrammarElement, List<GrammarElement>> nextOfEachNT;
    List<GrammarElement> selectionOfEachProduction;
    bool isSGrammar = false;
    bool isQGrammar = false;
    bool isSFGrammar = false;
    bool isLRGrammar = false;
    string endOfSequence = "Ω";

    /// <summary>
    /// Constructor de la Clase Grammar
    /// </summary>
    /// <param name="productions"></param>
    /// <param name="nonTerminals"></param>
    public Grammar(List<GrammarProduction> productions, List<GrammarElement> nonTerminals)
    {
        this.productions = productions;
        this.nonTerminals = nonTerminals;
        GenerateNonTerminals();
        Voidables();
        GenerateFirstOfEachNT();
        GenerateFirstOfEachProduction();
        GenerateNextOfEachNT();
        GenerateSelectionOfEachProduction();
        IsSGrammar();
        IsQGrammar();
        IsSFGrammar();
        IsLRGrammar();
    }

    /// <summary>
    /// Crea y define el diccionario de anulables, dándoles valor de false
    /// </summary>
    public void GenerateNonTerminals()
    {
        voidableProductions = new List<bool>();
        foreach (GrammarProduction nt in productions)
        {
            voidableProductions.Add(false);
        }

        voidableNT = new Dictionary<GrammarElement, bool>();
        foreach (GrammarElement nt in nonTerminals)
        {
            voidableNT.Add(nt, false);
        }
    }

    /// <summary>
    /// Crea una lista de Strings con los simbolos de los No Terminales anulables
    /// </summary>
    /// <returns>Dicha lista</returns>
    public List<string> GetVoidableNonTerminals()
    {
        List<string> voidables = new List<string>();

        foreach (KeyValuePair<GrammarElement, bool> nt in voidableNT)
        {
            if (nt.Value)
            {
                voidables.Add(nt.Key.GetSymbol());
            }
        }

        for (int i = 0; i < voidableProductions.Count; i++)
        {
            if (voidableProductions[i])
            {
                voidables.Add("" + i);
            }
        }

        return voidables;
    }

    /// <summary>
    /// Define cuales No Terminales son Anulables haciendo uso del Diccionario y define cuales Producciones son anulables usando la lista
    /// </summary>
    private void Voidables()
    {

        for (int i = 0; i < productions.Count; i++)
        {
            if (productions[i].GetRightSide().Count == 0)
            {
                voidableProductions[i] = true;

                if (!voidableNT[productions[i].GetLeftSide()])
                {
                    voidableNT[productions[i].GetLeftSide()] = true;
                    Voidables();
                }
            }
            else
            {
                bool isVoidable = true;
                foreach (GrammarElement element in productions[i].GetRightSide())
                {
                    if (!element.IsNonTerminal() || !voidableNT[element])
                    {
                        isVoidable = false;
                        break;
                    }
                }
                if (isVoidable)
                {
                    if (!voidableNT[productions[i].GetLeftSide()])
                    {
                        voidableProductions[i] = true;
                        voidableNT[productions[i].GetLeftSide()] = true;
                        Voidables();
                    }
                }
            }
        }
    }

    /// <summary>
    /// Inicializador de busqueda de primeros de cada produccion de la Clase Grammar
    /// </summary>
    private void GenerateFirstOfEachNT()
    {
        firstOfEachNT = new Dictionary<GrammarElement, List<GrammarElement>>();

        foreach (var nt in nonTerminals)
        {
            firstOfEachNT.Add(nt, new List<GrammarElement>());
        }

        for (var indexProduction = 0; indexProduction < productions.Count; indexProduction++)
        {
            var seekedNt = new List<GrammarElement> { productions[indexProduction].GetLeftSide() };
            FindFirstOfEachNT(productions[indexProduction].GetLeftSide(), indexProduction, seekedNt, 0);
        }
    }

    public List<string> GetFirstOfEachProduction()
    {
        List<string> firsts = new List<string>();
        string newFirsts = "";
        foreach (List<GrammarElement> item in firstOfEachProduction)
        {
            foreach (GrammarElement i in item)
            {
                newFirsts += i.GetSymbol() + " ";
            }
            firsts.Add(newFirsts);
            newFirsts = "";
        }
        return firsts;
    }

    public List<string> GetFirstOfEachNT()
    {
        List<string> firsts = new List<string>();
        string newFirsts = "";
        foreach (KeyValuePair<GrammarElement, List<GrammarElement>> item in firstOfEachNT)
        {
            foreach (GrammarElement i in item.Value)
            {
                newFirsts += i.GetSymbol() + " ";
            }
            firsts.Add(newFirsts);
            newFirsts = "";
        }
        return firsts;
    }

    /// <summary>
    /// Busqueda recursiva de Primeros de cada Produccion de la Clase Grammar
    /// </summary>
    /// <param name="nonterminal"></param>
    /// <param name="production"></param>
    /// <param name="seekedNt"></param>
    /// <param name="index"></param>
    private void FindFirstOfEachNT(GrammarElement actualNt, int indexProduction, List<GrammarElement> seekedNt, int indexElement)
    {
        if (productions[indexProduction].GetRightSide().Count == 0) return;
        var firstOfRightSide = productions[indexProduction].GetRightSide()[indexElement];
        if (!firstOfRightSide.IsNonTerminal())
        {
            if (!firstOfEachNT[actualNt].Contains(firstOfRightSide))
            {
                firstOfEachNT[actualNt].Add(firstOfRightSide);
            }
        }
        else
        {
            if (seekedNt.Contains(firstOfRightSide)) return;
            seekedNt.Add(firstOfRightSide);
            for (var index = 0; index < productions.Count; index++)
            {
                var production = productions[index];
                if (production.GetLeftSide() == firstOfRightSide)
                {
                    FindFirstOfEachNT(actualNt, index, seekedNt, 0);
                }
            }

            if (!voidableNT[firstOfRightSide]) return;
            var indexNextElement = indexElement + 1;
            if (indexNextElement < productions[indexProduction].GetRightSide().Count)
            {
                if (productions[indexProduction].GetRightSide()[indexNextElement].IsNonTerminal())
                {
                    if (seekedNt.Contains(productions[indexProduction].GetRightSide()[indexNextElement])) return;
                    seekedNt.Add(productions[indexProduction].GetRightSide()[indexNextElement]);
                    for (var index = 0; index < productions.Count; index++)
                    {
                        var production = productions[index];
                        if (production.GetLeftSide() == productions[indexProduction].GetRightSide()[indexNextElement])
                        {
                            FindFirstOfEachNT(actualNt, index, seekedNt, 0);
                        }
                    }
                }
                else
                {
                    if (!firstOfEachNT[actualNt].Contains(firstOfRightSide))
                    {
                        firstOfEachNT[actualNt].Add(productions[indexProduction].GetRightSide()[indexElement + 1]);
                    }
                }
            }
            else
            {
                for (var index = 0; index < productions.Count; index++)
                {
                    var production = productions[index];
                    if (production.GetLeftSide() == productions[indexProduction].GetLeftSide())
                    {
                        FindFirstOfEachNT(actualNt, index, seekedNt, indexElement + 1);
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// Genera primeros de cada produccion.
    /// </summary>
    private void GenerateFirstOfEachProduction()
    {
        firstOfEachProduction = new List<List<GrammarElement>>();
        for (var index = 0; index < productions.Count; index++)
        {
            //firstOfEachProduction.Add(new List<GrammarElement>(0));
            var toAdd = new List<GrammarElement>();
            var production = productions[index];
            
            foreach (var element in production.GetRightSide())
            {
                if (element.IsNonTerminal())
                {
                    foreach (var __element in firstOfEachNT[element])
                    {
                        toAdd.Add(__element);
                    }
                    if (!voidableNT[element]) break;
                }
                else
                {
                    toAdd.Add(element);
                    break;
                }
            }
            
            firstOfEachProduction.Add(toAdd);
            /*
            string __temp = "Primeros de "+index+": ";
            foreach (var value in firstOfEachProduction[index])
            {
                __temp += " " + value.GetSymbol();
            }
            Debug.Log(__temp);
            */
        }
    }

    /// <summary>
    /// Inicializador de busqueda de siguientes de cada produccion de la Clase Grammar
    /// </summary>
    private void GenerateNextOfEachNT()
    {
        nextOfEachNT = new Dictionary<GrammarElement, List<GrammarElement>>();
        
        foreach (var nt in nonTerminals)
        {
            firstOfEachNT.Add(nt, new List<GrammarElement>());
        }

        foreach (var nonTerminal in nonTerminals)
        {
            var seekedNt = new List<GrammarElement> {nonTerminal};
            if (productions[0].GetLeftSide() == nonTerminal)
            {
                nextOfEachNT[nonTerminal].Add(new GrammarElement(false,endOfSequence));
            }
            FindNextOfEachNT(nonTerminal, seekedNt);
        }
    }

    private void FindNextOfEachNT(GrammarElement nonTerminal, List<GrammarElement> seekedNt)
    {
        for (var indexProduction = 0; indexProduction < productions.Count; indexProduction++)
        {
            for (var indexElement = 0; indexElement < productions[indexProduction].GetRightSide().Count; indexElement++)
            {
                if (productions[indexProduction].GetRightSide()[indexElement].IsNonTerminal())
                {
                    seekedNt.Add(productions[indexProduction].GetRightSide()[indexElement]);
                    FindNextOfEachNT(nonTerminal, seekedNt);
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void GenerateSelectionOfEachProduction()
    {

    }

    /// <summary>
    /// Define no terminales, identifica símbolo de lado izquierdo que sea terminal, y que no esté repetido.
    /// No acepta secuencia nula
    /// </summary>
    private void IsSGrammar()
    {
        List<string> eachNT = new List<string>();
        for (int i = 0; i < productions.Count; i++)
        {
            string nTByProduction = productions[i].GetLeftSide().GetSymbol();
            if (!eachNT.Contains(nTByProduction))
            {
                eachNT.Add(nTByProduction);
            }
        }
        int helper = 0;
        for (int x = 0; x < eachNT.Count; x++)
        {
            List<string> firstTofEachNT = new List<string>();
            for (int i = 0; i < productions.Count; i++)
            {
                if (productions[i].GetLeftSide().GetSymbol() == eachNT[x])
                {
                    if (productions[i].GetRightSide().Count != 0)
                    {
                        if (!productions[i].GetRightSide()[0].IsNonTerminal() == true)
                        {
                            if (!firstTofEachNT.Contains(productions[i].GetRightSide()[0].GetSymbol()))
                            {
                                firstTofEachNT.Add(productions[i].GetRightSide()[0].GetSymbol()); helper++;
                            }
                        }
                    }
                }
            }
        }
        if (helper == productions.Count) isSGrammar = true;
        
    }

    /// <summary>
    /// Define no terminales, identifica símbolo de lado izquierdo que sea terminal, y que no esté repetido.
    /// Acepta secuencia nula
    /// </summary>
    private void IsQGrammar()
    {
        List<string> eachNT = new List<string>();
        for (int i = 0; i < productions.Count; i++)
        {
            string nTByProduction = productions[i].GetLeftSide().GetSymbol();
            if (!eachNT.Contains(nTByProduction))
            {
                eachNT.Add(nTByProduction);
            }
        }
        int helper = 0;
        for (int x = 0; x < eachNT.Count; x++)
        {
            List<string> firstTofEachNT = new List<string>();
            for (int i = 0; i < productions.Count; i++)
            {
                if (productions[i].GetLeftSide().GetSymbol() == eachNT[x])
                {
                    if (productions[i].GetRightSide().Count == 0)
                    {
                        helper++;
                    }
                    else
                    {
                        if (!productions[i].GetRightSide()[0].IsNonTerminal() == true)
                        {
                            if (!firstTofEachNT.Contains(productions[i].GetRightSide()[0].GetSymbol()))
                            {
                                firstTofEachNT.Add(productions[i].GetRightSide()[0].GetSymbol()); helper++;
                            }
                        }
                    }
                }
            }
        }
        if (helper == productions.Count) isQGrammar = true;
    }

    /// <summary>
    /// Verifica que el lado derecho tenga magnitud 2 o 0, si tiene 2, que cumpla con que sea terminal y no terminal respectivamente
    /// </summary>
    private void IsSFGrammar()
    {
        int helper = 0;
        for (int i = 0; i < productions.Count; i++)
        {
            if (productions[i].GetRightSide().Count == 0)
            {
                helper++;
            }
            else
            {
                if (productions[i].GetRightSide().Count == 2)
                {
                    if (productions[i].GetRightSide()[0].IsNonTerminal() == false && productions[i].GetRightSide()[1].IsNonTerminal() == true)
                    {
                        helper++;
                    }
                }
            }
        }
        if (helper == productions.Count) isSFGrammar = true;
    }

    /// <summary>
    /// Verifica que el lado derecho termine en no terminal, precedido de uno, varios o ningún terminal. Acepta secuencia nula
    /// </summary>
    private void IsLRGrammar()
    {
        int helper = 0;
        for (int i = 0; i < productions.Count; i++)
        {
            int helper2 = 0;
            if(productions[i].GetRightSide().Count != 0)
            {
                for (int x = productions[i].GetRightSide().Count; x >0; x--)
                {
                    if (x == productions[i].GetRightSide().Count)
                    {
                        if (productions[i].GetRightSide()[x-1].IsNonTerminal()==true)
                        {
                            helper2++;
                        }
                    }
                    else
                    {
                        if(productions[i].GetRightSide()[x-1].IsNonTerminal() == false)
                        {
                            helper2++;
                        }
                    }
                }
                if (helper2 == productions[i].GetRightSide().Count)
                {
                    helper++;
                }
            }
            else
            {
                helper++;
            }
            
        }
        if (helper == productions.Count) isLRGrammar = true;
    }

    public List<bool> GetGrammarTypes()
    {
        List<bool> grammarTypes = new List<bool>() { isSGrammar, isQGrammar, isSFGrammar, isLRGrammar };
        return grammarTypes;
    }
}