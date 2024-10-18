using UnityEngine;

public partial class HealthPoints : MonoBehaviour
{
    public struct DamageData
    {
        public MonoBehaviour damager;
        public int wpnAtk;
        public int wpnStun;
        public int arrowAtk;
        public int arrowStun;
        public int shdAtk;
        public int shdStun;
        public float isCritical;
        public float playerDefense;
        public Vector3 damageSource;
    }
}
