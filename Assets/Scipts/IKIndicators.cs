using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKIndicators : MonoBehaviour
{
    public IKSnap ikSnap; // Reference to the IKSnap script

    public GameObject indicatorPrefab; // Prefab for the indicator spheres
    private List<GameObject> indicators = new List<GameObject>();

    void Start()
    {
        if (indicatorPrefab == null)
        {
            Debug.LogError("Indicator Prefab is not set in IKIndicators script.");
            return;
        }

        if (ikSnap == null)
        {
            Debug.LogError("IKSnap reference not set in IKIndicators script.");
            return;
        }

        CreateIndicators();
    }

    void Update()
    {
        UpdateIndicators();
    }

    void CreateIndicators()
    {
        CreateIndicator(ikSnap.leftHandPos);
        CreateIndicator(ikSnap.rightHandPos);
        CreateIndicator(ikSnap.leftFootPos);
        CreateIndicator(ikSnap.rightFootPos);
    }

    void CreateIndicator(Vector3 position)
    {
        GameObject indicator = Instantiate(indicatorPrefab, position, Quaternion.identity);
        indicators.Add(indicator);
    }

    void UpdateIndicators()
    {
        UpdateIndicator(indicators[0], ikSnap.leftHandPos);
        UpdateIndicator(indicators[1], ikSnap.rightHandPos);
        UpdateIndicator(indicators[2], ikSnap.leftFootPos);
        UpdateIndicator(indicators[3], ikSnap.rightFootPos);
    }

    void UpdateIndicator(GameObject indicator, Vector3 position)
    {
        if (indicator != null)
        {
            indicator.transform.position = position;
        }
    }
}
