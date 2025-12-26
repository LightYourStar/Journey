using UnityEngine;
using UnityEngine.UI;

namespace JO
{
    public class AutoRebuildUI:MonoBehaviour
    {
        [SerializeField] private bool InChildren;

        private void Start()
        {
            if (!InChildren)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
            }
            else
            {
                LayoutGroup[] layouts = transform.GetComponentsInChildren<LayoutGroup>();
                foreach (LayoutGroup layout in layouts)
                {
                    LayoutRebuilder.ForceRebuildLayoutImmediate(layout.GetComponent<RectTransform>());
                }
            }
        }
    }
}