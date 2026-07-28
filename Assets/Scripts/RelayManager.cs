using System;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class RelayManager : MonoBehaviour
{
    [SerializeField] private UnityTransport transport;
    [SerializeField] private TMP_InputField joinCodeInput;
    [SerializeField] private TMP_Text hostCodeText;
    [SerializeField] private GameObject mainMenuPanel;

    private async void Awake()
    {
        await InitServices();
    }

    private async Task InitServices()
    {
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            await UnityServices.InitializeAsync();
        }

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        Debug.Log("Unity Services'e giris yapildi. Player ID: " + AuthenticationService.Instance.PlayerId);
    }

    public async void CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            Debug.Log("ODA KODU: " + joinCode);
            if (hostCodeText != null)
            {
                hostCodeText.text = "Oda Kodu: " + joinCode;
            }

            RelayServerData relayServerData = allocation.ToRelayServerData("dtls");
            transport.SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartHost();
            mainMenuPanel.SetActive(false);
        }
        catch (Exception e)
        {
            Debug.LogError("CreateRelay hatasi: " + e);
        }
    }

    public async void JoinRelay()
    {
        try
        {
            string joinCode = joinCodeInput.text;
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            RelayServerData relayServerData = joinAllocation.ToRelayServerData("dtls");
            transport.SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();
            mainMenuPanel.SetActive(false);
        }
        catch (Exception e)
        {
            Debug.LogError("JoinRelay hatasi: " + e);
        }
    }
}