using UnityEngine;

namespace SD.PlayerLogic
{
    class PlayerVehicleDamageReceiver : MonoBehaviour, IDamageable
    {
        PlayerVehicle vehicle;

        #region 'idamageable' through 'vehicle'
        public float Health => ((IDamageable)vehicle).Health;

        public void ReceiveDamage(Damage damage)
        {
            ((IDamageable)vehicle).ReceiveDamage(damage);
        }
        #endregion

        public void Init(PlayerVehicle vehicle)
        {
            this.vehicle = vehicle;
        }

        //public void OnCollisionEnter(Collision col)
        //{
        //    IVehicle otherVehicle = col.gameObject.GetComponent<IVehicle>();

        //    if (otherVehicle != null)
        //    {
        //        vehicle.Collide(otherVehicle);
        //    }
        //}
    }
}
