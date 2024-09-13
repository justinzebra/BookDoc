using RiptideNetworking;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private static UIManager _singleton;
    public static UIManager Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null)
                _singleton = value;
            else if (_singleton != value)
            {
                Debug.Log($"{nameof(UIManager)} instance already exists, destroying duplicate!");
                Destroy(value);
            }
        }
    }

    private void Awake()
    {
        Singleton = this;
        //Screen.SetResolution(499, 1080, FullScreenMode.Windowed);
    }

    public string username;

    [Header("For Test")]
    public bool loginSuccess;
    public bool signUpSuccess;
    public bool useDatabase;
    public bool goToMainSick;
    
    [Header("UI")]
    [SerializeField] private GameObject loginUI;
    [SerializeField] private GameObject signUpUI;
    [SerializeField] private GameObject chooseUI;
    [SerializeField] private GameObject docUI;
    [SerializeField] private GameObject sickUI;
    [SerializeField] private GameObject loginFailUI;

    [Header("Login")]
    [SerializeField] private InputField emailField_user;
    [SerializeField] private InputField passwordField_user;
    [SerializeField] private GameObject failText2;
    [SerializeField] private Button loginBtn;

    [Header("SignUp")]
    [SerializeField] private InputField usernameField;
    [SerializeField] private InputField IDField;
    [SerializeField] private InputField emailField;
    [SerializeField] private InputField phoneNumberField;
    [SerializeField] private InputField passwordField;
    [SerializeField] private InputField passwordConfirmField;
    [SerializeField] private Text failText;
    [SerializeField] private Button signUpBtn;

    private void Start()
    {
        Singleton = this;
        if(useDatabase && !NetworkManager.Singleton.isConnect){
            Invoke(nameof(Connect), 1f);
            Invoke(nameof(SendConnect), 2f);
        }
    }

#region Connect
    public void Connect()
    {
        if(!NetworkManager.Singleton.Client.IsConnected){
            NetworkManager.Singleton.Connect();
        }
    }

    public void SendConnect()
    {
        Message message = Message.Create(MessageSendMode.reliable, (ushort)ClientToServerId.connect);
        NetworkManager.Singleton.Client.Send(message);
    }
#endregion

#region Login and SignUp
    public void Btn_Login()
    {
        if(useDatabase){
            loginBtn.enabled = false;
            SendLoginData();
        }else{
            if(loginSuccess){
                LoginResult("", "", "", "", 0);
            }else{
                LoginError("10");
            }
        }
    }

    public void Btn_SignUp()
    {
        if(useDatabase){
            signUpBtn.enabled = false;
            SendSignUpData();
        }else{
            if(signUpSuccess){
                SignUpResult("0");
            }else{
                SignUpResult("1");
            }
        }
    }

    public void SendLoginData()
    {
        Message message = Message.Create(MessageSendMode.reliable, (ushort)ClientToServerId.login);
        message.AddString(emailField_user.text);
        message.AddString(passwordField_user.text);
        NetworkManager.Singleton.Client.Send(message);
    }

    public void SendSignUpData()
    {
        if(!CheckSignUpData()) return;
        
        Message message = Message.Create(MessageSendMode.reliable, (ushort)ClientToServerId.signUp);
        message.AddString(usernameField.text);
        message.AddString(IDField.text);
        message.AddString(emailField.text);
        message.AddString(phoneNumberField.text);
        message.AddString(passwordField.text);
        NetworkManager.Singleton.Client.Send(message);
    }

    bool CheckSignUpData()
    {
        if(passwordField.text != passwordConfirmField.text)
        {
            failText.text = "密碼與確認密碼不同";
            return false;
        }else if(usernameField.text == ""){
            failText.text = "請輸入用戶名";
            return false;
        }else if(IDField.text == ""){
            failText.text = "請輸入身分證字號";
            return false;
        }else if(emailField.text == ""){
            failText.text = "請輸入電子信箱";
            return false;
        }else if(phoneNumberField.text == ""){
            failText.text = "請輸入手機號碼";
            return false;
        }else if(passwordField.text == ""){
            failText.text = "請輸入密碼";
            return false;
        }
            
        return true;
    }

    public void LoginError(string index)
    {
        Debug.Log(index);
        loginBtn.enabled = true;
        failText2.SetActive(true);
    }

    public void LoginResult(string username, string userID, string email, string phoneNumber, int myID)
    {
        if(myID == 0){
            loginUI.SetActive(false);
            chooseUI.SetActive(true);
        }else if(myID == 1){
            SceneManager.LoadScene("MainSick");
        }else{
            SceneManager.LoadScene("MainDoc");
        }
    }

    public void SignUpResult(string result)
    {
        Debug.Log(result);
        signUpBtn.enabled = true;
        if(result == "0"){
            failText.text = "註冊成功";
            usernameField.text = "";
            IDField.text = "";
            emailField.text = "";
            phoneNumberField.text = "";
            passwordField.text = "";
            passwordConfirmField.text = "";
        }else if(result == "3"){
            failText.text = "此身分證已註冊過";
        }else{
            failText.text = "註冊失敗，請稍後再試";
        }
    }

    public void Btn_ChangeLoginAndSignUp()
    {
        loginUI.SetActive(!loginUI.activeSelf);
        signUpUI.SetActive(!loginUI.activeSelf);
        failText.text = "";
    }

    public void BackToMain()
    {
        loginUI.SetActive(true);
        signUpUI.SetActive(false);
    }
#endregion

    public void Btn_ChooseDocOrSick(bool isDoc)
    {
        chooseUI.SetActive(false);
    
        if(isDoc){
            docUI.SetActive(true);
        }else{
            sickUI.SetActive(true);
        }

        if(useDatabase){
            Message message = Message.Create(MessageSendMode.reliable, (ushort)ClientToServerId.chooseDocOrPatient);
            message.AddBool(isDoc);
            NetworkManager.Singleton.Client.Send(message);
        }
    }

    public void ChooseResult(int myID)
    {
        StartCoroutine(ChooseResult_IE(myID));
    }

    public IEnumerator ChooseResult_IE(int myID)
    {
        yield return new WaitForSeconds(2f);

        if(myID == 1){
            SceneManager.LoadScene("MainSick");
        }else{
            SceneManager.LoadScene("MainDoc");
        }
    }

    public void Btn_BackToChoose()
    {
        chooseUI.SetActive(true);
    }
}
