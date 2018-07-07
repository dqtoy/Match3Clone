using UnityEngine;

[System.Serializable]
public struct GridCoordinate
{
    [SerializeField]
    int m_X;
    [SerializeField]
    int m_Y;

    public int x
    {
        get
        {
            return m_X;
        }
        set
        {
            m_X = value;
        }
    }
    public int y
    {
        get
        {
            return m_Y;
        }
        set
        {
            m_Y = value;
        }
    }

    public Vector2 pos
    {
        get
        {
            return new Vector2(m_X, m_Y);
        }
        set
        {
            m_X = (int)value.x;
            m_Y = (int)value.y;
        }
    }
}

