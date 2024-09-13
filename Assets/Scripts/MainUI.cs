using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using RiptideNetworking;

public class MainUI : MonoBehaviour
{
    private static MainUI _singleton;
    public static MainUI Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null)
                _singleton = value;
            else if (_singleton != value)
            {
                Debug.Log($"{nameof(MainUI)} instance already exists, destroying duplicate!");
                Destroy(value);
            }
        }
    }

    private void Awake()
    {
        Singleton = this;
    }

    public GameObject[] UI;
    public GameObject[] UI_Image_UnSelect;
    public GameObject[] UI_Image_Select;

    public Text[] nameText;

    [Header("For Test")]
    public bool useDatabase;

    [Header("Favorite")]
    int currentFavoriteIndex;
    public Transform favoriteCollection;
    public GameObject[] favoriteUI;
    public GameObject[] nextPreviousBtn; // 0:leftCan 1:leftCant 2:rightCan 3:rightCant
    public InputField searchField_Favoirte;
    public GameObject searchList_Favorite;
    public GameObject[] searchListOptions_Favorite;
    //public Text[] searchText;

    //[Header("Settings")]

    [Header("Main")]
    public GameObject newList;
    public GameObject[] newPost;
    public GameObject backToMainBtn;

    [Header("Search")]
    public InputField searchField_Search;
    public GameObject searchList_Search;
    public GameObject[] searchListOptions_Search;
    public Dropdown[] dropdowns_Qestionnaire;
    public Dropdown[] dropdowns_Qestionnaire2;
    public InputField[] inputFields_Qestionnaire;
    public InputField[] inputFields_Qestionnaire2;
    public Text[] questionTexts;
    public Text[] questionTexts2;
    public Text[] docNameText;
    public Text[] addressText;
    public Text[] departmentText;
    public Text[] openText;
    public Text[] openText2;
    public Text[] docPhoneText;
    public GameObject[] isCloseBtn;

    [Header("Personal")]
    public GameObject[] personUI;
    public GameObject backToPersonBtn;
    //ClinicProgress
    public GameObject patientObject;
    public Transform patientCollection;
    public List<Patient> patients;
    public InputField userIDInputField;
    public InputField emailInputField;
    public InputField phoneNumberInputField;
    public GameObject clinicProgressPage1;
    public Text docNameText_clinicProgress;
    public Text dateText;

    void Start()
    {
        if(useDatabase){
            for (int i = 0; i < nameText.Length; i++)
            {
                nameText[i].text = Player.list[NetworkManager.Singleton.Client.Id].username;
            }
            userIDInputField.text = Player.list[NetworkManager.Singleton.Client.Id].userID;
            emailInputField.text = Player.list[NetworkManager.Singleton.Client.Id].email;
            phoneNumberInputField.text = Player.list[NetworkManager.Singleton.Client.Id].phoneNumber;
        }
        
    }

    public void Btn_Page(int index)
    {
        for (int i = 0; i < UI.Length; i++)
        {
            UI[i].SetActive(false);
            UI_Image_Select[i].SetActive(false);
            UI_Image_UnSelect[i].SetActive(true);
        }
        UI[index].SetActive(true);
        UI_Image_UnSelect[index].SetActive(false);
        UI_Image_Select[index].SetActive(true);

        //Initialize
        FavoriteInitialize();
        Btn_BackToNewPostList();
        Btn_BackToPersonMain();
    }

