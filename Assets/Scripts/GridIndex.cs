
public struct GridIndex
{
    public int rowNumber;
    public int columnNumber;


    public GridIndex(int _row, int _column)
    {
        rowNumber = _row;
        columnNumber = _column;
    }


    public static bool operator ==(GridIndex g1, GridIndex g2)
    {
        return (g1.rowNumber == g2.rowNumber && g1.columnNumber == g2.columnNumber);
    }

    public static bool operator !=(GridIndex g1, GridIndex g2)
    {
        return (g1.rowNumber != g2.rowNumber || g1.columnNumber != g2.columnNumber);
    }

    public override bool Equals(object g1)
    {
        return (((GridIndex)g1).rowNumber == rowNumber && ((GridIndex)g1).columnNumber == columnNumber);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}

