using System;
using System.Collections;
using System.Collections.Generic;
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

        private float _timeBelow200 = -1f;
        private float _timeBelow100 = -1f;
        private float _timeBelow60 = -1f;
        private float _fpsDropThreshold = 2f;
        private float _sceneStartTime;
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
            //StartCoroutine(TrackTimeElapsed());
            timeElapsedTill200FPS.text = "";
            timeElapsedTill100FPS.text = "";
            timeElapsedTill60FPS.text = "";
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
            float timeNow = Time.time;
            float timeSinceLoad = Time.time - _sceneStartTime;
            // 200 FPS Threshold
            if (_currentAveraged <= 200)
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
