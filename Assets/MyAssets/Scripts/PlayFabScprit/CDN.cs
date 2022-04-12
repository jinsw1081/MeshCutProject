using System;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.AdminModels;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using System.Collections;
using System.Net;
using UnityEngine.UI;
using System.Text;


public class CDN : MonoBehaviour
{  
/// <summary>
/// CDN은 객체를 암호화 하는 함수 실행하고 암호화된 파일 자체와 암호화할 때 저장한 key 값과 iv(초기화벡터)값을
/// 암호화된 파일을 CDN에 업로드하며 키 값은 TitleData에 IV값은 Title Internal data 보관합니다.
/// 이러한 방법으로 파일을 세이브하고 그 세이브한 파일을 
/// 이 반대 방법으로 파일을 다운로드 합니다.
/// 각각을 홈페이지에서 확인 할수 있습니다.
/// </summary>

    public InputField inputField;   //세이브 로드 할때 입력을 받는 창
    public Image messagebox;        //세이브를 성공 했을때 나타나는 이미지박스
    public GameObject BasicPrefab;  //오브젝트 다운로드 받았을때 새롭게만드는 기본유형 프리팹
   
    string Objname; //모델 이름

    bool boolSetname = false;   //이름이 입력이 됐는지 확인하기위한 bool변수
    
    //다운로드 호출 부분
    public void Download()
    {

        // 빨간색 오브젝트임.. 
        if (!CameraControls.TargetObj1 && CameraControls.TargetObj2)
        {
            CameraControls.TargetObj1 = CameraControls.TargetObj2;
            CameraControls.TargetObj2 = null;
        }
        if (!string.IsNullOrEmpty(Objname))
        {
            DownloadFileFromCDN(Objname);
        }
    }
    
    //업로드 호출 부분 
    public void Upload()
    {
        Objname = inputField.text;
        if (!string.IsNullOrEmpty(Objname) &&CameraControls.TargetObj1)
        {
            //키 값의 의미
            // 암호화 자료를 생성할 때 필요한 키 값임.
            // 업로드 할때 생성해서 서버에 보내고 
            // 받을 때도 서버에서 받게되어 있음. 
            Byte[] mIv = Encrpyt.GetIv();
            Byte[] mKey = Encrpyt.GetKey();
            
            Mesh me = CameraControls.TargetObj1.GetComponent<MeshFilter>().mesh;

            byte[] Upencrypted = Encrpyt.CreateKeyAndEncrypt(me, mKey, mIv);

            //저번에 노트북 와이파이에서 오류가 났었던 부분인데 문제가 있다면
            //지금은 연속으로 타이틀 데이터랑 이터널 타이틀  데이터를 저장을 하는데
            //타이틀 데이터 저장하는 도중에 이터널 타이틀 데이터를 저장하니 저장하던
            //http 요청 하는 도중에 다른요청을 또해버리니
            //데이터가 손실이나 생략 될수도 있습니다. 
            //수정 해야할 부분 서로 분리 하거나 기다리는 방식으로 
            PlayFabServerAPI.SetTitleData(new PlayFab.ServerModels.SetTitleDataRequest
            {
                Key = Objname,
                Value = Encoding.UTF8.GetString(mKey)
                  
            },
              Onsuccess => { Debug.Log(Encoding.UTF8.GetString(mKey)); },
              Onfail => { Onfail.GenerateErrorReport(); }
            );

            PlayFabServerAPI.SetTitleInternalData(new PlayFab.ServerModels.SetTitleDataRequest
            {
                Key = Objname,
                Value = Encoding.UTF8.GetString(mIv)
            },
              Onsuccess => { Debug.Log(Encoding.UTF8.GetString(mIv)); },
              Onfail => { Onfail.GenerateErrorReport(); }
            );

            UploadFileToCDN(Objname, Upencrypted);
            inputField.gameObject.SetActive(false);
        }
    }   
    //업로드 버튼 화면에 나타나게하는 함수
    public void UploadButtonSet()
    {
        if (boolSetname)
        {
            inputField.gameObject.SetActive(false);
            boolSetname = false;
        }
        else
        {
            inputField.gameObject.SetActive(true);
            inputField.transform.Find("SaveBut").gameObject.SetActive(true);
            inputField.transform.Find("LoadBut").gameObject.SetActive(false);
            boolSetname = true;
        }
    }

    //에러 발생시 에러 로그 보여주는 함수 람다식을 사용하기 전에 사용하던것
    //private void OnPlayFabError(PlayFabError error)
    //{
    //    Debug.LogError(error.GenerateErrorReport());
    //}
    ////성공 로그 보여주는 함수
    //void OnUploadSuccess(PlayFab.DataModels.FinalizeFileUploadsResponse result)
    //{
    //    Debug.Log("File upload success: ");
    //}

    //void OnSharedFailure(PlayFabError error)
    //{
    //    Debug.LogError(error.GenerateErrorReport());
    //}

    public void UploadFileToCDN(string key, byte[] content, string contentType = "binary/octet-stream")
    {
        GetUploadUrl(key, contentType, presignedUrl =>
        {
           PutFile(presignedUrl, content);
        });
    }

