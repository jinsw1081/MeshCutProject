using UnityEngine;
/// <summary>
/// 객체를 드래그해서 화면상에서 이동 관리하는 클래스
/// </summary>
public class Objects : MonoBehaviour
{
    Vector3 mOffset;        
    float mZCoord;
    GameObject Gizmos;  //이동 기즈모

    private void Start()
    {
        Gizmos = GameObject.Find("translate_gizmo");
    }
    //마우스 좌표랑 오브젝트 위치랑 
    void OnMouseDown()
    {
        //카메라의 z만 월드좌표로 바꾸기
        mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        
        mOffset = gameObject.transform.position - GetMouseAsWorldPointZ();
    }

    private Vector3 GetMouseAsWorldPointZ()
    {
        //마우스 위치받아오기
        Vector3 mousePoint = Input.mousePosition;
        //카메라 위치 z축 할당
        mousePoint.z = mZCoord;
        //월드 좌표로 바꾸기
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
    //
    void OnMouseDrag()
    {
        //SaveBox의 UI도 이 함수로 움직이는데 SaveBox에 할당이 안되어있을때
        //움직이는 것을 막기 위해서 조건문을 걸어줌
        if (1<GetComponent<MeshFilter>().mesh.vertexCount)
        {
            transform.position = GetMouseAsWorldPointZ() + mOffset;
            Gizmos.SetActive(true);
            Gizmos.transform.position = transform.position;
        }
    }
    
   
}
    