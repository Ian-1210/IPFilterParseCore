
public class CategorySearchItem
{
    public CategorySearchItemRow[] Rows { get; set; }
    public int RowCount { get; set; }
    public int Total { get; set; }
    public int Current { get; set; }
}

public class CategorySearchItemRow
{
    public string Uuid { get; set; }
    public string Name { get; set; }
    public string Auto { get; set; }
    public string Color { get; set; }
}