    void GetUploadUrl(string key, string contentType, Action<string> onComplete)
    {
        PlayFabAdminAPI.GetContentUploadUrl(new GetContentUploadUrlRequest()
        {
            ContentType = contentType,
            Key = key
        }, result => onComplete(result.URL),
        error => Debug.LogError(error.GenerateErrorReport()));
    }

    public void PutFile(string putURL, byte[] payload)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(putURL);
        request.Method = "PUT";
        request.ContentType = "binary/octet-stream";

        if (payload != null)
        {
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(payload, 0, payload.Length);
            dataStream.Close();
        }
        else
        {
            Debug.LogWarning(string.Format("ERROR: Byte arrry was empty or null"));
            return;
        }

        Debug.Log("Starting HTTP PUT...");  
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();

        if (response.StatusCode == HttpStatusCode.OK)
        {
            Debug.Log("...HTTP PUT Successful");
            messagebox.gameObject.SetActive(true);
            Invoke("deActiveMessageBox", 2);
        }
        else
        {
            Debug.LogWarning(string.Format("ERROR: [{0}] -- {1}", response.StatusCode, response.StatusDescription));
        }
    }

    public void DownloadFileFromCDN(string key)
    {
        GetDownloadUrl(key, presignedUrl =>
        {
            GetFile(presignedUrl);
        });
    }

    void GetDownloadUrl(string key, Action<string> onComplete)
    {
        if(CameraControls.TargetObj1)
            CameraControls.TargetObj1.name = key;
        PlayFabClientAPI.GetContentDownloadUrl(new GetContentDownloadUrlRequest()
        {
            Key = key,
            ThruCDN = true 
        }, result => onComplete(result.URL),
        error => Debug.LogError(error.GenerateErrorReport()));
    }
    
    void GetFile(string preauthorizedUrl)
    {
        StartCoroutine(FileRecevier(preauthorizedUrl));
    }
    
    IEnumerator FileRecevier(string preauthorizedUrl)
    {
        string keyVale = Objname;
        string ivVale = Objname;
        byte[] thisiv=new byte[0];
        byte[] thiskey = new byte[0];
        UnityWebRequest www = UnityWebRequest.Get(preauthorizedUrl);
        // 키값이 로그에 찍히게 되어 있음. 
        // 
         PlayFabServerAPI.GetTitleData(new PlayFab.ServerModels.GetTitleDataRequest(),
         result => {
           if (result.Data == null || !result.Data.ContainsKey(keyVale)) Debug.Log("No key");
           else Debug.Log("KeyValue: " + result.Data[keyVale]);
           byte[] newKey =  Encoding.UTF8.GetBytes(result.Data[keyVale]);
             thiskey = new byte[newKey.Length];
             Array.Copy(newKey, thiskey, newKey.Length);
         },                   //키값을 출력하는 방법이 result.data[keyname]을 
          error => {
               Debug.Log("Got error getting titleData:");
               Debug.Log(error.GenerateErrorReport());

         }
        );
        // prefab 홈페이지에서 title data 참조 
        PlayFabServerAPI.GetTitleInternalData(new PlayFab.ServerModels.GetTitleDataRequest(),
        result => {
            if (result.Data == null || !result.Data.ContainsKey(ivVale)) Debug.Log("No value");
            else Debug.Log("ivVale: " + result.Data[ivVale]);
            byte[] newIv = Encoding.UTF8.GetBytes(result.Data[ivVale]);
            thisiv = new byte[newIv.Length];
            Array.Copy(newIv, thisiv, newIv.Length);
        },
        error => {
            Debug.Log("Got error getting titleData:");
            Debug.Log(error.GenerateErrorReport());
        }
    );
        yield return www.SendWebRequest();
        yield return new WaitForSeconds(3.0f);

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("SucessfulDownload");
            byte[] results = www.downloadHandler.data;
            //파일 변환 부분
            // 객체화 하는 부분 ... Scene에 등장 시키는 부분
            // Gameobject로 캐스팅이 된다. 
            // BasicPrefab(프로젝트 창에 있음)을 등장시켜 놓고 
            //  메시를  바꿔치기한다. 
            GameObject downObj = Instantiate<GameObject>(BasicPrefab);
            // results가 다운로드 받은 데이터 임.. 
            Mesh me = Encrpyt.Decrypt(results, thiskey, thisiv);
            // 다운로드 받은 객체를 덮어 씌운다. 
            downObj.GetComponent<MeshFilter>().mesh = me;
            downObj.AddComponent<MeshCollider>();

        }
    }
    // 모델 다운로드 할때 타나타는 창 "Inputfield" 보여지는 텍스트를 바꾸는 과정이다. 
    public void SetNamefun(string name)
    {
        Objname = name;
    }
   
    // 다운로드 성공하면 메세지 박스가 나타나는데 그걸 없애는 기능이다. 
    void deActiveMessageBox()
    {
        messagebox.gameObject.SetActive(false);
    }
}
