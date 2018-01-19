using System.Collections.Generic;
using UberStrok.Core.Common;

namespace UberStrok.Core.Views
{
    public class DamageEventView
    {
        public Dictionary<byte, byte> Damage { get; set; }
        public byte BodyPartFlag { get; set; }
        public int DamageEffectFlag { get; set; }
        public float DamgeEffectValue { get; set; }

        public int Count => Damage == null ? 0 : Damage.Count;

        public void Clear()
        {
            if (Damage == null)
                Damage = new Dictionary<byte, byte>();

            BodyPartFlag = 0;
            Damage.Clear();
        }

        public void Add(byte angle, short damage, BodyPart bodyPart, DamageEffectType damageEffectFlag, float damageEffectValue)
        {
            if (Damage == null)
                Damage = new Dictionary<byte, byte>();

            if (Damage.ContainsKey(angle))
            {
                var oldDamage = Damage[angle];
                Damage[angle] = (byte)(oldDamage + damage);
            }
            else
            {
                Damage[angle] = (byte)damage;
            }

            BodyPartFlag |= (byte)bodyPart;
            DamageEffectFlag = (int)damageEffectFlag;
            DamgeEffectValue = damageEffectValue;
        }
    }
}
