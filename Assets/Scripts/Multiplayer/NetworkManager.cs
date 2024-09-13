using RiptideNetworking;
using RiptideNetworking.Utils;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum ServerToClientId : ushort
{
    connect = 1,
    loginResult,
    signUpResult,
    loginError,
    chooseResult,
    sendPatinetData,
    sendQuestionnaireDesign,
    sendClinicInfo,
    removeReserve,
}

public enum ClientToServerId : ushort
{
    connect = 1,
    login,
    signUp,
    chooseDocOrPatient,
    addFavorite,
    cancelFavorite,
    cancelReserve,
    sendQuestionnaire,//Reserve
    sendQuestionnaireDesign,
    getQuestionnaireDesign,
    storeClinicInfo,
    storePersonalInfo,
    getGetClinicInfo,
    skipReserve,
}

public class NetworkManager : MonoBehaviour
{
    private static NetworkManager _singleton;
    public static NetworkManager Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null)
                _singleton = value;
            else if (_singleton != value)
            {
                Debug.Log($"{nameof(NetworkManager)} instance already exists, destroying duplicate!");
                Destroy(value);
            }
        }
    }

    public Client Client { get; private set; }

    public bool isConnect;
    [SerializeField] private string ip;
    [SerializeField] private ushort port;

    private void Awake()
    {
        Singleton = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);

        Client = new Client();
        Client.Connected += DidConnect;
        Client.ConnectionFailed += FailedToConnect;
        Client.ClientDisconnected += PlayerLeft;
        Client.Disconnected += DidDisconnect;
    }

    private void FixedUpdate()
    {
        Client.Tick();
    }

    private void OnApplicationQuit()
    {
        Client.Disconnect();
    }

    public void Connect()
    {
        Client.Connect($"{ip}:{port}");
    }

    private void DidConnect(object sender, EventArgs e)
    {
        UIManager.Singleton.Connect();
    }

    private void FailedToConnect(object sender, EventArgs e)
    {
        UIManager.Singleton.BackToMain();
    }

    private void PlayerLeft(object sender, ClientDisconnectedEventArgs e)
    {
        //Destroy(Player.list[e.Id].gameObject);
    }

    private void DidDisconnect(object sender, EventArgs e)
    {
        if(SceneManager.GetActiveScene().name == "Menu") UIManager.Singleton.BackToMain();
        if(SceneManager.GetActiveScene().name == "MainSick") MainUI.Singleton.Btn_LogOut();
        if(SceneManager.GetActiveScene().name == "MainDoc") MainUI_Doc.Singleton.Btn_LogOut();
    }
}
