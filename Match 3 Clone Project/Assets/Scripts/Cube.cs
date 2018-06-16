using UnityEngine;

public class Cube : MonoBehaviour 
{

    public enum Color 
    {
        Blue, Green, Red, Yellow
    }

    [SerializeField]
    Color m_color;

    public Color CubeColor 
    { 
        get
        {
            return m_color;
        } 
    }

	void Start() 
	{
		
	}
	
	void Update() 
	{
		
	}
}
