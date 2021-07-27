using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Speech.Synthesis;
using System.IO;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

namespace Lector_de_voz
{
    public partial class Form1 : Form
    {
        SpeechSynthesizer voz;
        List<VoiceInfo> listarvoces = new List<VoiceInfo>();
        List<string> texto = new List<string>();
        public Form1()
        {
            InitializeComponent();
            voz = new SpeechSynthesizer();

            foreach (InstalledVoice x in voz.GetInstalledVoices())
            {
                listarvoces.Add(x.VoiceInfo);
                cbolistar.Items.Add(x.VoiceInfo.Name);
            }

            cbolistar.SelectedIndex = 0;
        }

        private void btnLeer_Click(object sender, EventArgs e)
        {
            voz = new SpeechSynthesizer();

            int ind;
            double volumen = trackVolumen.Value;
            double velocidad = trackVelocidad.Value;

            ind = cbolistar.SelectedIndex;
            String nombrevoces = listarvoces.ElementAt(ind).Name;
            voz.SelectVoice(nombrevoces);

            voz.Volume = (int)volumen;
            voz.Rate = (int)velocidad;
            foreach(string a in texto)
            {
                voz.SpeakAsync(a);
            }

            btnPausa.Text = "Pausa";
            
        }

        private void btnPausa_Click(object sender, EventArgs e)
        {
            if (voz != null)
            {

                if (voz.State == SynthesizerState.Speaking)
                {
                    voz.Pause(); btnPausa.Text = "Resumen";
                }
                else if (voz.State == SynthesizerState.Paused)
                {
                    voz.Resume(); btnPausa.Text = "Pausar";
                }

            }
        }

        private void btnDetener_Click(object sender, EventArgs e)
        {
            if (voz != null)
            {
                if (voz.State == SynthesizerState.Paused)
                {
                    voz.Resume(); btnPausa.Text = "Pausar";
                }
                voz.Dispose();

            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                voz = new SpeechSynthesizer();
                using (SaveFileDialog vai = new SaveFileDialog())
                {

                    vai.Filter = "Wav files|*.wav";
                    if (vai.ShowDialog() == DialogResult.OK)
                    {
                        int ind;
                        double volumen = trackVolumen.Value;
                        double velocidad = trackVelocidad.Value;

                        ind = cbolistar.SelectedIndex;
                        String nombrevoces = listarvoces.ElementAt(ind).Name;
                        voz.SelectVoice(nombrevoces);

                        FileStream team = new FileStream(vai.FileName, FileMode.Create, FileAccess.Write);
                        voz.SetOutputToWaveStream(team);
                        foreach (string a in texto)
                        {
                            voz.SpeakAsync(a);
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show("Intentar de nuevo", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAcercaDe_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            axAcroPDF1.src = openFileDialog1.FileName;
            leerPdf(axAcroPDF1.src);
        }

        private void axAcroPDF1_Enter(object sender, EventArgs e)
        {

        }

        private void leerPdf(string directorio)
        {
            int i = 0;
            var pdf = new PdfDocument(new PdfReader(directorio));
            var strategy = new LocationTextExtractionStrategy();
            

            for(i=1; i<= pdf.GetNumberOfPages(); i++)
            {
                var page = pdf.GetPage(i);
                texto.Add(PdfTextExtractor.GetTextFromPage(page));
            }
        }
    }
}
