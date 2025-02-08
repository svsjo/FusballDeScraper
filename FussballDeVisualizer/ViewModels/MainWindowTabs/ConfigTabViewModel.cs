using FusballDeScraper;
using FusballDeScraper.Datenklassen;
using FussballDeVisualizer.Datenklassen;
using FussballDeVisualizer.Helper;
using FussballDeVisualizer.ViewModels.PopupHelpers;
using FussballDeVisualizer.Views;
using FussballDeVisualizer.Views.MainWindowTabs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FussballDeVisualizer.ViewModels.MainWindowTabs;

public class ConfigTabViewModel : TabViewModel
{
    public override object View => new ConfigTabView();
    public override string Title => "Config";

    public EventHandler StartedExtractionEventHandler;
    public EventHandler TableFinishedEventHandler;

    private string _ligaUrl = @"https://www.fussball.de/spieltagsuebersicht/bezirksliga-herren-bezirk-offenburg-bezirksliga-herren-saison2425-suedbaden/-/staffel/02PTTC6NHK000006VS5489B4VVTKJJ35-G#!/";
    public string LigaUrl
    {
        get => _ligaUrl;
        set
        {
            _ligaUrl = value;
            OnPropertyChanged(nameof(LigaUrl));
        }
    }

    private ObservableCollection<string> _consoleOutput = new ObservableCollection<string>();
    public ObservableCollection<string> ConsoleOutput
    {
        get => _consoleOutput;
        set
        {
            _consoleOutput = value;
            OnPropertyChanged(nameof(ConsoleOutput));
        }
    }

    private Liga? _liga;
    public Liga? Liga
    {
        get => _liga;
        set => _liga = value;
    }

    private string _status;
    public string Status
    {
        get => _status;
        set { 
            _status = value; 
            OnPropertyChanged(nameof(Status)); 
        }
    }

    public ICommand OnStartLigaExtractionCommand { get; set; }
    public ICommand TestCommand { get; set; }

    public ConfigTabViewModel()
    {
        // Umleiten der Konsolenausgabe
        Console.SetOut(new ViewModelTextWriter(this));

        // Commands
        OnStartLigaExtractionCommand = new RelayCommand(_ => StartLigaExtraction());

        TestCommand = new RelayCommand(_ => TestMethod());
    }

    public void TestMethod()
    {
        var datasHome = GetAggregierteDaten();
        var datasAway = GetAggregierteDaten();
        var vm = new StaerkenSchwaechenViewModel(datasHome, datasAway, "Sv Schapbach", "Sc Lahr 2");
        var popup = new StaerkenSchwachenView(vm);
        popup.Show();
    }

    private List<TorGegentorDiagrammEintrag> GetAggregierteDaten()
    {
        Random rand = new Random();
        var daten = new List<TorGegentorDiagrammEintrag>();

        for (int minute = 0; minute <= 90; minute++)
        {
            daten.Add(new TorGegentorDiagrammEintrag
            {
                Minute = minute,
                Tore = rand.Next(0, 2), // Realistische Tore (0 oder 1 pro Minute)
                Gegentore = rand.Next(0, 2) // Realistische Gegentore
            });
        }

        return daten;
    }

    public void StartLigaExtraction()
    {
        StartLigaExtraction(_ligaUrl);
    }

    public async void StartLigaExtraction(string ligaUrl)
    {
        var progress = new Progress<string>(status => 
        {
            Status = status;

            if (Liga == default && Datenhaltung.Liga != default)
            {
                Liga = Datenhaltung.Liga;
                StartedExtractionEventHandler.Invoke(this, EventArgs.Empty);
            }

            if (Liga?.Tabelle != default && status == "Lade Torschützen...")
            {
                TableFinishedEventHandler.Invoke(this, EventArgs.Empty);
            }
        });

        await Task.Run(async () =>
        {
            await Datenhaltung.Initialize(ligaUrl, progress);
        });
    }
}
