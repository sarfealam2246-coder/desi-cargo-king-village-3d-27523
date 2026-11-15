using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.IO;

public class AutoBuildScene
{
    static void ColorObj(GameObject go, Color c)
    {
        var r = go.GetComponent<Renderer>();
        if (r != null) { var m = new Material(Shader.Find("Standard")); m.color = c; r.sharedMaterial = m; }
    }

    public static void PerformAndroidBuild()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        RenderSettings.fog = true;
        RenderSettings.fogColor = new Color(0.55f, 0.78f, 0.98f);
        RenderSettings.fogDensity = 0.012f;

        var grass = GameObject.CreatePrimitive(PrimitiveType.Plane);
        grass.transform.localScale = new Vector3(30f, 1f, 60f);
        ColorObj(grass, new Color(0.22f, 0.48f, 0.16f));

        var road = GameObject.CreatePrimitive(PrimitiveType.Plane);
        road.transform.position = new Vector3(0f, 0.02f, 0f);
        road.transform.localScale = new Vector3(1.3f, 1f, 60f);
        ColorObj(road, new Color(0.18f, 0.21f, 0.26f));

        for (int z = -200; z <= 200; z += 18)
        {
            var dash = GameObject.CreatePrimitive(PrimitiveType.Cube);
            dash.transform.position = new Vector3(0f, 0.05f, z);
            dash.transform.localScale = new Vector3(0.35f, 0.05f, 4.5f);
            ColorObj(dash, new Color(0.98f, 0.8f, 0.15f));

            foreach (int side in new[] { -1, 1 })
            {
                var tree = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                tree.transform.position = new Vector3(side * 10.5f, 2.5f, z);
                tree.transform.localScale = new Vector3(1.6f, 2.5f, 1.6f);
                ColorObj(tree, new Color(0.1f, 0.55f, 0.2f));
            }
        }

        var truck = new GameObject("BharatTruck");
        truck.transform.position = new Vector3(0f, 0.8f, 0f);
        var bed = GameObject.CreatePrimitive(PrimitiveType.Cube);
        bed.transform.SetParent(truck.transform);
        bed.transform.localPosition = new Vector3(0f, 0.7f, -0.8f);
        bed.transform.localScale = new Vector3(2.2f, 1.3f, 3.4f);
        ColorObj(bed, new Color(0.92f, 0.35f, 0.08f));

        var cab = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cab.transform.SetParent(truck.transform);
        cab.transform.localPosition = new Vector3(0f, 0.85f, 1.6f);
        cab.transform.localScale = new Vector3(2.1f, 1.5f, 1.6f);
        ColorObj(cab, new Color(0.85f, 0.14f, 0.16f));

        truck.AddComponent<BharatVehicleController>();
        if (Camera.main != null)
        {
            Camera.main.backgroundColor = new Color(0.38f, 0.68f, 0.98f);
            Camera.main.transform.SetParent(truck.transform);
            Camera.main.transform.localPosition = new Vector3(0f, 3.6f, -8.5f);
            Camera.main.transform.LookAt(truck.transform);
        }
        Directory.CreateDirectory("Assets/Scenes");
        EditorSceneManager.SaveScene(scene, "Assets/Scenes/Main.unity");
        PlayerSettings.applicationIdentifier = "com.sarfealam.desi_cargo_king__village_3d";
        PlayerSettings.productName = "Desi Cargo King: Village 3D";
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
        Directory.CreateDirectory("build/Android");
        BuildPipeline.BuildPlayer(new[] { "Assets/Scenes/Main.unity" }, "build/Android/game-release.apk", BuildTarget.Android, BuildOptions.None);
    }
}
