using UnityEngine;
using UnityEngine.Events;


namespace PhyGames
{
    [RequireComponent(typeof(TargetSpawner))]
    public class WorldController : MonoBehaviour
    {
        public UnityEvent TargetHit { get; private set; }

        public LayerMask m_TargetLayer;
        private TargetSpawner m_TargetSpawner;
        private Camera m_GameCamera;

        private float m_TargetHitCount;
        private float m_TargetHitChainCount;


        private void Awake()
        {
            m_GameCamera = Camera.main;
            m_TargetSpawner = GetComponent<TargetSpawner>();
        }

        private void Start()
        {
            if (TargetHit == null)
            {
                TargetHit = new UnityEvent();
            }

            m_TargetSpawner.SpawnRandomTarget();

            m_TargetHitCount = m_TargetHitChainCount = 0;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Hit(Input.mousePosition);
            } 
        }


        private Target CheckHitTarget(Vector3 originPosition)
        {
            Ray ray = m_GameCamera.ScreenPointToRay(originPosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, m_GameCamera.farClipPlane, m_TargetLayer))
            {
                Target target = hit.transform.gameObject.GetComponent<Target>();
                if (target)
                {
                    m_TargetHitCount++;
                    m_TargetHitChainCount++;
                    return target;
                }
            }
            else
            {
                m_TargetHitChainCount = 0;
            }

            return null;
        }

        private void Hit(Vector3 position) {
            Target targetHit = CheckHitTarget(position);
            if (targetHit)
            {
                targetHit.Destroy();
                m_TargetSpawner.SpawnRandomTarget();
            }
        }


        // ////////////////////////////////////////////////////////////////////////
        // Debugging UI
        // ////////////////////////////////////////////////////////////////////////

        void OnGUI()
        {
            GUI.skin.label.fontSize = (int)(Screen.width * 0.05f);
            GUI.skin.label.normal.textColor = Color.white;

            GUI.Label(new Rect(20f, 20f, 600f, 100f), "count = " + m_TargetHitCount.ToString());
            GUI.Label(new Rect(20f, 100f, 600f, 100f), "chain = " + m_TargetHitChainCount.ToString());
        }
    }
}
