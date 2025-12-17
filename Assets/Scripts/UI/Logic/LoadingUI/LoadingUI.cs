using System;
using UnityEngine;

namespace UIManager
{
    public partial class LoadingUI
    {
        private float m_ProgressVal;
        private IProgress<float> m_Progress;
        private bool m_isOpen;
        /// <summary>
        /// 对外暴露：获取一个可直接传给 SwitchToAsync 的进度桥
        /// </summary>
        public IProgress<float> CreateProgress()
        {
            return new Progress<float>(p =>
            {
                if (!m_isOpen) return;
                SetProgress(p);
            });
        }

        public void SetProgress(float v)
        {
            m_ProgressVal = Mathf.Clamp01(v);
            if (m_Slider_Slider) m_Slider_Slider.value = m_ProgressVal;
            if (m_Progress_Text) m_Progress_Text.text = $"{Mathf.RoundToInt(m_ProgressVal * 100f)}%";
        }

        public override void OnInit()
        {
            m_ProgressVal = 0f;
            if (m_Slider_Slider)
            {
                m_Slider_Slider.minValue = 0f;
                m_Slider_Slider.maxValue = 1f;
                m_Slider_Slider.value = 0f;
            }
            if (m_Progress_Text)
                m_Progress_Text.text = "0%";
        }

        public override void OnOpen(object param1 = null, object param2 = null, object param3 = null)
        {
            m_isOpen = true;
            SetProgress(0f);
        }

        public override void OnClose()
        {
            m_isOpen = false;
        }

        public override void OnUpdate(float dt)
        {
            if (m_Slider_Slider) m_Slider_Slider.value = m_ProgressVal;
            if (m_Progress_Text) m_Progress_Text.text = $"{Mathf.RoundToInt(m_ProgressVal * 100f)}%";
        }
    }
}