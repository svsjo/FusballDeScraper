namespace FusballDeScraper.Datenklassen.Spiele;

public class Spiel
{
    public string? HeimTeam { get; set; }
    public string? AuswaertsTeam { get; set; }
    public string? Platzart { get; set; }
    public string? Ort { get; set; }
    public DateTime? DateTime { get; set; }
    public int Spieltag { get; set; }
}
