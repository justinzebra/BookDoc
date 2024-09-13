using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using RiptideNetworking;

public class MainUI_Doc : MonoBehaviour
{
    private static MainUI_Doc _singleton;
    public static MainUI_Doc Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null)
                _singleton = value;
            else if (_singleton != value)
            {
                Debug.Log($"{nameof(MainUI_Doc)} instance already exists, destroying duplicate!");
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

    [Header("Time")]
    public GameObject patientObject;
    public Transform patientCollection;
    public List<Patient> patients;

    //[Header("Settings")]

    [Header("Main")]
    public GameObject newList;
    public GameObject[] newPost;
    public GameObject backToMainBtn;

    [Header("Questionnaire")]
    public InputField[] inputFields;

    [Header("Personal")]
    public InputField address;
    public string departmentIndex;
    public Button[] departmentBtns;
    public InputField openTime;
    public InputField openTime2;
    public InputField docPhoneNumber;

    void Start()
    {
        if(useDatabase){
            for (int i = 0; i < nameText.Length; i++)
            {
                nameText[i].text = Player.list[NetworkManager.Singleton.Client.Id].username;
            }
            address.text = Player.list[NetworkManager.Singleton.Client.Id].address;
            departmentIndex = Player.list[NetworkManager.Singleton.Client.Id].department;
            switch (Player.list[NetworkManager.Singleton.Client.Id].department)
            {
                case "耳鼻喉科":
                    departmentBtns[0].image.color = Color.green;
                    break;
                case "小兒科":
                    departmentBtns[1].image.color = Color.green;
                    break;
                case "皮膚科":
                    departmentBtns[2].image.color = Color.green;
                    break;
            }
            openTime.text = Player.list[NetworkManager.Singleton.Client.Id].openTime;
            openTime2.text = Player.list[NetworkManager.Singleton.Client.Id].openTime2;
            docPhoneNumber.text = Player.list[NetworkManager.Singleton.Client.Id].docPhoneNumber;
            for (int i = 0; i < 8; i++)
            {
                inputFields[i].text = Player.list[NetworkManager.Singleton.Client.Id].questions[i];
            }
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
        Btn_BackToNewPostList();
    }

#region Time

    public void AddPatient(int time, int no, string name, bool isMe, int docIndex)
    {
        foreach (Patient p in patients)
        {
            if(p.nameText.text == name) p.Initialize(time, no, name, isMe);
            return;
        }
        GameObject patientGO = Instantiate(patientObject, patientCollection);
        Patient patient = patientGO.GetComponent<Patient>();

        patient.Initialize(time, no, name, isMe);
        patients.Add(patient);
    }

    public void RemoveReserve(int no)
    {
        Destroy(patientCollection.GetChild(no-1));
        patients.RemoveAt(no-1);
        if(patients.Count == no-1) return;//this no is last number
        for (int i = no; i < patients.Count; i++)
        {
            patients[i].UpdateData(no);
        }
    }

    public void Btn_Skip()
    {
        Message message = Message.Create(MessageSendMode.reliable, (ushort)ClientToServerId.skipReserve);
        NetworkManager.Singleton.Client.Send(message);
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
        newPost[index].SetActive(true);
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

#region Questionnaire

    public void Btn_QuestionnaireSettingFinish()
    {
        if(!useDatabase) return;
        
        Message message = Message.Create(MessageSendMode.reliable, (ushort)ClientToServerId.sendQuestionnaireDesign);
        for (int i = 0; i < inputFields.Length; i++)
        { 
            message.AddString(inputFields[i].text);
        }
        NetworkManager.Singleton.Client.Send(message);
    }

#endregion

#region Pesonal
    public void Btn_SetDepartmentIndex(string index)
    {
        departmentIndex = index;
        for (int i = 0; i < departmentBtns.Length; i++)
        {
            departmentBtns[i].image.color = Color.white;
        }
        switch (index)
            {
                case "耳鼻喉科":
                    departmentBtns[0].image.color = Color.green;
                    break;
                case "小兒科":
                    departmentBtns[1].image.color = Color.green;
                    break;
                case "皮膚科":
                    departmentBtns[2].image.color = Color.green;
                    break;
            }
    }

    public void Btn_StoreClinicInfo()
    {
        if(!useDatabase) return;
        
        Message message = Message.Create(MessageSendMode.reliable, (ushort)ClientToServerId.storeClinicInfo);
        message.AddString(address.text);
        message.AddString(departmentIndex);
        message.AddString(openTime.text);
        message.AddString(openTime2.text);
        message.AddString(docPhoneNumber.text);
        NetworkManager.Singleton.Client.Send(message);
    }

    public void Btn_LogOut()
    {
        SceneManager.LoadScene("Menu");
    }

#endregion

}
