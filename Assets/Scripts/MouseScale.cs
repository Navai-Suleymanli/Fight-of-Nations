using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseScale : MonoBehaviour
{
    public void PointerEnter()
    {
        gameObject.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        gameObject.transform.SetAsLastSibling();
    }

    public void PointerExit()
    {
        gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
        gameObject.transform.SetSiblingIndex(transform.GetSiblingIndex() - 1);

    }
}
