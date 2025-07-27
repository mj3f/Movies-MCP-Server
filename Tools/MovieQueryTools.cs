using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;
using MoviesMcpServer.Services;

namespace MoviesMcpServer.Tools;

internal class MovieQueryTools
{
    private readonly MovieDataService _movieDataService;
    private readonly JsonSerializerOptions _jsonOptions;

    public MovieQueryTools(MovieDataService movieDataService)
    {
        _movieDataService = movieDataService;
        _jsonOptions = new JsonSerializerOptions { WriteIndented = true };
    }

    [McpServerTool]
    [Description("Search for movies by title. Returns movies that contain the search term in their title.")]
    public string SearchMoviesByTitle(
        [Description("Title or partial title to search for")] string title)
    {
        var movies = _movieDataService.SearchMoviesByTitle(title);
        return JsonSerializer.Serialize(movies, _jsonOptions);
    }

    [McpServerTool]
    [Description("Get movies released in a specific year.")]
    public string GetMoviesByYear(
        [Description("Year to filter movies by")] int year)
    {
        var movies = _movieDataService.GetMoviesByYear(year);
        return JsonSerializer.Serialize(movies, _jsonOptions);
    }

    [McpServerTool]
    [Description("Get movies in a specific language.")]
    public string GetMoviesByLanguage(
        [Description("Language code (e.g., 'en', 'ja', 'hi')")] string language)
    {
        var movies = _movieDataService.GetMoviesByLanguage(language);
        return JsonSerializer.Serialize(movies, _jsonOptions);
    }

    [McpServerTool]
    [Description("Get the top-rated movies ordered by vote average.")]
    public string GetTopRatedMovies(
        [Description("Number of movies to return (default: 10)")] int count = 10)
    {
        var movies = _movieDataService.GetTopRatedMovies(count);
        return JsonSerializer.Serialize(movies, _jsonOptions);
    }

    [McpServerTool]
    [Description("Get the most popular movies ordered by popularity score.")]
    public string GetMostPopularMovies(
        [Description("Number of movies to return (default: 10)")] int count = 10)
    {
        var movies = _movieDataService.GetMostPopularMovies(count);
        return JsonSerializer.Serialize(movies, _jsonOptions);
    }

    [McpServerTool]
    [Description("Get a specific movie by its ID.")]
    public string GetMovieById(
        [Description("Movie ID to retrieve")] int id)
    {
        var movie = _movieDataService.GetMovieById(id);
        if (movie == null)
        {
            return $"Movie with ID {id} not found.";
        }
        return JsonSerializer.Serialize(movie, _jsonOptions);
    }

    [McpServerTool]
    [Description("Get movies within a specific rating range.")]
    public string GetMoviesByRatingRange(
        [Description("Minimum rating (inclusive)")] double minRating,
        [Description("Maximum rating (inclusive)")] double maxRating)
    {
        Console.WriteLine($"Getting movies with ratings between {minRating} and {maxRating}");
        var movies = _movieDataService.GetMoviesByRatingRange(minRating, maxRating);
        return JsonSerializer.Serialize(movies, _jsonOptions);
    }

    [McpServerTool]
    [Description("Get all movies in the database.")]
    public string GetAllMovies()
    {
        var movies = _movieDataService.GetAllMovies();
        return JsonSerializer.Serialize(movies, _jsonOptions);
    }

    [McpServerTool]
    [Description("Get statistics about the movie database including total count, average rating, and language distribution.")]
    public string GetMovieStatistics()
    {
        var allMovies = _movieDataService.GetAllMovies().ToList();
        
        var stats = new
        {
            TotalMovies = allMovies.Count,
            AverageRating = allMovies.Average(m => m.VoteAverage),
            HighestRating = allMovies.Max(m => m.VoteAverage),
            LowestRating = allMovies.Min(m => m.VoteAverage),
            AveragePopularity = allMovies.Average(m => m.Popularity),
            LanguageDistribution = allMovies.GroupBy(m => m.OriginalLanguage)
                .Select(g => new { Language = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToList(),
            YearRange = new
            {
                EarliestYear = allMovies.Min(m => m.ReleaseDate.Year),
                LatestYear = allMovies.Max(m => m.ReleaseDate.Year)
            }
        };

        return JsonSerializer.Serialize(stats, _jsonOptions);
    }
}