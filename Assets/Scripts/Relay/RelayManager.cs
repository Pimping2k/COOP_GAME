using System.Threading.Tasks;
using QFSW.QC.Utilities;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using UnityEngine;

public class RelayManager : MonoBehaviour
{
    public static RelayManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

        }
        else
        {
            Destroy(gameObject);
        }
    }

    private async Task InitializeUnityServiceAndSignIn()
    {
        try
        {
            await UnityServices.InitializeAsync();
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log("Unity Service Initialization completed");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error: " + e);
        }
    }

    public async Task<string> CreateRelay(int maxPlayers)
    {
        if (UnityServices.State == ServicesInitializationState.Uninitialized)
        {
            await InitializeUnityServiceAndSignIn();
        }

        try
        {
            var allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers);
            var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            var relayData = new RelayServerData(allocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayData);

            NetworkManager.Singleton.StartHost();
            Debug.Log($"Relay is created. join code - {joinCode}".ColorText(Color.green));
            return joinCode;
        }
        catch (RelayServiceException e)
        {
            Debug.LogError("Error: " + e);
            return null;
        }
    }

    public async Task JoinRelay(string joinCode)
    {
        if (UnityServices.State == ServicesInitializationState.Uninitialized)
        {
            await InitializeUnityServiceAndSignIn();
        }

        try
        {
            var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            var relayData = new RelayServerData(joinAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayData);

            NetworkManager.Singleton.StartClient();
            Debug.Log("Connection with relay is finished".ColorText(Color.green));
        }
        catch (RelayServiceException e)
        {
            Debug.LogError("Error connect with relay : " + e);
        }
    }
}