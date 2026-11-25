using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Steel.Presentation.Characters;
using Steel.Presentation.Vehicles;
using Steel.Services;

namespace Steel.Editor.Showroom
{
    /// <summary>Review sheet of the rider on the triseda at the dealer lot, in the city's own lighting.</summary>
    public static class RideShowroom
    {
        [MenuItem("Steel/Showroom/Triseda di kota")]
        public static void RidingTriseda()
        {
            EditorSceneManager.OpenScene("Assets/_Project/Scenes/City.unity");
            RiderSeat seat = Object.FindObjectsOfType<RiderSeat>()
                .First(s => s.Driver.VehicleId == FleetCatalog.Triseda);
            Transform visual = seat.Rider, bike = seat.transform;
            visual.SetParent(seat.Mount, false);
            visual.localPosition = Vector3.zero; visual.localRotation = Quaternion.identity;
            visual.GetComponent<Steel.Presentation.Characters.Toon.ToonGait>().Joints.Rest();
            var pose = visual.GetComponent<SeatedPose>();
            pose.Sit(seat.Hips, seat.FootLeft, seat.FootRight);
            pose.Apply();
            var arms = visual.GetComponent<ArmReach>();
            arms.Hold(seat.GripLeft, seat.GripRight);
            arms.Snap();
            Debug.Log("RIDE_REACH hands " + Vector3.Distance(arms.Left.Tip.position, seat.GripLeft.position).ToString("F3") +
                " / " + Vector3.Distance(arms.Right.Tip.position, seat.GripRight.position).ToString("F3") +
                " feet " + Vector3.Distance(pose.LeftLeg.Tip.position, seat.FootLeft.position).ToString("F3"));
            int w = ShowroomStage.Width, h = ShowroomStage.Height;
            var camera = new ShowroomCamera(w, h);
            Vector3 focus = bike.position + Vector3.up * .95f;
            float yaw = bike.eulerAngles.y;
            camera.Shoot(focus, yaw + 40, 10, 8, new RectInt(0, 0, 1, 1));   // first frame warms up shaders
            camera.Shoot(focus, yaw + 40, 10, 8, new RectInt(0, 0, w / 2, h));
            camera.Shoot(focus, yaw + 90, 6, 6.5f, new RectInt(w / 2, h / 2, w / 2, h / 2));
            camera.Shoot(focus + bike.forward * .4f + Vector3.up * .25f, yaw + 320, 18, 3.2f,
                new RectInt(w / 2, 0, w / 2, h / 2));
            camera.Save(ShowroomStage.SheetPath("triseda-di-kota"));
        }
    }
}
