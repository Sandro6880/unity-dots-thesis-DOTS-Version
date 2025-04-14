using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.OOD_Scripts
{
    public class FPSTrackerOOD : MonoBehaviour
    {
        public CarSpawnerScript spawnAICars;

        public TMP_Text averageFPS;
        public TMP_Text spawnCarAmountText;
        public TMP_Text timeElapsedTill200FPS;
        public bool trackTill200FPS = false;
        public TMP_Text timeElapsedTill100FPS;
        public bool trackTill100FPS = false;
        public TMP_Text timeElapsedTill60FPS;
        public bool trackTill60FPS = false;

        private Dictionary<int, string> CachedNumberStrings = new();
        private int[] _frameRateSamples;
        private int _cacheNumbersAmount = 600;
        private int _averageFromAmount = 60;
        private int _averageCounter = 0;
        private int _currentAveraged;

        /*private float _timeBelow200 = -1f;
        private float _timeBelow100 = -1f;
        private float _timeBelow60 = -1f;
        private float _fpsDropThreshold = 1f;*/
        private float _sceneStartTime;

        //CSV
        //Cube, Car_low, Car_mid, Car_high
        public string currentCondition;
        private string filePath;
        private List<string> conditions = new List<string> { "Cube", "Car_low", "Car_mid", "Car_high", "Car_Prediction" };
        private Dictionary<string, List<float>> fpsData = new Dictionary<string, List<float>>();
        private bool isDone = false;
        private bool isInside = false;
        private int previousObjectCount = 0;
        void Awake()
        {
            // Cache strings and create array
            {
                for (int i = 0; i < _cacheNumbersAmount; i++)
                {
                    CachedNumberStrings[i] = i.ToString();
                }
                _frameRateSamples = new int[_averageFromAmount];
            }
        }
        private void Start()
        {
            filePath = "D:\\Bachelor\\ood_car_fps_data.csv";
            InitializeCSV();
            _sceneStartTime = Time.time;
        }
        void Update()
        {
            // Sample
            {
                var currentFrame = (int)Math.Round(1f / Time.smoothDeltaTime);
                _frameRateSamples[_averageCounter] = currentFrame;
            }

            // Average
            {
                var average = 0f;

                foreach (var frameRate in _frameRateSamples)
                {
                    average += frameRate;
                }

                _currentAveraged = (int)Math.Round(average / _averageFromAmount);
                _averageCounter = (_averageCounter + 1) % _averageFromAmount;
            }

            // Assign to UI
            {
                averageFPS.text = "Average FPS: " + _currentAveraged switch
                {
                    var x when x >= 0 && x < _cacheNumbersAmount => CachedNumberStrings[x],
                    var x when x >= _cacheNumbersAmount => $"> {_cacheNumbersAmount}",
                    var x when x < 0 => "< 0",
                    _ => "?"
                };
            }
            spawnCarAmountText.text = "GameObjects Spawned: " + spawnAICars.spawnCount;
            TrackFPSDrops();
        }
        private void TrackFPSDrops()
        {
            //float timeNow = Time.time;
            //float timeSinceLoad = Time.time - _sceneStartTime;
            //
            if (spawnAICars.spawnCount != 0 && spawnAICars.spawnCount % 100 == 0 && !isInside)
            {
                isInside = true;
                LogFPS(currentCondition, _currentAveraged, spawnAICars.spawnCount);
                isInside = false;
            }
            /*if (_currentAveraged <= 200)
            {
                if (_timeBelow200 < 0) _timeBelow200 = timeNow; // Start timer
                if (timeNow - _timeBelow200 >= _fpsDropThreshold && timeElapsedTill200FPS.text == "")
                {
                    timeElapsedTill200FPS.text = $"Below 200 FPS for {timeNow - _timeBelow200:F1}s in {timeSinceLoad:F1}s| {spawnCarAmountText.text}";
                }
            }
            else
            {
                _timeBelow200 = -1f; // Reset timer if FPS recovers
            }

            // 100 FPS Threshold
            if (_currentAveraged <= 100)
            {
                if (_timeBelow100 < 0) _timeBelow100 = timeNow;
                if (timeNow - _timeBelow100 >= _fpsDropThreshold && timeElapsedTill100FPS.text == "")
                {
                    timeElapsedTill100FPS.text = $"Below 100 FPS for {timeNow - _timeBelow100:F1}s in {timeSinceLoad:F1}s| {spawnCarAmountText.text}";
                }
            }
            else
            {
                _timeBelow100 = -1f;
            }

            // 60 FPS Threshold
            if (_currentAveraged <= 60)
            {
                if (_timeBelow60 < 0) _timeBelow60 = timeNow;
                if (timeNow - _timeBelow60 >= _fpsDropThreshold && timeElapsedTill60FPS.text == "")
                {
                    timeElapsedTill60FPS.text = $"Below 60 FPS for {timeNow - _timeBelow60:F1}s in {timeSinceLoad:F1}s| {spawnCarAmountText.text}";
                }
            }
            else
            {
                _timeBelow60 = -1f;
            }*/

        }

        void InitializeCSV()
        {
            if (!File.Exists(filePath))
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    // Write the header
                    writer.Write("Object");
                    for (int i = 100; i <= 10000; i += 100) // Adjust if needed
                    {
                        writer.Write($";{i}");
                    }
                    writer.WriteLine();
                }
            }

            // Initialize FPS tracking dictionary
            foreach (string condition in conditions)
            {
                fpsData[condition] = new List<float>();
            }
        }

        public void LogFPS(string condition, float fps, int objectCount)
        {
            if (!fpsData.ContainsKey(condition)) return;

            if (!isDone && previousObjectCount != objectCount)
            {
                previousObjectCount = objectCount;
                fpsData[condition].Add(fps);

                if (objectCount >= 10000) // Adjust based on the max tracked count
                {
                    isDone = true;
                    WriteToCSV();
                }
            }
        }

        void WriteToCSV()
        {
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.Write(currentCondition);
                foreach (var fps in fpsData[currentCondition])
                {
                    writer.Write($";{fps}");
                }
                writer.WriteLine();
              
            }

            // Clear data after writing
            foreach (var key in fpsData.Keys)
            {
                fpsData[key].Clear();
            }
        }
        public IEnumerator TrackTimeElapsed()
        {
            yield return new WaitForSeconds(5);
            while (_currentAveraged > 50)
            {
                if (_currentAveraged <= 200 && !trackTill200FPS)
                {
                    timeElapsedTill200FPS.text = "Average Time needed to reach below 200 FPS first time: " + Time.time
                        + " seconds with " + spawnCarAmountText.text + " Objects spawned";
                    trackTill200FPS = true;
                }
                if (_currentAveraged <= 100 && !trackTill100FPS)
                {
                    timeElapsedTill100FPS.text = "Average Time needed to reach below 100 FPS first time: " + Time.time
                        + " seconds with " + spawnCarAmountText.text + " Objects spawned";
                    trackTill100FPS = true;
                }
                if (_currentAveraged <= 60 && !trackTill60FPS)
                {
                    timeElapsedTill60FPS.text = "Average Time needed to reach below 60 FPS first time: " + Time.time
                        + " seconds with " + spawnCarAmountText.text + " Objects spawned";
                    trackTill60FPS = true;
                }
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
