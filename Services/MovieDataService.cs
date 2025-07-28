using System.Globalization;
using MoviesMcpServer.Models;

namespace MoviesMcpServer.Services;

public class MovieDataService
{
    private readonly List<Movie> _movies = new();
    private readonly string _csvFilePath;

    public MovieDataService()
    {
        _csvFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "tmdb_top_rated_movies.csv");
        LoadMoviesFromCsv();
    }


    public IEnumerable<Movie> GetAllMovies() => _movies;

    public IEnumerable<Movie> SearchMoviesByTitle(string title)
    {
        return _movies.Where(m => m.Title.Contains(title, StringComparison.OrdinalIgnoreCase));
    }

    public IEnumerable<Movie> GetMoviesByYear(int year)
    {
        return _movies.Where(m => m.ReleaseDate.Year == year);
    }

    public IEnumerable<Movie> GetMoviesByLanguage(string language)
    {
        return _movies.Where(m => m.OriginalLanguage.Equals(language, StringComparison.OrdinalIgnoreCase));
    }

    public IEnumerable<Movie> GetTopRatedMovies(int count = 10)
    {
        return _movies.OrderByDescending(m => m.VoteAverage).Take(count);
    }

    public IEnumerable<Movie> GetLowestRatedMovies(int count = 10)
    {
        return _movies.OrderBy(m => m.VoteAverage).Take(count);
    }

    public IEnumerable<Movie> GetMostPopularMovies(int count = 10)
    {
        return _movies.OrderByDescending(m => m.Popularity).Take(count);
    }

    public Movie? GetMovieById(int id)
    {
        return _movies.FirstOrDefault(m => m.Id == id);
    }

    public IEnumerable<Movie> GetMoviesByRatingRange(double minRating, double maxRating)
    {
        return _movies.Where(m => m.VoteAverage >= minRating && m.VoteAverage <= maxRating);
    }

    private void LoadMoviesFromCsv()
    {
        if (!File.Exists(_csvFilePath))
        {
            throw new FileNotFoundException($"CSV file not found at: {_csvFilePath}");
        }

        var lines = File.ReadAllLines(_csvFilePath);
        
        // Skip header line
        for (int i = 1; i < lines.Length; i++)
        {
            var movie = ParseMovieFromCsvLine(lines[i]);
            if (movie != null)
            {
                _movies.Add(movie);
            }
        }
    }

    private Movie? ParseMovieFromCsvLine(string csvLine)
    {
        try
        {
            var values = ParseCsvLine(csvLine);
            if (values.Count < 8) return null;

            return new Movie
            {
                Id = int.Parse(values[0]),
                OriginalLanguage = values[1],
                Overview = values[2],
                ReleaseDate = DateTime.ParseExact(values[3], "yyyy-MM-dd", CultureInfo.InvariantCulture),
                Title = values[4],
                Popularity = double.Parse(values[5], CultureInfo.InvariantCulture),
                VoteAverage = double.Parse(values[6], CultureInfo.InvariantCulture),
                VoteCount = int.Parse(values[7])
            };
        }
        catch
        {
            return null;
        }
    }

    private List<string> ParseCsvLine(string line)
    {
        var values = new List<string>();
        var inQuotes = false;
        var currentValue = "";

        for (int i = 0; i < line.Length; i++)
        {
            var c = line[i];

            if (c == '"' && (i == 0 || line[i - 1] != '\\'))
            {
                inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes)
            {
                values.Add(currentValue);
                currentValue = "";
            }
            else
            {
                currentValue += c;
            }
        }

        values.Add(currentValue);
        return values;
    }
}