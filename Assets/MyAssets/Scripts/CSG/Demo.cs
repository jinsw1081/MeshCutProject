using UnityEngine;
using Parabox.CSG;
using UnityEngine.Rendering;

/// <summary>
/// CSG를 사용하는 클래스 
/// </summary>
public class Demo : MonoBehaviour
{
	GameObject left, right; //적용할 두객체
//	bool wireframe = false; 

	public Material wireframeMaterial;  //와이어프레임 Mat 
    GameObject composite;   //합쳐진 두객체

	enum BoolOp
	{
		Union,
		SubtractLR,
		SubtractRL,
		Intersect
	};

    //void Awake()
    //{
    //	wireframeMaterial.SetFloat("_Opacity", 0);
    //	cur_alpha = 0f;
    //	dest_alpha = 0f;

    //  m= new Mesh();
    //  m.indexFormat = IndexFormat.UInt32;
    //  ToggleWireframe();
    //}

    public void Reset()
    {
        left = CameraControls.TargetObj1;
        right = CameraControls.TargetObj2;
        if (left && right)
        {
            GenerateBarycentric(left);
            GenerateBarycentric(right);
        }
    }

    //유니온은 버텍스가 너무 많이 필요해서 오류가 날확률이 많아서
    //public void Union()
	//{
	//	Reset();
	//	Boolean( BoolOp.Union );
	//}

	public void SubtractionLR()
	{
		Reset();
		Boolean( BoolOp.SubtractLR );
	}

    public void Intersection()
	{
		Reset();
		Boolean( BoolOp.Intersect );
	}
    
    void Boolean(BoolOp operation)
    {
        if (left && right)
        {
            //둘다 위치가 원점에 있지 않으니 원점으로 이동한 다음에 boolean operation 실행
            //이렇게 하지않으면 메쉬는 그위치 그대로 있는데 메쉬 중점이 원점에 있는 경우가 발생함

            Vector3 leftVe3 = left.transform.position;
            Vector3 rigthVe3 = right.transform.position;
            Vector3 interval = leftVe3-  rigthVe3;
            left.transform.position = Vector3.zero;
            right.transform.position = Vector3.zero - interval;
            Mesh m;
            /**
             * All boolean operations accept two gameobjects and return a new mesh.
             * Order matters - left, right vs. right, left will yield different
             * results in some cases.
             */
             //CSG를 사용해서 새롭게 Mesh를 만드는 부분
            switch (operation)
            {
                case BoolOp.Union:
                    m = CSG.Union(left, right);
                    break;

                case BoolOp.SubtractLR:
                    if(left.tag=="others")
                    m = CSG.Subtract(right, left);
                    else
                    m = CSG.Subtract(left, right);
                    break;

                case BoolOp.Intersect:
                default:
                    m = CSG.Intersect(right, left);
                    break;
            }
            //새롭게 객체 만들어서 만든 Mesh 컴퍼넌트를 교체하는 부분
            composite = new GameObject();
            composite.name = "new object";
            composite.AddComponent<MeshFilter>().sharedMesh = m;
            composite.AddComponent<MeshRenderer>().sharedMaterial = left.GetComponent<MeshRenderer>().sharedMaterial;
            composite.AddComponent<MeshCollider>();
            composite.GetComponent<Renderer>().material.color = Color.white;
            composite.AddComponent<Objects>();
            composite.layer = 9;

            left.transform.position = leftVe3;
            right.transform.position = rigthVe3;

            GenerateBarycentric(composite);
            Vector3 screenCenter=Camera.main.ScreenToWorldPoint(
                new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2));

            composite.transform.position = screenCenter;
            composite.transform.position +=new Vector3(1,0,1);//간격조절
            
            //composite.GetComponent<Renderer>().material = wireframeMaterial;
        }
    }

    //와이어 프레임 관련 부분
    /**
	 * Turn the wireframe overlay on or off.
	 */
    //public void ToggleWireframe()
    //{
    //	wireframe = !wireframe;

    //	cur_alpha = wireframe ? 0f : 1f;
    //	dest_alpha = wireframe ? 1f : 0f;
    //	start_time = Time.time;
    //}

    /**
	 * Swap the current example meshes
	 */

    //float wireframe_alpha = 0f, cur_alpha = 0f, dest_alpha = 1f, start_time = 0f;

    //void Update()
    //{
    //	wireframe_alpha = Mathf.Lerp(cur_alpha, dest_alpha, Time.time - start_time);
    //	wireframeMaterial.SetFloat("_Opacity", wireframe_alpha);
    //}

    /**
	 * Rebuild mesh with individual triangles, adding barycentric coordinates
	 * in the colors channel.  Not the most ideal wireframe implementation,
	 * but it works and didn't take an inordinate amount of time :)
	 */
    //와이어 프레임 관련 부분

    //메쉬 합치지 전에 중심축생성  
    void GenerateBarycentric(GameObject go)
	{
        if (go.GetComponent<MeshFilter>())
            go.GetComponent<MeshFilter>().sharedMesh =
                go.GetComponent<MeshFilter>().mesh;

        Mesh m = go.GetComponent<MeshFilter>().sharedMesh;
        
        if (m == null) return;

		int[] tris = m.triangles;
		int triangleCount = tris.Length;

		Vector3[] mesh_vertices		= m.vertices;
		Vector3[] mesh_normals		= m.normals;
		Vector2[] mesh_uv			= m.uv;

		Vector3[] vertices 	= new Vector3[triangleCount];
		Vector3[] normals 	= new Vector3[triangleCount];
		Vector2[] uv 		= new Vector2[triangleCount];
		Color[] colors 		= new Color[triangleCount];

		for(int i = 0; i < triangleCount; i++)
		{
			vertices[i] = mesh_vertices[tris[i]];
			normals[i] 	= mesh_normals[tris[i]];
			uv[i] 		= mesh_uv[tris[i]];

			colors[i] = i % 3 == 0 ? new Color(1, 0, 0, 0) : (i % 3) == 1 ? 
                new Color(0, 1, 0, 0) : new Color(0, 0, 1, 0);

			tris[i] = i;
		}

		//Mesh wireframeMesh = new Mesh();
  //      wireframeMesh.indexFormat= IndexFormat.UInt32;
  //      wireframeMesh.Clear();
		//wireframeMesh.vertices = vertices;
		//wireframeMesh.triangles = tris;
		//wireframeMesh.normals = normals;
		//wireframeMesh.colors = colors;
		//wireframeMesh.uv = uv;

	//	go.GetComponent<MeshFilter>().sharedMesh = wireframeMesh;
	}

}
