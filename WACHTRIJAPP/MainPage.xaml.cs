using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace BugsOfHorrorXAML
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void KnopUpdateInformatie_Click(object sender, RoutedEventArgs e)
        {
            // wachttijd uit xml halen en label bijwerken
            XmlDocument doc = new XmlDocument();

            doc.Load("Assets\\SensorData\\WachtrijSensoren.xml");
            int tijd = 0;
            var list = doc.DocumentElement.ChildNodes;
            for (int i = 0; i < list.Count && i < 12; i++)
            {
                if (list[i].InnerText != "True") break;
                tijd += (i < 2) ? 6 : 4;
            }
            LabelWachtTijdMelding.Text = tijd + " minuten";

            // karstatus opvragen
            doc.Load("Assets\\SensorData\\AttractieStatus.xml");
            if (doc.SelectSingleNode("/Status/Kar01") != null)
                LabelKar1.Text = "Kar 1: " + ConvertStatus(doc.SelectSingleNode("/Status/Kar01").InnerText);
            if (doc.SelectSingleNode("/Status/Kar02") != null)
                LabelKar2.Text = "Kar 2: " + ConvertStatus(doc.SelectSingleNode("/Status/Kar02").InnerText);
            if (doc.SelectSingleNode("/Status/Kar03") != null)
                LabelKar3.Text = "Kar 3: " + ConvertStatus(doc.SelectSingleNode("/Status/Kar03").InnerText);
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
	}
}
