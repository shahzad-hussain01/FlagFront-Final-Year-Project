using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lovatto.FloatingTextAsset {
    public class bl_FloatingTextTest : MonoBehaviour
    {

        public Camera testCamera;
        public bl_FloatingText[] textPrefabs;

        private float lastRay = 0;

        private void Update()
        {
            MouseTest();
        }

        void MouseTest()
        {
            if (Input.GetMouseButton(0))
            {
                if (Time.time - lastRay < 0.125f) return;
                lastRay = Time.time;

                Ray ray = testCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 100))
                {
                    var target = hit.transform.GetComponent<bl_DemoTarget>();
                    if (target != null) target.OnHit(hit);
                }
            }
        }

        public void BasicText(RaycastHit ray)
        {
            bl_FloatingTextManager.Instance.ChangeTextPrefab(textPrefabs[0]);

            new FloatingText($"{Random.Range(10, 150)}")
              .SetSettings("apex")
              .SetPositionOffset(Vector3.right)
              .SetTarget(ray.transform)
              .SetPosition(ray.point)
              .StickAtOriginWorldPosition()
              .SetOutlineColor(new Color(1,0,0,0.7f))
              .SetOutlineSize(2.5f)
              .DontRewindOnReuse()
              .SetReuses(3)
              .Show();
        }

        public void FortniteText(RaycastHit ray)
        {
            bl_FloatingTextManager.Instance.ChangeTextPrefab(textPrefabs[1]);
            new FloatingText($"{Random.Range(10, 150)}")
              .SetSettings("fortnite")
              .SetTarget(ray.transform)
              .SetPosition(ray.point)
              .StickAtOriginWorldPosition()
              .SetOutlineColor(new Color(0, 0, 0, 0.7f))
              .SetOutlineSize(3)
              .Show();
        }

        public void TomClansysTheDivision(RaycastHit ray)
        {
            bl_FloatingTextManager.Instance.ChangeTextPrefab(textPrefabs[2]);
            new FloatingText($"{Random.Range(10, 90)}")
              .SetSettings("division")
              .SetTarget(ray.transform)
              .SetPosition(ray.point)
              .StickAtOriginWorldPosition()
              .SetOutlineColor(new Color(0, 0, 0, 1f))
              .SetOutlineSize(1)
              .Show();
        }

        public void LeagueOfLegends(RaycastHit ray)
        {
            bl_FloatingTextManager.Instance.ChangeTextPrefab(textPrefabs[3]);
            new FloatingText($"{Random.Range(10, 90)}")
              .SetSettings("lol")
              .SetTarget(ray.transform)
              .SetPosition(ray.point)
              .StickAtOriginWorldPosition()
              .SetTextColor(new Color(0.1462264f, 0.8359416f, 1f, 1))
              .SetExtraTextSize(Random.Range(-10,10))//this should be based on the amount of damage
              .Show();
        }

        public void CandyCrush(RaycastHit ray)
        {
            bl_FloatingTextManager.Instance.ChangeTextPrefab(textPrefabs[4]);
            new FloatingText($"{Random.Range(10, 90)}")
              .SetSettings("candy")
              .SetTarget(ray.transform)
              .SetPosition(ray.point)
              .SetTextColor(new Color(0.3050465f, 1, 0.3050465f, 1))
              .SetOutlineColor(new Color(0.0993236f, 0.2264151f, 0.1272217f, 1))
              .Show();
        }

        string[] sampleComments = new string[] { "Awesome Comment!", "Inside cornflakes", "You never hear about wind farmers...", "No luck, all skills", "Blow me!" , "Game Over."
        , "Everything went silky smooth", "You can start creating faster", "Side hustle came to an end.", "No contribution is ever too small.", "Share with the world", "Pupusas"};
        private int currentComment = 0;
        public void CustomText1(RaycastHit ray)
        {
            bl_FloatingTextManager.Instance.ChangeTextPrefab(textPrefabs[2]);
            currentComment = (currentComment + 1) % sampleComments.Length;
            new FloatingText(sampleComments[currentComment])
              .SetSettings("comment")
              .SetTarget(ray.transform)
              .SetPositionOffset(Vector3.up * 1.5f)
              .SetOutlineColor(Color.black)
              .SetOutlineSize(1)
              .StickAtOriginWorldPosition()
              .ReuseWhileAlive()
              .Show();
        }

        public void RandomText(RaycastHit ray)
        {
            bl_FloatingTextManager.Instance.ChangeTextPrefab(textPrefabs[1]);
            new FloatingText($"{Random.Range(10, 99)}")
              .SetSettings("random")
              .SetTarget(ray.transform)
              .SetPosition(ray.point)
              .SetTextColor(new Color(Random.value, Random.value, Random.value,1))
              .SetExtraTextSize(Random.Range(-15,15))
              .SetOutlineColor(Color.black)
              .SetOutlineSize(1)
              .StickAtOriginWorldPosition()
              .Show();
        }

        public void SlideText(RaycastHit ray)
        {
            bl_FloatingTextManager.Instance.ChangeTextPrefab(textPrefabs[5]);
            currentComment = (currentComment + 1) % sampleComments.Length;
            new FloatingText(sampleComments[currentComment])
              .SetSettings("slide-left")
              .SetTarget(ray.transform)
              .SetPositionOffset(Vector3.up * 1.5f)
              .ReuseWhileAlive()
              .StickAtOriginWorldPosition()
              .Show();
        }

        public void ShakeText(RaycastHit ray)
        {
            bl_FloatingTextManager.Instance.ChangeTextPrefab(textPrefabs[3]);
            new FloatingText($"{Random.Range(10, 99)}")
              .SetSettings("shake")
              .SetTarget(ray.transform)
              .SetPosition(ray.point)
              .SetTextColor(Color.black)
              .SetOutlineSize(0)
              .StickAtOriginWorldPosition()
              .Show();
        }

        public void DropText(RaycastHit ray)
        {
            bl_FloatingTextManager.Instance.ChangeTextPrefab(textPrefabs[3]);
            new FloatingText($"{Random.Range(10, 99)}")
              .SetSettings("drop")
              .SetTarget(ray.transform)
              .SetPosition(ray.point)
              .SetOutlineSize(1)              
              .InvertHorizontalDirectionRandomly()
              .StickAtOriginWorldPosition()
              .Show();
        }
    }
}