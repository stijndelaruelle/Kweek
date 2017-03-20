using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingTip : MonoBehaviour
{
    [SerializeField]
    private Text m_Text;

    [SerializeField]
    private List<string> m_Tips;

    [SerializeField]
    private float m_TimeTillNextTip;
    private float m_Timer = 0.0f;

    private void Start()
    {
        UpdateTip();
    }

    private void Update()
    {
        m_Timer += Time.deltaTime;

        if (m_Timer >= m_TimeTillNextTip)
        {
            m_Timer -= m_TimeTillNextTip;
            UpdateTip();
        }
    }

    private void UpdateTip()
    {
        if (m_Tips == null)
            return;

        if (m_Tips.Count <= 0)
            return;

        //Set new random tip
        int rand = Random.Range(0, m_Tips.Count);
        m_Text.text = "Tip: " + m_Tips[rand];

        //Remove it from the tips list, so we can't see it again this loading screen
        m_Tips.RemoveAt(rand);
    }
}
