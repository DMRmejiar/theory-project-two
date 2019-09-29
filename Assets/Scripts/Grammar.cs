using System;
using System.Collections.Generic;
using UnityEngine;

public class Grammar
{
    List<GrammarProduction> productions;
    List<GrammarElement> nonTerminals;
    Dictionary<GrammarElement, bool> voidableNT;
    List<bool> voidableProductions;
    /* List<List<GrammarElement>> firstOfEachNT; */
    Dictionary<GrammarElement, List<GrammarElement>> firstOfEachNT;
    List<GrammarElement> firstOfEachProduction;
    List<GrammarElement> nextOfEachNT;
    List<GrammarElement> selectionOfEachProduction;

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

    public List<string> GetFirstOfEachNT()
    {
        List<string> firsts = new List<string>();
        foreach (var item in firstOfEachNT)
        {
            string value = item.Key.GetSymbol() + " =";
            foreach (var element in item.Value)
            {
                value += " "+element.GetSymbol();
            }
            firsts.Add(value);
        }
        return firsts;
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
    
    private void GenerateFirstOfEachNT()
    {
        List<GrammarElement> seekedNt = new List<GrammarElement>();
        firstOfEachNT = new Dictionary<GrammarElement, List<GrammarElement>>();
        
        foreach (GrammarElement nt in nonTerminals)
        {
            firstOfEachNT.Add(nt, new List<GrammarElement>());
        }
        
        for (var indexProduction = 0; indexProduction < productions.Count; indexProduction++)
        {
            Debug.Log(indexProduction+" Here Start Recursion GenerateFirstOfEachNT");
            seekedNt = new List<GrammarElement>();
            seekedNt.Add(productions[indexProduction].GetLeftSide());
            FindFirstOfEachNT(productions[indexProduction].GetLeftSide(), indexProduction, seekedNt, 0);
        }
    }
    private void FindFirstOfEachNT(GrammarElement actualNt, int indexProduction, List<GrammarElement> seekedNt, int indexElement)
    {
        if (productions[indexProduction].GetRightSide().Count == 0) return;
        Debug.Log(" Here add Firsts of production "+indexProduction+" to "+actualNt.GetSymbol());
        GrammarElement firstOfRightSide = productions[indexProduction].GetRightSide()[indexElement];
        if (!firstOfRightSide.IsNonTerminal())
        {
            if (!firstOfEachNT[actualNt].Contains(firstOfRightSide))
            {
                Debug.Log("  Add "+firstOfRightSide.GetSymbol()+" to "+actualNt.GetSymbol());
                firstOfEachNT[actualNt].Add(firstOfRightSide);
            }
        }
        else
        {
            Debug.Log("  <"+firstOfRightSide.GetSymbol()+"> is first of "+actualNt.GetSymbol());
    
            if (!seekedNt.Contains(firstOfRightSide))
            {
                Debug.Log("  Add "+firstOfRightSide.GetSymbol()+" to seekedNt");
                seekedNt.Add(firstOfRightSide);
                for (var index = 0; index < productions.Count; index++)
                {
                    var production = productions[index];
                    if (production.GetLeftSide() == firstOfRightSide)
                    {
                        FindFirstOfEachNT(actualNt, index, seekedNt, 0);
                    }
                }

                if (voidableNT[firstOfRightSide])
                {
                    Debug.Log("   <"+firstOfRightSide.GetSymbol()+"> is Voidable");
                    if (indexElement + 1 < productions[indexProduction].GetRightSide().Count)
                    {
                        Debug.Log("   <"+firstOfRightSide.GetSymbol()+"> is not at the end of the production");
                        if (productions[indexProduction].GetRightSide()[indexElement + 1].IsNonTerminal())
                        {
                            if (!seekedNt.Contains(productions[indexProduction].GetRightSide()[indexElement + 1]))
                            {
                                seekedNt.Add(productions[indexProduction].GetRightSide()[indexElement + 1]);
                                for (var index = 0; index < productions.Count; index++)
                                {
                                    var production = productions[index];
                                    if (production.GetLeftSide() == productions[indexProduction].GetRightSide()[indexElement + 1])
                                    {
                                        
                                        FindFirstOfEachNT(actualNt, index, seekedNt, indexElement + 1);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (!firstOfEachNT[actualNt].Contains(firstOfRightSide))
                            {
                                Debug.Log("  Add "+productions[indexProduction].GetRightSide()[indexElement + 1].GetSymbol()+" to "+actualNt.GetSymbol());
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
                    //if (productions[indexProduction].GetRightSide().Count)
                }
            }
            /*
            var __isBreaked = false;
            foreach (GrammarElement element in productions[indexProduction].GetRightSide())
            {
                Debug.Log("   In "+actualNt.GetSymbol()+" analise "+element.GetSymbol());
                if (element.IsNonTerminal())
                {
                    if (!voidableNT[element])
                    {
                        if (!seekedNt.Contains(firstOfRightSide))
                        {
                            Debug.Log("  Add "+firstOfRightSide.GetSymbol()+" to seekedNt");
                            seekedNt.Add(firstOfRightSide);
                            for (var index = 0; index < productions.Count; index++)
                            {
                                var production = productions[index];
                                if (production.GetLeftSide() == firstOfRightSide)
                                {
                                    FindFirstOfEachNT(actualNt, index, seekedNt);
                                }
                            }
                
                        }
                    }
                    else
                    {
                        __isBreaked = true;
                        break;
                    }
                }
                else
                {
                    if (!firstOfEachNT[actualNt].Contains(element)) firstOfEachNT[actualNt].Add(element);
                    __isBreaked = true;
                    break;
                }
            }

            if (!(__isBreaked) && (productions[indexProduction].GetLeftSide() != actualNt))
            {
                for (var index = 0; index < productions.Count; index++)
                {
                    if (productions[index].GetLeftSide() == productions[indexProduction].GetLeftSide() &&
                        !seekedNt.Contains(productions[index].GetLeftSide()))
                    {
                        seekedNt.Add(productions[index].GetLeftSide());
                        FindFirstOfEachNT(actualNt, index, seekedNt);
                    }
                }
            }
            */
        }
    }
}