using System.Collections.Generic;
using UnityEngine;

namespace JO
{
    public class UIUtil
    {
        public static BasicTypewriter ShowWriterTxt(GameObject obj,string txt)
        {

            BasicTypewriter typewriter = obj.GetComponent<BasicTypewriter>();
            if(typewriter == null)
            {
                typewriter = obj.AddComponent<BasicTypewriter>();
            }
            typewriter.SetText(txt);
            return typewriter;
        }
    }
}