#region Favorite
    public void FavoriteInitialize()
    {
        currentFavoriteIndex = 0;
       for (int i = 0; i < favoriteUI.Length; i++)
        {
            favoriteUI[i].SetActive(false);
        }
        
        nextPreviousBtn[0].SetActive(false);
        nextPreviousBtn[1].SetActive(true);
        nextPreviousBtn[2].SetActive(true);
        nextPreviousBtn[3].SetActive(false);
    }

    public void Btn_Favorite_Next()
    {
        if(currentFavoriteIndex == favoriteUI.Length-1) return;

        currentFavoriteIndex++;
        if(currentFavoriteIndex == favoriteUI.Length-1){
            nextPreviousBtn[0].SetActive(true);
            nextPreviousBtn[1].SetActive(false);
            nextPreviousBtn[2].SetActive(false);
            nextPreviousBtn[3].SetActive(true);
        }
    }

    public void Btn_Favorite_Previous()
    {
        if(currentFavoriteIndex == 0) return;
        
        currentFavoriteIndex--;
        if(currentFavoriteIndex == 0){
            nextPreviousBtn[0].SetActive(false);
            nextPreviousBtn[1].SetActive(true);
            nextPreviousBtn[2].SetActive(true);
            nextPreviousBtn[3].SetActive(false);
        }
    }

    public void Btn_AddFavorite()
    {
        int id = 0; // need to set
        favoriteUI[id].SetActive(true);
        Message message = Message.Create(MessageSendMode.reliable, (ushort)ClientToServerId.addFavorite);
        message.AddInt(id);
        NetworkManager.Singleton.Client.Send(message);
    }

    public void Btn_Favorite_Cancel(int id)
    {
        Destroy(favoriteUI[currentFavoriteIndex]);
        favoriteUI = new GameObject[0];
        for (int i = 0; i < favoriteCollection.childCount; i++)
        {
            favoriteUI[i] = favoriteCollection.GetChild(i).gameObject;
        }

        Message message = Message.Create(MessageSendMode.reliable, (ushort)ClientToServerId.cancelFavorite);
        message.AddInt(id);
        NetworkManager.Singleton.Client.Send(message);
    }

    public void CheckSearchString_Favorite()
    {
        switch (searchField_Favoirte.text)
        {
            case "":
                searchList_Favorite.SetActive(false);
                searchListOptions_Favorite[0].SetActive(false);
                searchListOptions_Favorite[1].SetActive(false);
                break;
            case "大":
                searchList_Favorite.SetActive(true);

                searchListOptions_Favorite[0].SetActive(true);
                searchListOptions_Favorite[1].SetActive(false);
                /*for (int i = 0; i < searchText.Length; i++)
                {
                    searchText[i].text = "";
                }
                searchText[0].text = "大安";*/
                break;
            case "蕭":
                searchList_Favorite.SetActive(true);

                searchListOptions_Favorite[0].SetActive(false);
                searchListOptions_Favorite[1].SetActive(true);
                /*for (int i = 0; i < searchText.Length; i++)
                {
                    searchText[i].text = "";
                }
                searchText[0].text = "大安";*/
                break;
            default:
                /*searchList.SetActive(false);
                for (int i = 0; i < searchText.Length; i++)
                {
                    searchText[i].text = "";
                }*/
                break;
        }
    }

    public void Btn_SearchFavorite(int index)
    {
        searchList_Favorite.SetActive(false);
        switch (index)
        {
            case 0:
                favoriteUI[0].SetActive(true);
                searchField_Favoirte.text = "";
                break;
            case 1:
                favoriteUI[1].SetActive(true);
                searchField_Favoirte.text = "";
                break;
        }
    }
#endregion

#region Settings

#endregion

#region Main

    public void Btn_newPost(int index)
    {
        newList.SetActive(false);
        for (int i = 0; i < newPost.Length; i++)
        {
            newPost[i].SetActive(false);
        }
        newPost[index].SetActive(false);
        backToMainBtn.SetActive(true);
    }

    public void Btn_BackToNewPostList()
    {
        newList.SetActive(true);
        for (int i = 0; i < newPost.Length; i++)
        {
            newPost[i].SetActive(false);
        }
        backToMainBtn.SetActive(false);
    }

#endregion

