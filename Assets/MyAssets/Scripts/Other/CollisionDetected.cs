using UnityEngine;

/// <summary>
/// others 태그를 가지는 객체들의 충돌 감지 클래스
/// </summary>
public class CollisionDetected : MonoBehaviour
{
    GameObject uIInformation;   //충돌한 오브젝트 정보 표시하는 ui창
    UiInformationScript uiInformationScript;    //ui스크립트 정보 표시
    
    private void Start()
    {
        uIInformation = GameObject.Find("uiInformation");
        uiInformationScript = uIInformation.GetComponent<UiInformationScript>();
    }
    //콜라이더 안으로 들어온 충돌감지 
    private void OnTriggerEnter(Collider other)
    {
        if (MouseManager.selectedObject == gameObject)
        {
            uiInformationScript.AddBumpedObject(other.gameObject, gameObject);
        }
    }
    //콜라이더의 경계 부분을 거치지않고 바로 중심으로 들어와 버렷을때
    //gizmo로 움직이는 경우가 이에 해당함
    private void OnTriggerStay(Collider other)
    {
        if (MouseManager.selectedObject == gameObject&&
            uiInformationScript.SearchBumpedObject(other.gameObject, gameObject))
        {
            uiInformationScript.AddBumpedObject(other.gameObject, gameObject);
           
        }
    }
    //콜라이더 밖으로 나간 충돌감지
    private void OnTriggerExit(Collider other)
    {
        uiInformationScript.RemoveBumpedObject(other.gameObject, gameObject);
    }
}
