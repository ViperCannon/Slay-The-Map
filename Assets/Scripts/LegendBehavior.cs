using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Map;

public class LegendBehavior : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public NodeType type;

    public List<NodeButtonFunction> buttons = new();

    public void Populate()
    {
        buttons.Clear();

        StartCoroutine(FillButtons());
    }

    public void Clear()
    {
        buttons.Clear();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponent<Outline>().enabled = true;

        foreach(NodeButtonFunction n in buttons)
        {
            n.pulsePaused = true;
            n.nodeButton.transform.localScale = Vector3.one;
            n.GetComponentInChildren<Outline>().enabled = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponent<Outline>().enabled = false;

        foreach (NodeButtonFunction n in buttons)
        {
            n.pulsePaused = false;
            n.GetComponentInChildren<Outline>().enabled = false;
        }
    }

    private IEnumerator FillButtons()
    {
        yield return new WaitForFixedUpdate();

        foreach (GameObject node in GameObject.FindGameObjectsWithTag(type.ToString()))
        {
            if (node != null && node.GetComponent<NodeButtonFunction>() != null)
            {
                buttons.Add(node.GetComponent<NodeButtonFunction>());
            }
        }
    }
}
