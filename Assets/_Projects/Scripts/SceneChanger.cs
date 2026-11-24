using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;

/// <summary>
/// シーン遷移関連の処理をまとめた静的クラス
/// </summary>
public static class SceneChanger
{
    public enum Scene
    {
        None = -1,
        Boot,
        Plane,
        Image,
        Game
    }

    public static void LoadScene(Scene targetScene)
    {
        switch (targetScene)
        {
            case Scene.Boot:
                LoaderUtility.Deinitialize();
                break;

            case Scene.Plane:
            case Scene.Image:
            case Scene.Game:
                LoaderUtility.Deinitialize();
                LoaderUtility.Initialize();
                break;

            default:
                return;
        }
        SceneManager.LoadScene((int)targetScene);
    }
}
