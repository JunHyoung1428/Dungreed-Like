using UnityEngine;

public static class Manager
{
    public static GameManager Game { get { return GameManager.Instance; } }
    public static DataManager Data { get { return DataManager.Instance; } }
    public static PoolManager Pool { get { return PoolManager.Instance; } }
    public static ResourceManager Resource { get { return ResourceManager.Instance; } }
    public static SceneManager Scene { get { return SceneManager.Instance; } }
    public static SoundManager Sound { get { return SoundManager.Instance; } }
    public static UIManager UI { get { return UIManager.Instance; } }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        //Editor > PlayMode 옵션에서 Reload Domain이 꺼져있어서 Static 영역 초기화 필요
        GameManager.ReleaseInstance();
        DataManager.ReleaseInstance();
        PoolManager.ReleaseInstance();
        ResourceManager.ReleaseInstance();
        SceneManager.ReleaseInstance();
        SoundManager.ReleaseInstance();
        UIManager.ReleaseInstance();

        GameManager.CreateInstance();
        DataManager.CreateInstance();
        PoolManager.CreateInstance();
        ResourceManager.CreateInstance();
        SceneManager.CreateInstance();
        SoundManager.CreateInstance();
        UIManager.CreateInstance();
    }
}
