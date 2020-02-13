using UnityEngine;


namespace PhyGames
{
    public class TargetSpawner : MonoBehaviour
    {
        public Bounds m_Offset;
        public GameObject m_TargetPrefab;


        public void SpawnRandomTarget()
        {
            Instantiate(m_TargetPrefab, GetRandomPointInBounds(), Quaternion.Euler(-90f, 0f, 0f));
        }

        private Vector3 GetRandomPointInBounds()
        {
            return new Vector3(
                Random.Range(m_Offset.min.x, m_Offset.max.x),
                Random.Range(m_Offset.min.y, m_Offset.max.y),
                Random.Range(m_Offset.min.z, m_Offset.max.z)
            );
        }


        // ////////////////////////////////////////////////////////////////////////
        // Gizmos
        // ////////////////////////////////////////////////////////////////////////

        private void OnDrawGizmos()
        {
            DrawBoundsPoints();
        }

        private void DrawBoundsPoints()
        {
            Gizmos.color = Color.red;
            
            Gizmos.DrawSphere(m_Offset.min, 0.1f);
            Gizmos.DrawSphere(new Vector3(m_Offset.min.x, m_Offset.min.y + (m_Offset.extents.y * 2), m_Offset.min.z), 0.1f);
            Gizmos.DrawSphere(m_Offset.max, 0.1f);
            Gizmos.DrawSphere(new Vector3(m_Offset.max.x, m_Offset.max.y - (m_Offset.extents.y * 2), m_Offset.min.z), 0.1f);
        }
    }   
}
