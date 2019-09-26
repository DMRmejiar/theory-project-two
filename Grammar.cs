public class Grammar
{
    List<GrammarProduction> productions;
    List<GrammarElement> NonTerminals;
    Dictionary<GrammarElement, bool> voidableNT;
    List<GrammarElement> voidableProductions;
    List<GrammarElement> firstOfEachNT;
    List<GrammarElement> firstOfEachProduction;
    List<GrammarElement> nextOfEachNT;
    List<GrammarElement> selectionOfEachProduction;
    
    string endOfSequence = "Ω";

    public void GenerateNonTerminals()
    {
        GrammarElement currentNT;
        
        foreach (GrammarProduction p in productions)
        {
            currentNT = p.GetLeftSide();
            if(!NonTerminals.Contains(currentNT))
            {
                NonTerminals.Add(currentNT);
            }
        }
    }

    public void Voidables()
    {
        foreach (GrammarProduction p in productions)
        {
            
        }
    }

}