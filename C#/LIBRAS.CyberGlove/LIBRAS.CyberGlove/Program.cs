﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LIBRAS.CyberGlove
{
    public class StringTable
    {
        public string[] ColumnNames { get; set; }
        public string[,] Values { get; set; }
    }

    class Program
    {
        static int QUANTITY_SENSORS = 22;

        static void Main(string[] args)
        {
            InvokeRequestResponseService().Wait();
        }

        static string[,] ReadFile()
        {
            string[] lines;

            try
            {
                lines = File.ReadAllLines(@"C:\LIBRAS.CyberGlove\gesture.txt");
                string[,] gestures = new string[lines.Length, QUANTITY_SENSORS];

                for (int i = 0; i < lines.Length; i++)
                {
                    string[] sensors = lines[i].Split(' ');
                    for (int j = 0; j < QUANTITY_SENSORS; j++)
                    {
                        gestures[i, j] = sensors[j];
                    }
                }

                return gestures;
            }
            catch (Exception)
            {
                Console.WriteLine("Gesture File Not Found");
                Console.ReadKey();
                Environment.Exit(0);
            }

            return null;
        }

        static async Task InvokeRequestResponseService()
        {
            Console.WriteLine("Detecting Gestures...\n");

            string[,] gestures = ReadFile();

            for (int i = 0; i < (gestures.Length / QUANTITY_SENSORS); i++)
            {
                using (var client = new HttpClient())
                {
                    var scoreRequest = new
                    {
                        Inputs = new Dictionary<string, StringTable>() { 
                            { 
                                "input1", 
                                new StringTable() 
                                {
                                    ColumnNames = new string[] {"S01", "S02", "S03", "S04", "S05", "S06", "S07", "S08", "S09", "S10", "S11", "S12", "S13", "S14", "S15", "S16", "S17", "S18", "S19", "S20", "S21", "S22", "OUTPUT"},
                                    Values = new string[,] { { gestures[i, 0],
                                                               gestures[i, 1],
                                                               gestures[i, 2],
                                                               gestures[i, 3],
                                                               gestures[i, 4],
                                                               gestures[i, 5],
                                                               gestures[i, 6],
                                                               gestures[i, 7],
                                                               gestures[i, 8],
                                                               gestures[i, 9],
                                                               gestures[i, 10],
                                                               gestures[i, 11],
                                                               gestures[i, 12],
                                                               gestures[i, 13],
                                                               gestures[i, 14],
                                                               gestures[i, 15],
                                                               gestures[i, 16],
                                                               gestures[i, 17],
                                                               gestures[i, 18],
                                                               gestures[i, 19],
                                                               gestures[i, 20],
                                                               gestures[i, 21],
                                                               ""}, }
                                }
                            },
                        },
                        GlobalParameters = new Dictionary<string, string>() { }
                    };

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "V9lbEZwQoj37OwWY5uK1/q6cU67LNlhSMXM8znmTXrtSvGycaq/gvBl1W55GBq9l8Gladiwungu34uJ1RSeMbg==");
                    client.BaseAddress = new Uri("https://ussouthcentral.services.azureml.net/workspaces/080e77d298664162a44a8e3f1377dfc6/services/44ec08620cb24e7cac1ea7782faa0b28/execute?api-version=2.0&details=true");
                    
                    HttpResponseMessage response = await client.PostAsJsonAsync("", scoreRequest);
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        dynamic obj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

                        foreach (var gesture in obj.Results.output1.value.Values)
                        {
                            Console.Write("Gesture {0}: ", i+1);
                            Console.WriteLine(gesture.Last);
                        }
                    }
                    else
                    {
                        Console.WriteLine(string.Format("The request failed with status code: {0}", response.StatusCode));
                        Console.WriteLine(response.Headers.ToString());
                        string responseContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(responseContent);
                    }
                }
            }
            Console.WriteLine("\nGesture Detection Complete");
            Console.ReadKey();
        }
    }
}