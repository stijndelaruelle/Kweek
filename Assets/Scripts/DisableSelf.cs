using UnityEngine;
using System.Collections;

public class DisableSelf : MonoBehaviour
{
    //Simple script that disables the gameobject it's attached to after x time
    //Created for the test laser (will probably become obsolete soon)

    [SerializeField]
    private float m_ActiveTime = 1.0f;
    private Coroutine m_DisableRoutine = null;

    private void OnEnable()
    {
        if (m_DisableRoutine != null)
            StopCoroutine(m_DisableRoutine);

        m_DisableRoutine = StartCoroutine(DisableRoutine());
    }

    private IEnumerator DisableRoutine()
    {
        yield return new WaitForSeconds(m_ActiveTime);
        gameObject.SetActive(false);
    }
}
