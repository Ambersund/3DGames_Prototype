using UnityEngine;
using UnityEngine.SceneManagement;

namespace FlightKit
{
	/// <summary>
	/// Performs the actions when a level is completed.
	/// </summary>
	public class LevelCompleteController : MonoBehaviour
	{
		[TooltipAttribute("Whether we should restart current scene or load the next one on level completion.")]
		/// <summary>
		/// Whether we should restart current scene or load the next one on level completion.
		/// </summary>
		public bool restartCurrentScene = false;
		
		/// <summary>
		/// Level is successfully completed, start the next level or go to menu from here.
		/// </summary>
		public virtual void HandleLevelComplete()
		{
			if (restartCurrentScene) {  	// Restart game.
				SceneManager.LoadScene(SceneManager.GetActiveScene().name);
			} else {  						// Load next scene.
				// Unity's index of the scene. Can be viewed and edited in Unity->File->Build Settings.
				int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
				if (nextSceneIndex < SceneManager.sceneCountInBuildSettings) {
					SceneManager.LoadScene(nextSceneIndex);	
				} else {  // There is no next scene, the current one is last.
					// Try to pause.
					var pause = GameObject.FindObjectOfType<PauseController>();
					if (pause != null) {
						pause.Pause();
					}
					
					// Report error.
					Debug.LogError("FLIGHT KIT: Trying to load next scene from the last scene.");
				}
			}
		}
	}
}