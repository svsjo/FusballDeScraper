using System.Text.Json;
using FusballDeScraper;

public class DataManager
{
    private string _path = "data.json";

    public void SaveResults(Datenhaltung datenhaltung)
    {
        var jsonString = JsonSerializer.Serialize(datenhaltung);
        System.IO.File.WriteAllText(_path, jsonString);
    }

    public Datenhaltung? GetResults()
    {
        var jsonString = System.IO.File.ReadAllText(_path);
        return JsonSerializer.Deserialize<Datenhaltung>(jsonString);
    }
}

