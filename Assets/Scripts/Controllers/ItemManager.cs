using KartDemo.Item;
using KartDemo.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace KartDemo
{
    public class ItemManager : MonoBehaviour
    {
        const int MAX_ITEM = 5;
        public float spinRadius;
        public float spinSpeed;
        public float rotateSpeed;
        public Vector3 offset;

        public int ItemCount => m_ThowableItems.Count;

        //Collects and throws items
        List<ThrowableItem> m_ThowableItems = new List<ThrowableItem>();

        ThrowableItem this[int i]
        {
            get => m_ThowableItems[i];
        }

        Rigidbody rb;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            for (int i = 0; i < m_ThowableItems.Count; i++)
            {
                Transform objTrans = this[i].transform;
                float spinAngle = this[i].spinAngle = Mathf.Repeat(this[i].spinAngle += Time.deltaTime * spinSpeed, 360);
                float radAngle = spinAngle * Mathf.Deg2Rad;
                objTrans.position = Position.Offset(transform, new Vector3(Mathf.Cos(radAngle), 0, Mathf.Sin(radAngle)) * spinRadius + offset);

                objTrans.rotation = transform.rotation;
                float rotAngle = this[i].rotateAngle = Mathf.Repeat(this[i].rotateAngle + rotateSpeed * Time.deltaTime, 360);
                objTrans.Rotate(Vector3.up, rotAngle, Space.Self);
            }
        }

        public bool AddItem(ThrowableItem item)
        {
            if (ItemCount >= MAX_ITEM)
            {
                return false;
            }

            float angularSpacing = 360F / (ItemCount + 1);

            for (int i = 1; i < ItemCount; i++)
            {
                var _item = this[i];
                _item.spinAngle = this[i - 1].spinAngle + angularSpacing;
            }
            if (ItemCount != 0)
                item.spinAngle = this[ItemCount - 1].spinAngle + angularSpacing;
            m_ThowableItems.Add(item);
            return true;
        }

        public void ThrowItem()
        {
            if (ItemCount == 0)
                return;

            float minAngle = this[0].spinAngle;
            int minIndex = 0;

            for (int i = 1; i < ItemCount; i++)
            {
                float angle = this[i].spinAngle;
                if (angle <= 180)
                {
                    if (angle > minAngle) continue;
                }
                else//angle > 180
                {
                    if (360 - angle > minAngle) continue;
                }

                minAngle = angle;
                minIndex = i;
            }

            ThrowableItem item = this[minIndex];
            item.Throw(gameObject, transform.InverseTransformDirection(rb.velocity).z);
            m_ThowableItems.RemoveAtSwapBack(minIndex);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            UnityEditor.Handles.color = Color.white;
            UnityEditor.Handles.DrawWireArc(Position.Offset(transform, offset), transform.up, Vector3.forward, 360, spinRadius);
        }
#endif
    }
}
