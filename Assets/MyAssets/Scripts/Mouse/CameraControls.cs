using UnityEngine;

/// <summary>
/// 전체적인 마우스로 객체를 이동하거나 카메라를 움직이는걸 관리하는 클래스
/// 기능 마우스 
/// 오른쪽 클릭 끌기 :화면을 끌면 화면을 회전 합니다.
/// 가운데 휠 클릭 : 오브젝트 지정(빨간색으로)
/// 가운뒤 휠 끌기 : 화면 움직이기
/// 가운뒤 휠 스크롤 위 아래: 멀어지기 가까워지기
/// SaveBox 저장 또는 가져오기
/// </summary>

public class CameraControls : MonoBehaviour
{
	const string INPUT_MOUSE_SCROLLWHEEL = "Mouse ScrollWheel";
	const string INPUT_MOUSE_X = "Mouse X";
	const string INPUT_MOUSE_Y = "Mouse Y";
    
    bool checkKeepSave = true;  //인벤토리의 객체를 연속생성할때 toggle
    public GameObject Gizmo;    //이동 Gizmo
    public GameObject bumpedUi; //정보표시 하는 객체 ui
    
    UiInformationScript uiInformationScript; //객체 정보 표시하는 ui 스크립트

    RaycastHit hit;
    int Mask;   //9:obj 10:ui 11: 이동 기즈모 12: 회전 기즈모
    int mouseDownSelectObject = 0; //0: 1,2가아닌것 1:obj가 선택되었음 2: UI가 선택되었음
                                   //마우스를 누를때 선택된 객체 구분하기위해서 
    
    public static GameObject selectionObj;  //지금 마우스로선택된 오브젝트
    public static GameObject TargetObj1;    //빨간색으로 설정된 오브젝트1 
    public static GameObject TargetObj2;    //빨간색으로 설정된 오브젝트2
    public static Vector3 originPos;        //(selectionObj) 선택된 오브젝트원래위치   

    public static bool gizmoMoveOn=false;
    
    [Range(1f, 10f)] //인스펙터창에서  드래그로 조정 가능하게 해줌
	public float orbitSpeed = 1f;  //회전속도
	[Range(.3f,2f)]
	public float zoomSpeed = .8f; //확대//축소 속도
    
    void Start()
	{
        uiInformationScript = bumpedUi.GetComponent<UiInformationScript>();
    }

    //selectionObj등 스태틱 함수에 할당된 값을 인스펙터창에서 보기위해서
    //public GameObject visibleTargetObj1;
    //public GameObject visibleTargetObj2;
    //public GameObject visibleselectionObj;
    //private void Update() 
    //{
    //    visibleTargetObj1 = TargetObj1;
    //    visibleTargetObj2 = TargetObj2;
    //    visibleselectionObj = selectionObj;
    //}

