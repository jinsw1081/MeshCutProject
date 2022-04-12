using UnityEngine;
/// <summary>
///마우스  관련 셀렉션 박스(빨간색 격자[] 사각형 박스 ),객체 정보 스크립트
///CamCon과 나눈 이유 카메라 컨트롤은 대부분 카메라 관련 함수 또는 클릭한 객체를 이동시키는 함수가 
///주를 이루고 있고 이 클래스는 클릭한 객체에대한 정보는 수정하지 않고 ui랑 셀력센 박스 위치만 조정함
/// </summary>
public class MouseManager : MonoBehaviour {
    public static GameObject selectedObject;    //선택된 객체
    public static GameObject notChangedObject; //others가 아닌 객체들만을 가지는 변수
    public GameObject uiImformation;   //객체 정보를 표시하는 UI
    int Mask;

    void LateUpdate() {
        //오브젝트인지 확인하여서 오브젝트가 맞고 others태그 아닌지 확인
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition)
            , out RaycastHit hitInfo, 100,Mask = 1 << 9)) {
            GameObject hitObject = hitInfo.collider.gameObject;
            if(hitInfo.collider.tag!="others"&&Input.GetMouseButtonDown(0))
            notChangedObject= hitInfo.collider.gameObject;
            SelectObject(hitObject);
        }
       
        else {
			ClearSelection();
		}

        //선택된 오브젝트가 있다면 그 사이즈를 측정해서  
        //측정한 사이즈로 셀렉션 박스를 그크기로 만든다
        if (selectedObject != null)
        {
            GetComponentInChildren<Renderer>().enabled = true;
            Bounds bigBounds = selectedObject.GetComponentInChildren<Renderer>().bounds;
            float padding = 1.0f;
            this.transform.position = new Vector3(bigBounds.center.x,
                bigBounds.center.y, bigBounds.center.z);
            this.transform.localScale = new Vector3(bigBounds.size.x * padding,
                bigBounds.size.y * padding, bigBounds.size.z * padding);
        }
        else
        {
            GetComponentInChildren<Renderer>().enabled = false;
        }
        //클릭한 오브젝트가 잇을경우 오브젝트 정보  위치 조정
        if (notChangedObject)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(notChangedObject.
                transform.position);
            
            uiImformation.transform.position = screenPos;
        }
    }
    
    //오브젝터 선택
	void SelectObject(GameObject obj) {
		if(selectedObject != null) {
			if(obj == selectedObject)
				return;

			ClearSelection();
		}
		selectedObject = obj;
	}
    //선택된 오브젝트 할당해제
	void ClearSelection() {
		if(selectedObject == null)
			return;
        
		selectedObject = null;
    }


}
