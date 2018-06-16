using UnityEngine;

public class BoardController : MonoBehaviour 
{

	void Start() 
	{
		
	}
	
	void Update() 
	{
        if(Input.GetMouseButtonDown(0)){
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            bool gotHit = Physics.Raycast(ray, out hit, 100);

            if(gotHit)
            {
                Cube cubeHit = hit.collider.GetComponent<Cube>();
                Debug.Log(cubeHit.CubeColor);
                Destroy(cubeHit.gameObject);
            }
        }
	}
}
