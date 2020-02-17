using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineTracer : MonoBehaviour
{
    public GameObject m_LinePrefab;
    public GameObject m_CurrentLine;
    public LineRenderer m_LineRenderer;
    public List<Vector2> m_FingerPositions;

    private void CreateLine()
    {
        m_CurrentLine = Instantiate(m_LinePrefab, Vector3.zero, Quaternion.identity);
        m_LineRenderer = m_CurrentLine.GetComponent<LineRenderer>();
        m_FingerPositions.Clear();
        m_FingerPositions.Add(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        m_FingerPositions.Add(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        m_LineRenderer.SetPosition(0, m_FingerPositions[0]);
        m_LineRenderer.SetPosition(1, m_FingerPositions[1]);
    }

    private void UpdateLine(Vector2 newFingerPos)
    {
        m_FingerPositions.Add(newFingerPos);
        m_LineRenderer.positionCount++;
        m_LineRenderer.SetPosition(m_LineRenderer.positionCount - 1, newFingerPos);
    }
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CreateLine();
        }

        if (Input.GetMouseButton(0))
        {
            Vector2 tempFingerPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (Vector2.Distance(tempFingerPos, m_FingerPositions[m_FingerPositions.Count -1]) > 0.1f)
            {
                UpdateLine(tempFingerPos);
            }
        }
    }
}