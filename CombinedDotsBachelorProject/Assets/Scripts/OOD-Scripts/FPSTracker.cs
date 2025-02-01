using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.OOD_Scripts
{
    public class FPSTracker : MonoBehaviour
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
        private int _cacheNumbersAmount = 300;
        private int _averageFromAmount = 30;
        private int _averageCounter = 0;
        private int _currentAveraged;

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
            StartCoroutine(TrackTimeElapsed());
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
                if (spawnAICars != null)
                {
                    spawnCarAmountText.text = "Objects Spawned: " + spawnAICars.spawnCount.ToString();
                }
                
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
