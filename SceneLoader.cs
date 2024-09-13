using System;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public static class SceneLoader {

    public enum Scene {
        MainMenuScene,
        LobbyScene,
        // LoadingScene,
        GameScene
    }
    
    public enum GameMode {
        OnlineMultiplayer,
        LocalMultiplayer
    }

    public static GameMode CurrentGameMode => _currentGameMode;
    private static GameMode _currentGameMode = GameMode.LocalMultiplayer;

    // for local multiplayer
    public static int NumPlayers => _numPlayers;
    private static int _numPlayers = 1;
    
    public static void Load(Scene scene) {
        if (scene == Scene.LobbyScene)
            _currentGameMode = GameMode.OnlineMultiplayer;
        
        SceneManager.LoadScene(scene.ToString());
    }

    public static void LoadNetwork(Scene targetScene) {
        NetworkManager.Singleton.SceneManager.LoadScene(targetScene.ToString(), LoadSceneMode.Single);
    }
    
    private static void LoadGameScene(GameMode gameMode) {
        _currentGameMode = gameMode;
        switch (gameMode) {
            case GameMode.LocalMultiplayer:
                LoadNetwork(Scene.GameScene);
                break;
            case GameMode.OnlineMultiplayer:
                LoadNetwork(Scene.GameScene);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(gameMode), gameMode, null);
        }
    }
    
    public static void LoadOnlineMultiplayer() {
        LoadGameScene(GameMode.OnlineMultiplayer);
    }
    
    public static void LoadLocalMultiplayer(int numPlayers) {
        LoadGameScene(GameMode.LocalMultiplayer);
        _numPlayers = numPlayers;
    }
}
