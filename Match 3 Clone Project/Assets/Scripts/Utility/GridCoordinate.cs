using UnityEngine;

/// <summary>
/// Helper class to keep coordinates of board objects.
/// </summary>
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

    public Vector2Int pos
    {
        get
        {
            return new Vector2Int(m_X, m_Y);
        }
        set
        {
            m_X = value.x;
            m_Y = value.y;
        }
    }
}

