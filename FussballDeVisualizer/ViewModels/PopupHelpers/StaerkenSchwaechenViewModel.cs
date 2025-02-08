using FusballDeScraper.Datenklassen.Spielereignisse;
using FussballDeVisualizer.Datenklassen;
using FussballDeVisualizer.Helper;
using ScottPlot;
using ScottPlot.TickGenerators.TimeUnits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace FussballDeVisualizer.ViewModels.PopupHelpers;

public class StaerkenSchwaechenViewModel : BaseViewModel
{
    public enum PlotType
    {
        Home,
        Away,
        HomeStrengths,
        AwayStrengths
    }

    public override string Title => "Staerken-Schwaechen";

    private List<TorGegentorDiagrammEintrag> _heimDaten;
    private string _heimName;
    private List<TorGegentorDiagrammEintrag> _auswaertsDaten;
    private string _auswaaertsName;

    private int[] geglätteteToreHeim;
    private int[] geglätteteToreAuswaerts;
    private int[] geglätteteGegentoreToreHeim;
    private int[] geglätteteGegentoreToreAuswaerts;
    private int[] splineDifferenzHeim;
    private int[] splineDifferenzAuswaerts;
    private double[] yReferenzHeim;
    private double[] yReferenzAuswaerts;

    public StaerkenSchwaechenViewModel(List<TorGegentorDiagrammEintrag> heimDaten, List<TorGegentorDiagrammEintrag> auswaertsDaten, string heimName, string auswaaertsName)
    {
        _heimDaten = heimDaten;
        _auswaertsDaten = auswaertsDaten;
        _heimName = heimName;
        _auswaaertsName = auswaaertsName;

        PreCalculateData();
    }

    private void PreCalculateData()
    {
        int[] toreHeim = _heimDaten.Select(e => e.Tore).ToArray();
        int[] gegentoreHeim = _heimDaten.Select(e => e.Gegentore).ToArray();

        geglätteteToreHeim = GewichteteGlättung(toreHeim);
        geglätteteGegentoreToreHeim = GewichteteGlättung(gegentoreHeim);

        splineDifferenzHeim = geglätteteToreHeim.Zip(geglätteteGegentoreToreHeim, (t, g) => t - g).ToArray();

        yReferenzHeim = Enumerable.Repeat(0.0, geglätteteToreHeim.Length).ToArray();

        int[] toreAuswaerts = _auswaertsDaten.Select(e => e.Tore).ToArray();
        int[] gegentoreAuswaerts = _auswaertsDaten.Select(e => e.Gegentore).ToArray();

        geglätteteToreAuswaerts = GewichteteGlättung(toreAuswaerts);
        geglätteteGegentoreToreAuswaerts = GewichteteGlättung(gegentoreAuswaerts);

        splineDifferenzAuswaerts = geglätteteToreAuswaerts.Zip(geglätteteGegentoreToreAuswaerts, (t, g) => t - g).ToArray();

        yReferenzAuswaerts = Enumerable.Repeat(0.0, geglätteteToreAuswaerts.Length).ToArray();
    }

