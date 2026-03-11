using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.IO;

namespace ticket_17_app_pra_b2
{
    /// Ik definieer hier de interacties voor MainWindow.xaml
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void KnopUpdateInformatie_Click(object sender, RoutedEventArgs e)
        {
            VerwerkWachtrijSensorData();
            VerwerkAttractieStatusData();
        }

        private void VerwerkWachtrijSensorData()
        {
            string path = FindSensorFile("WachtrijSensoren.xml");
            if (path == null)
            {
                LabelWachtTijdMelding.Text = "Bronbestand niet gevonden";
                return;
            }

            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            if (doc.DocumentElement == null)
            {
                LabelWachtTijdMelding.Text = "Ongeldig bronbestand";
                return;
            }

            int tijd = 0;
            // Er zijn precies 12 sensoren voor Debug Hero: Sensor01..Sensor12
            for (int i = 1; i <= 12; i++)
            {
                string nodeName = $"Sensor{i:00}";
                var node = doc.DocumentElement.SelectSingleNode(nodeName);
                if (node == null) break; // ontbrekende sensor -> ik stop met tellen
                string value = node.InnerText?.Trim() ?? string.Empty;
                // ik accepteer "True" (ongeacht hoofdletters) of "1" als actieve sensor
                bool active = value.Equals("True", System.StringComparison.OrdinalIgnoreCase) || value == "1";
                if (!active) break;
                tijd += (i <= 2) ? 6 : 4;
            }

            LabelWachtTijdMelding.Text = tijd + " minuten";
        }

        private void VerwerkAttractieStatusData()
        {
            string path = FindSensorFile("AttractieStatus.xml");
            if (path == null)
            {
                LabelKar1.Text = "Kar 1: (bron niet gevonden)";
                LabelKar2.Text = "Kar 2: (bron niet gevonden)";
                LabelKar3.Text = "Kar 3: (bron niet gevonden)";
                return;
            }

            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            var kar1 = doc.SelectSingleNode("/Status/Kar01");
            var kar2 = doc.SelectSingleNode("/Status/Kar02");
            var kar3 = doc.SelectSingleNode("/Status/Kar03");

            if (kar1 != null)
                LabelKar1.Text = "Kar 1: " + ConvertStatus(kar1.InnerText);
            if (kar2 != null)
                LabelKar2.Text = "Kar 2: " + ConvertStatus(kar2.InnerText);
            if (kar3 != null)
                LabelKar3.Text = "Kar 3: " + ConvertStatus(kar3.InnerText);
        }

        private string ConvertStatus(string StatusNr)
        {
            switch (StatusNr)
            {
                case "1": return "uit/instappen";
                case "2": return "klaar voor vertrek";
                case "3": return "op avontuur";
                case "4": return "komt binnen";
                case "5": return "in onderhoud";
                default: return "";
            }
        }

        private string FindSensorFile(string fileName)
        {
            // Eenvoudige versie voor beginners:
            // ik ga er meestal van uit dat de XML-bestanden in Assets\SensorData naast de exe staan
            // ik probeer een paar logische locaties zodat de bestanden gevonden worden
            // zowel als ik vanuit de uitvoermap draai als vanuit de projectmap in Visual Studio
            var candidates = new[]
            {
                // relatief ten opzichte van de huidige werkmap
                System.IO.Path.Combine("Assets", "SensorData", fileName),
                // relatief ten opzichte van de applicatiebase (meestal bin/Debug/...)
                System.IO.Path.Combine(AppContext.BaseDirectory, "Assets", "SensorData", fileName),
                // probeer een paar bovenliggende mappen vanaf de appbase (handig bij bin-mappen)
                System.IO.Path.GetFullPath(System.IO.Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Assets", "SensorData", fileName)),
                System.IO.Path.GetFullPath(System.IO.Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Assets", "SensorData", fileName)),
                // current directory + Assets
                System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Assets", "SensorData", fileName)
            };

            foreach (var path in candidates)
            {
                try
                {
                    if (File.Exists(path)) return path;
                }
                catch
                {
                    // negeer ongeldige paden en ga door met de volgende kandidaat
                }
            }

            // als ik niets gevonden heb geef ik null terug zodat de aanroeper een melding kan tonen
            return null;
        }
    }
}