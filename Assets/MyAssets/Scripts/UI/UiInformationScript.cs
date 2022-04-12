using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// UI정보를 표기하는 ui를 관리하는 클래스 
/// </summary>

public class UiInformationScript : MonoBehaviour
{
    //전체적인 할당 방법 그 객체 이름으로  Dictionary<string, List<GameObject>> 키값에 리스트를 담아서
    //객체에 물체 충돌 할때마다 해당 키값을 가지는 리스트에 빼고 넣고 하는 방식으로
    Text ObjText;           //자기 자신객체 이름
    Text bumpedObjTexts;    //부딪힌 객체 이름

    //부딪힌 객체들이 저장되는 List를 가지는 Ditcionary
    Dictionary<string, List<GameObject>> dictionaryofListBumpedGameobj 
        = new Dictionary<string, List<GameObject>>();
    
    
    void Start()
    {
        ObjText = transform.Find("OBJ").GetComponent<Text>();
        bumpedObjTexts = transform.Find("CollsionObj1").GetComponent<Text>();
    }
 
    private void LateUpdate()
    {
        SetText();
    }
    //others의 CollisonDetected에서 호출함
    //Dictionary에 추가하는함수
    public void AddBumpedObject(GameObject other, GameObject myself)
    {
        if (dictionaryofListBumpedGameobj.ContainsKey(other.name))
        {
            if (!dictionaryofListBumpedGameobj[other.name].Contains(myself))
                dictionaryofListBumpedGameobj[other.name].Add(myself);
        }
        else
        {
            List<GameObject> newList = new List<GameObject>();
            newList.Add(myself);
            dictionaryofListBumpedGameobj.Add(other.name, newList);
        }
    }
    //others의 CollisonDetected에서 호출함
    //Dictionary에 추가하는삭제
    public void RemoveBumpedObject(GameObject other, GameObject myself)
    {
        if (dictionaryofListBumpedGameobj.ContainsKey(other.name))
        {
            if (dictionaryofListBumpedGameobj[other.name].Contains(myself))
                dictionaryofListBumpedGameobj[other.name].Remove(myself);
        }
    }
    //Dictionary안에 그 객체가 있는지 검사하는 함수
    public bool SearchBumpedObject(GameObject other, GameObject myself)
    {
        if (dictionaryofListBumpedGameobj.ContainsKey(other.name))
        {
            if (dictionaryofListBumpedGameobj[other.name].Contains(myself))
                return false;
        }
        return true;
    }

    //ui정보의 텍스트 바꾸기
    public void SetText()
    {
        if (MouseManager.notChangedObject)
        {
            ObjText.text = MouseManager.notChangedObject.name;
            bumpedObjTexts.text = null;
            if (dictionaryofListBumpedGameobj.ContainsKey(MouseManager.notChangedObject.name))
            {
                for (int i = 0; i < dictionaryofListBumpedGameobj[MouseManager.notChangedObject.name].Count; i++)
                {
                    bumpedObjTexts.text += dictionaryofListBumpedGameobj
                        [MouseManager.notChangedObject.name][i].name + ", ";
                }
            }
        }
    }
}
