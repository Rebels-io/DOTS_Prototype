using Unity.Collections;
using Unity.Entities;
using UnityEngine.SceneManagement;
/// <summary>
/// Use this class to restart your current scene. Make sure to close any subscenes in the editor or it will not work correctly.
/// </summary>
public class RestartLevelSystem : SystemBase
{
    public static NativeArray<bool> restarting = new NativeArray<bool>(1, Allocator.Persistent);
    public static void Restart()
    {
        restarting[0] = true;
    }

    protected override void OnUpdate()
    {
        if (restarting[0])
        {
            restarting[0] = false;
            Scene active = SceneManager.GetActiveScene();
            //SceneManager.UnloadSceneAsync(active);
            SceneManager.LoadScene(active.buildIndex, LoadSceneMode.Single);
            World.DisposeAllWorlds();
            //must init default world after reload or ECS objects will not spawn
            DefaultWorldInitialization.Initialize("Default World", false);

        }
    }
}