    void LateUpdate() // 마우스 클릭과 관련된 이벤트 처리 
    {
        if (Input.GetMouseButtonDown(0)) //마우스 왼쪽을 누를때
        {
            //마우스 포지션 위치 기준으로 Ray를 만듬
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //레이를 기준으로 빛을 100길이 만큼 쏴서 레이어 11에 닿는다면
            //hit으로 맞은 객체를 반환한다.
             
            if (Physics.Raycast(ray, out hit, 100, Mask = 1 << 11))  // 11은 객체에 붙어 다니는 xyz 기즈모 
            {
                originPos = selectionObj.transform.position;
                mouseDownSelectObject = 0;
                //GizmoMove 스크립트에서 사용하기위해서 누를때 현재위치를 할당해줌
            }
            //마우스 클릭이 obj(body)에 맞았을때 
            //이동 기즈모를 obj위치로 이동
            else if (Physics.Raycast(ray, out hit, 100, Mask = 1 << 9))  // scene내의 일반 object
            {
                selectionObj = hit.collider.gameObject;
                mouseDownSelectObject = 1;
                if (selectionObj.tag != "others")
                    uiInformationScript.SetText();
                Gizmo.transform.position = hit.transform.position;
            }
            //카메라 ui닫았을때 (세이브 박스) 
            //mouseDownSelectObject 설정해준다
            else if (Physics.Raycast(ray, out hit, 100, Mask = 1 << 10))  //saveBOx의 object.. 
            {
                selectionObj = hit.collider.gameObject;
                //콜라이더 false해주는 이유 아래의 MousebuttonUp을 할때의 인식을 안당하기위해서
                //안해주게 되면 부자연스러움발생;
                selectionObj.GetComponent<Collider>().enabled = false;
                mouseDownSelectObject = 2;
            }
            //회전 기즈모에 다았을때 회전 기즈모 함수 호출
            else if (Physics.Raycast(ray, out hit, 100, Mask = 1 << 12))
            {
                GizmoRotate gizmoRotateSc=transform.Find("SetRotateGizmo")
                    .GetComponent<GizmoRotate>();
                int num = gizmoRotateSc.StringConverttoInt(hit.collider.name);
                if(num!=7)
                {
                    gizmoRotateSc.SetRotateCamera(num);
                }
            }
            else
            {
                Gizmo.transform.position=new Vector3(-100,-100,-100);
            }
        }

        // 인벤토리와 관련된 기능... 
        //누른 왼쪽 마우스 버튼이 올라왔을때(누르고 손에서 땠을 때)
        if (Input.GetMouseButtonUp(0)) // 0이 마우스 왼쪽 클릭 
        {
            if (selectionObj)
                selectionObj.GetComponent<Collider>().enabled = true;
            
            Mask = 1 << 10;  //ui에서 손을 땠을 때  MASK 10은 camera UI  save Box가 10번에 있음. 
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, Mask))
            {
                if (selectionObj)
                {
                    //마우스 왼쪽 버튼을 땟을때 그 위치가 SaveBox일때 그 처음 선택된 obj가
                    //obj엿을때 세이브 박스에 저장된다.
                    if (selectionObj.tag != "others")
                    {
                        
                        if (mouseDownSelectObject == 1) 
                        {
                            hit.collider.transform.GetComponent<MeshFilter>().mesh =
                           selectionObj.GetComponent<MeshFilter>().mesh;
                            hit.collider.transform.name = selectionObj.transform.name;
                            Gizmo.transform.position=new Vector3(-100,-100,-100); // 안으로 가지고 들어오므로 gizmo를 deactivation 시킨다. 
                            bumpedUi.transform.position = new Vector3(-100, -100, -100); // UI정보창을 안보이게... 한다... 
                            Destroy(selectionObj);
                        }
                        //처음 선택된 obj가 ui엿을때 SaveBox안에있는 객체를 밖으로 복사하는 부분
                        else if (mouseDownSelectObject == 2) // 밖으로 물건을 가지고 나갈때... 
                        {
                            int num = selectionObj.GetComponent<MeshFilter>().mesh.vertices.Length;
                            if (num > 1)    
                            {
                                selectionObj.transform.position = hit.point; // 
                                GameObject Gma = Instantiate<GameObject>(selectionObj,  // 오브젝트 생성하게 하고 
                                selectionObj.transform.position, transform.rotation);
                                string name = hit.collider.transform.name;
                                //hit.collider.transform.name = null;
                                Gizmo.transform.position = new Vector3(-100, -100, -100);
                                Gma.name = name;        //이름에 clone안생기게하는부분;
                                DestroyImmediate(Gma.GetComponent<BoxCollider>());
                                Gma.transform.rotation = Quaternion.Euler(Vector3.one);
                                Gma.AddComponent<MeshCollider>();
                                
                                Gma.layer = 9;
                                if(!checkKeepSave)
                                hit.collider.transform.GetComponent<MeshFilter>().mesh = null; // hit.collider 는 saveBox의 collider를 의미한다. 
                            }
                        }
                    }
                }
            }
            transform.Find("SaveBox").GetChild(0).localPosition = Vector3.zero;
            transform.Find("SaveBox2").GetChild(0).localPosition = Vector3.zero;
            transform.Find("SaveBox3").GetChild(0).localPosition = Vector3.zero;
            mouseDownSelectObject = 0;
        }
        //마우스 오른쪽키 회전 부분  ==> trackball 기능
        if (Input.GetMouseButton(1) && mouseDownSelectObject == 0)
        {
            float rot_x = Input.GetAxis(INPUT_MOUSE_X);
            float rot_y = -Input.GetAxis(INPUT_MOUSE_Y);

            //getAxis는 마우스를 누른 상태에서 처음 위치를 측정한뒤
            //지금 위치값이랑 맨처음 누른 위치값이랑 뺄셈한 결과
            Vector3 eulerRotation = transform.localRotation.eulerAngles;
            eulerRotation.x += rot_y * orbitSpeed;
            eulerRotation.y += rot_x * orbitSpeed;
            //가져온 오일러 값으로
            //마우스 x축은 rotation y값을 수정하고 마우스y축은 rotation x값을 수정한다
            transform.localRotation = Quaternion.Euler(eulerRotation);
            
            // Gizmo.transform.localRotation= Quaternion.Euler(eulerRotation);
        }
        //마우스 휠을 위아래로 굴린 값을 받아서 
        //카메라 위치 수정해주는 부분
        if ( Input.GetAxis(INPUT_MOUSE_SCROLLWHEEL) != 0f )
		{
			float delta = Input.GetAxis(INPUT_MOUSE_SCROLLWHEEL);
            if(delta>0)
            transform.position += transform.forward ;
            else
                transform.position -= transform.forward * 2;

        }

        //마우스 휠 패닝 (화면이동)
        //화면 전체를 4사분면으로 구분하여서 어느부분에 위치해있느냐에따라서
        //카메라를 위치를 카메라 위치기준으로 위 아래 왼쪽 오른쪽으로만 이동
        if (Input.GetMouseButton(2))
        {
            float _x = Input.GetAxis("Mouse X");
            float _y = Input.GetAxis("Mouse Y");

            Vector3 curPos = transform.position;
            if (Input.GetAxis("Mouse X") > 0)
                curPos = curPos - transform.right / 20;
            else if (Input.GetAxis("Mouse X") < 0)
                curPos = curPos + transform.right / 20;
            if (Input.GetAxis("Mouse Y") > 0)
                curPos = curPos - transform.up / 20;
            else if (Input.GetAxis("Mouse Y") < 0)
                curPos = curPos + transform.up / 20;
            transform.position = curPos;
        }

        //오브젝트 휠버튼을 눌었을 때 지정하는(빨간색으로)만드는 부분
        
        if (Input.GetMouseButtonDown(2))  // 2는 가운데 휠 버튼을 눌렀을 때의 의미 
        {
            Mask = 1 << 9;
            
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, Mask))
            {
                //TargetObj1 이 기존에 선택되었던 것일경우
                if (TargetObj1 == hit.collider.gameObject) // targetOBject는 두개만 선택될 수 있다.. 지정되면 빨간색으로 표시됨..  
                {
                    TargetObj1.GetComponent<Renderer>().material.color = Color.white;
                    TargetObj1 = TargetObj2;
                    TargetObj2 = null;
                }
                //TargetObj2 이 기존에 선택되었던 것일경우
                else if (TargetObj2 == hit.collider.gameObject)
                {
                    TargetObj2.GetComponent<Renderer>().material.color = Color.white;
                    TargetObj2 = null;
                }
                else
                {
                    //TargetObj1이 없을경우
                    if (!TargetObj1)
                    {
                        TargetObj2 = hit.collider.gameObject;
                        TargetObj2.GetComponent<Renderer>().material.color = Color.red;
                        Vector3 ve3 = TargetObj2.GetComponent<Renderer>().bounds.size;
                        TargetObj1 = TargetObj2;
                        TargetObj2 = null;
                    }
                    //TargetObj2이 없을경우
                    else if (!TargetObj2)
                    {
                        TargetObj2 = hit.collider.gameObject;
                        TargetObj2.GetComponent<Renderer>().material.color = Color.red;
                    }
                    //둘다 있을경우 하나삭제
                    else
                    {
                        TargetObj1.GetComponent<Renderer>().material.color = Color.white;
                        TargetObj1 = TargetObj2;
                        TargetObj2 = hit.collider.gameObject;
                        TargetObj2.GetComponent<Renderer>().material.color = Color.red;
                    }
                }
            }

        }
        
    }
    //UI Toggle이 사용 하는 함수
    public void ChageBool()  // 물건을 꺼낸뒤에 saveBOx에 물건을 유지 할 것인지를 조절하는 originMaintain의 checkBox 
    {
        if (checkKeepSave)
            checkKeepSave = false;
        else
            checkKeepSave = true;
    }

}
