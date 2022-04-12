using UnityEngine;
using B83.MeshTools;
using System.Text;

/// <summary>
/// 키생성과 암호화를 동시에 해주는 클래스
/// 복호화도 여기서 진행 
/// 이렇게 나누어 한 이유는 CDN에서 전체적으로 다진행하다 보니 
/// EncDec.Decrypt[복호화]0 에서 byte끝자리를 알수없는 Padding
/// 오류가 지속적으로 발생하여서 padding오류 해결 하기위해서 CDN전체에서 진행하는게 아니라
/// 기능을 분리해서 하였습니다.
/// 키 값과 iv값의 길이는 16입니다.
/// 키값생성 방식은 랜덤 숫자8개 +년도(4) +몇초(2) +몇분(2)
/// /// </summary>
public class Encrpyt : MonoBehaviour
{
    public static byte[] CreateKeyAndEncrypt(Mesh enMesh,byte[] key, byte[] iv)
    {
        //MeshSerializer 메쉬 바이너리화
        byte[] encrypted = MeshSerializer.SerializeMesh(enMesh);
        byte[] newby = EncDec.Encrypt(encrypted, key, iv);
        
        return newby;
    }

    public static Mesh Decrypt(byte[] deMesh, byte[] key, byte[] iv)
    {
        byte[] newby = EncDec.Decrypt(deMesh, key, iv);
        
        Mesh mes = MeshSerializer.DeserializeMesh(newby);
        return mes;
    }

    //key값 만들기
    static public byte[] GetKey()
    {
        string secondStr = System.DateTime.Now.Second.ToString();
        string minuteStr = System.DateTime.Now.Minute.ToString();
        string monthStr = System.DateTime.Now.Month.ToString();

        if (secondStr.Length == 1)
        {
            secondStr = secondStr + "0";
        }
        if (minuteStr.Length == 1)
        {
            minuteStr = minuteStr + "0";
        }
        if (monthStr.Length == 1)
        {
            monthStr = monthStr + "0";
        }

        int num = 0;
        for (int i = 0; i < 8; i++)
        {
            num *= 10;
            num += Random.Range(1, 9);
        }
       
        string ivStr = num.ToString()+ System.DateTime.Now.Year.ToString() + monthStr + secondStr;
        byte[] GetKey = Encoding.UTF8.GetBytes(ivStr);
        return GetKey;
    }

    //Iv값 만들기
    static public byte[] GetIv()
    {
        string secondStr = System.DateTime.Now.Second.ToString();
        string minuteStr = System.DateTime.Now.Minute.ToString();

        if (secondStr.Length == 1)
        {
            secondStr = secondStr + "0";
        }
        if (minuteStr.Length == 1)
        {
            minuteStr = minuteStr + "0";
        }
        int num = 0;
        for (int i = 0; i < 8; i++)
        {
            num *= 10;
            num += Random.Range(1, 9);
        }
        
        string ivStr = num
            + System.DateTime.Now.Year.ToString() + minuteStr + secondStr;
        byte[] iv = Encoding.UTF8.GetBytes(ivStr);
        
        return iv;
    }
}
