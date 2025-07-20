using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Map;

public class NodeButtonFunction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    GameObject mapGenerator;

    public Button nodeButton;

    Node node;

    float currentRatio = 1;
    float growthSpeed = 0.5f;
    readonly float upperBound = 1.5f;
    readonly float lowerBound = 0.95f;
    
    public bool pulsePaused = false;

    void Start()
    {
        mapGenerator = GameObject.FindWithTag("Map");
    }

    public void Click()
    {
        mapGenerator.GetComponent<MapGenerator>().SelectNode(this.gameObject);
    }

    public void StartPulse()
    {
        StartCoroutine(Pulse());
    }

    IEnumerator Pulse()
    {
        currentRatio = Random.Range(lowerBound, upperBound);

        nodeButton.transform.localScale = new Vector3(currentRatio, currentRatio, currentRatio);

        while (nodeButton.enabled && nodeButton.interactable)
        {
            if (!(nodeButton.enabled && nodeButton.interactable))
            {
                break;
            }

            while (!(currentRatio >= upperBound))
            {
                if (!(nodeButton.enabled && nodeButton.interactable))
                {
                    break;
                }

                currentRatio = Mathf.MoveTowards(currentRatio, upperBound, growthSpeed * Time.deltaTime);

                if (!pulsePaused)
                {
                    nodeButton.transform.localScale = Vector3.one * currentRatio;
                }
                
                yield return null;
            }

            while (!(currentRatio <= lowerBound))
            {
                if (!(nodeButton.enabled && nodeButton.interactable))
                {
                    break;
                }

                currentRatio = Mathf.MoveTowards(currentRatio, lowerBound, growthSpeed * Time.deltaTime);

                if (!pulsePaused)
                {
                    nodeButton.transform.localScale = Vector3.one * currentRatio;
                }

                yield return null;
            }

            yield return null;
        }

        nodeButton.transform.localScale = Vector3.one;
        currentRatio = 1;

        yield return null;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!(node.GetNodeType() == NodeType.Boss))
        {
            pulsePaused = true;
            nodeButton.transform.localScale = Vector3.one * 1.8f;
        }

        if (nodeButton.enabled && nodeButton.interactable)
        {
            GetComponentInChildren<Outline>().enabled = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!(node.GetNodeType() == NodeType.Boss))
        {
            pulsePaused = false;
            nodeButton.transform.localScale = Vector3.one;
        }

        if (nodeButton.enabled && nodeButton.interactable)
        {
            GetComponentInChildren<Outline>().enabled = false;
        }
    }

    public void SetNode(Node n)
    {
        node = n;
    }
}