using System.Collections;
using UnityEngine.Events;
using UnityEngine;


namespace PhyGames
{
    public class Target : MonoBehaviour
    {
        public float m_ScaleSpeed;
        public Vector3 m_InitialScale;
        public Vector3 m_TargetScale;

        public UnityEvent DestroyEvent { get; private set; }


        IEnumerator ScaleUp()
        {
            float percent = 0;

            while (percent <= 1)
            {
                percent += Time.deltaTime * m_ScaleSpeed;
                transform.localScale = Vector3.Lerp(m_InitialScale, m_TargetScale, percent);

                yield return null;
            }
        }


        private void Awake()
        {
            transform.localScale = m_InitialScale;
        }

        private void Start()
        {
            if (DestroyEvent == null)
            {
                DestroyEvent = new UnityEvent();
            }

            StartCoroutine(ScaleUp());
        }


        public void Destroy()
        {
            DestroyEvent.Invoke();
            GameObject.Destroy(gameObject);
        }
    }
}
