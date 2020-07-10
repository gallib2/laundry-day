using UnityEngine.SceneManagement;

public static class ScenesManager 
{
    public static void LoadScene(Scene scene)
    {
        SceneManager.LoadScene((int)scene);
    }
}

public enum Scene
{
    MAIN_MENU = 0, GAME = 1,
}