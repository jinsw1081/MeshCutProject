using PlayFab;
using PlayFab.AdminModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 서버에서 콘텐츠를 받아와서 출력하는 클래스
/// 최대로 받아올수 있는 량을 정할수 있다. (지금은 20)
/// </summary>

public class ScrollView : MonoBehaviour
{
    public ScrollView scrollView;   //스크롤뷰
    public Button newbutton;        //새롭게 만들 버튼
    public InputField input;        //세이브 로드 할때 사용 하는 inputField
    List<ContentInfo> contentInfos=new List<ContentInfo>(); //다운받은 컨텐츠 리스트 저장하기
    bool OnActive = false;          //스크롤뷰가 활성화 bool값
    Transform Content;              //하위객체를 모두 받아오기 위해서 만든 변수

    void Start()
    {
        //시작 할때 버튼을 미리 만들고 만든 버튼을 비활성화 해두고 배치 해두는 부분
        Content = scrollView.transform.GetChild(0).GetChild(0);
        for (int i = 1; i < 20; i++)        //20개생성
        {
            RectTransform recttr = Content.GetChild(0).GetComponent<RectTransform>();

            Vector3 vector3 = recttr.localPosition;
            Button instancBut = Instantiate<Button>(newbutton);
            instancBut.transform.SetParent(Content, false);
            
            instancBut.transform.localScale = Vector3.one;
            vector3.y -= 40*i;
            instancBut.transform.localPosition = vector3;
        }
        for (int i = 1; i < Content.childCount; i++)
        {
            Content.GetChild(i).gameObject.SetActive(false);
        }
        scrollView.gameObject.SetActive(false);
    }
    //활성화 하는 함수
    public void DownloadButtonSet()
    {
        if (!OnActive)
        {
            input.transform.Find("SaveBut").gameObject.SetActive(false);
            input.transform.Find("LoadBut").gameObject.SetActive(true);
            scrollView.gameObject.SetActive(true);
            input.gameObject.SetActive(true);
            StartCoroutine(enumerator(2f));
            OnActive = true;
        }
        else
        {
            scrollView.gameObject.SetActive(false);
            input.gameObject.SetActive(false);
            OnActive = false;
            for (int i = 1; i < Content.childCount; i++)
            {
                Content.GetChild(i).gameObject.SetActive(false);
            }

        }
    }

    //Playfab에서 GetContentList로 컨텐츠 리스트 받오는 부분
    IEnumerator enumerator(float time)
    {
        Dictionary<string, string> keyValues = new Dictionary<string, string>();
        GetContentListRequest getContentListRequest = new GetContentListRequest { Prefix = "" };
        
        PlayFabAdminAPI.GetContentList(getContentListRequest, result =>
        {
            contentInfos = result.Contents;
        },
            error => { Debug.LogError(error.GenerateErrorReport()); }, keyValues);
        yield return new WaitForSeconds(time);

        for (int i = 0; i < contentInfos.Count; i++)
        {
            Content.GetChild(i + 1).gameObject.SetActive(true); 
            Content.GetChild(i+1).GetChild(0).GetComponent<Text>().text = contentInfos[i].Key;
        }

    }
    
}
