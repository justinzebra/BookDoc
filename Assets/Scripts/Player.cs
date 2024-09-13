using RiptideNetworking;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Dictionary<ushort, Player> list = new Dictionary<ushort, Player>();

    public ushort Id { get; private set; }

    public string username;
    public string userID;
    public string email;
    public string phoneNumber;
    public int myID;
    public string address;
    public string department;
    public string openTime;
    public string openTime2;
    public string docPhoneNumber;
    public string[] questions;

    private void Start() {
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        list.Remove(Id);
    }

    public static void Spawn(ushort id)
    {
        Player player;

        player = Instantiate(GameLogic.Singleton.LocalPlayerPrefab).GetComponent<Player>();
        if (id == NetworkManager.Singleton.Client.Id)
        {
            player.Id = id;
        }
        else
        {
            Debug.Log("Id Issue");
            player.Id = NetworkManager.Singleton.Client.Id;
        }

        NetworkManager.Singleton.isConnect = true;
    
        list.Add(id, player);
    }

    [MessageHandler((ushort)ServerToClientId.connect)]
    private static void SpawnPlayer(Message message)
    {
        Spawn(message.GetUShort());
    }

    public void GetLoginResult(string _username, string _userID, string _email, string _phoneNumber, int _myID, string _address, string _department, string _openTime, string  _openTime2, string _docPhoneNumber, string _question, string _question2, string _question3, string _question4, string _question5, string _question6, string _question7, string _question8)
    {
        username = _username;
        userID = _userID;
        email = _email;
        phoneNumber = _phoneNumber;
        myID = _myID;
        address = _address;
        department = _department;
        openTime = _openTime;
        openTime2 = _openTime2;
        docPhoneNumber = _docPhoneNumber;
        questions[0] = _question;
        questions[1] = _question2;
        questions[2] = _question3;
        questions[3] = _question4;
        questions[4] = _question5;
        questions[5] = _question6;
        questions[6] = _question7;
        questions[7] = _question8;
        UIManager.Singleton.LoginResult(_username, _userID, _email, _phoneNumber, _myID);
    }

    [MessageHandler((ushort)ServerToClientId.loginResult)]
    private static void LoginResult(Message message)
    {
        if (list.TryGetValue(message.GetUShort(), out Player player))
        {
            player.GetLoginResult(message.GetString(), message.GetString(), message.GetString(), message.GetString(), message.GetInt()
                , message.GetString(), message.GetString(), message.GetString(), message.GetString(), message.GetString(), message.GetString()
                , message.GetString(), message.GetString(), message.GetString(), message.GetString(), message.GetString(), message.GetString()
                , message.GetString());
        }
    }

    public static void GetSignUpResult(string result)
    {
        UIManager.Singleton.SignUpResult(result);
    }

    [MessageHandler((ushort)ServerToClientId.signUpResult)]
    private static void SignUpResult(Message message)
    {
        GetSignUpResult(message.GetString());
    }

    public static void LoginError(string index)
    {
        UIManager.Singleton.LoginError(index);
    }

    [MessageHandler((ushort)ServerToClientId.loginError)]
    private static void LoginError(Message message)
    {
        LoginError(message.GetString());
    }

    public static void ChooseResult(int myID)
    {
        UIManager.Singleton.ChooseResult(myID);
    }

    [MessageHandler((ushort)ServerToClientId.chooseResult)]
    private static void ChooseResult(Message message)
    {
        ChooseResult(message.GetInt());
    }

    public static void SendPatinetData(ushort Id, int time, int no, string _username, bool isDoc, int docIndex)
    {
        if(isDoc){
            MainUI_Doc.Singleton.AddPatient(time, no, _username, true, docIndex);
        }else{
            bool isMe = false;
            if (list.TryGetValue(Id, out Player player))
            {
                if(player.username == _username) isMe = true;
            }
            MainUI.Singleton.AddPatient(time, no, _username, isMe, false, docIndex);
        }
    }

    [MessageHandler((ushort)ServerToClientId.sendPatinetData)]
    private static void SendPatinetData(Message message)
    {
        SendPatinetData(message.GetUShort(), message.GetInt(), message.GetInt(), message.GetString(), message.GetBool(), message.GetInt());
    }

    public static void SendQuestionnaireDesign(int docIndex, string question, string question2, string question3, string question4, string question5, string question6, string question7, string question8)
    {
        MainUI.Singleton.GetQuestionnaireDesign(docIndex, question, question2, question3, question4, question5, question6, question7, question8);
    }

    [MessageHandler((ushort)ServerToClientId.sendQuestionnaireDesign)]
    private static void SendQuestionnaireDesign(Message message)
    {
        SendQuestionnaireDesign(message.GetInt(), message.GetString(), message.GetString(), message.GetString(), message.GetString(), message.GetString(), message.GetString(), message.GetString(), message.GetString());
    }

    public static void SetClinicInfo(int docIndex, bool isOnline, string name, string address, string department, string openTime, string openTime2, string docPhoneNumber)
    {
        MainUI.Singleton.SetClinicInfo(docIndex, isOnline, name, address, department, openTime, openTime2, docPhoneNumber);
    }

    [MessageHandler((ushort)ServerToClientId.sendClinicInfo)]
    private static void SendClinicInfo(Message message)
    {
        SetClinicInfo(message.GetInt(), message.GetBool(), message.GetString(), message.GetString(), message.GetString(), message.GetString(), message.GetString(), message.GetString());
    }

    public static void RemoveReserve(int no, bool isDoc)
    {
        if(isDoc){
            MainUI_Doc.Singleton.RemoveReserve(no);
        }else{
            MainUI.Singleton.RemoveReserve(no);
        }
    }

    [MessageHandler((ushort)ServerToClientId.removeReserve)]
    private static void removeReserve(Message message)
    {
        RemoveReserve(message.GetInt(), message.GetBool());
    }
}
