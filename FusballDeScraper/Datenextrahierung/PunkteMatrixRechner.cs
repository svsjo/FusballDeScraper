using FusballDeScraper.Datenklassen.Tabelle;

namespace FusballDeScraper.Datenextrahierung;

public class PunkteMatrix
{
    public string Mannschaft { get; set; } // Name des Teams
    public Dictionary<string, int> PunkteDifferenzen { get; set; } = new(); // Punkte-Differenzen zu anderen Teams
}

public static class PunkteMatrixRechner
{
    public static List<PunkteMatrix> BerechnePunkteMatrix(List<VereinPlatzierung> tabelle)
    {
        var matrix = new List<PunkteMatrix>();

        foreach (var team in tabelle)
        {
            var eintrag = new PunkteMatrix { Mannschaft = team.Mannschaft };

            foreach (var anderesTeam in tabelle)
            {
                // Differenz berechnen
                int differenz = team.Punkte - anderesTeam.Punkte;
                eintrag.PunkteDifferenzen[anderesTeam.Mannschaft!] = differenz;
            }

            matrix.Add(eintrag);
        }

        return matrix;
    }
}



