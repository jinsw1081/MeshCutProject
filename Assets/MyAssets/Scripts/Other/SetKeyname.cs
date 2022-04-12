using UnityEngine;
using UnityEngine.UI;

public class SetKeyname : MonoBehaviour
{
    /// <summary>
    /// SaveLoad할때의 Inputfield 이름을 바꾸는 클래스
    /// </summary>
    GameObject can; //UI canvas
    private void OnEnable()
    {
        can = GameObject.Find("Canvas");
    }
    //버튼 눌러서 input필드 이름 바꾸기
    public void OnclickSetKeyname()
    {
        // inputfield는 모델리스트 버튼 누르면 나타나는 창... 
        InputField inputField;  //saveLoad할때 이름 입력하는 창
        inputField = can.transform.Find("InputField").GetComponent<InputField>();
        string str=transform.GetChild(0).GetComponent<Text>().text;
        inputField.text = str;
        GameObject.Find("PlayfabManger").GetComponent<CDN>().SetNamefun(str);
    }
}
