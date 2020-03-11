using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.UIElements.Runtime;
using UnityEngine.UIElements.Experimental;
using Random = UnityEngine.Random;

public class UIManager : MonoBehaviour
{
    public int m_NumberOfNodes;
    public int m_EndAnimationDuration;
    
    private int m_CurrentNode;
    private List<VisualElement> m_Nodes = new List<VisualElement>();
    private VisualElement m_Root;

    private void Start()
    {
        m_Root = GetComponent<PanelRenderer>().visualTree;
        PopulateNodesInScene(m_NumberOfNodes);
    }

    private void Update()
    {
        if (!Input.GetMouseButton(0) && m_CurrentNode != 0)
            ResetGameState();
    }

    private void PopulateNodesInScene(int nodesNumber)
    {
        for (var i = 0; i < nodesNumber; i++)
        {
            var ve = new VisualElement();
            ve.AddToClassList("pattern-node");
            ve.style.left = Random.Range(0, Screen.width/2);
            ve.style.top = Random.Range(0, Screen.height/2);

            var label = new Label(i.ToString());
            label.AddToClassList("pattern-node-number");
            ve.Add(label);
            
            SetCallbacksOnNode(ve);
            m_Nodes.Add(ve);
            m_Root.Add(ve);
        }
    }

    private void CheckForOrder(VisualElement node)
    {
        if (!Input.GetMouseButton(0))
            return;
        
        if (m_Nodes.IndexOf(node) == m_CurrentNode)
        {
            node.style.backgroundColor = Color.blue;
            m_CurrentNode++;
            UnsetCallbacksOnNode(node);

            if (m_CurrentNode == m_Nodes.Count)
                GameOver();
        }
        else
        {
            Debug.Log("Wrong order!");
        }
    }

    private void ResetGameState()
    {
        foreach (var ve in m_Nodes)
        {
            ve.style.backgroundColor = Color.red;
            UnsetCallbacksOnNode(ve);
            SetCallbacksOnNode(ve);
        }

        var lines = GameObject.FindGameObjectsWithTag("PatternLine");
        foreach (var line in lines)
        {
            Destroy(line);
        }
        
        m_CurrentNode = 0;
    }

    private void GameOver()
    {
        foreach (var node in m_Nodes)
        {
            var nodeLeftPos = node.resolvedStyle.left;
            
            node.experimental.animation
                .Start(new StyleValues {opacity = 1, left = nodeLeftPos}, new StyleValues {opacity = 0, left = nodeLeftPos + 400}, m_EndAnimationDuration)
                .Ease(Easing.OutCubic);
        }
    }

    private void OnMouseEnter(MouseEnterEvent e)
    {
        var ve = e.target as VisualElement;
        CheckForOrder(ve);
    }
    
    private void SetCallbacksOnNode(VisualElement node)
    {
        node.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
    }

    private void UnsetCallbacksOnNode(VisualElement node)
    {
        node.UnregisterCallback<MouseEnterEvent>(OnMouseEnter);
    }
}