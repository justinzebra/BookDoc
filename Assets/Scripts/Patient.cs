using UnityEngine;
using UnityEngine.UI;

public class Patient : MonoBehaviour
{
    public Text timeText;
    public Text noText;
    public Text nameText;
    //public Text genderText;
    public float waitMin;
    public GameObject skipBtn;

    public void Initialize(int time, int no, string name, bool isMe)
    {
        //waitMin = float.Parse(time) * 60 + 60;//second to min and plus 1min
        if(no == 1){
            timeText.text = "";
        }else{
            if(time == 0){
                timeText.text = "請稍候";
            }else{
                timeText.text = time + "分鐘";
            }
        }
        noText.text = no.ToString();
        nameText.text = name;
        //genderText.text = gender;
    }

    public void UpdateData(int no) 
    {
        if(no == 1){
            timeText.text = "";
        }
        noText.text = no.ToString();
    }
}
