using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 기즈모를 통해서 화면을 회전하는 클래스
/// 버튼인 resetPosition 포함되어있습니다.
/// </summary>

public class GizmoRotate : MonoBehaviour{
   
    public GameObject gimzo;// 이동기즈모
    public enum set
    {
        down,up,left,right,back,
        count
    }
    public int StringConverttoInt(string name)
    {
        int num = 7;
        for (set i =0; i <set.count; i++)
        {
           if(i.ToString()==name)
            {
                num = (int)i;
            }
        }
        if (num != 7)
            return num;
        else
            return 7;
    }
    //포지션 리셋
    public void OnclickReset()
    {
        if(CameraControls.TargetObj1)
            CameraControls.TargetObj1.transform.position = Vector3.zero;
        else if(CameraControls.TargetObj2)
            CameraControls.TargetObj2.transform.position = Vector3.zero;
    }

    //원점 또는 객체를 기준으로 두고 기준으로 둔 방향에서 
    //한바퀴 또는 90 회전 하는 함수
    public void SetRotateCamera(int set)
    {
        Vector3 rotateOrigin = Vector3.zero;
        if(CameraControls.TargetObj1)
        {
            rotateOrigin = CameraControls.TargetObj1.transform.position;
        }
        //   left = 0, right = 1, up = 2, down = 3, back = 4

        switch (set)
        {
            case 0: //left
                Camera.main.transform.RotateAround(rotateOrigin, Vector3.left, 90f);
                //gimzo.transform.RotateAround(gimzo.transform.position,
                //    Vector3.left, 90f);
                break;
            case 1: //right
                Camera.main.transform.RotateAround(rotateOrigin, Vector3.right, 90f);
         //       gimzo.transform.RotateAround(gimzo.transform.position,Vector3.right, 90f);
                break;
            case 2://down
                Camera.main.transform.RotateAround(rotateOrigin, Vector3.up, 90f);
         //       gimzo.transform.RotateAround(gimzo.transform.position,Vector3.up, 90f);
                break;
            case 3://up
                Camera.main.transform.RotateAround(rotateOrigin, Vector3.down, 90f);
          //      gimzo.transform.RotateAround(gimzo.transform.position,Vector3.down, 90f);
                break;
            case 4://back
                Camera.main.transform.RotateAround(rotateOrigin, Vector3.right, 180f);
           //     gimzo.transform.RotateAround(gimzo.transform.position,Vector3.right, 180f);
                break;
          
        }
    
    }
}
