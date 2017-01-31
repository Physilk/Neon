using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUICrosshair : MonoBehaviour
{
    public float crosshairScale = 1.0f;
    public Texture2D m_CrosshairTex;
    Vector2 m_WindowSize;    //More like "last known window size".
    Rect m_CrosshairRect;

    void Start()
    {
        //m_CrosshairTex = new Texture2D(2, 2);
        m_WindowSize = new Vector2(Screen.width, Screen.height);
        CalculateRect();
    }

    void Update()
    {
        if (m_WindowSize.x != Screen.width || m_WindowSize.y != Screen.height)
        {
            CalculateRect();
        }
    }

    void CalculateRect()
    {
        //new Rect((m_WindowSize.x - m_CrosshairRect.width * crosshairScale) / 2, (m_WindowSize.y - m_CrosshairRect.height * crosshairScale) / 2, m_CrosshairRect.width * crosshairScale, m_CrosshairRect.height * crosshairScale)
        m_CrosshairRect = new Rect((m_WindowSize.x - m_CrosshairTex.width * crosshairScale) / 2.0f,
                                    (m_WindowSize.y - m_CrosshairTex.height * crosshairScale) / 2.0f,
                                    m_CrosshairTex.width * crosshairScale, m_CrosshairTex.height * crosshairScale);
    }

    void OnGUI()
    {
        GUI.DrawTexture(m_CrosshairRect, m_CrosshairTex);
    }
}