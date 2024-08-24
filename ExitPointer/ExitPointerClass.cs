using System;
using UnityEngine;

namespace ExitPointer
{
    internal class ExitPointerClass
    {
        // Initialize variables
        private readonly GameObject _pointer;
        private RectTransform _rectTransform;

        // Initialize function
        internal ExitPointerClass(GameObject pointer)
        {
            this._pointer = pointer ?? throw new ArgumentNullException(nameof(pointer));
            this._rectTransform = _pointer.GetComponent<RectTransform>();
        }
        
        // Set default status
        internal bool SetDefaultStatus(Transform parent, Vector3 scale)
        {
            try
            {
                _pointer.transform.SetParent(parent.transform, false);
                _pointer.transform.localScale = scale;
                return true;
            }
            
            catch (Exception e)
            {
                return false;
            }
        }
        
        // Set active
        internal void SetActive(bool active)
        {
            _pointer.SetActive(active);
        }
        
        // Move Pointer
        internal void MovePointer(Camera camera, Transform targetTransform)
        {
            _rectTransform.anchoredPosition = CalculatePos(camera, targetTransform,
                new Vector2(860, 520), new Vector2(860 / 2 , 520 / 2), 
                new Vector2(860 / 2 , 520 / 2 - 50));
        }
        
        // Calculate position of pointer
        private Vector2 CalculatePos(Camera Cam, Transform TargetTransform, Vector2 ScreenSize, Vector2 ScreenCenter, Vector2 EllipticalRadius)
        {
            Vector2 screenPos = (Vector2)Cam.WorldToScreenPoint(TargetTransform.position);
            
            Vector3 dir = Cam.transform.forward;
            Vector3 dir2 = TargetTransform.transform.position - Cam.transform.position;
            bool IsOutScreen = false;
    
            if (Vector3.Dot(dir, dir2) <= 0)//判断摄像机是否背对目标
            {
                screenPos.x = ScreenSize.x - screenPos.x;
                screenPos.y = ScreenSize.y - screenPos.y;
                IsOutScreen = true;
            }
            
            if(screenPos.x < 0 || screenPos.x > ScreenSize.x || screenPos.y < 0 || screenPos.y > ScreenSize.y)
            {
                IsOutScreen= true;
               
            }
            screenPos -= ScreenCenter;
    
            if (IsOutScreen|| Mathf.Pow(screenPos.x / EllipticalRadius.x, 2) + Mathf.Pow(screenPos.y / EllipticalRadius.y, 2) > 1)//如果超出屏幕或者超出椭圆的范围，将位置约束在椭圆上
            {
                
                float x = Mathf.Sqrt(Mathf.Pow(EllipticalRadius.x * EllipticalRadius.y * screenPos.x, 2) / (Mathf.Pow(screenPos.x * EllipticalRadius.y, 2) + Mathf.Pow(EllipticalRadius.x * screenPos.y, 2)));
    
                if (Mathf.Sign(x) != Mathf.Sign(screenPos.x))
                {
                    x *= -1;
                }
                float y = x * (screenPos.y / screenPos.x);
    
                screenPos.x = x;
                screenPos.y = y;
            }
            
            return screenPos;
        }
    }
}