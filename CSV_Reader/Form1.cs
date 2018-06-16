using CsvHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSV_Reader
{
    public partial class Form1 : Form
    {
        public List<Client> clientList = new List<Client>();
        public Dictionary<string, List<Client>> files = new Dictionary<string, List<Client>>();    //slownik
        public string sourceFolder;
        public string onlyName;

        public Form1()
        {
            InitializeComponent();
        }

        private void otwórzToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = true; // multiselect
            openFileDialog1.InitialDirectory = @"c:\\";
            openFileDialog1.Filter = "CSV files (*.csv)|*.csv|XML files (*.xml)|*.xml";

            if (openFileDialog1.ShowDialog().Equals(DialogResult.OK))
            {
                foreach (String s in openFileDialog1.FileNames)//s sciezka do pliku
                {
                    List<Client> tempListClient = new List<Client>();

                    sourceFolder = s;        //sciezka do pliku
                    onlyName = Path.GetFileName(sourceFolder);         //sama nazwa 
                    var sr = new StreamReader(new FileStream(sourceFolder, FileMode.Open)); //otwarcie pliku 
                    int lineCounter = 1;
                    while (!sr.EndOfStream)
                    {
                        Client client = new Client();

                        var line = sr.ReadLine();
                        var val = line.Split(';');
                        if (lineCounter != 1)
                        {
                            client.liczbaPrzadkowa = val[0];
                            client.imieNazwisko = val[1];
                            client.miasto = val[2];
                            client.ulica = val[3];
                            client.umowa = val[4];
                            client.dataPodpisaniaUmowy = val[5];
                            client.dataPodpisaniaOstatniegoAP = val[6];
                            client.status = val[7];
                            client.rejestr = val[8];
                            client.rejestrPrzedWO = val[9];
                            client.osobaPodpisujaca = val[10];
                            client.telefon = val[11];
                            client.zadluzenie = val[12];
                            client.kwotaPozyczki = val[13];
                            client.saldo = val[14];
                            client.kwotaWplatyNarastajaco = val[15];
                            client.kwotaOstatnioNarastajacegoAP = val[16];
                            client.zaleglosciProcentowo = val[17];
                            client.zaleglosci = val[18];
                            client.pakietKomfort = val[19];
                            client.ostatniaWplata = val[20];
                            client.wysokoscRaty = val[21];
                            client.lkn = val[22];

                            tempListClient.Add(client);
                        }
                        else
                        {
                            for (int i = 0; i < dataGridView1.Columns.Count; i++)
                            {
                                dataGridView1.Columns[i].HeaderText = val[i];
                            }
                        }
                        lineCounter++;

                    }
                    sr.Close();
                    files.Add(onlyName, tempListClient);
                }
            }
            try
            {
                List<Client> temp = new List<Client>();
                foreach (var item in files)
                {
                    temp.AddRange(item.Value);
                }
                BindingSource bs = new BindingSource();
                bs.DataSource = temp;
                clientBindingSource.DataSource = bs;
                dataGridView1.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("nie ma rekordów!");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
         
        }

        private void zapiszToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "CSV | *.csv";
            
            if (saveFileDialog1.ShowDialog().Equals(DialogResult.OK))
            {
                string path = saveFileDialog1.FileName;
                string folder = Path.GetDirectoryName(path);
                
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Imie Nazwisko; Cel tygodnia; Realizacja; %Realizacja; Ilosc Wplat; Ilosc klientow; Uregulowane; Braki rzeczywiste; Procentowa Ilosc Wplat;"); //tabela generowana
                foreach (var item in files)
                {
                    string tempString = folder + @"\" + Path.GetFileNameWithoutExtension(item.Key) + "_wielokrotnosc.csv";
                    Representative rp = new Representative();

                    string imieNazwisko = item.Key;
                    float cel = rp.countTarget(item.Value);
                    float realizacja = rp.countResult(item.Value);
                    int ilosc = rp.countHowMany(item.Value);
                    int uregulowane = rp.countHowManyNegativ(item.Value);
                    float procent = (realizacja / cel) * 100;
                    int iloscKlientow = item.Value.Count();
                    int brakiRzeczywiste = iloscKlientow - ilosc - uregulowane;
                    float procentowaIloscWplat = ((float)ilosc / (float)iloscKlientow) * 100;

                    string line = imieNazwisko + "; " + cel.ToString("0.00") + "; " + realizacja.ToString() + "; " + procent.ToString("0.00") + "; " + ilosc.ToString() + "; " + iloscKlientow.ToString() + "; " + uregulowane.ToString() + "; " + brakiRzeczywiste.ToString() + "; " + procentowaIloscWplat.ToString("0.00") + ";";

                    sb.AppendLine(line);

                    rp.countSixTimes(item.Value);
                    rp.generateNewFile(tempString);
                }
                
                File.WriteAllText(path, sb.ToString());
            }
            
            
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs anError)
        {

            MessageBox.Show("Error happened " + anError.Context.ToString());

            if (anError.Context == DataGridViewDataErrorContexts.Commit)
            {
                MessageBox.Show("Commit error");
            }
            if (anError.Context == DataGridViewDataErrorContexts.CurrentCellChange)
            {
                MessageBox.Show("Cell change");
            }
            if (anError.Context == DataGridViewDataErrorContexts.Parsing)
            {
                MessageBox.Show("parsing error");
            }
            if (anError.Context == DataGridViewDataErrorContexts.LeaveControl)
            {
                MessageBox.Show("leave control error");
            }

            if ((anError.Exception) is ConstraintException)
            {
                DataGridView view = (DataGridView)sender;
                view.Rows[anError.RowIndex].ErrorText = "an error";
                view.Rows[anError.RowIndex].Cells[anError.ColumnIndex].ErrorText = "an error";

                anError.ThrowException = false;
            }
        }
    }
}
