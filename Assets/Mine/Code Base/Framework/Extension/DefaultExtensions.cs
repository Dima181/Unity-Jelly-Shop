using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Mine.CodeBase.Framework.Extension
{
    /// <summary>
    /// Defining extension methods
    /// </summary>
    public static class DefaultExtensions
    {
        #region For Component

        public static T GetOrAddComponent<T>(this Component component) 
            where T : Component
        {
            if(!component.TryGetComponent(out T result))
                result = component.AddComponent<T>();

            return result;
        }

        #endregion


        #region For Vector3

        // Random position between two given Vector2s
        public static Vector2 RandomPosition(this Vector2 position, Vector2 otherPosition)
        {
            var x = Random.Range(position.x, otherPosition.x);
            var y = Random.Range(position.y, otherPosition.y);
            return new Vector2(x, y);
        }

        #endregion


        #region For Vector3

        public static Vector3 DropZ(this Vector3 position) => 
            new Vector3(position.x, position.y, 0);

        public static bool IsInRange(this Vector3 position, Vector3 min, Vector3 max) =>
               position.x >= min.x && position.x <= max.x
            && position.y >= min.y && position.y <= max.y
            && position.z >= min.z && position.z <= max.z;

        #endregion

    }
} 
