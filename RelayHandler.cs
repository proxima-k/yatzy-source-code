using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class RelayHandler : MonoBehaviour {

    public string JoinCode => _joinCode;
    private string _joinCode = "";

    public static RelayHandler Instance { get; private set; }

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    private async Task<bool> InitializeService() {
        if (UnityServices.State == ServicesInitializationState.Initialized) {
            Debug.Log("Unity Services already initialized.");
            return true;
        }
        
        
        try {
            Debug.Log("Initializing Unity Services...");
            await UnityServices.InitializeAsync();
        }
        catch (Exception e) {
            Debug.LogWarning($"Error initializing Unity Services: {e.Message}");
            return false;
        }
        
        return true;
    }

    private async Task<bool> AuthenticateService() {
        
        bool isServiceInitialized = await InitializeService();
        
        if (!isServiceInitialized) {
            Debug.LogWarning("Failed to initialize Unity Services.");
            return false;
        }
        
        if (AuthenticationService.Instance.IsSignedIn) {
            Debug.Log($"Already signed in as {AuthenticationService.Instance.PlayerId}");
            return true;
        }
        
        bool sessionTokenExists = AuthenticationService.Instance.SessionTokenExists;

        if (sessionTokenExists)
            AuthenticationService.Instance.ClearSessionToken();
        
        Debug.Log($"Session token exists: {sessionTokenExists}");

        
        // anonymous sign in
        AuthenticationService.Instance.SignedIn += AuthenticationService_SignedIn;
        try {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            return true;
        }
        catch (Exception e) {
            Debug.LogWarning($"Error signing in anonymously: {e.Message}");
            return false;
        }
    }
    
    
    private void AuthenticationService_SignedIn() {
        Debug.Log($"Signed in as {AuthenticationService.Instance.PlayerId}");
    }
    

    public async Task<bool> CreateRelay() {
        
        bool isAuthenticated = await AuthenticateService();
        
        if (!isAuthenticated) {
            Debug.LogWarning("Failed to authenticate Unity Services.");
            return false;
        }
        
        try {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(ConnectionHandler.MAX_PLAYER_COUNT);

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            
            Debug.Log($"Join code: {joinCode}");
            
            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
            UnityTransport unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            unityTransport.SetRelayServerData(relayServerData);
            unityTransport.UseEncryption = true;
            
            _joinCode = joinCode;
            if (ConnectionHandler.Instance.StartHost()) {
                return true;
            }
            
            Debug.LogWarning("Failed to start host.");
            return false;
            
        } catch (RelayServiceException e) {
            Debug.LogWarning($"Error creating allocation: {e.Message}");
            return false;
        }
    }

    public async Task<bool> JoinRelay(string joinCode) {
        
        bool isAuthenticated = await AuthenticateService();
        
        if (!isAuthenticated) {
            Debug.LogWarning("Failed to authenticate Unity Services.");
            return false;
        }
        
        try {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            
            // if relay is full
            Debug.Log($"Join Allocation is null? {joinAllocation == null}");
            Debug.Log($"AllocationID: {joinAllocation.AllocationId}");
            Debug.Log($"Endpoint Count: {joinAllocation.ServerEndpoints.Count}");
            Debug.Log($"Host: {joinAllocation.ServerEndpoints[0].Host}");
            // RelayServerEndpoint
            
            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
            UnityTransport unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            unityTransport.SetRelayServerData(relayServerData);
            unityTransport.UseEncryption = true;

            _joinCode = joinCode;
            if (ConnectionHandler.Instance.StartClient()) {
                return true;
            }
            
            Debug.LogWarning("Failed to start client.");
            return false;
            
        } catch (RelayServiceException e) {
            Debug.LogWarning($"Error joining relay: {e.Message}");
            
            return false;
        }
    }
    
    private void OnDestroy() {
        try {
            if (AuthenticationService.Instance.IsSignedIn)
                AuthenticationService.Instance.SignedIn -= AuthenticationService_SignedIn;
        } catch (Exception e) {
            Debug.LogWarning($"Error removing SignedIn event handler: {e.Message}");
        }
        
        // if (AuthenticationService.Instance.IsSignedIn)
        //     AuthenticationService.Instance.SignedIn -= AuthenticationService_SignedIn;
    }
}
