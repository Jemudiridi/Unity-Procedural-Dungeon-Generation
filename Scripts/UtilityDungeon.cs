using System.Collections;
using UnityEngine;

namespace Utility.Diridi
{
    public static class UtilityDungeon
    {
        public static RaycastHit RaycastCamera(float rayLenght, LayerMask layerMask)
        {
            Vector2 screenCenterPoint = new Vector2(Screen.width / 2, Screen.height / 2);
            Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
            Physics.Raycast(ray, out RaycastHit raycastHit, rayLenght, layerMask);
            return raycastHit;
        }

        public static Vector3 RandomVector3(float Xmin, float Ymin, float Zmin, float Xmax, float Ymax, float Zmax)
        {
            float x = Random.Range(Xmin, Xmax);
            float y = Random.Range(Ymin, Ymax);
            float z = Random.Range(Zmin, Zmax);

            return new Vector3(x, y, z);
        }

        public static Quaternion RandomQuaternion(float Xmin, float Ymin, float Zmin, float Xmax, float Ymax, float Zmax)
        {
            float x = Random.Range(Xmin, Xmax);
            float y = Random.Range(Ymin, Ymax);
            float z = Random.Range(Zmin, Zmax);

            return Quaternion.Euler(x, y, z);
        }


        public static void SetObjectAndAllChildsLayer(GameObject gameObject, string layerName)
        {
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                Transform child = gameObject.transform.GetChild(i);
                child.gameObject.layer = LayerMask.NameToLayer(layerName);
            }

            gameObject.layer = LayerMask.NameToLayer(layerName);
        }

        public static IEnumerator Frames(int frameCount)
        {
            while (frameCount > 0)
            {
                frameCount--;
                yield return null;
            }
        }

        public static Bounds GetBounds(GameObject obj)

        {

            Bounds bounds = new Bounds();

            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();

            if (renderers.Length > 0)

            {

                //Find first enabled renderer to start encapsulate from it

                foreach (Renderer renderer in renderers)

                {

                    if (renderer.enabled)

                    {

                        bounds = renderer.bounds;

                        break;

                    }

                }

                //Encapsulate for all renderers

                foreach (Renderer renderer in renderers)

                {

                    if (renderer.enabled)

                    {

                        bounds.Encapsulate(renderer.bounds);

                    }

                }

            }

            return bounds;

        }
    }

    
}



