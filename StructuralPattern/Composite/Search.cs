
namespace DesignPattern.StructuralPattern.Composite;

public enum SearchType
{
    Product,
    Post
}

public class Product
{
    public int Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

public class Post
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}

public class SearchResult
{
    public SearchType SearchType { get; set; }
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public interface ISearchService
{
    List<SearchResult> Search(string searchText);
}

public class ProductSearchService : ISearchService
{
    private readonly List<Product> _products =
    [
        new Product { Id = 1, ProductName = "iPhone 15", Price   = 1500 },
        new Product { Id = 2, ProductName = "Samsung S24", Price = 1300 }
    ];

    public List<SearchResult> Search(string keyword)
    {
        return _products
            .Where(p => p.ProductName.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            .Select(p => new SearchResult
            {
                SearchType  = SearchType.Product,
                Id          = p.Id,
                Title       = p.ProductName,
                Description = $"Price: {p.Price}"
            })
            .ToList();
    }
}

public class PostSearchService : ISearchService
{
    private readonly List<Post> _posts = new()
    {
        new Post { Id = 1, Title = "AI là gì?", Content      = "Giới thiệu AI" },
        new Post { Id = 2, Title = "Design Pattern", Content = "Composite Pattern" }
    };

    public List<SearchResult> Search(string keyword)
    {
        return _posts
            .Where(p => p.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                       || p.Content.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            .Select(p => new SearchResult
            {
                SearchType  = SearchType.Post,
                Id          = p.Id,
                Title       = p.Title,
                Description = p.Content
            })
            .ToList();
    }
}


public class CompositeSearchService : ISearchService
{
    private readonly List<ISearchService> _searchServices = new();

    public void Add(ISearchService searchService)
    {
        _searchServices.Add(searchService);
    }

    public List<SearchResult> Search(string keyword)
    {
        var results = new List<SearchResult>();

        foreach (var service in _searchServices)
        {
            results.AddRange(service.Search(keyword));
        }

        return results;
    }
}

