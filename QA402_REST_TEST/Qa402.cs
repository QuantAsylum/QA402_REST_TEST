﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace QA402_REST_TEST
{
    class LeftRightPair
    {
        public double Left;
        public double Right;
    }

    class LeftRightTimeSeries
    {
        /// <summary>
        /// dt is the time between samples. 1/dt is the sample rate
        /// </summary>
        public double dt;
        public double[] Left;
        public double[] Right;
    }

    class LeftRightFrequencySeries
    {
        /// <summary>
        /// df is the frequency spacing of FFT bins
        /// </summary>
        public double df;
        public double[] Left;
        public double[] Right;
    }

    class Qa402
    {
        static HttpClient Client = new HttpClient();
        static string RootUrl;

        static Qa402()
        {
            SetRootUrl("http://localhost:9402");
        }

        static void SetRootUrl(string rootUrl)
        {
            RootUrl = rootUrl;
            Client = new HttpClient
            {
                BaseAddress = new Uri(RootUrl)
            };
        }

        static public async Task<double> GetVersion()
        {
            string s = await Get("/Status/Version", "Value");
            return Convert.ToDouble(s);
        }

        static public async Task SetDefaults(string fileName = "")
        {
            if (fileName == "")
            {
                await Put("/Settings/Default");
            }
            else
            {
                await Put(string.Format("/Settings/LoadFromFile/{0}", HttpUtility.UrlEncode(fileName)));
            }
        }

        static public async Task SetBufferSize(uint bufferSizePowerOfTwo)
        {
            await Put(string.Format("/Settings/BufferSize/{0}", bufferSizePowerOfTwo));
        }

        static public async Task SetInputRange(int maxInputDbv, bool roundToNearest = false)
        {
            if (roundToNearest)
            {
                maxInputDbv = (int)Math.Round(maxInputDbv / 6f) * 6;

                if (maxInputDbv > 42)
                    maxInputDbv = 42;

                if (maxInputDbv < 0)
                    maxInputDbv = 0;
            }

            await Put(string.Format("/Settings/Input/Max/{0}", maxInputDbv));
        }

        static public async Task SetGen1(double freqHz, double ampDbv, bool enabled)
        {
            await Put(string.Format("/Settings/AudioGen/Gen1/{0}/{1}/{2}", enabled ? "On" : "Off", freqHz.ToString(), ampDbv.ToString()));
        }

        static public async Task DoAcquisitionAsync(double[] left, double[] right)
        {
            string l = Convert.ToBase64String(GetBytes(left));
            string r = Convert.ToBase64String(GetBytes(right));

            string s = $"{{ \"Left\":\"{l}\", \"Right\":\"{r}\" }}";

            await Post("/Acquisition", s);
        }

        static public async Task DoAcquisition()
        {
            await Post("/Acquisition");
        }

        static public async Task DoAcquisitionAsync()
        {
            _ = Task.Factory.StartNew(() =>
            {
                _ = Post("/AcquisitionAsync");
            });
        }

        static public async Task<bool> IsBusy()
        {
            string s = await Get("/AcquisitionBusy", "Value");
            return Convert.ToBoolean(s);
        }

        static public async Task<bool> AuditionStart(string fileName, int dacMaxOutput, double volume, bool repeat)
        {
            string s = $"/AuditionStart/{fileName}/{dacMaxOutput.ToString()}/{volume.ToString()}/" + (repeat ? "True" : "False");
            await Post(s);
            return true;
        }

        static public async Task<bool> AuditionStop()
        {
            await Post($"/AuditionStop");
            return true;
        }

        static public async Task<LeftRightPair> GetThdDb(double fundFreq, double maxFreq)
        {
            Dictionary<string, string> d = await Get(string.Format("/ThdDb/{0}/{1}", fundFreq, maxFreq));

            LeftRightPair lrp = new LeftRightPair() { Left = Convert.ToDouble(d["Left"]), Right = Convert.ToDouble(d["Right"]) };
            return lrp;
        }

        static public async Task<LeftRightPair> GetThdnDb(double fundFreq, double minFreq, double maxFreq)
        {
            Dictionary<string, string> d = await Get(string.Format("/ThdnDb/{0}/{1}/{2}", fundFreq, minFreq, maxFreq));

            LeftRightPair lrp = new LeftRightPair() { Left = Convert.ToDouble(d["Left"]), Right = Convert.ToDouble(d["Right"]) };
            return lrp;
        }

        static public async Task<LeftRightPair> GetRmsDbv(double startFreq, double endFreq, bool aWeighting = false)
        {
            Dictionary<string, string> d = await Get(string.Format("/RmsDbv/{0}{1}/{2}", aWeighting ? "AWeighting/" : "", startFreq, endFreq));

            LeftRightPair lrp = new LeftRightPair() { Left = Convert.ToDouble(d["Left"]), Right = Convert.ToDouble(d["Right"]) };
            return lrp;
        }

        static public async Task<LeftRightTimeSeries> GetInputTimeSeries()
        {
            Dictionary<string, string> d = await Get(string.Format("/Data/Time/Input"));

            LeftRightTimeSeries lrts = new LeftRightTimeSeries() { dt = Convert.ToDouble(d["Dx"]), Left = ConvertBase64ToDoubles(d["Left"]), Right = ConvertBase64ToDoubles(d["Right"]) };

            return lrts;
        }

        static public async Task<LeftRightFrequencySeries> GetInputFrequencySeries()
        {
            Dictionary<string, string> d = await Get(string.Format("/Data/Frequency/Input"));

            LeftRightFrequencySeries lrfs = new LeftRightFrequencySeries() { df = Convert.ToDouble(d["Dx"]), Left = ConvertBase64ToDoubles(d["Left"]), Right = ConvertBase64ToDoubles(d["Right"]) };

            return lrfs;
        }




        //*****************************
        //*** Helpers for the above ***
        //*****************************


        static double[] ConvertBase64ToDoubles(string base64DoubleArray)
        {
            byte[] byteArray = Convert.FromBase64String(base64DoubleArray);
            double[] doubleArray = new double[byteArray.Length / sizeof(double)];
            Buffer.BlockCopy(byteArray, 0, doubleArray, 0, byteArray.Length);
            return doubleArray;
        }
        static private async Task Put(string url, string token = "", int value = 0)
        {
            string json;

            if (token != "")
                json = string.Format("{{\"{0}\":{1}}}", token, value);
            else
                json = "{{}}";

            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await Client.PutAsync(url, content);

            // Throw an exception if not successful
            response.EnsureSuccessStatusCode();
            response.Dispose();
        }

        static private async Task Post(string url, string value = "")
        {
            StringContent content = new StringContent(value, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await Client.PostAsync(url, content);

            // Throw an exception if not successful
            response.EnsureSuccessStatusCode();
            response.Dispose();
        }

        /// <summary>
        /// This function allows us to get JSON fields. For example, if you do a GET
        /// and a JSON struct is returned:
        /// {
        ///    "Dogs" : "3"
        ///    "Cats" : "5"
        /// }
        /// 
        /// this function puts the return IDs into a dictionary for easy access. So:
        ///      Get("/my/url")["Dogs"] will return "3"
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        static private async Task<Dictionary<string, string>> Get(string url)
        {
            string content;

            Client.DefaultRequestHeaders.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await Client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            content = response.Content.ReadAsStringAsync().Result;

            // You need to use NUGET to install System.Text.Json from MSFT
            var result = JsonSerializer.Deserialize<Dictionary<string, string>>(content);

            return result;
        }

        static private async Task<string> Get(string url, string token)
        {
            Dictionary<string, string> dict = await Get(url);
            return dict[token].ToString();
        }

        static byte[] GetBytes(double[] vals)
        {
            var byteArray = new byte[vals.Length * sizeof(double)];
            Buffer.BlockCopy(vals, 0, byteArray, 0, byteArray.Length);
            return byteArray;
        }
    }
}