#region Search
    public void Btn_CompleteQuestionnaire(int _docIndex)
    {
        if(!useDatabase) return;

        Message message = Message.Create(MessageSendMode.reliable, (ushort)ClientToServerId.sendQuestionnaire);
        message.AddInt(_docIndex);//use for check doc index.
        if(_docIndex == 0){//dropdown.options[dropdown.value].text
            message.AddString(dropdowns_Qestionnaire[0].options[dropdowns_Qestionnaire[0].value].text);
            message.AddString(dropdowns_Qestionnaire[1].options[dropdowns_Qestionnaire[1].value].text);
            message.AddString(dropdowns_Qestionnaire[2].options[dropdowns_Qestionnaire[2].value].text);
            message.AddString(dropdowns_Qestionnaire[3].options[dropdowns_Qestionnaire[3].value].text);
            message.AddString(dropdowns_Qestionnaire[4].options[dropdowns_Qestionnaire[4].value].text);
            message.AddString(dropdowns_Qestionnaire[5].options[dropdowns_Qestionnaire[5].value].text);
            message.AddString(inputFields_Qestionnaire[0].text);
            message.AddString(inputFields_Qestionnaire[1].text);
        }else if(_docIndex == 1){
            message.AddString(dropdowns_Qestionnaire[0].options[dropdowns_Qestionnaire[0].value].text);
            message.AddString(dropdowns_Qestionnaire[1].options[dropdowns_Qestionnaire[1].value].text);
            message.AddString(dropdowns_Qestionnaire[2].options[dropdowns_Qestionnaire[2].value].text);
            message.AddString(dropdowns_Qestionnaire[3].options[dropdowns_Qestionnaire[3].value].text);
            message.AddString(dropdowns_Qestionnaire[4].options[dropdowns_Qestionnaire[4].value].text);
            message.AddString(dropdowns_Qestionnaire[5].options[dropdowns_Qestionnaire[5].value].text);
            message.AddString(inputFields_Qestionnaire[1].text);
            message.AddString(inputFields_Qestionnaire[2].text);
        }
        NetworkManager.Singleton.Client.Send(message);
    }

    public void Btn_Search(int index)
    {
        searchList_Search.SetActive(false);
        searchField_Search.text = "";
    }

    public void CheckSearchString_Search()
    {
        switch (searchField_Search.text)
        {
            case "":
                searchList_Search.SetActive(false);
                searchListOptions_Search[0].SetActive(false);
                searchListOptions_Search[1].SetActive(false);
                break;
            case "大":
                searchList_Search.SetActive(true);

                searchListOptions_Search[0].SetActive(true);
                searchListOptions_Search[1].SetActive(false);
                /*for (int i = 0; i < searchText.Length; i++)
                {
                    searchText[i].text = "";
                }
                searchText[0].text = "大安";*/
                break;
            case "蕭":
                searchList_Search.SetActive(true);

                searchListOptions_Search[0].SetActive(false);
                searchListOptions_Search[1].SetActive(true);
                /*for (int i = 0; i < searchText.Length; i++)
                {
                    searchText[i].text = "";
                }
                searchText[0].text = "大安";*/
                break;
            default:
                /*searchList.SetActive(false);
                for (int i = 0; i < searchText.Length; i++)
                {
                    searchText[i].text = "";
                }*/
                break;
        }
    }

    public void Btn_GetClinicInfo(int docIndex)
    {
        if(!useDatabase) return;
        isCloseBtn[docIndex].SetActive(true);
        docNameText[docIndex].text = "";
        addressText[docIndex].text = "";
        departmentText[docIndex].text = "";
        openText[docIndex].text = "";
        openText2[docIndex].text = "";
        docPhoneText[docIndex].text = "";

        Message message = Message.Create(MessageSendMode.reliable, (ushort)ClientToServerId.getGetClinicInfo);
        message.AddInt(docIndex);
        NetworkManager.Singleton.Client.Send(message);
    }

    public void Btn_GetQuestionnaireDesign(int docIndex)
    {
        if(!useDatabase) return;
        if(docIndex == 0){
            questionTexts[0].text = "";
            questionTexts[1].text = "";
            questionTexts[2].text = "";
            questionTexts[3].text = "";
            questionTexts[4].text = "";
            questionTexts[5].text = "";
            questionTexts[6].text = "";
            questionTexts[7].text = "";
        }else{
            questionTexts2[0].text = "";
            questionTexts2[1].text = "";
            questionTexts2[2].text = "";
            questionTexts2[3].text = "";
            questionTexts2[4].text = "";
            questionTexts2[5].text = "";
            questionTexts2[6].text = "";
            questionTexts2[7].text = "";
        }

        Message message = Message.Create(MessageSendMode.reliable, (ushort)ClientToServerId.getQuestionnaireDesign);
        message.AddInt(docIndex);
        NetworkManager.Singleton.Client.Send(message);
    }

    public void SetClinicInfo(int docIndex, bool isOnline, string name, string address, string department, string openTime, string openTime2, string docPhoneNumber)
    {
        isCloseBtn[docIndex].SetActive(!isOnline);
        docNameText[docIndex].text = name;
        addressText[docIndex].text = address;
        departmentText[docIndex].text = department;
        openText[docIndex].text = openTime;
        openText2[docIndex].text = openTime2;
        docPhoneText[docIndex].text = docPhoneNumber;
    }

    public void GetQuestionnaireDesign(int docIndex, string question, string question2, string question3, string question4, string question5, string question6, string question7, string question8)
    {
        if(docIndex == 0){
            questionTexts[0].text = question;
            questionTexts[1].text = question2;
            questionTexts[2].text = question3;
            questionTexts[3].text = question4;
            questionTexts[4].text = question5;
            questionTexts[5].text = question6;
            questionTexts[6].text = question7;
            questionTexts[7].text = question8;
        }else{
            questionTexts2[0].text = question;
            questionTexts2[1].text = question2;
            questionTexts2[2].text = question3;
            questionTexts2[3].text = question4;
            questionTexts2[4].text = question5;
            questionTexts2[5].text = question6;
            questionTexts2[6].text = question7;
            questionTexts2[7].text = question8;
        }
    }