    public void GeneratePlot(ScottPlot.WPF.WpfPlot plot, PlotType type)
    {
        switch (type)
        {
            case PlotType.Home:
                {
                    var toreLine = plot.Plot.Add.FillY(
                        _heimDaten.Select(e => (double)e.Minute).ToArray(),
                        geglätteteToreHeim.Select(e => (double)e).ToArray(),
                        yReferenzHeim);
                    toreLine.FillColor = Colors.Yellow.WithAlpha(80);

                    var gegentoreLine = plot.Plot.Add.FillY(
                        _heimDaten.Select(e => (double)e.Minute).ToArray(),
                        geglätteteGegentoreToreHeim.Select(e => (double)e).ToArray(),
                        yReferenzHeim);
                    gegentoreLine.FillColor = Colors.Black.WithAlpha(80);

                    var differenzLine = plot.Plot.Add.Scatter(
                        _heimDaten.Select(e => (double)e.Minute).ToArray(),
                        splineDifferenzHeim.Select(e => (double)e).ToArray());
                    differenzLine.Smooth = true;

                    plot.Plot.Title($"Stärke- und Schwäche- Phasen allgemein: {_heimName}");
                }
                break;

            case PlotType.Away:
                {
                    var toreLine = plot.Plot.Add.FillY(
                        _auswaertsDaten.Select(e => (double)e.Minute).ToArray(),
                        geglätteteToreAuswaerts.Select(e => (double)e).ToArray(),
                        yReferenzAuswaerts);
                    toreLine.FillColor = Colors.Yellow.WithAlpha(80);

                    var gegentoreLine = plot.Plot.Add.FillY(
                        _auswaertsDaten.Select(e => (double)e.Minute).ToArray(),
                        geglätteteGegentoreToreAuswaerts.Select(e => (double)e).ToArray(),
                        yReferenzAuswaerts);
                    gegentoreLine.FillColor = Colors.Black.WithAlpha(80);

                    var differenzLine = plot.Plot.Add.Scatter(
                        _auswaertsDaten.Select(e => (double)e.Minute).ToArray(),
                        splineDifferenzAuswaerts.Select(e => (double)e).ToArray());
                    differenzLine.Smooth = true;

                    plot.Plot.Title($"Stärke- und Schwäche- Phasen allgemein: {_auswaaertsName}");
                }
                break;
            case PlotType.HomeStrengths:
                {
                    var splineHomeStrength = geglätteteToreHeim.Zip(geglätteteGegentoreToreAuswaerts, (t, g) => t + g).ToArray();
                    var splineHomeWeakness = geglätteteGegentoreToreHeim.Zip(geglätteteToreAuswaerts, (t, g) => -(t + g)).ToArray();

                    var differenzLineStrength = plot.Plot.Add.Scatter(
                        _heimDaten.Select(e => (double)e.Minute).ToArray(),
                        splineHomeStrength.Select(e => (double)e).ToArray());
                    differenzLineStrength.Smooth = true;
                    differenzLineStrength.Color = Colors.Green;

                    var differenzLineWeakness = plot.Plot.Add.Scatter(
                        _heimDaten.Select(e => (double)e.Minute).ToArray(),
                        splineHomeWeakness.Select(e => (double)e).ToArray());
                    differenzLineWeakness.Smooth = true;
                    differenzLineWeakness.Color = Colors.Red;

                    plot.Plot.Title($"Stärke- und Schwäche- Phasen in Bezug auf den Gegner: {_heimName}");
                }
                break;
            case PlotType.AwayStrengths:
                {
                    var splineAwayStrength = geglätteteToreAuswaerts.Zip(geglätteteGegentoreToreHeim, (t, g) => t + g).ToArray();
                    var splineAwayWeakness = geglätteteGegentoreToreAuswaerts.Zip(geglätteteToreHeim, (t, g) => -(t + g)).ToArray();

                    var differenzLineStrength = plot.Plot.Add.Scatter(
                        _auswaertsDaten.Select(e => (double)e.Minute).ToArray(),
                        splineAwayStrength.Select(e => (double)e).ToArray());
                    differenzLineStrength.Smooth = true;
                    differenzLineStrength.Color = Colors.Green;

                    var differenzLineWeakness = plot.Plot.Add.Scatter(
                        _auswaertsDaten.Select(e => (double)e.Minute).ToArray(),
                        splineAwayWeakness.Select(e => (double)e).ToArray());
                    differenzLineWeakness.Smooth = true;
                    differenzLineWeakness.Color = Colors.Red;

                    plot.Plot.Title($"Stärke- und Schwäche- Phasen in Bezug auf den Gegner: {_auswaaertsName}");
                }

                break;
        }

        plot.Plot.XLabel("Spielminute");

        plot.Refresh();
    }

    private int[] GewichteteGlättung(int[] daten)
    {
        int[] result = new int[daten.Length];
        int maxAbstand = 4; // Maximaler Abstand für den Hügel

        for (int i = 0; i < daten.Length; i++)
        {
            if (daten[i] > 0) // Nur Tore berücksichtigen
            {
                for (int offset = -maxAbstand; offset <= maxAbstand; offset++)
                {
                    int pos = i + offset;
                    if (pos >= 0 && pos < daten.Length)
                    {
                        result[pos] += Math.Max(0, 5 - Math.Abs(offset)); // Gewicht hinzufügen
                    }
                }
            }
        }

        return result;
    }
}
