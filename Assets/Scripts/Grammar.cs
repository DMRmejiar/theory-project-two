using System.Collections.Generic;

public class Grammar
{
    List<GrammarProduction> productions;
    List<GrammarElement> nonTerminals;
    Dictionary<GrammarElement, bool> voidableNT;
    List<bool> voidableProductions;
    List<List<GrammarElement>> firstOfEachNT;
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
                bool __isVoidable = true;
                foreach (GrammarElement element in productions[i].GetRightSide())
                {
                    if (!element.IsNonTerminal() || !voidableNT[element])
                    {
                        __isVoidable = false;
                        break;
                    }
                }
                if (__isVoidable)
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
        firstOfEachNT = new List<List<GrammarElement>>();
        int indexOfNT;
        foreach (GrammarElement nt in nonTerminals)
        {
            firstOfEachNT.Add(new List<GrammarElement>());
        }
        for (int i = 0; i < productions.Count; i++)
        {
            indexOfNT = nonTerminals.IndexOf(productions[i].GetLeftSide());
            if (!voidableProductions[i] && !productions[i].GetRightSide()[0].IsNonTerminal())
            {
                if (!firstOfEachNT[indexOfNT].Contains(productions[i].GetRightSide()[0]))
                {
                    firstOfEachNT[indexOfNT].Add(productions[i].GetRightSide()[0]);
                }
            }
        }
    }
}