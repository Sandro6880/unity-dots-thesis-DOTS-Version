using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.OOD_Scripts
{
    public class MainMenu : MonoBehaviour
    {
        private EntityManager entityManager;
        private void Start()
        {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }
        public void LoadScene(string sceneName)
        {
            entityManager.DestroyEntity(entityManager.UniversalQuery);
            entityManager.CompleteAllTrackedJobs();
            entityManager.Debug.CheckInternalConsistency();
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
            SceneManager.LoadScene(sceneName);
        }
        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
