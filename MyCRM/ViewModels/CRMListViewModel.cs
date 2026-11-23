namespace MyCRM.ViewModels;

public sealed record CrmListViewModel(
    string Title,
    string EmptyMessage,
    IReadOnlyList<string> Columns)
{
    public IReadOnlyList<IReadOnlyList<string>> Rows { get; init; } = [];
}
