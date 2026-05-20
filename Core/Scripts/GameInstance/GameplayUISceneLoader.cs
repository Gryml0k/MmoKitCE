using LiteNetLibManager;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace MultiplayerARPG
{
    [DisallowMultipleComponent]
    public class GameplayUISceneLoader : MonoBehaviour
    {
        [SerializeField]
        private SceneField uiScene;
        [SerializeField]
        private SceneField[] gameplayScenes = new SceneField[0];

        private bool _isUnloadingUiScene;

        protected virtual void OnEnable()
        {
            if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null)
                return;

            SceneManager.sceneLoaded += OnSceneLoaded;
            HandleLoadedScene(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        }

        protected virtual void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            HandleLoadedScene(scene, mode);
        }

        private void HandleLoadedScene(Scene scene, LoadSceneMode mode)
        {
            if (mode != LoadSceneMode.Single || !uiScene.IsDataValid())
                return;

            if (IsGameplayScene(scene.name))
                LoadUiSceneIfNeeded();
            else
                UnloadUiSceneIfNeeded();
        }

        private bool IsGameplayScene(string sceneName)
        {
            if (gameplayScenes == null)
                return false;

            for (int i = 0; i < gameplayScenes.Length; ++i)
            {
                if (gameplayScenes[i].IsSameSceneName(sceneName))
                    return true;
            }
            return false;
        }

        private void LoadUiSceneIfNeeded()
        {
            if (SceneManager.GetSceneByName(uiScene.SceneName).isLoaded)
                return;

            _isUnloadingUiScene = false;
            SceneManager.LoadScene(uiScene.SceneName, LoadSceneMode.Additive);
        }

        private void UnloadUiSceneIfNeeded()
        {
            Scene loadedUiScene = SceneManager.GetSceneByName(uiScene.SceneName);
            if (!loadedUiScene.isLoaded || _isUnloadingUiScene)
                return;

            _isUnloadingUiScene = true;
            AsyncOperation operation = SceneManager.UnloadSceneAsync(loadedUiScene);
            if (operation != null)
                operation.completed += _ => _isUnloadingUiScene = false;
            else
                _isUnloadingUiScene = false;
        }
    }
}
