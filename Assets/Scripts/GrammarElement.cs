public class GrammarElement
{
    private bool isNonTerminal;
    private string symbol;

    GrammarElement(bool isNonTerminal, string symbol)
    {
        this.isNonTerminal = isNonTerminal;
        this.symbol = symbol;
    }

    public bool GetIsNonTerminal()
    {
        return isNonTerminal;
    }

    public string GetSymbol()
    {
        return symbol;
    }
}