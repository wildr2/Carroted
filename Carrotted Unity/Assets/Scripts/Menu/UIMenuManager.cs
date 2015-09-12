using UnityEngine;
using System.Collections;

public class UIMenuManager : MonoBehaviour
{
    private static UIMenuManager _instance;
    public Transform[] page_layers;
	

    private void Awake()
    {
        _instance = this;
    }

    public static bool IsTopPage(UIMenuPage page)
    {
        if (_instance == null) return false;

        for (int i = _instance.page_layers.Length - 1; i >= 0; --i)
        {
            for (int j = _instance.page_layers[i].childCount-1; j >= 0; --j)
            {
                Transform top_page = _instance.page_layers[i].GetChild(j);
                if (top_page.gameObject.activeInHierarchy)
                {
                    return page.transform == top_page;
                }
            }
        }
        return false;
    }
}
