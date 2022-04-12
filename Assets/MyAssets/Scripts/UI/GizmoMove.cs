using UnityEngine;
/// <summary>
/// 기즈모로 객체의 이동을 담당하는 부분
/// </summary>


public class GizmoMove : MonoBehaviour
{
    public int HowToMove; //x y z 축 다르게 인스펙터창에서 설정이 되어있습니다.
    private Vector3 mOffset; 
    private float mZCoord;

    //처음 객체의 위치를 ve2로 할당해서 할당한 객체 위치를 ve2에 저장한다음
    //처음 객체의 위치와 마우스의 getaxis위치를(누른 다음에 움직인 정도를 표현하는) 를 사용해서
    //dot 사용해서 특정 방향으로 더한다
    void OnMouseDrag()
    {
        Vector2 ve2 = CameraControls.originPos;
        switch (HowToMove)      
        {
            case 0:
                Vector2 XAxis = (new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")));
                float deltaX = Vector2.Dot(XAxis, ve2);
                mOffset = Vector3.left * deltaX * 0.3f;
                CameraControls.selectionObj.transform.position += mOffset;
                transform.root.position = (CameraControls.selectionObj.transform.position);
                break;
            case 1:
                Vector2 YAxis = (new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")));
                float deltaY = Vector2.Dot(YAxis, ve2);
                mOffset = Vector3.up * deltaY * 0.3f;
                CameraControls.selectionObj.transform.position += mOffset;
                transform.root.position = (CameraControls.selectionObj.transform.position);
                break;
            case 2:
                Vector2 ZAxis = (new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")));
                float deltaZ = Vector2.Dot(ZAxis, ve2);
                mOffset = Vector3.back * deltaZ * 0.3f;
                CameraControls.selectionObj.transform.position += mOffset;
                transform.root.position = (CameraControls.selectionObj.transform.position);
                break;
        }

    }
}
