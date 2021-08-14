using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QA402_REST_TEST
{
    public partial class Form1 : Form
    {
        CancellationTokenSource Ct;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Tlp.SuspendLayout();
            Tlp.Controls.Clear();
            Tlp.ColumnStyles.Clear();
            Tlp.RowStyles.Clear();
            Tlp.RowCount = 10;
            Tlp.ColumnCount = 1;
            Tlp.Dock = DockStyle.Fill;

            TGroupBox tgb = new TGroupBox("Basic");
            tgb.Height = 190;

            TFlowLayoutPanel tflp = new TFlowLayoutPanel();

            //
            // Most of the code below is just scaffolding for the UI. Search below for the method
            // MakeMeasurement() and this shows the type of flow you'd likely want to use for your
            // custom test routines. The procedure is as follows:
            //    1. Setup test parameters
            //    2. Do the acquisition
            //    3. Ask the Qa40x application to make measurements on the acquired data. Additionally,
            //       you can pull over the input time or frequency series and do your processing on the
            //       data. The time series is in volts, the frequency series is in linear units
            //


            //
            // If you aren't familiar with C#, the code below might be confusing. But all it's doing
            // is defining an inline function that will block until completed, while still allowing
            // background processing. So, when you click the button labeled "GET /Status/Version" then
            // the Qa402.GetVersion() code is called, and when it returns a string is displayed
            // 

            string sVersion = "GET /Status/Version";
            tflp.Controls.Add(new TButton(sVersion, async () => { await RunnerReturnDouble(() => Qa402.GetVersion(), sVersion + "  Version: {0}"); }));

            string sConnection = "GET /Status/Connection";
            tflp.Controls.Add(new TButton(sConnection, async () => { await RunnerReturnBool(() => Qa402.IsConnected(), sConnection + "  IsConnected: {0}"); }));

            string sDefault = "PUT /Settings/Default";
            tflp.Controls.Add(new TButton(sDefault, async () => { await RunnerNoReturn(() => Qa402.SetDefaults(), sDefault); }));

            string sMax42 = "PUT /Settings/Input/Max/42";
            tflp.Controls.Add(new TButton(sMax42, async () => { await RunnerNoReturn(() => Qa402.SetInputRange(42), sMax42); }));

            string sMax0 = "/Settings/Input/Max/0";
            tflp.Controls.Add(new TButton(sMax0, async () => { await RunnerNoReturn(() => Qa402.SetInputRange(0), sMax0); }));

            string bufferSizeBig = "/Settings/BuffersSize/2^18";
            tflp.Controls.Add(new TButton(bufferSizeBig, async () => { await RunnerNoReturn(() => Qa402.SetBufferSize((uint)Math.Pow(2, 18)), bufferSizeBig); }));

            string bufferSizeSmall = "/Settings/BuffersSize/2^12";
            tflp.Controls.Add(new TButton(bufferSizeSmall, async () => { await RunnerNoReturn(() => Qa402.SetBufferSize((uint)Math.Pow(2, 12)), bufferSizeSmall); }));

            string setWindowRectangle = "/Settings/Windowing/Rectangle";
            tflp.Controls.Add(new TButton(setWindowRectangle, async () => { await RunnerNoReturn(() => Qa402.SetWindowing(Windowing.Rectangle), setWindowRectangle); }));

            string setWindowFlatTop = "/Settings/Windowing/FlatTop";
            tflp.Controls.Add(new TButton(setWindowFlatTop, async () => { await RunnerNoReturn(() => Qa402.SetWindowing(Windowing.FlatTop), setWindowFlatTop); }));

            string setSampleRate192 = "/Settings/SampleRate/192000";
            tflp.Controls.Add(new TButton(setSampleRate192, async () => { await RunnerNoReturn(() => Qa402.SetSampleRate(192000), setSampleRate192); }));
            string setSampleRate48 = "/Settings/SampleRate/48000";
            tflp.Controls.Add(new TButton(setSampleRate48, async () => { await RunnerNoReturn(() => Qa402.SetSampleRate(48000), setSampleRate48); }));

            string setWeightingNone = "/Settings/Weighting/None";
            tflp.Controls.Add(new TButton(setWeightingNone, async () => { await RunnerNoReturn(() => Qa402.SetWeighting(Weighting.None), setWeightingNone); }));

            string setWeightingAWeighting = "/Settings/Weighting/AWeighting";
            tflp.Controls.Add(new TButton(setWeightingAWeighting, async () => { await RunnerNoReturn(() => Qa402.SetWeighting(Weighting.AWeighting), setWeightingAWeighting); }));

            string idleGenOn = "/Settings/IdleGen/On";
            tflp.Controls.Add(new TButton(idleGenOn, async () => { await RunnerNoReturn(() => Qa402.SetIdleGen(true), idleGenOn); }));

            string idleGenOff = "/Settings/IdleGen/Off";
            tflp.Controls.Add(new TButton(idleGenOff, async () => { await RunnerNoReturn(() => Qa402.SetIdleGen(false), idleGenOff); }));

            string sourceSine = "/Settings/OutputSource/Sine";
            tflp.Controls.Add(new TButton(sourceSine, async () => { await RunnerNoReturn(() => Qa402.SetOutputSource(OutputSources.Sine), sourceSine); }));

            string sourceNoise = "/Settings/OutputSource/WhiteNoise";
            tflp.Controls.Add(new TButton(sourceNoise, async () => { await RunnerNoReturn(() => Qa402.SetOutputSource(OutputSources.WhiteNoise), sourceNoise); }));

            string sourceExpoChirp = "/Settings/OutputSource/ExpoChirp";
            tflp.Controls.Add(new TButton(sourceExpoChirp, async () => { await RunnerNoReturn(() => Qa402.SetOutputSource(OutputSources.ExpoChirp), sourceNoise); }));

            string gen1Level = "/Settings/AudioGen/Gen1/On/1000/-10";
            tflp.Controls.Add(new TButton(gen1Level, async () => { await RunnerNoReturn(() => Qa402.SetGen1(1000, -10, true), gen1Level); }));

            string NoiseGenLevel = "/Settings/NoiseGen/-10";
            tflp.Controls.Add(new TButton(NoiseGenLevel, async () => { await RunnerNoReturn(() => Qa402.SetNoiseGen(-10), NoiseGenLevel); }));

            string ExpoChirp = "/Settings/ExpoChirpGen/-10/0.0/48/False/True";
            tflp.Controls.Add(new TButton(ExpoChirp, async () => { await RunnerNoReturn(() => Qa402.SetExpoChirpGen(-10, 0, 48, false), ExpoChirp); }));

            tgb.Controls.Add(tflp);

            Tlp.Controls.Add(tgb);

            tgb = new TGroupBox("Measurements");
            tflp = new TFlowLayoutPanel();
            string sThd = "GET ThdDb/1000/20000";
            tflp.Controls.Add(new TButton(sThd, async () => { await RunnerReturnDoublePair(() => Qa402.GetThdDb(1000, 20000), sThd + " Left:{0:0.00}dB  Right: {1:0.00}dB"); }));

            string sSnr = "GET SnrDb/1000/20/20000";
            tflp.Controls.Add(new TButton(sSnr, async () => { await RunnerReturnDoublePair(() => Qa402.GetSnrDb(1000, 20, 20000), sSnr + " Left:{0:0.00}dB  Right: {1:0.00}dB"); }));

            string sRms = "GET RmsDbv/20/2000";
            tflp.Controls.Add(new TButton(sRms, async () => { await RunnerReturnDoublePair(() => Qa402.GetRmsDbv(20, 20000), sRms + " Left:{0:0.00}dB  Right: {1:0.00}dB"); }));
            tgb.Controls.Add(tflp);

            string sFreq = "GET Data/Frequency/Input";
            tflp.Controls.Add(new TButton(sFreq, async () => { await RunnerReturnDoubleArray(() => Qa402.GetInputFrequencySeries(), sFreq + " dF:{0:0.00}"); }));
            tgb.Controls.Add(tflp);

            Tlp.Controls.Add(tgb);

            tgb = new TGroupBox("Acquisition");
            tflp = new TFlowLayoutPanel();

            string sAcq = "POST /Acquisition";
            tflp.Controls.Add(new TButton(sAcq, async () => { await RunnerNoReturn(() => Qa402.DoAcquisition(), sAcq); }));
            tgb.Controls.Add(tflp);

            string sAcqAsync = "POST /AcquisitionAsync";
            tflp.Controls.Add(new TButton(sAcqAsync, async () => { await RunnerNoReturn(() => Qa402.DoAcquisitionAsync(), sAcqAsync); }));
            tgb.Controls.Add(tflp);

            string sAcqBusy = "POST /AcquisitionBusy";
            tflp.Controls.Add(new TButton(sAcqBusy, async () => { await RunnerReturnBool(() => Qa402.IsBusy(), sAcqBusy + " Busy: {0}"); }));
            tgb.Controls.Add(tflp);

            string sStartAudition = "POST /AuditionStart/-2/TestFile.wav/0.9/true";
            tflp.Controls.Add(new TButton(sStartAudition, async () => { await RunnerReturnBool(() => Qa402.AuditionStart("TestFile.wav", -2, 0.9, false), sStartAudition); }));
            tgb.Controls.Add(tflp);

            string sStopAudition = "POST /AuditionStop";
            tflp.Controls.Add(new TButton(sStopAudition, async () => { await RunnerReturnBool(() => Qa402.AuditionStop(), sStopAudition); }));
            tgb.Controls.Add(tflp);

            string sStepFreq = "Step Frequency";
            tflp.Controls.Add(new TButton(sStepFreq, async () => { await RunnerReturnBool(() => StepFrequency(), sStepFreq); }));
            tgb.Controls.Add(tflp);

            string sMeasureFreqResponse = "Measure FreqResponse";
            tflp.Controls.Add(new TButton(sMeasureFreqResponse, async () => { await RunnerReturnBool(() => MeasureFreqResponse(), sMeasureFreqResponse); }));
            tgb.Controls.Add(tflp);

            string sFullMeasurementStart = "Full Measurement Start";
            tflp.Controls.Add(new TButton(sFullMeasurementStart, async () => 
            {
                Ct?.Dispose();
                Ct = new CancellationTokenSource();
                await MakeMeasurement(Ct.Token); 
            }));
            tgb.Controls.Add(tflp);

            string sFullMeasurementStop = "Full Measurement Stop";
            tflp.Controls.Add(new TButton(sFullMeasurementStop, async () =>
            {
                if (Ct == null)
                    return;

                Ct.Cancel();
            }));
            tgb.Controls.Add(tflp);

            string sStartRunning = "Start Running";
            tflp.Controls.Add(new TButton(sStartRunning, async () =>
            {
                Ct?.Dispose();
                Ct = new CancellationTokenSource();
                await RunUntilStopped(Ct.Token);
            }));
            tgb.Controls.Add(tflp);

            string sStopRunning = "Stop Running";
            tflp.Controls.Add(new TButton(sStopRunning, async () =>
            {
                if (Ct == null)
                    return;

                Ct.Cancel();
            }));
            tgb.Controls.Add(tflp);
            
            
            Tlp.Controls.Add(tgb);
            Tlp.ResumeLayout();
        }

        async Task<bool> StepFrequency()
        {
            LeftRightPair lrp;
            double freq;

            // Load a settings file with the particulars we want
            await Qa402.SetDefaults("test16.settings");
            await Qa402.SetBufferSize(8192);
            await Qa402.SetInputRange(6);

            // First measurement at 50 Hz
            freq = 50;
            await Qa402.SetGen1(freq, -2, true);
            await Qa402.DoAcquisition();
            // Check left and right measurements to make sure they make sense
            lrp = await Qa402.GetRmsDbv(20, 22000);
            if (lrp.Left < -4 || lrp.Left > 0)
            {
                // fail...the measured amplitude was not withing -4 dBV to 0 dBV
            }
            lrp = await Qa402.GetThdDb(freq, 22000);
            if (lrp.Left < -130 || lrp.Left > -90)
            {
                // fail...the measured thd was not withing -130 to -90 dB
            }

            // Now repeat for other frequencies of interest. We'll ignore
            freq = 100;
            await Qa402.SetGen1(freq, -2, true);
            await Qa402.DoAcquisition();
            lrp = await Qa402.GetRmsDbv(20, 22000);
            lrp = await Qa402.GetThdDb(freq, 22000);

            freq = 1000;
            await Qa402.SetGen1(freq, -2, true);
            await Qa402.DoAcquisition();
            lrp = await Qa402.GetRmsDbv(20, 22000);
            lrp = await Qa402.GetThdDb(freq, 22000);

            freq = 10000;
            await Qa402.SetGen1(freq, -2, true);
            await Qa402.DoAcquisition();
            lrp = await Qa402.GetRmsDbv(20, 22000);
            lrp = await Qa402.GetThdDb(freq, 22000);

            freq = 20000;
            await Qa402.SetGen1(freq, -2, true);
            await Qa402.DoAcquisition();
            lrp = await Qa402.GetRmsDbv(20, 22000);

            return true;
        }

        async Task<bool> MeasureFreqResponse()
        {
            double freq, ampDbv;
            int bin;

            // Set defaults and the set options
            await Qa402.SetDefaults();
            await Qa402.SetBufferSize(32768);
            await Qa402.SetInputRange(6);

            // Set up chirp generation
            await Qa402.SetOutputSource(OutputSources.ExpoChirp);
            // Set -10 dBV sweep, with no windowing and no smoothing, and do NOT use right channel as reference
            await Qa402.SetExpoChirpGen(-10, 0.0, 0, false);

            // Do the acqusition. This will take about 1 second (32768/48000 = 682 mS)
            await Qa402.DoAcquisition();

            // Pull across the frequency series. The is an array of doubles for the
            // left channel, and an array of doubles for the right. The Df parameter
            // indicates the bin spacing in Hz
            LeftRightFrequencySeries lrfs = await Qa402.GetInputFrequencySeries();

            // Compute amplitude in dBV at 100 Hz, and deterine if OK
            freq = 100;
            // Find the FFT bin number of the frequency we want
            bin = (int)Math.Round(freq / lrfs.Df);
            // Convert to dBV
            ampDbv = 20 * Math.Log10(lrfs.Left[bin]);

            // Compute amplitude in dBV at 1 kHz, and deterine if OK
            freq = 1000;
            bin = (int)Math.Round(freq / lrfs.Df);
            ampDbv = 20 * Math.Log10(lrfs.Left[bin]);

            // Compute amplitude in dBV at 20 kHz, and deterine if OK
            freq = 20000;
            bin = (int)Math.Round(freq / lrfs.Df);
            ampDbv = 20 * Math.Log10(lrfs.Left[bin]);

            return true;
        }


        /// <summary>
        /// This function sets several parameters and collects the measurement result. This can act as a stress test and run
        /// over and over. Notice that each method/function is prefaced with an 'await'. This is done because the QA402 methods
        /// were written to be async. You could chose to to write everything to be async or sync. That will depend on the library
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        async Task MakeMeasurement(CancellationToken ct)
        {
            bool randomLevel = true; // Set to true for stress testing. Makes it easy to hear if stress is still running
            Random r = new Random();

            // Caller will set the cancellation token when it's time to stop
            while (ct.IsCancellationRequested == false)
            {
                if (randomLevel)
                    await Qa402.SetGen1(1000, r.Next(-40, 10), true);
                else
                    await Qa402.SetGen1(1000, 0, true);

                await Qa402.SetBufferSize(8192);

                // Do the acquisition. The await operator means the execution of this method will
                // block until the operation (in this case, DoAcquisition() is complete. 
                await Qa402.DoAcquisition();

                // Now that the acquisition is complete, we can ask the QA402 to compute THD with 
                // a fundamental of 1000 and a max harmonic level up to 20K
                LeftRightPair lrp = await Qa402.GetThdDb(1000, 20000);
                LogLine($" THD   LEFT: {lrp.Left:0.00} dB   RIGHT: {lrp.Right:0.00} dB");

                // We can compute the THDN on the same data. In this case, we compute the THDN assuming
                // a 1 kHz fundamental, and a noise bandwidth from 20 to 20k
                lrp = await Qa402.GetThdnDb(1000, 20, 20000);
                LogLine($" THDN   LEFT: {lrp.Left:0.00} dB   RIGHT: {lrp.Right:0.00} dB");

                // Combpute the RMS from 20 to 20 kHz.
                lrp = await Qa402.GetRmsDbv(20, 20000);
                LogLine($" RMS   LEFT: {lrp.Left:0.00} dB   RIGHT: {lrp.Right:0.00} dB");

                // With sig gen set to 0 dBV above, this means a time series would have a sine peak of Sqrt(2) or 1.41V
                LeftRightTimeSeries lrts = await Qa402.GetInputTimeSeries();
                // Do whatever processing you neeed on the raw time series

                // With sig gen set to 0 dBV above, this means a freq series would have a sine peak of 1.00 = 0 dBV
                LeftRightFrequencySeries lrfs = await Qa402.GetInputFrequencySeries();
                // Do whatever processing you need on the raw freq series
            }
        }

        /// <summary>
        /// Shows how to update the frequency display repeatedly while the test app UI still remains responsive. This can 
        /// be useful if test personnel are required to tune parameters while looking at a display (for example, to adjust
        /// frequency or amplitude while turning a potentiometer).
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        async Task RunUntilStopped(CancellationToken ct)
        {
            // Caller will set the cancellation token when it's time to stop
            while (ct.IsCancellationRequested == false)
            {
                // Do the acquisition. The await operator means the execution of this method will
                // block until the operation (in this case, DoAcquisition() is complete. 
                await Qa402.DoAcquisition();
            }
        }

        /// <summary>
        /// UI support code
        /// </summary>
        /// <param name="funcToRun"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        async Task RunnerReturnDouble(Func<Task<double>> funcToRun, string s)
        {
            try
            {
                double x = await funcToRun();

                LogLine(string.Format(s, x));
            }
            catch (Exception ex)
            {
                LogLine($"Exception: {ex.Message}");
            }
        }

        /// <summary>
        /// UI support code
        /// </summary>
        /// <param name="funcToRun"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        async Task RunnerReturnDoublePair(Func<Task<LeftRightPair>> funcToRun, string s)
        {
            try
            {
                LeftRightPair lrp = await funcToRun();

                LogLine(string.Format(s, lrp.Left, lrp.Right));
            }
            catch (Exception ex)
            {
                LogLine($"Exception: {ex.Message}");
            }
        }

        async Task RunnerReturnDoubleArray(Func<Task<LeftRightFrequencySeries>> funcToRun, string s)
        {
            try
            {
                LeftRightFrequencySeries lrp = await funcToRun();

                LogLine(string.Format(s, lrp.Df));
            }
            catch (Exception ex)
            {
                LogLine($"Exception: {ex.Message}");
            }
        }

        /// <summary>
        /// UI support code
        /// </summary>
        /// <param name="funcToRun"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        async Task RunnerReturnBool(Func<Task<bool>> funcToRun, string s)
        {
            try
            {
                bool b = await funcToRun();

                LogLine(string.Format(s, b.ToString()));
            }
            catch (Exception ex)
            {
                LogLine($"Exception: {ex.Message}");
            }
        }

        /// <summary>
        /// UI support code
        /// </summary>
        /// <param name="funcToRun"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        async Task RunnerNoReturn(Func<Task> funcToRun, string s)
        {
            try
            {
                Log("Starting...");
                await funcToRun();
                LogLine("Done.  " + s);
            }
            catch (Exception ex)
            {
                LogLine($"Exception: {ex.Message}");
            }
        }

        void Log(string s)
        {
            textBox1.AppendText(s);
        }

        void LogLine(string s)
        {
            textBox1.AppendText(s + Environment.NewLine);
        }

    }
}
