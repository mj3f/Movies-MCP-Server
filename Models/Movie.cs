namespace MoviesMcpServer.Models;

public class Movie
{
    public int Id { get; set; }
    public string OriginalLanguage { get; set; } = string.Empty;
    public string Overview { get; set; } = string.Empty;
    public DateTime ReleaseDate { get; set; }
    public string Title { get; set; } = string.Empty;
    public double Popularity { get; set; }
    public double VoteAverage { get; set; }
    public int VoteCount { get; set; }
}