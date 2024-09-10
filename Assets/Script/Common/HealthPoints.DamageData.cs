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
        public Vector3 damageSource;
    }
}
