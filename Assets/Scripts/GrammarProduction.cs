import System.collections;

public class GrammarProduction
{
    private GrammarElement leftSide;
    private List<GrammarElement> rightSide;
    
    GrammarProduction(GrammarElement leftSide, List<GrammarElement> rightSide)
    {
        this.leftSide = leftSide;
        this.rightSide = rightSide;
    }

    public GrammarElement GetLeftSide()
    {
        return leftSide;
    }

    public List<GrammarElement> GetRightSide()
    {
        return rightSide;
    }
}