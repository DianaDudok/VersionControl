using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Webszolgaltatas_5.het.MnbServiceReference;
using Webszolgaltatas_5.het.Entities;
using System.Xml;
using System.Windows.Forms.DataVisualization.Charting;

namespace Webszolgaltatas_5.het
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            RefreshData();
        }

        BindingList<RateData> Rates = new BindingList<RateData>();
        

        private string GetExchangeRates()
        {
            var mnbService = new MNBArfolyamServiceSoapClient();

            var request = new GetExchangeRatesRequestBody()
            {
                currencyNames = "EUR",
                startDate = "2020-01-01",
                endDate = "2020-06-30"
            };
            var response = mnbService.GetExchangeRates(request);

            var result = response.GetExchangeRatesResult;

            return result;
        }
        private void DokumentumFeldolgozas(string result) 
        {
            var xml = new XmlDocument();
            xml.LoadXml(result);
            foreach (XmlElement element in xml.DocumentElement)
            {
                var rate = new RateData();
                Rates.Add(rate);

                // Dátum
                rate.Date = DateTime.Parse(element.GetAttribute("date"));

                // Valuta
                var childElement = (XmlElement)element.ChildNodes[0];
                rate.Currency = childElement.GetAttribute("curr");

                // Érték
                var unit = decimal.Parse(childElement.GetAttribute("unit"));
                var value = decimal.Parse(childElement.InnerText);
                if (unit != 0)
                    rate.Value = value / unit;
            }
        }

        private void Diagram()
        {
            chartRateData.DataSource = Rates;

            var series = chartRateData.Series[0];
            series.ChartType = SeriesChartType.Line;
            series.XValueMember = "Date"; // X-ek az időpontok
            series.YValueMembers = "Value"; // Y-ok az értékek
            series.BorderWidth = 2; //adatsor vastagsága kétszeres

            var legend = chartRateData.Legends[0];
            legend.Enabled = false;

            var chartArea = chartRateData.ChartAreas[0];
            chartArea.AxisX.MajorGrid.Enabled = false; //Függőleges grid vonalak
            chartArea.AxisY.MajorGrid.Enabled = false; // Vízszintes grid vonalak
            chartArea.AxisY.IsStartedFromZero = false;  // Y értékek ne 0-nál kezdődjenek
        }

        private void RefreshData()
        {
            //GetExchangeRates();
            DokumentumFeldolgozas(GetExchangeRates());
            Diagram();
            dataGridView1.DataSource = Rates;
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
