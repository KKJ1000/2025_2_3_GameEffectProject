using System.Collections.Generic;
using UnityEngine;

public class LineRendererController : MonoBehaviour
{
    [SerializeField] List<LineRenderer> _lineRenderers = new List<LineRenderer>();

    public void SetPosition(Transform startPps, Transform endPos)
    {
        if (_lineRenderers.Count > 0)
        {
            for (int i = 0; i < _lineRenderers.Count; i++)
            {
                if (_lineRenderers[i].positionCount >= 2)
                {
                    _lineRenderers[i].SetPosition(0, startPps.position);
                    _lineRenderers[i].SetPosition(1, endPos.position);
                }
            }
        }
    }
}