#endregion

#region Pesonal
    //ClinicProgress
    public void AddPatient(int time, int no, string name, bool isMe, bool isDoc, int docIndex)
    {
        foreach (Patient p in patients)
        {
            if(p.nameText.text == name) p.Initialize(time, no, name, isMe);
            return;
        }
        if(patients.Count == 0){
            clinicProgressPage1.SetActive(true);
            if(docIndex == 0){
                docNameText_clinicProgress.text = "大安耳鼻喉科診所";
            }else{
                docNameText_clinicProgress.text = "蕭奕仁診所";
            }
            dateText.text = DateTime.Now.ToString("yyyy/MM/dd");
        }
        GameObject patientGO = Instantiate(patientObject, patientCollection);
        Patient patient = patientGO.GetComponent<Patient>();
        
        patient.Initialize(time, no, name, isMe);
        patients.Add(patient);
    }

    public void RemoveReserve(int no)
    {
        Destroy(patientCollection.GetChild(no-1).gameObject);
        patients.RemoveAt(no-1);
        if(patients.Count == no-1) return;//this no is last number
        for (int i = no; i < patients.Count; i++)
        {
            patients[i].UpdateData(no);
        }
    }

    public void Btn_CancelReserve()
    {
        if(!useDatabase) return;
        int docIndex = 0;
        if(docNameText_clinicProgress.text == "大安耳鼻喉科診所"){
            docIndex = 0;
        }else{
            docIndex = 1;
        }

        Message message = Message.Create(MessageSendMode.reliable, (ushort)ClientToServerId.cancelReserve);
        message.AddString(Player.list[NetworkManager.Singleton.Client.Id].userID);
        message.AddInt(docIndex);
        NetworkManager.Singleton.Client.Send(message);
    }

    //PersonalInfo
    public void Btn_StorePersonalInfo()
    {
        if(!useDatabase) return;
        
        Message message = Message.Create(MessageSendMode.reliable, (ushort)ClientToServerId.storePersonalInfo);
        message.AddString(userIDInputField.text);
        message.AddString(emailInputField.text);
        message.AddString(phoneNumberInputField.text);
        NetworkManager.Singleton.Client.Send(message);
    }

    //Btns
    public void Btn_ClinicProcess()
    {
        personUI[0].SetActive(false);
        personUI[1].SetActive(true);
        backToPersonBtn.SetActive(true);
    }
    
    public void Btn_ClinicRecord()
    {
        personUI[0].SetActive(false);
        personUI[2].SetActive(true);
        backToPersonBtn.SetActive(true);
    }
    
    public void Btn_PersonalInfo()
    {
        personUI[0].SetActive(false);
        personUI[3].SetActive(true);
        backToPersonBtn.SetActive(true);
    }

    public void Btn_LogOut()
    {
        SceneManager.LoadScene("Menu");
    }

    public void Btn_BackToPersonMain()
    {
        for (int i = 0; i < personUI.Length; i++)
        {
            personUI[i].SetActive(false);
        }
        personUI[0].SetActive(true);
        backToPersonBtn.SetActive(false);
    }

#endregion

}
