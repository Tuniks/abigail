using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drawing : MonoBehaviour
{
    public Camera m_camera;
    public GameObject brush;
    public GameObject drawingSurface; // The GameObject you want to limit drawing to

    private LineRenderer currentLineRender;
    private Vector2 lastPos;

    private void Update()
    {
        Draw();
    }

    void Draw()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (IsMouseOverSurface())
            {
                CreateBrush();
            }
        }

        if (Input.GetKey(KeyCode.Mouse0) && currentLineRender != null)
        {
            Vector2 mousePos = m_camera.ScreenToWorldPoint(Input.mousePosition);
            if (mousePos != lastPos && IsMouseOverSurface())
            {
                AddPoint(mousePos);
                lastPos = mousePos;
            }
        }
        else
        {
            currentLineRender = null;
        }
    }

    void CreateBrush()
    {
        GameObject brushInstance = Instantiate(brush);
        currentLineRender = brushInstance.GetComponent<LineRenderer>();

        // Set sorting layer and order in layer
        //currentLineRender.sortingLayerName = "Drawings";  
        //currentLineRender.sortingOrder = 5;  

        Vector2 mousePos = m_camera.ScreenToWorldPoint(Input.mousePosition);
        
        currentLineRender.SetPosition(0, mousePos);
        currentLineRender.SetPosition(1, mousePos);
    }

    void AddPoint(Vector2 pointPos)
    {
        currentLineRender.positionCount++;
        int positionIndex = currentLineRender.positionCount - 1;
        currentLineRender.SetPosition(positionIndex, pointPos);
    }

    bool IsMouseOverSurface()
    {
        Ray ray = m_camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Check if the hit object is the drawing surface
            return hit.collider.gameObject == drawingSurface;
        }
        return false;
    }
}
